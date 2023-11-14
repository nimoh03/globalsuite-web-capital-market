using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class AcctReceivableItem
    {
        #region Declaration
        private string strTransNo,  strDescrip;
        private int intAcctReceivable;
        private decimal decAmount;
        private string strUserID, strSaveType;
        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public int AcctReceivable
        {
            set { intAcctReceivable = value; }
            get { return intAcctReceivable; }
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
            dbCommand = db.GetStoredProcCommand("AcctReceivableItemTempAdd") as SqlCommand;
            db.AddInParameter(dbCommand, "AcctReceivable", SqlDbType.BigInt, intAcctReceivable);
            db.AddInParameter(dbCommand, "Descrip", SqlDbType.VarChar, strDescrip.Trim());
            db.AddInParameter(dbCommand, "Amount", SqlDbType.Money,decAmount);
            return dbCommand;
        }
        #endregion

        #region Add Return Command
        public SqlCommand AddCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("AcctReceivableItemAdd") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo);
            db.AddInParameter(dbCommand, "AcctReceivable", SqlDbType.BigInt, intAcctReceivable);
            db.AddInParameter(dbCommand, "Descrip", SqlDbType.VarChar, strDescrip.Trim());
            db.AddInParameter(dbCommand, "Amount", SqlDbType.Money,decAmount);
            db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return dbCommand;
        }
        #endregion


        #region Delete By AcctReceivable Temp Table Return Command
        public SqlCommand DeleteTempByAcctReceivableCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("AcctReceivableItemTempDeleteByAcctReceivable") as SqlCommand;
            db.AddInParameter(dbCommand, "AcctReceivable", SqlDbType.BigInt, intAcctReceivable);
            return dbCommand;
        }
        #endregion

        #region Delete By AcctReceivable Return Command
        public SqlCommand DeleteByAcctReceivableCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("AcctReceivableItemDeleteByAcctReceivable") as SqlCommand;
            db.AddInParameter(dbCommand, "AcctReceivable", SqlDbType.BigInt, intAcctReceivable);
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
                oCommand = db.GetStoredProcCommand("AcctReceivableItemChkTransNoExist") as SqlCommand;
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
                dbCommand = db.GetStoredProcCommand("AcctReceivableItemSelectAllOrderByName") as SqlCommand;
            }
            else
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivableItemSelectAll") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By AcctReceivable
        public DataSet GetAllByAcctReceivable(string strOrderBy)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strOrderBy.Trim() == "NAME")
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivableItemSelectAllByAcctReceivableOrderByName") as SqlCommand;
            }
            else
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivableItemSelectAllByAcctReceivable") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "AcctReceivable", SqlDbType.BigInt, intAcctReceivable);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All In Temp Table By AcctReceivable
        public DataSet GetTempAllByAcctReceivable()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;

            dbCommand = db.GetStoredProcCommand("AcctReceivableItemTempSelectAllByAcctReceivable") as SqlCommand;

            db.AddInParameter(dbCommand, "AcctReceivable", SqlDbType.BigInt, intAcctReceivable);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get AcctReceivableItem
        public bool GetAcctReceivableItem()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AcctReceivableItemSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                intAcctReceivable = int.Parse(thisRow[0]["AcctReceivable"].ToString());
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
            SqlCommand oCommand = db.GetStoredProcCommand("AcctReceivableItemChkNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.Char, strTransNo.Trim());
            db.AddInParameter(oCommand, "AcctReceivable", SqlDbType.BigInt, intAcctReceivable);
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
            SqlCommand oCommand = db.GetStoredProcCommand("AcctReceivableItemDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion
    }
}
