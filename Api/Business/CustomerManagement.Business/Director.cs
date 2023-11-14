using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using HR.Business;
using Asset.Business;
using Bank.Business;
using BaseUtility.Business;
using GL.Business;

namespace CustomerManagement.Business
{
    public class Director
    {
        #region Declarations
        private Int64 intTransNo;
        private string strCustAId, strSurname, strFirstname;
        private string strOthername, strAllname, strContact, strTitle, strEmail, strCity, strPostalCode;
        private string strAddress1, strAddress2, strMobPhone,strAcctOfficer;
        private string strState, strPhone, strFax, strPOBox, strBranch, strRCNo;
        private DateTime datBirthDate, datContactDate;
        private byte[] imgSignature;
        private byte[] imgPhoto;
        private string strMotherMaidenName,strOccupation,strInvestmentObjective,strBVNNumber;
        private bool blnDeactivateSMSAlert, blnDeactivateEmailAlert,blnForeignCustomer, blnExemptStampDuty;
        private int intCountry, intNationality, intSOrigin, intClientType;
        private long lngLGA;
        private string strSaveType;
        public DataSet oDsSameFullName = null;
        #endregion

        #region Properties

        public Int64 TransNo
        {
            set { intTransNo = value; }
            get { return intTransNo; }
        }

        public DateTime BirthDate
        {
            set { datBirthDate = value; }
            get { return datBirthDate; }
        }
        public string Contact
        {
            set { strContact = value; }
            get { return strContact; }
        }
        
        public string POBox
        {
            set { strPOBox = value; }
            get { return strPOBox; }
        }
       
        public DateTime ContactDate
        {
            set { datContactDate = value; }
            get { return datContactDate; }
        }
        public byte[] Photo
        {
            set { imgPhoto = value; }
            get { return imgPhoto; }
        }
        
        public byte[] Signature
        {
            set { imgSignature = value; }
            get { return imgSignature; }
        }
        
