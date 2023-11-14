using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class SMSHistory
    {
        #region Declarations
        private string strTransNo;
	    private string strOperationType;
	    private string strOperationNumber;
	    private DateTime datEffDate;
        private string strProduct;
	    private string strCustomer;
        private string strCustomerPhone;
	    private string strMessageSent;
        private long lngSMSReturnNumber;
        #endregion

        #region Properties

        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public string OperationType
        {
            set { strOperationType = value; }
            get { return strOperationType; }
        }

        public string OperationNumber
        {
            set { strOperationNumber = value; }
            get { return strOperationNumber; }
        }
        public DateTime EffDate
        {
            set { datEffDate = value; }
            get { return datEffDate; }
        }
        public string Product
        {
            set { strProduct = value; }
            get { return strProduct; }
        }
        public string Customer
        {
            set { strCustomer = value; }
            get { return strCustomer; }
        }
        public string CustomerPhone
        {
            set { strCustomerPhone = value; }
            get { return strCustomerPhone; }
        }
        public string MessageSent
        {
            set { strMessageSent = value; }
            get { return strMessageSent; }
        }
        public long SMSReturnNumber
        {
            set { lngSMSReturnNumber = value; }
            get { return lngSMSReturnNumber; }
        }
        #endregion

        #region Save
        public void Save()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("SMSHistoryAdd") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, "");
            db.AddInParameter(oCommand, "OperationType", SqlDbType.VarChar, strOperationType.ToUpper().Trim());
            db.AddInParameter(oCommand, "OperationNumber", SqlDbType.VarChar, strOperationNumber.Trim());
            db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, datEffDate);
            db.AddInParameter(oCommand, "Product", SqlDbType.VarChar, strProduct.Trim());
            db.AddInParameter(oCommand, "Customer", SqlDbType.VarChar, strCustomer.Trim());
            db.AddInParameter(oCommand, "CustomerPhone", SqlDbType.VarChar, strCustomerPhone.Trim());
            db.AddInParameter(oCommand, "MessageSent", SqlDbType.VarChar, strMessageSent.Trim());
            db.AddInParameter(oCommand, "SMSReturnNumber", SqlDbType.BigInt, lngSMSReturnNumber);
            db.ExecuteNonQuery(oCommand);

        }
        #endregion

        #region Save And Return Command
        public SqlCommand SaveCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("SMSHistoryAdd") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, "");
            db.AddInParameter(oCommand, "OperationType", SqlDbType.VarChar, strOperationType.ToUpper().Trim());
            db.AddInParameter(oCommand, "OperationNumber", SqlDbType.VarChar, strOperationNumber.Trim());
            db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, datEffDate);
            db.AddInParameter(oCommand, "Product", SqlDbType.VarChar, strProduct.Trim());
            db.AddInParameter(oCommand, "Customer", SqlDbType.VarChar, strCustomer.Trim());
            db.AddInParameter(oCommand, "CustomerPhone", SqlDbType.VarChar, strCustomerPhone.Trim());
            db.AddInParameter(oCommand, "MessageSent", SqlDbType.VarChar, strMessageSent.Trim());
            db.AddInParameter(oCommand, "SMSReturnNumber", SqlDbType.BigInt, lngSMSReturnNumber);
            return oCommand;

        }
        #endregion

        #region Get SMS Number By Customer
        public DataSet GetSMSNumberByCustomer()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("SMSHistorySelectSMSNumberByCustomer") as SqlCommand;
            db.AddInParameter(dbCommand, "EffDateFrom", SqlDbType.DateTime, DateTime.ParseExact("01" + "/" +
                DateTime.ParseExact(datEffDate.ToString().Substring(0, 10), "dd/MM/yyyy", format).Month.ToString().PadLeft(2, char.Parse("0")) + "/" +
                        DateTime.ParseExact(datEffDate.ToString().Substring(0, 10), "dd/MM/yyyy", format).Year.ToString(),"dd/MM/yyyy",format));
            db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime, datEffDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

    }
}
