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
    public class AcctReceivablePayment
    {
        #region Declarations
        private string strTransNo;
        private int intAcctReceivable;
        private string strChqueNo;
        private string strAmtword, strUserId;
        private string strRecNo, strBranch, strCashAcct, strReceivableAcct;
        private decimal decAmountPaid;
        private DateTime datChqDate, datPaymentDate;
        private bool blnPosted, blnReversed;
        private string strSaveType;
        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public int AcctReceivable
        {
            set { intAcctReceivable = value; }
            get { return intAcctReceivable; }
        }

        public decimal AmountPaid
        {
            set { decAmountPaid = value; }
            get { return decAmountPaid; }
        }
        public DateTime ChqDate
        {
            set { datChqDate = value; }
            get { return datChqDate; }
        }
        public string ChqueNo
        {
            set { strChqueNo = value; }
            get { return strChqueNo; }
        }
        
        public string Amtword
        {
            set { strAmtword = value; }
            get { return strAmtword; }
        }
        public DateTime PaymentDate
        {
            set { datPaymentDate = value; }
            get { return datPaymentDate; }
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
        
        public string CashAcct
        {
            set { strCashAcct = value; }
            get { return strCashAcct; }
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
                        oCommand = db.GetStoredProcCommand("AcctReceivablePaymentAddNew") as SqlCommand;

                    }
                    else if (strSaveType == "EDIT")
                    {
                        oCommand = db.GetStoredProcCommand("AcctReceivablePaymentEdit") as SqlCommand;

                    }
                    db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
                    db.AddInParameter(oCommand, "AcctReceivable", SqlDbType.BigInt, intAcctReceivable);
                    if (datChqDate != DateTime.MinValue)
                    {
                        db.AddInParameter(oCommand, "ChqDate", SqlDbType.DateTime, datChqDate);
                    }
                    else
                    {
                        db.AddInParameter(oCommand, "ChqDate", SqlDbType.DateTime, SqlDateTime.Null);
                    }
                    db.AddInParameter(oCommand, "ChqueNo", SqlDbType.VarChar, strChqueNo.Trim());
                    db.AddInParameter(oCommand, "Amtword", SqlDbType.VarChar, strAmtword.Trim());
                    if (datPaymentDate != DateTime.MinValue)
                    {
                        db.AddInParameter(oCommand, "PaymentDate", SqlDbType.DateTime, datPaymentDate);
                    }
                    else
                    {
                        db.AddInParameter(oCommand, "PaymentDate", SqlDbType.DateTime, SqlDateTime.Null);
                    }
                    db.AddInParameter(oCommand, "Posted", SqlDbType.Bit, blnPosted);
                    db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId.Trim());
                    db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, blnReversed);
                    db.AddInParameter(oCommand, "RecNo", SqlDbType.VarChar, strRecNo.Trim());
                    db.AddInParameter(oCommand, "ReceivableAcct", SqlDbType.VarChar, strReceivableAcct.Trim());
                    db.AddInParameter(oCommand, "CashAcct", SqlDbType.VarChar, strCashAcct.Trim());
                    db.AddInParameter(oCommand, "Branch", SqlDbType.VarChar, strBranch.Trim());
                    db.AddInParameter(oCommand, "AmountPaid", SqlDbType.Money, decAmountPaid);

                    db.ExecuteNonQuery(oCommand, transaction);
                    
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

        #region Save Double
        public DataGeneral.SaveStatus SaveDouble(AcctReceivable oAcctReceivable)
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            if (!ChkTransNoExist())
            {
                enSaveStatus = DataGeneral.SaveStatus.NotExist;
                return enSaveStatus;
            }

            if (!oAcctReceivable.ChkTransNoExist())
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
                    SqlCommand dbCommandAcctReceivable = oAcctReceivable.SaveCommand();
                    db.ExecuteNonQuery(dbCommandAcctReceivable, transaction);

                    if (strSaveType == "ADDS")
                    {
                        oCommand = db.GetStoredProcCommand("AcctReceivablePaymentAddNew") as SqlCommand;
                        db.AddInParameter(oCommand, "AcctReceivable", SqlDbType.BigInt, int.Parse(db.GetParameterValue(dbCommandAcctReceivable, "TransNo").ToString()));

                    }
                    else if (strSaveType == "EDIT")
                    {
                       oCommand = db.GetStoredProcCommand("AcctReceivablePaymentEdit") as SqlCommand;
                       db.AddInParameter(oCommand, "AcctReceivable", SqlDbType.BigInt, intAcctReceivable);

                    }
                    db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
                    if (datChqDate != DateTime.MinValue)
                    {
                        db.AddInParameter(oCommand, "ChqDate", SqlDbType.DateTime, datChqDate);
                    }
                    else
                    {
                        db.AddInParameter(oCommand, "ChqDate", SqlDbType.DateTime, SqlDateTime.Null);
                    }
                    db.AddInParameter(oCommand, "ChqueNo", SqlDbType.VarChar, strChqueNo.Trim());
                    db.AddInParameter(oCommand, "Amtword", SqlDbType.VarChar, strAmtword.Trim());
                    if (datPaymentDate != DateTime.MinValue)
                    {
                        db.AddInParameter(oCommand, "PaymentDate", SqlDbType.DateTime, datPaymentDate);
                    }
                    else
                    {
                        db.AddInParameter(oCommand, "PaymentDate", SqlDbType.DateTime, SqlDateTime.Null);
                    }
                    db.AddInParameter(oCommand, "Posted", SqlDbType.Bit, blnPosted);
                    db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId.Trim());
                    db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, blnReversed);
                    db.AddInParameter(oCommand, "RecNo", SqlDbType.VarChar, strRecNo.Trim());
                    db.AddInParameter(oCommand, "ReceivableAcct", SqlDbType.VarChar, strReceivableAcct.Trim());
                    db.AddInParameter(oCommand, "CashAcct", SqlDbType.VarChar, strCashAcct.Trim());
                    db.AddInParameter(oCommand, "Branch", SqlDbType.VarChar, strBranch.Trim());
                    db.AddInParameter(oCommand, "AmountPaid", SqlDbType.Money, decAmountPaid);
                    db.ExecuteNonQuery(oCommand, transaction);

                    if (strSaveType == "ADDS")
                    {
                        AcctReceivableItem oAcctReceivableItem = new AcctReceivableItem();
                        oAcctReceivableItem.AcctReceivable = int.Parse(db.GetParameterValue(dbCommandAcctReceivable, "TransNo").ToString());
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

        #region Check Trans No Exist
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("AcctReceivablePaymentChkTransNoExistUnPosted") as SqlCommand;
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

        #region Check Trans No Exist Payment
        public bool ChkTransNoExistPayment()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("AcctReceivablePaymentChkTransNoExistPosted") as SqlCommand;
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

        

        

        #region Get All Payment
        public DataSet GetAllPayment(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivablePaymentSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivablePaymentSelectAllPosted") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "AcctReceivable", SqlDbType.BigInt, intAcctReceivable);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion



        #region Get All Payment Only
        public DataSet GetAllPaymentOnly(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivablePaymentPaymentOnlySelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivablePaymentPaymentOnlySelectAllPosted") as SqlCommand;
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
                dbCommand = db.GetStoredProcCommand("AcctReceivablePaymentSelectUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivablePaymentSelectPosted") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                if (thisRow[0]["ChqDate"] != null && thisRow[0]["ChqDate"].ToString() != "")
                {
                    datChqDate = DateTime.ParseExact(thisRow[0]["ChqDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                }
                else
                {
                    datChqDate = DateTime.MinValue;
                }
                strChqueNo = thisRow[0]["ChqueNo"].ToString();
                strAmtword = thisRow[0]["Amtword"].ToString();
                if (thisRow[0]["PaymentDate"] != null && thisRow[0]["PaymentDate"].ToString() != "")
                {
                    datPaymentDate = DateTime.ParseExact(thisRow[0]["PaymentDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                }
                else
                {
                    datPaymentDate = DateTime.MinValue;
                }
                blnPosted = bool.Parse(thisRow[0]["Posted"].ToString());
                strUserId = thisRow[0]["UserId"].ToString();
                blnReversed = bool.Parse(thisRow[0]["Reversed"].ToString());
                strRecNo = thisRow[0]["RecNo"].ToString();
                strCashAcct = thisRow[0]["CashAcct"].ToString();
                strReceivableAcct = thisRow[0]["ReceivableAcct"].ToString();
                strBranch = thisRow[0]["Branch"].ToString();
                decAmountPaid = decimal.Parse(thisRow[0]["AmountPaid"].ToString());
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get Payment
        public bool GetAcctReceivablePayment(DataGeneral.PostStatus TransStatus)
        {
            bool blnStatus = false;
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivablePaymentSelectUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivablePaymentSelectPosted") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                if (thisRow[0]["ChqDate"] != null && thisRow[0]["ChqDate"].ToString() != "")
                {
                    datChqDate = DateTime.ParseExact(thisRow[0]["ChqDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                }
                else
                {
                    datChqDate = DateTime.MinValue;
                }
                strChqueNo = thisRow[0]["ChqueNo"].ToString();
                strAmtword = thisRow[0]["Amtword"].ToString();
                if (thisRow[0]["PaymentDate"] != null && thisRow[0]["PaymentDate"].ToString() != "")
                {
                    datPaymentDate = DateTime.ParseExact(thisRow[0]["PaymentDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                }
                else
                {
                    datPaymentDate = DateTime.MinValue;
                }
                blnPosted = bool.Parse(thisRow[0]["Posted"].ToString());
                strUserId = thisRow[0]["UserId"].ToString();
                blnReversed = bool.Parse(thisRow[0]["Reversed"].ToString());
                strRecNo = thisRow[0]["PVNo"].ToString();
                strCashAcct = thisRow[0]["CashAcct"].ToString();
                strReceivableAcct = thisRow[0]["ReceivableAcct"].ToString();
                strBranch = thisRow[0]["Branch"].ToString();
                decAmountPaid = decimal.Parse(thisRow[0]["AmountPaid"].ToString());

                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get Payment Only
        public bool GetAcctReceivablePaymentOnly(DataGeneral.PostStatus TransStatus)
        {
            bool blnStatus = false;
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivablePaymentPaymentOnlySelectUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivablePaymentPaymentOnlySelectPosted") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                if (thisRow[0]["ChqDate"] != null && thisRow[0]["ChqDate"].ToString() != "")
                {
                    datChqDate = DateTime.ParseExact(thisRow[0]["ChqDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                }
                else
                {
                    datChqDate = DateTime.MinValue;
                }
                strChqueNo = thisRow[0]["ChqueNo"].ToString();
                strAmtword = thisRow[0]["Amtword"].ToString();
                if (thisRow[0]["PaymentDate"] != null && thisRow[0]["PaymentDate"].ToString() != "")
                {
                    datPaymentDate = DateTime.ParseExact(thisRow[0]["PaymentDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                }
                else
                {
                    datPaymentDate = DateTime.MinValue;
                }
                blnPosted = bool.Parse(thisRow[0]["Posted"].ToString());
                strUserId = thisRow[0]["UserId"].ToString();
                blnReversed = bool.Parse(thisRow[0]["Reversed"].ToString());
                strRecNo = thisRow[0]["RecNo"].ToString();
                strCashAcct = thisRow[0]["CashAcct"].ToString();
                strReceivableAcct = thisRow[0]["ReceivableAcct"].ToString();
                strBranch = thisRow[0]["Branch"].ToString();
                decAmountPaid = decimal.Parse(thisRow[0]["AmountPaid"].ToString());

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
            SqlCommand dbCommand = db.GetStoredProcCommand("AcctReceivablePaymentSelectPostSource") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo);
            return db.ExecuteScalar(dbCommand).ToString();
        }
        #endregion

        

        #region Update Only Reversal and Return A Command
        public SqlCommand UpDateRevCommand()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AcctReceivablePaymentUpdateRev") as SqlCommand;
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
            SqlCommand oCommand = db.GetStoredProcCommand("AcctReceivablePaymentDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Delete Payment
        public DataGeneral.SaveStatus DeletePayment()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AcctReceivablePaymentDeletePayment") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Get Last AcctReceivable Payment
        public string GetLastAcctReceivablePayment(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivablePaymentSelectLastPaymentUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivablePaymentSelectLastPaymentPosted") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "AcctReceivable", SqlDbType.BigInt, intAcctReceivable);
            if (db.ExecuteScalar(dbCommand) != null && db.ExecuteScalar(dbCommand).ToString() != "")
            {
                return db.ExecuteScalar(dbCommand).ToString();
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region Get Total AcctPayable Payment
        public decimal GetTotalAcctReceivablePayment(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivablePaymentSelectTotalPaymentUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivablePaymentSelectTotalPaymentPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.All)
            {
                dbCommand = db.GetStoredProcCommand("AcctReceivablePaymentSelectTotalPaymentNoRev") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "AcctReceivable", SqlDbType.BigInt, intAcctReceivable);
            if (db.ExecuteScalar(dbCommand) != null && db.ExecuteScalar(dbCommand).ToString() != "")
            {
                return decimal.Parse(db.ExecuteScalar(dbCommand).ToString());
            }
            else
            {
                return 0;
            }
        }
        #endregion
    }
}
