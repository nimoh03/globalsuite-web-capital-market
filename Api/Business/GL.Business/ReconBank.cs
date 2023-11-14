using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{  

    public class ReconBank
    {
        #region Declaration

        private string strTransNo,strSaveType, strBankAcctCode, strCashBookAcct;
    
        #endregion

        #region Properties
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }


        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }

        public string  BankAcctCode
        {
            set { strBankAcctCode = value; }
            get { return strBankAcctCode; }
        }
        public string  CashBookAcct
        {
            set { strCashBookAcct = value; }
            get { return strCashBookAcct; }
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

            if (ChkGLAcctExist())
            {
                enSaveStatus = DataGeneral.SaveStatus.NameExistEdit;
                return enSaveStatus;
            }

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (strSaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("ReconBankAdd") as SqlCommand;

            }
            else
            {
                oCommand = db.GetStoredProcCommand("ReconBankEdit") as SqlCommand;
                
            }
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "BankAcctCode", SqlDbType.VarChar, strBankAcctCode.Trim().ToUpper());
            db.AddInParameter(oCommand, "CashBookAcct", SqlDbType.VarChar, strCashBookAcct.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
           
        }
        #endregion

        
        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("ReconBankSelectAll") as SqlCommand;

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get  
        public bool GetByTransNo()
        {
            
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("ReconBankSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            if (oDS.Tables[0].Rows.Count > 0)
            {
                this.strBankAcctCode = oDS.Tables[0].Rows[0]["BankAcctCode"].ToString();
                this.strCashBookAcct = oDS.Tables[0].Rows[0]["CashBookAcct"].ToString();
                return true;
            }
            else
            {
                return false;
            }
            
        }
        #endregion

        #region Get
        public bool GetByBankCode()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("ReconBankSelectByBankCode") as SqlCommand;
            db.AddInParameter(dbCommand, "BankAcctCode", SqlDbType.VarChar, strBankAcctCode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            if (oDS.Tables[0].Rows.Count > 0)
            {
                this.strTransNo = oDS.Tables[0].Rows[0]["TransNo"].ToString();
                this.strCashBookAcct = oDS.Tables[0].Rows[0]["CashBookAcct"].ToString();
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ReconBankDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());            
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Check Bank Name Exist
        public bool ChkNameExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ReconBankChkNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.Char, strTransNo.Trim());
            db.AddInParameter(oCommand, "BankAcctCode", SqlDbType.VarChar, strBankAcctCode.Trim());
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

        #region Check GL Account Exist
        public bool ChkGLAcctExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ReconBankChkGLAcctExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.Char, strTransNo.Trim());
            db.AddInParameter(oCommand, "CashBookAcct", SqlDbType.VarChar, strCashBookAcct.Trim());
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

        #region Check TransNo Exist
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("ReconBankChkTransNoExist") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
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
    }
}
