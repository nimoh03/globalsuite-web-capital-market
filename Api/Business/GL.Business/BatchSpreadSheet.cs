using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class BatchSpreadSheet
    {
        #region Properties
        public Int64 BatchSpreadSheetMasterId { set; get; }
        public Int64 BatchSpreadSheetMasterIdRev { set; get; }
        public Int32 BatchSpreadSheetId { set; get; }
        public Int32 BatchSpreadSheetIdRev { set; get; }
        public DateTime EffectiveDate { set; get; }
        public string AccountId { set; get; }
        public string Description { set; get; }
        public decimal Debit { set; get; }
        public decimal Credit { set; get; }
        public string SaveType { set; get; }

        #endregion

        #region Save And Return Command
        public SqlCommand SaveCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (SaveType == "ADDS")
            {
                dbCommand = db.GetStoredProcCommand("BatchSpreadSheetAddNew") as SqlCommand;
            }
            else if (SaveType == "EDIT")
            {
                dbCommand = db.GetStoredProcCommand("BatchSpreadSheetEdit") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "BatchSpreadSheetMasterId", SqlDbType.BigInt, BatchSpreadSheetMasterId);
            db.AddInParameter(dbCommand, "BatchSpreadSheetId", SqlDbType.BigInt, BatchSpreadSheetId);
            db.AddInParameter(dbCommand, "AccountID", SqlDbType.VarChar, AccountId.Trim());
            db.AddInParameter(dbCommand, "EffectiveDate", SqlDbType.DateTime, EffectiveDate);
            db.AddInParameter(dbCommand, "Description", SqlDbType.VarChar, Description.Trim());
            db.AddInParameter(dbCommand, "Debit", SqlDbType.Decimal, Debit);
            db.AddInParameter(dbCommand, "Credit", SqlDbType.Decimal, Credit);
            
            db.AddInParameter(dbCommand, "Branch", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());
            return dbCommand;
        }
        #endregion

        #region Add And Return Command
        public SqlCommand AddCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BatchSpreadSheetAddNew") as SqlCommand;
            db.AddInParameter(dbCommand, "BatchSpreadSheetMasterId", SqlDbType.BigInt, BatchSpreadSheetMasterId);
            db.AddInParameter(dbCommand, "BatchSpreadSheetId", SqlDbType.BigInt, BatchSpreadSheetId);
            db.AddInParameter(dbCommand, "AccountID", SqlDbType.VarChar, AccountId.Trim());
            db.AddInParameter(dbCommand, "EffectiveDate", SqlDbType.DateTime, EffectiveDate);
            db.AddInParameter(dbCommand, "Description", SqlDbType.VarChar, Description.Trim());
            db.AddInParameter(dbCommand, "Debit", SqlDbType.Decimal, Debit);
            db.AddInParameter(dbCommand, "Credit", SqlDbType.Decimal, Credit);
            db.AddInParameter(dbCommand, "Branch", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());
            return dbCommand;
        }
        #endregion

        #region Save Other Reversed Return Command
        public SqlCommand SaveOtherReversedCommand(Int64 BatchSpreadSheetMasterIdNew)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BatchSpreadSheetSaveOtherReversedCommand") as SqlCommand;
            db.AddInParameter(dbCommand, "BatchSpreadSheetNoNew", SqlDbType.VarChar, BatchSpreadSheetMasterIdNew);
            db.AddInParameter(dbCommand, "BatchSpreadSheetNo", SqlDbType.VarChar, BatchSpreadSheetMasterId);
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, BatchSpreadSheetId);
            return dbCommand;
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
                    oCommand = db.GetStoredProcCommand("BatchSpreadSheetChkTransNoExistUnPosted") as SqlCommand;
                }
                db.AddInParameter(oCommand, "BatchSpreadSheetMasterId", SqlDbType.BigInt, BatchSpreadSheetMasterId);
                db.AddInParameter(oCommand, "BatchSpreadSheetId", SqlDbType.BigInt, BatchSpreadSheetId);
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

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BatchSpreadSheetSelectAll") as SqlCommand;
            db.AddInParameter(dbCommand, "BatchSpreadSheetMasterId", SqlDbType.BigInt, BatchSpreadSheetMasterId);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Batch Post Transaction Posting Info UnPosted and Reversed
        public bool GetBatch(DataGeneral.PostStatus TransStatus)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
            dtfi.ShortDatePattern = "dd/MM/yyyy";
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("BatchSpreadSheetSelectUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("BatchSpreadSheetSelectPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("BatchSpreadSheetSelectReversed") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "BatchSpreadSheetNo", SqlDbType.VarChar, BatchSpreadSheetMasterId);
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, BatchSpreadSheetId);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                BatchSpreadSheetMasterId = Convert.ToInt64( thisRow[0]["BatchSpreadSheetSpreadSheetMasterId"]);
                BatchSpreadSheetId = Convert.ToInt32( thisRow[0]["BatchSpreadSheetId"]);
                AccountId = thisRow[0]["AccountId"].ToString();
                EffectiveDate = DateTime.ParseExact(DateTime.Parse(thisRow[0]["EffectiveDate"].ToString()).ToString("d", dtfi), "dd/MM/yyyy", format);
                Description = thisRow[0]["Description"].ToString();
                Debit = decimal.Parse(thisRow[0]["Debit"].ToString());
                Credit = decimal.Parse(thisRow[0]["Credit"].ToString());
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }


            return blnStatus;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BatchSpreadSheetDelete") as SqlCommand;
            db.AddInParameter(oCommand, "BatchSpreadSheetMasterId", SqlDbType.VarChar, BatchSpreadSheetMasterId);
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, BatchSpreadSheetId);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;


            return blnStatus;
        }
        #endregion

        #region Delete All By BatchSpreadSheetId Return Command
        public SqlCommand DeleteAllByBatchSpreadSheetIdCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BatchSpreadSheetDeleteAllByBatchSpreadSheetId") as SqlCommand;
            db.AddInParameter(oCommand, "BatchSpreadSheetMasterId", SqlDbType.BigInt, BatchSpreadSheetMasterId);
            return oCommand;
        }
        #endregion

        #region Get Batch Date
        public DateTime GetBatchDate()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BatchSpreadSheetSelectBatchDate") as SqlCommand;
            db.AddInParameter(oCommand, "BatchSpreadSheetNo", SqlDbType.VarChar, BatchSpreadSheetMasterId);
            var varBatchDate = db.ExecuteScalar(oCommand);
            return varBatchDate != null && varBatchDate.ToString().Trim() != "" ? DateTime.ParseExact(varBatchDate.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format) : DateTime.MinValue;
        }
        #endregion

        #region Change Batch Date
        public void ChangeBatchDate(string strNewDate)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BatchSpreadSheetChangeBatchDate") as SqlCommand;
            db.AddInParameter(oCommand, "BatchSpreadSheetSpreadSheetMaster", SqlDbType.VarChar, BatchSpreadSheetMasterId);
            db.AddInParameter(oCommand, "NewDate", SqlDbType.DateTime, DateTime.ParseExact(strNewDate, "dd/MM/yyyy", format));
            db.ExecuteNonQuery(oCommand);
        }
        #endregion
    }
}
