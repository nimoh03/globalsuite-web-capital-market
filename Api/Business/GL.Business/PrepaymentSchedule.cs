using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class PrepaymentSchedule
    {
        #region Declaration
        private string strTransNo;
        private long lngPrepaymentNumber;
        private int intRecordNumber;
        private decimal decAmount;
        private DateTime datDueDate;
        private long lngExpensedNumber;
        private DatabaseProviderFactory factory = new DatabaseProviderFactory();
        private SqlDatabase db;
        public PrepaymentSchedule()
        {
            db = factory.Create("GlobalSuitedb") as SqlDatabase;
        }

        
        #endregion

        #region Properties

        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }

        public long PrepaymentNumber
        {
            set { lngPrepaymentNumber = value; }
            get { return lngPrepaymentNumber; }
        }

        public int RecordNumber
        {
            set { intRecordNumber = value; }
            get { return intRecordNumber; }
        }

        public DateTime DueDate
        {
            set { datDueDate = value; }
            get { return datDueDate; }
        }

        public decimal Amount
        {
            set { decAmount = value; }
            get { return decAmount; }
        }

        public long ExpensedNumber
        {
            set { lngExpensedNumber = value; }
            get { return lngExpensedNumber; }
        }
        #endregion

        #region Save
        public void Save(ref SqlTransaction transaction)
        {
            GeneralFunc oGeneralFunc = new GeneralFunc();
            IFormatProvider format = new CultureInfo("en-GB");
            Prepayment oPrepayment = new Prepayment();
            oPrepayment.TransNo = lngPrepaymentNumber.ToString();
            if (!oPrepayment.GetPrepayment(DataGeneral.PostStatus.UnPosted))
            {
                throw new Exception("Cannot Schedule Prepayment,Prepayment Setup Number Does Not Exist");
            }

            //Delete Prepayment Schedule
            PrepaymentSchedule oPrepaymentSchedule = new PrepaymentSchedule();
            oPrepaymentSchedule.PrepaymentNumber = long.Parse(oPrepayment.TransNo);
            SqlCommand dbCommandPrepaymentScheduleDelete = oPrepaymentSchedule.DeleteCommand();
            db.ExecuteNonQuery(dbCommandPrepaymentScheduleDelete, transaction);

            decimal decPaymentAmount = 0;
            SqlCommand dbCommandPrepaymentSchedule = null;

            DateTime datNewPrepaymentStartDate = oPrepayment.FirstDueDate;
            decPaymentAmount = oPrepayment.Amount / oPrepayment.NumberOfTimeExpensed;
            decPaymentAmount = Math.Round(decPaymentAmount, 2);

            //Start Saving To Prepayment Schedule
            for (int i = 1; i <= oPrepayment.NumberOfTimeExpensed; i++)
            {
                if (oPrepayment.PaymentFrequency.Trim() == "1")
                {
                    oPrepaymentSchedule.PrepaymentNumber = long.Parse(oPrepayment.TransNo);
                    oPrepaymentSchedule.RecordNumber = i;
                    oPrepaymentSchedule.Amount = decPaymentAmount;
                    if (i != oPrepayment.NumberOfTimeExpensed)
                    {
                        oPrepaymentSchedule.Amount = decPaymentAmount;
                    }
                    else
                    {
                        oPrepaymentSchedule.Amount = oPrepayment.Amount - (decPaymentAmount * (oPrepayment.NumberOfTimeExpensed - 1));
                    }
                    oPrepaymentSchedule.DueDate = DateTime.ParseExact(datNewPrepaymentStartDate.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                    datNewPrepaymentStartDate = DateTime.ParseExact(DateTime.ParseExact(datNewPrepaymentStartDate.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format).AddDays(1).ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                    dbCommandPrepaymentSchedule = oPrepaymentSchedule.SaveCommand();
                    db.ExecuteNonQuery(dbCommandPrepaymentSchedule, transaction);
                }
                else if (oPrepayment.PaymentFrequency.Trim() == "2")
                {
                    oPrepaymentSchedule.PrepaymentNumber = long.Parse(oPrepayment.TransNo);
                    oPrepaymentSchedule.RecordNumber = i;
                    if (i != oPrepayment.NumberOfTimeExpensed)
                    {
                        oPrepaymentSchedule.Amount = decPaymentAmount;
                    }
                    else
                    {
                        oPrepaymentSchedule.Amount = oPrepayment.Amount - (decPaymentAmount * (oPrepayment.NumberOfTimeExpensed - 1));
                    }
                    oPrepaymentSchedule.DueDate = DateTime.ParseExact(datNewPrepaymentStartDate.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                    datNewPrepaymentStartDate = DateTime.ParseExact(DateTime.ParseExact(datNewPrepaymentStartDate.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format).AddDays(7).ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                    dbCommandPrepaymentSchedule = oPrepaymentSchedule.SaveCommand();
                    db.ExecuteNonQuery(dbCommandPrepaymentSchedule, transaction);
                }
                else if (oPrepayment.PaymentFrequency.Trim() == "3")
                {
                    oPrepaymentSchedule.PrepaymentNumber = long.Parse(oPrepayment.TransNo);
                    oPrepaymentSchedule.RecordNumber = i;
                    if (i != oPrepayment.NumberOfTimeExpensed)
                    {
                        oPrepaymentSchedule.Amount = decPaymentAmount;
                    }
                    else
                    {
                        oPrepaymentSchedule.Amount = oPrepayment.Amount - (decPaymentAmount * (oPrepayment.NumberOfTimeExpensed - 1));
                    }
                    oPrepaymentSchedule.DueDate = DateTime.ParseExact(datNewPrepaymentStartDate.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                    if (DateTime.ParseExact(datNewPrepaymentStartDate.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format).Day == int.Parse(GeneralFunc.GetMonthLastDay(DateTime.ParseExact(datNewPrepaymentStartDate.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format).Month, DateTime.ParseExact(datNewPrepaymentStartDate.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format).Year)))
                    {
                        datNewPrepaymentStartDate = DateTime.ParseExact(DateTime.ParseExact(datNewPrepaymentStartDate.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format).AddMonths(1).ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                        datNewPrepaymentStartDate = DateTime.ParseExact(GeneralFunc.GetMonthLastDay(datNewPrepaymentStartDate.Month, datNewPrepaymentStartDate.Year) + "/" + datNewPrepaymentStartDate.Month.ToString().Trim().PadLeft(2, char.Parse("0")) + "/" + datNewPrepaymentStartDate.Year.ToString().Trim(), "dd/MM/yyyy", format);
                    }
                    else
                    {
                        datNewPrepaymentStartDate = DateTime.ParseExact(DateTime.ParseExact(datNewPrepaymentStartDate.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format).AddMonths(1).ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                    }
                    dbCommandPrepaymentSchedule = oPrepaymentSchedule.SaveCommand();
                    db.ExecuteNonQuery(dbCommandPrepaymentSchedule, transaction);
                }
                else if (oPrepayment.PaymentFrequency.Trim() == "4")
                {
                    oPrepaymentSchedule.PrepaymentNumber = long.Parse(oPrepayment.TransNo);
                    oPrepaymentSchedule.RecordNumber = i;
                    if (i != oPrepayment.NumberOfTimeExpensed)
                    {
                        oPrepaymentSchedule.Amount = decPaymentAmount;
                    }
                    else
                    {
                        oPrepaymentSchedule.Amount = oPrepayment.Amount - (decPaymentAmount * (oPrepayment.NumberOfTimeExpensed - 1));
                    }
                    oPrepaymentSchedule.DueDate = DateTime.ParseExact(datNewPrepaymentStartDate.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                    if (DateTime.ParseExact(datNewPrepaymentStartDate.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format).Day == int.Parse(GeneralFunc.GetMonthLastDay(DateTime.ParseExact(datNewPrepaymentStartDate.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format).Month, DateTime.ParseExact(datNewPrepaymentStartDate.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format).Year)))
                    {
                        datNewPrepaymentStartDate = DateTime.ParseExact(DateTime.ParseExact(datNewPrepaymentStartDate.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format).AddMonths(3).ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                        datNewPrepaymentStartDate = DateTime.ParseExact(GeneralFunc.GetMonthLastDay(datNewPrepaymentStartDate.Month, datNewPrepaymentStartDate.Year) + "/" + datNewPrepaymentStartDate.Month.ToString().Trim().PadLeft(2, char.Parse("0")) + "/" + datNewPrepaymentStartDate.Year.ToString().Trim(), "dd/MM/yyyy", format);
                    }
                    else
                    {
                        datNewPrepaymentStartDate = DateTime.ParseExact(DateTime.ParseExact(datNewPrepaymentStartDate.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format).AddMonths(3).ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                    }
                    dbCommandPrepaymentSchedule = oPrepaymentSchedule.SaveCommand();
                    db.ExecuteNonQuery(dbCommandPrepaymentSchedule, transaction);
                }
                else if (oPrepayment.PaymentFrequency.Trim() == "5")
                {
                    oPrepaymentSchedule.PrepaymentNumber = long.Parse(oPrepayment.TransNo);
                    oPrepaymentSchedule.RecordNumber = i;
                    if (i != oPrepayment.NumberOfTimeExpensed)
                    {
                        oPrepaymentSchedule.Amount = decPaymentAmount;
                    }
                    else
                    {
                        oPrepaymentSchedule.Amount = oPrepayment.Amount - (decPaymentAmount * (oPrepayment.NumberOfTimeExpensed - 1));
                    }
                    oPrepaymentSchedule.DueDate = DateTime.ParseExact(datNewPrepaymentStartDate.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                    datNewPrepaymentStartDate = DateTime.ParseExact(DateTime.ParseExact(datNewPrepaymentStartDate.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format).AddYears(1).ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                    dbCommandPrepaymentSchedule = oPrepaymentSchedule.SaveCommand();
                    db.ExecuteNonQuery(dbCommandPrepaymentSchedule, transaction);
                }
            }
        }
        #endregion

        #region Save Return Command
        public SqlCommand SaveCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            oCommand = db.GetStoredProcCommand("PrepaymentScheduleAdd") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, "");
            db.AddInParameter(oCommand, "PrepaymentNumber", SqlDbType.BigInt, lngPrepaymentNumber);
            db.AddInParameter(oCommand, "RecordNumber", SqlDbType.Int, intRecordNumber);
            db.AddInParameter(oCommand, "DueDate", SqlDbType.DateTime, datDueDate);
            db.AddInParameter(oCommand, "Amount", SqlDbType.Money, decAmount);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Delete And Return Command
        public SqlCommand DeleteCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PrepaymentScheduleDelete") as SqlCommand;
            db.AddInParameter(oCommand, "PrepaymentNumber", SqlDbType.BigInt, lngPrepaymentNumber);
            return oCommand;
        }
        #endregion

        #region Get All
        public DataSet GetAll(DataGeneral.PostStatus TranStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TranStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentScheduleSelectAll") as SqlCommand;
            }
            else if (TranStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentScheduleSelectAllDue") as SqlCommand;
                db.AddInParameter(dbCommand, "DueDate", SqlDbType.DateTime, datDueDate);
            }
            db.AddInParameter(dbCommand, "PrepaymentNumber", SqlDbType.BigInt, lngPrepaymentNumber);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get
        public bool GetPrepaymentSchedule(DataGeneral.PostStatus TranStatus)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TranStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentScheduleSelect") as SqlCommand;
            }
            else if (TranStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentScheduleSelectDue") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                lngPrepaymentNumber = long.Parse(thisRow[0]["PrepaymentNumber"].ToString());
                intRecordNumber = int.Parse(thisRow[0]["RecordNumber"].ToString());
                datDueDate = DateTime.ParseExact(thisRow[0]["DueDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString());
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Update Expensed Number To Zero
        public void UpdateExpensedNumberToZero(ref SqlTransaction transaction)
        {
            SqlCommand oCommand = db.GetStoredProcCommand("PrepaymentScheduleUpdateExpensedNumberToZero") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.ExecuteNonQuery(oCommand, transaction);
        }
        #endregion

        #region Update Expensed Number
        public void UpdateExpensedNumber(ref SqlTransaction transaction)
        {
            SqlCommand oCommand = db.GetStoredProcCommand("PrepaymentScheduleUpdateExpensedNumber") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "ExpensedNumber", SqlDbType.BigInt, lngExpensedNumber);
            db.ExecuteNonQuery(oCommand, transaction);
        }
        #endregion

        #region Get Number Of Schedule
        public int GetNumberOfSchedule()
        {
            SqlCommand oCommand = db.GetStoredProcCommand("PrepaymentScheduleSelectNumberOfSchedule") as SqlCommand;
            db.AddInParameter(oCommand, "PrepaymentNumber", SqlDbType.BigInt, lngPrepaymentNumber);
            return int.Parse(db.ExecuteScalar(oCommand).ToString());
        }
        #endregion
        
    }   
}
