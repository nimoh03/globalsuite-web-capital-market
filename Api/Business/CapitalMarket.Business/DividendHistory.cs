using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using BaseUtility.Business;

namespace CapitalMarket.Business
{
    public class DividendHistory
    {
        #region Declarations
        private string strTransNo;
        private DateTime datEffDate, datDeclareDate;
        private decimal decDividendAmt, decDividendUnit;
        private string strCustomer,strStockCode, strDescription;
        private string strRef01, strTranstype, strSysRef;
        private string strSaveType;
        #endregion

        #region Properties

        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public decimal DividendAmt
        {
            set { decDividendAmt = value; }
            get { return decDividendAmt; }
        }
        public decimal DividendUnit
        {
            set { decDividendUnit = value; }
            get { return decDividendUnit; }
        }
        public DateTime EffDate
        {
            set { datEffDate = value; }
            get { return datEffDate; }
        }
        public DateTime DeclareDate
        {
            set { datDeclareDate = value; }
            get { return datDeclareDate; }
        }
        public string Customer
        {
            set { strCustomer = value; }
            get { return strCustomer; }
        }
        public string StockCode
        {
            set { strStockCode = value; }
            get { return strStockCode; }
        }

        public string Description
        {
            set { strDescription = value; }
            get { return strDescription; }
        }

        public string Ref01
        {
            set { strRef01 = value; }
            get { return strRef01; }
        }
        public string Transtype
        {
            set { strTranstype = value; }
            get { return strTranstype; }
        }
        public string SysRef
        {
            set { strSysRef = value; }
            get { return strSysRef; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }

        #endregion

        #region Add and Return Command
        public SqlCommand AddCommand()
        {
            
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            oCommand = db.GetStoredProcCommand("DividendHistoryAddNew") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "Customer", SqlDbType.VarChar, strCustomer.Trim());
            db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, datEffDate);
            db.AddInParameter(oCommand, "DeclareDate", SqlDbType.DateTime, datDeclareDate);
            db.AddInParameter(oCommand, "DividendAmt", SqlDbType.Decimal, decDividendAmt);
            db.AddInParameter(oCommand, "DividendUnit", SqlDbType.Decimal, decDividendUnit);
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(oCommand, "Description", SqlDbType.VarChar, strDescription.Trim());
            db.AddInParameter(oCommand, "Ref01", SqlDbType.VarChar, strRef01.Trim());
            db.AddInParameter(oCommand, "Transtype", SqlDbType.VarChar, strTranstype.Trim());
            db.AddInParameter(oCommand, "SysRef", SqlDbType.VarChar, strSysRef.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;

        }
        #endregion

        #region Delete Dividend History Selected By Sysref And Return SqlCommand
        public SqlCommand DeleteBySysrefCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("DividendHistoryReverseBySysref") as SqlCommand;
            db.AddInParameter(oCommand, "Sysref", SqlDbType.VarChar, strSysRef.Trim());
            db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, datEffDate);
            db.AddInParameter(oCommand, "Customer", SqlDbType.VarChar, strCustomer.Trim());
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(oCommand, "DividendAmt", SqlDbType.Decimal, decDividendAmt);

            return oCommand;
        }
        #endregion
    }
}
