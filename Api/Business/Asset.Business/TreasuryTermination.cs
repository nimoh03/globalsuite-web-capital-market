using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using BaseUtility.Business;

namespace Asset.Business
{
    public class TreasuryTermination
    {
        #region Properties
        public Int64 TransNo { get; set; }
        public Int64 TreasuryDepositNo { get; set; }
        public DateTime TerminationDate { get; set; }
        public string BankAccountNo { get; set; }
        public decimal WithdrawalAmount { get; set; }
        public float PenaltyRate { get; set; }
        public decimal TotalCharges { get; set; }
        public decimal TerminationValue { get; set; }
        public string Description { get; set; }
        public string Amountword { get; set; }
        public DataGeneral.GLInstrumentType PayingInstrument { get; set; }
        public string ChequeNo { get; set; }
        public string Branch { get; set; }
        public string SaveType { get; set; }
        #endregion

        #region Save
        public DataGeneral.SaveStatus Save()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            if (!ChkTransNoExist(DataGeneral.PostStatus.UnPosted))
            {
                enSaveStatus = DataGeneral.SaveStatus.NotExist;
                return enSaveStatus;
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    SqlCommand oCommand = null;
                    if (SaveType == "ADDS")
                    {
                        oCommand = db.GetStoredProcCommand("TreasuryTerminationAddNew") as SqlCommand;
                    }
                    else if (SaveType == "EDIT")
                    {
                        oCommand = db.GetStoredProcCommand("TreasuryTerminationEdit") as SqlCommand;
                    }
                    db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, TransNo);
                    db.AddInParameter(oCommand, "TreasuryDepositNo", SqlDbType.BigInt, TreasuryDepositNo);
                    db.AddInParameter(oCommand, "TerminationDate", SqlDbType.Date, TerminationDate);
                    db.AddInParameter(oCommand, "WithdrawalAmount", SqlDbType.Decimal, WithdrawalAmount);
                    db.AddInParameter(oCommand, "BankAccountNo", SqlDbType.NVarChar,BankAccountNo.Trim());
                    db.AddInParameter(oCommand, "PenaltyRate", SqlDbType.Real,PenaltyRate);
                    db.AddInParameter(oCommand, "TotalCharges", SqlDbType.Decimal, TotalCharges);
                    db.AddInParameter(oCommand, "Description", SqlDbType.NVarChar, Description.Trim());
                    db.AddInParameter(oCommand, "Amountword", SqlDbType.NVarChar, Amountword.Trim());
                    db.AddInParameter(oCommand, "Posted", SqlDbType.Bit, false);
                    db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, false);
                    db.AddInParameter(oCommand, "PayingInstrument", SqlDbType.Char, PayingInstrument);
                    db.AddInParameter(oCommand, "ChequeNo", SqlDbType.NVarChar, ChequeNo.Trim());
                    db.AddInParameter(oCommand, "Branch", SqlDbType.Char, Branch.Trim());
                    db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
                    db.ExecuteNonQuery(oCommand, transaction);
                    transaction.Commit();
                    enSaveStatus = DataGeneral.SaveStatus.Saved;
                    return enSaveStatus;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }

            }

        }
        #endregion

        #region Check TransNo Exist
        public bool ChkTransNoExist(DataGeneral.PostStatus ePostStatus)
        {
            bool blnStatus = false;
            if (SaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = null;
                if (ePostStatus == DataGeneral.PostStatus.UnPosted)
                {
                    oCommand = db.GetStoredProcCommand("TreasuryTerminationChkTransNoExistUnPosted") as SqlCommand;
                }
                db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, TransNo);
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
        public bool GetTreasuryTermination(DataGeneral.PostStatus TranStatus)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TranStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("TreasuryTerminationSelectUnPost") as SqlCommand;
            }
            else if (TranStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("TreasuryTerminationSelectPosted") as SqlCommand;
            }
            else if (TranStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("TreasuryTerminationSelectReversed") as SqlCommand;
            }
            else if (TranStatus == DataGeneral.PostStatus.All)
            {
                dbCommand = db.GetStoredProcCommand("TreasuryTerminationSelect") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, TransNo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                TransNo = Convert.ToInt64(thisRow[0]["TransNo"]);
                TreasuryDepositNo = Convert.ToInt64(thisRow[0]["TreasuryDepositNo"]);
                TerminationDate = Convert.ToDateTime(thisRow[0]["TerminationDate"]);
                WithdrawalAmount = Convert.ToDecimal(thisRow[0]["WithdrawalAmount"]);
                BankAccountNo = thisRow[0]["BankAccountNo"].ToString().Trim();
                PenaltyRate = float.Parse(thisRow[0]["PenaltyRate"].ToString());
                TotalCharges = Convert.ToDecimal(thisRow[0]["TotalCharges"]);
                Description = thisRow[0]["Description"].ToString().Trim();
                Amountword = thisRow[0]["Amountword"].ToString().Trim();
                PayingInstrument = (DataGeneral.GLInstrumentType)Enum.Parse(typeof(DataGeneral.GLInstrumentType), thisRow[0]["PayingInstrument"].ToString().Trim(), false);
                ChequeNo = thisRow[0]["ChequeNo"].ToString().Trim();
                Branch = thisRow[0]["Branch"].ToString().Trim();
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion    

        #region Get All
        public DataSet GetAll(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("TreasuryTerminationSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("TreasuryTerminationSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("TreasuryTerminationSelectAllReversed") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("TreasuryTerminationSelectAllUnPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("TreasuryTerminationSelectAllPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.All)
            {
                dbCommand = db.GetStoredProcCommand("TreasuryTerminationSelectAll") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region PenaltyAmount
        public decimal GetPenaltyAmount(decimal decAccruedInterest)
        {
            return decAccruedInterest * (decimal.Parse(PenaltyRate.ToString())/100);
        }
        #endregion    
    }
}
