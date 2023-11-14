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
    public class RecomputePropSale
    {
        #region Declaration
        private Int64 intTransNo;
        private DateTime datFromDate, datToDate;
        private string strCustomerId;
        private string strSaveType;
        #endregion

        #region Properties

        public Int64 TransNo
        {
            set { intTransNo = value; }
            get { return intTransNo; }
        }
        
        public DateTime FromDate
        {
            set { datFromDate = value; }
            get { return datFromDate; }
        }

        public DateTime ToDate
        {
            set { datToDate = value; }
            get { return datToDate; }
        }
        public string CustomerId
        {
            set { strCustomerId = value; }
            get { return strCustomerId; }
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
                oCommand = db.GetStoredProcCommand("RecomputePropSaleAdd") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intTransNo);
                db.AddInParameter(oCommand, "FromDate", SqlDbType.DateTime, datFromDate);
                db.AddInParameter(oCommand, "ToDate", SqlDbType.DateTime, datToDate);
                db.AddInParameter(oCommand, "CustomerId", SqlDbType.VarChar, strCustomerId.Trim());
                db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            }
            return oCommand;
        }
        #endregion
    }
}

