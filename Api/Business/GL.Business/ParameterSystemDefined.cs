using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class ParameterSystemDefined
    {
        #region Properties
        public Int32 ParameterValue { get; set; }
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
                oCommand = db.GetStoredProcCommand("ParameterSystemDefinedAdd") as SqlCommand;
            }
            else if (SaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("ParameterSystemDefinedEdit") as SqlCommand;
            }
            db.AddOutParameter(oCommand, "SaveErrMsg", SqlDbType.NVarChar, 300);
            db.AddInParameter(oCommand, "ParameterValue", SqlDbType.VarChar, ParameterValue);
            db.AddInParameter(oCommand, "ParameterType", SqlDbType.VarChar, ParameterType.Trim());
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
            SqlCommand oCommand = db.GetStoredProcCommand("ParameterSystemDefinedSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion

        #region Get
        public bool GetParameterSystemDefined(string strTableName, string strColumnName, string strTableType)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GenSelectByType") as SqlCommand;
            db.AddInParameter(oCommand, "RecordValue", SqlDbType.BigInt, ParameterValue);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            db.AddInParameter(oCommand, "TableType", SqlDbType.VarChar, strTableType.Trim());
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
        public List<ParameterSystemDefined> ConvertDataSetToList(DataSet oDataSet)
        {
            List<ParameterSystemDefined> lstParameterSystemDefined = oDataSet.Tables[0].AsEnumerable().Select(
                            oRow => new ParameterSystemDefined
                            {
                                ParameterValue = Convert.ToInt32(oRow["ParameterValue"]),
                                ParameterType = oRow["ParameterType"].ToString(),
                                ParameterName = oRow["ParameterName"].ToString(),
                            }).ToList();
            return lstParameterSystemDefined;
        }
        #endregion
    }
}