        public string CustAID
        {
            set { strCustAId = value; }
            get { return strCustAId; }
        }

        
        public string AcctOfficer
        {
            set { strAcctOfficer = value; }
            get { return strAcctOfficer; }
        }
        public int ClientType
        {
            set { intClientType = value; }
            get { return intClientType; }
        }
        
        
        public int Nationality
        {
            set { intNationality = value; }
            get { return intNationality; }
        }
        
        
        public string Title
        {
            set { strTitle = value; }
            get { return strTitle; }
        }
        public string Surname
        {
            set { strSurname = value; }
            get { return strSurname; }
        }
        public string Firstname
        {
            set { strFirstname = value; }
            get { return strFirstname; }
        }
        public string Othername
        {
            set { strOthername = value; }
            get { return strOthername; }
        }
        public string Allname
        {
            set { strAllname = value; }
            get { return strAllname; }
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

        public string City
        {
            set { strCity = value; }
            get { return strCity; }
        }

        public string PostalCode
        {
            set { strPostalCode = value; }
            get { return strPostalCode; }
        }

        public string State
        {
            set { strState = value; }
            get { return strState; }
        }

        public int Country
        {
            set { intCountry = value; }
            get { return intCountry; }
        }

        public string MobPhone
        {
            set { strMobPhone = value; }
            get { return strMobPhone; }
        }

        public string Fax
        {
            set { strFax = value; }
            get { return strFax; }
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
        
        public long LGA
        {
            set { lngLGA = value; }
            get { return lngLGA; }
        }
        public string MotherMaidenName
        {
            set { strMotherMaidenName = value; }
            get { return strMotherMaidenName; }
        }
        public string Occupation
        {
            set { strOccupation = value; }
            get { return strOccupation; }
        }
        public string InvestmentObjective
        {
            set { strInvestmentObjective = value; }
            get { return strInvestmentObjective; }
        }
        public string BVNNumber
        {
            set { strBVNNumber = value; }
            get { return strBVNNumber; }
        }
        
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        public bool DeactivateSMSAlert
        {
            set { blnDeactivateSMSAlert = value; }
            get { return blnDeactivateSMSAlert; }
        }
        public bool DeactivateEmailAlert
        {
            set { blnDeactivateEmailAlert = value; }
            get { return blnDeactivateEmailAlert; }
        }
        public bool ForeignCustomer
        {
            set { blnForeignCustomer = value; }
            get { return blnForeignCustomer; }
        }

        public bool ExemptStampDuty
        {
            set { blnExemptStampDuty = value; }
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("DirectorSelectExemptStampDuty") as SqlCommand;
                db.AddInParameter(dbCommand, "CustAId", SqlDbType.VarChar, strCustAId.Trim());
                var varExemptDirector = db.ExecuteScalar(dbCommand);
                blnExemptStampDuty = varExemptDirector != null && varExemptDirector.ToString().Trim() != "" ? bool.Parse(varExemptDirector.ToString()) : false;
                return blnExemptStampDuty;
            }
        }
        public int SOrigin
        {
            set { intSOrigin = value; }
            get { return intSOrigin; }
        }

        
        public StringBuilder KYCErrorMessage = new StringBuilder(500);
        #endregion

        #region Save And Return Command
        public SqlCommand SaveCommand()
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "DirectorFULL";
            
            //Check That Director Has Transaction For Changing Of Branch
            //if (ChkCustBranchEditedForDirectorIdWithGLTransaction())
            //{
            //    throw new Exception("Cannot Save Change Director Branch, GL Transaction Exist For This Director");
            //}
            if (!ChkTransNoExist())
            {
                throw new Exception("Cannot Save Director Does Not Exist");
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (strSaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("DirectorAdd") as SqlCommand;
                db.AddOutParameter(oCommand, "TransNo", SqlDbType.BigInt, 8);
            }
            else if (strSaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("DirectorEdit") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt,intTransNo);
            }
            db.AddInParameter(oCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(oCommand, "MobPhone", SqlDbType.VarChar, strMobPhone.Trim());
            db.AddInParameter(oCommand, "POBox", SqlDbType.VarChar, strPOBox.Trim());
            db.AddInParameter(oCommand, "SOrigin", SqlDbType.Int, intSOrigin);
            db.AddInParameter(oCommand, "ClientType", SqlDbType.Int, intClientType);
            db.AddInParameter(oCommand, "AccountOfficer", SqlDbType.VarChar, strAcctOfficer.Trim());
            if (datBirthDate != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "BirthDate", SqlDbType.DateTime, datBirthDate);

            }
            else
            {
                db.AddInParameter(oCommand, "BirthDate", SqlDbType.DateTime, SqlDateTime.Null);

            }

            if (datContactDate != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "ContactDate", SqlDbType.DateTime, datContactDate);
            }
            else
            {
                db.AddInParameter(oCommand, "ContactDate", SqlDbType.DateTime, SqlDateTime.Null);
            }
            db.AddInParameter(oCommand, "Nationality", SqlDbType.Int, intNationality);
            db.AddInParameter(oCommand, "Contact", SqlDbType.VarChar, strContact.Trim());
            db.AddInParameter(oCommand, "Phone", SqlDbType.VarChar, strPhone.Trim());
            db.AddInParameter(oCommand, "Country", SqlDbType.Int, intCountry);
            db.AddInParameter(oCommand, "State", SqlDbType.VarChar, strState);
            db.AddInParameter(oCommand, "Surname", SqlDbType.VarChar, strSurname.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.Char, GeneralFunc.UserName.Trim());
            db.AddInParameter(oCommand, "Othername", SqlDbType.VarChar, strOthername.Trim());
            db.AddInParameter(oCommand, "Firstname", SqlDbType.VarChar, strFirstname.Trim());
            db.AddInParameter(oCommand, "Title", SqlDbType.VarChar, strTitle.Trim());
            db.AddInParameter(oCommand, "Address1", SqlDbType.Char, strAddress1.Trim());
            db.AddInParameter(oCommand, "Address2", SqlDbType.VarChar, strAddress2.Trim());
            db.AddInParameter(oCommand, "Email", SqlDbType.VarChar, strEmail.Trim());
            db.AddInParameter(oCommand, "Fax", SqlDbType.VarChar, strFax.Trim());
            db.AddInParameter(oCommand, "Photo", SqlDbType.Image, imgPhoto);
            if (imgPhoto != null)
            {
                db.AddInParameter(oCommand, "PhotoIndicator", SqlDbType.VarChar, "Y");
            }
            else
            {
                db.AddInParameter(oCommand, "PhotoIndicator", SqlDbType.VarChar, "N");
            }
            db.AddInParameter(oCommand, "Signature", SqlDbType.Image, imgSignature);
            db.AddInParameter(oCommand, "Branch", SqlDbType.VarChar, strBranch.Trim());
            db.AddInParameter(oCommand, "RCNo", SqlDbType.VarChar, strRCNo.Trim());
            db.AddInParameter(oCommand, "LGA", SqlDbType.BigInt, lngLGA);
            db.AddInParameter(oCommand, "MotherMaidenName", SqlDbType.VarChar, strMotherMaidenName.Trim());
            db.AddInParameter(oCommand, "Occupation", SqlDbType.VarChar, strOccupation.Trim());
            db.AddInParameter(oCommand, "InvestmentObjective", SqlDbType.VarChar, strInvestmentObjective.Trim());
            db.AddInParameter(oCommand, "BVNNumber", SqlDbType.VarChar, strBVNNumber.Trim());
            db.AddInParameter(oCommand, "DeactivateSMSAlert", SqlDbType.Bit, blnDeactivateSMSAlert);
            db.AddInParameter(oCommand, "DeactivateEmailAlert", SqlDbType.Bit, blnDeactivateEmailAlert);
            db.AddInParameter(oCommand, "CustomerFull", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(oCommand, "ForeignCustomer", SqlDbType.Bit, blnForeignCustomer);
            return oCommand;
        }
        #endregion

