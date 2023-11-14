using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace CapitalMarket.Business
{
    public class Market
    {
        #region Properties
        public Int32 MarketId { get; set; }
        public string MarketName { get; set; }
        public string CustomerProduct { get; set; }
        public string PropProduct { get; set; }
        public string SaveType { get; set; }
        #endregion

        #region Save 
        public DataGeneral.SaveStatus Save()
        {
            List<Market> lstMarket = new List<Market>();
            lstMarket = ConvertDataSetToList(GeneralFunc.GetAll("StkMarket"));

            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            if ((SaveType == "EDIT") && !(lstMarket.Exists(e => e.MarketId == MarketId)))
            {
                enSaveStatus = DataGeneral.SaveStatus.NotExist;
                return enSaveStatus;
            }
            if ((SaveType == "ADDS" && lstMarket.Exists(e => e.MarketName == MarketName))
                || (SaveType == "EDIT" && lstMarket.Exists(e => e.MarketName == MarketName && e.MarketId != MarketId)))
            {
                enSaveStatus = DataGeneral.SaveStatus.NameExistAdd;
                return enSaveStatus;
            }

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (SaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("MarketAdd") as SqlCommand;
            }
            else if (SaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("MarketEdit") as SqlCommand;
            }
            db.AddInParameter(oCommand, "MarketId", SqlDbType.BigInt, MarketId);
            db.AddInParameter(oCommand, "MarketName", SqlDbType.NVarChar, MarketName.Trim().ToUpper());
            db.AddInParameter(oCommand, "CustomerProduct", SqlDbType.VarChar, CustomerProduct.Trim());
            db.AddInParameter(oCommand, "PropProduct", SqlDbType.VarChar, PropProduct.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.NVarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Get
        public bool GetMarket(string strTableName, string strColumnName)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GenSelect") as SqlCommand;
            db.AddInParameter(oCommand, "Recordvalue", SqlDbType.BigInt, MarketId);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                MarketId = Convert.ToInt32(thisRow[0]["MarketId"]);
                MarketName = thisRow[0]["MarketName"].ToString();
                CustomerProduct = thisRow[0]["CustomerProduct"].ToString();
                PropProduct = thisRow[0]["PropProduct"].ToString();
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
            db.AddInParameter(oCommand, "RecordValue", SqlDbType.BigInt, MarketId);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            db.ExecuteNonQuery(oCommand);

            return true;
        }
        #endregion

        #region Convert DataSet To List
        public List<Market> ConvertDataSetToList(DataSet oDataSet)
        {
            List<Market> lstMarket = oDataSet.Tables[0].AsEnumerable().Select(
                            oRow => new Market
                            {
                                MarketId = Convert.ToInt32(oRow["MarketId"]),
                                MarketName = oRow["MarketName"].ToString(),
                                CustomerProduct = oRow["CustomerProduct"].ToString(),
                                PropProduct = oRow["PropProduct"].ToString(),
                            }).ToList();
            return lstMarket;
        }
        #endregion
    }
}

