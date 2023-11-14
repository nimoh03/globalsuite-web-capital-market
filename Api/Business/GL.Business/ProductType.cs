using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class ProductType
    {
        #region Declaration
        private int intTransNo;
        private string strProductTypeName, strProductClass;
        #endregion

        #region Properties
        public int TransNo
        {
            set { intTransNo = value; }
            get { return intTransNo; }
        }
        public string ProductTypeName
        {
            set { strProductTypeName = value; }
            get { return strProductTypeName; }
        }
        public string ProductClass
        {
            set { strProductClass = value; }
            get { return strProductClass; }
        }
        #endregion

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ProductTypeSelect") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion

        #region Get By Product Class
        public DataSet GetByProductClass()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ProductTypeSelectGivenProductClass") as SqlCommand;
            db.AddInParameter(oCommand, "ProductClass", SqlDbType.VarChar, strProductClass.Trim());
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion
    }
}

