using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class ParameterUserDefined
    {
        #region Properties
        public Int64 ParameterValue { get; set; }
        public string ParameterType { get; set; }
        public string ParameterName { get; set; }
        public string SaveType { get; set; }
        #endregion

        #region Save
        public List<string> Save()
        {
            List<string> oErrMsg = new List<string>();

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (SaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("ParameterUserDefinedAdd") as SqlCommand;
            }
            else if (SaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("ParameterUserDefinedEdit") as SqlCommand;

            }
            db.AddOutParameter(oCommand, "SaveErrMsg", SqlDbType.NVarChar, 300);
            db.AddInParameter(oCommand, "ParameterValue", SqlDbType.BigInt, ParameterValue);
            db.AddInParameter(oCommand, "ParameterType", SqlDbType.VarChar, ParameterType.Trim().ToUpper());
            db.AddInParameter(oCommand, "ParameterName", SqlDbType.NVarChar, ParameterName.Trim());
            db.ExecuteNonQuery(oCommand);
            var varErrMsg = db.GetParameterValue(oCommand, "SaveErrMsg").ToString();
            if (varErrMsg.Trim() != "")
            {
                oErrMsg.Add(varErrMsg);
            }
            return oErrMsg;

        }
        #endregion

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ParameterUserDefinedSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion

        #region Get
        public bool GetParameterUserDefined(string strTableName, string strColumnName)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GenSelect") as SqlCommand;
            db.AddInParameter(oCommand, "RecordValue", SqlDbType.BigInt, ParameterValue);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                ParameterType = thisRow[0]["ParameterType"].ToString();
                ParameterName = thisRow[0]["ParameterName"].ToString();
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Delete
        public bool Delete(string strTableName, string strColumnName)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GenDelete") as SqlCommand;
            db.AddInParameter(oCommand, "RecordValue", SqlDbType.BigInt, ParameterValue);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            db.ExecuteNonQuery(oCommand);

            return true;
        }
        #endregion
        
        #region Convert DataSet To List
        public List<ParameterUserDefined> ConvertDataSetToList(DataSet oDataSet)
        {
            List<ParameterUserDefined> lstParameterUserDefined = oDataSet.Tables[0].AsEnumerable().Select(
                            oRow => new ParameterUserDefined
                            {
                                ParameterValue = Convert.ToInt64(oRow["ParameterValue"]),
                                ParameterType = oRow["ParameterType"].ToString(),
                                ParameterName = oRow["ParameterName"].ToString(),
                            }).ToList();
            return lstParameterUserDefined;
        }
        #endregion
    }
}

