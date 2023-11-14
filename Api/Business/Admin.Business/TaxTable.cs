using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace Admin.Business
{
    public class TaxTable
    {
        #region Declaration
        private string strTransNo;
        private decimal decAmount;
        private decimal decAdditionalAmount;
        private float fltRate;
        private string strUserID;
        private string strSaveType;

        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public decimal Amount
        {
            set { decAmount = value; }
            get { return decAmount; }
        }
        public decimal AdditionalAmount
        {
            set { decAdditionalAmount = value; }
            get { return decAdditionalAmount; }
        }
        public float Rate
        {
            set { fltRate = value; }
            get { return fltRate; }
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


        #region Save
        public DataGeneral.SaveStatus Save()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            if (!ChkTransNoExist())
            {
                enSaveStatus = DataGeneral.SaveStatus.NotExist;
                return enSaveStatus;
            }
            if (ChkAmountAgainstExist())
            {
                enSaveStatus = DataGeneral.SaveStatus.NameExistAdd;
                return enSaveStatus;
            }
            
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strSaveType.Trim() == "ADDS")
            {
                dbCommand = db.GetStoredProcCommand("TaxTableAddNew") as SqlCommand;
            }
            else if (strSaveType.Trim() == "EDIT")
            {
                dbCommand = db.GetStoredProcCommand("TaxTableEdit") as SqlCommand;
            }

            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "Amount", SqlDbType.Money, decAmount);
            db.AddInParameter(dbCommand, "AdditionalAmount", SqlDbType.Money, decAdditionalAmount);
            db.AddInParameter(dbCommand, "Rate", SqlDbType.VarChar, fltRate);
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
                oCommand = db.GetStoredProcCommand("TaxTableChkTransNoExist") as SqlCommand;
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

        #region Check Amount Against Existing Amount
        public bool ChkAmountAgainstExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("TaxTableChkAgainstAmount") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.Char, strTransNo.Trim());
            db.AddInParameter(oCommand, "Amount", SqlDbType.Money, decAmount);
            db.AddInParameter(oCommand, "SaveType", SqlDbType.VarChar, strSaveType.Trim());
            db.AddOutParameter(oCommand, "ChkResult", SqlDbType.Bit,1);
            db.ExecuteNonQuery(oCommand);
            blnStatus = bool.Parse(db.GetParameterValue(oCommand,"ChkResult").ToString());
            return blnStatus;
        }
        #endregion

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("TaxTableSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Tax Table
        public bool GetTaxTable()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("TaxTableSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString());
                decAdditionalAmount = decimal.Parse(thisRow[0]["AdditionalAmount"] != null && thisRow[0]["AdditionalAmount"].ToString().Trim() != "" ? thisRow[0]["AdditionalAmount"].ToString() : "0");
                fltRate = float.Parse(thisRow[0]["Rate"].ToString());
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion


        #region Get Tax Table Maximum TransNo
        public int GetTaxTableMaxTransNo()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("TaxTableSelectMaxTransNo") as SqlCommand;
            return int.Parse(db.ExecuteScalar(dbCommand).ToString());
        }
        #endregion


        
        
    }
}
