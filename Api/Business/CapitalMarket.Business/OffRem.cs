using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using BaseUtility.Business;

namespace CapitalMarket.Business
{
    public class OffRem
    {
        #region Declaration
        private string strTransNo, strTransNoRev, strOffCode, strMasAcct, strSubAcct;
        private DateTime datPVDate,datPVDateTo;
        private string strDescription;
        private decimal decAmount;
        private DateTime datChqDate;
        private string strChqNo, strRefNo;
        private bool blnReversed, blnPosted, blnCash, blnCheque;
        private string strAmtWord, strPvNo, strSubAcct2, strSubAcct3;
        private decimal decSubAmt, decSubAmt2, decSubAmt3;
        private string strSaveType, strRecNo,strBranch;
        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public string TransNoRev
        {
            set { strTransNoRev = value; }
            get { return strTransNoRev; }
        }
        public string OffCode
        {
            set { strOffCode = value; }
            get { return strOffCode; }
        }
        public string MasAcct
        {
            set { strMasAcct = value; }
            get { return strMasAcct; }
        }
        public string SubAcct
        {
            set { strSubAcct = value; }
            get { return strSubAcct; }
        }
        public DateTime PVDate
        {
            set { datPVDate = value; }
            get { return datPVDate; }
        }
        public DateTime PVDateTo
        {
            set { datPVDateTo = value; }
            get { return datPVDateTo; }
        }
        public string Description
        {
            set { strDescription = value; }
            get { return strDescription; }
        }
        public decimal Amount
        {
            set { decAmount = value; }
            get { return decAmount; }
        }
        public DateTime ChqDate
        {
            set { datChqDate = value; }
            get { return datChqDate; }
        }
        public string ChqNo
        {
            set { strChqNo = value; }
            get { return strChqNo; }
        }
        public string RefNo
        {
            set { strRefNo = value; }
            get { return strRefNo; }
        }
        public bool Reversed
        {
            set { blnReversed = value; }
            get { return blnReversed; }
        }
        public bool Posted
        {
            set { blnPosted = value; }
            get { return blnPosted; }
        }
        public bool Cash
        {
            set { blnCash = value; }
            get { return blnCash; }
        }
        public bool Cheque
        {
            set { blnCheque = value; }
            get { return blnCheque; }
        }
        
        public string AmtWord
        {
            set { strAmtWord = value; }
            get { return strAmtWord; }
        }
        public string PvNo
        {
            set { strPvNo = value; }
            get { return strPvNo; }
        }
        public string SubAcct2
        {
            set { strSubAcct2 = value; }
            get { return strSubAcct2; }
        }
        public string SubAcct3
        {
            set { strSubAcct3 = value; }
            get { return strSubAcct3; }
        }
        public decimal SubAmt
        {
            set { decSubAmt = value; }
            get { return decSubAmt; }
        }
        public decimal SubAmt2
        {
            set { decSubAmt2 = value; }
            get { return decSubAmt2; }
        }
        public decimal SubAmt3
        {
            set { decSubAmt3 = value; }
            get { return decSubAmt3; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }

        public string RecNo
        {
            set { strRecNo = value; }
            get { return strRecNo; }
        }

        public string Branch
        {
            set { strBranch = value; }
            get { return strBranch; }
        }
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
                    SqlCommand dbCommand = null;
                    if (strSaveType == "ADDS")
                    {
                        dbCommand = db.GetStoredProcCommand("OffRemAddNew") as SqlCommand;
                        db.AddInParameter(dbCommand, "TableName", SqlDbType.VarChar, "OFFREM");
                    }
                    else if (strSaveType == "EDIT")
                    {
                        dbCommand = db.GetStoredProcCommand("OffRemEdit") as SqlCommand;
                    }
                    db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo);
                    db.AddInParameter(dbCommand, "OffCode", SqlDbType.VarChar, strOffCode);
                    db.AddInParameter(dbCommand, "MasAcct", SqlDbType.VarChar, strMasAcct);
                    db.AddInParameter(dbCommand, "SubAcct", SqlDbType.VarChar, strSubAcct);
                    db.AddInParameter(dbCommand, "PVDate", SqlDbType.DateTime, datPVDate);
                    db.AddInParameter(dbCommand, "Description", SqlDbType.VarChar, strDescription);
                    db.AddInParameter(dbCommand, "Amount", SqlDbType.Decimal, decAmount);
                    db.AddInParameter(dbCommand, "ChqDate", SqlDbType.DateTime, datChqDate);
                    db.AddInParameter(dbCommand, "ChqNo", SqlDbType.VarChar, strChqNo);
                    db.AddInParameter(dbCommand, "RefNo", SqlDbType.VarChar, strRefNo);
                    db.AddInParameter(dbCommand, "Reversed", SqlDbType.Bit, blnReversed);
                    db.AddInParameter(dbCommand, "Posted", SqlDbType.Bit, blnPosted);
                    db.AddInParameter(dbCommand, "Cash", SqlDbType.Bit, blnCash);
                    db.AddInParameter(dbCommand, "Cheque", SqlDbType.Bit, blnCheque);
                    db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    db.AddInParameter(dbCommand, "AmtWord", SqlDbType.VarChar, strAmtWord);
                    db.AddInParameter(dbCommand, "PvNo", SqlDbType.VarChar, strPvNo);
                    db.AddInParameter(dbCommand, "SubAcct2", SqlDbType.VarChar, strSubAcct2);
                    db.AddInParameter(dbCommand, "SubAcct3", SqlDbType.VarChar, strSubAcct3);
                    db.AddInParameter(dbCommand, "SubAmt", SqlDbType.Decimal, decSubAmt);
                    db.AddInParameter(dbCommand, "SubAmt2", SqlDbType.Decimal, decSubAmt2);
                    db.AddInParameter(dbCommand, "SubAmt3", SqlDbType.Decimal, decSubAmt3);
                    db.AddInParameter(dbCommand, "Branch", SqlDbType.VarChar, "");
                    db.ExecuteNonQuery(dbCommand, transaction);
                    if (strTransNoRev.Trim() != "")
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

