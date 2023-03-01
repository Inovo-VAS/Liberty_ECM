#region [ Using ]
using InovoCIM.Data.Entities;
using InovoCIM.Data.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace InovoCIM.FileProcess
{
    public class FileKYC
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
        public FileKYC(string _InstanceID, ImportFile _ImportFileModel, FileInfo _File)
        {
            this.LogRepo = new LogRepository();
            this.SqlRepo = new SQLRepository();
            this.ImportFileModel = _ImportFileModel;
            this.FileDetail = _File;
            this.InstanceID = _InstanceID;
            this.Class = "File KYC";
            this.ScriptTable = "[PSCRIPT].[Service_KYC_S]";

            this.StatsLineTotal = 0;
            this.StatsFileDuplicate = 0;
            this.StatsPresenceDuplicate = 0;
            this.StatsScriptDuplicate = 0;

            this.IndexSourceID = 34;
            this.IndexLoadIndicator = 35;

            this.IndexPhone01 = 28;
            this.IndexPhone02 = 29;
            this.IndexPhone03 = 30;
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
                Table = TableRepo.ColumnTableKYC(Table);
                TableScript = TableRepo.ColumnTableKYCScript(TableScript);
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

                            int ServiceID = 1701;
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
                    LoadModel Load = new LoadModel(this.InstanceID, ServiceID, InData[this.IndexLoadIndicator], this.ImportFileModel.Partial, "Batch");
                    LoadID = await Load.GetLoadID();
                }
                else
                {
                    IndicatorList.Add(this.StatsLineTotal, InData[this.IndexLoadIndicator]);
                    LoadModel Load = new LoadModel(this.InstanceID, ServiceID, InData[this.IndexLoadIndicator], this.ImportFileModel.Partial, "Batch");
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
                    int result = Sql.SendToInovoCIM(Table, "[ECM].[FileTableKYC]");
                }
                if (Presence.Rows.Count > 0)
                {
                    int result = Sql.SendToPresence(Presence, "[PREP].[PCO_OUTBOUNDQUEUE]");
                }
                if (TableScript.Rows.Count > 0)
                {
                    int result = Sql.SendToPresence(TableScript, "[PSCRIPT].[Service_KYC_S]");
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

        #region [ Map File Log ]
        private async Task<string> MapFileLog(string[] InData, int FileLine, int ServiceID, int LoadID, int SourceID, string IsFileDuplicate, string IsPresenceDuplicate, string IsScriptDuplicate, string MapPresenceFail, string MapScriptFail, string PhoneStatus)
        {
            try
            {
                FileLine++;

                DataRow Row = Table.NewRow();
                Row["ID"] = DBNull.Value;
                Row["InstanceID"] = this.InstanceID;
                Row["Input"] = this.FileDetail.Name;
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
                Row["PhoneStatus"] = PhoneStatus;
                Row["RSAIDNumber"] = InData[0];
                Row["ClientNumber"] = InData[1];
                Row["ClientName"] = InData[2];
                Row["Affinity"] = InData[3];
                Row["EPN"] = InData[4];
                Row["CommencementDateOriginal"] = InData[5];
                Row["CollectionCount"] = InData[6];
                Row["MonthlyPremiumCurrent"] = InData[7];
                Row["AnnualPremiumCurrent"] = InData[8];
                Row["SumAssured"] = InData[9];
                Row["MonthlyPremiumOverThreshold"] = InData[10];
                Row["AnnualPremiumOverThreshold"] = InData[11];
                Row["ProductGroup"] = InData[12];
                Row["ProductStatus"] = InData[13];
                Row["ReplacementIndicator"] = InData[14];
                Row["GrossIncome"] = InData[15];
                Row["HouseholdIncome"] = InData[16];
                Row["LineCount"] = InData[17];
                Row["PreviousEPN"] = InData[18];
                Row["PreviousEPNAddedBy"] = InData[19];
                Row["PreviousEPNAddedOn"] = InData[20];
                Row["PreviousEPNNotes"] = InData[21];
                Row["PreviousRSAID"] = InData[22];
                Row["PreviousRSAIDAddedBy"] = InData[23];
                Row["PreviousRSAIDAddedOn"] = InData[24];
                Row["PreviousRSAIDNotes"] = InData[25];
                Row["CountAcrossAffinitie"] = InData[26];
                Row["ClientGroup"] = InData[27];
                Row["MobileNumber"] = InData[28];
                Row["Work"] = InData[29];
                Row["Home"] = InData[30];
                Row["EmailAddress"] = InData[31];
                Row["Priority"] = InData[32];
                Row["AffinityCode"] = InData[33];
                Row["SourceID"] = InData[34];
                Row["Batch"] = InData[35];
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
                string name = InData[2].Length <= 39 ? InData[2] : InData[2].Substring(0, 39);
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
                Row["Name"] = name;
                Row["Phone"] = InData[28];
                Row["ScheduleDate"] = DBNull.Value;
                Row["Priority"] = 1;
                Row["CapturingAgent"] = -1;
                Row["Phone01"] = InData[28];
                Row["Phone02"] = InData[29];
                Row["Phone03"] = InData[30];
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
                string name = InData[2].Length <= 39 ? InData[2] : InData[2].Substring(0, 39);
                DataRow Row = Presence.NewRow();
                Row["ID"] = DBNull.Value;
                Row["SERVICEID"] = ServiceID;
                Row["NAME"] = name;
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
            string Script = "[PSCRIPT].[Service_KYC_S]";

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
            try { Row["RSAIDNumber"] = InData[0]; } catch (Exception) { await log.SaveSync("RSAIDNumber", InData[0], "VARCHAR(255)"); return "Fail"; }
            try { Row["ClientNumber"] = InData[1]; } catch (Exception) { await log.SaveSync("ClientNumber", InData[1], "VARCHAR(255)"); return "Fail"; }
            try { Row["ClientName"] = InData[2]; } catch (Exception) { await log.SaveSync("ClientName", InData[2], "VARCHAR(255)"); return "Fail"; }
            try { Row["Affinity"] = InData[3]; } catch (Exception) { await log.SaveSync("Affinity", InData[3], "VARCHAR(255)"); return "Fail"; }
            try { Row["EPN"] = InData[4]; } catch (Exception) { await log.SaveSync("EPN", InData[4], "VARCHAR(255)"); return "Fail"; }
            try { Row["CollectionCount"] = InData[6]; } catch (Exception) { await log.SaveSync("CollectionCount", InData[6], "VARCHAR(255)"); return "Fail"; }
            try { Row["CommencementDate"] = InData[5]; } catch (Exception) { await log.SaveSync("CommencementDate", InData[5], "DATETIME"); return "Fail"; }
            try { Row["MonthlyPremiumCurrent"] = InData[7]; } catch (Exception) { await log.SaveSync("MonthlyPremiumCurrent", InData[7], "VARCHAR(255)"); return "Fail"; }
            try { Row["AnnualPremiumCurrent"] = InData[8]; } catch (Exception) { await log.SaveSync("AnnualPremiumCurrent", InData[8], "VARCHAR(255)"); return "Fail"; }
            try { Row["MonthlyPremiumOverThreshold"] = InData[10]; } catch (Exception) { await log.SaveSync("MonthlyPremiumOverThreshold", InData[10], "VARCHAR(255)"); return "Fail"; }
            try { Row["AnnualPremiumOverThreshold"] = InData[11]; } catch (Exception) { await log.SaveSync("AnnualPremiumOverThreshold", InData[11], "VARCHAR(255)"); return "Fail"; }
            try { Row["ProductGroup"] = InData[12]; } catch (Exception) { await log.SaveSync("ProductGroup", InData[12], "VARCHAR(255)"); return "Fail"; }
            try { Row["ProductStatus"] = InData[13]; } catch (Exception) { await log.SaveSync("ProductStatus", InData[13], "VARCHAR(255)"); return "Fail"; }
            try { Row["ReplacementIndicator"] = InData[14]; } catch (Exception) { await log.SaveSync("ReplacementIndicator", InData[14], "VARCHAR(255)"); return "Fail"; }
            try { Row["GrossIncome"] = InData[15]; } catch (Exception) { await log.SaveSync("GrossIncome", InData[15], "VARCHAR(255)"); return "Fail"; }
            try { Row["HouseholdIncome"] = InData[16]; } catch (Exception) { await log.SaveSync("HouseholdIncome", InData[16], "VARCHAR(255)"); return "Fail"; }
            try { Row["LineCount"] = InData[17]; } catch (Exception) { await log.SaveSync("LineCount", InData[17], "VARCHAR(255)"); return "Fail"; }
            try { Row["PreviousEPN"] = InData[18]; } catch (Exception) { await log.SaveSync("PreviousEPN", InData[18], "VARCHAR(255)"); return "Fail"; }
            try { Row["PreviousEPNAddedBy"] = InData[19]; } catch (Exception) { await log.SaveSync("PreviousEPNAddedBy", InData[19], "VARCHAR(255)"); return "Fail"; }
            try { Row["PreviousEPNAddedOn"] = InData[20]; } catch (Exception) { await log.SaveSync("PreviousEPNAddedOn", InData[20], "VARCHAR(255)"); return "Fail"; }
            try { Row["PreviousEPNNotes"] = InData[21]; } catch (Exception) { await log.SaveSync("PreviousEPNNotes", InData[21], "VARCHAR(255)"); return "Fail"; }
            try { Row["PreviousRSAID"] = InData[22]; } catch (Exception) { await log.SaveSync("PreviousRSAID", InData[22], "VARCHAR(255)"); return "Fail"; }
            try { Row["PreviousRSAIDAddedBy"] = InData[23]; } catch (Exception) { await log.SaveSync("PreviousRSAIDAddedBy", InData[23], "VARCHAR(255)"); return "Fail"; }
            try { Row["PreviousRSAIDAddedOn"] = InData[24]; } catch (Exception) { await log.SaveSync("PreviousRSAIDAddedOn", InData[24], "VARCHAR(255)"); return "Fail"; }
            try { Row["PreviousRSAIDNotes"] = InData[25]; } catch (Exception) { await log.SaveSync("PreviousRSAIDNotes", InData[25], "VARCHAR(255)"); return "Fail"; }
            try { Row["CountAcrossAffinitie"] = InData[26]; } catch (Exception) { await log.SaveSync("CountAcrossAffinitie", InData[26], "VARCHAR(255)"); return "Fail"; }
            try { Row["ClientGroup"] = InData[27]; } catch (Exception) { await log.SaveSync("ClientGroup", InData[27], "VARCHAR(255)"); return "Fail"; }
            try { Row["MobileNumber"] = (!string.IsNullOrEmpty(InData[28].Replace("-", ""))) ? InData[28] : "0"; } catch (Exception) { await log.SaveSync("MobileNumber", InData[28], "VARCHAR(30)"); return "Fail"; }
            try { Row["EmailAddress"] = InData[31]; } catch (Exception) { await log.SaveSync("EmailAddress", InData[31], "VARCHAR(255)"); return "Fail"; }
            try { Row["Priority"] = (!string.IsNullOrEmpty(InData[32])) ? int.Parse(InData[32]) : 0; } catch (Exception) { await log.SaveSync("Priority", InData[32], "INT"); return "Fail"; }
            try { Row["AffinityCode"] = int.Parse(InData[33]); } catch (Exception) { await log.SaveSync("AffinityCode", InData[33], "INT"); return "Fail"; }
            try { Row["Work"] = (!string.IsNullOrEmpty(InData[29].Replace("-",""))) ? int.Parse(InData[29]) : 0; } catch (Exception) { await log.SaveSync("Work", InData[29], "INT"); return "Fail"; }
            try { Row["Home"] = (!string.IsNullOrEmpty(InData[30].Replace("-", ""))) ? int.Parse(InData[30]) : 0; } catch (Exception) { await log.SaveSync("Home", InData[30], "INT"); return "Fail"; }

            TableScript.Rows.Add(Row);
            return "Ok";
        }
        #endregion
    }
}