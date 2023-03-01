#region [ Using ]
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InovoCIM.Data.Entities;
using InovoCIM.Data.Repository;
#endregion

namespace InovoCIM.FileProcess
{
    public class FileMissed
    {
        public ImportFile ImportFileModel { get; set; }
        public FileInfo FileDetail { get; set; }
        public LogRepository LogRepo { get; set; }
        public SQLRepository SqlRepo { get; set; }

        public string InstanceID { get; set; }
        public string Class { get; set; }
        public string ScriptTable { get; set; }

        public int StatsLineTotal { get; set; }
        public int StatsFileDuplicate { get; set; }
        public int StatsPresenceDuplicate { get; set; }
        public int StatsScriptDuplicate { get; set; }

        public int IndexSourceID { get; set; }
        public int IndexLoadIndicator { get; set; }
        public int IndexPhone01 { get; set; }
        public int IndexPhone02 { get; set; }
        public int IndexPhone03 { get; set; }

        public DataTable Table = new DataTable();
        public DataTable TableScript = new DataTable();
        public DataTable Presence = new DataTable();
        public DataTable QueueComplete = new DataTable();

        public Dictionary<int, string> IndicatorList = new Dictionary<int, string>();

        #region [ Default Constructor ]
        public FileMissed(string _InstanceID, ImportFile _ImportFileModel, FileInfo _File)
        {
            this.LogRepo = new LogRepository();
            this.SqlRepo = new SQLRepository();
            this.ImportFileModel = _ImportFileModel;
            this.FileDetail = _File;
            this.InstanceID = _InstanceID;
            this.Class = "File Missed";
            this.ScriptTable = "[PSCRIPT].[RetentionsNewUpdated_S]";

            this.StatsLineTotal = 0;
            this.StatsFileDuplicate = 0;
            this.StatsPresenceDuplicate = 0;
            this.StatsScriptDuplicate = 0;

            this.IndexSourceID = 199;
            this.IndexLoadIndicator = 2;

            this.IndexPhone01 = 3;
            this.IndexPhone02 = 4;
            this.IndexPhone03 = 5;
        }
        #endregion

        //---------------------------------------------------------------------------//

        #region [ Master ]
        public async Task<bool> Master()
        {
            DateTime StartTime = DateTime.Now;
            var Event = new LogConsoleEvent(this.InstanceID);
            try
            {
                await Event.SaveAsync(this.Class, "Master()", "Start");

                TableRepository TableRepo = new TableRepository();
                Table = TableRepo.ColumnTableMissed(Table);
                TableScript = TableRepo.ColumnTableMissedScript(TableScript);
                Presence = TableRepo.ColumnTablePresence(Presence);
                QueueComplete = TableRepo.ColumnTableQueuePhoneComplete(QueueComplete);

                string ResultHeader = await ValidateHeader();
                await Event.SaveAsync(this.Class, "Master()", "ValidateHeader() - " + ResultHeader);
                if (ResultHeader == "Ok")
                {
                    string ResultRead = await ReadTextFile();
                    await Event.SaveAsync(this.Class, "Master()", "ReadTextFile() - " + ResultRead);

                    string ResultDatabase = await SendToDatabase();
                    await Event.SaveAsync(this.Class, "Master()", "SendToDatabase() - " + ResultDatabase);

                    string ResultClose = await CloseTextFile(StartTime);
                    await Event.SaveAsync(this.Class, "Master()", "CloseTextFile() - " + ResultClose);
                }

                await Event.SaveAsync(this.Class, "Master()", "End");
                var Runtime = new LogConsoleRuntime(this.InstanceID, this.Class, "Master()", StartTime);
                await Runtime.SaveSync();

                return true;
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(this.InstanceID, this.Class, "Master()", ex.Message);
                await log.SaveSync();

                return false;
            }
        }
        #endregion

        #region [ Validate Header ]
        private async Task<string> ValidateHeader()
        {
            var Event = new LogConsoleEvent(this.InstanceID);
            try
            {
                ConfigurationDirectory Directory = new ConfigurationDirectory();
                Directory = await Directory.GetSingleAsync(this.InstanceID);

                string header = File.ReadLines(Path.Combine(Directory.Input, this.FileDetail.Name)).First();
                if (header != ImportFileModel.Header)
                {
                    var EmailRepo = new EmailRepository();
                    await EmailRepo.SendHeaderFailed(this.InstanceID, this.FileDetail.Name, header, ImportFileModel.Header);

                    await Event.SaveAsync(this.Class, "ValidateHeader()", "Header Failed: " + this.FileDetail.Name);
                    return "Fail";
                }

                await Event.SaveAsync(this.Class, "ValidateHeader()", "Header Passed: " + this.FileDetail.Name);
                return "Ok";
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(this.InstanceID, this.Class, "ValidateHeader()", ex.Message);
                await log.SaveSync();

                return "Fail";
            }
        }
        #endregion

