using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class ReactivateDormantAC
    {
        #region Declaration
        private Int64 intTransNo;
        private DateTime datReacivatedDate;
        private string strProductCode, strCustAID;
        #endregion

        #region Properties
        public Int64 TransNo
        {
            set { intTransNo = value; }
            get { return intTransNo; }
        }
        public DateTime ReacivatedDate
        {
            set { datReacivatedDate = value; }
            get { return datReacivatedDate; }
        }
        public string ProductCode
        {
            set { strProductCode = value; }
            get { return strProductCode; }
        }
        public string CustAID
        {
            set { strCustAID = value; }
            get { return strCustAID; }
        }
        #endregion


        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ReactivateDormantACSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get  Reactivate Dormant AC
        public bool GetReactivateDormantAC()
        {
            bool blnStatus = false;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
            dtfi.ShortDatePattern = "dd/MM/yyyy";
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ReactivateDormantACSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                datReacivatedDate = DateTime.ParseExact(DateTime.Parse(thisRow[0]["ReacivatedDate"].ToString()).ToString("d", dtfi), "dd/MM/yyyy", format);
                strProductCode = thisRow[0]["ProductCode"].ToString();
                strCustAID = thisRow[0]["CustAID"].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion
    }
}
