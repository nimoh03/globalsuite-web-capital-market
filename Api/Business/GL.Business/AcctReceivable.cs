using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class AcctReceivable
    {
        #region Declarations
        private string strTransNo;
        private string strCustomer,strProduct, strAmtword, strUserId;
        private string strPostSource, strBranch, strSaleAcct, strReceivableAcct;
        private decimal decAmount, decVATAmount;
        private DateTime datIssuedDate;
        private bool blnPosted, blnReversed, blnCompletePay;
        private string strSaveType;
        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }

        public DateTime IssuedDate
        {
            set { datIssuedDate = value; }
            get { return datIssuedDate; }
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
        
        public string Customer
        {
            set { strCustomer = value; }
            get { return strCustomer; }
        }
        public string Product
        {
            set { strProduct = value; }
            get { return strProduct; }
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
        public string SaleAcct
        {
            set { strSaleAcct = value; }
            get { return strSaleAcct; }
        }
       
        public string ReceivableAcct
        {
            set { strReceivableAcct = value; }
            get { return strReceivableAcct; }
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
            SqlCommand oCommand = null;
            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    if (strSaveType == "ADDS")
                    {
                        oCommand = db.GetStoredProcCommand("AcctReceivableAddNew") as SqlCommand;

                    }
                    else if (strSaveType == "EDIT")
                    {
                        oCommand = db.GetStoredProcCommand("AcctReceivableEdit") as SqlCommand;

                    }
                    db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
                    db.AddInParameter(oCommand, "SaleAcct", SqlDbType.VarChar, strSaleAcct.Trim());
                    db.AddInParameter(oCommand, "IssuedDate", SqlDbType.DateTime, datIssuedDate);
                    db.AddInParameter(oCommand, "Amount", SqlDbType.Money, decAmount);
                    db.AddInParameter(oCommand, "Customer", SqlDbType.VarChar, strCustomer.Trim());
                    db.AddInParameter(oCommand, "Product", SqlDbType.VarChar, strProduct.Trim());
                    db.AddInParameter(oCommand, "Amtword", SqlDbType.VarChar, strAmtword.Trim());
                    db.AddInParameter(oCommand, "Posted", SqlDbType.Bit, blnPosted);
                    db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId.Trim());
                    db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, blnReversed);
                    db.AddInParameter(oCommand, "ReceivableAcct", SqlDbType.VarChar, strReceivableAcct.Trim());
                    db.AddInParameter(oCommand, "PostSource", SqlDbType.VarChar, strPostSource.Trim());
                    db.AddInParameter(oCommand, "CompletePay", SqlDbType.Bit, blnCompletePay);
                    db.AddInParameter(oCommand, "Branch", SqlDbType.VarChar, strBranch.Trim());
                    db.AddInParameter(oCommand, "VATAmount", SqlDbType.Money, decVATAmount);

                    db.ExecuteNonQuery(oCommand, transaction);
                    if (strSaveType == "ADDS")
                    {
                        AcctReceivableItem oAcctReceivableItem = new AcctReceivableItem();
                        oAcctReceivableItem.AcctReceivable = int.Parse(db.GetParameterValue(oCommand, "TransNo").ToString());
                        foreach (DataRow oconRow in oAcctReceivableItem.GetTempAllByAcctReceivable().Tables[0].Rows)
                        {
                            oAcctReceivableItem.Descrip = oconRow[1].ToString();
                            oAcctReceivableItem.Amount = decimal.Parse(oconRow[2].ToString());
                            SqlCommand oAddContact = oAcctReceivableItem.AddCommand();
                            db.ExecuteNonQuery(oAddContact, transaction);

                        }
                        SqlCommand oDelContactTemp = oAcctReceivableItem.DeleteTempByAcctReceivableCommand();
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
                oCommand = db.GetStoredProcCommand("AcctReceivableAddNew") as SqlCommand;

            }
            else if (strSaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("AcctReceivableEdit") as SqlCommand;

            }
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "SaleAcct", SqlDbType.VarChar, strSaleAcct.Trim());
            db.AddInParameter(oCommand, "IssuedDate", SqlDbType.DateTime, datIssuedDate);
            db.AddInParameter(oCommand, "Amount", SqlDbType.Money, decAmount);
            db.AddInParameter(oCommand, "Customer", SqlDbType.VarChar, strCustomer.Trim());
            db.AddInParameter(oCommand, "Product", SqlDbType.VarChar, strProduct.Trim());
            db.AddInParameter(oCommand, "Amtword", SqlDbType.VarChar, strAmtword.Trim());
            db.AddInParameter(oCommand, "Posted", SqlDbType.Bit, blnPosted);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId.Trim());
            db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, blnReversed);
            db.AddInParameter(oCommand, "ReceivableAcct", SqlDbType.VarChar, strReceivableAcct.Trim());
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
                SqlCommand oCommand = db.GetStoredProcCommand("AcctReceivableChkTransNoExistUnPosted") as SqlCommand;
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
                dbCommand = db.GetStoredProcCommand("AcctReceivableSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivableSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivableSelectAllReversed") as SqlCommand;
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
                dbCommand = db.GetStoredProcCommand("AcctReceivableDoubleSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivableDoubleSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivableDoubleSelectAllReversed") as SqlCommand;
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
                dbCommand = db.GetStoredProcCommand("AcctReceivableSingleSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivableSingleSelectAllPosted") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get
        public bool GetAcctReceivable(DataGeneral.PostStatus TransStatus)
        {
            bool blnStatus = false;
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivableSelectUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivableSelectPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivableSelectReversed") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strSaleAcct = thisRow[0]["SaleAcct"].ToString();
                datIssuedDate = DateTime.ParseExact(thisRow[0]["IssuedDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString());
                
                strCustomer = thisRow[0]["Customer"].ToString();
                strProduct = thisRow[0]["Product"].ToString();
                strAmtword = thisRow[0]["Amtword"].ToString();
               
                blnPosted = bool.Parse(thisRow[0]["Posted"].ToString());
                strUserId = thisRow[0]["UserId"].ToString();
                blnReversed = bool.Parse(thisRow[0]["Reversed"].ToString());
                strPostSource = thisRow[0]["PostSource"].ToString();
                strReceivableAcct = thisRow[0]["ReceivableAcct"].ToString();
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

        #region Get Double
        public bool GetAcctReceivableDouble(DataGeneral.PostStatus TransStatus)
        {
            bool blnStatus = false;
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivableDoubleSelectUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivableDoubleSelectPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivableDoubleSelectReversed") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strSaleAcct = thisRow[0]["SaleAcct"].ToString();
                datIssuedDate = DateTime.ParseExact(thisRow[0]["IssuedDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString());
                
                strCustomer = thisRow[0]["Customer"].ToString();
                strProduct = thisRow[0]["Product"].ToString();
                strAmtword = thisRow[0]["Amtword"].ToString();
                
                blnPosted = bool.Parse(thisRow[0]["Posted"].ToString());
                strUserId = thisRow[0]["UserId"].ToString();
                blnReversed = bool.Parse(thisRow[0]["Reversed"].ToString());
                
                strPostSource = thisRow[0]["PostSource"].ToString();
                
                strReceivableAcct = thisRow[0]["ReceivableAcct"].ToString();
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
        public bool GetAcctReceivableSingle(DataGeneral.PostStatus TransStatus)
        {
            bool blnStatus = false;
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivableSingleSelectUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivableSingleSelectPosted") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strSaleAcct = thisRow[0]["SaleAcct"].ToString();
                datIssuedDate = DateTime.ParseExact(thisRow[0]["IssuedDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString());
                
                strCustomer = thisRow[0]["Customer"].ToString();
                strProduct = thisRow[0]["Product"].ToString();
                strAmtword = thisRow[0]["Amtword"].ToString();
               
                blnPosted = bool.Parse(thisRow[0]["Posted"].ToString());
                strUserId = thisRow[0]["UserId"].ToString();
                blnReversed = bool.Parse(thisRow[0]["Reversed"].ToString());
               
                strPostSource = thisRow[0]["PostSource"].ToString();
                
                strReceivableAcct = thisRow[0]["ReceivableAcct"].ToString();
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
            SqlCommand dbCommand = db.GetStoredProcCommand("AcctReceivableSelectPostSource") as SqlCommand;
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
            SqlCommand oCommand = db.GetStoredProcCommand("AcctReceivableUpdateRev") as SqlCommand;
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
            SqlCommand oCommand = db.GetStoredProcCommand("AcctReceivableDelete") as SqlCommand;
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
            AcctReceivablePayment oAcctReceivablePayment = new AcctReceivablePayment();
            oAcctReceivablePayment.AcctReceivable = int.Parse(strTransNo);
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AcctReceivableEditCompletePay") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "TotalPostedPayment", SqlDbType.Money, oAcctReceivablePayment.GetTotalAcctReceivablePayment(DataGeneral.PostStatus.All));
            return dbCommand;
        }
        #endregion
      
    }
}
