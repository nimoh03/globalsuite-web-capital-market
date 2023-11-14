using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class AcctPayable
    {
        #region Declarations
        private string strTransNo;
        private string strVendorInvoiceNo;
        private string strAmtword, strUserId;
        private Int64 intVendor;
        private string strPostSource, strBranch,strExpenseAcct,strPayableAcct;
        private decimal decAmount,decVATAmount;
        private DateTime datReceivedDate;
        private bool blnPosted,blnReversed, blnCompletePay;
        private string strSaveType;
        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
       
        public DateTime ReceivedDate
        {
            set { datReceivedDate = value; }
            get { return datReceivedDate; }
        }

        public decimal Amount
        {
            set { decAmount = value; }
            get { return decAmount; }
        }
        public decimal VATAmount
        {
            set { decVATAmount = value; }
            get { return decVATAmount; }
        }
        
        public string VendorInvoiceNo
        {
            set { strVendorInvoiceNo = value; }
            get { return strVendorInvoiceNo; }
        }
       
        public Int64 Vendor
        {
            set { intVendor = value; }
            get { return intVendor; }
        }
        public string Amtword
        {
            set { strAmtword = value; }
            get { return strAmtword; }
        }
       
        public bool Posted
        {
            set { blnPosted = value; }
            get { return blnPosted; }
        }
        
        public string UserId
        {
            set { strUserId = value; }
            get { return strUserId; }
        }
        public bool Reversed
        {
            set { blnReversed = value; }
            get { return blnReversed; }
        }
        public bool CompletePay
        {
            set { blnCompletePay = value; }
            get { return blnCompletePay; }
        }
        
        public string PostSource
        {
            set { strPostSource = value; }
            get { return strPostSource; }
        }
        public string Branch
        {
            set { strBranch = value; }
            get { return strBranch; }
        }
        public string ExpenseAcct
        {
            set { strExpenseAcct = value; }
            get { return strExpenseAcct; }
        }
        
        public string PayableAcct
        {
            set { strPayableAcct = value; }
            get { return strPayableAcct; }
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
            if (ChkNameExist())
            {
                if (strSaveType == "ADDS")
                {
                    enSaveStatus = DataGeneral.SaveStatus.NameExistAdd;
                }
                else if (strSaveType == "EDIT")
                {
                    enSaveStatus = DataGeneral.SaveStatus.NameExistEdit;
                }
                return enSaveStatus;
            }
            
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    if (strSaveType == "ADDS")
                    {
                        oCommand = db.GetStoredProcCommand("AcctPayableAddNew") as SqlCommand;

                    }
                    else if (strSaveType == "EDIT")
                    {
                        oCommand = db.GetStoredProcCommand("AcctPayableEdit") as SqlCommand;

                    }
                    db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
                    db.AddInParameter(oCommand, "ExpenseAcct", SqlDbType.VarChar, strExpenseAcct.Trim());
                    db.AddInParameter(oCommand, "ReceivedDate", SqlDbType.DateTime, datReceivedDate);
                    db.AddInParameter(oCommand, "Amount", SqlDbType.Money, decAmount);
                    db.AddInParameter(oCommand, "VendorInvoiceNo", SqlDbType.VarChar, strVendorInvoiceNo.Trim());
                    
                    db.AddInParameter(oCommand, "Vendor", SqlDbType.BigInt, intVendor);
                    db.AddInParameter(oCommand, "Amtword", SqlDbType.VarChar, strAmtword.Trim());
                    
                    db.AddInParameter(oCommand, "Posted", SqlDbType.Bit, blnPosted);
                    db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId.Trim());
                    db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, blnReversed);
                    
                    db.AddInParameter(oCommand, "PayableAcct", SqlDbType.VarChar, strPayableAcct.Trim());
                    db.AddInParameter(oCommand, "PostSource", SqlDbType.VarChar, strPostSource.Trim());
                    db.AddInParameter(oCommand, "CompletePay", SqlDbType.Bit, blnCompletePay);
                    db.AddInParameter(oCommand, "Branch", SqlDbType.VarChar, strBranch.Trim());
                    db.AddInParameter(oCommand, "VATAmount", SqlDbType.Money, decVATAmount);
                    db.ExecuteNonQuery(oCommand,transaction);

                    if (strSaveType == "ADDS")
                    {
                        AcctPayableItem oAcctPayableItem = new AcctPayableItem();
                        oAcctPayableItem.AcctPayable = int.Parse(db.GetParameterValue(oCommand, "TransNo").ToString());
                        foreach (DataRow oconRow in oAcctPayableItem.GetTempAllByAcctPayable().Tables[0].Rows)
                        {
                            oAcctPayableItem.Descrip = oconRow[1].ToString();
                            oAcctPayableItem.Amount = decimal.Parse(oconRow[2].ToString());
                            SqlCommand oAddContact = oAcctPayableItem.AddCommand();
                            db.ExecuteNonQuery(oAddContact, transaction);

                        }
                        SqlCommand oDelContactTemp = oAcctPayableItem.DeleteTempByAcctPayableCommand();
                        db.ExecuteNonQuery(oDelContactTemp, transaction);
                    }
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

        #region Save And Return Command
        public SqlCommand SaveCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            
            if (strSaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("AcctPayableAddNew") as SqlCommand;

            }
            else if (strSaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("AcctPayableEdit") as SqlCommand;

            }
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "ExpenseAcct", SqlDbType.VarChar, strExpenseAcct.Trim());
            db.AddInParameter(oCommand, "ReceivedDate", SqlDbType.DateTime, datReceivedDate);
            db.AddInParameter(oCommand, "Amount", SqlDbType.Money, decAmount);
            db.AddInParameter(oCommand, "VendorInvoiceNo", SqlDbType.VarChar, strVendorInvoiceNo.Trim());

            db.AddInParameter(oCommand, "Vendor", SqlDbType.BigInt, intVendor);
            db.AddInParameter(oCommand, "Amtword", SqlDbType.VarChar, strAmtword.Trim());

            db.AddInParameter(oCommand, "Posted", SqlDbType.Bit, blnPosted);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId.Trim());
            db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, blnReversed);

            db.AddInParameter(oCommand, "PayableAcct", SqlDbType.VarChar, strPayableAcct.Trim());
            db.AddInParameter(oCommand, "PostSource", SqlDbType.VarChar, strPostSource.Trim());
            db.AddInParameter(oCommand, "CompletePay", SqlDbType.Bit, blnCompletePay);
            db.AddInParameter(oCommand, "Branch", SqlDbType.VarChar, strBranch.Trim());
            db.AddInParameter(oCommand, "VATAmount", SqlDbType.Money, decVATAmount);

            return oCommand;

        }
        #endregion

        #region Check Trans No Exist
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("AcctPayableChkTransNoExistUnPosted") as SqlCommand;
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


        #region Check Name Exist
        public bool ChkNameExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AcctPayableChkNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.Char, strTransNo.Trim());
            db.AddInParameter(oCommand, "Vendor", SqlDbType.BigInt, intVendor);
            db.AddInParameter(oCommand, "VendorInvoiceNo", SqlDbType.VarChar, strVendorInvoiceNo.Trim());
            db.AddInParameter(oCommand, "SaveType", SqlDbType.VarChar, strSaveType.Trim());
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
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
                dbCommand = db.GetStoredProcCommand("AcctPayableSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayableSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayableSelectAllReversed") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Double
        public DataSet GetAllDouble(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayableDoubleSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayableDoubleSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayableDoubleSelectAllReversed") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Single
        public DataSet GetAllSingle(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayableSingleSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayableSingleSelectAllPosted") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get
        public bool GetAcctPayable(DataGeneral.PostStatus TransStatus)
        {
            bool blnStatus = false;
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayableSelectUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayableSelectPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayableSelectReversed") as SqlCommand;
            } 
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strExpenseAcct = thisRow[0]["ExpenseAcct"].ToString();
                datReceivedDate = DateTime.ParseExact(thisRow[0]["ReceivedDate"].ToString().Trim().Substring(0,10), "dd/MM/yyyy", format);
                decAmount= decimal.Parse(thisRow[0]["Amount"].ToString());
                strVendorInvoiceNo= thisRow[0]["VendorInvoiceNo"].ToString();

                intVendor = Convert.ToInt64(thisRow[0]["Vendor"]);
                strAmtword= thisRow[0]["Amtword"].ToString();
               
                blnPosted= bool.Parse(thisRow[0]["Posted"].ToString());
                strUserId= thisRow[0]["UserId"].ToString();
                blnReversed= bool.Parse(thisRow[0]["Reversed"].ToString());
                
                strPostSource= thisRow[0]["PostSource"].ToString();
                
                strPayableAcct = thisRow[0]["PayableAcct"].ToString();
                blnCompletePay = bool.Parse(thisRow[0]["CompletePay"].ToString());
                strBranch= thisRow[0]["Branch"].ToString();
                decVATAmount = decimal.Parse(thisRow[0]["VATAmount"].ToString());

                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get Double
        public bool GetAcctPayableDouble(DataGeneral.PostStatus TransStatus)
        {
            bool blnStatus = false;
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayableDoubleSelectUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayableDoubleSelectPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayableDoubleSelectReversed") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strExpenseAcct = thisRow[0]["ExpenseAcct"].ToString();
                datReceivedDate = DateTime.ParseExact(thisRow[0]["ReceivedDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString());
                strVendorInvoiceNo = thisRow[0]["VendorInvoiceNo"].ToString();

                intVendor = Convert.ToInt64(thisRow[0]["Vendor"]);
                strAmtword = thisRow[0]["Amtword"].ToString();
                
                blnPosted = bool.Parse(thisRow[0]["Posted"].ToString());
                strUserId = thisRow[0]["UserId"].ToString();
                blnReversed = bool.Parse(thisRow[0]["Reversed"].ToString());
               
                strPostSource = thisRow[0]["PostSource"].ToString();
                
                strPayableAcct = thisRow[0]["PayableAcct"].ToString();
                blnCompletePay = bool.Parse(thisRow[0]["CompletePay"].ToString());
                
                strBranch = thisRow[0]["Branch"].ToString();
                decVATAmount = decimal.Parse(thisRow[0]["VATAmount"].ToString());
                

                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get Single
        public bool GetAcctPayableSingle(DataGeneral.PostStatus TransStatus)
        {
            bool blnStatus = false;
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayableSingleSelectUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayableSingleSelectPosted") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strExpenseAcct = thisRow[0]["ExpenseAcct"].ToString();
                datReceivedDate = DateTime.ParseExact(thisRow[0]["ReceivedDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString());
                strVendorInvoiceNo = thisRow[0]["VendorInvoiceNo"].ToString();

                intVendor = Convert.ToInt64(thisRow[0]["Vendor"]);
                strAmtword = thisRow[0]["Amtword"].ToString();
                
                blnPosted = bool.Parse(thisRow[0]["Posted"].ToString());
                strUserId = thisRow[0]["UserId"].ToString();
                blnReversed = bool.Parse(thisRow[0]["Reversed"].ToString());
                strPostSource = thisRow[0]["PostSource"].ToString();
                strPayableAcct = thisRow[0]["PayableAcct"].ToString();
                blnCompletePay = bool.Parse(thisRow[0]["CompletePay"].ToString());
                strBranch = thisRow[0]["Branch"].ToString();
                decVATAmount = decimal.Parse(thisRow[0]["VATAmount"].ToString());
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get Post Source
        public string GetPostSource()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AcctPayableSelectPostSource") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo);
            return db.ExecuteScalar(dbCommand).ToString();
        }
        #endregion

        #region Edit Amount Only And Return Command
        public SqlCommand EditAmountCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AcctReceivableEditAmount") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "Amount", SqlDbType.Money, decAmount);
            db.AddInParameter(dbCommand, "Amtword", SqlDbType.VarChar, strAmtword.Trim());
            return dbCommand;
        }
        #endregion

        #region Update Only Reversal and Return A Command
        public SqlCommand UpDateRevCommand()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AcctPayableUpdateRev") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, blnReversed);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId.Trim().ToUpper());
            return oCommand;
        }
        #endregion

        #region Delete
        public DataGeneral.SaveStatus Delete()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AcctPayableDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Edit Complete Pay Only And Return Command
        public SqlCommand EditCompletePayCommand()
        {
            AcctPayablePayment oAcctPayablePayment = new AcctPayablePayment();
            oAcctPayablePayment.AcctPayable = int.Parse(strTransNo);
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AcctPayableEditCompletePay") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "TotalPostedPayment", SqlDbType.Money, oAcctPayablePayment.GetTotalAcctPayablePayment(DataGeneral.PostStatus.All));
            return dbCommand;
        }
        #endregion

        
    }
}