        #region [ Read Text File ]
        private async Task<string> ReadTextFile()
        {
            string LeadStatus = "Success";
            Dictionary<int, int> SourceList = new Dictionary<int, int>();
            try
            {
                ConfigurationDirectory Directory = new ConfigurationDirectory();
                Directory = await Directory.GetSingleAsync(this.InstanceID);

                string fileLine;
                using (var fileStream = File.OpenRead(Path.Combine(Directory.Input, this.FileDetail.Name)))
                using (var reader = new StreamReader(fileStream, Encoding.UTF8, true, 1024))
                {
                    while ((fileLine = reader.ReadLine()) != null)
                    {
                        if (fileLine != ImportFileModel.Header)
                        {
                            this.StatsLineTotal++;
                            string[] InData = fileLine.Split(ImportFileModel.Delimiter).ToArray();

                            int ServiceID = (InData[44] == "Processing") ? 1904 : 427;
                            int SourceID = int.Parse(InData[this.IndexSourceID]);
                            int LoadID = await CalculateLoadID(InData, ServiceID);

                            bool IsFileDuplicate = SourceList.ContainsValue(SourceID);
                            SourceList.Add(this.StatsLineTotal, SourceID);

                            string _PhoneStatus = "Valid Numbers";
                            string _IsFileDuplicate = "No", _IsPresenceDuplicate = "No", _IsScriptDuplicate = "No", _MapPresenceFail = "No", _MapScriptFail = "No";

                            if (IsFileDuplicate == false)
                            {
                                bool IsPresenceDuplicate = await SqlRepo.IsPresenceDuplicate(SourceID, ServiceID, LoadID);
                                if (IsPresenceDuplicate == false)
                                {
                                    PhoneModel Phones = new PhoneModel(this.InstanceID, InData[this.IndexPhone01], InData[this.IndexPhone02], InData[this.IndexPhone03]);
                                    Phones = await Phones.GetPhoneNumbers();
                                    _PhoneStatus = await Phones.GetStatus();
                                    if (_PhoneStatus == "Valid Numbers")
                                    {
                                        string QueuePresence = await MapFilePresence(InData, SourceID, ServiceID, LoadID, Phones);
                                        if (QueuePresence == "Fail")
                                        {
                                            LeadStatus = "Map Presence Failed";
                                            _MapPresenceFail = "Yes";
                                        }
                                    }
                                    else
                                    {
                                        LeadStatus = _PhoneStatus;
                                        _MapPresenceFail = "N/A";
                                    }
                                }
                                else
                                {
                                    this.StatsPresenceDuplicate++;
                                    LeadStatus = "Presence Duplicate";
                                    _IsPresenceDuplicate = "Yes";
                                    _MapPresenceFail = "N/A";
                                    _PhoneStatus = "N/A";
                                }

                                bool IsScriptDuplicate = await SqlRepo.IsScriptDuplicate(SourceID, this.ScriptTable);
                                if (IsScriptDuplicate == false)
                                {
                                    string Script = await MapFileScript(InData, this.StatsLineTotal, SourceID, ServiceID, LoadID);
                                    if (Script == "Fail")
                                    {
                                        LeadStatus = "Map Script Failed";
                                        _MapScriptFail = "Yes";
                                    }
                                }
                                else
                                {
                                    this.StatsScriptDuplicate++;
                                    LeadStatus = "Script Duplicate";
                                    _IsScriptDuplicate = "Yes";
                                    _MapScriptFail = "N/A";
                                }
                            }
                            else
                            {
                                this.StatsFileDuplicate++;
                                LeadStatus = "File Duplicate";
                                _IsFileDuplicate = "Yes";
                                _IsPresenceDuplicate = "N/A";
                                _IsScriptDuplicate = "N/A";
                                _MapPresenceFail = "N/A";
                                _MapScriptFail = "N/A";
                                _PhoneStatus = "N/A";
                            }

                            string Log = await MapFileLog(InData, this.StatsLineTotal, ServiceID, LoadID, SourceID, _IsFileDuplicate, _IsPresenceDuplicate, _IsScriptDuplicate, _MapPresenceFail, _MapScriptFail, _PhoneStatus);
                            string QueueComplete = await MapFileQueueComplete(InData, LeadStatus, SourceID, ServiceID, LoadID);
                        }
                    }
                }

                return "Ok";
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(this.InstanceID, this.Class, "ReadTextFile()", ex.Message);
                await log.SaveSync();

                return "Fail";
            }
        }
        #endregion

        #region [ Calculate LoadID ]
        public async Task<int> CalculateLoadID(string[] InData, int ServiceID)
        {
            int LoadID = 0;
            try
            {
                bool InIndicatorList = IndicatorList.ContainsValue(InData[this.IndexLoadIndicator]);
                if (InIndicatorList == true)
                {
                    LoadModel Load = new LoadModel(this.InstanceID, ServiceID, InData[this.IndexLoadIndicator], this.ImportFileModel.Partial, "Priority");
                    LoadID = await Load.GetLoadID();
                }
                else
                {
                    IndicatorList.Add(this.StatsLineTotal, InData[this.IndexLoadIndicator]);
                    LoadModel Load = new LoadModel(this.InstanceID, ServiceID, InData[this.IndexLoadIndicator], this.ImportFileModel.Partial, "Priority");
                    LoadID = await Load.CreateLoadID();
                }
                return LoadID;
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(this.InstanceID, this.Class, "CalculateLoadID()", ex.Message);
                await log.SaveSync();

                return LoadID;
            }
        }
        #endregion

        #region [ Send To Database ]
        private async Task<string> SendToDatabase()
        {
            SqlBulkRepository Sql = new SqlBulkRepository();
            try
            {
                if (Table.Rows.Count > 0)
                {
                    int result = Sql.SendToInovoCIM(Table, "[ECM].[FileTableMissed]");
                }

                if (TableScript.Rows.Count > 0)
                {
                    int result = Sql.SendToPresence(TableScript, "[PSCRIPT].[RetentionsNewUpdated_S]");
                }

                if (Presence.Rows.Count > 0)
                {
                    int result = Sql.SendToPresence(Presence, "[PREP].[PCO_OUTBOUNDQUEUE]");
                }

                if (QueueComplete.Rows.Count > 0)
                {
                    int result = Sql.SendToInovoCIM(QueueComplete, "[ECM].[QueuePhoneComplete]");
                }

                return "Ok";
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(this.InstanceID, this.Class, "SendToDatabase()", ex.Message);
                await log.SaveSync();

                return "Fail";
            }
        }
        #endregion

        #region [ Close Text File ]
        private async Task<string> CloseTextFile(DateTime StartTime)
        {
            try
            {
                ConfigurationDirectory Directory = new ConfigurationDirectory();
                Directory = await Directory.GetSingleAsync(this.InstanceID);

                string source = Path.Combine(Directory.Input, this.FileDetail.Name);
                string destination = Path.Combine(Directory.Complete, this.FileDetail.Name + "_" + StartTime.ToString("dd-MM-yyyy-HH.mm.ss") + ".csv");
                File.Move(source, destination);

                var EmailRepo = new EmailRepository();
                bool IsSent = await EmailRepo.SendReportFile(this.InstanceID, StartTime, this.FileDetail.Name, this.StatsLineTotal, this.StatsFileDuplicate, this.StatsPresenceDuplicate, this.StatsScriptDuplicate);

                return "Ok";
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(this.InstanceID, this.Class, "CloseTextFile()", ex.Message);
                await log.SaveSync();

                return "Fail";
            }
        }
        #endregion

