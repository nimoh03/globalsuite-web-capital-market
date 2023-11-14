using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;

namespace CapitalMarket.Business
{
    public class ReconcileACWithPortValue
    {
        #region Declaration
        private Int64 intTransNo;
        private DateTime datRevaluationDate;
        private string strInvestmentType;
        #endregion

        #region Properties

        public Int64 TransNo
        {
            set { intTransNo = value; }
            get { return intTransNo; }
        }

        public DateTime RevaluationDate
        {
            set { datRevaluationDate = value; }
            get { return datRevaluationDate; }
        }

        
        public string InvestmentType
        {
            set { strInvestmentType = value; }
            get { return strInvestmentType; }
        }

        
        #endregion

        #region Save And Return Command
        public SqlCommand SaveCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ReconcileACWithPortValueAdd") as SqlCommand;
                db.AddOutParameter(oCommand, "TransNo", SqlDbType.BigInt, 8);
                db.AddInParameter(oCommand, "RevaluationDate", SqlDbType.DateTime, datRevaluationDate);
                db.AddInParameter(oCommand, "InvestmentType", SqlDbType.VarChar, strInvestmentType.Trim());
                db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            
            return oCommand;
        }
        #endregion
    }
}
