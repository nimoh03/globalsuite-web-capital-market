using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;

namespace CapitalMarket.Business
{
    public class ChangeUnitCostMaster
    {
        #region Declaration
        private Int64 intTransNo;
        private DateTime datEffectiveDate;
        private string strAgentId;
        private string strSecurityCode;
        private float fltNewPrice;
        private bool blnBatchAgent;
        private string strSaveType;
        #endregion

        #region Properties

        public Int64 TransNo
        {
            set { intTransNo = value; }
            get { return intTransNo; }
        }
        public DateTime EffectiveDate
        {
            set { datEffectiveDate = value; }
            get { return datEffectiveDate; }
        }
        public string AgentId
        {
            set { strAgentId = value; }
            get { return strAgentId; }
        }
        public string SecurityCode
        {
            set { strSecurityCode = value; }
            get { return strSecurityCode; }
        }
        public float NewPrice
        {
            set { fltNewPrice = value; }
            get { return fltNewPrice; }
        }
        public bool BatchAgent
        {
            set { blnBatchAgent = value; }
            get { return blnBatchAgent; }
        }

        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        #endregion

        #region Save And Return Command
        public SqlCommand SaveCommand()
        {
            if (!ChkTransNoExist())
            {
                throw new Exception("Cannot Edit Transaction Does Not Exist");
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (strSaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("ChangeUnitCostMasterAdd") as SqlCommand;
                db.AddOutParameter(oCommand, "TransNo", SqlDbType.VarChar, 8);
            }
            else if (strSaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("ChangeUnitCostMasterEdit") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            }
            db.AddInParameter(oCommand, "EffectiveDate", SqlDbType.DateTime, datEffectiveDate);
            db.AddInParameter(oCommand, "AgentId", SqlDbType.VarChar, strAgentId.Trim());
            db.AddInParameter(oCommand, "SecurityCode", SqlDbType.VarChar, strSecurityCode.Trim());
            db.AddInParameter(oCommand, "NewPrice", SqlDbType.Float, fltNewPrice);
            db.AddInParameter(oCommand, "BatchAgent", SqlDbType.Bit, blnBatchAgent);
            db.AddInParameter(oCommand, "UserID", SqlDbType.Char, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Get 
        public bool GetChangeUnitCostMaster()
        {
            bool blnStatus = false;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ChangeUnitCostMasterSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                intTransNo = Int64.Parse(thisRow[0]["TransNo"].ToString());
                datEffectiveDate = DateTime.ParseExact(thisRow[0]["EffectiveDate"].ToString().Trim().Substring(0,10), "dd/MM/yyyy", format);
                strAgentId = thisRow[0]["AgentId"].ToString();
                strSecurityCode = thisRow[0]["SecurityCode"].ToString();
                fltNewPrice = float.Parse(thisRow[0]["NewPrice"].ToString());
                blnBatchAgent = bool.Parse(thisRow[0]["BatchAgent"].ToString());
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Check TransNo Exist
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("ChangeUnitCostMasterChkTransNoExist") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intTransNo);
                DataSet oDs = db.ExecuteDataSet(oCommand);
                if (oDs.Tables[0].Rows.Count > 0)
                {
                    blnStatus = true;
                }
            }
            else if (strSaveType == "ADDS")
            {
                blnStatus = true;
            }

            return blnStatus;
        }
        #endregion
    }
}