        //---------------------------------------------------------------------------//
        //---------------------------------------------------------------------------//

        #region [ Map File Log ]
        private async Task<string> MapFileLog(string[] InData, int FileLine, int ServiceID, int LoadID, int SourceID, string IsFileDuplicate, string IsPresenceDuplicate, string IsScriptDuplicate, string MapPresenceFail, string MapScriptFail, string PhoneStatus)
        {
            try
            {
                FileLine++;

                DataRow Row = Table.NewRow();
                Row["ID"] = DBNull.Value;
                Row["InstanceID"] = (this.InstanceID.Length <= 49) ? this.InstanceID : this.InstanceID.Substring(0, 49);
                Row["Input"] = (this.FileDetail.Name.Length <= 249) ? this.FileDetail.Name : this.FileDetail.Name.Substring(0, 249);
                Row["Received"] = DateTime.Now;
                Row["FileLine"] = FileLine;
                Row["InputServiceID"] = ServiceID;
                Row["InputLoadID"] = LoadID;
                Row["InputSourceID"] = SourceID;
                Row["IsFileDuplicate"] = IsFileDuplicate;
                Row["IsPresenceDuplicate"] = IsPresenceDuplicate;
                Row["IsScriptDuplicate"] = IsScriptDuplicate;
                Row["MapPresenceFail"] = MapPresenceFail;
                Row["MapScriptFail"] = MapScriptFail;
                Row["PhoneStatus"] = (PhoneStatus.Length <= 29) ? PhoneStatus : PhoneStatus.Substring(0,29);
                Row["AffinityCode"] = (InData[0].Length <= 249) ? InData[0] : InData[0].Substring(0, 249);
                Row["InforceStatus"] = (InData[1].Length <= 249) ? InData[1] : InData[1].Substring(0, 249);
                Row["Priority"] = InData[2];
                Row["MobilePhone"] = InData[3];
                Row["Telephone1"] = InData[4];
                Row["Telephone2"] = InData[5];
                Row["Title"] = InData[6];
                Row["FirstName"] = InData[7];
                Row["LastName"] = InData[8];
                Row["RSAID"] = InData[9];
                Row["ClientSegementCode"] = InData[10];
                Row["PolicyNumberFromDesciption"] = InData[11];
                Row["ProductFromDesciption"] = InData[12];
                Row["ProcessingDateFromDesciption"] = InData[13];
                Row["PremiumAmountFromDesciption"] = InData[14];
                Row["RejectionReasonFromDesciption"] = InData[15];
                Row["FirstDebitOrderIndicatorFromDesciption"] = InData[16];
                Row["Prd1PolicyProductNumber"] = InData[17];
                Row["Prd1TaskGUID"] = InData[18];
                Row["Prd1InforceStatus"] = InData[19];
                Row["Prd1Priority"] = InData[20];
                Row["Prd1BankAccountName"] = InData[21];
                Row["Prd1BankAccountNumber"] = InData[22];
                Row["Prd1BankBranchName"] = InData[23];
                Row["Prd1BankName"] = InData[24];
                Row["Prd1PolicyNumber"] = InData[25];
                Row["Prd1CollectionCount"] = InData[26];
                Row["Prd1DebitOrderDay"] = InData[27];
                Row["Prd1EPN"] = InData[28];
                Row["Prd1ProductName"] = InData[29];
                Row["Prd1TaskNumber"] = InData[30];
                Row["Prd1SkippedMonthAllowed"] = InData[31];
                Row["Prd1InforceMonthCount"] = InData[32];
                Row["Prd1MonthlyPremium"] = InData[33];
                Row["Prd1TotalSumInsured"] = InData[34];
                Row["Prd1InforceMonthCountBand"] = InData[35];
                Row["Prd1NewExisting"] = InData[36];
                Row["Prd1ProductCategory"] = InData[37];
                Row["Prd1TaskSubType"] = InData[38];
                Row["Prd1LastCoverMonthMissedPayments"] = InData[39];
                Row["Prd1CollectionCountOverInforceMonths"] = InData[40];
                Row["Prd1ACCAUpfrontRejection"] = InData[41];
                Row["Prd1ACCAPendingAction"] = InData[42];
                Row["Prd1CalcPriority"] = InData[43];
                Row["Prd1Service"] = InData[44];
                Row["Prd1Profile"] = InData[45];
                Row["Prd1CoverMonth"] = InData[46];
                Row["Prd2PolicyProductNumber"] = InData[47];
                Row["Prd2TaskGUID"] = InData[48];
                Row["Prd2InforceStatus"] = InData[49];
                Row["Prd2Priority"] = InData[50];
                Row["Prd2BankAccountName"] = InData[51];
                Row["Prd2BankAccountNumber"] = InData[52];
                Row["Prd2BankBranchName"] = InData[53];
                Row["Prd2BankName"] = InData[54];
                Row["Prd2PolicyNumber"] = InData[55];
                Row["Prd2CollectionCount"] = InData[56];
                Row["Prd2DebitOrderDay"] = InData[57];
                Row["Prd2EPN"] = InData[58];
                Row["Prd2ProductName"] = InData[59];
                Row["Prd2TaskNumber"] = InData[60];
                Row["Prd2SkippedMonthAllowed"] = InData[61];
                Row["Prd2InforceMonthCount"] = InData[62];
                Row["Prd2MonthlyPremium"] = InData[63];
                Row["Prd2TotalSumInsured"] = InData[64];
                Row["Prd2InforceMonthCountBand"] = InData[65];
                Row["Prd2NewExisting"] = InData[66];
                Row["Prd2ProductCategory"] = InData[67];
                Row["Prd2TaskSubType"] = InData[68];
                Row["Prd2LastCoverMonthMissedPayments"] = InData[69];
                Row["Prd2CollectionCountOverInforceMonths"] = InData[70];
                Row["Prd2ACCAUpfrontRejection"] = InData[71];
                Row["Prd2ACCAPendingAction"] = InData[72];
                Row["Prd2CalcPriority"] = InData[73];
                Row["Prd2Service"] = InData[74];
                Row["Prd2Profile"] = InData[75];
                Row["Prd2CoverMonth"] = InData[76];
                Row["Prd3PolicyProductNumber"] = InData[77];
                Row["Prd3TaskGUID"] = InData[78];
                Row["Prd3InforceStatus"] = InData[79];
                Row["Prd3Priority"] = InData[80];
                Row["Prd3BankAccountName"] = InData[81];
                Row["Prd3BankAccountNumber"] = InData[82];
                Row["Prd3BankBranchName"] = InData[83];
                Row["Prd3BankName"] = InData[84];
                Row["Prd3PolicyNumber"] = InData[85];
                Row["Prd3CollectionCount"] = InData[86];
                Row["Prd3DebitOrderDay"] = InData[87];
                Row["Prd3EPN"] = InData[88];
                Row["Prd3ProductName"] = InData[89];
                Row["Prd3TaskNumber"] = InData[90];
                Row["Prd3SkippedMonthAllowed"] = InData[91];
                Row["Prd3InforceMonthCount"] = InData[92];
                Row["Prd3MonthlyPremium"] = InData[93];
                Row["Prd3TotalSumInsured"] = InData[94];
                Row["Prd3InforceMonthCountBand"] = InData[95];
                Row["Prd3NewExisting"] = InData[96];
                Row["Prd3ProductCategory"] = InData[97];
                Row["Prd3TaskSubType"] = InData[98];
                Row["Prd3LastCoverMonthMissedPayments"] = InData[99];
                Row["Prd3CollectionCountOverInforceMonths"] = InData[100];
                Row["Prd3ACCAUpfrontRejection"] = InData[101];
                Row["Prd3ACCAPendingAction"] = InData[102];
                Row["Prd3CalcPriority"] = InData[103];
                Row["Prd3Service"] = InData[104];
                Row["Prd3Profile"] = InData[105];
                Row["Prd3CoverMonth"] = InData[106];
                Row["Prd4PolicyProductNumber"] = InData[107];
                Row["Prd4TaskGUID"] = InData[108];
                Row["Prd4InforceStatus"] = InData[109];
                Row["Prd4Priority"] = InData[110];
                Row["Prd4BankAccountName"] = InData[111];
                Row["Prd4BankAccountNumber"] = InData[112];
                Row["Prd4BankBranchName"] = InData[113];
                Row["Prd4BankName"] = InData[114];
                Row["Prd4PolicyNumber"] = InData[115];
                Row["Prd4CollectionCount"] = InData[116];
                Row["Prd4DebitOrderDay"] = InData[117];
                Row["Prd4EPN"] = InData[118];
                Row["Prd4ProductName"] = InData[119];
                Row["Prd4TaskNumber"] = InData[120];
                Row["Prd4SkippedMonthAllowed"] = InData[121];
                Row["Prd4InforceMonthCount"] = InData[122];
                Row["Prd4MonthlyPremium"] = InData[123];
                Row["Prd4TotalSumInsured"] = InData[124];
                Row["Prd4InforceMonthCountBand"] = InData[125];
                Row["Prd4NewExisting"] = InData[126];
                Row["Prd4ProductCategory"] = InData[127];
                Row["Prd4TaskSubType"] = InData[128];
                Row["Prd4LastCoverMonthMissedPayments"] = InData[129];
                Row["Prd4CollectionCountOverInforceMonths"] = InData[130];
                Row["Prd4ACCAUpfrontRejection"] = InData[131];
                Row["Prd4ACCAPendingAction"] = InData[132];
                Row["Prd4CalcPriority"] = InData[133];
                Row["Prd4Service"] = InData[134];
                Row["Prd4Profile"] = InData[135];
                Row["Prd4CoverMonth"] = InData[136];
                Row["Prd5PolicyProductNumber"] = InData[137];
                Row["Prd5TaskGUID"] = InData[138];
                Row["Prd5InforceStatus"] = InData[139];
                Row["Prd5Priority"] = InData[140];
                Row["Prd5BankAccountName"] = InData[141];
                Row["Prd5BankAccountNumber"] = InData[142];
                Row["Prd5BankBranchName"] = InData[143];
                Row["Prd5BankName"] = InData[144];
                Row["Prd5PolicyNumber"] = InData[145];
                Row["Prd5CollectionCount"] = InData[146];
                Row["Prd5DebitOrderDay"] = InData[147];
                Row["Prd5EPN"] = InData[148];
                Row["Prd5ProductName"] = InData[149];
                Row["Prd5TaskNumber"] = InData[150];
                Row["Prd5SkippedMonthAllowed"] = InData[151];
                Row["Prd5InforceMonthCount"] = InData[152];
                Row["Prd5MonthlyPremium"] = InData[153];
                Row["Prd5TotalSumInsured"] = InData[154];
                Row["Prd5InforceMonthCountBand"] = InData[155];
                Row["Prd5NewExisting"] = InData[156];
                Row["Prd5ProductCategory"] = InData[157];
                Row["Prd5TaskSubType"] = InData[158];
                Row["Prd5LastCoverMonthMissedPayments"] = InData[159];
                Row["Prd5CollectionCountOverInforceMonths"] = InData[160];
                Row["Prd5ACCAUpfrontRejection"] = InData[161];
                Row["Prd5ACCAPendingAction"] = InData[162];
                Row["Prd5CalcPriority"] = InData[163];
                Row["Prd5Service"] = InData[164];
                Row["Prd5Profile"] = InData[165];
                Row["Prd5CoverMonth"] = InData[166];
                Row["Prd6PolicyProductNumber"] = InData[167];
                Row["Prd6TaskGUID"] = InData[168];
                Row["Prd6InforceStatus"] = InData[169];
                Row["Prd6Priority"] = InData[170];
                Row["Prd6BankAccountName"] = InData[171];
                Row["Prd6BankAccountNumber"] = InData[172];
                Row["Prd6BankBranchName"] = InData[173];
                Row["Prd6BankName"] = InData[174];
                Row["Prd6PolicyNumber"] = InData[175];
                Row["Prd6CollectionCount"] = InData[176];
                Row["Prd6DebitOrderDay"] = InData[177];
                Row["Prd6EPN"] = InData[178];
                Row["Prd6ProductName"] = InData[179];
                Row["Prd6TaskNumber"] = InData[180];
                Row["Prd6SkippedMonthAllowed"] = InData[181];
                Row["Prd6InforceMonthCount"] = InData[182];
                Row["Prd6MonthlyPremium"] = InData[183];
                Row["Prd6TotalSumInsured"] = InData[184];
                Row["Prd6InforceMonthCountBand"] = InData[185];
                Row["Prd6NewExisting"] = InData[186];
                Row["Prd6ProductCategory"] = InData[187];
                Row["Prd6TaskSubType"] = InData[188];
                Row["Prd6LastCoverMonthMissedPayments"] = InData[189];
                Row["Prd6CollectionCountOverInforceMonths"] = InData[190];
                Row["Prd6ACCAUpfrontRejection"] = InData[191];
                Row["Prd6ACCAPendingAction"] = InData[192];
                Row["Prd6CalcPriority"] = InData[193];
                Row["Prd6Service"] = InData[194];
                Row["Prd6Profile"] = InData[195];
                Row["Prd6CoverMonth"] = InData[196];
                Row["TaskGUIDCount"] = InData[197];
                Row["TaskNumberCount"] = InData[198];
                Row["SourceID"] = InData[199];

                Table.Rows.Add(Row);
                return "Ok";
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(this.InstanceID, this.Class, "MapFileLog()", ex.Message);
                await log.SaveSync();

                return "Fail";
            }
        }
        #endregion

