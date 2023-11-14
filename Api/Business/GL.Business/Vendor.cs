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
    public class Vendor
    {
        #region Declaration
        private Int64 intTransNo;
        private string strName;
        private string strContact;
        private string strAddress;
        private string strAddress1;
        private string strPhone;
        private string strEmail;
        private string strUserID;
        private DateTime datContactDate;
        private string strSaveType;


        #endregion

        #region Properties
        public Int64 TransNo
        {
            set { intTransNo = value; }
            get { return intTransNo; }
        }

        public string Name
        {
            set { strName = value; }
            get { return strName; }
        }
        public string Contact
        {
            set { strContact = value; }
            get { return strContact; }
        }
        public string Address
        {
            set { strAddress = value; }
            get { return strAddress; }
        }
        public string Address1
        {
            set { strAddress1 = value; }
            get { return strAddress1; }
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
        public string UserID
        {
            set { strUserID = value; }
            get { return strUserID; }
        }

        public DateTime ContactDate
        {
            set { datContactDate = value; }
            get { return datContactDate; }
        }
        public string ProductId { get; set; }
        public string CustomerId { get; set; }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
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
                if (strSaveType == "ADDS")
                {
                    enSaveStatus = DataGeneral.SaveStatus.NameExistAdd;
                }
                else if (strSaveType == "EDIT")
                {
                    enSaveStatus = DataGeneral.SaveStatus.NameExistEdit;
                }
                return enSaveStatus;
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    SqlCommand oCommand = null;
                    if (strSaveType == "ADDS")
                    {
                        oCommand = db.GetStoredProcCommand("VendorAdd") as SqlCommand;
                    }
                    else if (strSaveType == "EDIT")
                    {
                        oCommand = db.GetStoredProcCommand("VendorEdit") as SqlCommand;
                    }
                    
                    db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intTransNo);
                    if (datContactDate != DateTime.MinValue)
                    {
                        db.AddInParameter(oCommand, "ContactDate", SqlDbType.DateTime, datContactDate);
                    }
                    else
                    {
                        db.AddInParameter(oCommand, "ContactDate", SqlDbType.DateTime, SqlDateTime.Null);
                    }
                    db.AddInParameter(oCommand, "Contact", SqlDbType.VarChar, strContact.Trim());
                    db.AddInParameter(oCommand, "Phone", SqlDbType.VarChar, strPhone.Trim());
                    db.AddInParameter(oCommand, "Name", SqlDbType.VarChar, strName.Trim().ToUpper());
                    db.AddInParameter(oCommand, "UserID", SqlDbType.Char, GeneralFunc.UserName.Trim());
                    db.AddInParameter(oCommand, "Address1", SqlDbType.Char, strAddress.Trim());
                    db.AddInParameter(oCommand, "Address2", SqlDbType.VarChar, strAddress1.Trim());
                    db.AddInParameter(oCommand, "Email", SqlDbType.VarChar, strEmail.Trim());
                    db.AddInParameter(oCommand, "ProductId", SqlDbType.VarChar, ProductId.Trim());
                    db.AddInParameter(oCommand, "CustomerId", SqlDbType.VarChar, CustomerId.Trim());
                    db.ExecuteNonQuery(oCommand, transaction);
                    transaction.Commit();
                    enSaveStatus = DataGeneral.SaveStatus.Saved;
                    return enSaveStatus;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
        #endregion

        #region Save And Return Command
        public SqlCommand SaveCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (strSaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("VendorAdd") as SqlCommand;
                db.AddOutParameter(oCommand, "TransNo", SqlDbType.BigInt, 8);
            }
            else if (strSaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("VendorEdit") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            }

            
            if (datContactDate != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "ContactDate", SqlDbType.DateTime, datContactDate);
            }
            else
            {
                db.AddInParameter(oCommand, "ContactDate", SqlDbType.DateTime, SqlDateTime.Null);
            }
            db.AddInParameter(oCommand, "Contact", SqlDbType.VarChar, strContact.Trim());
            db.AddInParameter(oCommand, "Phone", SqlDbType.VarChar, strPhone.Trim());
            db.AddInParameter(oCommand, "Name", SqlDbType.VarChar, strName.Trim().ToUpper());
            db.AddInParameter(oCommand, "UserID", SqlDbType.Char, GeneralFunc.UserName.Trim());
            db.AddInParameter(oCommand, "Address1", SqlDbType.Char, strAddress.Trim());
            db.AddInParameter(oCommand, "Address2", SqlDbType.VarChar, strAddress1.Trim());
            db.AddInParameter(oCommand, "Email", SqlDbType.VarChar, strEmail.Trim());
            db.AddInParameter(oCommand, "ProductId", SqlDbType.VarChar, ProductId.Trim());
            db.AddInParameter(oCommand, "CustomerId", SqlDbType.VarChar, CustomerId.Trim());
            return oCommand;

        }
        #endregion

        #region Check TransNo Exist
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("VendorChkTransNoExist") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intTransNo);
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
            SqlCommand oCommand = db.GetStoredProcCommand("VendorChkNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            db.AddInParameter(oCommand, "Name", SqlDbType.VarChar, strName.Trim().ToUpper());
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

        #region Get All
        public DataSet GetAll(string strOrderBy)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strOrderBy.Trim() == "NAME")
            {
                dbCommand = db.GetStoredProcCommand("VendorSelectAllOrderByName") as SqlCommand;
            }
            else
            {
                dbCommand = db.GetStoredProcCommand("VendorSelectAll") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Vendor
        public bool GetVendor()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("VendorSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, intTransNo);

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strName = thisRow[0]["VendorName"].ToString();
                strContact = thisRow[0]["VendorContact"].ToString();
                strAddress = thisRow[0]["Address"].ToString();
                strAddress1 = thisRow[0]["Address1"].ToString();
                strPhone = thisRow[0]["Phone"].ToString();
                if (thisRow[0]["ContactDate"] != null && thisRow[0]["ContactDate"].ToString() != "")
                {
                    datContactDate = DateTime.ParseExact(thisRow[0]["ContactDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                }
                else
                {
                    datContactDate = DateTime.MinValue;
                }
                strEmail = thisRow[0]["Email"].ToString();
                ProductId = thisRow[0]["ProductId"] != null ? thisRow[0]["ProductId"].ToString() : "";
                CustomerId = thisRow[0]["CustomerId"] != null ? thisRow[0]["CustomerId"].ToString() : "";
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }

            return blnStatus;
        }
        #endregion

       
    }
}
