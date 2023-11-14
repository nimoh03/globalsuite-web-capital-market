using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class CustomerFieldData
    {
        #region Declarations
        private string strCustomerFieldId;
        private string strCustomerFieldName;
        private int intCustomerFieldDataType;
        private string strCustomerFormInput;
        
        private string strTableName;
        #endregion

        #region Properties

        public string CustomerFieldId
        {
            set { strCustomerFieldId = value; }
            get { return strCustomerFieldId; }
        }
        public string CustomerFieldName
        {
            set { strCustomerFieldName = value; }
            get { return strCustomerFieldName; }
        }
        public int CustomerFieldDataType
        {
            set { intCustomerFieldDataType = value; }
            get { return intCustomerFieldDataType; }
        }
        public string CustomerFormInput
        {
            set { strCustomerFormInput = value; }
            get { return strCustomerFormInput; }
        }
        public string TableName
        {
            set { strTableName = value; }
            get { return strTableName; }
        }
        #endregion

        public class KYCCompulsoryCustomer
        {
            public string CustomerFieldId { get; set; }
            public string CustomerFieldName { get; set; }
            public int CustomerFieldDataType { get; set; }
            
            public string TableName { get; set; }
            
            #region Save Return Command
            public SqlCommand SaveCommand()
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("KYCCompulsoryCustomerAdd") as SqlCommand;
                db.AddInParameter(oCommand, "CustomerFieldId", SqlDbType.NVarChar, CustomerFieldId.Trim());
                db.AddInParameter(oCommand, "CustomerFieldName", SqlDbType.NVarChar, CustomerFieldName.Trim());
                db.AddInParameter(oCommand, "CustomerFieldDataType", SqlDbType.Int, CustomerFieldDataType);
                db.AddInParameter(oCommand, "TableName", SqlDbType.NVarChar, TableName.Trim());
                db.AddInParameter(oCommand, "UserId", SqlDbType.NVarChar, GeneralFunc.UserName);
                return oCommand;
            }
            #endregion

            #region Delete All Return Command
            public SqlCommand DeleteAllCommand()
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("KYCCompulsoryCustomerDeleteAll") as SqlCommand;
                return oCommand;
            }
            #endregion

            #region Delete Return Command
            public SqlCommand DeleteCommand()
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("KYCCompulsoryCustomerDelete") as SqlCommand;
                db.AddInParameter(oCommand, "CustomerFieldId", SqlDbType.NVarChar, CustomerFieldId.Trim());
                return oCommand;
            }
            #endregion

            #region Get All
            public DataSet GetAll()
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("KYCCompulsoryCustomerSelectAll") as SqlCommand;
                DataSet oDS = db.ExecuteDataSet(dbCommand);
                return oDS;
            }
            #endregion

            #region Get All KYC Compulsory Customer Return List
            public List<CustomerFieldData.KYCCompulsoryCustomer> GetAllReturnList()
            {
                List<CustomerFieldData.KYCCompulsoryCustomer> lstKYCCompulsoryCustomer = new List<CustomerFieldData.KYCCompulsoryCustomer>();
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("KYCCompulsoryCustomerSelectAll") as SqlCommand;
                DataSet oDS = db.ExecuteDataSet(dbCommand);
                foreach (DataRow oRow in oDS.Tables[0].Rows)
                {
                    CustomerFieldData.KYCCompulsoryCustomer oKYCCompulsoryCustomer = new CustomerFieldData.KYCCompulsoryCustomer();
                    oKYCCompulsoryCustomer.CustomerFieldId = oRow["CustomerFieldId"].ToString();
                    oKYCCompulsoryCustomer.CustomerFieldName = oRow["CustomerFieldName"].ToString();
                    lstKYCCompulsoryCustomer.Add(oKYCCompulsoryCustomer);
                }
                return lstKYCCompulsoryCustomer;
            }
            #endregion
        }

        public class KYCCompulsoryCustomerOptional
        {
            public string CustomerFieldId { get; set; }
            public string CustomerFieldName { get; set; }
            public int CustomerFieldDataType { get; set; }
            public string TableName { get; set; }
            #region Save Return Command
            public SqlCommand SaveCommand()
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("KYCCompulsoryCustomerAdd") as SqlCommand;
                db.AddInParameter(oCommand, "CustomerFieldId", SqlDbType.NVarChar, CustomerFieldId.Trim());
                db.AddInParameter(oCommand, "CustomerFieldName", SqlDbType.NVarChar, CustomerFieldName.Trim());
                db.AddInParameter(oCommand, "CustomerFieldDataType", SqlDbType.Int, CustomerFieldDataType);
                db.AddInParameter(oCommand, "TableName", SqlDbType.NVarChar, TableName.Trim());
                db.AddInParameter(oCommand, "UserId", SqlDbType.NVarChar, GeneralFunc.UserName);
                return oCommand;
            }
            #endregion

            #region Delete All Return Command
            public SqlCommand DeleteAllCommand()
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("KYCCompulsoryCustomerDeleteAll") as SqlCommand;
                return oCommand;
            }
            #endregion

            #region Delete Return Command
            public SqlCommand DeleteCommand()
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("KYCCompulsoryCustomerDelete") as SqlCommand;
                db.AddInParameter(oCommand, "CustomerFieldId", SqlDbType.NVarChar, CustomerFieldId.Trim());
                return oCommand;
            }
            #endregion

            #region Get All
            public DataSet GetAll()
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("KYCCompulsoryCustomerSelectAll") as SqlCommand;
                DataSet oDS = db.ExecuteDataSet(dbCommand);
                return oDS;
            }
            #endregion

            #region Get All KYC Compulsory Customer Return List
            public List<CustomerFieldData.KYCCompulsoryCustomer> GetAllReturnList()
            {
                List<CustomerFieldData.KYCCompulsoryCustomer> lstKYCCompulsoryCustomer = new List<CustomerFieldData.KYCCompulsoryCustomer>();
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("KYCCompulsoryCustomerSelectAll") as SqlCommand;
                DataSet oDS = db.ExecuteDataSet(dbCommand);
                foreach (DataRow oRow in oDS.Tables[0].Rows)
                {
                    CustomerFieldData.KYCCompulsoryCustomer oKYCCompulsoryCustomer = new CustomerFieldData.KYCCompulsoryCustomer();
                    oKYCCompulsoryCustomer.CustomerFieldId = oRow["CustomerFieldId"].ToString();
                    oKYCCompulsoryCustomer.CustomerFieldName = oRow["CustomerFieldName"].ToString();
                    lstKYCCompulsoryCustomer.Add(oKYCCompulsoryCustomer);
                }
                return lstKYCCompulsoryCustomer;
            }
            #endregion
        }
    }
}