        #region [ Map File Queue Complete ]
        private async Task<string> MapFileQueueComplete(string[] InData, string Status, int SourceID, int ServiceID, int LoadID)
        {

            try
            {
                string Name = InData[7] + " " + InData[8];
                Name = Name.Length <= 39 ? Name : Name.Substring(0, 39);
                DataRow Row = QueueComplete.NewRow();
                Row["QueuePhoneCompleteID"] = DBNull.Value;
                Row["Command"] = "AddCall";
                Row["Input"] = "File Import";
                Row["InputName"] = (this.FileDetail.Name.Length <= 119) ? this.FileDetail.Name : this.FileDetail.Name.Substring(0, 119);
                Row["Status"] = Status;
                Row["Received"] = DateTime.Now;
                Row["NextExecute"] = DateTime.Now;
                Row["Actioned"] = DateTime.Now;
                Row["RetryCount"] = 0;
                Row["RetryDate"] = DBNull.Value;
                Row["PersonID"] = 0;
                Row["SourceID"] = SourceID;
                Row["ServiceID"] = ServiceID;
                Row["LoadID"] = LoadID;
                Row["Name"] = Name;
                Row["Phone"] = InData[3];
                Row["ScheduleDate"] = DBNull.Value;
                Row["Priority"] = 1;
                Row["CapturingAgent"] = -1;
                Row["Phone01"] = InData[3];
                Row["Phone02"] = InData[4];
                Row["Phone03"] = InData[5];
                Row["Phone04"] = DBNull.Value;
                Row["Phone05"] = DBNull.Value;
                Row["Phone06"] = DBNull.Value;
                Row["Phone07"] = DBNull.Value;
                Row["Phone08"] = DBNull.Value;
                Row["Phone09"] = DBNull.Value;
                Row["Phone10"] = DBNull.Value;
                Row["Comments"] = DBNull.Value;
                Row["CustomData1"] = SourceID;
                Row["CustomData2"] = DBNull.Value;
                Row["CustomData3"] = DBNull.Value;
                Row["CallerID"] = DBNull.Value;
                Row["CallerName"] = DBNull.Value;
                QueueComplete.Rows.Add(Row);

                return "Ok";
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(this.InstanceID, this.Class, "MapFileQueueComplete()", ex.Message);
                await log.SaveSync();

                return "Fail";
            }
        }
        #endregion

