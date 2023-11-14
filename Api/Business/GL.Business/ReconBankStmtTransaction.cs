using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class ReconBankStmtTransaction
    {
        #region Declaration
        private Int64 intTransNo;
        private Int64 intReconBank;
        private DateTime datDateFrom, datDateTo;
        private int intWordToMatch;
        private bool blnCompareTranDate, blnCompareValueDate, blnComparePartDescription;
        private string strSaveType;
        #endregion

        #region Properties
        public Int64 TransNo
        {
            set { intTransNo = value; }
            get { return intTransNo; }
        }
        public Int64 ReconBank
        {
            set { intReconBank = value; }
            get { return intReconBank; }
        }

        public DateTime DateFrom
        {
            set { datDateFrom = value; }
            get { return datDateFrom; }
        }
        public DateTime DateTo
        {
            set { datDateTo = value; }
            get { return datDateTo; }
        }

        public int WordToMatch
        {
            set { intWordToMatch = value; }
            get { return intWordToMatch; }
        }

        public bool CompareTranDate
        {
            set { blnCompareTranDate = value; }
            get { return blnCompareTranDate; }
        }

        public bool CompareValueDate
        {
            set { blnCompareValueDate = value; }
            get { return blnCompareValueDate; }
        }
        public bool ComparePartDescription
        {
            set { blnComparePartDescription = value; }
            get { return blnComparePartDescription; }
        }
        
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }

        #endregion

        #region Save Return Command
        public SqlCommand SaveCommand()
        {
            SqlCommand oCommand = null;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            oCommand = db.GetStoredProcCommand("ReconBankStmtTransactionAdd") as SqlCommand;
            db.AddOutParameter(oCommand, "TransNo", SqlDbType.BigInt, 8);
            db.AddInParameter(oCommand, "ReconBank", SqlDbType.BigInt, intReconBank);
            db.AddInParameter(oCommand, "DateFrom", SqlDbType.DateTime, datDateFrom);
            db.AddInParameter(oCommand, "DateTo", SqlDbType.DateTime, datDateTo);
            db.AddInParameter(oCommand, "WordToMatch", SqlDbType.Int, intWordToMatch);
            db.AddInParameter(oCommand, "CompareTranDate", SqlDbType.Bit, blnCompareTranDate);
            db.AddInParameter(oCommand, "CompareValueDate", SqlDbType.Bit, blnCompareValueDate);
            db.AddInParameter(oCommand, "ComparePartDescription", SqlDbType.Bit, blnComparePartDescription);
            return oCommand;
        }
        #endregion


        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("ReconBankStmtTransactionSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get ReconBankStmtTransaction
        public bool GetReconBankStmtTransaction()
        {
            bool blnStatus = false;
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ReconBankStmtTransactionSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                intTransNo = long.Parse(thisRow[0]["TransNo"].ToString());
                intReconBank = long.Parse(thisRow[0]["ReconBankNumber"].ToString());
                datDateFrom = DateTime.ParseExact(thisRow[0]["DateFrom"].ToString().Trim().Substring(0,10),"dd/MM/yyyy",format);
                datDateTo = DateTime.ParseExact(thisRow[0]["DateTo"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                intWordToMatch = int.Parse(thisRow[0]["NumberOfWordInDescription"].ToString());
                blnCompareTranDate = bool.Parse(thisRow[0]["CompareTransDate"].ToString());
                blnCompareValueDate = bool.Parse(thisRow[0]["CompareValueDate"].ToString());
                blnComparePartDescription = bool.Parse(thisRow[0]["CompareDescription"].ToString());
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Get Bank Account Code
        public string GetBankAcctCode()
        {
            ReconBank oReconBank = new ReconBank();
            GetReconBankStmtTransaction();
            oReconBank.TransNo = intReconBank.ToString();
            oReconBank.GetByTransNo();
            return oReconBank.CashBookAcct;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ReconBankStmtTransactionDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion
    }
}
