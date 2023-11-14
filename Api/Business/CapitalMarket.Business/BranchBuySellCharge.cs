using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.SqlClient;
using System.Data;
using BaseUtility.Business;

namespace CapitalMarket.Business
{
    public class BranchBuySellCharge
    {
        #region Declarations
        private string strTransNo;
        private decimal decCommissionBuy, decCommVATBuy, decCommissionSell;
        private decimal decCommVATSell, decTotalFeeBranchBuy;
        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public decimal CommissionBuy
        {
            set { decCommissionBuy = value; }
            get { return decCommissionBuy; }
        }
        public decimal CommVATBuy
        {
            set { decCommVATBuy = value; }
            get { return decCommVATBuy; }
        }
        public decimal CommissionSell
        {
            set { decCommissionSell = value; }
            get { return decCommissionSell; }
        }
        public decimal CommVATSell
        {
            set { decCommVATSell = value; }
            get { return decCommVATSell; }
        }
        public decimal TotalFeeBranchBuy
        {
            set { decTotalFeeBranchBuy = value; }
            get { return decTotalFeeBranchBuy; }
        }

        
        #endregion


        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BranchBuySellChargeSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Save
        public bool Save()
        {
            bool blnResult = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            oCommand = db.GetStoredProcCommand("BranchBuySellChargeAdd") as SqlCommand;
            db.AddInParameter(oCommand, "CommissionBuy", SqlDbType.Decimal, decCommissionBuy);
            db.AddInParameter(oCommand, "CommVATBuy", SqlDbType.Decimal, decCommVATBuy);
            db.AddInParameter(oCommand, "CommissionSell", SqlDbType.Decimal, decCommissionSell);
            db.AddInParameter(oCommand, "CommVATSell", SqlDbType.Decimal, decCommVATSell);
            db.AddInParameter(oCommand, "TotalFeeBranchBuy", SqlDbType.Decimal, decTotalFeeBranchBuy);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            blnResult = true;
            return blnResult;
        }

        #endregion

        #region Get BranchBuySellCharges
        public bool GetBranchBuySellCharges()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BranchBuySellChargeSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                decCommissionBuy = decimal.Parse(thisRow[0]["CommissionBuy"].ToString());
                decCommVATBuy = decimal.Parse(thisRow[0]["CommVATBuy"].ToString());
                decCommissionSell = decimal.Parse(thisRow[0]["CommissionSell"].ToString());
                decCommVATSell = decimal.Parse(thisRow[0]["CommVATSell"].ToString());
                decTotalFeeBranchBuy = decimal.Parse(thisRow[0]["TotalFeeBranchBuy"].ToString());
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion
        


    }
}