        #region Save And Return Command Temporary For Reward
        public SqlCommand SaveCommand(string strUserName)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "DirectorFULL";

            //Check That Director Has Transaction For Changing Of Branch
            //if (ChkCustBranchEditedForDirectorIdWithGLTransaction())
            //{
            //    throw new Exception("Cannot Save Change Director Branch, GL Transaction Exist For This Director");
            //}
            if (!ChkTransNoExist())
            {
                throw new Exception("Cannot Save Director Does Not Exist");
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (strSaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("DirectorAdd") as SqlCommand;
                db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "DirectorORIGINAL");
                db.AddOutParameter(oCommand, "CustAID", SqlDbType.VarChar, 8);
            }
            else if (strSaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("DirectorEdit") as SqlCommand;
                db.AddInParameter(oCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            }
            db.AddInParameter(oCommand, "MobPhone", SqlDbType.VarChar, strMobPhone.Trim());
            db.AddInParameter(oCommand, "POBox", SqlDbType.VarChar, strPOBox.Trim());
            db.AddInParameter(oCommand, "SOrigin", SqlDbType.Int, intSOrigin);
            db.AddInParameter(oCommand, "ClientType", SqlDbType.Int, intClientType);
            db.AddInParameter(oCommand, "AccountOfficer", SqlDbType.VarChar, strAcctOfficer.Trim());
            if (datBirthDate != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "BirthDate", SqlDbType.DateTime, datBirthDate);

            }
            else
            {
                db.AddInParameter(oCommand, "BirthDate", SqlDbType.DateTime, SqlDateTime.Null);

            }

            if (datContactDate != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "ContactDate", SqlDbType.DateTime, datContactDate);
            }
            else
            {
                db.AddInParameter(oCommand, "ContactDate", SqlDbType.DateTime, SqlDateTime.Null);
            }
            db.AddInParameter(oCommand, "Nationality", SqlDbType.Int, intNationality);
            db.AddInParameter(oCommand, "Contact", SqlDbType.VarChar, strContact.Trim());
            db.AddInParameter(oCommand, "Phone", SqlDbType.VarChar, strPhone.Trim());
            db.AddInParameter(oCommand, "Country", SqlDbType.Int, intCountry);
            db.AddInParameter(oCommand, "State", SqlDbType.VarChar, strState);
            db.AddInParameter(oCommand, "Surname", SqlDbType.VarChar, strSurname.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.Char, strUserName.Trim());
            db.AddInParameter(oCommand, "Othername", SqlDbType.VarChar, strOthername.Trim());
            db.AddInParameter(oCommand, "Firstname", SqlDbType.VarChar, strFirstname.Trim());
            db.AddInParameter(oCommand, "Title", SqlDbType.VarChar, strTitle.Trim());
            db.AddInParameter(oCommand, "Address1", SqlDbType.Char, strAddress1.Trim());
            db.AddInParameter(oCommand, "Address2", SqlDbType.VarChar, strAddress2.Trim());
            db.AddInParameter(oCommand, "Email", SqlDbType.VarChar, strEmail.Trim());
            db.AddInParameter(oCommand, "Fax", SqlDbType.VarChar, strFax.Trim());
            db.AddInParameter(oCommand, "Photo", SqlDbType.Image, imgPhoto);
            if (imgPhoto != null)
            {
                db.AddInParameter(oCommand, "PhotoIndicator", SqlDbType.VarChar, "Y");
            }
            else
            {
                db.AddInParameter(oCommand, "PhotoIndicator", SqlDbType.VarChar, "N");
            }
            db.AddInParameter(oCommand, "Signature", SqlDbType.Image, imgSignature);
            db.AddInParameter(oCommand, "Branch", SqlDbType.VarChar, strBranch.Trim());
            db.AddInParameter(oCommand, "RCNo", SqlDbType.VarChar, strRCNo.Trim());
            db.AddInParameter(oCommand, "LGA", SqlDbType.BigInt, lngLGA);
            db.AddInParameter(oCommand, "MotherMaidenName", SqlDbType.VarChar, strMotherMaidenName.Trim());
            db.AddInParameter(oCommand, "Occupation", SqlDbType.VarChar, strOccupation.Trim());
            db.AddInParameter(oCommand, "InvestmentObjective", SqlDbType.VarChar, strInvestmentObjective.Trim());
            db.AddInParameter(oCommand, "BVNNumber", SqlDbType.VarChar, strBVNNumber.Trim());
            db.AddInParameter(oCommand, "DeactivateSMSAlert", SqlDbType.Bit, blnDeactivateSMSAlert);
            db.AddInParameter(oCommand, "DeactivateEmailAlert", SqlDbType.Bit, blnDeactivateEmailAlert);
            db.AddInParameter(oCommand, "DirectorFull", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(oCommand, "ForeignCustomer", SqlDbType.Bit, blnForeignCustomer);
            return oCommand;
        }
        #endregion

