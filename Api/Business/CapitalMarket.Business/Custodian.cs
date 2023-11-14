using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace CapitalMarket.Business
{
    public class Custodian
    {
        #region Declarations
        private string strTransno, strCustodianCode;
        private string strCustodianAcct,strUserId,strSaveType;
        #endregion

        #region Properties
        public string Transno
        {
            set { strTransno = value; }
            get { return strTransno; }
        }
        public string CustodianCode
        {
            set { strCustodianCode = value; }
            get { return strCustodianCode; }
        }
        public string CustodianAcct
        {
            set { strCustodianAcct = value; }
            get { return strCustodianAcct; }
        }
       
        public string UserId
        {
            set { strUserId = value; }
            get { return strUserId; }
        }
        
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
                enSaveStatus = DataGeneral.SaveStatus.NameExistAdd;
                return enSaveStatus;
            }

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strSaveType.Trim() == "ADDS")
            {
                dbCommand = db.GetStoredProcCommand("CustodianAddNew") as SqlCommand;
            }
            else if (strSaveType.Trim() == "EDIT")
            {
                dbCommand = db.GetStoredProcCommand("CustodianEdit") as SqlCommand;
            }

            db.AddInParameter(dbCommand, "Transno", SqlDbType.VarChar, strTransno.Trim());
            db.AddInParameter(dbCommand, "CustodianCode", SqlDbType.VarChar, strCustodianCode.ToUpper().Trim());
            db.AddInParameter(dbCommand, "Account", SqlDbType.VarChar, strCustodianAcct.Trim());
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
                oCommand = db.GetStoredProcCommand("CustodianChkTransNoExist") as SqlCommand;
                db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransno.Trim());
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
            SqlCommand oCommand = db.GetStoredProcCommand("CustodianChkNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.Char, strTransno.Trim());
            db.AddInParameter(oCommand, "Name", SqlDbType.VarChar, strCustodianCode.Trim().ToUpper());
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


        #region Get Custodian
        public bool GetCustodian()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustodianSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "Transno", SqlDbType.VarChar, strTransno.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransno = thisRow[0]["Transno"].ToString();
                strCustodianCode = thisRow[0]["CustodianCode"].ToString();
                strCustodianAcct = thisRow[0]["TradeBankAC"].ToString();
                strUserId = thisRow[0]["UserId"].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get Custodian By Custodian Code
        public bool GetCustodianByCustodianCode()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustodianSelectByCustodianCode") as SqlCommand;
            db.AddInParameter(dbCommand, "CustodianCode", SqlDbType.VarChar, strCustodianCode.ToUpper().Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransno = thisRow[0]["Transno"].ToString();
                strCustodianCode = thisRow[0]["CustodianCode"].ToString();
                strCustodianAcct = thisRow[0]["TradeBankAC"].ToString();
                strUserId = thisRow[0]["UserId"].ToString();
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
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustodianSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public DataGeneral.SaveStatus Delete()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("CustodianDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransno.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion
    }
}