        #region [ Map File Presence ]
        private async Task<string> MapFilePresence(string[] InData, int SourceID, int ServiceID, int LoadID, PhoneModel Phones)
        {
            try
            {
                string Name = InData[7] + " " + InData[8];
                Name = Name.Length <= 39 ? Name : Name.Substring(0, 39);
                DataRow Row = Presence.NewRow();
                Row["ID"] = DBNull.Value;
                Row["SERVICEID"] = ServiceID;
                Row["NAME"] = Name;
                Row["PHONE"] = Phones.PH01;
                Row["CALLINGHOURS"] = DBNull.Value;
                Row["SOURCEID"] = SourceID;
                SQLRepository sqlrepo = new SQLRepository();Row["STATUS"] = (await sqlrepo.IsActive(ServiceID, LoadID)) ? "1" : "41";
                Row["SCHEDULETYPE"] = DBNull.Value;
                Row["SCHEDULEDATE"] = DBNull.Value;
                Row["LOADID"] = LoadID;
                Row["LASTAGENT"] = DBNull.Value;
                Row["LASTQCODE"] = DBNull.Value;
                Row["FIRSTHANDLINGDATE"] = DBNull.Value;
                Row["LASTHANDLINGDATE"] = DBNull.Value;
                Row["DAILYCOUNTER"] = DBNull.Value;
                Row["TOTALCOUNTER"] = DBNull.Value;
                Row["BUSYSIGNALCOUNTER"] = DBNull.Value;
                Row["NOANSWERCOUNTER"] = DBNull.Value;
                Row["ANSWERMACHINECOUNTER"] = DBNull.Value;
                Row["FAXCOUNTER"] = DBNull.Value;
                Row["INVGENREASONCOUNTER"] = DBNull.Value;
                Row["PRIORITY"] = 1;
                Row["CAPTURINGAGENT"] = DBNull.Value;
                Row["PHONE1"] = Phones.PH01;
                Row["PHONE2"] = Phones.PH02;
                Row["PHONE3"] = Phones.PH03;
                Row["PHONE4"] = DBNull.Value;
                Row["PHONE5"] = DBNull.Value;
                Row["PHONE6"] = DBNull.Value;
                Row["PHONE7"] = DBNull.Value;
                Row["PHONE8"] = DBNull.Value;
                Row["PHONE9"] = DBNull.Value;
                Row["PHONE10"] = DBNull.Value;
                Row["PHONEDESC1"] = "1";
                Row["PHONEDESC2"] = "2";
                Row["PHONEDESC3"] = "3";
                Row["PHONEDESC4"] = "4";
                Row["PHONEDESC5"] = "5";
                Row["PHONEDESC6"] = "6";
                Row["PHONEDESC7"] = "7";
                Row["PHONEDESC8"] = "8";
                Row["PHONEDESC9"] = "9";
                Row["PHONEDESC10"] = "10";
                Row["PHONESTATUS1"] = DBNull.Value;
                Row["PHONESTATUS2"] = DBNull.Value;
                Row["PHONESTATUS3"] = DBNull.Value;
                Row["PHONESTATUS4"] = DBNull.Value;
                Row["PHONESTATUS5"] = DBNull.Value;
                Row["PHONESTATUS6"] = DBNull.Value;
                Row["PHONESTATUS7"] = DBNull.Value;
                Row["PHONESTATUS8"] = DBNull.Value;
                Row["PHONESTATUS9"] = DBNull.Value;
                Row["PHONESTATUS10"] = DBNull.Value;
                Row["PHONETIMEZONEID"] = "Presence_Server";
                Row["PHONETIMEZONEID1"] = "Presence_Server";
                Row["PHONETIMEZONEID2"] = "Presence_Server";
                Row["PHONETIMEZONEID3"] = "Presence_Server";
                Row["PHONETIMEZONEID4"] = "Presence_Server";
                Row["PHONETIMEZONEID5"] = "Presence_Server";
                Row["PHONETIMEZONEID6"] = "Presence_Server";
                Row["PHONETIMEZONEID7"] = "Presence_Server";
                Row["PHONETIMEZONEID8"] = "Presence_Server";
                Row["PHONETIMEZONEID9"] = "Presence_Server";
                Row["PHONETIMEZONEID10"] = "Presence_Server";
                Row["CURRENTPHONE"] = "0";
                Row["CURRENTPHONECOUNTER"] = "0";
                Row["TIMEZONEID"] = "Presence_Server";
                Row["COMMENTS"] = DBNull.Value;
                Row["RDATE"] = DateTime.Now;
                Row["CUSTOMDATA1"] = SourceID;
                Row["CUSTOMDATA2"] = DBNull.Value;
                Row["CUSTOMDATA3"] = DBNull.Value;
                Row["CALLERID"] = DBNull.Value;
                Row["CALLERNAME"] = DBNull.Value;
                Presence.Rows.Add(Row);

                return "Ok";
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(this.InstanceID, this.Class, "MapFilePresence()", ex.Message);
                await log.SaveSync();

                return "Fail";
            }
        }
        #endregion