        #region Check CustAId Exist
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("DirectorChkTransNoExist") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intTransNo);
                DataSet oDs = db.ExecuteDataSet(oCommand);
                if (oDs.Tables[0].Rows.Count > 0)
                {
                    blnStatus = true;
                }
            }
            else if (strSaveType == "ADDS")
            {
                blnStatus = true;
            }

            return blnStatus;
        }
        #endregion

        #region Check Full Name Exist Return Only One
        public bool ChkFullNameExistReturnOne(string strFullName)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("DirectorChkFullNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "FullName", SqlDbType.VarChar, strFullName.Trim());
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
                oDsSameFullName = oDs;
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion
        
        #region Get All
        public DataSet GetAll(string strCustomerNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorSelectAll") as SqlCommand;
            db.AddInParameter(dbCommand, "CustomerNumber", SqlDbType.VarChar, strCustomerNumber.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All AC Not Created
        public DataSet GetAllACNotCreate(string strProductCode)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorSelectAllACNotCreate") as SqlCommand;
            db.AddInParameter(dbCommand, "Product", SqlDbType.VarChar, strProductCode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Order By Surname
        public DataSet GetAllBySurname()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorSelectAllBySurname") as SqlCommand;
            db.AddInParameter(dbCommand, "Branch", SqlDbType.VarChar, strBranch.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Order By Surname Show Only Director Name
        public DataSet GetAllBySurnameDirectorName()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorSelectAllBySurnameDirectorName") as SqlCommand;
            db.AddInParameter(dbCommand, "Branch", SqlDbType.VarChar, strBranch.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Order By Surname - Search
        public DataSet GetAllBySurnameSearch(string strCustomerNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorSelectAllBySurnameSearch") as SqlCommand;
            db.AddInParameter(dbCommand, "Surname", SqlDbType.VarChar, strSurname.Trim());
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            db.AddInParameter(dbCommand, "OtherName", SqlDbType.VarChar, strOthername.Trim());
            db.AddInParameter(dbCommand, "FirstName", SqlDbType.VarChar, strFirstname.Trim());
            db.AddInParameter(dbCommand, "AllName", SqlDbType.VarChar, strAllname.Trim());
            db.AddInParameter(dbCommand, "Branch", SqlDbType.VarChar, strBranch.Trim());
            db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustomerNumber.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All AC Not Create Order By Surname - Search
        public DataSet GetAllACNotCreateBySurnameSearch(string strProductCode)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorSelectAllACNotCreateBySurnameSearch") as SqlCommand;
            db.AddInParameter(dbCommand, "Surname", SqlDbType.VarChar, strSurname.Trim());
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(dbCommand, "OtherName", SqlDbType.VarChar, strOthername.Trim());
            db.AddInParameter(dbCommand, "FirstName", SqlDbType.VarChar, strFirstname.Trim());
            db.AddInParameter(dbCommand, "Product", SqlDbType.VarChar, strProductCode.Trim());
            db.AddInParameter(dbCommand, "AllName", SqlDbType.VarChar, strAllname.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Birth Date For Alert Only
        public DataSet GetAllBirthDateForAlertOnly(DateTime datDirectorBirthDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorSelectAllBirthDate") as SqlCommand;
            db.AddInParameter(dbCommand, "BirthDay", SqlDbType.Int, datDirectorBirthDate.Day);
            db.AddInParameter(dbCommand, "BirthMonth", SqlDbType.Int, datDirectorBirthDate.Month);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion
        
        #region Get 
        public bool GetDirector()
        {
            bool blnStatus = false;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strCustAId = thisRow[0]["CustAID"].ToString();
                strContact = thisRow[0]["Contact"].ToString();
                strPhone = thisRow[0]["Phone"].ToString();
                intCountry = thisRow[0]["Country"] != null && thisRow[0]["Country"].ToString().Trim() != "" ? int.Parse(thisRow[0]["Country"].ToString()) : 0;
                strEmail = thisRow[0]["Email"].ToString();
                strFax = thisRow[0]["Fax"].ToString();
                strFirstname = thisRow[0]["Firstname"].ToString();
                strOthername = thisRow[0]["Othername"].ToString();
                strMobPhone = thisRow[0]["MobPhone"].ToString();
                if (thisRow[0]["AccountOfficer_ID"].ToString() == "0" || thisRow[0]["AccountOfficer_ID"].ToString() == null)
                {
                    strAcctOfficer = "";
                }
                else
                {
                    strAcctOfficer = thisRow[0]["AccountOfficer_ID"].ToString();
                }
                strPOBox = thisRow[0]["POBox"].ToString();
                strState = thisRow[0]["State"] != null ? thisRow[0]["State"].ToString() : "";
                intSOrigin = thisRow[0]["SOrigin"] != null && thisRow[0]["SOrigin"].ToString().Trim() != "" ? int.Parse(thisRow[0]["SOrigin"].ToString()) : 0;
                strAddress1 = thisRow[0]["Address1"].ToString();
                strSurname = thisRow[0]["Surname"].ToString();
                intNationality = thisRow[0]["Nationality"] != null && thisRow[0]["Nationality"].ToString().Trim() != "" ? int.Parse(thisRow[0]["Nationality"].ToString()) : 0;
                strAddress2 = thisRow[0]["Address2"].ToString();
                if (thisRow[0]["BirthDate"].ToString() == "" || thisRow[0]["BirthDate"].ToString() == null)
                {
                    datBirthDate = DateTime.MinValue;
                }
                else
                {
                    datBirthDate = DateTime.ParseExact(thisRow[0]["BirthDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                }
                if (thisRow[0]["ContactDate"].ToString() == "" || thisRow[0]["ContactDate"].ToString() == null)
                {
                    datContactDate = DateTime.MinValue;
                }
                else
                {
                    datContactDate = DateTime.ParseExact(thisRow[0]["ContactDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);

                }
                intClientType = thisRow[0]["ClientType"] != null && thisRow[0]["ClientType"].ToString().Trim() != "" ? Convert.ToInt16(thisRow[0]["ClientType"]) : 0;
                strTitle = thisRow[0]["Title"] != null ? thisRow[0]["Title"].ToString() : "";
                if (thisRow[0]["Photo"] != System.DBNull.Value)
                {
                    imgPhoto = (byte[])thisRow[0]["Photo"];
                }
                else
                {
                    imgPhoto = null;
                }
                if (thisRow[0]["Signature"] != System.DBNull.Value)
                {
                    imgSignature = (byte[])thisRow[0]["Signature"];
                }
                else
                {
                    imgSignature = null;
                }
                strBranch = thisRow[0]["Branch"].ToString();
                strRCNo = thisRow[0]["RCNo"].ToString();
                lngLGA = thisRow[0]["LGA"] != null && thisRow[0]["LGA"].ToString().Trim() != "" ? long.Parse(thisRow[0]["LGA"].ToString()) : 0;
                strMotherMaidenName = thisRow[0]["MotherMaidenName"].ToString();
                strInvestmentObjective = thisRow[0]["InvestmentObjective"].ToString();
                strBVNNumber = thisRow[0]["BVNNumber"] != null ? thisRow[0]["BVNNumber"].ToString() : "";
                strOccupation = thisRow[0]["Occupation"].ToString();
                if (thisRow[0]["DeactivateSMSAlert"] != null && thisRow[0]["DeactivateSMSAlert"].ToString().Trim() != "")
                {
                    blnDeactivateSMSAlert = bool.Parse(thisRow[0]["DeactivateSMSAlert"].ToString());
                }
                else
                {
                    blnDeactivateSMSAlert = false;
                }
                if (thisRow[0]["DeactivateEmailAlert"] != null && thisRow[0]["DeactivateEmailAlert"].ToString().Trim() != "")
                {
                    blnDeactivateEmailAlert = bool.Parse(thisRow[0]["DeactivateEmailAlert"].ToString());
                }
                else
                {
                    blnDeactivateEmailAlert = false;
                }

                if (thisRow[0]["ForeignCustomer"] != null && thisRow[0]["ForeignCustomer"].ToString().Trim() != "")
                {
                    blnForeignCustomer = bool.Parse(thisRow[0]["ForeignCustomer"].ToString());
                }
                else
                {
                    blnForeignCustomer = false;
                }
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get Approval
        public bool GetDirectorApproval(DataGeneral.PostStatus TranStatus)
        {
            bool blnStatus = false;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TranStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("DirectorSelectUnPost") as SqlCommand;
            }
            else if (TranStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("DirectorSelectPosted") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt,intTransNo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strCustAId = thisRow[0]["CustAID"].ToString();
                strContact = thisRow[0]["Contact"].ToString();
                strPhone = thisRow[0]["Phone"].ToString();
                intCountry = int.Parse(thisRow[0]["Country"] != null && thisRow[0]["Country"].ToString().Trim() != "" ? thisRow[0]["Country"].ToString() : "0");
                strEmail = thisRow[0]["Email"].ToString();
                strFax = thisRow[0]["Fax"].ToString();
                strFirstname = thisRow[0]["Firstname"].ToString();
                strOthername = thisRow[0]["Othername"].ToString();
                strMobPhone = thisRow[0]["MobPhone"].ToString();
                if (thisRow[0]["AccountOfficer_ID"].ToString() == "0" || thisRow[0]["AccountOfficer_ID"].ToString() == null)
                {
                    strAcctOfficer = "";
                }
                else
                {
                    strAcctOfficer = thisRow[0]["AccountOfficer_ID"].ToString();
                }
                strPOBox = thisRow[0]["POBox"].ToString();
                strState = thisRow[0]["State"] != null ? thisRow[0]["State"].ToString() : "";
                strAddress1 = thisRow[0]["Address1"].ToString();
                strSurname = thisRow[0]["Surname"].ToString();
                intNationality = int.Parse(thisRow[0]["Nationality"] != null && thisRow[0]["Nationality"].ToString().Trim() != "" ? thisRow[0]["Nationality"].ToString() : "0");
                strAddress2 = thisRow[0]["Address2"].ToString();
                if (thisRow[0]["BirthDate"].ToString() == "" || thisRow[0]["BirthDate"].ToString() == null)
                {
                    datBirthDate = DateTime.MinValue;
                }
                else
                {
                    datBirthDate = DateTime.ParseExact(thisRow[0]["BirthDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                }
                if (thisRow[0]["ContactDate"].ToString() == "" || thisRow[0]["ContactDate"].ToString() == null)
                {
                    datContactDate = DateTime.MinValue;
                }
                else
                {
                    datContactDate = DateTime.ParseExact(thisRow[0]["ContactDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);

                }

                intClientType = thisRow[0]["ClientType"] != null && thisRow[0]["ClientType"].ToString().Trim() != "" ? Convert.ToInt16(thisRow[0]["ClientType"]) : 0;
                strTitle = thisRow[0]["Title"].ToString();
                if (thisRow[0]["Photo"] != System.DBNull.Value)
                {
                    imgPhoto = (byte[])thisRow[0]["Photo"];
                }
                else
                {
                    imgPhoto = null;
                }
                if (thisRow[0]["Signature"] != System.DBNull.Value)
                {
                    imgSignature = (byte[])thisRow[0]["Signature"];
                }
                else
                {
                    imgSignature = null;
                }
                strBranch = thisRow[0]["Branch"].ToString();
                strRCNo = thisRow[0]["RCNo"].ToString();
                lngLGA = long.Parse(thisRow[0]["LGA"] != null && thisRow[0]["LGA"].ToString().Trim() != "" ? thisRow[0]["LGA"].ToString() : "0");
                strMotherMaidenName = thisRow[0]["MotherMaidenName"].ToString();
                strInvestmentObjective = thisRow[0]["InvestmentObjective"].ToString();
                strBVNNumber = thisRow[0]["BVNNumber"] != null ? thisRow[0]["BVNNumber"].ToString() : "";
                strOccupation = thisRow[0]["Occupation"].ToString();
                if (thisRow[0]["DeactivateSMSAlert"] != null && thisRow[0]["DeactivateSMSAlert"].ToString().Trim() != "")
                {
                    blnDeactivateSMSAlert = bool.Parse(thisRow[0]["DeactivateSMSAlert"].ToString());
                }
                else
                {
                    blnDeactivateSMSAlert = false;
                }
                if (thisRow[0]["DeactivateEmailAlert"] != null && thisRow[0]["DeactivateEmailAlert"].ToString().Trim() != "")
                {
                    blnDeactivateEmailAlert = bool.Parse(thisRow[0]["DeactivateEmailAlert"].ToString());
                }
                else
                {
                    blnDeactivateEmailAlert = false;
                }
                intSOrigin = thisRow[0]["SOrigin"] != null && thisRow[0]["SOrigin"].ToString().Trim() != "" ? int.Parse(thisRow[0]["SOrigin"].ToString()) : 0;
                blnStatus = true;

            }
            return blnStatus;
        }
        #endregion

        #region Get Director Name
        public bool GetDirectorName(string ProductCode)
        {
            Product oProduct = new Product();
            oProduct.TransNo = ProductCode;
            if (!oProduct.GetProduct())
            {
                throw new Exception("Product Does Not Exist");
            }
            bool blnStatus = false;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
           
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strFirstname = thisRow[0]["Firstname"].ToString();
                strOthername = thisRow[0]["Othername"].ToString();
                strSurname = thisRow[0]["Surname"].ToString();
                strBranch = thisRow[0]["Branch"].ToString();
                strEmail = thisRow[0]["Email"].ToString();
                strPhone = thisRow[0]["Phone"].ToString();
                strMobPhone = thisRow[0]["MobPhone"].ToString();
                strAddress1 = thisRow[0]["Address1"].ToString();
                strAddress2 = thisRow[0]["Address2"].ToString();
                intClientType = thisRow[0]["ClientType"] != null && thisRow[0]["ClientType"].ToString().Trim() != "" ? Convert.ToInt16(thisRow[0]["ClientType"]) : 0;
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get Director Name With Treasury Product
        public bool GetDirectorNameWithTreasuryProduct(string ProductCode)
        {
            TreasuryProduct oTreasuryProduct = new TreasuryProduct();
            oTreasuryProduct.ProductCode = ProductCode;
            if (!oTreasuryProduct.GetTreasuryProduct())
            {
                throw new Exception("Product Does Not Exist");
            }
            bool blnStatus = false;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;

            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strFirstname = thisRow[0]["Firstname"].ToString();
                strOthername = thisRow[0]["Othername"].ToString();
                strSurname = thisRow[0]["Surname"].ToString();
                strBranch = thisRow[0]["Branch"].ToString();
                strEmail = thisRow[0]["Email"].ToString();
                strPhone = thisRow[0]["Phone"].ToString();
                strMobPhone = thisRow[0]["MobPhone"].ToString();
                strAddress1 = thisRow[0]["Address1"].ToString();
                strAddress2 = thisRow[0]["Address2"].ToString();
                intClientType = thisRow[0]["ClientType"] != null && thisRow[0]["ClientType"].ToString().Trim() != "" ? Convert.ToInt16(thisRow[0]["ClientType"]) : 0;
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get Director Name With Loan Product
        public bool GetDirectorNameWithLoanProduct(string ProductCode)
        {
            LoanProduct oLoanProduct = new LoanProduct();
            oLoanProduct.ProductCode = ProductCode;
            if (!oLoanProduct.GetLoanProduct())
            {
                throw new Exception("Product Does Not Exist");
            }
            bool blnStatus = false;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;

            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strFirstname = thisRow[0]["Firstname"].ToString();
                strOthername = thisRow[0]["Othername"].ToString();
                strSurname = thisRow[0]["Surname"].ToString();
                strBranch = thisRow[0]["Branch"].ToString();
                strEmail = thisRow[0]["Email"].ToString();
                strPhone = thisRow[0]["Phone"].ToString();
                strMobPhone = thisRow[0]["MobPhone"].ToString();
                strAddress1 = thisRow[0]["Address1"].ToString();
                strAddress2 = thisRow[0]["Address2"].ToString();
                intClientType = thisRow[0]["ClientType"] != null && thisRow[0]["ClientType"].ToString().Trim() != "" ? Convert.ToInt16(thisRow[0]["ClientType"]) : 0;
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get Director Name Without Product
        public bool GetDirectorNameWithoutProduct()
        {
           
            bool blnStatus = false;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;

            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strFirstname = thisRow[0]["Firstname"].ToString();
                strOthername = thisRow[0]["Othername"].ToString();
                strSurname = thisRow[0]["Surname"].ToString();
                strBranch = thisRow[0]["Branch"].ToString();
                strEmail = thisRow[0]["Email"].ToString();
                strPhone = thisRow[0]["Phone"].ToString();
                strMobPhone = thisRow[0]["MobPhone"].ToString();
                strAddress1 = thisRow[0]["Address1"].ToString();
                strAddress2 = thisRow[0]["Address2"].ToString();
                intClientType = thisRow[0]["ClientType"] != null && thisRow[0]["ClientType"].ToString().Trim() != "" ? Convert.ToInt16(thisRow[0]["ClientType"]) : 0;
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get Branch Id
        public string GetBranchId()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorSelectBranchId") as SqlCommand;
            db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            var varBranchId = db.ExecuteScalar(dbCommand);
            if (varBranchId != null)
            {
                return (string)varBranchId;
            }
            else
            {
                return "";
            }

        }
        #endregion

        #region Get Director Notification Status
        public bool GetDirectorNotificationStatus()
        {
            bool blnStatus = false;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                if (thisRow[0]["DeactivateSMSAlert"] != null && thisRow[0]["DeactivateSMSAlert"].ToString().Trim() != "")
                {
                    blnDeactivateSMSAlert = bool.Parse(thisRow[0]["DeactivateSMSAlert"].ToString());
                }
                else
                {
                    blnDeactivateSMSAlert = false;
                }
                if (thisRow[0]["DeactivateEmailAlert"] != null && thisRow[0]["DeactivateEmailAlert"].ToString().Trim() != "")
                {
                    blnDeactivateEmailAlert = bool.Parse(thisRow[0]["DeactivateEmailAlert"].ToString());
                }
                else
                {
                    blnDeactivateEmailAlert = false;
                }
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get Director Table Name
        public bool GetDirectorTableName()
        {
            
            bool blnStatus = false;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strFirstname = thisRow[0]["Firstname"].ToString();
                strOthername = thisRow[0]["Othername"].ToString();
                strSurname = thisRow[0]["Surname"].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Combine Director Names
        public string CombineName()
        {
            return strSurname.Trim() + " " + strFirstname.Trim() + " " + strOthername.Trim();
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("DirectorDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar,strCustAId.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Director Approval
        public void Post()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommandBatchOwner = new SqlCommand();
            oCommandBatchOwner = db.GetStoredProcCommand("DirectorUpdatePost") as SqlCommand;
            db.AddInParameter(oCommandBatchOwner, "Code", SqlDbType.VarChar,strCustAId.Trim());
            db.AddInParameter(oCommandBatchOwner, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommandBatchOwner);
        }
        #endregion

        #region Update Director Email And Return Command
        public SqlCommand UpdateDirectorEmailCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            oCommand = db.GetStoredProcCommand("DirectorUpdateDirectorEmail") as SqlCommand;
            db.AddInParameter(oCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(oCommand, "Email", SqlDbType.VarChar, strEmail);
            return oCommand;
        }
        #endregion

        #region Delete And Return Command
        public SqlCommand DeleteCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("DirectorDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar,strCustAId.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
            return oCommand;
        }
        #endregion

        #region Get Next Director Number
        public int GetNextDirectorNumber()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("DirectorSelectNumberOfDirector") as SqlCommand;
            db.AddInParameter(oCommand, "DirectorNumber", SqlDbType.VarChar, strCustAId.Trim());
            var varNumberOfDirector = db.ExecuteScalar(oCommand);
            return (varNumberOfDirector != null && varNumberOfDirector.ToString().Trim() != "" ? int.Parse(varNumberOfDirector.ToString()) : 0) + 1;
        }
        #endregion

        #region Get Director Created By Full Name Or UserId
        public string GetDirectorCreatedByFullNameOrUserId()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorSelectCreatedByFullNameOrUserId") as SqlCommand;
            db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length >= 1)
            {
                return thisRow[0]["FullName"] != null && thisRow[0]["FullName"].ToString().Trim() != "" ? thisRow[0]["FullName"].ToString() : thisRow[0]["UserId"].ToString();
            }
            else
            {
                return "";
            }
        }
        #endregion



        #region KYC

        #region Get All By Director Type And KYC Document Type
        public DataSet GetAllByDirectorTypeKYCDocType(Int64 intKYCDocType)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorSelectByDirectorTypeKYCDocType") as SqlCommand;
            db.AddInParameter(dbCommand, "KYCDocType", SqlDbType.BigInt, intKYCDocType);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All No Picture And Picture
        public DataSet GetAllNoPictureAndPicture()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorSelectAllNoPictureAndPicture") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All No Signature And Signature
        public DataSet GetAllNoSignatureAndSignature()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorSelectAllNoSignatureAndSignature") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

       
        #endregion

    }
}
