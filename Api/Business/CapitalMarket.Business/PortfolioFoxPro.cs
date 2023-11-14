using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using System.Data.SqlTypes;
using BaseUtility.Business;

namespace CapitalMarket.Business
{
    public class PortfolioFoxPro
    {
        #region Declaration
        private DateTime datEffDate;
        private string strCustNo;
        private string strStockCode;
        private Int64 intPendingUnit;
        private Int64 intAvailableUnit;
       
        #endregion

        #region Properties
        public DateTime EffDate
        {
            set { datEffDate = value; }
            get { return datEffDate; }
        }
        
        public string CustNo
        {
            set { strCustNo = value; }
            get { return strCustNo; }
        }
        public string StockCode
        {
            set { strStockCode = value; }
            get { return strStockCode; }
        }
        public Int64 PendingUnit
        {
            set { intPendingUnit = value; }
            get { return intPendingUnit; }
        }
        public Int64 AvailableUnit
        {
            set { intAvailableUnit = value; }
            get { return intAvailableUnit; }
        }
        

        #endregion

        #region Add New Portfolio and Return A Command
        public SqlCommand AddCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PortfolioFoxProAdd") as SqlCommand;
            db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, datEffDate);
            db.AddInParameter(oCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(oCommand, "PendingUnit", SqlDbType.BigInt, intPendingUnit);
            db.AddInParameter(oCommand, "AvailableUnit", SqlDbType.BigInt, intAvailableUnit);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar,GeneralFunc.UserName.Trim());

            return oCommand;
        }
        #endregion
    }
}