        #region [ Map File Script ]
        private async Task<string> MapFileScript(string[] InData, int FileLine, int SourceID, int ServiceID, int LoadID)
        {
            string Script = "[PSCRIPT].[RetentionsNewUpdated_S]";

            DataRow Row = TableScript.NewRow();
            var log = new LogConsoleScriptError(this.InstanceID, this.FileDetail.Name, FileLine, Script);

            try { Row["ID"] = SourceID; } catch (Exception) { await log.SaveSync("ID", SourceID.ToString(), "INT"); return "Fail"; }
            try { Row["LOGIN"] = DBNull.Value; } catch (Exception) { await log.SaveSync("LOGIN", "NULL", "INT"); return "Fail"; }
            try { Row["QCODE"] = DBNull.Value; } catch (Exception) { await log.SaveSync("QCODE", "NULL", "INT"); return "Fail"; }
            try { Row["STATION"] = DBNull.Value; } catch (Exception) { await log.SaveSync("STATION", "NULL", "INT"); return "Fail"; }
            try { Row["RDATE"] = DBNull.Value; } catch (Exception) { await log.SaveSync("RDATE", "NULL", "DAETTIME"); return "Fail"; }
            try { Row["LOADID"] = LoadID; } catch (Exception) { await log.SaveSync("LOADID", "NULL", "INT"); return "Fail"; }
            try { Row["SERVICEID"] = ServiceID; } catch (Exception) { await log.SaveSync("SERVICEID", ServiceID.ToString(), "INT"); return "Fail"; }
            try { Row["PARENTSCRIPTID"] = DBNull.Value; } catch (Exception) { await log.SaveSync("PARENTSCRIPTID", "NULL", "INT"); return "Fail"; }
            try { Row["PARENTRECID"] = DBNull.Value; } catch (Exception) { await log.SaveSync("PARENTRECID", "NULL", "INT"); return "Fail"; }
            try { Row["Customer_Intro"] = DBNull.Value; } catch (Exception) { await log.SaveSync("Customer_Intro", "NULL", "VARCHAR(255)"); return "Fail"; }
            try { Row["RetentionsInformations"] = DBNull.Value; } catch (Exception) { await log.SaveSync("RetentionsInformations", "NULL", "VARCHAR(255)"); return "Fail"; }
            try { Row["Authentication"] = DBNull.Value; } catch (Exception) { await log.SaveSync("Authentication", "NULL", "VARCHAR(255)"); return "Fail"; }
            try { Row["Mobile_Cell"] = (!string.IsNullOrEmpty(InData[3])) ? int.Parse(InData[3]) : 0; } catch (Exception) { await log.SaveSync("Mobile_Cell", InData[3], "INT"); return "Fail"; }
            try { Row["Mobile_Work"] = (!string.IsNullOrEmpty(InData[4])) ? int.Parse(InData[4]) : 0; } catch (Exception) { await log.SaveSync("Mobile_Work", InData[4], "INT"); return "Fail"; }
            try { Row["Mobile_Home"] = (!string.IsNullOrEmpty(InData[5])) ? int.Parse(InData[5]) : 0; } catch (Exception) { await log.SaveSync("Mobile_Home", InData[5], "INT"); return "Fail"; }
            try { Row["Sourceid"] = SourceID; } catch (Exception) { await log.SaveSync("Sourceid", SourceID.ToString(), "INT"); return "Fail"; }
            try { Row["ClientsInfo2"] = DBNull.Value; } catch (Exception) { await log.SaveSync("ClientsInfo2", "NULL", "VARCHAR(255)"); return "Fail"; }
            try { Row["LastName"] = InData[8]; } catch (Exception) { await log.SaveSync("LastName", InData[8], "VARCHAR(255)"); return "Fail"; }
            try { Row["Subject"] = DBNull.Value; } catch (Exception) { await log.SaveSync("Subject", "NULL", "VARCHAR(255)"); return "Fail"; }
            try { Row["Priority"] = (!string.IsNullOrEmpty(InData[2])) ? int.Parse(InData[2]) : 0; } catch (Exception) { await log.SaveSync("Priority", InData[2], "INT"); return "Fail"; }
            try { Row["InforceStatus"] = InData[1]; } catch (Exception) { await log.SaveSync("InforceStatus", InData[1], "VARCHAR(255)"); return "Fail"; }
            try { Row["BankName"] = InData[24]; } catch (Exception) { await log.SaveSync("BankName", InData[24], "VARCHAR(255)"); return "Fail"; }
            try { Row["BankBranchName"] = InData[23]; } catch (Exception) { await log.SaveSync("BankBranchName", InData[23], "VARCHAR(255)"); return "Fail"; }
            try { Row["FRANK"] = DBNull.Value; } catch (Exception) { await log.SaveSync("FRANK", "NULL", "VARCHAR(200)"); return "Fail"; }
            try { Row["SBSA"] = DBNull.Value; } catch (Exception) { await log.SaveSync("SBSA", "NULL", "VARCHAR(255)"); return "Fail"; }
            try { Row["VODACOM"] = DBNull.Value; } catch (Exception) { await log.SaveSync("VODACOM", "NULL", "VARCHAR(255)"); return "Fail"; }
            try { Row["SBSAClientInfo"] = DBNull.Value; } catch (Exception) { await log.SaveSync("SBSAClientInfo", "NULL", "VARCHAR(255)"); return "Fail"; }
            try { Row["AffinityCode"] = InData[0]; } catch (Exception) { await log.SaveSync("AffinityCode", InData[0], "VARCHAR(255)"); return "Fail"; }
            try { Row["FirstName"] = InData[7]; } catch (Exception) { await log.SaveSync("FirstName", InData[7], "VARCHAR(255)"); return "Fail"; }
            try { Row["BankAccountNumber"] = InData[22]; } catch (Exception) { await log.SaveSync("BankAccountNumber", InData[22], "VARCHAR(255)"); return "Fail"; }
            try { Row["RSAID"] = InData[9]; } catch (Exception) { await log.SaveSync("RSAID", InData[9], "VARCHAR(255)"); return "Fail"; }
            try { Row["Task1"] = InData[199]; } catch (Exception) { await log.SaveSync("Task1", InData[199], "VARCHAR(10)"); return "Fail"; }
            try { Row["Outcome"] = DBNull.Value; } catch (Exception) { await log.SaveSync("Outcome", "NULL", "VARCHAR(255)"); return "Fail"; }
            try { Row["Task2"] = InData[60]; } catch (Exception) { await log.SaveSync("Outcome", InData[60], "VARCHAR(255)"); return "Fail"; }
            try { Row["Outcome2"] = DBNull.Value; } catch (Exception) { await log.SaveSync("Outcome", "NULL", "VARCHAR(255)"); return "Fail"; }
            try { Row["Task3"] = InData[90]; } catch (Exception) { await log.SaveSync("Outcome", InData[90], "VARCHAR(255)"); return "Fail"; }
            try { Row["Outcome3"] = DBNull.Value; } catch (Exception) { await log.SaveSync("Outcome", "NULL", "VARCHAR(255)"); return "Fail"; }
            try { Row["Task4"] = InData[120]; } catch (Exception) { await log.SaveSync("Outcome", InData[120], "VARCHAR(255)"); return "Fail"; }
            try { Row["Outcome4"] = DBNull.Value; } catch (Exception) { await log.SaveSync("Outcome", "NULL", "VARCHAR(255)"); return "Fail"; }
            try { Row["BankAccountName"] = InData[21]; } catch (Exception) { await log.SaveSync("BankAccountName", InData[21], "VARCHAR(255)"); return "Fail"; }
            try { Row["NameofBank"] = InData[24]; } catch (Exception) { await log.SaveSync("NameofBank", InData[24], "VARCHAR(255)"); return "Fail"; }
            try { Row["DebitOrderDate"] = InData[27]; } catch (Exception) { await log.SaveSync("DebitOrderDate", InData[27], "VARCHAR(255)"); return "Fail"; }
            try { Row["PremiumAmount"] = InData[14]; } catch (Exception) { await log.SaveSync("PremiumAmount", InData[14], "VARCHAR(255)"); return "Fail"; }
            try { Row["NumberOfTasks"] = InData[198]; } catch (Exception) { await log.SaveSync("NumberOfTasks", InData[198], "VARCHAR(255)"); return "Fail"; }
            try { Row["Task5"] = InData[150]; } catch (Exception) { await log.SaveSync("Task5", InData[150], "VARCHAR(255)"); return "Fail"; }
            try { Row["Task6"] = InData[180]; } catch (Exception) { await log.SaveSync("Task6", InData[180], "VARCHAR(255)"); return "Fail"; }
            try { Row["Outcome5"] = DBNull.Value; } catch (Exception) { await log.SaveSync("Outcome5", "NULL", "VARCHAR(255)"); return "Fail"; }
            try { Row["Outcome6"] = DBNull.Value; } catch (Exception) { await log.SaveSync("Outcome6", "NULL", "VARCHAR(255)"); return "Fail"; }
            try { Row["RejectionReasonFromDesciption"] = InData[15]; } catch (Exception) { await log.SaveSync("RejectionReasonFromDesciption", InData[15], "VARCHAR(255)"); return "Fail"; }
            try { Row["Title"] = InData[6]; } catch (Exception) { await log.SaveSync("Title", InData[6], "VARCHAR(255)"); return "Fail"; }
            try { Row["Month1"] = InData[31]; } catch (Exception) { await log.SaveSync("Month1", InData[31], "VARCHAR(255)"); return "Fail"; }
            try { Row["Month2"] = InData[61]; } catch (Exception) { await log.SaveSync("Month2", InData[61], "VARCHAR(255)"); return "Fail"; }
            try { Row["Month3"] = InData[91]; } catch (Exception) { await log.SaveSync("Month3", InData[91], "VARCHAR(255)"); return "Fail"; }
            try { Row["Month4"] = InData[121]; } catch (Exception) { await log.SaveSync("Month4", InData[121], "VARCHAR(255)"); return "Fail"; }
            try { Row["Month5"] = InData[151]; } catch (Exception) { await log.SaveSync("Month5", InData[151], "VARCHAR(255)"); return "Fail"; }
            try { Row["Month6"] = InData[181]; } catch (Exception) { await log.SaveSync("Month6", InData[181], "VARCHAR(255)"); return "Fail"; }
            try { Row["Prd1EPN"] = InData[28]; } catch (Exception) { await log.SaveSync("Prd1EPN", InData[28], "VARCHAR(255)"); return "Fail"; }
            try { Row["Prd2EPN"] = InData[58]; } catch (Exception) { await log.SaveSync("Prd2EPN", InData[58], "VARCHAR(255)"); return "Fail"; }
            try { Row["Prd3EPN"] = InData[88]; } catch (Exception) { await log.SaveSync("Prd3EPN", InData[88], "VARCHAR(255)"); return "Fail"; }
            try { Row["Prd4EPN"] = InData[118]; } catch (Exception) { await log.SaveSync("Prd4EPN", InData[118], "VARCHAR(255)"); return "Fail"; }
            try { Row["Prd5EPN"] = InData[148]; } catch (Exception) { await log.SaveSync("Prd5EPN", InData[148], "VARCHAR(255)"); return "Fail"; }
            try { Row["Prd6EPN"] = InData[178]; } catch (Exception) { await log.SaveSync("Prd6EPN", InData[178], "VARCHAR(255)"); return "Fail"; }
            try { Row["ImportantInfo"] = DBNull.Value; } catch (Exception) { await log.SaveSync("ImportantInfo", "NULL", "VARCHAR(255)"); return "Fail"; }
            try { Row["DurationOnBook"] = InData[32]; } catch (Exception) { await log.SaveSync("DurationOnBook", InData[32], "VARCHAR(255)"); return "Fail"; }
            try { Row["Service1"] = DBNull.Value; } catch (Exception) { await log.SaveSync("Service1", "NULL", "VARCHAR(255)"); return "Fail"; }
            try { Row["Prd1NewExisting"] = InData[36]; } catch (Exception) { await log.SaveSync("Prd1NewExisting", InData[36], "VARCHAR(255)"); return "Fail"; }
            try { Row["Prd1Service"] = InData[44]; } catch (Exception) { await log.SaveSync("Prd1Service", InData[44], "VARCHAR(255)"); return "Fail"; }
            try { Row["Prd2Service"] = InData[74]; } catch (Exception) { await log.SaveSync("Prd2Service", InData[74], "VARCHAR(255)"); return "Fail"; }
            try { Row["Prd3Service"] = InData[104]; } catch (Exception) { await log.SaveSync("Prd3Service", InData[104], "VARCHAR(255)"); return "Fail"; }
            try { Row["Prd4Service"] = InData[134]; } catch (Exception) { await log.SaveSync("Prd4Service", InData[134], "VARCHAR(255)"); return "Fail"; }
            try { Row["Prd5Service"] = InData[164]; } catch (Exception) { await log.SaveSync("Prd5Service", InData[164], "VARCHAR(255)"); return "Fail"; }
            try { Row["Prd6Service"] = InData[194]; } catch (Exception) { await log.SaveSync("Prd6Service", InData[194], "VARCHAR(255)"); return "Fail"; }
            try { Row["PolicyNumberFromDescription"] = InData[11]; } catch (Exception) { await log.SaveSync("PolicyNumberFromDescription", InData[11], "VARCHAR(255)"); return "Fail"; }
            try { Row["Prd1MonthCountBand"] = InData[32]; } catch (Exception) { await log.SaveSync("Prd1MonthCountBand", InData[32], "VARCHAR(255)"); return "Fail"; }
            try { Row["Prd1ProductName"] = InData[29]; } catch (Exception) { await log.SaveSync("Prd1ProductName", InData[29], "VARCHAR(255)"); return "Fail"; }
            try { Row["Prd1Profile"] = InData[45]; } catch (Exception) { await log.SaveSync("Prd1Profile", InData[45], "VARCHAR(255)"); return "Fail"; }
            try { Row["Prd2Profile"] = InData[75]; } catch (Exception) { await log.SaveSync("Prd2Profile", InData[75], "VARCHAR(255)"); return "Fail"; }
            try { Row["Prd3Profile"] = InData[105]; } catch (Exception) { await log.SaveSync("Prd3Profile", InData[105], "VARCHAR(255)"); return "Fail"; }
            try { Row["Prd4Profile"] = InData[135]; } catch (Exception) { await log.SaveSync("Prd4Profile", InData[135], "VARCHAR(255)"); return "Fail"; }
            try { Row["Prd5Profile"] = InData[165]; } catch (Exception) { await log.SaveSync("Prd5Profile", InData[165], "VARCHAR(255)"); return "Fail"; }
            try { Row["Prd6Profile"] = InData[195]; } catch (Exception) { await log.SaveSync("Prd6Profile", InData[195], "VARCHAR(255)"); return "Fail"; }
            try { Row["Prd1TaskSubType"] = InData[38]; } catch (Exception) { await log.SaveSync("Prd1TaskSubType", InData[38], "VARCHAR(255)"); return "Fail"; }

            TableScript.Rows.Add(Row);
            return "Ok";
        }
        #endregion
    }
}