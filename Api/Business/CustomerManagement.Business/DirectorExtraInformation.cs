using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using BaseUtility.Business;

namespace CustomerManagement.Business
{
    public class DirectorExtraInformation
    {
        #region Declarations
        private Int64 intDirectorId;
        private string strDisplayName, strGender,strReligion, strMaritalStatus, strSpouseFullName,strSMSPhone;
        private string strWebsite, strMailingAddress1, strMailingAddress2, strSpouseEmailAddress, strEmployeeLoanAccount;
        private string strNIMCID, strRiskTolerance, strDurationOfInvestment;
        private DateTime datWedAnniversaryDate;
        private bool blnOnlineRegistration, blnPEP, blnHNI, blnBVNVerified, blnAddressVerified;

        private string strSaveType;
        #endregion

        #region Properties
        public Int64 DirectorId
        {
            set { intDirectorId = value; }
            get { return intDirectorId; }
        }
        public string DisplayName
        {
            set { strDisplayName = value; }
            get { return strDisplayName; }
        }
        public string Gender
        {
            set { strGender = value; }
            get { return strGender; }
        }
        public string Religion
        {
            set { strReligion = value; }
            get { return strReligion; }
        }
        public string MaritalStatus
        {
            set { strMaritalStatus = value; }
            get { return strMaritalStatus; }
        }
        public string SpouseFullName
        {
            set { strSpouseFullName = value; }
            get { return strSpouseFullName; }
        }
        public string SMSPhone
        {
            set { strSMSPhone = value; }
            get { return strSMSPhone; }
        }
        public string Website
        {
            set { strWebsite = value; }
            get { return strWebsite; }
        }
        public string MailingAddress1
        {
            set { strMailingAddress1 = value; }
            get { return strMailingAddress1; }
        }
        public string MailingAddress2
        {
            set { strMailingAddress2 = value; }
            get { return strMailingAddress2; }
        }
        public string SpouseEmailAddress
        {
            set { strSpouseEmailAddress = value; }
            get { return strSpouseEmailAddress; }
        }
        public DateTime WedAnniversaryDate
        {
            set { datWedAnniversaryDate = value; }
            get { return datWedAnniversaryDate; }
        }

        public bool DirectCashSettlement
        {
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("DirectorExtraInformationSelectDirectCash") as SqlCommand;
                db.AddInParameter(dbCommand, "DirectorId", SqlDbType.BigInt, intDirectorId);
                var varDSC = db.ExecuteScalar(dbCommand);
                return varDSC != null && varDSC.ToString().Trim() != "" ? (bool)varDSC : false; 
            }
        }

