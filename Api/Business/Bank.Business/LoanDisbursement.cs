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

namespace Bank.Business
{
    public class LoanDisbursement
    {
        #region Properties
        public Int64 TransNo { get; set; }
        public string ProductCode { get; set; }
        public string CustomerNo { get; set; }
        public string BankAccountNo { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime MaturityDate { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestAmount { get; set; }
        public Int32 Tenure { get; set; }
        public float InterestRate { get; set; }
        public string Description { get; set; }
        public string Amountword { get; set; }
        public string ReceiptNo { get; set; }
        public DataGeneral.InterestPeriod InterestPeriod { get; set; }
        public DataGeneral.InterestType InterestType { get; set; }
        public DateTime BankValueDate { get; set; }
        public DataGeneral.GLInstrumentType PayingInstrument { get; set; }
        public string ChequeNo { get; set; }
        public bool DoNotChargeBankStampDuty { get; set; }
        public string RolloverType { get; set; }
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
                        string strRnumberNext;
                        SqlCommand oCommandRnumber = db.GetStoredProcCommand("ReceiptAdd") as SqlCommand;
                        db.AddOutParameter(oCommandRnumber, "Rnumber", SqlDbType.BigInt, 8);
                        db.AddInParameter(oCommandRnumber, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                        db.ExecuteNonQuery(oCommandRnumber, transaction);
                        strRnumberNext = db.GetParameterValue(oCommandRnumber, "Rnumber").ToString();

                        oCommand = db.GetStoredProcCommand("LoanDisbursementAddNew") as SqlCommand;
                        db.AddInParameter(oCommand, "ReceiptNo", SqlDbType.VarChar, strRnumberNext.Trim());
                    }
                    else if (SaveType == "EDIT")
                    {
                        oCommand = db.GetStoredProcCommand("LoanDisbursementEdit") as SqlCommand;
                    }
                    db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt,TransNo);
                    db.AddInParameter(oCommand, "ProductCode", SqlDbType.Char, ProductCode.Trim());
                    db.AddInParameter(oCommand, "CustomerNo", SqlDbType.NVarChar, CustomerNo.Trim());
                    db.AddInParameter(oCommand, "BankAccountNo", SqlDbType.NVarChar, BankAccountNo.Trim());
                    db.AddInParameter(oCommand, "EffectiveDate", SqlDbType.Date, EffectiveDate);
                    db.AddInParameter(oCommand, "MaturityDate", SqlDbType.Date, MaturityDate);
                    db.AddInParameter(oCommand, "PrincipalAmount", SqlDbType.Decimal, PrincipalAmount);
                    db.AddInParameter(oCommand, "InterestAmount", SqlDbType.Decimal, InterestAmount);
                    db.AddInParameter(oCommand, "Tenure", SqlDbType.Int, Tenure);
                    db.AddInParameter(oCommand, "InterestRate", SqlDbType.Real, InterestRate);
                    db.AddInParameter(oCommand, "Description", SqlDbType.NVarChar, Description.Trim());
                    db.AddInParameter(oCommand, "Amountword", SqlDbType.NVarChar, Amountword.Trim());
                    db.AddInParameter(oCommand, "Posted", SqlDbType.Bit, false);
                    db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, false);
                    db.AddInParameter(oCommand, "InterestPeriod", SqlDbType.Char, InterestPeriod);
                    db.AddInParameter(oCommand, "InterestType", SqlDbType.Char, InterestType);
                    db.AddInParameter(oCommand, "BankValueDate", SqlDbType.Date, BankValueDate);
                    db.AddInParameter(oCommand, "PayingInstrument", SqlDbType.Char, PayingInstrument);
                    db.AddInParameter(oCommand, "ChequeNo", SqlDbType.NVarChar, ChequeNo.Trim());
                    db.AddInParameter(oCommand, "DoNotChargeBankStampDuty", SqlDbType.Bit, DoNotChargeBankStampDuty);
                    db.AddInParameter(oCommand, "RolloverType", SqlDbType.NVarChar, RolloverType.Trim());
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
                    oCommand = db.GetStoredProcCommand("LoanDisbursementChkTransNoExistUnPosted") as SqlCommand;
                }
                db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt,TransNo);
                DataSet oDs = db.ExecuteDataSet(oCommand);
                if (oDs.Tables[0].Rows.Count > 0)
                {
                    blnStatus = true;
                }
                else
                {
                    blnStatus = false;
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
        public bool GetLoanDisbursement(DataGeneral.PostStatus TranStatus)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TranStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("LoanDisbursementSelectUnPost") as SqlCommand;
            }
            else if (TranStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("LoanDisbursementSelectPosted") as SqlCommand;
            }
            else if (TranStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("LoanDisbursementSelectReversed") as SqlCommand;
            }
            else if (TranStatus == DataGeneral.PostStatus.All)
            {
                dbCommand = db.GetStoredProcCommand("LoanDisbursementSelect") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, TransNo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                ProductCode = thisRow[0]["ProductCode"].ToString().Trim();
                CustomerNo = thisRow[0]["CustomerNo"].ToString().Trim();
                BankAccountNo = thisRow[0]["BankAccountNo"].ToString().Trim();
                EffectiveDate = Convert.ToDateTime(thisRow[0]["EffectiveDate"]);
                MaturityDate = Convert.ToDateTime(thisRow[0]["MaturityDate"]);
                PrincipalAmount = Convert.ToDecimal(thisRow[0]["PrincipalAmount"]);
                InterestAmount = Convert.ToDecimal(thisRow[0]["InterestAmount"]);
                Tenure = Convert.ToInt32(thisRow[0]["Tenure"]);
                InterestRate = float.Parse(thisRow[0]["InterestRate"].ToString());
                Description = thisRow[0]["Description"].ToString().Trim();
                Amountword = thisRow[0]["Amountword"].ToString().Trim();
                ReceiptNo = thisRow[0]["ReceiptNo"].ToString().Trim();
                InterestPeriod = (DataGeneral.InterestPeriod)Enum.Parse(typeof(DataGeneral.InterestPeriod), thisRow[0]["InterestPeriod"].ToString().Trim(), false);
                InterestType = (DataGeneral.InterestType)Enum.Parse(typeof(DataGeneral.InterestType), thisRow[0]["InterestType"].ToString().Trim(), false);
                BankValueDate = Convert.ToDateTime(thisRow[0]["BankValueDate"]);
                PayingInstrument = (DataGeneral.GLInstrumentType)Enum.Parse(typeof(DataGeneral.GLInstrumentType), thisRow[0]["PayingInstrument"].ToString().Trim(), false);
                ChequeNo = thisRow[0]["ChequeNo"].ToString().Trim();
                DoNotChargeBankStampDuty = Convert.ToBoolean(thisRow[0]["DoNotChargeBankStampDuty"]);
                RolloverType = thisRow[0]["RolloverType"].ToString().Trim();
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
                dbCommand = db.GetStoredProcCommand("LoanDisbursementSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("LoanDisbursementSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("LoanDisbursementSelectAllReversed") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("LoanDisbursementSelectAllUnPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("LoanDisbursementSelectAllPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.All)
            {
                dbCommand = db.GetStoredProcCommand("LoanDisbursementSelectAll") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Compute Interest Amount
        public decimal ComputeInterestAmount()
        {
            decimal decInterestAmount = 0;
            if (InterestPeriod == DataGeneral.InterestPeriod.Y)
            {
                decInterestAmount = (PrincipalAmount * decimal.Parse(InterestRate.ToString()) * decimal.Parse(Tenure.ToString())) / (365 * 100);
            }
            else
            {
                decInterestAmount = (PrincipalAmount * decimal.Parse(InterestRate.ToString()) * decimal.Parse(Tenure.ToString())) / (30 * 100);
            }
            decInterestAmount = Math.Round(decInterestAmount, 2);
            return decInterestAmount;
        }
        #endregion

        #region CalculateMaturityDate
        public DateTime CalculateMaturityDate()
        {
            DateTime datMaturityDate = EffectiveDate.AddDays(Tenure);
            return datMaturityDate;
        }
        #endregion

        #region Get Customer Treasury Deposit
        public DataSet GetCustomerLoanDisbursement(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("LoanDisbursementSelectCustomerProductPosted") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.Char, ProductCode.Trim());
            db.AddInParameter(dbCommand, "CustomerNo", SqlDbType.NVarChar, CustomerNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Customer Treasury Deposit Valid
        public DataSet GetCustomerLoanDisbursementValid(DateTime datEffDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("LoanDisbursementSelectCustomerProductValid") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.Char, ProductCode.Trim());
            db.AddInParameter(dbCommand, "CustomerNo", SqlDbType.NVarChar, CustomerNo.Trim());
            db.AddInParameter(dbCommand, "EffectiveDate", SqlDbType.Date, datEffDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Customer Treasury Deposit Running For Start Date
        public DataSet GetCustomerLoanDisbursementRunningForStartDate(DateTime datDateFrom)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("LoanDisbursementSelectRunningForStartDate") as SqlCommand;
            db.AddInParameter(dbCommand, "DateFrom", SqlDbType.Date, datDateFrom);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion
    }
}
