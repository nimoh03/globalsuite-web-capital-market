using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class ReconStmtPosition
    {
        #region Declaration

        private string strTransNo, strSaveType, strBankAcctCode;
        private int intTransDatePos, intValueDatePos, intDescriptionPos, intCreditPos, intDebitPos, intDebCredPos,  intTransTypePos;

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

        public string BankAcctCode
        {
            set { strBankAcctCode = value; }
            get { return strBankAcctCode; }
        }

        public int TransDatePos
        {
            set { intTransDatePos = value; }
            get { return intTransDatePos; }
        }
        public int ValueDatePos
        {
            set { intValueDatePos = value; }
            get { return intValueDatePos; }
        }



        public int DescriptionPos
        {
            set { intDescriptionPos = value; }
            get { return intDescriptionPos; }
        }

        public int CreditPos
        {
            set { intCreditPos = value; }
            get { return intCreditPos; }
        }
        public int DebitPos
        {
            set { intDebitPos = value; }
            get { return intDebitPos; }
        }

        public int DebCredPos
        {
            set { intDebCredPos = value; }
            get { return intDebCredPos; }
        }

        public int TransTypePos
        {
            set { intTransTypePos = value; }
            get { return intTransTypePos; }
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
            SqlCommand oCommand = null;
            if (strSaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("ReconStmtPositionAdd") as SqlCommand;
            }
            else
            {
                oCommand = db.GetStoredProcCommand("ReconStmtPositionEdit") as SqlCommand;
            }
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "BankAcctCode", SqlDbType.VarChar, strBankAcctCode.Trim());
            db.AddInParameter(oCommand, "TransDatePos", SqlDbType.Int, intTransDatePos);
            db.AddInParameter(oCommand, "ValueDatePos", SqlDbType.Int, intValueDatePos);
            db.AddInParameter(oCommand, "DescriptionPos", SqlDbType.Int, intDescriptionPos);
            db.AddInParameter(oCommand, "CreditPos", SqlDbType.Int, intCreditPos);
            db.AddInParameter(oCommand, "DebitPos", SqlDbType.Int, intDebitPos);
            db.AddInParameter(oCommand, "DebCredPos", SqlDbType.Int, intDebCredPos);
            db.AddInParameter(oCommand, "AmountPos", SqlDbType.Int, 0);
            db.AddInParameter(oCommand, "TransTypePos", SqlDbType.Int, intTransTypePos);
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);

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
                oCommand = db.GetStoredProcCommand("ReconStmtPositionChkTransNoExist") as SqlCommand;
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

        #region Check Bank Name Exist
        public bool ChkNameExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ReconStmtPositionChkNameExist") as SqlCommand;
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


        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("ReconStmtPositionSelectAll") as SqlCommand;

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion


        #region Get
        public bool GetByTransNo()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("ReconStmtPositionSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            if (oDS.Tables[0].Rows.Count > 0)
            {
                this.strBankAcctCode = oDS.Tables[0].Rows[0]["BankAcctCode"].ToString();
                this.intTransDatePos = int.Parse(oDS.Tables[0].Rows[0]["TransDatePos"].ToString());
                this.intValueDatePos = int.Parse(oDS.Tables[0].Rows[0]["ValueDatePos"].ToString());
                this.intDescriptionPos = int.Parse(oDS.Tables[0].Rows[0]["DescriptionPos"].ToString());
                this.intCreditPos = int.Parse(oDS.Tables[0].Rows[0]["CreditPos"].ToString());
                this.intDebitPos = int.Parse(oDS.Tables[0].Rows[0]["DebitPos"].ToString());
                this.intDebCredPos = int.Parse(oDS.Tables[0].Rows[0]["DebCredPos"].ToString());
                this.intTransTypePos = int.Parse(oDS.Tables[0].Rows[0]["TransTypePos"].ToString());
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
            dbCommand = db.GetStoredProcCommand("ReconStmtPositionSelectByBankCode") as SqlCommand;
            db.AddInParameter(dbCommand, "BankAcctCode", SqlDbType.VarChar, strBankAcctCode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            if (oDS.Tables[0].Rows.Count > 0)
            {
                this.strTransNo = oDS.Tables[0].Rows[0]["TransNo"].ToString();
                this.intTransDatePos = int.Parse(oDS.Tables[0].Rows[0]["TransDatePos"].ToString());
                this.intValueDatePos = int.Parse(oDS.Tables[0].Rows[0]["ValueDatePos"].ToString());
                this.intDescriptionPos = int.Parse(oDS.Tables[0].Rows[0]["DescriptionPos"].ToString());
                this.intCreditPos = int.Parse(oDS.Tables[0].Rows[0]["CreditPos"].ToString());
                this.intDebitPos = int.Parse(oDS.Tables[0].Rows[0]["DebitPos"].ToString());
                this.intDebCredPos = int.Parse(oDS.Tables[0].Rows[0]["DebCredPos"].ToString());
                this.intTransTypePos = int.Parse(oDS.Tables[0].Rows[0]["TransTypePos"].ToString());
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
            SqlCommand oCommand = db.GetStoredProcCommand("ReconStmtPositionDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion
    }
}
