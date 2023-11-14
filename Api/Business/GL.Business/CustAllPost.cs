using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class CustAllPost
    {

        #region Properties
        public Int64 TransNo { get; set; }
        public Int64 TransNoRev { get; set; }
        public DateTime EffDate { get; set; }
        public string ProductCode { get; set; }
        public string Narration { get; set; }
        public decimal Amount { get; set; }
        public string DebCred { get; set; }
        public string GLAccountSecondEntry { get; set; }
        public bool Posted { get; set; }
        public bool Reversed { get; set; }
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
                        oCommand = db.GetStoredProcCommand("CustAllPostAddNew") as SqlCommand;
                    }
                    else if (SaveType == "EDIT")
                    {
                        oCommand = db.GetStoredProcCommand("CustAllPostEdit") as SqlCommand;
                    }
                    db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, TransNo);
                    db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, EffDate);
                    db.AddInParameter(oCommand, "Narration", SqlDbType.VarChar, Narration.Trim());
                    db.AddInParameter(oCommand, "Amount", SqlDbType.Decimal, Amount);
                    db.AddInParameter(oCommand, "ProductCode", SqlDbType.VarChar, ProductCode);
                    db.AddInParameter(oCommand, "Posted", SqlDbType.Bit, Posted);
                    db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, Reversed);
                    db.AddInParameter(oCommand, "DebCred", SqlDbType.VarChar, DebCred.Trim());
                    db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    db.AddInParameter(oCommand, "Branch", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());
                    db.ExecuteNonQuery(oCommand, transaction);
                    if (TransNoRev != 0)
                    {
                        SqlCommand dbCommandDeleteReversal = DeleteReversalCommand();
                        db.ExecuteNonQuery(dbCommandDeleteReversal, transaction);
                    }
                    transaction.Commit();
                    enSaveStatus = DataGeneral.SaveStatus.Saved;
                }
                catch (Exception err)
                {
                    transaction.Rollback();
                    throw new Exception(err.Message);
                }
            }
            return enSaveStatus;
        }
        #endregion

        #region Save And Return Command
        public SqlCommand SaveCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("CustAllPostAddNew") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, TransNo);
            db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, EffDate);
            db.AddInParameter(oCommand, "Narration", SqlDbType.VarChar, Narration.Trim());
            db.AddInParameter(oCommand, "Amount", SqlDbType.Decimal, Amount);
            db.AddInParameter(oCommand, "ProductCode", SqlDbType.VarChar, ProductCode);
            db.AddInParameter(oCommand, "Posted", SqlDbType.Bit, Posted);
            db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, Reversed);
            db.AddInParameter(oCommand, "DebCred", SqlDbType.VarChar, DebCred.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.AddInParameter(oCommand, "Branch", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());
            return oCommand;
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
                    oCommand = db.GetStoredProcCommand("CustAllPostChkTransNoExistUnPosted") as SqlCommand;
                }
                db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, TransNo);
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
        public DataSet GetAll(DataGeneral.PostStatus TransStatus)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("CustAllPostSelectAllUnPosted") as SqlCommand;
            }

            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("CustAllPostSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("CustAllPostSelectAllReversed") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("CustAllPostSelectAllPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("CustAllPostSelectAllUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        

        

        #region Get All Customer Opening Balance Transactions By Transaction No
        public DataSet GetAllGivenTxnNo(DataGeneral.PostStatus TransStatus, Int64 intTxnFrom, Int64 intTxnTo)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("CustAllPostSelectAllGivenTxnNoPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("CustAllPostSelectAllGivenTxnNoPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("CustAllPostSelectAllGivenTxnNoUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("CustAllPostSelectAllGivenTxnNoUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TxnFrom", SqlDbType.BigInt, intTxnFrom);
            db.AddInParameter(dbCommand, "TxnTo", SqlDbType.BigInt, intTxnTo);
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Customer Account Balance Posting 
        public bool GetCustAllPost(DataGeneral.PostStatus TransStatus)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            bool blnStatus = false;
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("CustAllPostSelectUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("CustAllPostSelectPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("CustAllPostSelectReversed") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, TransNo);
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                EffDate = DateTime.ParseExact(thisRow[0]["EffDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                Narration = thisRow[0]["Narration"].ToString();
                Amount = decimal.Parse(thisRow[0]["Amount"].ToString());
                ProductCode = thisRow[0]["ProductCode"].ToString();
                DebCred = thisRow[0]["DebCred"].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }

            return blnStatus;
        }
        #endregion

        #region Update Post and Return A Command
        public SqlCommand UpDatePostCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("CustAllPostUpdatePost") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.BigInt, TransNo);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Update Only Reversal and Return A Command
        public SqlCommand UpDateRevCommand()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("CustAllPostUpdateRev") as SqlCommand;

            db.AddInParameter(oCommand, "Transno", SqlDbType.BigInt, TransNo);
            db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, Reversed);
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName);
            return oCommand;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("CustAllPostDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.BigInt, TransNo);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;


            return blnStatus;
        }
        #endregion

        #region Delete Reversal and Return A Command
        public SqlCommand DeleteReversalCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("CustAllPostDeleteReversal") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.BigInt, TransNoRev);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.ToUpper());
            return oCommand;
        }
        #endregion
    }
}
