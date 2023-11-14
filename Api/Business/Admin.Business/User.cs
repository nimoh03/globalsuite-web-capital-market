using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Text.RegularExpressions;
using BaseUtility.Business;
using GL.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace Admin.Business
{
    public class User
    {
        #region Declaration
        private string strUserNameAccount, strPassword, strGroup, strUserDeactivated,strFullName, strBranchId;
        private DateTime datPswExpDate, datCreationDate, datPassChangeDate;
        private int intLocation, intPassAge;
        private bool blnLoggedIn, blnPassLockup;

        const string strEnforcePattern = @"^.*(?=.{6,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).*$";
        static readonly Regex EnforcePatternRegex = new Regex(strEnforcePattern);
        private readonly string _encryptionKey = ConfigurationManager.AppSettings["EncryptionKey"];
        public string EmailAddress { set; get; }
        #endregion

        #region Properties
        public string UserNameAccount
        {
            set { strUserNameAccount = value; }
            get { return strUserNameAccount; }
        }

        public string Password
        {
            set { strPassword = value; }
            get { return strPassword; }
        }
        public string Group
        {
            set { strGroup = value; }
            get { return strGroup; }
        }
        public string UserDeactivated
        {
            set { strUserDeactivated = value; }
            get { return strUserDeactivated; }
        }
        public string FullName
        {
            set { strFullName = value; }
            get { return strFullName; }
        }
        public string BranchId
        {
            set { strBranchId = value; }
            get { return strBranchId; }
        }
        public DateTime PswExpDate
        {
            set { datPswExpDate = value; }
            get { return datPswExpDate; }
        }
        public DateTime CreationDate
        {
            set { datCreationDate = value; }
            get { return datCreationDate; }
        }
        public DateTime PassChangeDate
        {
            set { datPassChangeDate = value; }
            get { return datPassChangeDate; }
        }
        public int PassAge
        {
            set { intPassAge = value; }
            get { return intPassAge; }
        }
        
        public int Location
        {
            set { intLocation = value; }
            get { return intLocation; }
        }
        public bool LoggedIn
        {
            set { blnLoggedIn = value; }
            get { return blnLoggedIn; }
        }
        public bool PassLockup
        {
            set { blnPassLockup = value; }
            get { return blnPassLockup; }
        }
        
        #endregion

        #region Get User
        public bool GetUser(string strUserInputtedPassword)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            var blnStatus = false;
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var dbCommand = db.GetStoredProcCommand("UserSelectNoPassword") as SqlCommand;
            db.AddInParameter(dbCommand, "UserNameAccount", SqlDbType.VarChar, strUserNameAccount.Trim());
            
            var oDS = db.ExecuteDataSet(dbCommand);
            var thisTable = oDS.Tables[0];
            var thisRow = thisTable.Select();
            var key = ConfigurationManager.AppSettings["encryptionKey"];
            if (_encryptionKey == null || (thisRow.Length != 1) || (strUserInputtedPassword !=
                                                                    StringCipher.Decrypt(
                                                                        thisRow[0]["Password"].ToString(),
                                                                        _encryptionKey))) // TODO
                return false;
            
                strUserDeactivated = thisRow[0]["UserDeactivated"].ToString();
                strGroup = thisRow[0]["UserGroup"].ToString();
                strFullName = thisRow[0]["FullName"] != null ? thisRow[0]["FullName"].ToString() : "";
                var expDate = thisRow[0]["ExpDate"].ToString().Trim();
                datPswExpDate = thisRow[0]["ExpDate"] != null && thisRow[0]["ExpDate"].ToString() != ""
                    ? DateHelper.ToDate(thisRow[0]["ExpDate"].ToString().Trim())
                    : datPswExpDate = DateTime.MinValue;
                intLocation = thisRow[0]["Location"] != null && thisRow[0]["Location"].ToString().Trim() != ""
                    ? int.Parse(thisRow[0]["Location"].ToString())
                    : 0;
                blnLoggedIn = thisRow[0]["Login"] != null && thisRow[0]["Login"].ToString().Trim() != ""
                    ? bool.Parse(thisRow[0]["Login"].ToString())
                    : false;
                blnPassLockup = thisRow[0]["PassLockup"] != null && thisRow[0]["PassLockup"].ToString().Trim() != ""
                    ? bool.Parse(thisRow[0]["PassLockup"].ToString())
                    : false;
                datCreationDate =
                    thisRow[0]["CreationDate"] != null && thisRow[0]["CreationDate"].ToString().Trim() != ""
                        ? DateHelper.ToDate(thisRow[0]["CreationDate"].ToString().Trim())
                        : DateTime.MinValue;
                datPassChangeDate =
                    thisRow[0]["PassChangeDate"] != null && thisRow[0]["PassChangeDate"].ToString().Trim() != ""
                        ? DateHelper.ToDate(thisRow[0]["PassChangeDate"].ToString().Trim())
                        : DateTime.MinValue;
                intPassAge = thisRow[0]["PassAge"] != null && thisRow[0]["PassAge"].ToString().Trim() != ""
                    ? int.Parse(thisRow[0]["PassAge"].ToString())
                    : 0;
                EmailAddress = thisRow[0]["EmailAddress"] != null ? thisRow[0]["EmailAddress"].ToString() : "";
                strBranchId = thisRow[0]["BranchId"].ToString();
                return true;
        }
        #endregion

        #region Get User No Password
        public bool GetUserNoPassword()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            var blnStatus = false;
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var dbCommand = db.GetStoredProcCommand("UserSelectNoPassword") as SqlCommand;
            db.AddInParameter(dbCommand, "UserNameAccount", SqlDbType.VarChar, strUserNameAccount.Trim());
            var oDS = db.ExecuteDataSet(dbCommand);
            var thisTable = oDS.Tables[0];
            var thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strUserDeactivated = thisRow[0]["UserDeactivated"].ToString();
                strGroup = thisRow[0]["UserGroup"].ToString();
                strFullName = thisRow[0]["FullName"] != null ? thisRow[0]["FullName"].ToString() : "";
                datPswExpDate = thisRow[0]["ExpDate"] != null && thisRow[0]["ExpDate"].ToString().Trim() != "" ?
                    DateHelper.ToDate(thisRow[0]["ExpDate"].ToString().Trim()) : datPswExpDate = DateTime.MinValue;
                intLocation = thisRow[0]["Location"] != null && thisRow[0]["Location"].ToString().Trim() != "" ?
                            int.Parse(thisRow[0]["Location"].ToString()) : 0;
                blnLoggedIn = thisRow[0]["Login"] != null && thisRow[0]["Login"].ToString().Trim() != "" ?
                            bool.Parse(thisRow[0]["Login"].ToString()) : false;
                blnPassLockup = thisRow[0]["PassLockup"] != null && thisRow[0]["PassLockup"].ToString().Trim() != "" ?
                            bool.Parse(thisRow[0]["PassLockup"].ToString()) : false;
                datCreationDate = thisRow[0]["CreationDate"] != null && thisRow[0]["CreationDate"].ToString().Trim() != "" ?
                    DateHelper.ToDate(thisRow[0]["CreationDate"].ToString().Trim()) : datPswExpDate = DateTime.MinValue;
                datPassChangeDate = thisRow[0]["PassChangeDate"] != null && thisRow[0]["PassChangeDate"].ToString().Trim() != "" ?
                    DateHelper.ToDate(thisRow[0]["PassChangeDate"].ToString().Trim()) : datPswExpDate = DateTime.MinValue;
                intPassAge = thisRow[0]["PassAge"] != null && thisRow[0]["PassAge"].ToString().Trim() != "" ?
                        int.Parse(thisRow[0]["PassAge"].ToString()) : 0;
                EmailAddress = thisRow[0]["EmailAddress"] != null ? thisRow[0]["EmailAddress"].ToString() : "";
                strBranchId = thisRow[0]["BranchId"].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get User By Email Address
        public bool GetUserByEmailAddress()
        {
            var blnStatus = false;
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var dbCommand = db.GetStoredProcCommand("UserLoginSelectByEmailAddress") as SqlCommand;
            db.AddInParameter(dbCommand, "EmailAddress", SqlDbType.NVarChar, EmailAddress.Trim());
            var oDS = db.ExecuteDataSet(dbCommand);
            var thisTable = oDS.Tables[0];
            var thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strFullName = thisRow[0]["FullName"] != null ? thisRow[0]["FullName"].ToString() : "";
                Password = thisRow[0]["Password"].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Save
        public DataGeneral.SaveStatus Save()
        {
            var oCompany = new Company();
            oCompany.GetCompany(1);
            IFormatProvider format = new CultureInfo("en-GB");
            Password = StringCipher.Encrypt(Password, _encryptionKey);
            var enSaveStatus = DataGeneral.SaveStatus.Nothing;
            if (ChkUserLoginNameExist())
            {
                enSaveStatus = DataGeneral.SaveStatus.NameExistAdd;
                return enSaveStatus;
            }
            if (ChkUserEmailAddressExist())
            {
                enSaveStatus = DataGeneral.SaveStatus.HeadOfficeExistAdd;
                return enSaveStatus;
            }
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var oCommand = db.GetStoredProcCommand("UserAddNew") as SqlCommand;
            db.AddInParameter(oCommand, "UserNameAccount", SqlDbType.VarChar, strUserNameAccount.Trim().ToUpper());
            db.AddInParameter(oCommand, "Password", SqlDbType.VarChar, strPassword.Trim());
            db.AddInParameter(oCommand, "UserGroup", SqlDbType.VarChar, strGroup.Trim());
            db.AddInParameter(oCommand, "FullName", SqlDbType.VarChar, strFullName.Trim().ToUpper());
            db.AddInParameter(oCommand, "CreationDate", SqlDbType.DateTime,DateTime.ParseExact(GeneralFunc.GetTodayDate().ToString().Trim().Substring(0,10),"dd/MM/yyyy",format));
            db.AddInParameter(oCommand, "PassChangeDate", SqlDbType.DateTime, DateTime.ParseExact(GeneralFunc.GetTodayDate().ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format));
            if (oCompany.PasswordValidityDay != 0)
            {
                db.AddInParameter(oCommand, "ExpDate", SqlDbType.DateTime, DateTime.ParseExact(GeneralFunc.GetTodayDate().ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format).AddDays(oCompany.PasswordValidityDay));
            }
            else
            {
                db.AddInParameter(oCommand, "ExpDate", SqlDbType.DateTime, SqlDateTime.Null);
            }
            db.AddInParameter(oCommand, "Location", SqlDbType.Int, intLocation);
            db.AddInParameter(oCommand, "EmailAddress", SqlDbType.NVarChar, EmailAddress.Trim());
            db.AddInParameter(oCommand, "BranchId", SqlDbType.VarChar, strBranchId.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.NVarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Check That User Name Already Exist
        public bool ChkUserLoginNameExist()
        {
            var blnStatus = false;
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var dbCommand = db.GetStoredProcCommand("UserLoginSelectByUserName") as SqlCommand;
            db.AddInParameter(dbCommand, "UserNameAccount", SqlDbType.VarChar, strUserNameAccount.Trim());
            var oDs = db.ExecuteDataSet(dbCommand);
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
        #region Check That User Email Address Already Exist
        public bool ChkUserEmailAddressExist()
        {
            var blnStatus = false;
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var dbCommand = db.GetStoredProcCommand("UserLoginSelectByEmailAddress") as SqlCommand;
            db.AddInParameter(dbCommand, "EmailAddress", SqlDbType.NVarChar, EmailAddress.Trim());
            var oDs = db.ExecuteDataSet(dbCommand);
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
        
        #region Check That User Name For User Group Exist
        public bool ChkUserForUserGroupExist()
        {
            var blnStatus = false;
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var dbCommand = db.GetStoredProcCommand("UserSelectByUserGroup") as SqlCommand;
            db.AddInParameter(dbCommand, "UserGroup", SqlDbType.VarChar,strGroup.Trim());
            var oDs = db.ExecuteDataSet(dbCommand);
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

        #region Get User Group
        public string GetUserGroup()
        {
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var dbCommand = db.GetStoredProcCommand("UserSelectUserGroup") as SqlCommand;
            db.AddInParameter(dbCommand, "UserNameAccount", SqlDbType.VarChar, strUserNameAccount.Trim());
            var strGroupReturn = db.ExecuteScalar(dbCommand);
            return strGroupReturn != null ? strGroupReturn.ToString() : "";
        }
        #endregion

        #region Get Full Name And Email Address
        public bool GetFullNameAndEmailAddress()
        {
            bool blnStatus;
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var dbCommand = db.GetStoredProcCommand("UserSelectFullNameEmailAddress") as SqlCommand;
            db.AddInParameter(dbCommand, "UserNameAccount", SqlDbType.VarChar, strUserNameAccount.Trim());
            var oDS = db.ExecuteDataSet(dbCommand);
            var thisTable = oDS.Tables[0];
            var thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strFullName = thisRow[0]["FullName"].ToString();
                EmailAddress = thisRow[0]["EmailAddress"] != null ? thisRow[0]["EmailAddress"].ToString() : "";
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get BranchId
        public string GetBranchId()
        {
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var dbCommand = db.GetStoredProcCommand("UserSelectBranchId") as SqlCommand;
            db.AddInParameter(dbCommand, "UserNameAccount", SqlDbType.VarChar, strUserNameAccount.Trim());
            var strBranchIdReturn = db.ExecuteScalar(dbCommand);
            return strBranchIdReturn != null ? strBranchIdReturn.ToString() : "";
        }
        #endregion

        #region Get All Users
        public DataSet GetAll()
        {
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var dbCommand = db.GetStoredProcCommand("UserSelectAll") as SqlCommand;
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region Get All Suspended Users
        public DataSet GetAllSuspend()
        {
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var dbCommand = db.GetStoredProcCommand("UserSelectAllSuspend") as SqlCommand;
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region Get All Users
        public DataSet GetAllActive()
        {
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var dbCommand = db.GetStoredProcCommand("UserSelectAllActive") as SqlCommand;
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion
        
        #region Suspend
        public bool Suspend()
        {
            var blnStatus = false;
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var oCommand = db.GetStoredProcCommand("UserSuspend") as SqlCommand;
            db.AddInParameter(oCommand, "UserNameAccount", SqlDbType.VarChar, strUserNameAccount.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Reactivate
        public bool Reactivate()
        {
            var blnStatus = false;
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var oCommand = db.GetStoredProcCommand("UserReactivate") as SqlCommand;
            db.AddInParameter(oCommand, "UserNameAccount", SqlDbType.VarChar, strUserNameAccount.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.AddInParameter(oCommand, "UserDeactivated", SqlDbType.VarChar,strUserDeactivated.Trim());
            db.AddInParameter(oCommand, "PassLockup", SqlDbType.Bit, blnPassLockup);
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Remove
        public bool Remove()
        {
            var blnStatus = false;
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var oCommand = db.GetStoredProcCommand("UserRemove") as SqlCommand;
            db.AddInParameter(oCommand, "UserNameAccount", SqlDbType.VarChar, strUserNameAccount.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Check User Locked
        public bool CheckUserLocked()
        {
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var oCommand = db.GetStoredProcCommand("UserSelectLocked") as SqlCommand;
            db.AddInParameter(oCommand, "UserNameAccount", SqlDbType.VarChar, strUserNameAccount.Trim());
            var blnResult = db.ExecuteScalar(oCommand);
            return blnResult != null && blnResult.ToString().Trim() != "" ? bool.Parse(blnResult.ToString()) : false;
        }
        #endregion

        #region Locked up
        public void Lockedup()
        {
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var oCommand = db.GetStoredProcCommand("UserLocked") as SqlCommand;
            db.AddInParameter(oCommand, "UserNameAccount", SqlDbType.VarChar, strUserNameAccount.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, strUserNameAccount.Trim());
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        #region UnLocked
        public bool UnLocked()
        {
            var blnStatus = false;
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var oCommand = db.GetStoredProcCommand("UserUnLocked") as SqlCommand;
            db.AddInParameter(oCommand, "UserNameAccount", SqlDbType.VarChar, strUserNameAccount.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region UnSuccessfull Login
        public void UnSuccessLogin()
        {
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var oCommand = db.GetStoredProcCommand("UserUnSuccessLogin") as SqlCommand;
            db.AddInParameter(oCommand, "UserNameAccount", SqlDbType.VarChar, strUserNameAccount.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, strUserNameAccount.Trim());
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        #region Logged In 
        public bool LoggedInAction()
        {
            var blnStatus = false;
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var oCommand = db.GetStoredProcCommand("UserLoggedIn") as SqlCommand;
            db.AddInParameter(oCommand, "UserNameAccount", SqlDbType.VarChar, strUserNameAccount.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Logged Out
        public void LoggedOut()
        {
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var oCommand = db.GetStoredProcCommand("UserLoggedOut") as SqlCommand;
            db.AddInParameter(oCommand, "UserNameAccount", SqlDbType.VarChar, strUserNameAccount.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        #region Change Password
        public bool ChangePassword()
        {
            var blnStatus = false;
            IFormatProvider format = new CultureInfo("en-GB");
            var oCompany = new Company();
            oCompany.GetCompany(1);
            strPassword = StringCipher.Encrypt(strPassword, _encryptionKey);
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var oCommand = db.GetStoredProcCommand("UserChangePass") as SqlCommand;
            db.AddInParameter(oCommand, "UserNameAccount", SqlDbType.VarChar, strUserNameAccount.Trim().ToUpper());
            db.AddInParameter(oCommand, "Password", SqlDbType.VarChar, strPassword.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.AddInParameter(oCommand, "PassChangeDate", SqlDbType.DateTime, DateTime.ParseExact(GeneralFunc.GetTodayDate().ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format));
            if (oCompany.PasswordValidityDay != 0)
            {
                db.AddInParameter(oCommand, "ExpDate", SqlDbType.DateTime, DateTime.ParseExact(GeneralFunc.GetTodayDate().ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format).AddDays(oCompany.PasswordValidityDay));
            }
            else
            {
                if (datPswExpDate != DateTime.MinValue)
                {
                    db.AddInParameter(oCommand, "ExpDate", SqlDbType.DateTime, datPswExpDate);
                }
                else
                {
                    db.AddInParameter(oCommand, "ExpDate", SqlDbType.DateTime, SqlDateTime.Null);
                }
            }
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Change Password And Update Created New 
        public bool ChangePasswordAndUpdateCreatedNew()
        {
            var blnStatus = false;
            IFormatProvider format = new CultureInfo("en-GB");
            var oCompany = new Company();
            oCompany.GetCompany(1);
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            using (var connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                try
                {
                    var oCommand = db.GetStoredProcCommand("UserChangePass") as SqlCommand;
                    db.AddInParameter(oCommand, "UserNameAccount", SqlDbType.VarChar, strUserNameAccount.Trim().ToUpper());
                    db.AddInParameter(oCommand, "Password", SqlDbType.VarChar, strPassword.Trim());
                    db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    db.AddInParameter(oCommand, "PassChangeDate", SqlDbType.DateTime, DateTime.ParseExact(GeneralFunc.GetTodayDate().ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format));
                    if (oCompany.PasswordValidityDay != 0)
                    {
                        db.AddInParameter(oCommand, "ExpDate", SqlDbType.DateTime, DateTime.ParseExact(GeneralFunc.GetTodayDate().ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format).AddDays(oCompany.PasswordValidityDay));
                    }
                    else
                    {
                        if (datPswExpDate != DateTime.MinValue)
                        {
                            db.AddInParameter(oCommand, "ExpDate", SqlDbType.DateTime, datPswExpDate);
                        }
                        else
                        {
                            db.AddInParameter(oCommand, "ExpDate", SqlDbType.DateTime, SqlDateTime.Null);
                        }
                    }
                    db.ExecuteNonQuery(oCommand, transaction);

                    var dbCommand = db.GetStoredProcCommand("UserEditCreatedNew") as SqlCommand;
                    db.AddInParameter(dbCommand, "UserNameAccount", SqlDbType.VarChar, strUserNameAccount.Trim().ToUpper());
                    db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    db.ExecuteNonQuery(dbCommand, transaction);

                    transaction.Commit();
                }
                catch (Exception err)
                {
                    transaction.Rollback();
                    throw err;
                }
            }
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Reset Password
        public bool ResetPassword()
        {
            var blnStatus = false;
            IFormatProvider format = new CultureInfo("en-GB");
            var oCompany = new Company();
            oCompany.GetCompany(1);
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var oCommand = db.GetStoredProcCommand("UserChangePass") as SqlCommand;
            db.AddInParameter(oCommand, "UserNameAccount", SqlDbType.VarChar, strUserNameAccount.Trim().ToUpper());
            db.AddInParameter(oCommand, "Password", SqlDbType.VarChar, strPassword.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.AddInParameter(oCommand, "PassChangeDate", SqlDbType.DateTime, DateTime.ParseExact(GeneralFunc.GetTodayDate().ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format));
            if (oCompany.PasswordValidityDay != 0)
            {
                db.AddInParameter(oCommand, "ExpDate", SqlDbType.DateTime, DateTime.ParseExact(GeneralFunc.GetTodayDate().ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format).AddDays(oCompany.PasswordValidityDay));
            }
            else
            {
                if (datPswExpDate != DateTime.MinValue)
                {
                    db.AddInParameter(oCommand, "ExpDate", SqlDbType.DateTime, datPswExpDate);
                }
                else
                {
                    db.AddInParameter(oCommand, "ExpDate", SqlDbType.DateTime, SqlDateTime.Null);
                }
            }
            db.ExecuteNonQuery(oCommand);

            var dbCommand = db.GetStoredProcCommand("UserEditCreatedNewToTrue") as SqlCommand;
            db.AddInParameter(dbCommand, "UserNameAccount", SqlDbType.VarChar, strUserNameAccount.Trim().ToUpper());
            db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(dbCommand);

            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Check Expired Password With Specific Date
        public bool ChkExpiredPassWithSpecificDate(User oUserReturn, DateTime datCurrentDate)
        {
            var blnStatus = true;
            if (oUserReturn.PswExpDate != DateTime.MinValue)
            {
                if (oUserReturn.PswExpDate > datCurrentDate)
                {
                    blnStatus = false;
                }
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Check Expired Password With Validity Day
        public bool ChkExpiredPassWithValidityDay(User oUserReturn, DateTime datCurrentDate)
        {
            var blnStatus = true;
            if (!((oUserReturn.PassAge != 0) && (oUserReturn.PassChangeDate != DateTime.MinValue) && (oUserReturn.PassChangeDate.AddDays(oUserReturn.PassAge) <= datCurrentDate)))
            { blnStatus = false; }
            return blnStatus;
        }
        #endregion

        #region Get Expired Date
        public bool GetExpDate()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            var blnStatus = false;
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var dbCommand = db.GetStoredProcCommand("UserSelectExpDate") as SqlCommand;
            db.AddInParameter(dbCommand, "UserNameAccount", SqlDbType.VarChar, strUserNameAccount.Trim());
            var oDS = db.ExecuteDataSet(dbCommand);
            var thisTable = oDS.Tables[0];
            var thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                datPswExpDate = thisRow[0]["ExpDate"].ToString() != "" && thisRow[0]["ExpDate"].ToString() != null ?
                                DateTime.ParseExact(thisRow[0]["ExpDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format) : DateTime.MinValue;
                datPassChangeDate = thisRow[0]["PassChangeDate"] != null && thisRow[0]["PassChangeDate"].ToString().Trim() != "" ?
                                    DateTime.ParseExact(thisRow[0]["PassChangeDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format) : DateTime.MinValue;
                intPassAge = thisRow[0]["PassAge"] != null && thisRow[0]["PassAge"].ToString().Trim() != "" ?
                             int.Parse(thisRow[0]["PassAge"].ToString()) : 0;
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Change User Group
        public bool ChangeUserGroup()
        {
            var blnStatus = false;
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var oCommand = db.GetStoredProcCommand("UserChangeGroup") as SqlCommand;
            db.AddInParameter(oCommand, "UserNameAccount", SqlDbType.VarChar, strUserNameAccount.Trim().ToUpper());
            db.AddInParameter(oCommand, "UserGroup", SqlDbType.VarChar, strGroup.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Change Full Name
        public bool ChangeFullName()
        {
            var blnStatus = false;
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var oCommand = db.GetStoredProcCommand("UserChangeFullName") as SqlCommand;
            db.AddInParameter(oCommand, "UserNameAccount", SqlDbType.VarChar, strUserNameAccount.Trim().ToUpper());
            db.AddInParameter(oCommand, "FullName", SqlDbType.VarChar, strFullName.Trim());
            db.AddInParameter(oCommand, "EmailAddress", SqlDbType.NVarChar, EmailAddress.Trim());
            db.AddInParameter(oCommand, "BranchId", SqlDbType.VarChar, strBranchId.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Change User Name
        public bool ChangeUserName()
        {
            var blnStatus = false;
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var oCommand = db.GetStoredProcCommand("UserChangeUserName") as SqlCommand;
            db.AddInParameter(oCommand, "UserNameAccount", SqlDbType.VarChar, strUserNameAccount.Trim().ToUpper());
            db.AddInParameter(oCommand, "NewUserName", SqlDbType.VarChar,strFullName.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Get Created New User Status
        public string GetCreatedNewStatus()
        {

            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var oCommand = db.GetStoredProcCommand("UserSelectCreatedNew") as SqlCommand;
            db.AddInParameter(oCommand, "UserNameAccount", SqlDbType.VarChar, strUserNameAccount.Trim());
            if (db.ExecuteScalar(oCommand) == null || db.ExecuteScalar(oCommand).ToString().Trim() == "")
            {
                return "Y";
            }
            else
            {
                return db.ExecuteScalar(oCommand).ToString();
            }
        }
        #endregion

        #region Enforce Password Policy
        public string EnforcePasswordPolicy(string strPasswordToCheck)
        {
            var strStrongPassword = "";
            var oGLParamStrongPass = new GLParam();
            oGLParamStrongPass.Type = "USESTRONGPASS";
            strStrongPassword = oGLParamStrongPass.CheckParameter();

            var strStatus = "";
            if (strStrongPassword.Trim() == "YES")
            {
                if (!EnforcePatternRegex.IsMatch(strPasswordToCheck))
                {
                    if (strPasswordToCheck.Trim().Length < 8)
                    {
                        strStatus = "Password Length Must Be Equal Or Greater Than 8";
                    }
                    else
                    {
                        strStatus = "Password Must Contain At Least A Number, An Upper Case Letter And A Lower Case Letter";
                    }
                }
            }
            return strStatus;
        }
        #endregion
    }
}
