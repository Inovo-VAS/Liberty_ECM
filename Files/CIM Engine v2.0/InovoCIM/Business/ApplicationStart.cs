#region [ Using ]
using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using InovoCIM.Data.Entities;
using InovoCIM.Data.Models;
using InovoCIM.Data.Repository;
#endregion

namespace InovoCIM.Business
{
    public class ApplicationStart
    {
        public LogRepository LogRepo { get; set; }
        public string InstanceID { get; set; }
        public string Class { get; set; }

		public string Schema { get; set; }

        #region [ Default Constructor ]
        public ApplicationStart(string _InstanceID)
        {
            this.LogRepo = new LogRepository();
            this.InstanceID = _InstanceID;
            this.Class = "Start Process";
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
                bool FinalResult = await SetSettings();                                                         // Get the settings from the InovoCIM.Settings.xml in the route folder of application
                if (FinalResult)
                {
                    FinalResult = await TestDatabase();                                                         // Make a connection to CIM and then Presence - to make sure we are connected
                    if (FinalResult)
                    {
                        FinalResult = await ManageTables();                                                     // Create the Adhoc tables, from this end - as they are not standard as part of the Web Application.
                    }
                }

                await Event.SaveAsync(this.Class, "Master()", "End");                                           // Just logs events to database in order to see if anything goes wrong
                var Runtime = new LogConsoleRuntime(this.InstanceID, this.Class, "Master()", StartTime);        // Logs the time the method took to run
                await Runtime.SaveSync();

                return FinalResult;                                                                             // Returns True or False - then the rest of the code knows if it should run or not.
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(this.InstanceID, this.Class, "Master()", ex.Message);
                await log.SaveSync();

                return false;
            }
        }
        #endregion

        //---------------------------------------------------------------------------//

