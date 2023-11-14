using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class ParameterTable
    {
        #region Declaration
        private int intMajor, intMajorRevision,intMinor, intMinorRevision;
        #endregion

        #region Properties
        public int Major
        {
            set { intMajor = value; }
            get { return intMajor; }
        }
        public int MajorRevision
        {
            set { intMajorRevision = value; }
            get { return intMajorRevision; }
        }
        public int Minor
        {
            set { intMinor = value; }
            get { return intMinor; }
        }
        public int MinorRevision
        {
            set { intMinorRevision = value; }
            get { return intMinorRevision; }
        }
        #endregion

        #region Get Parameter Table
        public bool GetParameterTable()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ParameterTableSelect") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                intMajor = thisRow[0]["Major"] == null || thisRow[0]["Major"].ToString().Trim() == ""
                          ? 0 : int.Parse(thisRow[0]["Major"].ToString());
                intMajorRevision = thisRow[0]["MajorRevision"] == null || thisRow[0]["MajorRevision"].ToString().Trim() == ""
                                     ? 0 : int.Parse(thisRow[0]["MajorRevision"].ToString());
                intMinor = thisRow[0]["Minor"] == null || thisRow[0]["Minor"].ToString().Trim() == ""
                          ? 0 : int.Parse(thisRow[0]["Minor"].ToString());
                intMinorRevision = thisRow[0]["MinorRevision"] == null || thisRow[0]["MinorRevision"].ToString().Trim() == ""
                              ? 0 : int.Parse(thisRow[0]["MinorRevision"].ToString());
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

    }
}
