using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;

namespace CapitalMarket.Business
{
    public class ChangeUnitCost
    {
        #region Declaration
        private Int64 intTransNo, intMasterNo;
        private DateTime datEffDate;
        private string strCustomerId;
        private string strStockCode;
        private float fltOldCost;
        private float fltNewCost;
        private bool blnBatchAgent;
        private string strSaveType;
        #endregion

        #region Properties

        public Int64 TransNo
        {
            set { intTransNo = value; }
            get { return intTransNo; }
        }
        public Int64 MasterNo
        {
            set { intMasterNo = value; }
            get { return intMasterNo; }
        }
        public DateTime EffDate
        {
            set { datEffDate = value; }
            get { return datEffDate; }
        }
        public string CustomerId
        {
            set { strCustomerId = value; }
            get { return strCustomerId; }
        }
        public string Stockcode
        {
            set { strStockCode = value; }
            get { return strStockCode; }
        }
        public float OldCost
        {
            set { fltOldCost = value; }
            get { return fltOldCost; }
        }
        public float NewCost
        {
            set { fltNewCost = value; }
            get { return fltNewCost; }
        }
        public bool BatchAgent
        {
            set { blnBatchAgent = value; }
            get { return blnBatchAgent; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        #endregion

        #region Save And Return Command
        public SqlCommand SaveCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (strSaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("ChangeUnitCostAdd") as SqlCommand;
            }
            else if (strSaveType == "EDIT")
            {
                if (intTransNo != 0)
                {
                    oCommand = db.GetStoredProcCommand("ChangeUnitCostEdit") as SqlCommand;
                }
                else
                {
                    oCommand = db.GetStoredProcCommand("ChangeUnitCostAdd") as SqlCommand;
                }
            }
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            db.AddInParameter(oCommand, "MasterNo", SqlDbType.BigInt, intMasterNo);
            db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, datEffDate);
            db.AddInParameter(oCommand, "CustomerId", SqlDbType.VarChar, strCustomerId.Trim());
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar,strStockCode.Trim());
            db.AddInParameter(oCommand, "OldCost", SqlDbType.Float, fltOldCost);
            db.AddInParameter(oCommand, "NewCost", SqlDbType.Float, fltNewCost);
            db.AddInParameter(oCommand, "BatchAgent", SqlDbType.Bit, blnBatchAgent);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Get
        public DataSet GetChangeUnitCostByMasterNo()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustomerBankSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "MasterNo", SqlDbType.BigInt, intMasterNo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion
    }
}
