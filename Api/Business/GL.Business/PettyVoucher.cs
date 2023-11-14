using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class PettyVoucher
    {
        #region Declarations
        private string strTransNo, strReceivedBy,strReceivedByEmployee, strDescription, strApprovedBy;
        private string strExpenseAcct,strPettyAcct, strAmtWord;
        private decimal decAmount;
        private DateTime datEffDate;
        private string strReceivedType, strReceivedByCustomer, strReceivedByVendor, strPettyCashStatus;
        private bool blnPosted, blnReversed, blnPostedRequest, blnReversedRequest;
        private string strBranch;
        private string strSaveType;
        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public string ReceivedBy
        {
            set { strReceivedBy = value; }
            get { return strReceivedBy; }
        }
        public string ReceivedByEmployee
        {
            set { strReceivedByEmployee = value; }
            get { return strReceivedByEmployee; }
        }
        public string Description
        {
            set { strDescription = value; }
            get { return strDescription; }
        }
        public string ApprovedBy
        {
            set { strApprovedBy = value; }
            get { return strApprovedBy; }
        }
        public string PettyAcct
        {
            set { strPettyAcct = value; }
            get { return strPettyAcct; }
        }
        public string ExpenseAcct
        {
            set { strExpenseAcct = value; }
            get { return strExpenseAcct; }
        }
        public string AmtWord
        {
            set { strAmtWord = value; }
            get { return strAmtWord; }
        }
        public decimal Amount
        {
            set { decAmount = value; }
            get { return decAmount; }
        }
        public DateTime EffDate
        {
            set { datEffDate = value; }
            get { return datEffDate; }
        }
        public bool Posted
        {
            set { blnPosted = value; }
            get { return blnPosted; }
        }
        public bool Reversed
        {
            set { blnReversed = value; }
            get { return blnReversed; }
        }
        public bool PostedRequest
        {
            set { blnPostedRequest = value; }
            get { return blnPostedRequest; }
        }
        public bool ReversedRequest
        {
            set { blnReversedRequest = value; }
            get { return blnReversedRequest; }
        }
        public string ReceivedType
        {
            set { strReceivedType = value; }
            get { return strReceivedType; }
        }
        public string ReceivedByCustomer
        {
            set { strReceivedByCustomer = value; }
            get { return strReceivedByCustomer; }
        }
        public string ReceivedByVendor
        {
            set { strReceivedByVendor = value; }
            get { return strReceivedByVendor; }
        }
        public string PettyCashStatus
        {
            set { strPettyCashStatus = value; }
            get { return strPettyCashStatus; }
        }


        public string Branch
        {
            set { strBranch = value; }
            get { return strBranch; }
        }
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
            
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strSaveType.Trim() == "ADDS")
            {
                dbCommand = db.GetStoredProcCommand("PettyVoucherAddNew") as SqlCommand;
            }
            else if (strSaveType.Trim() == "EDIT")
            {
                dbCommand = db.GetStoredProcCommand("PettyVoucherEdit") as SqlCommand;
            }

            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "ReceivedBy", SqlDbType.VarChar, strReceivedBy.Trim());
            db.AddInParameter(dbCommand, "ReceivedByEmployee", SqlDbType.VarChar, strReceivedByEmployee.Trim());
            db.AddInParameter(dbCommand, "ReceivedType", SqlDbType.VarChar, strReceivedType.Trim());
            db.AddInParameter(dbCommand, "ReceivedByCustomer", SqlDbType.VarChar, strReceivedByCustomer.Trim());
            db.AddInParameter(dbCommand, "ReceivedByVendor", SqlDbType.VarChar, strReceivedByVendor.Trim());
            db.AddInParameter(dbCommand, "Description", SqlDbType.VarChar, strDescription.Trim());
            db.AddInParameter(dbCommand, "ApprovedBy", SqlDbType.VarChar, strApprovedBy.Trim());
            db.AddInParameter(dbCommand, "ExpenseAcct", SqlDbType.VarChar, strExpenseAcct.Trim());
            db.AddInParameter(dbCommand, "PettyAcct", SqlDbType.VarChar,strPettyAcct.Trim());
            db.AddInParameter(dbCommand, "PettyCashStatus", SqlDbType.VarChar, strPettyCashStatus.Trim());
            db.AddInParameter(dbCommand, "Posted", SqlDbType.Bit, blnPosted);
            db.AddInParameter(dbCommand, "Reversed", SqlDbType.Bit, blnReversed);
            db.AddInParameter(dbCommand, "PostedRequest", SqlDbType.Bit, blnPostedRequest);
            db.AddInParameter(dbCommand, "ReversedRequest", SqlDbType.Bit, blnReversedRequest);
            db.AddInParameter(dbCommand, "AmtWord", SqlDbType.VarChar, strAmtWord.Trim());
            db.AddInParameter(dbCommand, "Amount", SqlDbType.VarChar, decAmount.ToString());
            db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime, datEffDate);
            db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.AddInParameter(dbCommand, "Branch", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());
            db.ExecuteNonQuery(dbCommand);
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
                oCommand = db.GetStoredProcCommand("PettyVoucherChkTransNoExistUnPosted") as SqlCommand;
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

        #region Get
        public bool GetPettyVoucher(DataGeneral.PostStatus ePostStatus)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (ePostStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("PettyVoucherSelectUnPosted") as SqlCommand;
            }
            else if (ePostStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PettyVoucherSelectPosted") as SqlCommand;
            }
            else if (ePostStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("PettyVoucherSelectReversed") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                strReceivedBy = thisRow[0]["ReceivedBy"].ToString();
                strReceivedByEmployee = thisRow[0]["ReceivedByEmployee"].ToString();
                strReceivedType = thisRow[0]["ReceivedType"].ToString();
                strReceivedByCustomer = thisRow[0]["ReceivedByCustomer"].ToString();
                strReceivedByVendor = thisRow[0]["ReceivedByVendor"].ToString();
                strDescription = thisRow[0]["Description"].ToString();
                strApprovedBy = thisRow[0]["ApprovedBy"].ToString();
                strExpenseAcct = thisRow[0]["ExpenseAcct"].ToString();
                strPettyAcct = thisRow[0]["PettyAcct"].ToString();
                strAmtWord = thisRow[0]["AmtWord"].ToString();
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString());
                if (thisRow[0]["EffDate"].ToString() == "" || thisRow[0]["EffDate"].ToString() == null)
                {
                    datEffDate = DateTime.MinValue;
                }
                else
                {
                    datEffDate = DateTime.ParseExact(thisRow[0]["EffDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                }
                strBranch = thisRow[0]["Branch"].ToString();
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
                dbCommand = db.GetStoredProcCommand("PettyVoucherSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PettyVoucherSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("PettyVoucherSelectAllReversed") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Request
        public bool GetPettyVoucherRequest(DataGeneral.PostStatus ePostStatus)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (ePostStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("PettyVoucherRequestSelectUnPosted") as SqlCommand;
            }
            else if (ePostStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PettyVoucherRequestSelectPosted") as SqlCommand;
            }
            else if (ePostStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("PettyVoucherRequestSelectReversed") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                strReceivedBy = thisRow[0]["ReceivedBy"].ToString();
                strReceivedByEmployee = thisRow[0]["ReceivedByEmployee"].ToString();
                strReceivedType = thisRow[0]["ReceivedType"].ToString();
                strReceivedByCustomer = thisRow[0]["ReceivedByCustomer"].ToString();
                strReceivedByVendor = thisRow[0]["ReceivedByVendor"].ToString();
                strDescription = thisRow[0]["Description"].ToString();
                strApprovedBy = thisRow[0]["ApprovedBy"].ToString();
                strExpenseAcct = thisRow[0]["ExpenseAcct"].ToString();
                strPettyAcct = thisRow[0]["PettyAcct"].ToString();
                strAmtWord = thisRow[0]["AmtWord"].ToString();
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString());
                if (thisRow[0]["EffDate"].ToString() == "" || thisRow[0]["EffDate"].ToString() == null)
                {
                    datEffDate = DateTime.MinValue;
                }
                else
                {
                    datEffDate = DateTime.ParseExact(thisRow[0]["EffDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                }
                strBranch = thisRow[0]["Branch"].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get Request All 
        public DataSet GetRequestAll(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("PettyVoucherRequestSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PettyVoucherRequestSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("PettyVoucherRequestSelectAllReversed") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Latest Request Transaction
        public DataSet GetLatestRequestTransaction(DataGeneral.PostStatus TransStatus)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("PettyVoucherRequestSelectLatestUnPosted") as SqlCommand;
            }

            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Latest Disbursement Transaction
        public DataSet GetLatestDisbursementTransaction(DataGeneral.PostStatus TransStatus)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("PettyVoucherDisbursementSelectLatestUnPosted") as SqlCommand;
            }

            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public DataGeneral.SaveStatus Delete()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PettyVoucherDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Update Post Request
        public void UpDatePostRequest()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PettyVoucherRequestUpdatePost") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar,strTransNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar,GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        

        #region Update Only Reversal and Return A Command
        public SqlCommand UpDateRevCommand()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PettyVoucherUpdateRev") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, blnReversed);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Update Only Reversal Request
        public void UpDateRevRequest()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PettyVoucherRequestUpdateRev") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "ReversedRequest", SqlDbType.Bit, blnReversedRequest);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        
    }
}
