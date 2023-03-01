using System;
using System.Data;
using InovoCIM.Data.Entities;

namespace InovoCIM.Data.Repository
{

    public class TableRepository
    {
        #region [ Column Table Broker Lapse ]
        public DataTable ColumnTableBrokerLapse(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("Source_ID", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("IDNumber", typeof(string));
            table.Columns.Add("CellNumber", typeof(string));
            table.Columns.Add("ProductName_&_StatusDate", typeof(string));
            table.Columns.Add("Premium", typeof(string));
            return table;
        }
        #endregion

        #region [ Column Table Liberty Win Back ]
        public DataTable ColumnTableLibertyWinBack(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("Source_ID", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("IDNumber", typeof(string));
            table.Columns.Add("CellNumber", typeof(string));
            table.Columns.Add("Last_Q_Code", typeof(string));
            table.Columns.Add("Last_Handled_Date", typeof(string));
            return table;
        }
        #endregion

        #region [ Column Table Direct NTU ]
        public DataTable ColumnTableDirectNTU(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("Source_ID", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("IDNumber", typeof(string));
            table.Columns.Add("CellNumber", typeof(string));
            table.Columns.Add("ProductName_&_StatusDate", typeof(string));
            table.Columns.Add("Premium", typeof(string));
            return table;
        }
        #endregion

        #region [ Column Table Extended Family Funeral ]
        public DataTable ColumnTableExtendedFamilyFuneral(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("Source_ID", typeof(int));
            table.Columns.Add("FirstName", typeof(string));
            table.Columns.Add("Surname", typeof(string));
            table.Columns.Add("Policy_Holder_IDNumber", typeof(string));
            table.Columns.Add("Policy_Number", typeof(string));
            table.Columns.Add("CellNumber", typeof(string));
            table.Columns.Add("Commencement_Date", typeof(string));
            table.Columns.Add("Scheme_Number", typeof(string));
            table.Columns.Add("Scheme_Name", typeof(string));
            table.Columns.Add("Bank_Branch", typeof(string));
            table.Columns.Add("Bank_Name", typeof(string));
            table.Columns.Add("Bank_Account", typeof(string));
            table.Columns.Add("Product_Name", typeof(string));
            table.Columns.Add("Premium", typeof(string));
            table.Columns.Add("Debit_Date", typeof(string));
            table.Columns.Add("Address1", typeof(string));
            table.Columns.Add("Address2", typeof(string));
            table.Columns.Add("Address3", typeof(string));
            table.Columns.Add("Postal_Code", typeof(string));
            table.Columns.Add("Main_Life_Cover", typeof(string));
            return table;
        }
        #endregion

        #region [ Column Table Extended Family Funeral ]
        public DataTable ColumnTableExtendedFamilyFuneralScript(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("LOGIN", typeof(int));
            table.Columns.Add("QCODE", typeof(int));
            table.Columns.Add("STATION", typeof(int));
            table.Columns.Add("RDATE", typeof(DateTime));
            table.Columns.Add("LOADID", typeof(int));
            table.Columns.Add("SERVICEID", typeof(int));
            table.Columns.Add("PARENTSCRIPTID", typeof(int));
            table.Columns.Add("PARENTRECID", typeof(int));
            table.Columns.Add("PolicyNumberL", typeof(string));
            table.Columns.Add("PolicyHolderNamesL", typeof(string));
            table.Columns.Add("PolicyHolderSurnameL", typeof(string));
            table.Columns.Add("PolicyHolderIDNumberL", typeof(string));
            table.Columns.Add("PHONE", typeof(string));
            table.Columns.Add("CalMonthL", typeof(string));
            table.Columns.Add("CalYearL", typeof(string));
            table.Columns.Add("BankNameL", typeof(string));
            table.Columns.Add("BankAccountNumberL", typeof(string));
            table.Columns.Add("BankBranchL", typeof(string));
            table.Columns.Add("ProductNameL", typeof(string));
            table.Columns.Add("AddressLine1L", typeof(string));
            table.Columns.Add("Addressline2L", typeof(string));
            table.Columns.Add("AddressLine3L", typeof(string));
            table.Columns.Add("PostalCodeL", typeof(string));
            table.Columns.Add("PremiumAmountL", typeof(string));
            table.Columns.Add("CorrectPerson", typeof(string));
            table.Columns.Add("FewMinutes", typeof(string));
            table.Columns.Add("Important", typeof(string));
            table.Columns.Add("ReadinessForClose", typeof(string));
            table.Columns.Add("UpdateDetails", typeof(string));
            table.Columns.Add("ConfirmAddress", typeof(string));
            table.Columns.Add("UpdateAddressLine1", typeof(string));
            table.Columns.Add("UpdateAddressLine2", typeof(string));
            table.Columns.Add("UpdateAddressLine3", typeof(string));
            table.Columns.Add("UpdatePostalCode", typeof(string));
            table.Columns.Add("SOURCEID", typeof(int));
            table.Columns.Add("DEBITDAYL", typeof(string));
            table.Columns.Add("CommencementDateL", typeof(string));
            table.Columns.Add("SchemeNameL", typeof(string));
            table.Columns.Add("SchemeNumberL", typeof(string));
            table.Columns.Add("CurrentlyEnjoyBenefits", typeof(string));
            table.Columns.Add("ConfirmIDNumber", typeof(string));
            table.Columns.Add("Benefit1", typeof(string));
            table.Columns.Add("Benefit2", typeof(string));
            table.Columns.Add("ActivateCover", typeof(string));
            table.Columns.Add("PersalNumber", typeof(string));
            table.Columns.Add("MainLifeCoverL", typeof(string));
            return table;
        }
        #endregion

        #region [ Column Table L@W NTU ]
        public DataTable ColumnLATWNTU(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("Source_ID", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("IDNumber", typeof(string));
            table.Columns.Add("CellNumber", typeof(string));
            table.Columns.Add("ProductName_&_StatusDate", typeof(string));
            table.Columns.Add("Premium", typeof(string));
            return table;
        }
        #endregion

        #region [ Column Table Qualified Burial ]
        public DataTable ColumnTableQualifiedBurial(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("Source_ID", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("CellNumber", typeof(string));
            table.Columns.Add("Best_Time_To_Contact", typeof(string));
            table.Columns.Add("Product", typeof(string));
            return table;
        }
        #endregion

        #region [ Column Table KYC ]
        public DataTable ColumnTableKYC(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("RSAIDNumber", typeof(string));
            table.Columns.Add("ClientNumber", typeof(string));
            table.Columns.Add("ClientName", typeof(string));
            table.Columns.Add("Affinity", typeof(string));
            table.Columns.Add("EPN", typeof(string));
            table.Columns.Add("CommencementDateOriginal", typeof(string));
            table.Columns.Add("CollectionCount", typeof(string));
            table.Columns.Add("MonthlyPremiumCurrent", typeof(string));
            table.Columns.Add("AnnualPremiumCurrent", typeof(string));
            table.Columns.Add("SumAssured", typeof(string));
            table.Columns.Add("MonthlyPremiumOverThreshold", typeof(string));
            table.Columns.Add("AnnualPremiumOverThreshold", typeof(string));
            table.Columns.Add("ProductGroup", typeof(string));
            table.Columns.Add("ProductStatus", typeof(string));
            table.Columns.Add("ReplacementIndicator", typeof(string));
            table.Columns.Add("GrossIncome", typeof(string));
            table.Columns.Add("HouseholdIncome", typeof(string));
            table.Columns.Add("LineCount", typeof(string));
            table.Columns.Add("PreviousEPN", typeof(string));
            table.Columns.Add("PreviousEPNAddedBy", typeof(string));
            table.Columns.Add("PreviousEPNAddedOn", typeof(string));
            table.Columns.Add("PreviousEPNNotes", typeof(string));
            table.Columns.Add("PreviousRSAID", typeof(string));
            table.Columns.Add("PreviousRSAIDAddedBy", typeof(string));
            table.Columns.Add("PreviousRSAIDAddedOn", typeof(string));
            table.Columns.Add("PreviousRSAIDNotes", typeof(string));
            table.Columns.Add("CountAcrossAffinitie", typeof(string));
            table.Columns.Add("ClientGroup", typeof(string));
            table.Columns.Add("MobileNumber", typeof(string));
            table.Columns.Add("Work", typeof(string));
            table.Columns.Add("Home", typeof(string));
            table.Columns.Add("EmailAddress", typeof(string));
            table.Columns.Add("Priority", typeof(string));
            table.Columns.Add("AffinityCode", typeof(string));
            table.Columns.Add("SourceID", typeof(string));
            table.Columns.Add("Batch", typeof(string));
            return table;
        }
        #endregion

        #region [ Column Table KYC Script ]
        public DataTable ColumnTableKYCScript(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("LOGIN", typeof(int));
            table.Columns.Add("QCODE", typeof(int));
            table.Columns.Add("STATION", typeof(int));
            table.Columns.Add("RDATE", typeof(DateTime));
            table.Columns.Add("LOADID", typeof(int));
            table.Columns.Add("SERVICEID", typeof(int));
            table.Columns.Add("PARENTSCRIPTID", typeof(int));
            table.Columns.Add("PARENTRECID", typeof(int));
            table.Columns.Add("RSAIDNumber", typeof(string));
            table.Columns.Add("ClientNumber", typeof(string));
            table.Columns.Add("ClientName", typeof(string));
            table.Columns.Add("Affinity", typeof(string));
            table.Columns.Add("EPN", typeof(string));
            table.Columns.Add("CollectionCount", typeof(string));
            table.Columns.Add("CommencementDate", typeof(DateTime));
            table.Columns.Add("MonthlyPremiumCurrent", typeof(string));
            table.Columns.Add("AnnualPremiumCurrent", typeof(string));
            table.Columns.Add("MonthlyPremiumOverThreshold", typeof(string));
            table.Columns.Add("AnnualPremiumOverThreshold", typeof(string));
            table.Columns.Add("ProductGroup", typeof(string));
            table.Columns.Add("ProductStatus", typeof(string));
            table.Columns.Add("ReplacementIndicator", typeof(string));
            table.Columns.Add("GrossIncome", typeof(string));
            table.Columns.Add("HouseholdIncome", typeof(string));
            table.Columns.Add("LineCount", typeof(string));
            table.Columns.Add("PreviousEPN", typeof(string));
            table.Columns.Add("PreviousEPNAddedBy", typeof(string));
            table.Columns.Add("PreviousEPNAddedOn", typeof(string));
            table.Columns.Add("PreviousEPNNotes", typeof(string));
            table.Columns.Add("PreviousRSAID", typeof(string));
            table.Columns.Add("PreviousRSAIDAddedBy", typeof(string));
            table.Columns.Add("PreviousRSAIDAddedOn", typeof(string));
            table.Columns.Add("PreviousRSAIDNotes", typeof(string));
            table.Columns.Add("CountAcrossAffinitie", typeof(string));
            table.Columns.Add("ClientGroup", typeof(string));
            table.Columns.Add("MobileNumber", typeof(string));
            table.Columns.Add("EmailAddress", typeof(string));
            table.Columns.Add("Priority", typeof(int));
            table.Columns.Add("AffinityCode", typeof(int));
            table.Columns.Add("Work", typeof(int));
            table.Columns.Add("Home", typeof(int));
            return table;
        }
        #endregion

        //----------------------------------------------------------------//

        #region [ Column Table Missed ]
        public DataTable ColumnTableMissed(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("AffinityCode", typeof(string));
            table.Columns.Add("InforceStatus", typeof(string));
            table.Columns.Add("Priority", typeof(string));
            table.Columns.Add("MobilePhone", typeof(string));
            table.Columns.Add("Telephone1", typeof(string));
            table.Columns.Add("Telephone2", typeof(string));
            table.Columns.Add("Title", typeof(string));
            table.Columns.Add("FirstName", typeof(string));
            table.Columns.Add("LastName", typeof(string));
            table.Columns.Add("RSAID", typeof(string));
            table.Columns.Add("ClientSegementCode", typeof(string));
            table.Columns.Add("PolicyNumberFromDesciption", typeof(string));
            table.Columns.Add("ProductFromDesciption", typeof(string));
            table.Columns.Add("ProcessingDateFromDesciption", typeof(string));
            table.Columns.Add("PremiumAmountFromDesciption", typeof(string));
            table.Columns.Add("RejectionReasonFromDesciption", typeof(string));
            table.Columns.Add("FirstDebitOrderIndicatorFromDesciption", typeof(string));
            table.Columns.Add("Prd1PolicyProductNumber", typeof(string));
            table.Columns.Add("Prd1TaskGUID", typeof(string));
            table.Columns.Add("Prd1InforceStatus", typeof(string));
            table.Columns.Add("Prd1Priority", typeof(string));
            table.Columns.Add("Prd1BankAccountName", typeof(string));
            table.Columns.Add("Prd1BankAccountNumber", typeof(string));
            table.Columns.Add("Prd1BankBranchName", typeof(string));
            table.Columns.Add("Prd1BankName", typeof(string));
            table.Columns.Add("Prd1PolicyNumber", typeof(string));
            table.Columns.Add("Prd1CollectionCount", typeof(string));
            table.Columns.Add("Prd1DebitOrderDay", typeof(string));
            table.Columns.Add("Prd1EPN", typeof(string));
            table.Columns.Add("Prd1ProductName", typeof(string));
            table.Columns.Add("Prd1TaskNumber", typeof(string));
            table.Columns.Add("Prd1SkippedMonthAllowed", typeof(string));
            table.Columns.Add("Prd1InforceMonthCount", typeof(string));
            table.Columns.Add("Prd1MonthlyPremium", typeof(string));
            table.Columns.Add("Prd1TotalSumInsured", typeof(string));
            table.Columns.Add("Prd1InforceMonthCountBand", typeof(string));
            table.Columns.Add("Prd1NewExisting", typeof(string));
            table.Columns.Add("Prd1ProductCategory", typeof(string));
            table.Columns.Add("Prd1TaskSubType", typeof(string));
            table.Columns.Add("Prd1LastCoverMonthMissedPayments", typeof(string));
            table.Columns.Add("Prd1CollectionCountOverInforceMonths", typeof(string));
            table.Columns.Add("Prd1ACCAUpfrontRejection", typeof(string));
            table.Columns.Add("Prd1ACCAPendingAction", typeof(string));
            table.Columns.Add("Prd1CalcPriority", typeof(string));
            table.Columns.Add("Prd1Service", typeof(string));
            table.Columns.Add("Prd1Profile", typeof(string));
            table.Columns.Add("Prd1CoverMonth", typeof(string));
            table.Columns.Add("Prd2PolicyProductNumber", typeof(string));
            table.Columns.Add("Prd2TaskGUID", typeof(string));
            table.Columns.Add("Prd2InforceStatus", typeof(string));
            table.Columns.Add("Prd2Priority", typeof(string));
            table.Columns.Add("Prd2BankAccountName", typeof(string));
            table.Columns.Add("Prd2BankAccountNumber", typeof(string));
            table.Columns.Add("Prd2BankBranchName", typeof(string));
            table.Columns.Add("Prd2BankName", typeof(string));
            table.Columns.Add("Prd2PolicyNumber", typeof(string));
            table.Columns.Add("Prd2CollectionCount", typeof(string));
            table.Columns.Add("Prd2DebitOrderDay", typeof(string));
            table.Columns.Add("Prd2EPN", typeof(string));
            table.Columns.Add("Prd2ProductName", typeof(string));
            table.Columns.Add("Prd2TaskNumber", typeof(string));
            table.Columns.Add("Prd2SkippedMonthAllowed", typeof(string));
            table.Columns.Add("Prd2InforceMonthCount", typeof(string));
            table.Columns.Add("Prd2MonthlyPremium", typeof(string));
            table.Columns.Add("Prd2TotalSumInsured", typeof(string));
            table.Columns.Add("Prd2InforceMonthCountBand", typeof(string));
            table.Columns.Add("Prd2NewExisting", typeof(string));
            table.Columns.Add("Prd2ProductCategory", typeof(string));
            table.Columns.Add("Prd2TaskSubType", typeof(string));
            table.Columns.Add("Prd2LastCoverMonthMissedPayments", typeof(string));
            table.Columns.Add("Prd2CollectionCountOverInforceMonths", typeof(string));
            table.Columns.Add("Prd2ACCAUpfrontRejection", typeof(string));
            table.Columns.Add("Prd2ACCAPendingAction", typeof(string));
            table.Columns.Add("Prd2CalcPriority", typeof(string));
            table.Columns.Add("Prd2Service", typeof(string));
            table.Columns.Add("Prd2Profile", typeof(string));
            table.Columns.Add("Prd2CoverMonth", typeof(string));
            table.Columns.Add("Prd3PolicyProductNumber", typeof(string));
            table.Columns.Add("Prd3TaskGUID", typeof(string));
            table.Columns.Add("Prd3InforceStatus", typeof(string));
            table.Columns.Add("Prd3Priority", typeof(string));
            table.Columns.Add("Prd3BankAccountName", typeof(string));
            table.Columns.Add("Prd3BankAccountNumber", typeof(string));
            table.Columns.Add("Prd3BankBranchName", typeof(string));
            table.Columns.Add("Prd3BankName", typeof(string));
            table.Columns.Add("Prd3PolicyNumber", typeof(string));
            table.Columns.Add("Prd3CollectionCount", typeof(string));
            table.Columns.Add("Prd3DebitOrderDay", typeof(string));
            table.Columns.Add("Prd3EPN", typeof(string));
            table.Columns.Add("Prd3ProductName", typeof(string));
            table.Columns.Add("Prd3TaskNumber", typeof(string));
            table.Columns.Add("Prd3SkippedMonthAllowed", typeof(string));
            table.Columns.Add("Prd3InforceMonthCount", typeof(string));
            table.Columns.Add("Prd3MonthlyPremium", typeof(string));
            table.Columns.Add("Prd3TotalSumInsured", typeof(string));
            table.Columns.Add("Prd3InforceMonthCountBand", typeof(string));
            table.Columns.Add("Prd3NewExisting", typeof(string));
            table.Columns.Add("Prd3ProductCategory", typeof(string));
            table.Columns.Add("Prd3TaskSubType", typeof(string));
            table.Columns.Add("Prd3LastCoverMonthMissedPayments", typeof(string));
            table.Columns.Add("Prd3CollectionCountOverInforceMonths", typeof(string));
            table.Columns.Add("Prd3ACCAUpfrontRejection", typeof(string));
            table.Columns.Add("Prd3ACCAPendingAction", typeof(string));
            table.Columns.Add("Prd3CalcPriority", typeof(string));
            table.Columns.Add("Prd3Service", typeof(string));
            table.Columns.Add("Prd3Profile", typeof(string));
            table.Columns.Add("Prd3CoverMonth", typeof(string));
            table.Columns.Add("Prd4PolicyProductNumber", typeof(string));
            table.Columns.Add("Prd4TaskGUID", typeof(string));
            table.Columns.Add("Prd4InforceStatus", typeof(string));
            table.Columns.Add("Prd4Priority", typeof(string));
            table.Columns.Add("Prd4BankAccountName", typeof(string));
            table.Columns.Add("Prd4BankAccountNumber", typeof(string));
            table.Columns.Add("Prd4BankBranchName", typeof(string));
            table.Columns.Add("Prd4BankName", typeof(string));
            table.Columns.Add("Prd4PolicyNumber", typeof(string));
            table.Columns.Add("Prd4CollectionCount", typeof(string));
            table.Columns.Add("Prd4DebitOrderDay", typeof(string));
            table.Columns.Add("Prd4EPN", typeof(string));
            table.Columns.Add("Prd4ProductName", typeof(string));
            table.Columns.Add("Prd4TaskNumber", typeof(string));
            table.Columns.Add("Prd4SkippedMonthAllowed", typeof(string));
            table.Columns.Add("Prd4InforceMonthCount", typeof(string));
            table.Columns.Add("Prd4MonthlyPremium", typeof(string));
            table.Columns.Add("Prd4TotalSumInsured", typeof(string));
            table.Columns.Add("Prd4InforceMonthCountBand", typeof(string));
            table.Columns.Add("Prd4NewExisting", typeof(string));
            table.Columns.Add("Prd4ProductCategory", typeof(string));
            table.Columns.Add("Prd4TaskSubType", typeof(string));
            table.Columns.Add("Prd4LastCoverMonthMissedPayments", typeof(string));
            table.Columns.Add("Prd4CollectionCountOverInforceMonths", typeof(string));
            table.Columns.Add("Prd4ACCAUpfrontRejection", typeof(string));
            table.Columns.Add("Prd4ACCAPendingAction", typeof(string));
            table.Columns.Add("Prd4CalcPriority", typeof(string));
            table.Columns.Add("Prd4Service", typeof(string));
            table.Columns.Add("Prd4Profile", typeof(string));
            table.Columns.Add("Prd4CoverMonth", typeof(string));
            table.Columns.Add("Prd5PolicyProductNumber", typeof(string));
            table.Columns.Add("Prd5TaskGUID", typeof(string));
            table.Columns.Add("Prd5InforceStatus", typeof(string));
            table.Columns.Add("Prd5Priority", typeof(string));
            table.Columns.Add("Prd5BankAccountName", typeof(string));
            table.Columns.Add("Prd5BankAccountNumber", typeof(string));
            table.Columns.Add("Prd5BankBranchName", typeof(string));
            table.Columns.Add("Prd5BankName", typeof(string));
            table.Columns.Add("Prd5PolicyNumber", typeof(string));
            table.Columns.Add("Prd5CollectionCount", typeof(string));
            table.Columns.Add("Prd5DebitOrderDay", typeof(string));
            table.Columns.Add("Prd5EPN", typeof(string));
            table.Columns.Add("Prd5ProductName", typeof(string));
            table.Columns.Add("Prd5TaskNumber", typeof(string));
            table.Columns.Add("Prd5SkippedMonthAllowed", typeof(string));
            table.Columns.Add("Prd5InforceMonthCount", typeof(string));
            table.Columns.Add("Prd5MonthlyPremium", typeof(string));
            table.Columns.Add("Prd5TotalSumInsured", typeof(string));
            table.Columns.Add("Prd5InforceMonthCountBand", typeof(string));
            table.Columns.Add("Prd5NewExisting", typeof(string));
            table.Columns.Add("Prd5ProductCategory", typeof(string));
            table.Columns.Add("Prd5TaskSubType", typeof(string));
            table.Columns.Add("Prd5LastCoverMonthMissedPayments", typeof(string));
            table.Columns.Add("Prd5CollectionCountOverInforceMonths", typeof(string));
            table.Columns.Add("Prd5ACCAUpfrontRejection", typeof(string));
            table.Columns.Add("Prd5ACCAPendingAction", typeof(string));
            table.Columns.Add("Prd5CalcPriority", typeof(string));
            table.Columns.Add("Prd5Service", typeof(string));
            table.Columns.Add("Prd5Profile", typeof(string));
            table.Columns.Add("Prd5CoverMonth", typeof(string));
            table.Columns.Add("Prd6PolicyProductNumber", typeof(string));
            table.Columns.Add("Prd6TaskGUID", typeof(string));
            table.Columns.Add("Prd6InforceStatus", typeof(string));
            table.Columns.Add("Prd6Priority", typeof(string));
            table.Columns.Add("Prd6BankAccountName", typeof(string));
            table.Columns.Add("Prd6BankAccountNumber", typeof(string));
            table.Columns.Add("Prd6BankBranchName", typeof(string));
            table.Columns.Add("Prd6BankName", typeof(string));
            table.Columns.Add("Prd6PolicyNumber", typeof(string));
            table.Columns.Add("Prd6CollectionCount", typeof(string));
            table.Columns.Add("Prd6DebitOrderDay", typeof(string));
            table.Columns.Add("Prd6EPN", typeof(string));
            table.Columns.Add("Prd6ProductName", typeof(string));
            table.Columns.Add("Prd6TaskNumber", typeof(string));
            table.Columns.Add("Prd6SkippedMonthAllowed", typeof(string));
            table.Columns.Add("Prd6InforceMonthCount", typeof(string));
            table.Columns.Add("Prd6MonthlyPremium", typeof(string));
            table.Columns.Add("Prd6TotalSumInsured", typeof(string));
            table.Columns.Add("Prd6InforceMonthCountBand", typeof(string));
            table.Columns.Add("Prd6NewExisting", typeof(string));
            table.Columns.Add("Prd6ProductCategory", typeof(string));
            table.Columns.Add("Prd6TaskSubType", typeof(string));
            table.Columns.Add("Prd6LastCoverMonthMissedPayments", typeof(string));
            table.Columns.Add("Prd6CollectionCountOverInforceMonths", typeof(string));
            table.Columns.Add("Prd6ACCAUpfrontRejection", typeof(string));
            table.Columns.Add("Prd6ACCAPendingAction", typeof(string));
            table.Columns.Add("Prd6CalcPriority", typeof(string));
            table.Columns.Add("Prd6Service", typeof(string));
            table.Columns.Add("Prd6Profile", typeof(string));
            table.Columns.Add("Prd6CoverMonth", typeof(string));
            table.Columns.Add("TaskGUIDCount", typeof(string));
            table.Columns.Add("TaskNumberCount", typeof(string));
            table.Columns.Add("SourceID", typeof(string));

            return table;
        }
        #endregion

        #region [ Column Table Missed Script ]
        public DataTable ColumnTableMissedScript(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("LOGIN", typeof(int));
            table.Columns.Add("QCODE", typeof(int));
            table.Columns.Add("STATION", typeof(int));
            table.Columns.Add("RDATE", typeof(DateTime));
            table.Columns.Add("LOADID", typeof(int));
            table.Columns.Add("SERVICEID", typeof(int));
            table.Columns.Add("PARENTSCRIPTID", typeof(int));
            table.Columns.Add("PARENTRECID", typeof(int));
            table.Columns.Add("Customer_Intro", typeof(string));
            table.Columns.Add("RetentionsInformations", typeof(string));
            table.Columns.Add("Authentication", typeof(string));
            table.Columns.Add("Mobile_Cell", typeof(int));
            table.Columns.Add("Mobile_Work", typeof(int));
            table.Columns.Add("Mobile_Home", typeof(int));
            table.Columns.Add("Sourceid", typeof(int));
            table.Columns.Add("ClientsInfo2", typeof(string));
            table.Columns.Add("LastName", typeof(string));
            table.Columns.Add("Subject", typeof(string));
            table.Columns.Add("Priority", typeof(int));
            table.Columns.Add("InforceStatus", typeof(string));
            table.Columns.Add("BankName", typeof(string));
            table.Columns.Add("BankBranchName", typeof(string));
            table.Columns.Add("FRANK", typeof(string));
            table.Columns.Add("SBSA", typeof(string));
            table.Columns.Add("VODACOM", typeof(string));
            table.Columns.Add("SBSAClientInfo", typeof(string));
            table.Columns.Add("AffinityCode", typeof(string));
            table.Columns.Add("FirstName", typeof(string));
            table.Columns.Add("BankAccountNumber", typeof(string));
            table.Columns.Add("RSAID", typeof(string));
            table.Columns.Add("Task1", typeof(string));
            table.Columns.Add("Outcome", typeof(string));
            table.Columns.Add("Task2", typeof(string));
            table.Columns.Add("Outcome2", typeof(string));
            table.Columns.Add("Task3", typeof(string));
            table.Columns.Add("Outcome3", typeof(string));
            table.Columns.Add("Task4", typeof(string));
            table.Columns.Add("Outcome4", typeof(string));
            table.Columns.Add("BankAccountName", typeof(string));
            table.Columns.Add("NameofBank", typeof(string));
            table.Columns.Add("DebitOrderDate", typeof(string));
            table.Columns.Add("PremiumAmount", typeof(string));
            table.Columns.Add("NumberOfTasks", typeof(string));
            table.Columns.Add("Task5", typeof(string));
            table.Columns.Add("Task6", typeof(string));
            table.Columns.Add("Outcome5", typeof(string));
            table.Columns.Add("Outcome6", typeof(string));
            table.Columns.Add("RejectionReasonFromDesciption", typeof(string));
            table.Columns.Add("Title", typeof(string));
            table.Columns.Add("Month1", typeof(string));
            table.Columns.Add("Month2", typeof(string));
            table.Columns.Add("Month3", typeof(string));
            table.Columns.Add("Month4", typeof(string));
            table.Columns.Add("Month5", typeof(string));
            table.Columns.Add("Month6", typeof(string));
            table.Columns.Add("Prd1EPN", typeof(string));
            table.Columns.Add("Prd2EPN", typeof(string));
            table.Columns.Add("Prd3EPN", typeof(string));
            table.Columns.Add("Prd4EPN", typeof(string));
            table.Columns.Add("Prd5EPN", typeof(string));
            table.Columns.Add("Prd6EPN", typeof(string));
            table.Columns.Add("ImportantInfo", typeof(string));
            table.Columns.Add("DurationOnBook", typeof(string));
            table.Columns.Add("Service1", typeof(string));
            table.Columns.Add("Prd1NewExisting", typeof(string));
            table.Columns.Add("Prd1Service", typeof(string));
            table.Columns.Add("Prd2Service", typeof(string));
            table.Columns.Add("Prd3Service", typeof(string));
            table.Columns.Add("Prd4Service", typeof(string));
            table.Columns.Add("Prd5Service", typeof(string));
            table.Columns.Add("Prd6Service", typeof(string));
            table.Columns.Add("PolicyNumberFromDescription", typeof(string));
            table.Columns.Add("Prd1MonthCountBand", typeof(string));
            table.Columns.Add("Prd1ProductName", typeof(string));
            table.Columns.Add("Prd1Profile", typeof(string));
            table.Columns.Add("Prd2Profile", typeof(string));
            table.Columns.Add("Prd3Profile", typeof(string));
            table.Columns.Add("Prd4Profile", typeof(string));
            table.Columns.Add("Prd5Profile", typeof(string));
            table.Columns.Add("Prd6Profile", typeof(string));
            table.Columns.Add("Prd1TaskSubType", typeof(string));

            return table;
        }
        #endregion

        #region [ Column Table BMI SMS ]
        public DataTable ColumnTableBMISMS(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("Source_ID", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("IDNumer", typeof(string));
            table.Columns.Add("CellNumber", typeof(string));
            table.Columns.Add("Message", typeof(string));
            table.Columns.Add("DOB", typeof(string));
            return table;
        }
        #endregion

        #region [ Column Table QLINK NTU ]
        public DataTable ColumnTableQLINKNTU(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("SOURCE_ID", typeof(string));
            table.Columns.Add("ID_NUMBER", typeof(string));
            table.Columns.Add("SURNAME", typeof(string));
            table.Columns.Add("NAME", typeof(string));
            table.Columns.Add("CELL_NUMBER", typeof(string));
            return table;
        }
        #endregion

        #region [ Column Table QLINK NTU ]
        public DataTable ColumnTableQLINKLAPSE(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("SOURCE_ID", typeof(string));
            table.Columns.Add("ID_NUMBER", typeof(string));
            table.Columns.Add("SURNAME", typeof(string));
            table.Columns.Add("NAME", typeof(string));
            table.Columns.Add("CELL_NUMBER", typeof(string));
            return table;
        }
        #endregion

        #region [ Column Table QLINK NTU ]
        public DataTable ColumnTableCompassLapse(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("SOURCE_ID", typeof(string));
            table.Columns.Add("ID_NUMBER", typeof(string));
            table.Columns.Add("SURNAME", typeof(string));
            table.Columns.Add("NAME", typeof(string));
            table.Columns.Add("CELL_NUMBER", typeof(string));
            return table;
        }
        #endregion

        #region [ Column Table Quoted Hot Leads ]
        public DataTable ColumnTableQuotedHotLeads(DataTable table) //Source_ID,Name,IDNumber,CellNumber,ProductName_&_StatusDate,Premium,DOB
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("Source_ID", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("IDNumber", typeof(string));
            table.Columns.Add("CellNumber", typeof(string));
            table.Columns.Add("ProductName_&_StatusDate", typeof(string));
            table.Columns.Add("Premium", typeof(string));
            table.Columns.Add("DOB", typeof(string));
            return table;
        }
        #endregion

        #region [ Column Table ECM LAPSE ]
        public DataTable ColumnTableECMLapse(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("SOURCE_ID", typeof(string));
            table.Columns.Add("ID_NUMBER", typeof(string));
            table.Columns.Add("NAME_SURNAME", typeof(string));
            table.Columns.Add("CONTACT_NUMBER", typeof(string));
            return table;
        }
        #endregion

        #region [ Column Table ECM LAPSE ]
        public DataTable ColumnTableWCCompass(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("Contract_Number", typeof(string));
            table.Columns.Add("Customer", typeof(string));
            table.Columns.Add("Owner_Cell_No", typeof(string));
            table.Columns.Add("Owner_Home_No", typeof(string));
            table.Columns.Add("Owner_ID_No", typeof(string));
            table.Columns.Add("Number_Of_Policies", typeof(string));//Contract_Number,Customer,Owner_Cell_No,Owner_Home_No,Owner_ID_No,Number_Of_Policies
            return table;
        }
        #endregion

        #region [ Column Table ECM LAPSE ]
        public DataTable ColumnTableECMNTUPDATE(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("SOURCE_ID", typeof(string));
            table.Columns.Add("ID_NUMBER", typeof(string));
            table.Columns.Add("SURNAME", typeof(string));
            table.Columns.Add("NAME", typeof(string));
            table.Columns.Add("CELL_NUMBER", typeof(string));
            table.Columns.Add("POLICY_NUMBER", typeof(string));
            return table;
        }
        #endregion

        #region [ Column Table ECM LAPSE ]
        public DataTable ColumnTableECMNTU(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("SOURCE_ID", typeof(string));
            table.Columns.Add("ID_NUMBER", typeof(string));
            table.Columns.Add("NAME_SURNAME", typeof(string));
            table.Columns.Add("CONTACT_NUMBER", typeof(string));
            return table;
        }
        #endregion

        #region [ Column Table SMS Campaign ]
        public DataTable ColumnTableSMSCampaign(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("SOURCE_ID", typeof(string));
            table.Columns.Add("COMMENTS", typeof(string));
            table.Columns.Add("OWNID", typeof(string));
            table.Columns.Add("FIRSTNAME", typeof(string));
            table.Columns.Add("SURNAME", typeof(string));
            table.Columns.Add("PHONE", typeof(string));
            table.Columns.Add("PRODUCT_DESC", typeof(string));
            table.Columns.Add("PREMIUM", typeof(string));
            table.Columns.Add("AMOUNT", typeof(string));
            table.Columns.Add("COLLECTION_DATE", typeof(string));
            table.Columns.Add("RD_DATE", typeof(string));
            table.Columns.Add("ERR_CODE", typeof(string));
            table.Columns.Add("ERR_DESC", typeof(string));
            table.Columns.Add("BRANCH", typeof(string));
            table.Columns.Add("BRANCH_NAME", typeof(string));
            return table;
        }
        #endregion

        #region [ Column Table QLINK NTU ]
        public DataTable ColumnTableCompassNTU(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("SOURCE_ID", typeof(string));
            table.Columns.Add("ID_NUMBER", typeof(string));
            table.Columns.Add("SURNAME", typeof(string));
            table.Columns.Add("NAME", typeof(string));
            table.Columns.Add("CELL_NUMBER", typeof(string));
            return table;
        }
        #endregion

        #region [ Column Table DFS Referrals ]
        public DataTable ColumnTableDFSReferrals(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("Source_ID", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("IDNumber", typeof(string));
            table.Columns.Add("CellNumber", typeof(string));
            table.Columns.Add("Message", typeof(string));
            table.Columns.Add("DOB", typeof(string));
            return table;
        }
        #endregion

        #region [ Column Table Blue Label ]
        public DataTable ColumnTableBlueLabel(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("SOURCE_ID", typeof(string));
            table.Columns.Add("NAME", typeof(string));
            table.Columns.Add("SURNAME", typeof(string));
            table.Columns.Add("ID_NUMBER", typeof(string));
            table.Columns.Add("CELL_NUMBER", typeof(string));
            return table;
        }
        #endregion

        #region [ Column Table TSF Referrals ]
        public DataTable ColumnTableTSFReferrals(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("SOURCE_ID", typeof(string));
            table.Columns.Add("NAME", typeof(string));
            table.Columns.Add("PHONE", typeof(string));
            table.Columns.Add("LOGIN", typeof(string));
            table.Columns.Add("SCHEDULE", typeof(string));
            return table;
        }
        #endregion

        #region [ Column Table BMI SMS ]
        public DataTable ColumnTableRecoveriesSMS(DataTable table) //Source_ID;Name;IDNumber;CellNumber;ProductName_&_StatusDate;Premium;DOB;SERVICEID
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("Source_ID", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("IDNumber", typeof(string));
            table.Columns.Add("CellNumber", typeof(string));
            table.Columns.Add("ProductName_&_StatusDate", typeof(string));
            table.Columns.Add("Premium", typeof(string));
            table.Columns.Add("DOB", typeof(string));
            table.Columns.Add("SERVICEID", typeof(string));
            return table;
        }
        #endregion

        #region [ Column Table Please Call Me ]
        public DataTable ColumnTablePleaseCallMe(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("Source_ID", typeof(string));
            table.Columns.Add("CellNumber", typeof(string));
            table.Columns.Add("Message1", typeof(string));
            table.Columns.Add("Message2", typeof(string));
            return table;
        }
        #endregion

        #region [ Column Table Olico Moneyshop ]
        public DataTable ColumnTableOlicoMoneyshop(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("Source_ID", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("IDNumber", typeof(string));
            table.Columns.Add("CellNumber", typeof(string));
            table.Columns.Add("Message", typeof(string));
            table.Columns.Add("DOB", typeof(string));
            return table;
        }
        #endregion

        #region [ Column Table DFS DNQ YES ]
        public DataTable ColumnTableDFSDNQYES(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("Source_ID", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("IDNumber", typeof(string));
            table.Columns.Add("CellNumber", typeof(string));
            table.Columns.Add("DOB", typeof(string));
            return table;
        }
        #endregion

        #region [ Column Table DFS DNQ DEL ]
        public DataTable ColumnTableDFSDNQDEL(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("Source_ID", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("IDNumber", typeof(string));
            table.Columns.Add("CellNumber", typeof(string));
            table.Columns.Add("DOB", typeof(string));
            return table;
        }
        #endregion

        #region [ Column Table BMI WEB ]
        public DataTable ColumnTableBMIWeb(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("Source_ID", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("CellNumber", typeof(string));
            table.Columns.Add("Message", typeof(string));

            return table;
        }
        #endregion

        #region [ Column Table BMI AVM ]
        public DataTable ColumnTableBMIAVM(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("Source_ID", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("IDNumber", typeof(string));
            table.Columns.Add("CellNumber", typeof(string));

            return table;
        }
        #endregion

        //----------------------------------------------------------------//

        #region [ Column Table Standard ]
        public DataTable ColumnTableStandard(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("InstanceID", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("FileLine", typeof(int));
            table.Columns.Add("InputServiceID", typeof(int));
            table.Columns.Add("InputLoadID", typeof(int));
            table.Columns.Add("InputSourceID", typeof(int));
            table.Columns.Add("IsFileDuplicate", typeof(string));
            table.Columns.Add("IsPresenceDuplicate", typeof(string));
            table.Columns.Add("IsScriptDuplicate", typeof(string));
            table.Columns.Add("MapPresenceFail", typeof(string));
            table.Columns.Add("MapScriptFail", typeof(string));
            table.Columns.Add("PhoneStatus", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("IDN", typeof(string));
            table.Columns.Add("Mobile", typeof(string));
            table.Columns.Add("HomePhone", typeof(string));
            table.Columns.Add("WorkPhone", typeof(string));
            table.Columns.Add("SourceID", typeof(string));
            table.Columns.Add("SourceCampaign", typeof(string));
            table.Columns.Add("Batch", typeof(string));

            return table;
        }
        #endregion

        //----------------------------------------------------------------//

        #region [ Column Table Presence ]
        public DataTable ColumnTablePresence(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("SERVICEID", typeof(int));
            table.Columns.Add("NAME", typeof(string));
            table.Columns.Add("PHONE", typeof(string));
            table.Columns.Add("CALLINGHOURS", typeof(string));
            table.Columns.Add("SOURCEID", typeof(int));
            table.Columns.Add("STATUS", typeof(int));
            table.Columns.Add("SCHEDULETYPE", typeof(char));
            table.Columns.Add("SCHEDULEDATE", typeof(DateTime));
            table.Columns.Add("LOADID", typeof(int));
            table.Columns.Add("LASTAGENT", typeof(int));
            table.Columns.Add("LASTQCODE", typeof(int));
            table.Columns.Add("FIRSTHANDLINGDATE", typeof(DateTime));
            table.Columns.Add("LASTHANDLINGDATE", typeof(DateTime));
            table.Columns.Add("DAILYCOUNTER", typeof(int));
            table.Columns.Add("TOTALCOUNTER", typeof(int));
            table.Columns.Add("BUSYSIGNALCOUNTER", typeof(int));
            table.Columns.Add("NOANSWERCOUNTER", typeof(int));
            table.Columns.Add("ANSWERMACHINECOUNTER", typeof(int));
            table.Columns.Add("FAXCOUNTER", typeof(int));
            table.Columns.Add("INVGENREASONCOUNTER", typeof(int));
            table.Columns.Add("PRIORITY", typeof(int));
            table.Columns.Add("CAPTURINGAGENT", typeof(int));
            table.Columns.Add("PHONE1", typeof(string));
            table.Columns.Add("PHONE2", typeof(string));
            table.Columns.Add("PHONE3", typeof(string));
            table.Columns.Add("PHONE4", typeof(string));
            table.Columns.Add("PHONE5", typeof(string));
            table.Columns.Add("PHONE6", typeof(string));
            table.Columns.Add("PHONE7", typeof(string));
            table.Columns.Add("PHONE8", typeof(string));
            table.Columns.Add("PHONE9", typeof(string));
            table.Columns.Add("PHONE10", typeof(string));
            table.Columns.Add("PHONEDESC1", typeof(int));
            table.Columns.Add("PHONEDESC2", typeof(int));
            table.Columns.Add("PHONEDESC3", typeof(int));
            table.Columns.Add("PHONEDESC4", typeof(int));
            table.Columns.Add("PHONEDESC5", typeof(int));
            table.Columns.Add("PHONEDESC6", typeof(int));
            table.Columns.Add("PHONEDESC7", typeof(int));
            table.Columns.Add("PHONEDESC8", typeof(int));
            table.Columns.Add("PHONEDESC9", typeof(int));
            table.Columns.Add("PHONEDESC10", typeof(int));
            table.Columns.Add("PHONESTATUS1", typeof(int));
            table.Columns.Add("PHONESTATUS2", typeof(int));
            table.Columns.Add("PHONESTATUS3", typeof(int));
            table.Columns.Add("PHONESTATUS4", typeof(int));
            table.Columns.Add("PHONESTATUS5", typeof(int));
            table.Columns.Add("PHONESTATUS6", typeof(int));
            table.Columns.Add("PHONESTATUS7", typeof(int));
            table.Columns.Add("PHONESTATUS8", typeof(int));
            table.Columns.Add("PHONESTATUS9", typeof(int));
            table.Columns.Add("PHONESTATUS10", typeof(int));
            table.Columns.Add("PHONETIMEZONEID", typeof(string));
            table.Columns.Add("PHONETIMEZONEID1", typeof(string));
            table.Columns.Add("PHONETIMEZONEID2", typeof(string));
            table.Columns.Add("PHONETIMEZONEID3", typeof(string));
            table.Columns.Add("PHONETIMEZONEID4", typeof(string));
            table.Columns.Add("PHONETIMEZONEID5", typeof(string));
            table.Columns.Add("PHONETIMEZONEID6", typeof(string));
            table.Columns.Add("PHONETIMEZONEID7", typeof(string));
            table.Columns.Add("PHONETIMEZONEID8", typeof(string));
            table.Columns.Add("PHONETIMEZONEID9", typeof(string));
            table.Columns.Add("PHONETIMEZONEID10", typeof(string));
            table.Columns.Add("CURRENTPHONE", typeof(int));
            table.Columns.Add("CURRENTPHONECOUNTER", typeof(int));
            table.Columns.Add("TIMEZONEID", typeof(string));
            table.Columns.Add("COMMENTS", typeof(string));
            table.Columns.Add("CUSTOMDATA1", typeof(string));
            table.Columns.Add("CUSTOMDATA2", typeof(string));
            table.Columns.Add("CUSTOMDATA3", typeof(string));
            table.Columns.Add("CALLERID", typeof(string));
            table.Columns.Add("CALLERNAME", typeof(string));
            table.Columns.Add("LASTDATASOURCEID", typeof(string));
            table.Columns.Add("LASTDATASOURCECUSTOMERID", typeof(string));
            table.Columns.Add("RDATE", typeof(DateTime));
            return table;
        }
        #endregion

        #region [ Column Table Queue Phone Complete ]
        public DataTable ColumnTableQueuePhoneComplete(DataTable table)
        {
            table.Columns.Add("QueuePhoneCompleteID", typeof(int));
            table.Columns.Add("Command", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("InputName", typeof(string));
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("NextExecute", typeof(DateTime));
            table.Columns.Add("Actioned", typeof(DateTime));
            table.Columns.Add("RetryCount", typeof(int));
            table.Columns.Add("RetryDate", typeof(DateTime));
            table.Columns.Add("PersonID", typeof(int));
            table.Columns.Add("SourceID", typeof(int));
            table.Columns.Add("ServiceID", typeof(int));
            table.Columns.Add("LoadID", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Phone", typeof(string));
            table.Columns.Add("ScheduleDate", typeof(DateTime));
            table.Columns.Add("Priority", typeof(int));
            table.Columns.Add("CapturingAgent", typeof(int));
            table.Columns.Add("Phone01", typeof(string));
            table.Columns.Add("Phone02", typeof(string));
            table.Columns.Add("Phone03", typeof(string));
            table.Columns.Add("Phone04", typeof(string));
            table.Columns.Add("Phone05", typeof(string));
            table.Columns.Add("Phone06", typeof(string));
            table.Columns.Add("Phone07", typeof(string));
            table.Columns.Add("Phone08", typeof(string));
            table.Columns.Add("Phone09", typeof(string));
            table.Columns.Add("Phone10", typeof(string));
            table.Columns.Add("Comments", typeof(string));
            table.Columns.Add("CustomData1", typeof(string));
            table.Columns.Add("CustomData2", typeof(string));
            table.Columns.Add("CustomData3", typeof(string));
            table.Columns.Add("CallerID", typeof(string));
            table.Columns.Add("CallerName", typeof(string));

            return table;
        }
        #endregion

        #region [ Map Queue Phone Complete to DataRow ]
        public DataRow MapQPhoneComplete(QueuePhone inPhone, DataTable inTable)
        {
            try
            {
                DataRow outRow = inTable.NewRow();
                outRow["QueuePhoneCompleteID"] = DBNull.Value;
                outRow["Command"] = inPhone.Command;
                outRow["Input"] = inPhone.Input;
                outRow["InputName"] = inPhone.InputName;
                outRow["Status"] = inPhone.Status;
                outRow["Received"] = inPhone.Received;
                outRow["NextExecute"] = inPhone.NextExecute;
                outRow["Actioned"] = DateTime.Now;
                outRow["RetryCount"] = 0;
                outRow["RetryDate"] = DBNull.Value;
                outRow["PersonID"] = inPhone.PersonID;
                outRow["SourceID"] = inPhone.SourceID;
                outRow["ServiceID"] = inPhone.ServiceID;
                outRow["LoadID"] = inPhone.LoadID;
                outRow["Name"] = inPhone.Name;
                outRow["Phone"] = inPhone.Phone;
                outRow["ScheduleDate"] = DBNull.Value;
                outRow["Priority"] = inPhone.Priority;
                outRow["CapturingAgent"] = inPhone.CapturingAgent;
                outRow["Phone01"] = inPhone.Phone01;
                outRow["Phone02"] = inPhone.Phone02;
                outRow["Phone03"] = inPhone.Phone03;
                outRow["Phone04"] = inPhone.Phone04;
                outRow["Phone05"] = inPhone.Phone05;
                outRow["Phone06"] = inPhone.Phone06;
                outRow["Phone07"] = inPhone.Phone07;
                outRow["Phone08"] = inPhone.Phone08;
                outRow["Phone09"] = inPhone.Phone09;
                outRow["Phone10"] = inPhone.Phone10;
                outRow["Comments"] = inPhone.Comments;
                outRow["CustomData1"] = inPhone.CustomData1;
                outRow["CustomData2"] = inPhone.CustomData2;
                outRow["CustomData3"] = inPhone.CustomData3;
                outRow["CallerID"] = DBNull.Value;
                outRow["CallerName"] = DBNull.Value;
                return outRow;
            }
            catch (Exception ex)
            {
                return null; 
            }
        }
        #endregion

        #region [ Map Queue Phone Complete to DataRow ]
        public DataRow MapPresenceQ(QueuePhone inPhone, DataTable inTable, bool isActive)
        {
            try
            {
                DataRow outRow = inTable.NewRow();
                string Name = inPhone.Name;
                Name = Name.Length <= 40 ? Name : Name.Substring(0, 40);
                outRow["ID"] = DBNull.Value;
                outRow["SERVICEID"] = inPhone.ServiceID;
                outRow["NAME"] = Name;
                outRow["PHONE"] = inPhone.Phone;
                outRow["CALLINGHOURS"] = DBNull.Value;
                outRow["SOURCEID"] = inPhone.SourceID;
                outRow["SCHEDULETYPE"] = DBNull.Value;
                outRow["SCHEDULEDATE"] = DBNull.Value;
                outRow["LOADID"] = inPhone.LoadID;
                outRow["LASTAGENT"] = DBNull.Value;
                outRow["LASTQCODE"] = DBNull.Value;
                outRow["STATUS"] = (isActive) ? 1 : 41;
                outRow["FIRSTHANDLINGDATE"] = DBNull.Value;
                outRow["LASTHANDLINGDATE"] = DBNull.Value;
                outRow["DAILYCOUNTER"] = DBNull.Value;
                outRow["TOTALCOUNTER"] = DBNull.Value;
                outRow["BUSYSIGNALCOUNTER"] = DBNull.Value;
                outRow["NOANSWERCOUNTER"] = DBNull.Value;
                outRow["ANSWERMACHINECOUNTER"] = DBNull.Value;
                outRow["FAXCOUNTER"] = DBNull.Value;
                outRow["INVGENREASONCOUNTER"] = DBNull.Value;
                outRow["PRIORITY"] = 1;
                outRow["CAPTURINGAGENT"] = DBNull.Value;
                outRow["PHONE1"] = inPhone.Phone01;
                outRow["PHONE2"] = DBNull.Value;
                outRow["PHONE3"] = DBNull.Value;
                outRow["PHONE4"] = DBNull.Value;
                outRow["PHONE5"] = DBNull.Value;
                outRow["PHONE6"] = DBNull.Value;
                outRow["PHONE7"] = DBNull.Value;
                outRow["PHONE8"] = DBNull.Value;
                outRow["PHONE9"] = DBNull.Value;
                outRow["PHONE10"] = DBNull.Value;
                outRow["PHONEDESC1"] = "1";
                outRow["PHONEDESC2"] = "2";
                outRow["PHONEDESC3"] = "3";
                outRow["PHONEDESC4"] = "4";
                outRow["PHONEDESC5"] = "5";
                outRow["PHONEDESC6"] = "6";
                outRow["PHONEDESC7"] = "7";
                outRow["PHONEDESC8"] = "8";
                outRow["PHONEDESC9"] = "9";
                outRow["PHONEDESC10"] = "10";
                outRow["PHONESTATUS1"] = DBNull.Value;
                outRow["PHONESTATUS2"] = DBNull.Value;
                outRow["PHONESTATUS3"] = DBNull.Value;
                outRow["PHONESTATUS4"] = DBNull.Value;
                outRow["PHONESTATUS5"] = DBNull.Value;
                outRow["PHONESTATUS6"] = DBNull.Value;
                outRow["PHONESTATUS7"] = DBNull.Value;
                outRow["PHONESTATUS8"] = DBNull.Value;
                outRow["PHONESTATUS9"] = DBNull.Value;
                outRow["PHONESTATUS10"] = DBNull.Value;
                outRow["PHONETIMEZONEID"] = "Presence_Server";
                outRow["PHONETIMEZONEID1"] = "Presence_Server";
                outRow["PHONETIMEZONEID2"] = "Presence_Server";
                outRow["PHONETIMEZONEID3"] = "Presence_Server";
                outRow["PHONETIMEZONEID4"] = "Presence_Server";
                outRow["PHONETIMEZONEID5"] = "Presence_Server";
                outRow["PHONETIMEZONEID6"] = "Presence_Server";
                outRow["PHONETIMEZONEID7"] = "Presence_Server";
                outRow["PHONETIMEZONEID8"] = "Presence_Server";
                outRow["PHONETIMEZONEID9"] = "Presence_Server";
                outRow["PHONETIMEZONEID10"] = "Presence_Server";
                outRow["CURRENTPHONE"] = "0";
                outRow["CURRENTPHONECOUNTER"] = "0";
                outRow["TIMEZONEID"] = "Presence_Server";
                outRow["COMMENTS"] = DBNull.Value;
                outRow["RDATE"] = DateTime.Now;
                outRow["CUSTOMDATA1"] = inPhone.CustomData1;
                outRow["CUSTOMDATA2"] = DBNull.Value;
                outRow["CUSTOMDATA3"] = DBNull.Value;
                outRow["CALLERID"] = DBNull.Value;
                outRow["CALLERNAME"] = DBNull.Value;
                return outRow;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
    }
}
