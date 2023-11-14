using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace Admin.Business
{
    public class Audit
    {
        #region Declaration
        private string strProgram;
        private string strMenuItem;
        private string strMenuItemName;

        #endregion

        #region Properties
        public string Program
        {
            set { strProgram = value; }
            get { return strProgram; }
        }
        public string MenuItem
        {
            set { strMenuItem = value; }
            get { return strMenuItem; }
        }
        public string MenuItemName
        {
            set { strMenuItemName = value; }
            get { return strMenuItemName; }
        }
        
        #endregion

        #region Get All Tables Without Triggers
        public DataSet GetAllTablesWithoutTriggers()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("SystemSelectAllTableWithoutTrigger") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion
    }
}
