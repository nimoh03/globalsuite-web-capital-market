using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace CapitalMarket.Business
{
    public class PortNot
    {
        #region Declaration
        private string strTransNo, strCsCsAcct, strCustId;
        private string strCustName, strStockCode, strPortDesc, strUserId;
        private DateTime datEffDate;
        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public string CsCsAcct
        {
            set { strCsCsAcct = value; }
            get { return strCsCsAcct; }
        }
        public string CustId
        {
            set { strCustId = value; }
            get { return strCustId; }
        }
        public string CustName
        {
            set { strCustName = value; }
            get { return strCustName; }
        }
        public string StockCode
        {
            set { strStockCode = value; }
            get { return strStockCode; }
        }
        public string PortDesc
        {
            set { strPortDesc = value; }
            get { return strPortDesc; }
        }
        public string UserId
        {
            set { strUserId = value; }
            get { return strUserId; }
        }
        public DateTime EffDate
        {
            set { datEffDate = value; }
            get { return datEffDate; }
        }
        #endregion


        #region Add New Portfolio Stock Not Enough - Used In CsCs Transaction Upload
        public bool Add()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortNotAddNew") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo);
            db.AddInParameter(dbCommand, "CsCsAcct", SqlDbType.VarChar, strCsCsAcct.Trim());
            db.AddInParameter(dbCommand, "CustId", SqlDbType.VarChar, strCustId.Trim());
            db.AddInParameter(dbCommand, "CustName", SqlDbType.VarChar, strCustName.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(dbCommand, "PortDesc", SqlDbType.VarChar, strPortDesc.Trim());
            db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime, datEffDate);
            db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, strUserId.Trim());
                db.ExecuteNonQuery(dbCommand);
                blnStatus = true;
            return blnStatus;

        }
        #endregion

        #region Get All Portfolio Stock Not Enough
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortNotSelectByAllOrderByTransno") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete All Portfolio Stock Not Enough
        public bool DeleteAll()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortNotDeleteAll") as SqlCommand;
            try
            {
                db.ExecuteNonQuery(dbCommand);
                blnStatus = true;
            }
            catch (Exception e)
            {
                string you = e.Message;
                blnStatus = false;
                return blnStatus;
            }
            return blnStatus;

        }
        #endregion
    }
}
