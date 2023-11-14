using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using BaseUtility.Business;

namespace CapitalMarket.Business
{
    public class SendPortfolioToEmail
    {
        IFormatProvider format = new CultureInfo("en-GB");

        #region Declaration
        private Int64 intTransNo;
        private DateTime datUploadDate;
        #endregion

        #region Properties
        public Int64 TransNo
        {
            set { intTransNo = value; }
            get { return intTransNo; }
        }
        public DateTime UploadDate
        {
            set { datUploadDate = value; }
            get { return datUploadDate; }
        }

        #endregion

        #region Add New UploadDate
        public bool Add()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("SendPortfolioToEmailAdd") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            db.AddInParameter(oCommand, "UploadDate", SqlDbType.DateTime, datUploadDate);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Get Portfolio Upload Date
        public bool GetPortfolioUploadDate()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("SendPortfolioToEmailSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "UploadDate", SqlDbType.DateTime, datUploadDate);

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                intTransNo = Int64.Parse(thisRow[0]["TransNo"].ToString());
                datUploadDate = DateTime.ParseExact(thisRow[0]["UploadDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }

            return blnStatus;
        }
        #endregion

        #region Get All 
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("SendPortfolioToEmailSelectAll") as SqlCommand;
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

       
    }
}
