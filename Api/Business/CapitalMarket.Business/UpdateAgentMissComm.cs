using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using BaseUtility.Business;

namespace CapitalMarket.Business
{
    public class UpdateAgentMissComm
    {
        #region Declaration
        public Int64 TransNo { set; get; }
        public DateTime DateFrom { set; get; }
        public DateTime DateTo { set; get; }
        public string CustomerId { set; get; }
        public string AgentId { set; get; }
        #endregion




        #region Add New Update Missing Comm Return Command
        public SqlCommand AddCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("UpdateAgentMissCommAdd") as SqlCommand;
            db.AddOutParameter(oCommand, "TransNo", SqlDbType.BigInt, 10);
            db.AddInParameter(oCommand, "DateFrom", SqlDbType.DateTime, DateFrom);
            db.AddInParameter(oCommand, "DateTo", SqlDbType.DateTime, DateTo);
            db.AddInParameter(oCommand, "CustomerId", SqlDbType.VarChar, CustomerId);
            db.AddInParameter(oCommand, "AgentId", SqlDbType.VarChar, AgentId);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
            return oCommand;
        }
        #endregion

        #region Get Check Customer And Date Exist
        public bool ChkCustDateExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("UpdateAgentMissCommChkCustDateExist") as SqlCommand;
            db.AddInParameter(dbCommand, "DateFrom", SqlDbType.DateTime, DateFrom);
            db.AddInParameter(dbCommand, "DateTo", SqlDbType.DateTime, DateTo);
            db.AddInParameter(dbCommand, "CustomerId", SqlDbType.VarChar, CustomerId);
            DataSet oDS =  db.ExecuteDataSet(dbCommand);
            if(oDS.Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion
    }
}