        public bool DoNotChargeStampDuty
        {
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("DirectorExtraInformationSelectDoNotChargeStampDuty") as SqlCommand;
                db.AddInParameter(dbCommand, "DirectorId", SqlDbType.BigInt, intDirectorId);
                var varSDY = db.ExecuteScalar(dbCommand);
                return varSDY != null && varSDY.ToString().Trim() != "" ? (bool)varSDY : false;
            }
        }

        public string EmployeeLoanAccount
        {
            set { strEmployeeLoanAccount = value; }
            get { return strEmployeeLoanAccount; }
        }
        public string NIMCID
        {
            set { strNIMCID = value; }
            get { return strNIMCID; }
        }
        public string RiskTolerance
        {
            set { strRiskTolerance = value; }
            get { return strRiskTolerance; }
        }
        public string DurationOfInvestment
        {
            set { strDurationOfInvestment = value; }
            get { return strDurationOfInvestment; }
        }
        public bool OnlineRegistration
        {
            set { blnOnlineRegistration = value; }
            get { return blnOnlineRegistration; }
        }
        public bool PEP
        {
            set { blnPEP = value; }
            get { return blnPEP; }
        }
        public bool HNI
        {
            set { blnHNI = value; }
            get { return blnHNI; }
        }
        public bool BVNVerified
        {
            set { blnBVNVerified = value; }
            get { return blnBVNVerified; }
        }
        public bool AddressVerified
        {
            set { blnAddressVerified = value; }
            get { return blnAddressVerified; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        #endregion

        #region Save And Return Command
        public SqlCommand SaveCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (strSaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("DirectorExtraInformationAdd") as SqlCommand;
            }
            else if (strSaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("DirectorExtraInformationEdit") as SqlCommand;
            }
            db.AddInParameter(oCommand, "DirectorId", SqlDbType.BigInt, intDirectorId);
            db.AddInParameter(oCommand, "DisplayName", SqlDbType.VarChar, strDisplayName.Trim());
            db.AddInParameter(oCommand, "Gender", SqlDbType.VarChar, strGender.Trim());
            db.AddInParameter(oCommand, "Religion", SqlDbType.VarChar, strReligion.Trim());
            db.AddInParameter(oCommand, "MaritalStatus", SqlDbType.VarChar, strMaritalStatus.Trim());
            db.AddInParameter(oCommand, "SpouseFullName", SqlDbType.VarChar, strSpouseFullName.Trim());
            db.AddInParameter(oCommand, "SMSPhone", SqlDbType.VarChar, strSMSPhone.Trim());
            db.AddInParameter(oCommand, "Website", SqlDbType.VarChar, strWebsite.Trim());
            db.AddInParameter(oCommand, "MailingAddress1", SqlDbType.VarChar, strMailingAddress1.Trim());
            db.AddInParameter(oCommand, "MailingAddress2", SqlDbType.VarChar, strMailingAddress2.Trim());
            db.AddInParameter(oCommand, "SpouseEmailAddress", SqlDbType.VarChar, strSpouseEmailAddress.Trim());
            if (datWedAnniversaryDate != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "WedAnniversaryDate", SqlDbType.DateTime, datWedAnniversaryDate);
            }
            else
            {
                db.AddInParameter(oCommand, "WedAnniversaryDate", SqlDbType.DateTime, SqlDateTime.Null);

            }
            db.AddInParameter(oCommand, "EmployeeLoanAccount", SqlDbType.VarChar, strEmployeeLoanAccount.Trim());
            db.AddInParameter(oCommand, "NIMCID", SqlDbType.VarChar,strNIMCID.Trim());
            db.AddInParameter(oCommand, "RiskTolerance", SqlDbType.VarChar,strRiskTolerance.Trim());
            db.AddInParameter(oCommand, "DurationOfInvestment", SqlDbType.VarChar,strDurationOfInvestment.Trim());
            db.AddInParameter(oCommand, "OnlineRegistration", SqlDbType.Bit, false);
            db.AddInParameter(oCommand, "PEP", SqlDbType.Bit, blnPEP);
            db.AddInParameter(oCommand, "HNI", SqlDbType.Bit, blnHNI);
            db.AddInParameter(oCommand, "BVNVerified", SqlDbType.Bit, blnBVNVerified);
            db.AddInParameter(oCommand, "AddressVerified", SqlDbType.Bit, blnAddressVerified);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Get
        public bool GetDirectorExtraInformation()
        {
            bool blnStatus = false;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorExtraInformationSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "DirectorId", SqlDbType.BigInt, intDirectorId);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                intDirectorId = Convert.ToInt64(thisRow[0]["DirectorId"]);
                strDisplayName = thisRow[0]["DisplayName"].ToString();
                strGender = thisRow[0]["Gender"].ToString();
                strReligion = thisRow[0]["Religion"].ToString();
                strMaritalStatus = thisRow[0]["MaritalStatus"].ToString();
                strSpouseFullName = thisRow[0]["SpouseFullName"].ToString();
                strSMSPhone = thisRow[0]["SMSPhone"].ToString();
                strWebsite = thisRow[0]["Website"].ToString();
                strMailingAddress1 = thisRow[0]["MailingAddress1"].ToString();
                strMailingAddress2 = thisRow[0]["MailingAddress2"].ToString();
                strSpouseEmailAddress = thisRow[0]["SpouseEmailAddress"] != null ? thisRow[0]["SpouseEmailAddress"].ToString() : "";
                if (thisRow[0]["WedAnniversaryDate"].ToString() == "" || thisRow[0]["WedAnniversaryDate"].ToString() == null)
                {
                    datWedAnniversaryDate = DateTime.MinValue;
                }
                else
                {
                    datWedAnniversaryDate = DateTime.ParseExact(thisRow[0]["WedAnniversaryDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                }
                strEmployeeLoanAccount = thisRow[0]["EmployeeLoanAccount"] != null ? thisRow[0]["EmployeeLoanAccount"].ToString() : "";
                strNIMCID = thisRow[0]["NIMCID"] != null ? thisRow[0]["NIMCID"].ToString() : "";
                strRiskTolerance = thisRow[0]["RiskTolerance"] != null ? thisRow[0]["RiskTolerance"].ToString() : "";
                strDurationOfInvestment = thisRow[0]["DurationOfInvestment"] != null ? thisRow[0]["DurationOfInvestment"].ToString() : "";
                blnOnlineRegistration = thisRow[0]["OnlineRegistration"] != null && thisRow[0]["OnlineRegistration"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["OnlineRegistration"].ToString()) : false;
                blnPEP = thisRow[0]["PEP"] != null && thisRow[0]["PEP"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["PEP"].ToString()) : false;
                blnHNI = thisRow[0]["HNI"] != null && thisRow[0]["HNI"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["HNI"].ToString()) : false;
                blnBVNVerified = thisRow[0]["BVNVerified"] != null && thisRow[0]["BVNVerified"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["BVNVerified"].ToString()) : false;
                blnAddressVerified = thisRow[0]["AddressVerified"] != null && thisRow[0]["AddressVerified"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["AddressVerified"].ToString()) : false;

                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Get Check Employee Loan Account Exist
        public bool CheckEmployeeLoanAccountExist()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorExtraInformationSelectEmployeeLoanAccount") as SqlCommand;
            db.AddInParameter(dbCommand, "EmployeeLoanAccount", SqlDbType.VarChar,strEmployeeLoanAccount.Trim());
            var varEmployeeAccount = db.ExecuteScalar(dbCommand);
            return varEmployeeAccount != null && varEmployeeAccount.ToString().Trim() != "" ? true : false;
        }
        #endregion

        #region Get Director ID With Employee Loan Account
        public string GetDirectorIdWithEmployeeLoanAccount()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorExtraInformationSelectEmployeeLoanAccount") as SqlCommand;
            db.AddInParameter(dbCommand, "EmployeeLoanAccount", SqlDbType.VarChar, strEmployeeLoanAccount.Trim());
            var varEmployeeAccount = db.ExecuteScalar(dbCommand);
            return varEmployeeAccount != null ? varEmployeeAccount.ToString() : "";
        }
        #endregion
    }
}
