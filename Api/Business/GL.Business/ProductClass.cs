using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class ProductClass
    {
        #region Declaration
        private string strProductClassCode, strProductClassName, strModule;
        #endregion

        #region Properties
        public string ProductClassCode
        {
            set { strProductClassCode = value; }
            get { return strProductClassCode; }
        }
        public string ProductClassName
        {
            set { strProductClassName = value; }
            get { return strProductClassName; }
        }
        public string Module
        {
            set { strModule = value; }
            get { return strModule; }
        }
        
        #endregion

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ProductClassSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion

        #region Get By Module
        public DataSet GetByModule()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ProductClassSelectGivenModule") as SqlCommand;
            db.AddInParameter(oCommand, "Module", SqlDbType.VarChar, strModule.Trim());
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion
    }
}

