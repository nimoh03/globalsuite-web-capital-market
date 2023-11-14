using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace Admin.Business
{
  public  class SchoolMaster
    {
        #region Properties
        public Int32 SchoolId { get; set; }
        public string SchoolName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string TelephoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Website { get; set; }
        public string SaveType { get; set; }
        #endregion

        #region Save Return Command
        public SqlCommand SaveCommand()
        {
            
            if (!ChkSchoolIdExist())
            {
                throw new Exception("School Does Not Exist");
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (SaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("SchoolAdd") as SqlCommand;
            }
            else if (SaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("SchoolEdit") as SqlCommand;

            }
            db.AddInParameter(oCommand, "SchoolId", SqlDbType.Int, SchoolId);
            db.AddInParameter(oCommand, "SchoolName", SqlDbType.NVarChar, SchoolName);
            db.AddInParameter(oCommand, "Address1", SqlDbType.NVarChar, Address1);
            db.AddInParameter(oCommand, "Address2", SqlDbType.NVarChar, Address2);
            db.AddInParameter(oCommand, "TelephoneNumber", SqlDbType.NVarChar, TelephoneNumber);
            db.AddInParameter(oCommand, "EmailAddress", SqlDbType.NVarChar, EmailAddress);
            db.AddInParameter(oCommand, "Website", SqlDbType.NVarChar, Website);
            db.AddInParameter(oCommand, "UserId", SqlDbType.NVarChar, GeneralFunc.UserName);
            return oCommand;
        }
        #endregion

        #region Check School Id Exist
        public bool ChkSchoolIdExist()
        {
            bool blnStatus = false;
            if (SaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = null;
                oCommand = db.GetStoredProcCommand("SchoolChkTransNoExist") as SqlCommand;
                db.AddInParameter(oCommand, "Transno", SqlDbType.Int, SchoolId);
                DataSet oDs = db.ExecuteDataSet(oCommand);
                if (oDs.Tables[0].Rows.Count > 0)
                {
                    blnStatus = true;
                }
            }
            else if (SaveType == "ADDS")
            {
                blnStatus = true;
            }

            return blnStatus;
        }
        #endregion

       


        #region Get
        public bool GetSchoolMaster(string strTableName, string strColumnName)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GenSelect") as SqlCommand;
            db.AddInParameter(oCommand, "Recordvalue", SqlDbType.Int, SchoolId);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                SchoolId = Convert.ToInt32(thisRow[0]["SchoolId"]);
                SchoolName = Convert.ToString(thisRow[0]["SchoolName"]);
                Address1 = Convert.ToString(thisRow[0]["Address1"]);
                Address2 = Convert.ToString(thisRow[0]["Address2"]);
                TelephoneNumber = Convert.ToString(thisRow[0]["TelephoneNumber"]);
                EmailAddress = Convert.ToString(thisRow[0]["EmailAddress"]);
                Website = Convert.ToString(thisRow[0]["Website"]);
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
            db.AddInParameter(oCommand, "RecordValue", SqlDbType.BigInt, SchoolId);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            db.ExecuteNonQuery(oCommand);

            return true;
        }
        #endregion
        #region Convert To List
        public List<SchoolMaster> ConvertToList(DataRowCollection oDataRowCollection)
        {
            return (from DataRow oRow in oDataRowCollection
                    select new SchoolMaster()
                    {
                        SchoolId = Convert.ToInt32(oRow["SchoolId"]),
                        SchoolName = Convert.ToString(oRow["SchoolName"]),
                       
                    }).ToList();
        }
        #endregion

        #region Convert DataSet To List
        public List<SchoolMaster> ConvertDataSetToList(DataSet oDataSet)
        {
            List<SchoolMaster> lstSchoolMaster = oDataSet.Tables[0].AsEnumerable().Select(
                            oRow => new SchoolMaster
                            {
                                SchoolId = Convert.ToInt32(oRow["SchoolId"]),
                                SchoolName = Convert.ToString(oRow["SchoolName"]),
                            }).ToList();
            return lstSchoolMaster;
        }
        #endregion
    }
}
