using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class Batch
    {
        #region Declaration
        private string strBatchNo, strBatchNoRev, strTransNoRev;
        private string strTransNo, strAccountID, strSub, strProductCode;
        private DateTime datVdate;
        private string strDescription;
        private decimal decDebit, decCredit;
        private string strTransType, strPosted, strReversed, strApproved, strApprovedBy, strChequeNo;
        private string strSaveType;
        private DataGeneral.GLInstrumentType enumInstrumentType;

        #endregion

        #region Properties

        public string BatchNo
        {
            set { strBatchNo = value; }
            get { return strBatchNo; }
        }
        public string BatchNoRev
        {
            set { strBatchNoRev = value; }
            get { return strBatchNoRev; }
        }
        public string TransNoRev
        {
            set { strTransNoRev = value; }
            get { return strTransNoRev; }
        }
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public string AccountID
        {
            set { strAccountID = value; }
            get { return strAccountID; }
        }
        public string Sub
        {
            set { strSub = value; }
            get { return strSub; }
        }
        public string ProductCode
        {
            set { strProductCode = value; }
            get { return strProductCode; }
        }
        public DateTime Vdate
        {
            set { datVdate = value; }
            get { return datVdate; }
        }
        
        public string Description
        {
            set { strDescription = value; }
            get { return strDescription; }
        }
        public decimal Debit
        {
            set { decDebit = value; }
            get { return decDebit; }
        }
        public decimal Credit
        {
            set { decCredit = value; }
            get { return decCredit; }
        }
        public string TransType
        {
            set { strTransType = value; }
            get { return strTransType; }
        }
        public string Posted
        {
            set { strPosted = value; }
            get { return strPosted; }
        }
        public string Reversed
        {
            set { strReversed = value; }
            get { return strReversed; }
        }
        public string Approved
        {
            set { strApproved = value; }
            get { return strApproved; }
        }
        public string ApprovedBy
        {
            set { strApprovedBy = value; }
            get { return strApprovedBy; }
        }
        

        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        
        public string ChequeNo
        {
            set { strChequeNo = value; }
            get { return strChequeNo; }
        }
        public DataGeneral.GLInstrumentType InstrumentType
        {
            set { enumInstrumentType = value; }
            get { return enumInstrumentType; }
        }
        #endregion

        #region Save
        public DataGeneral.SaveStatus Save()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            BatchOwner oBatchOwner = new BatchOwner();
            BatchOwner oBatchOwnerChkReversalDate = new BatchOwner();
            Batch oBatchChkReversedAmountAndDebCred = new Batch();
            oBatchOwner.SaveType = strSaveType;
            oBatchOwner.Batchno = strBatchNo;
            if (!oBatchOwner.ChkBatchOwnerExist(DataGeneral.PostStatus.UnPosted))
            {
                enSaveStatus = DataGeneral.SaveStatus.NotExist;
                return enSaveStatus;
            }
            if (!ChkTransNoExist(DataGeneral.PostStatus.UnPosted))
            {
                enSaveStatus = DataGeneral.SaveStatus.NotExist;
                return enSaveStatus;
            }
            var varReturnDate = oBatchOwner.GetDateOfBatchOwner();
            if (DateTime.Parse(varReturnDate.ToString()) != DateTime.MinValue)
            {
                if (DateTime.ParseExact(varReturnDate.ToString().Substring(0,10), "dd/MM/yyyy", format) != datVdate && GetTransGivenBatchNo().Tables[0].Rows.Count != 0)
                {
                    enSaveStatus = DataGeneral.SaveStatus.DateNotEqual;
                    return enSaveStatus;
                }
            }

            if (strBatchNoRev.Trim() != "" && strTransNoRev.Trim() != "")
            {
                oBatchOwnerChkReversalDate.Batchno = strBatchNoRev;
                var varReturnDateReversal = oBatchOwnerChkReversalDate.GetDateOfBatchOwner();
                if (DateTime.Parse(varReturnDateReversal.ToString()) != DateTime.MinValue)
                {
                    if (DateTime.ParseExact(varReturnDateReversal.ToString().Substring(0, 10), "dd/MM/yyyy",format) != datVdate)
                    {
                        enSaveStatus = DataGeneral.SaveStatus.DateNotEqual;
                        return enSaveStatus;
                    }
                }

                //oBatchChkReversedAmountAndDebCred.BatchNo = strBatchNoRev;
                //oBatchChkReversedAmountAndDebCred.TransNo = strTransNoRev;
                //oBatchChkReversedAmountAndDebCred.GetBatch(DataGeneral.PostStatus.Reversed);
                
                //if (oBatchChkReversedAmountAndDebCred.Credit != Credit ||
                //    oBatchChkReversedAmountAndDebCred.decCredit != Debit)
                //{
                //    enSaveStatus = DataGeneral.SaveStatus.DuplicateRef;
                //    return enSaveStatus;
                //}
            }

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    string strReturnBatchNumber = "";
                    if (strSaveType == "ADDS")
                    {
                        BatchOwner oBatchOwnerAddNew = new BatchOwner();
                        SqlCommand dbCommandBatchOwner = null;
                        if (strBatchNoRev.Trim() != "" && strTransNoRev.Trim() != "")
                        {
                            oBatchOwnerAddNew.ValDate = datVdate;
                            dbCommandBatchOwner = oBatchOwnerAddNew.AddCommand();
                            db.ExecuteNonQuery(dbCommandBatchOwner, transaction);
                            strReturnBatchNumber = db.GetParameterValue(dbCommandBatchOwner, "Batchno").ToString();
                        }
                        else if (int.Parse(strTransNo) == 1)
                        {
                            oBatchOwnerAddNew.ValDate = datVdate;
                            dbCommandBatchOwner = oBatchOwnerAddNew.AddCommand();
                            db.ExecuteNonQuery(dbCommandBatchOwner, transaction);
                            strReturnBatchNumber = db.GetParameterValue(dbCommandBatchOwner, "Batchno").ToString();
                        }
                    }
                    SqlCommand dbCommand = null;
                    if (strSaveType == "ADDS")
                    {
                        if (strBatchNoRev.Trim() != "" && strTransNoRev.Trim() != "")
                        {
                            dbCommand = db.GetStoredProcCommand("BatchAddNewReversed") as SqlCommand;
                            db.AddInParameter(dbCommand, "BatchNo", SqlDbType.VarChar, strReturnBatchNumber.Trim());
                        }
                        else if (int.Parse(strTransNo) == 1)
                        {
                            dbCommand = db.GetStoredProcCommand("BatchAddNew") as SqlCommand;
                            db.AddInParameter(dbCommand, "BatchNo", SqlDbType.VarChar, strReturnBatchNumber.Trim());
                        }
                        else
                        {
                            dbCommand = db.GetStoredProcCommand("BatchAddNew") as SqlCommand;
                            db.AddInParameter(dbCommand, "BatchNo", SqlDbType.VarChar, strBatchNo.Trim());
                        }
                    }
                    else if (strSaveType == "EDIT")
                    {
                        dbCommand = db.GetStoredProcCommand("BatchEdit") as SqlCommand;
                        db.AddInParameter(dbCommand, "BatchNo", SqlDbType.VarChar, strBatchNo.Trim());
                    }
                    db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
                    db.AddInParameter(dbCommand, "AccountID", SqlDbType.VarChar, strAccountID.Trim());
                    db.AddInParameter(dbCommand, "Sub", SqlDbType.VarChar, strSub.Trim());
                    db.AddInParameter(dbCommand, "ProductCode", SqlDbType.Char, strProductCode.Trim());
                    db.AddInParameter(dbCommand, "Vdate", SqlDbType.DateTime, datVdate);
                    db.AddInParameter(dbCommand, "ChequeDate", SqlDbType.DateTime, SqlDateTime.Null);
                    db.AddInParameter(dbCommand, "Description", SqlDbType.VarChar, strDescription.Trim());
                    db.AddInParameter(dbCommand, "Debit", SqlDbType.Decimal, decDebit);
                    db.AddInParameter(dbCommand, "Credit", SqlDbType.Decimal, decCredit);
                    db.AddInParameter(dbCommand, "TransType", SqlDbType.VarChar, strTransType.Trim());
                    db.AddInParameter(dbCommand, "Approved", SqlDbType.Char, strApproved.Trim());
                    db.AddInParameter(dbCommand, "Posted", SqlDbType.VarChar, "N");
                    db.AddInParameter(dbCommand, "Reversed", SqlDbType.VarChar, "N");
                    db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    db.AddInParameter(dbCommand, "ChequeNo", SqlDbType.VarChar, strChequeNo.Trim());
                    db.AddInParameter(dbCommand, "InstrumentType", SqlDbType.VarChar, enumInstrumentType.ToString().Trim());
                    db.AddInParameter(dbCommand, "Branch", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());
                    if (strSaveType == "ADDS")
                    {
                        if (strBatchNoRev.Trim() != "" && strTransNoRev.Trim() != "")
                        {
                            Batch oBatchOtherOfReversed = new Batch();
                            oBatchOtherOfReversed.BatchNo = strBatchNoRev.Trim();
                            oBatchOtherOfReversed.TransNo = strTransNoRev.Trim();
                            SqlCommand dbCommandOtherReversed = oBatchOtherOfReversed.SaveOtherReversedCommand(strReturnBatchNumber);
                            db.ExecuteNonQuery(dbCommandOtherReversed, transaction);

                            BatchOwner oBatchOwnerDeleteReversal = new BatchOwner();
                            oBatchOwnerDeleteReversal.Batchno = strBatchNoRev;
                            SqlCommand dbCommandDeleteReversal = oBatchOwnerDeleteReversal.DeleteReversalCommand();
                            db.ExecuteNonQuery(dbCommandDeleteReversal, transaction);
                        }
                    }
                    db.ExecuteNonQuery(dbCommand,transaction);
                    transaction.Commit();
                    enSaveStatus = DataGeneral.SaveStatus.Saved;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    string strErrMessage = "Error In Saving Batch Postings " + ex.Message.Trim();
                    throw new Exception(strErrMessage);
                }
            }
            return enSaveStatus;
        }
        #endregion

        #region Save Other Reversed Return Command
        public SqlCommand SaveOtherReversedCommand(string strBatchNoNew)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BatchSaveOtherReversedCommand") as SqlCommand;
            db.AddInParameter(dbCommand, "BatchNoNew", SqlDbType.VarChar, strBatchNoNew.Trim());
            db.AddInParameter(dbCommand, "BatchNo", SqlDbType.VarChar, strBatchNo.Trim());
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            return dbCommand;
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
                    oCommand = db.GetStoredProcCommand("BatchChkTransNoExistUnPosted") as SqlCommand;
                }
                db.AddInParameter(oCommand, "BatchNo", SqlDbType.VarChar, strBatchNo.Trim());
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

        #region Get All
        public DataSet GetAll(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("BatchSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("BatchSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("BatchSelectAllReversed") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "BatchNo", SqlDbType.VarChar, strBatchNo.Trim());
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
                dbCommand = db.GetStoredProcCommand("BatchSelectUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("BatchSelectPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("BatchSelectReversed") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "BatchNo", SqlDbType.VarChar, strBatchNo.Trim());
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strBatchNo = thisRow[0]["BatchNo"].ToString();
                strTransNo = thisRow[0]["TransNo"].ToString();
                strAccountID = thisRow[0]["AccountID"].ToString();
                strSub = thisRow[0]["Sub"].ToString();
                strProductCode = thisRow[0]["ProductCode"].ToString();
                datVdate = DateTime.ParseExact(DateTime.Parse(thisRow[0]["Vdate"].ToString()).ToString("d", dtfi), "dd/MM/yyyy", format);
                strDescription = thisRow[0]["Description"].ToString();
                decDebit = decimal.Parse(thisRow[0]["Debit"].ToString());
                decCredit = decimal.Parse(thisRow[0]["Credit"].ToString());
                strTransType = thisRow[0]["TransType"].ToString();
                strPosted = thisRow[0]["Posted"].ToString();
                strReversed = thisRow[0]["Reversed"].ToString();
                strApproved = thisRow[0]["Approved"].ToString();
                strApprovedBy = thisRow[0]["ApprovedBy"].ToString();
                strChequeNo = thisRow[0]["ChequeNo"].ToString().Trim();
                enumInstrumentType = (DataGeneral.GLInstrumentType)Enum.Parse(typeof(DataGeneral.GLInstrumentType), thisRow[0]["InstrumentType"].ToString().Trim(), false); 
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
        

            return blnStatus;
        }
        #endregion
       
        #region Get Batches(Transactions) For A Batch Owner Number
        public DataSet GetTransGivenBatchNo()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BatchSelectAllAsc") as SqlCommand;
            db.AddInParameter(oCommand, "BatchNo", SqlDbType.VarChar, strBatchNo.Trim());
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BatchDelete") as SqlCommand;
            db.AddInParameter(oCommand, "BatchOwner", SqlDbType.VarChar, strBatchNo.Trim());
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;


            return blnStatus;
        }
        #endregion

        #region Delete Return Command
        public SqlCommand DeleteCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BatchDelete") as SqlCommand;
            db.AddInParameter(oCommand, "BatchOwner", SqlDbType.VarChar, strBatchNo.Trim());
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Get Batch Date
        public DateTime GetBatchDate()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BatchSelectBatchDate") as SqlCommand;
            db.AddInParameter(oCommand, "BatchNo", SqlDbType.VarChar, strBatchNo.Trim());
            var varBatchDate = db.ExecuteScalar(oCommand);
            return varBatchDate != null && varBatchDate.ToString().Trim() != "" ? DateTime.ParseExact(varBatchDate.ToString().Trim().Substring(0,10),"dd/MM/yyyy",format) : DateTime.MinValue;
        }
        #endregion

        #region Change Batch Date
        public void ChangeBatchDate(string strNewDate)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BatchChangeBatchDate") as SqlCommand;
            db.AddInParameter(oCommand, "BatchOwner", SqlDbType.VarChar, strBatchNo.Trim());
            db.AddInParameter(oCommand, "NewDate", SqlDbType.DateTime, DateTime.ParseExact(strNewDate, "dd/MM/yyyy", format));
            db.ExecuteNonQuery(oCommand);
        }
        #endregion
    }
}
