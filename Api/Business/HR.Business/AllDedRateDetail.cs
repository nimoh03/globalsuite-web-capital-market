using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace HR.Business
{
    public class AllDedRateDetail
    {
        #region Declaration
        private string strTransNo;
        private int intAllDedRate;
        private int intAllDed;
        private string strRateType;
        private float fltRate;
        private decimal decAmount;
        private string strPayAmountPeriod;
        private string strUserID;
        private string strSaveType;

        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        
        public int AllDedRate
        {
            set { intAllDedRate = value; }
            get { return intAllDedRate; }
        }

        public int AllDed
        {
            set { intAllDed = value; }
            get { return intAllDed; }
        }
        public string RateType
        {
            set { strRateType = value; }
            get { return strRateType; }
        }
        public float Rate
        {
            set { fltRate = value; }
            get { return fltRate; }
        }
        public decimal Amount
        {
            set { decAmount = value; }
            get { return decAmount; }
        }
        public string PayAmountPeriod
        {
            set { strPayAmountPeriod = value; }
            get { return strPayAmountPeriod; }
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

        #region Add Command
        public SqlCommand AddCommand(int oAllDedRate)
        {
            if (ChkNameExist(oAllDedRate))
            {
                throw new Exception("Pay Item Rate For This Grade/Step Already Exist");
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strSaveType.Trim() == "ADDS")
            {
                dbCommand = db.GetStoredProcCommand("AllDedRateDetailAdd") as SqlCommand;
                db.AddInParameter(dbCommand, "AllDedRate", SqlDbType.Int, oAllDedRate);
            }
            else if (strSaveType.Trim() == "EDIT")
            {
                dbCommand = db.GetStoredProcCommand("AllDedRateDetailEdit") as SqlCommand;
                db.AddInParameter(dbCommand, "AllDedRate", SqlDbType.Int, intAllDedRate);
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "AllDed", SqlDbType.Int, intAllDed);
            db.AddInParameter(dbCommand, "RateType", SqlDbType.VarChar, strRateType.Trim());
            db.AddInParameter(dbCommand, "Rate", SqlDbType.Real, fltRate);
            db.AddInParameter(dbCommand, "Amount", SqlDbType.Decimal, decAmount);
            db.AddInParameter(dbCommand, "PayAmountPeriod", SqlDbType.VarChar, strPayAmountPeriod.Trim());
            db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return dbCommand;
        }
        #endregion

        #region Edit Command
        public SqlCommand EditCommand(int oAllDedRate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (ChkNameExist(oAllDedRate))
            {
                dbCommand = db.GetStoredProcCommand("AllDedRateDetailEdit") as SqlCommand;
            }
            else
            {
                dbCommand = db.GetStoredProcCommand("AllDedRateDetailAdd") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "AllDedRate", SqlDbType.Int, intAllDedRate);
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "AllDed", SqlDbType.Int, intAllDed);
            db.AddInParameter(dbCommand, "RateType", SqlDbType.VarChar, strRateType.Trim());
            db.AddInParameter(dbCommand, "Rate", SqlDbType.Real, fltRate);
            db.AddInParameter(dbCommand, "Amount", SqlDbType.Decimal, decAmount);
            db.AddInParameter(dbCommand, "PayAmountPeriod", SqlDbType.VarChar, strPayAmountPeriod.Trim());
            db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
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
                oCommand = db.GetStoredProcCommand("AllDedRateDetailChkTransNoExist") as SqlCommand;
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
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("OccupationRateSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Allowances and Deductions Rate Detail Given TransNo
        public bool GetAllDedRateDetail()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("OccupationRateSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                intAllDedRate = int.Parse(thisRow[0]["AllDedRate"].ToString());
                strRateType = thisRow[0]["RateType"].ToString();
                fltRate = float.Parse(thisRow[0]["Rate"].ToString());
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString());
                strPayAmountPeriod = thisRow[0]["PayAmountPeriod"].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get Allowances and Deductions Rate Detail Given Allowance and Deduction Rate And Allowance and Deduction Type
        public bool GetAllDedRateDetailByAllDedRateWithAllDed()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllDedRateDetailSelectAllDedRateWithAllDed") as SqlCommand;
            db.AddInParameter(dbCommand, "AllDedRate", SqlDbType.Int, intAllDedRate);
            db.AddInParameter(dbCommand, "AllDed", SqlDbType.Int, intAllDed);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                intAllDedRate = int.Parse(thisRow[0]["AllDedRate"].ToString());
                strRateType = thisRow[0]["RateType"].ToString();
                fltRate = float.Parse(thisRow[0]["Rate"].ToString());
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString());
                strPayAmountPeriod = thisRow[0]["PayAmountPeriod"].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get Allowances and Deductions Rate Details Given Allowance and Deduction Rate
        public DataSet GetAllDedRateDetailsByAllDedRate()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllDedRateDetailSelectAllDedRate") as SqlCommand;
            db.AddInParameter(dbCommand, "AllDedRate", SqlDbType.Int,intAllDedRate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Allowances and Deductions Rate Details Given Allowance and Deduction Type
        public DataSet GetAllDedRateDetailByAllDed()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllDedRateDetailSelectAllDed") as SqlCommand;
            db.AddInParameter(dbCommand, "AllDed", SqlDbType.Int, intAllDed);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Check Name Exist
        public bool ChkNameExist(int oAllDedRate)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AllDedRateDetailChkNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.Char, strTransNo.Trim());
            db.AddInParameter(oCommand, "AllDedRate", SqlDbType.Int, oAllDedRate);
            db.AddInParameter(oCommand, "AllDed", SqlDbType.Int, intAllDed);
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
            SqlCommand oCommand = db.GetStoredProcCommand("AllDedRateDetailDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Delete By AllDedRate And AllDed And Return Command
        public SqlCommand DeleteByAllDedRateAndAllDedReturnCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AllDedRateDetailDeleteByAllDedRateAndAllDed") as SqlCommand;
            db.AddInParameter(oCommand, "AllDedRate", SqlDbType.Int, intAllDedRate);
            db.AddInParameter(oCommand, "AllDed", SqlDbType.Int, intAllDed);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
            return oCommand;
        }
        #endregion
    }
}