        #region Check TransNo Exist
        public bool ChkTransNoExist(DataGeneral.PostStatus ePostStatus)
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = null;
                if (ePostStatus == DataGeneral.PostStatus.UnPosted)
                {
                    oCommand = db.GetStoredProcCommand("OffRemChkTransNoExistUnPosted") as SqlCommand;
                }
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

        #region Get Offer Remmittance Posting Info
        public bool GetOffRem(DataGeneral.PostStatus ePostStatus)
        {
            bool blnStatus = false;
           
                System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
                IFormatProvider format = new CultureInfo("en-GB");
                DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
                dtfi.ShortDatePattern = "dd/MM/yyyy";
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = null;
                if (ePostStatus == DataGeneral.PostStatus.UnPosted)
                {
                    dbCommand = db.GetStoredProcCommand("OffRemSelectUnPosted") as SqlCommand;
                }
                else if (ePostStatus == DataGeneral.PostStatus.Posted)
                {
                    dbCommand = db.GetStoredProcCommand("OffRemSelectPosted") as SqlCommand;
                }
                else if (ePostStatus == DataGeneral.PostStatus.Reversed)
                {
                    dbCommand = db.GetStoredProcCommand("OffRemSelectReversed") as SqlCommand;
                }
                db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo);
                DataSet oDS = db.ExecuteDataSet(dbCommand);
                DataTable thisTable = oDS.Tables[0];
                DataRow[] thisRow = thisTable.Select();
                if (thisRow.Length == 1)
                {
                    strTransNo = thisRow[0]["TransNo"].ToString();
                    strOffCode = thisRow[0]["OffCode"].ToString();
                    strMasAcct = thisRow[0]["MasAcct"].ToString();
                    strSubAcct = thisRow[0]["SubAcct"].ToString();
                    datPVDate = DateTime.ParseExact(thisRow[0]["PVDate"].ToString().Substring(0, 10), "dd/MM/yyyy", format);
                    strDescription = thisRow[0]["Description"].ToString();
                    decAmount = decimal.Parse(thisRow[0]["Amount"].ToString());
                    datChqDate = DateTime.ParseExact(thisRow[0]["ChqDate"].ToString().Substring(0, 10), "dd/MM/yyyy", format);
                    strChqNo = thisRow[0]["ChqNo"].ToString();
                    strRefNo = thisRow[0]["RefNo"].ToString();
                    blnReversed = bool.Parse(thisRow[0]["Reversed"].ToString());
                    blnPosted = bool.Parse(thisRow[0]["Posted"].ToString());
                    blnCash = bool.Parse(thisRow[0]["Cash"].ToString());
                    blnCheque = bool.Parse(thisRow[0]["Cheque"].ToString());
                    strAmtWord = thisRow[0]["AmtWord"].ToString();
                    strPvNo = thisRow[0]["PvNo"].ToString();
                    strSubAcct2 = thisRow[0]["SubAcct2"].ToString();
                    strSubAcct3 = thisRow[0]["SubAcct3"].ToString();
                    if (thisRow[0]["SubAmt"] != null && thisRow[0]["SubAmt"].ToString().Trim() != "")
                    {
                        decSubAmt = decimal.Parse(thisRow[0]["SubAmt"].ToString());
                    }
                    else
                    {
                        decSubAmt = 0;
                    }
                    if (thisRow[0]["SubAmt2"] != null && thisRow[0]["SubAmt2"].ToString().Trim() != "")
                    {
                        decSubAmt2 = decimal.Parse(thisRow[0]["SubAmt2"].ToString());
                    }
                    else
                    {
                        decSubAmt2 = 0;
                    }
                    if (thisRow[0]["SubAmt3"] != null && thisRow[0]["SubAmt3"].ToString().Trim() != "")
                    {
                        decSubAmt3 = decimal.Parse(thisRow[0]["SubAmt3"].ToString());
                    }
                    else
                    {
                        decSubAmt3 = 0;
                    }
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
                dbCommand = db.GetStoredProcCommand("OffRemSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("OffRemSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("OffRemSelectAllReversed") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Offer Remmitance Transactions By Entry Date
        public DataSet GetAllGivenEntryDate(DataGeneral.PostStatus TransStatus, DateTime datTxnDateFrom, DateTime datTxnDateTo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("OffRemSelectAllGivenEntryDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("OffRemSelectAllGivenEntryDatePostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("OffRemSelectAllGivenEntryDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("OffRemSelectAllGivenEntryDateUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "EntryDateFrom", SqlDbType.DateTime, datTxnDateFrom);
            db.AddInParameter(dbCommand, "EntryDateTo", SqlDbType.DateTime, datTxnDateTo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Offer Remmitance Transactions By Effective Date
        public DataSet GetAllGivenEffDate(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("OffRemSelectAllGivenEffDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("OffRemSelectAllGivenEffDatePostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("OffRemSelectAllGivenEffDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("OffRemSelectAllGivenEffDateUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "EffDateFrom", SqlDbType.DateTime,datPVDate);
            db.AddInParameter(dbCommand, "EffDateTo", SqlDbType.DateTime, datPVDateTo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Job Order Transactions By Transaction No
        public DataSet GetAllGivenTxnNo(DataGeneral.PostStatus TransStatus, string strTxnFrom, string strTxnTo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("OffRemSelectAllGivenTxnNoPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("OffRemSelectAllGivenTxnNoPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("OffRemSelectAllGivenTxnNoUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("OffRemSelectAllGivenTxnNoUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TxnFrom", SqlDbType.VarChar, strTxnFrom.Trim());
            db.AddInParameter(dbCommand, "TxnTo", SqlDbType.VarChar, strTxnTo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        

        #region Update Post and Reversal and Return A Command
        public SqlCommand UpDatePostRevCommand()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("OffRemUpdatePostRev") as SqlCommand;

            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "Posted", SqlDbType.Bit, blnPosted);
            db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, blnReversed);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Update Only Reversal and Return A Command
        public SqlCommand UpDateRevCommand()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("OffRemUpdateRev") as SqlCommand;

            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, blnReversed);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        
        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("DepositDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strTransNo.Trim());


            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
                    IFormatProvider format = new CultureInfo("en-GB");


                    db.ExecuteNonQuery(oCommand, transaction);
                    
                    transaction.Commit();
                    blnStatus = true;

                }
                catch (Exception e)
                {
                    string your = e.Message;
                    transaction.Rollback();
                    blnStatus = false;
                    return blnStatus;

                }
                connection.Close();
            }
            return blnStatus;
        }
        #endregion

        #region Delete Reversal and Return A Command
        public SqlCommand DeleteReversalCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("OffRemDeleteReversal") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNoRev.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.ToUpper());
            return oCommand;
        }
        #endregion
    }
}
