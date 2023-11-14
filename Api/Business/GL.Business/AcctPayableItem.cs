using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class AcctPayableItem
    {
        #region Declaration
        private string strTransNo, strDescrip;
        private int intAcctPayable;
        private decimal decAmount;
        private string strUserID, strSaveType;
        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public int AcctPayable
        {
            set { intAcctPayable = value; }
            get { return intAcctPayable; }
        }
        public string Descrip
        {
            set { strDescrip = value; }
            get { return strDescrip; }
        }
        public decimal Amount
        {
            set { decAmount = value; }
            get { return decAmount; }
        }
        public string UserID
        {
            set { strUserID = value; }
            get { return strUserID; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }

        #endregion

        #region Add To Temp Table Return Command
        public SqlCommand AddTempCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("AcctPayableItemTempAdd") as SqlCommand;
            db.AddInParameter(dbCommand, "AcctPayable", SqlDbType.BigInt, intAcctPayable);
            db.AddInParameter(dbCommand, "Descrip", SqlDbType.VarChar, strDescrip.Trim());
            db.AddInParameter(dbCommand, "Amount", SqlDbType.Money, decAmount);
            return dbCommand;
        }
        #endregion

        #region Add Return Command
        public SqlCommand AddCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("AcctPayableItemAdd") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo);
            db.AddInParameter(dbCommand, "AcctPayable", SqlDbType.BigInt, intAcctPayable);
            db.AddInParameter(dbCommand, "Descrip", SqlDbType.VarChar, strDescrip.Trim());
            db.AddInParameter(dbCommand, "Amount", SqlDbType.Money, decAmount);
            db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return dbCommand;
        }
        #endregion


        #region Delete By AcctPayable Temp Table Return Command
        public SqlCommand DeleteTempByAcctPayableCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("AcctPayableItemTempDeleteByAcctPayable") as SqlCommand;
            db.AddInParameter(dbCommand, "AcctPayable", SqlDbType.BigInt, intAcctPayable);
            return dbCommand;
        }
        #endregion

        #region Delete By AcctPayable Return Command
        public SqlCommand DeleteByAcctPayableCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("AcctPayableItemDeleteByAcctPayable") as SqlCommand;
            db.AddInParameter(dbCommand, "AcctPayable", SqlDbType.BigInt, intAcctPayable);
            return dbCommand;
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
                oCommand = db.GetStoredProcCommand("AcctPayableItemChkTransNoExist") as SqlCommand;
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

        #region Get All
        public DataSet GetAll(string strOrderBy)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strOrderBy.Trim() == "NAME")
            {
                dbCommand = db.GetStoredProcCommand("AcctPayableItemSelectAllOrderByName") as SqlCommand;
            }
            else
            {
                dbCommand = db.GetStoredProcCommand("AcctPayableItemSelectAll") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By AcctPayable
        public DataSet GetAllByAcctPayable(string strOrderBy)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strOrderBy.Trim() == "NAME")
            {
                dbCommand = db.GetStoredProcCommand("AcctPayableItemSelectAllByAcctPayableOrderByName") as SqlCommand;
            }
            else
            {
                dbCommand = db.GetStoredProcCommand("AcctPayableItemSelectAllByAcctPayable") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "AcctPayable", SqlDbType.BigInt, intAcctPayable);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All In Temp Table By AcctPayable
        public DataSet GetTempAllByAcctPayable()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;

            dbCommand = db.GetStoredProcCommand("AcctPayableItemTempSelectAllByAcctPayable") as SqlCommand;

            db.AddInParameter(dbCommand, "AcctPayable", SqlDbType.BigInt, intAcctPayable);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get AcctPayableItem
        public bool GetAcctPayableItem()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AcctPayableItemSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                intAcctPayable = int.Parse(thisRow[0]["AcctPayable"].ToString());
                strDescrip = thisRow[0]["Descrip"].ToString();
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString());
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Check Name Exist
        public bool ChkNameExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AcctPayableItemChkNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.Char, strTransNo.Trim());
            db.AddInParameter(oCommand, "AcctPayable", SqlDbType.BigInt, intAcctPayable);
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

        #region Delete
        public DataGeneral.SaveStatus Delete()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AcctPayableItemDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion
    }
}