        #region [ Set Settings ]
        private async Task<bool> SetSettings()
        {
            string SettingsFile = AppDomain.CurrentDomain.BaseDirectory + "\\InovoCIM.Settings.xml";
            try
            {
                if (!File.Exists(SettingsFile))
                {
                    XmlWriterSettings xml = new XmlWriterSettings { Async = false, Indent = true, IndentChars = "\t" };
                    using (XmlWriter writer = XmlWriter.Create(SettingsFile, xml))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("Database");
                        writer.WriteElementString("dbInovoCIM", @"Data Source=localhost\SQLEXPRESS;Initial Catalog=LibertyCim;Integrated Security=SSPI;MultipleActiveResultSets=true;");
                        writer.WriteElementString("dbPresence", @"Data Source=localhost\SQLEXPRESS;Initial Catalog=SQLPR1;Integrated Security=SSPI;MultipleActiveResultSets=true;");
                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                        writer.Flush();
                    }
                    return false;
                }

                var xdoc = XDocument.Load(SettingsFile);
                var xmlFile = from item in xdoc.Descendants("Database") select new { dbSchema = (string)item.Element("dbSchema").Value, dbInovoCIM = (string)item.Element("dbInovoCIM").Value, dbPresence = (string)item.Element("dbPresence").Value };
                foreach (var xmlItem in xmlFile)
                {
					Database.dbSchema = xmlItem.dbSchema;
                    Database.dbInovoCIM = xmlItem.dbInovoCIM;
                    Database.dbPresence = xmlItem.dbPresence;
                }

                return true;
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(this.InstanceID, this.Class, "SetSettings()", ex.Message);
                await log.SaveSync();

                return false;
            }
        }
        #endregion

        #region [ Test Database ]
        private async Task<bool> TestDatabase()
        {
            try
            {
                using (var conn = new SqlConnection(Database.dbInovoCIM)) { conn.Open(); conn.Close(); }
                using (var conn = new SqlConnection(Database.dbPresence)) { conn.Open(); conn.Close(); }
                return true;
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(this.InstanceID, this.Class, "TestDatabase()", ex.Message);
                await log.SaveSync();

                return false;
            }
        }
        #endregion

        #region [ Manage Tables ]
        private async Task<bool> ManageTables()
        {
            try
            {
                #region [ dbInovoCIM ] - [ECM].[FileTableMissed]
                using (var conn = new SqlConnection(Database.dbInovoCIM))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ECM].[FileTableMissed]') AND type in (N'U'))
                                            BEGIN
                                                CREATE TABLE [ECM].[FileTableMissed]
                                                (
	                                                [ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	                                                [InstanceID] VARCHAR(50),
	                                                [Input] VARCHAR(250),
	                                                [Received] DATETIME,
                                                    [FileLine] INT,
                                                    [InputServiceID] INT,
	                                                [InputLoadID] INT,
	                                                [InputSourceID] INT,
	                                                [IsFileDuplicate] VARCHAR(3),
	                                                [IsPresenceDuplicate] VARCHAR(3),
	                                                [IsScriptDuplicate] VARCHAR(3),
	                                                [MapPresenceFail] VARCHAR(3),
	                                                [MapScriptFail] VARCHAR(3),
                                                    [PhoneStatus] VARCHAR(30),
	                                                [AffinityCode] VARCHAR(250),
	                                                [InforceStatus] VARCHAR(250),
	                                                [Priority] VARCHAR(250),
	                                                [MobilePhone] VARCHAR(250),
	                                                [Telephone1] VARCHAR(250),
	                                                [Telephone2] VARCHAR(250),
	                                                [Title] VARCHAR(250),
	                                                [FirstName] VARCHAR(250),
	                                                [LastName] VARCHAR(250),
	                                                [RSAID] VARCHAR(250),
	                                                [ClientSegementCode] VARCHAR(250),
	                                                [PolicyNumberFromDesciption] VARCHAR(250),
	                                                [ProductFromDesciption] VARCHAR(250),
	                                                [ProcessingDateFromDesciption] VARCHAR(250),
	                                                [PremiumAmountFromDesciption] VARCHAR(250),
	                                                [RejectionReasonFromDesciption] VARCHAR(250),
	                                                [FirstDebitOrderIndicatorFromDesciption] VARCHAR(250),
	                                                [Prd1PolicyProductNumber] VARCHAR(250),
	                                                [Prd1TaskGUID] VARCHAR(250),
	                                                [Prd1InforceStatus] VARCHAR(250),
	                                                [Prd1Priority] VARCHAR(250),
	                                                [Prd1BankAccountName] VARCHAR(250),
	                                                [Prd1BankAccountNumber] VARCHAR(250),
	                                                [Prd1BankBranchName] VARCHAR(250),
	                                                [Prd1BankName] VARCHAR(250),
	                                                [Prd1PolicyNumber] VARCHAR(250),
	                                                [Prd1CollectionCount] VARCHAR(250),
	                                                [Prd1DebitOrderDay] VARCHAR(250),
	                                                [Prd1EPN] VARCHAR(250),
	                                                [Prd1ProductName] VARCHAR(250),
	                                                [Prd1TaskNumber] VARCHAR(250),
	                                                [Prd1SkippedMonthAllowed] VARCHAR(250),
	                                                [Prd1InforceMonthCount] VARCHAR(250),
	                                                [Prd1MonthlyPremium] VARCHAR(250),
	                                                [Prd1TotalSumInsured] VARCHAR(250),
	                                                [Prd1InforceMonthCountBand] VARCHAR(250),
	                                                [Prd1NewExisting] VARCHAR(250),
	                                                [Prd1ProductCategory] VARCHAR(250),
	                                                [Prd1TaskSubType] VARCHAR(250),
	                                                [Prd1LastCoverMonthMissedPayments] VARCHAR(250),
	                                                [Prd1CollectionCountOverInforceMonths] VARCHAR(250),
	                                                [Prd1ACCAUpfrontRejection] VARCHAR(250),
	                                                [Prd1ACCAPendingAction] VARCHAR(250),
	                                                [Prd1CalcPriority] VARCHAR(250),
	                                                [Prd1Service] VARCHAR(250),
	                                                [Prd1Profile] VARCHAR(250),
	                                                [Prd1CoverMonth] VARCHAR(250),
	                                                [Prd2PolicyProductNumber] VARCHAR(250),
	                                                [Prd2TaskGUID] VARCHAR(250),
	                                                [Prd2InforceStatus] VARCHAR(250),
	                                                [Prd2Priority] VARCHAR(250),
	                                                [Prd2BankAccountName] VARCHAR(250),
	                                                [Prd2BankAccountNumber] VARCHAR(250),
	                                                [Prd2BankBranchName] VARCHAR(250),
	                                                [Prd2BankName] VARCHAR(250),
	                                                [Prd2PolicyNumber] VARCHAR(250),
	                                                [Prd2CollectionCount] VARCHAR(250),
	                                                [Prd2DebitOrderDay] VARCHAR(250),
	                                                [Prd2EPN] VARCHAR(250),
	                                                [Prd2ProductName] VARCHAR(250),
	                                                [Prd2TaskNumber] VARCHAR(250),
	                                                [Prd2SkippedMonthAllowed] VARCHAR(250),
	                                                [Prd2InforceMonthCount] VARCHAR(250),
	                                                [Prd2MonthlyPremium] VARCHAR(250),
	                                                [Prd2TotalSumInsured] VARCHAR(250),
	                                                [Prd2InforceMonthCountBand] VARCHAR(250),
	                                                [Prd2NewExisting] VARCHAR(250),
	                                                [Prd2ProductCategory] VARCHAR(250),
	                                                [Prd2TaskSubType] VARCHAR(250),
	                                                [Prd2LastCoverMonthMissedPayments] VARCHAR(250),
	                                                [Prd2CollectionCountOverInforceMonths] VARCHAR(250),
	                                                [Prd2ACCAUpfrontRejection] VARCHAR(250),
	                                                [Prd2ACCAPendingAction] VARCHAR(250),
	                                                [Prd2CalcPriority] VARCHAR(250),
	                                                [Prd2Service] VARCHAR(250),
	                                                [Prd2Profile] VARCHAR(250),
	                                                [Prd2CoverMonth] VARCHAR(250),
	                                                [Prd3PolicyProductNumber] VARCHAR(250),
	                                                [Prd3TaskGUID] VARCHAR(250),
	                                                [Prd3InforceStatus] VARCHAR(250),
	                                                [Prd3Priority] VARCHAR(250),
	                                                [Prd3BankAccountName] VARCHAR(250),
	                                                [Prd3BankAccountNumber] VARCHAR(250),
	                                                [Prd3BankBranchName] VARCHAR(250),
	                                                [Prd3BankName] VARCHAR(250),
	                                                [Prd3PolicyNumber] VARCHAR(250),
	                                                [Prd3CollectionCount] VARCHAR(250),
	                                                [Prd3DebitOrderDay] VARCHAR(250),
	                                                [Prd3EPN] VARCHAR(250),
	                                                [Prd3ProductName] VARCHAR(250),
	                                                [Prd3TaskNumber] VARCHAR(250),
	                                                [Prd3SkippedMonthAllowed] VARCHAR(250),
	                                                [Prd3InforceMonthCount] VARCHAR(250),
	                                                [Prd3MonthlyPremium] VARCHAR(250),
	                                                [Prd3TotalSumInsured] VARCHAR(250),
	                                                [Prd3InforceMonthCountBand] VARCHAR(250),
	                                                [Prd3NewExisting] VARCHAR(250),
	                                                [Prd3ProductCategory] VARCHAR(250),
	                                                [Prd3TaskSubType] VARCHAR(250),
	                                                [Prd3LastCoverMonthMissedPayments] VARCHAR(250),
	                                                [Prd3CollectionCountOverInforceMonths] VARCHAR(250),
	                                                [Prd3ACCAUpfrontRejection] VARCHAR(250),
	                                                [Prd3ACCAPendingAction] VARCHAR(250),
	                                                [Prd3CalcPriority] VARCHAR(250),
	                                                [Prd3Service] VARCHAR(250),
	                                                [Prd3Profile] VARCHAR(250),
	                                                [Prd3CoverMonth] VARCHAR(250),
	                                                [Prd4PolicyProductNumber] VARCHAR(250),
	                                                [Prd4TaskGUID] VARCHAR(250),
	                                                [Prd4InforceStatus] VARCHAR(250),
	                                                [Prd4Priority] VARCHAR(250),
	                                                [Prd4BankAccountName] VARCHAR(250),
	                                                [Prd4BankAccountNumber] VARCHAR(250),
	                                                [Prd4BankBranchName] VARCHAR(250),
	                                                [Prd4BankName] VARCHAR(250),
	                                                [Prd4PolicyNumber] VARCHAR(250),
	                                                [Prd4CollectionCount] VARCHAR(250),
	                                                [Prd4DebitOrderDay] VARCHAR(250),
	                                                [Prd4EPN] VARCHAR(250),
	                                                [Prd4ProductName] VARCHAR(250),
	                                                [Prd4TaskNumber] VARCHAR(250),
	                                                [Prd4SkippedMonthAllowed] VARCHAR(250),
	                                                [Prd4InforceMonthCount] VARCHAR(250),
	                                                [Prd4MonthlyPremium] VARCHAR(250),
	                                                [Prd4TotalSumInsured] VARCHAR(250),
	                                                [Prd4InforceMonthCountBand] VARCHAR(250),
	                                                [Prd4NewExisting] VARCHAR(250),
	                                                [Prd4ProductCategory] VARCHAR(250),
	                                                [Prd4TaskSubType] VARCHAR(250),
	                                                [Prd4LastCoverMonthMissedPayments] VARCHAR(250),
	                                                [Prd4CollectionCountOverInforceMonths] VARCHAR(250),
	                                                [Prd4ACCAUpfrontRejection] VARCHAR(250),
	                                                [Prd4ACCAPendingAction] VARCHAR(250),
	                                                [Prd4CalcPriority] VARCHAR(250),
	                                                [Prd4Service] VARCHAR(250),
	                                                [Prd4Profile] VARCHAR(250),
	                                                [Prd4CoverMonth] VARCHAR(250),
	                                                [Prd5PolicyProductNumber] VARCHAR(250),
	                                                [Prd5TaskGUID] VARCHAR(250),
	                                                [Prd5InforceStatus] VARCHAR(250),
	                                                [Prd5Priority] VARCHAR(250),
	                                                [Prd5BankAccountName] VARCHAR(250),
	                                                [Prd5BankAccountNumber] VARCHAR(250),
	                                                [Prd5BankBranchName] VARCHAR(250),
	                                                [Prd5BankName] VARCHAR(250),
	                                                [Prd5PolicyNumber] VARCHAR(250),
	                                                [Prd5CollectionCount] VARCHAR(250),
	                                                [Prd5DebitOrderDay] VARCHAR(250),
	                                                [Prd5EPN] VARCHAR(250),
	                                                [Prd5ProductName] VARCHAR(250),
	                                                [Prd5TaskNumber] VARCHAR(250),
	                                                [Prd5SkippedMonthAllowed] VARCHAR(250),
	                                                [Prd5InforceMonthCount] VARCHAR(250),
	                                                [Prd5MonthlyPremium] VARCHAR(250),
	                                                [Prd5TotalSumInsured] VARCHAR(250),
	                                                [Prd5InforceMonthCountBand] VARCHAR(250),
	                                                [Prd5NewExisting] VARCHAR(250),
	                                                [Prd5ProductCategory] VARCHAR(250),
	                                                [Prd5TaskSubType] VARCHAR(250),
	                                                [Prd5LastCoverMonthMissedPayments] VARCHAR(250),
	                                                [Prd5CollectionCountOverInforceMonths] VARCHAR(250),
	                                                [Prd5ACCAUpfrontRejection] VARCHAR(250),
	                                                [Prd5ACCAPendingAction] VARCHAR(250),
	                                                [Prd5CalcPriority] VARCHAR(250),
	                                                [Prd5Service] VARCHAR(250),
	                                                [Prd5Profile] VARCHAR(250),
	                                                [Prd5CoverMonth] VARCHAR(250),
	                                                [Prd6PolicyProductNumber] VARCHAR(250),
	                                                [Prd6TaskGUID] VARCHAR(250),
	                                                [Prd6InforceStatus] VARCHAR(250),
	                                                [Prd6Priority] VARCHAR(250),
	                                                [Prd6BankAccountName] VARCHAR(250),
	                                                [Prd6BankAccountNumber] VARCHAR(250),
	                                                [Prd6BankBranchName] VARCHAR(250),
	                                                [Prd6BankName] VARCHAR(250),
	                                                [Prd6PolicyNumber] VARCHAR(250),
	                                                [Prd6CollectionCount] VARCHAR(250),
	                                                [Prd6DebitOrderDay] VARCHAR(250),
	                                                [Prd6EPN] VARCHAR(250),
	                                                [Prd6ProductName] VARCHAR(250),
	                                                [Prd6TaskNumber] VARCHAR(250),
	                                                [Prd6SkippedMonthAllowed] VARCHAR(250),
	                                                [Prd6InforceMonthCount] VARCHAR(250),
	                                                [Prd6MonthlyPremium] VARCHAR(250),
	                                                [Prd6TotalSumInsured] VARCHAR(250),
	                                                [Prd6InforceMonthCountBand] VARCHAR(250),
	                                                [Prd6NewExisting] VARCHAR(250),
	                                                [Prd6ProductCategory] VARCHAR(250),
	                                                [Prd6TaskSubType] VARCHAR(250),
	                                                [Prd6LastCoverMonthMissedPayments] VARCHAR(250),
	                                                [Prd6CollectionCountOverInforceMonths] VARCHAR(250),
	                                                [Prd6ACCAUpfrontRejection] VARCHAR(250),
	                                                [Prd6ACCAPendingAction] VARCHAR(250),
	                                                [Prd6CalcPriority] VARCHAR(250),
	                                                [Prd6Service] VARCHAR(250),
	                                                [Prd6Profile] VARCHAR(250),
	                                                [Prd6CoverMonth] VARCHAR(250),
	                                                [TaskGUIDCount] VARCHAR(250),
	                                                [TaskNumberCount] VARCHAR(250),
	                                                [SourceID] VARCHAR(250)
                                                )
                                            END";
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
                #endregion

                #region [ dbInovoCIM ] - [ECM].[FileTableStandard]
                using (var conn = new SqlConnection(Database.dbInovoCIM))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ECM].[FileTableStandard]') AND type in (N'U'))
                                            BEGIN
                                                CREATE TABLE [ECM].[FileTableStandard]
                                                (
	                                                [ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	                                                [InstanceID] VARCHAR(50),
	                                                [Input] VARCHAR(250),
	                                                [Received] DATETIME,
                                                    [FileLine] INT,
                                                    [InputServiceID] INT,
	                                                [InputLoadID] INT,
	                                                [InputSourceID] INT,
	                                                [IsFileDuplicate] VARCHAR(3),
	                                                [IsPresenceDuplicate] VARCHAR(3),
	                                                [IsScriptDuplicate] VARCHAR(3),
	                                                [MapPresenceFail] VARCHAR(3),
	                                                [MapScriptFail] VARCHAR(3),
                                                    [PhoneStatus] VARCHAR(30),
	                                                [Name] VARCHAR(250),
	                                                [IDN] VARCHAR(250),
	                                                [Mobile] VARCHAR(250),
	                                                [HomePhone] VARCHAR(250),
	                                                [WorkPhone] VARCHAR(250),
	                                                [SourceID] VARCHAR(250),
	                                                [SourceCampaign] VARCHAR(250),
	                                                [Batch] VARCHAR(250)
                                                )
                                            END";
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
                #endregion

                #region [ dbInovoCIM ] - [ECM].[FileTableFYC]
                using (var conn = new SqlConnection(Database.dbInovoCIM))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ECM].[FileTableKYC]') AND type in (N'U'))
                                            BEGIN
                                                CREATE TABLE [ECM].[FileTableKYC]
                                                (
	                                                [ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	                                                [InstanceID] VARCHAR(50),
	                                                [Input] VARCHAR(250),
	                                                [Received] DATETIME,
                                                    [FileLine] INT,
                                                    [InputServiceID] INT,
	                                                [InputLoadID] INT,
	                                                [InputSourceID] INT,
	                                                [IsFileDuplicate] VARCHAR(3),
	                                                [IsPresenceDuplicate] VARCHAR(3),
	                                                [IsScriptDuplicate] VARCHAR(3),
	                                                [MapPresenceFail] VARCHAR(3),
	                                                [MapScriptFail] VARCHAR(3),
                                                    [PhoneStatus] VARCHAR(30),
	                                                [RSAIDNumber] VARCHAR(250),
	                                                [ClientNumber] VARCHAR(250),
	                                                [ClientName] VARCHAR(250),
	                                                [Affinity] VARCHAR(250),
	                                                [EPN] VARCHAR(250),
	                                                [CommencementDateOriginal] VARCHAR(250),
	                                                [CollectionCount] VARCHAR(250),
	                                                [MonthlyPremiumCurrent] VARCHAR(250),
	                                                [AnnualPremiumCurrent] VARCHAR(250),
	                                                [SumAssured] VARCHAR(250),
	                                                [MonthlyPremiumOverThreshold] VARCHAR(250),
	                                                [AnnualPremiumOverThreshold] VARCHAR(250),
	                                                [ProductGroup] VARCHAR(250),
	                                                [ProductStatus] VARCHAR(250),
	                                                [ReplacementIndicator] VARCHAR(250),
	                                                [GrossIncome] VARCHAR(250),
	                                                [HouseholdIncome] VARCHAR(250),
	                                                [LineCount] VARCHAR(250),
	                                                [PreviousEPN] VARCHAR(250),
	                                                [PreviousEPNAddedBy] VARCHAR(250),
	                                                [PreviousEPNAddedOn] VARCHAR(250),
	                                                [PreviousEPNNotes] VARCHAR(250),
	                                                [PreviousRSAID] VARCHAR(250),
	                                                [PreviousRSAIDAddedBy] VARCHAR(250),
	                                                [PreviousRSAIDAddedOn] VARCHAR(250),
	                                                [PreviousRSAIDNotes] VARCHAR(250),
	                                                [CountAcrossAffinitie] VARCHAR(250),
	                                                [ClientGroup] VARCHAR(250),
	                                                [MobileNumber] VARCHAR(250),
	                                                [Work] VARCHAR(250),
	                                                [Home] VARCHAR(250),
	                                                [EmailAddress] VARCHAR(250),
	                                                [Priority] VARCHAR(250),
	                                                [AffinityCode] VARCHAR(250),
	                                                [SourceID] VARCHAR(250),
	                                                [Batch] VARCHAR(250)
                                                )
                                            END";
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
                #endregion

                #region [ dbInovoCIM ] - [ECM].[LogConsoleScriptError]
                using (var conn = new SqlConnection(Database.dbInovoCIM))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ECM].[LogConsoleScriptError]') AND type in (N'U'))
                                            BEGIN
                                                CREATE TABLE [ECM].[LogConsoleScriptError]
                                                (
	                                                [LogConsoleScriptErrorID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	                                                [InstanceID] VARCHAR(50),
	                                                [FileName] VARCHAR(300),
	                                                [FileLine] INT,
	                                                [Script] VARCHAR(300),
                                                    [Column] VARCHAR(300),
                                                    [Input] VARCHAR(300),
	                                                [Type] VARCHAR(50),
	                                                [Updated] DATETIME
                                                )
                                            END";
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
                #endregion

                return true;
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(this.InstanceID, this.Class, "ManageTables()", ex.Message);
                await log.SaveSync();

                return false;
            }
        }
        #endregion
    }
}