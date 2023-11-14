using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace CapitalMarket.Business
{
    public class Register
    {
        #region Declaration
        private string strTransno, strSecCode, strSecName, strAddr1, strAddr2, strAddr3;
        private string strTel, strEmail, strFax, strUserId;
        #endregion

        #region Properties
        public string Transno
        {
            set { strTransno = value; }
            get { return strTransno; }
        }
        public string SecCode
        {
            set { strSecCode = value; }
            get { return strSecCode; }
        }
        public string SecName
        {
            set { strSecName = value; }
            get { return strSecName; }
        }
        public string Addr1
        {
            set { strAddr1 = value; }
            get { return strAddr1; }
        }
        public string Addr2
        {
            set { strAddr2 = value; }
            get { return strAddr2; }
        }
        public string Addr3
        {
            set { strAddr3 = value; }
            get { return strAddr3; }
        }
        public string Tel
        {
            set { strTel = value; }
            get { return strTel; }
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

        public string UserId
        {
            set { strUserId = value; }
            get { return strUserId; }
        }
        #endregion


        #region Enum
        public enum SaveStatus { Nothing, NotExist, Saved }
        #endregion

        #region Add New Register
        public bool Add()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("RegistrarAddNew") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransno);
            db.AddInParameter(oCommand, "SecCode", SqlDbType.VarChar, strSecCode.ToUpper());
            db.AddInParameter(oCommand, "SecName", SqlDbType.VarChar, strSecName.ToUpper());
            db.AddInParameter(oCommand, "Addr1", SqlDbType.VarChar, strAddr1);
            db.AddInParameter(oCommand, "Addr2", SqlDbType.VarChar, strAddr2);
            db.AddInParameter(oCommand, "Addr3", SqlDbType.VarChar, strAddr3);
            db.AddInParameter(oCommand, "Tel", SqlDbType.VarChar, strTel);
            db.AddInParameter(oCommand, "Email", SqlDbType.VarChar, strEmail);
            db.AddInParameter(oCommand, "Fax", SqlDbType.VarChar, strFax.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "REGISTRAR");
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;


            return blnStatus;
        }
        #endregion

        #region Edit Register
        public bool Edit()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("RegistrarEdit") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransno);
            db.AddInParameter(oCommand, "SecCode", SqlDbType.VarChar, strSecCode.ToUpper());
            db.AddInParameter(oCommand, "SecName", SqlDbType.VarChar, strSecName.ToUpper());
            db.AddInParameter(oCommand, "Addr1", SqlDbType.VarChar, strAddr1);
            db.AddInParameter(oCommand, "Addr2", SqlDbType.VarChar, strAddr2);
            db.AddInParameter(oCommand, "Addr3", SqlDbType.VarChar, strAddr3);
            db.AddInParameter(oCommand, "Tel", SqlDbType.VarChar, strTel);
            db.AddInParameter(oCommand, "Email", SqlDbType.VarChar, strEmail);
            db.AddInParameter(oCommand, "Fax", SqlDbType.VarChar, strFax);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId);

                db.ExecuteNonQuery(oCommand);
                blnStatus = true;

            return blnStatus;
        }
        #endregion

        #region Get Registrar
        public bool GetRegister()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("RegistrarSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "Transno", SqlDbType.VarChar, strTransno.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransno = thisRow[0]["Transno"].ToString();
                strSecCode = thisRow[0]["SecCode"].ToString();
                strSecName = thisRow[0]["SecName"].ToString();
                strAddr1 = thisRow[0]["Addr1"].ToString();
                strAddr2 = thisRow[0]["Addr2"].ToString();
                strAddr3 = thisRow[0]["Addr3"].ToString();
                strTel = thisRow[0]["Tel"].ToString();
                strEmail = thisRow[0]["Email"].ToString();
                strFax = thisRow[0]["Fax"].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get Registrar Given Registrar Code
        public bool GetRegisterGivenRegistrarCode()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("RegistrarSelectByRegistrarCode") as SqlCommand;
            db.AddInParameter(dbCommand, "Seccode", SqlDbType.VarChar,strSecCode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransno = thisRow[0]["Transno"].ToString();
                strSecCode = thisRow[0]["SecCode"].ToString();
                strSecName = thisRow[0]["SecName"].ToString();
                strAddr1 = thisRow[0]["Addr1"].ToString();
                strAddr2 = thisRow[0]["Addr2"].ToString();
                strAddr3 = thisRow[0]["Addr3"].ToString();
                strTel = thisRow[0]["Tel"].ToString();
                strEmail = thisRow[0]["Email"].ToString();
                strFax = thisRow[0]["Fax"].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get All Registrar Order By Officer No
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("RegistrarSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("RegistrarDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransno);
            db.AddInParameter(oCommand, "SecCode", SqlDbType.VarChar, strSecCode.ToUpper());
            db.AddInParameter(oCommand, "SecName", SqlDbType.VarChar, strSecName.ToUpper());
            db.AddInParameter(oCommand, "Addr1", SqlDbType.VarChar, strAddr1);
            db.AddInParameter(oCommand, "Addr2", SqlDbType.VarChar, strAddr2);
            db.AddInParameter(oCommand, "Addr3", SqlDbType.VarChar, strAddr3);
            db.AddInParameter(oCommand, "Tel", SqlDbType.VarChar, strTel);
            db.AddInParameter(oCommand, "Email", SqlDbType.VarChar, strEmail);
            db.AddInParameter(oCommand, "Fax", SqlDbType.VarChar, strFax.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());



            db.ExecuteNonQuery(oCommand);
            blnStatus = true;


            return blnStatus;
        }
        #endregion

        #region Check That Registrar Code Already Exist For Existing Record
        public bool RegistrarCodeExist()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("RegistrarSelectByCodeExist") as SqlCommand;
            db.AddInParameter(dbCommand, "SecCode", SqlDbType.VarChar, strSecCode.Trim());
            db.AddOutParameter(dbCommand, "CodeExist", SqlDbType.VarChar, 1);
            db.AddInParameter(dbCommand, "Transno", SqlDbType.VarChar, strTransno.Trim());
            db.ExecuteNonQuery(dbCommand);
            if (db.GetParameterValue(dbCommand, "CodeExist").ToString().Trim() == "0")
            {
                blnStatus = false;
            }
            else
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Check That Registrar Code Already Exist For New Record
        public bool RegistrarCodeExistNoTransNo()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("RegistrarSelectByCodeExistNoTransno") as SqlCommand;
            db.AddInParameter(dbCommand, "SecCode", SqlDbType.VarChar, strSecCode.Trim());
            db.AddOutParameter(dbCommand, "CodeExist", SqlDbType.VarChar, 1);
            db.ExecuteNonQuery(dbCommand);
            if (db.GetParameterValue(dbCommand, "CodeExist").ToString().Trim() == "0")
            {
                blnStatus = false;
            }
            else
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Check That Registrar Name Already Exist For Existing Record
        public bool RegistrarNameExist()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("RegistrarSelectByNameExist") as SqlCommand;
            db.AddInParameter(dbCommand, "Secname", SqlDbType.VarChar, strSecName.Trim());
            db.AddOutParameter(dbCommand, "NameExist", SqlDbType.VarChar, 1);
            db.AddInParameter(dbCommand, "Transno", SqlDbType.VarChar, strTransno.Trim());
            db.ExecuteNonQuery(dbCommand);
            if (db.GetParameterValue(dbCommand, "NameExist").ToString().Trim() == "0")
            {
                blnStatus = false;
            }
            else
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Check That Registrar Name Already Exist For New Record
        public bool RegistrarNameExistNoTransNo()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("RegistrarSelectByNameExistNoTransno") as SqlCommand;
            db.AddInParameter(dbCommand, "Secname", SqlDbType.VarChar, strSecName.Trim());
            db.AddOutParameter(dbCommand, "NameExist", SqlDbType.VarChar, 1);
            db.ExecuteNonQuery(dbCommand);
            if (db.GetParameterValue(dbCommand, "NameExist").ToString().Trim() == "0")
            {
                blnStatus = false;
            }
            else
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

    }
}
