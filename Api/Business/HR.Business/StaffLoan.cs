using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using BaseUtility.Business;

namespace HR.Business
{
    public class StaffLoan
    {
        #region Declarations
        private string strTransNo, strEmpNo;
        private string strReason;
        private decimal decTenure;
        private decimal  decAmount,decTotalAmount, decMonthlyDeductionAmount;
        private float  fltInterest;
        private DateTime datLoanDate;
        private string strBankAccount;
        private bool blnDoNotPostToGL;
        private string strSaveType;
        private DatabaseProviderFactory factory = new DatabaseProviderFactory();
        private SqlDatabase db;
        SqlTransaction transaction = null;

        #endregion

        public StaffLoan()
        {
            db = factory.Create("GlobalSuitedb") as SqlDatabase;
        }

        #region Properties
        public DateTime LoanDate
        {
            set { datLoanDate = value; }
            get { return datLoanDate; }
        }
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        
        public string EmpNo
        {
            set { strEmpNo = value; }
            get { return strEmpNo; }
        }
        public decimal  Tenure
        {
            set { decTenure = value; }
            get { return decTenure; }
        }
        public decimal Amount
        {
            set { decAmount = value; }
            get { return decAmount; }
        }
        public decimal TotalAmount
        {
            set { decTotalAmount = value; }
            get { return decTotalAmount; }
        }
        public decimal MonthlyDeductionAmount
        {
            set { decMonthlyDeductionAmount = value; }
            get { return decMonthlyDeductionAmount; }
        }
        public float Interest
        {
            set { fltInterest = value; }
            get { return fltInterest; }
        }
        public string Reason
        {
            set { strReason = value; }
            get { return strReason; }
        }
        public string BankAccount
        {
            set { strBankAccount = value; }
            get { return strBankAccount; }
        }
        public bool DoNotPostToGL
        {
            set { blnDoNotPostToGL = value; }
            get { return blnDoNotPostToGL; }
        }

        public string ProductId { get; set; }
        public string CustomerId { get; set; }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        #endregion

        #region Save
        public DataGeneral.SaveStatus Save()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            if (!ChkTransNoExist())
            {
                enSaveStatus = DataGeneral.SaveStatus.NotExist;
                return enSaveStatus;
            }

            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                transaction = connection.BeginTransaction();
                try
                {
                    SqlCommand dbCommand = null;
                    if (strSaveType.Trim() == "ADDS")
                    {
                        dbCommand = db.GetStoredProcCommand("StaffLoanAdd") as SqlCommand;
                    }
                    else if (strSaveType.Trim() == "EDIT")
                    {
                        dbCommand = db.GetStoredProcCommand("StaffLoanEdit") as SqlCommand;
                    }
                    db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
                    db.AddInParameter(dbCommand, "Employee", SqlDbType.VarChar, strEmpNo.Trim());
                    db.AddInParameter(dbCommand, "Tenure", SqlDbType.Decimal, decTenure);
                    db.AddInParameter(dbCommand, "Amount", SqlDbType.Money, decAmount);
                    db.AddInParameter(dbCommand, "TotalAmount", SqlDbType.Money, decTotalAmount);
                    db.AddInParameter(dbCommand, "MonthlyDeductionAmount", SqlDbType.Money, decMonthlyDeductionAmount);
                    db.AddInParameter(dbCommand, "Interest", SqlDbType.Float, fltInterest);
                    db.AddInParameter(dbCommand, "LoanDate", SqlDbType.DateTime, datLoanDate);
                    db.AddInParameter(dbCommand, "Reason", SqlDbType.VarChar, strReason.Trim());
                    db.AddInParameter(dbCommand, "BankAccount", SqlDbType.VarChar, strBankAccount.Trim());
                    db.AddInParameter(dbCommand, "DoNotPostToGL", SqlDbType.Bit, blnDoNotPostToGL);
                    db.AddInParameter(dbCommand, "ProductId", SqlDbType.VarChar, ProductId);
                    db.AddInParameter(dbCommand, "CustomerId", SqlDbType.VarChar, CustomerId);
                    db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
                    db.ExecuteNonQuery(dbCommand, transaction);
                    
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Check TransNo Exist
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = null;
                oCommand = db.GetStoredProcCommand("StaffLoanChkTransNoExist") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
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
            else if (strSaveType == "ADDS")
            {
                blnStatus = true;
            }

            return blnStatus;
        }
        #endregion

        #region Get Staff Loan
        public bool GetStaffLoan(DataGeneral.PostStatus TransStatus)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("StaffLoanSelectUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("StaffLoanSelectPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("StaffLoanSelectReversed") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                strEmpNo = thisRow[0]["Employee"].ToString();
                decTenure = decimal.Parse(thisRow[0]["Tenure"].ToString());
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString());
                decTotalAmount = decimal.Parse(thisRow[0]["TotalAmount"].ToString());
                decMonthlyDeductionAmount = decimal.Parse(thisRow[0]["MonthlyDeductionAmount"].ToString());
                fltInterest = float.Parse(thisRow[0]["Interest"].ToString());
                datLoanDate = DateTime.ParseExact(thisRow[0]["LoanDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                strReason = thisRow[0]["Reason"].ToString();
                strBankAccount = thisRow[0]["BankAccount"].ToString();
                blnDoNotPostToGL = bool.Parse(thisRow[0]["DoNotPostToGL"].ToString());
                ProductId = thisRow[0]["ProductId"] != null ? thisRow[0]["ProductId"].ToString() : "";
                CustomerId = thisRow[0]["CustomerId"] != null ? thisRow[0]["CustomerId"].ToString() : "";

                blnStatus = true;
            }
            else
            {
                blnStatus = false;
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
                dbCommand = db.GetStoredProcCommand("StaffLoanSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("StaffLoanSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("StaffLoanSelectAllReversed") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public DataGeneral.SaveStatus Delete()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("StaffLoanDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Update Post
        public SqlCommand UpDatePostCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("StaffLoanUpdatePost") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar,GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Compute Interest
        public decimal ComputeInterest(decimal decLoanPrincipal, decimal decLoanRate, decimal intLoanTenor)
        {
            decimal decLoanInterestAmt = (decLoanPrincipal * decLoanRate * intLoanTenor) / (100 * 12);
            decLoanInterestAmt = Math.Round(decLoanInterestAmt, 2);
            return decLoanInterestAmt;
        }
        #endregion

        #region Get All Loan By Staff
        public DataSet GetAllLoanByStaffAndDate(string strEmployeeNumber,DateTime datPayRollLoanDate)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("StaffLoanSelectByStaffAndDate") as SqlCommand;
            db.AddInParameter(dbCommand, "EmployeeNumber", SqlDbType.VarChar, strEmployeeNumber.Trim());
            db.AddInParameter(dbCommand, "PayRollDate", SqlDbType.DateTime,datPayRollLoanDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

    }
}
