using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class Company
    {
        #region Declaration
        private Int64 intTransNo;
        private string strMemberCode, strCompanyName, strAddress1, strAddress2, strTown, strState;
        private DateTime datStartYear;
        private DateTime datEndYear;
        private string strPhone, strEmail, strFax;
        private string strWeb, strVATNo, strUserId, strDefaultBranch, strBranch;
        private string strRCNo;
        private string strSMSDefaultPhoneNo, strSMSSenderID, strSMSDefaultText;
        private string strSMSUserName, strSMSUserPassword, strSaveType;
        private string strErrMessageToReturn;
        private DateTime datEOMRunDate, datEODRunDate, datFinEOMRunDate, datFinEODRunDate;
        private DateTime datCurrentYearStartDate, datCurrentYearEndDate, datCurrentMonthDate;
        private bool blnDayIsClosed, blnMonthIsClosed, blnFinDayIsClosed, blnFinMonthIsClosed, blnYearIsClosed;
        private bool blnOpenClosedPeriod, blnCurrentYearIsClosed, blnCurrentMonthIsClosed;
        private string strFtpAddress, strFtpUserName, strFtpUserPassword;
        private int intPasswordValidityDay, intDormantValidityDay;
        public int EndYearDeadlineExtension { get; set; }
        public int UtilityBillMonthAllowance { get; set; }
        public byte[] CompanyPhoto { get; set; }
        public byte[] CompanyLogo { get; set; }

        #endregion

        #region Properties

        public Int64 TransNo
        {
            set { intTransNo = value; }
            get { return intTransNo; }
        }

        public string MemberCode
        {
            set { strMemberCode = value; }
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("CompanySelectMemberCode") as SqlCommand;
                strMemberCode = (string)db.ExecuteScalar(dbCommand).ToString();
                return strMemberCode;
            }
        }

        public string CompanyName
        {
            set { strCompanyName = value; }
            get { return strCompanyName; }
        }
        public string Address1
        {
            set { strAddress1 = value; }
            get { return strAddress1; }
        }
        public string Address2
        {
            set { strAddress2 = value; }
            get { return strAddress2; }
        }
        public string Town
        {
            set { strTown = value; }
            get { return strTown; }
        }
        public string State
        {
            set { strState = value; }
            get { return strState; }
        }

        public DateTime StartYear
        {
            set { datStartYear = value; }
            get { return datStartYear; }
        }

        public DateTime EndYear
        {
            set { datEndYear = value; }
            get { return datEndYear; }
        }


        public string Phone
        {
            set { strPhone = value; }
            get { return strPhone; }
        }
        public string Email
        {
            set { strEmail = value; }
            get { return strEmail; }
        }
        public string Fax
        {
            set { strFax = value; }
            get { return strFax; }
        }

        public string Web
        {
            set { strWeb = value; }
            get { return strWeb; }
        }
        public string VATNo
        {
            set { strVATNo = value; }
            get { return strVATNo; }
        }
        public string UserId
        {
            set { strUserId = value; }
            get { return strUserId; }
        }
        public string DefaultBranch
        {
            set { strDefaultBranch = value; }
            get { return strDefaultBranch; }
        }
        public string Branch
        {
            set { strBranch = value; }
            get { return strBranch; }
        }

        public string RCNo
        {
            set { strRCNo = value; }
            get { return strRCNo; }
        }
        public string SMSDefaultPhoneNo
        {
            set { strSMSDefaultPhoneNo = value; }
            get { return strSMSDefaultPhoneNo; }
        }

        public string SMSSenderID
        {
            set { strSMSSenderID = value; }
            get { return strSMSSenderID; }
        }

        public string SMSDefaultText
        {
            set { strSMSDefaultText = value; }
            get { return strSMSDefaultText; }
        }

        public string SMSUserName
        {
            set { strSMSUserName = value; }
            get { return strSMSUserName; }
        }
        public string SMSUserPassword
        {
            set { strSMSUserPassword = value; }
            get { return strSMSUserPassword; }
        }
        
        public string ErrMessageToReturn
        {
            set { strErrMessageToReturn = value; }
            get { return strErrMessageToReturn; }
        }
        public DateTime EOMRunDate
        {
            set { datEOMRunDate = value; }
            get { return datEOMRunDate; }
        }
        public DateTime EODRunDate
        {
            set { datEODRunDate = value; }
            get { return datEODRunDate; }
        }
        public DateTime FinEOMRunDate
        {
            set { datFinEOMRunDate = value; }
            get { return datFinEOMRunDate; }
        }
        public DateTime FinEODRunDate
        {
            set { datFinEODRunDate = value; }
            get { return datFinEODRunDate; }
        }
        public DateTime CurrentMonthDate
        {
            set { datCurrentMonthDate = value; }
            get { return datCurrentMonthDate; }
        }
        public DateTime CurrentYearStartDate
        {
            set { datCurrentYearStartDate = value; }
            get { return datCurrentYearStartDate; }
        }
        public DateTime CurrentYearEndDate
        {
            set { datCurrentYearEndDate = value; }
            get { return datCurrentYearEndDate; }
        }
        public bool DayIsClosed
        {
            set { blnDayIsClosed = value; }
            get { return blnDayIsClosed; }
        }
        public bool MonthIsClosed
        {
            set { blnMonthIsClosed = value; }
            get { return blnMonthIsClosed; }
        }
        public bool FinDayIsClosed
        {
            set { blnFinDayIsClosed = value; }
            get { return blnFinDayIsClosed; }
        }
        public bool FinMonthIsClosed
        {
            set { blnFinMonthIsClosed = value; }
            get { return blnFinMonthIsClosed; }
        }
        public bool YearIsClosed
        {
            set { blnYearIsClosed = value; }
            get { return blnYearIsClosed; }
        }
        public bool OpenClosedPeriod
        {
            set { blnOpenClosedPeriod = value; }
            get { return blnOpenClosedPeriod; }
        }
        public bool CurrentMonthIsClosed
        {
            set { blnCurrentMonthIsClosed = value; }
            get { return blnCurrentMonthIsClosed; }
        }
        public bool CurrentYearIsClosed
        {
            set { blnCurrentYearIsClosed = value; }
            get { return blnCurrentYearIsClosed; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        public string FtpAddress
        {
            set { strFtpAddress = value; }
            get { return strFtpAddress; }
        }
        public string FtpUserName
        {
            set { strFtpUserName = value; }
            get { return strFtpUserName; }
        }
        public string FtpUserPassword
        {
            set { strFtpUserPassword = value; }
            get { return strFtpUserPassword; }
        }

        public int PasswordValidityDay
        {
            set { intPasswordValidityDay = value; }
            get { return intPasswordValidityDay; }
        }

        public int DormantValidityDay
        {
            set { intDormantValidityDay = value; }
            get { return intDormantValidityDay; }
        }
        #endregion

        #region Save
        public DataGeneral.SaveStatus Save()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            if (!ChkTransNoExist())
            {
                enSaveStatus = DataGeneral.SaveStatus.NotExist;
                return enSaveStatus;
            }
            if (ChkNameExist())
            {
                enSaveStatus = DataGeneral.SaveStatus.NameExistAdd;
                return enSaveStatus;
            }

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strSaveType == "ADDS")
            {
                dbCommand = db.GetStoredProcCommand("CompanyAddNew") as SqlCommand;
            }
            else if (strSaveType == "EDIT")
            {
                dbCommand = db.GetStoredProcCommand("CompanyEdit") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            db.AddInParameter(dbCommand, "CompanyName", SqlDbType.VarChar, strCompanyName.Trim());
            db.AddInParameter(dbCommand, "Address1", SqlDbType.VarChar, strAddress1.Trim());
            db.AddInParameter(dbCommand, "Address2", SqlDbType.VarChar, strAddress2.Trim());
            db.AddInParameter(dbCommand, "Town", SqlDbType.VarChar, strTown.Trim());
            db.AddInParameter(dbCommand, "State", SqlDbType.VarChar, strState.Trim());
            if (datStartYear != DateTime.MinValue)
            {
                db.AddInParameter(dbCommand, "StartYear", SqlDbType.DateTime, datStartYear);

            }
            else
            {
                db.AddInParameter(dbCommand, "StartYear", SqlDbType.DateTime, SqlDateTime.Null);

            }
            if (datEndYear != DateTime.MinValue)
            {
                db.AddInParameter(dbCommand, "EndYear", SqlDbType.DateTime, datEndYear);

            }
            else
            {
                db.AddInParameter(dbCommand, "EndYear", SqlDbType.DateTime, SqlDateTime.Null);

            }
            db.AddInParameter(dbCommand, "Phone", SqlDbType.VarChar, strPhone.Trim());
            db.AddInParameter(dbCommand, "Email", SqlDbType.VarChar, strEmail.Trim());
            db.AddInParameter(dbCommand, "Fax", SqlDbType.VarChar, strFax.Trim());
            db.AddInParameter(dbCommand, "Web", SqlDbType.VarChar, strWeb.Trim());
            db.AddInParameter(dbCommand, "VATNo", SqlDbType.VarChar, strVATNo.Trim());
            db.AddInParameter(dbCommand, "RCNo", SqlDbType.VarChar, strRCNo.Trim());
            db.AddInParameter(dbCommand, "SMSDefaultPhoneNo", SqlDbType.VarChar, strSMSDefaultPhoneNo.Trim());
            db.AddInParameter(dbCommand, "SMSSenderID", SqlDbType.VarChar, strSMSSenderID.Trim());
            db.AddInParameter(dbCommand, "SMSDefaultText", SqlDbType.VarChar, strSMSDefaultText.Trim());
            db.AddInParameter(dbCommand, "SMSUserName", SqlDbType.VarChar, strSMSUserName.Trim());
            db.AddInParameter(dbCommand, "SMSUserPassword", SqlDbType.VarChar, strSMSUserPassword.Trim());
            if (datEOMRunDate != DateTime.MinValue)
            {
                db.AddInParameter(dbCommand, "EOMRunDate", SqlDbType.DateTime, datEOMRunDate);
            }
            else
            {
                db.AddInParameter(dbCommand, "EOMRunDate", SqlDbType.DateTime, SqlDateTime.Null);

            }
            if (datEODRunDate != DateTime.MinValue)
            {
                db.AddInParameter(dbCommand, "EODRunDate", SqlDbType.DateTime, datEODRunDate);
            }
            else
            {
                db.AddInParameter(dbCommand, "EODRunDate", SqlDbType.DateTime, SqlDateTime.Null);
            }
            db.AddInParameter(dbCommand, "PasswordValidityDay", SqlDbType.Int, intPasswordValidityDay);
            db.AddInParameter(dbCommand, "DormantValidityDay", SqlDbType.Int, intDormantValidityDay);
            db.AddInParameter(dbCommand, "EndYearDeadlineExtension", SqlDbType.Int, EndYearDeadlineExtension);
            db.AddInParameter(dbCommand, "UtilityBillMonthAllowance", SqlDbType.Int, UtilityBillMonthAllowance);
            db.AddInParameter(dbCommand, "CompanyPhoto", SqlDbType.Image, CompanyPhoto);
            db.AddInParameter(dbCommand, "CompanyLogo", SqlDbType.Image, CompanyLogo);
            db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(dbCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Check TransNo Exist
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = null;
                oCommand = db.GetStoredProcCommand("CompanyChkTransNoExist") as SqlCommand;
                db.AddInParameter(oCommand, "Transno", SqlDbType.BigInt, intTransNo);
                DataSet oDs = db.ExecuteDataSet(oCommand);
                if (oDs.Tables[0].Rows.Count > 0)
                {
                    blnStatus = true;
                }
                else
                {
                    blnStatus = false;
                }
            }
            else if (strSaveType == "ADDS")
            {
                blnStatus = true;
            }

            return blnStatus;
        }
        #endregion

        #region Check Name Exist
        public bool ChkNameExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("CompanyChkNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            db.AddInParameter(oCommand, "CompanyName", SqlDbType.VarChar, strCompanyName.Trim());
            db.AddInParameter(oCommand, "SaveType", SqlDbType.VarChar, strSaveType.Trim());
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get Company
        public bool GetCompany(int intParamTransNo)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
            dtfi.ShortDatePattern = "dd/MM/yyyy";
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanySelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, intParamTransNo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strCompanyName = thisRow[0]["Coy_Name"].ToString();
                strAddress1 = thisRow[0]["Address_1"].ToString();
                strAddress2 = thisRow[0]["Address_2"].ToString();
                strTown = thisRow[0]["Town"].ToString();
                strState = thisRow[0]["State"].ToString();
                if (thisRow[0]["Current_Date_of_Processing"] == null || thisRow[0]["Current_Date_of_Processing"].ToString().Trim() == "")
                {
                    datStartYear = DateTime.MinValue;
                }
                else
                {
                    datStartYear = DateTime.ParseExact(DateTime.Parse(thisRow[0]["Current_Date_of_Processing"].ToString()).ToString("d", dtfi), "dd/MM/yyyy", format);
                }
                if (thisRow[0]["Last_Date_of_Processing"] == null || thisRow[0]["Last_Date_of_Processing"].ToString().Trim() == "")
                {
                    datEndYear = DateTime.MinValue;
                }
                else
                {
                    datEndYear = DateTime.ParseExact(DateTime.Parse(thisRow[0]["Last_Date_of_Processing"].ToString()).ToString("d", dtfi), "dd/MM/yyyy", format);
                }
                strPhone = thisRow[0]["Phone"].ToString();
                strEmail = thisRow[0]["Email"].ToString();
                strFax = thisRow[0]["Fax"].ToString();
                strWeb = thisRow[0]["Web"].ToString();
                strVATNo = thisRow[0]["VATNo"].ToString();
                strRCNo = thisRow[0]["RCNo"].ToString();
                strSMSDefaultPhoneNo = thisRow[0]["SMSDefaultPhoneNo"].ToString();
                strSMSSenderID = thisRow[0]["SMSSenderID"].ToString();
                strSMSDefaultText = thisRow[0]["SMSDefaultText"].ToString();
                strSMSUserName = thisRow[0]["SMSUserName"].ToString();
                strSMSUserPassword = thisRow[0]["SMSUserPassword"].ToString();
                if (thisRow[0]["EOMRunDate"] == null || thisRow[0]["EOMRunDate"].ToString().Trim() == "")
                {
                    datEOMRunDate = DateTime.MinValue;
                }
                else
                {
                    datEOMRunDate = DateTime.ParseExact(DateTime.Parse(thisRow[0]["EOMRunDate"].ToString()).ToString("d", dtfi), "dd/MM/yyyy", format); 
                }
                if (thisRow[0]["EODRunDate"] == null || thisRow[0]["EODRunDate"].ToString().Trim() == "")
                {
                    datEODRunDate = DateTime.MinValue;
                }
                else
                {
                    datEODRunDate = DateTime.ParseExact(DateTime.Parse(thisRow[0]["EODRunDate"].ToString()).ToString("d", dtfi), "dd/MM/yyyy", format); 
                }
                if (thisRow[0]["FinEOMRunDate"] == null || thisRow[0]["FinEOMRunDate"].ToString().Trim() == "")
                {
                    datFinEOMRunDate = DateTime.MinValue;
                }
                else
                {
                    datFinEOMRunDate = DateTime.Parse(thisRow[0]["FinEOMRunDate"].ToString());
                }
                if (thisRow[0]["FinEODRunDate"] == null || thisRow[0]["FinEODRunDate"].ToString().Trim() == "")
                {
                    datFinEODRunDate = DateTime.MinValue;
                }
                else
                {
                    datFinEODRunDate = DateTime.Parse(thisRow[0]["FinEODRunDate"].ToString());
                }
                datCurrentMonthDate =  thisRow[0]["CurrentMonthDate"] != null && thisRow[0]["CurrentMonthDate"].ToString().Trim() != "" ? DateTime.ParseExact(DateTime.Parse(thisRow[0]["CurrentMonthDate"].ToString()).ToString("d", dtfi), "dd/MM/yyyy", format) : DateTime.MinValue;
                datCurrentYearStartDate = thisRow[0]["CurrentYearStartDate"] != null && thisRow[0]["CurrentYearStartDate"].ToString().Trim() != "" ? DateTime.ParseExact(DateTime.Parse(thisRow[0]["CurrentYearStartDate"].ToString()).ToString("d", dtfi), "dd/MM/yyyy", format) : DateTime.MinValue;
                datCurrentYearEndDate = thisRow[0]["CurrentYearEndDate"] != null && thisRow[0]["CurrentYearEndDate"].ToString().Trim() != "" ? DateTime.ParseExact(DateTime.Parse(thisRow[0]["CurrentYearEndDate"].ToString()).ToString("d", dtfi), "dd/MM/yyyy", format) : DateTime.MinValue;
                if (thisRow[0]["DayIsClosed"] == null || thisRow[0]["DayIsClosed"].ToString().Trim() == "")
                {
                    blnDayIsClosed = false;
                }
                else
                {
                    blnDayIsClosed = bool.Parse(thisRow[0]["DayIsClosed"].ToString());
                }
                if (thisRow[0]["MonthIsClosed"] == null || thisRow[0]["MonthIsClosed"].ToString().Trim() == "")
                {
                    blnMonthIsClosed = false;
                }
                else
                {
                    blnMonthIsClosed = bool.Parse(thisRow[0]["MonthIsClosed"].ToString());
                }
                if (thisRow[0]["FinDayIsClosed"] == null || thisRow[0]["FinDayIsClosed"].ToString().Trim() == "")
                {
                    blnFinDayIsClosed = false;
                }
                else
                {
                    blnFinDayIsClosed = bool.Parse(thisRow[0]["FinDayIsClosed"].ToString());
                }
                if (thisRow[0]["FinMonthIsClosed"] == null || thisRow[0]["FinMonthIsClosed"].ToString().Trim() == "")
                {
                    blnFinMonthIsClosed = false;
                }
                else
                {
                    blnFinMonthIsClosed = bool.Parse(thisRow[0]["FinMonthIsClosed"].ToString());
                }
                if (thisRow[0]["YearIsClosed"] == null || thisRow[0]["YearIsClosed"].ToString().Trim() == "")
                {
                    blnYearIsClosed = false;
                }
                else
                {
                    blnYearIsClosed = bool.Parse(thisRow[0]["YearIsClosed"].ToString());
                }
                blnOpenClosedPeriod = thisRow[0]["OpenClosedPeriod"] != null && thisRow[0]["OpenClosedPeriod"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["OpenClosedPeriod"].ToString()) : false;
                blnCurrentMonthIsClosed = thisRow[0]["CurrentMonthIsClosed"] != null && thisRow[0]["CurrentMonthIsClosed"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["CurrentMonthIsClosed"].ToString()) : false;
                blnCurrentYearIsClosed = thisRow[0]["CurrentYearIsClosed"] != null && thisRow[0]["CurrentYearIsClosed"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["CurrentYearIsClosed"].ToString()) : false;
                intPasswordValidityDay = thisRow[0]["PasswordValidityDay"] != null && thisRow[0]["PasswordValidityDay"].ToString().Trim() != "" ? Int32.Parse(thisRow[0]["PasswordValidityDay"].ToString()) : 0;
                intDormantValidityDay = thisRow[0]["DormantValidityDay"] != null && thisRow[0]["DormantValidityDay"].ToString().Trim() != "" ? Int32.Parse(thisRow[0]["DormantValidityDay"].ToString()) : 0;
                EndYearDeadlineExtension = thisRow[0]["EndYearDeadlineExtension"] != null && thisRow[0]["EndYearDeadlineExtension"].ToString().Trim() != "" ? Int32.Parse(thisRow[0]["EndYearDeadlineExtension"].ToString()) : 0;
                UtilityBillMonthAllowance = thisRow[0]["UtilityBillMonthAllowance"] != null && thisRow[0]["UtilityBillMonthAllowance"].ToString().Trim() != "" ? Int32.Parse(thisRow[0]["UtilityBillMonthAllowance"].ToString()) : 0;
                if (thisRow[0]["Photo"] != System.DBNull.Value)
                {
                    CompanyPhoto = (byte[])thisRow[0]["Photo"];
                }
                else
                {
                    CompanyPhoto = null;
                }
                if (thisRow[0]["Logo"] != System.DBNull.Value)
                {
                    CompanyLogo = (byte[])thisRow[0]["Logo"];
                }
                else
                {
                    CompanyLogo = null;
                }
                strUserId = thisRow[0]["UserId"].ToString();
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanySelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public DataGeneral.SaveStatus Delete()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("CompanyDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Get FTP Detail
        public void GetFtpDetail(int intParamTransNo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanySelectFtpDetail") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, intParamTransNo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strFtpAddress = thisRow[0]["FtpAddress"] != null ? thisRow[0]["FtpAddress"].ToString() : "";
                strFtpUserName = thisRow[0]["FtpUserName"] != null ? thisRow[0]["FtpUserName"].ToString() : "";
                strFtpUserPassword = thisRow[0]["FtpUserPassword"] != null ? thisRow[0]["FtpUserPassword"].ToString() : "";
            }
            else
            {
                strFtpAddress = "";
                strFtpUserName = "";
                strFtpUserPassword = "";
            }
        }
        #endregion

        #region Get Email Default Message
        public string GetEmailDefaultMessage(int intParamTransNo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanySelectEmailDefaultMessage") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, intParamTransNo);
            var varEmailDefaultMsg = db.ExecuteScalar(dbCommand);
            return varEmailDefaultMsg != null ? varEmailDefaultMsg.ToString() : "";
        }
        #endregion

        #region Save MemberCode
        public bool SaveMemberCode()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanySaveMemberCode") as SqlCommand;
            db.AddInParameter(dbCommand, "MemberCode", SqlDbType.VarChar, strMemberCode.Trim());
            db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(dbCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Save EndYearDeadlineExtension
        public bool SaveEndYearDeadlineExtension()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanySaveEndYearDeadlineExtension") as SqlCommand;
            db.AddInParameter(dbCommand, "EndYearDeadlineExtension", SqlDbType.Int, EndYearDeadlineExtension);
            db.ExecuteNonQuery(dbCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        //Reporting Parameter

        #region Update Portfolio Date
        public bool UpdatePortfolioDate(DateTime datPortfolioDate)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyUpdatePortfolioDate") as SqlCommand;
            if (datPortfolioDate != DateTime.MinValue)
            {
                db.AddInParameter(dbCommand, "PortfolioDate", SqlDbType.DateTime, datPortfolioDate);
            }
            else
            {
                db.AddInParameter(dbCommand, "PortfolioDate", SqlDbType.DateTime, SqlDateTime.Null);

            }
            db.ExecuteNonQuery(dbCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Update Customer Balance Date
        public bool UpdateCustBalDate(DateTime datCustBalDate, DateTime datCustBalDateFrom)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyUpdateCustBalDate") as SqlCommand;
            if (datCustBalDate != DateTime.MinValue)
            {
                db.AddInParameter(dbCommand, "CustBalDate", SqlDbType.DateTime, datCustBalDate);
            }
            else
            {
                db.AddInParameter(dbCommand, "CustBalDate", SqlDbType.DateTime, SqlDateTime.Null);

            }
            if (datCustBalDateFrom != DateTime.MinValue)
            {
                db.AddInParameter(dbCommand, "CustBalDateFrom", SqlDbType.DateTime, datCustBalDateFrom);
            }
            else
            {
                db.AddInParameter(dbCommand, "CustBalDateFrom", SqlDbType.DateTime, SqlDateTime.Null);

            }
            db.ExecuteNonQuery(dbCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion
       
        //SMS and Email Parameter

        #region Check SMS Param Is Correct
        public bool ChkSMSParamIsCorrect()
        {
            bool blnStatus = true;
            strErrMessageToReturn = "";
            if (GetCompany(GeneralFunc.CompanyNumber))
            {
                if (strSMSUserName.Trim() == "")
                {
                    strErrMessageToReturn = "SMS User Name Cannot Be Empty";
                    blnStatus = false;
                    return blnStatus;
                }
                if (SMSUserPassword.Trim() == "")
                {
                    strErrMessageToReturn = "SMS User Password Cannot Be Empty";
                    blnStatus = false;
                    return blnStatus;
                }
                if (SMSSenderID.Trim() == "")
                {
                    strErrMessageToReturn = "SMS Sender ID Cannot Be Empty";
                    blnStatus = false;
                    return blnStatus;
                }

            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        //EOD and EOM and EOY Features
        #region Get EOY Run Date
        public bool GetEOYRunDate(int intParamTransNo)
        {
            strErrMessageToReturn = "";
            bool blnStatus = false;
            IFormatProvider format = new CultureInfo("en-GB");
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
            dtfi.ShortDatePattern = "dd/MM/yyyy";

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanySelectEOYRunDate") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, intParamTransNo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                if (thisRow[0]["Current_Date_of_Processing"] == null || thisRow[0]["Current_Date_of_Processing"].ToString().Trim() == "" ||
                    thisRow[0]["Last_Date_of_Processing"] == null || thisRow[0]["Last_Date_of_Processing"].ToString() == "")
                {
                    datStartYear = DateTime.MinValue;
                    datEndYear = DateTime.MinValue;
                    EndYearDeadlineExtension = 0;
                    strErrMessageToReturn = "Start Or End Of Year Date Must Not Be Empty";
                }
                else
                {
                    datStartYear = DateTime.ParseExact(DateTime.Parse(thisRow[0]["Current_Date_of_Processing"].ToString().Trim().Substring(0, 10)).ToString("d", dtfi), "dd/MM/yyyy", format);
                    datEndYear = DateTime.ParseExact(DateTime.Parse(thisRow[0]["Last_Date_of_Processing"].ToString().Trim().Substring(0, 10)).ToString("d", dtfi), "dd/MM/yyyy", format);
                    EndYearDeadlineExtension = Convert.ToInt32(thisRow[0]["EndYearDeadlineExtension"] != null && thisRow[0]["EndYearDeadlineExtension"].ToString().Trim() != "" ? thisRow[0]["EndYearDeadlineExtension"] : 0); 
                    blnStatus = true;
                }
            }
            else
            {
                datStartYear = DateTime.MinValue;
                datEndYear = DateTime.MinValue;
                strErrMessageToReturn = "Company Information Table Not Set Or Error Recalling";
            }
            return blnStatus;
        }
        #endregion

        #region Get EOM Run Date
        public DateTime GetEOMRunDate()
        {
            strErrMessageToReturn = "";
            IFormatProvider format = new CultureInfo("en-GB");

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanySelectEOMRunDate") as SqlCommand;
            var strResult = db.ExecuteScalar(dbCommand);
            return strResult != null && strResult.ToString().Trim() != "" ? DateTime.ParseExact(strResult.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format) : DateTime.MinValue;
        }
        #endregion

        #region Get EOD Run Date
        public DateTime GetEODRunDate()
        {
            strErrMessageToReturn = "";
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanySelectEODRunDate") as SqlCommand;
            var strResult = db.ExecuteScalar(dbCommand);
            return strResult != null && strResult.ToString().Trim() != "" ? DateTime.ParseExact(strResult.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format) : DateTime.MinValue;
        }
        #endregion

        #region Update EOY Run Date and Return Command
        public SqlCommand UpdateEOYDateCommand(DateTime datEOYRunStartDateParam, DateTime datEOYRunEndDateParam)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyUpdateEOYRunDate") as SqlCommand;
            db.AddInParameter(dbCommand, "EOYRunStartDate", SqlDbType.DateTime, datEOYRunStartDateParam);
            db.AddInParameter(dbCommand, "EOYRunEndDate", SqlDbType.DateTime, datEOYRunEndDateParam);
            return dbCommand;
        }
        #endregion

        #region Update EOM Run Date And Return Command
        public SqlCommand UpdateEOMRunDateCommand(DateTime datEOMRunDateParam)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyUpdateEOMRunDate") as SqlCommand;
            db.AddInParameter(dbCommand, "EOMRunDate", SqlDbType.DateTime, datEOMRunDateParam);
            return dbCommand;
        }
        #endregion

        #region Update EOD Run Date And Return Command
        public SqlCommand UpdateEODRunDateCommand(DateTime datEODRunDateParam)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyUpdateEODRunDate") as SqlCommand;
            db.AddInParameter(dbCommand, "EODRunDate", SqlDbType.DateTime, datEODRunDateParam);
            return dbCommand;
        }
        #endregion

        #region Close EOD And Return Command
        public SqlCommand CloseEODCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyCloseDay") as SqlCommand;
            return dbCommand;
        }
        #endregion

        #region Open EOD And Return Command
        public SqlCommand OpenEODCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyOpenDay") as SqlCommand;
            return dbCommand;
        }
        #endregion

        #region Close EOM And Return Command
        public SqlCommand CloseEOMCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyCloseMonth") as SqlCommand;
            return dbCommand;
        }
        #endregion

        #region Open EOM And Return Command
        public SqlCommand OpenEOMCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyOpenMonth") as SqlCommand;
            return dbCommand;
        }
        #endregion

        #region Close EOY And Return Command
        public SqlCommand CloseEOYCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyCloseYear") as SqlCommand;
            return dbCommand;
        }
        #endregion

        #region Open EOY And Return Command
        public SqlCommand OpenEOYCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyOpenYear") as SqlCommand;
            return dbCommand;
        }
        #endregion

        #region Check Day Is Closed
        public bool CheckDayIsClosed()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyGetDayIsClosed") as SqlCommand;
            var varDayIsClosed = db.ExecuteScalar(dbCommand);
            return varDayIsClosed != null && varDayIsClosed.ToString().Trim() != "" ? bool.Parse(varDayIsClosed.ToString()) : true;
        }
        #endregion

        #region Check Month Is Closed
        public bool CheckMonthIsClosed()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyGetMonthIsClosed") as SqlCommand;
            var varMonthIsClosed = db.ExecuteScalar(dbCommand);
            return varMonthIsClosed != null && varMonthIsClosed.ToString().Trim() != "" ? bool.Parse(varMonthIsClosed.ToString()) : true;

        }
        #endregion

        #region Check Year Is Closed
        public bool CheckYearIsClosed()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyGetYearIsClosed") as SqlCommand;
            var varYearIsClosed = db.ExecuteScalar(dbCommand);
            return varYearIsClosed != null && varYearIsClosed.ToString().Trim() != "" ? bool.Parse(varYearIsClosed.ToString()) : true;
        }
        #endregion

        #region Check Open Closed Period
        public bool CheckOpenClosedPeriod()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyGetOpenClosedPeriod") as SqlCommand;
            var varOpenClosedPeriod = db.ExecuteScalar(dbCommand);
            return varOpenClosedPeriod != null && varOpenClosedPeriod.ToString().Trim() != "" ? bool.Parse(varOpenClosedPeriod.ToString()) : true;
        }
        #endregion

        #region Update Open Closed Period and Return Command
        public SqlCommand UpdateOpenClosedPeriodCommand(DateTime datNewEOYRunStartDate, DateTime datNewEOYRunEndDate, DateTime datNewEOMRunDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyUpdateOpenClosedPeriod") as SqlCommand;
            db.AddInParameter(dbCommand, "NewEOYRunStartDate", SqlDbType.DateTime, datNewEOYRunStartDate);
            db.AddInParameter(dbCommand, "NewEOYRunEndDate", SqlDbType.DateTime, datNewEOYRunEndDate);
            db.AddInParameter(dbCommand, "NewEOMRunDate", SqlDbType.DateTime, datNewEOMRunDate != DateTime.MinValue ? datNewEOMRunDate : SqlDateTime.Null);
            return dbCommand;
        }
        #endregion

        #region Update Open Closed Month Period and Return Command
        public SqlCommand UpdateOpenClosedMonthPeriodCommand(DateTime datNewEOMRunDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyUpdateOpenClosedMonthPeriod") as SqlCommand;
            db.AddInParameter(dbCommand, "NewEOMRunDate", SqlDbType.DateTime, datNewEOMRunDate);
            return dbCommand;
        }
        #endregion

        #region Update Close Closed Period and Return Command
        public SqlCommand UpdateCloseClosedPeriodCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyUpdateCloseClosedPeriod") as SqlCommand;
            return dbCommand;
        }
        #endregion

        #region Update Close Closed Month Period and Return Command
        public SqlCommand UpdateCloseClosedMonthPeriodCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyUpdateCloseClosedMonthPeriod") as SqlCommand;
            return dbCommand;
        }
        #endregion


        //Financial EOD and EOM Features

        #region Get Fin EOM Run Date
        public DateTime GetFinEOMRunDate()
        {
            strErrMessageToReturn = "";
            IFormatProvider format = new CultureInfo("en-GB");

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanySelectFinEOMRunDate") as SqlCommand;
            var strResult = db.ExecuteScalar(dbCommand);
            return strResult != null && strResult.ToString().Trim() != "" ? DateTime.ParseExact(strResult.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format) : DateTime.MinValue;

        }
        #endregion

        #region Get Fin EOD Run Date
        public DateTime GetFinEODRunDate()
        {
            strErrMessageToReturn = "";
            IFormatProvider format = new CultureInfo("en-GB");

            
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanySelectFinEODRunDate") as SqlCommand;
            var strResult = db.ExecuteScalar(dbCommand);
            return strResult != null && strResult.ToString().Trim() != "" ? DateTime.ParseExact(strResult.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format) : DateTime.MinValue;
        }
        #endregion

        #region Get Fin EOD Run Date For EOM Run
        public DateTime GetFinEODRunDateForEOMRun()
        {
            strErrMessageToReturn = "";
            IFormatProvider format = new CultureInfo("en-GB");

            
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanySelectFinEODRunDate") as SqlCommand;
            var strResult = db.ExecuteScalar(dbCommand);
            return strResult != null && strResult.ToString().Trim() != "" ? DateTime.ParseExact(strResult.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format) : DateTime.MinValue;
        }
        #endregion

        #region Update Fin EOM Run Date And Return Command
        public SqlCommand UpdateFinEOMRunDateCommand(DateTime datEOMRunDateParam)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyUpdateFinEOMRunDate") as SqlCommand;
            db.AddInParameter(dbCommand, "EOMRunDate", SqlDbType.DateTime, datEOMRunDateParam);
            return dbCommand;
        }
        #endregion

        #region Update Fin EOD Run Date And Return Command
        public SqlCommand UpdateFinEODRunDateCommand(DateTime datEODRunDateParam)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyUpdateFinEODRunDate") as SqlCommand;
            db.AddInParameter(dbCommand, "EODRunDate", SqlDbType.DateTime, datEODRunDateParam);
            return dbCommand;
        }
        #endregion

        #region Close Fin EOD And Return Command
        public SqlCommand CloseFinEODCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyFinCloseDay") as SqlCommand;
            return dbCommand;
        }
        #endregion

        #region Open Fin EOD And Return Command
        public SqlCommand OpenFinEODCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyFinOpenDay") as SqlCommand;
            return dbCommand;
        }
        #endregion

        #region Close Fin EOM And Return Command
        public SqlCommand CloseFinEOMCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyFinCloseMonth") as SqlCommand;
            return dbCommand;
        }
        #endregion

        #region Open Fin EOM And Return Command
        public SqlCommand OpenFinEOMCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyFinOpenMonth") as SqlCommand;
            return dbCommand;
        }
        #endregion

        #region Check Fin Day Is Closed
        public bool CheckFinDayIsClosed()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyGetFinDayIsClosed") as SqlCommand;
            if (db.ExecuteScalar(dbCommand) != null && db.ExecuteScalar(dbCommand).ToString().Trim() != "")
            {
                blnStatus = bool.Parse(db.ExecuteScalar(dbCommand).ToString());
            }
            return blnStatus;

        }
        #endregion

        #region Check Fin Month Is Closed
        public bool CheckFinMonthIsClosed()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CompanyGetFinMonthIsClosed") as SqlCommand;
            if (db.ExecuteScalar(dbCommand) != null && db.ExecuteScalar(dbCommand).ToString().Trim() != "")
            {
                blnStatus = bool.Parse(db.ExecuteScalar(dbCommand).ToString());
            }
            return blnStatus;

        }
        #endregion


        //Sending Notification
        
        #region Send Email Messaging To Customer
        public void SendEmailMessagingToCustomer(string strCustomerEmail,string strMessageToSend, string strSubject)
        {
            SMSEngine oSMSEngine = new SMSEngine();
            oSMSEngine.Subject = strSubject;
            oSMSEngine.BodyText = strMessageToSend;
            oSMSEngine.SendEmail(strCustomerEmail);
        }
        #endregion

        #region Send SMS Messaging To Customer
        public void SendSMSMessagingToCustomer(string strCustomerSMS,string strMessageToSent)
        {
            SMSEngine oSMSEngine = new SMSEngine();
            if (ChkSMSParamIsCorrect())
            {
                oSMSEngine.SendingSMSText = strMessageToSent;
                oSMSEngine.SendSMSMessage(strCustomerSMS);
            }
            else
            {
                new Exception (ErrMessageToReturn);
            }
        }
        #endregion
    }
}