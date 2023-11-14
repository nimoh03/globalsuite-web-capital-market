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
    public class AcctPayablePayment
    {
        #region Declarations
        private string strTransNo;
        private int intAcctPayable;
        private string strChqueNo;
        private string strAmtword, strUserId;
        private string strPVNo, strBranch, strCashAcct, strPayableAcct;
        private decimal decAmountPaid;
        private DateTime datChqDate,datPaymentDate;
        private bool blnPosted, blnReversed;
        private string strSaveType;
        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public int AcctPayable
        {
            set { intAcctPayable = value; }
            get { return intAcctPayable; }
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
       
        public string PVNo
        {
            set { strPVNo = value; }
            get { return strPVNo; }
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
                        oCommand = db.GetStoredProcCommand("AcctPayablePaymentAddNew") as SqlCommand;

                    }
                    else if (strSaveType == "EDIT")
                    {
                        oCommand = db.GetStoredProcCommand("AcctPayablePaymentEdit") as SqlCommand;

                    }
                    db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
                    db.AddInParameter(oCommand, "AcctPayable", SqlDbType.BigInt,intAcctPayable);
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
                    db.AddInParameter(oCommand, "PVNo", SqlDbType.VarChar, strPVNo.Trim());
                    db.AddInParameter(oCommand, "PayableAcct", SqlDbType.VarChar, strPayableAcct.Trim());
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
        public DataGeneral.SaveStatus SaveDouble(AcctPayable oAcctPayable)
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            if (!ChkTransNoExist())
            {
                enSaveStatus = DataGeneral.SaveStatus.NotExist;
                return enSaveStatus;
            }

            if (!oAcctPayable.ChkTransNoExist())
            {
                enSaveStatus = DataGeneral.SaveStatus.NotExist;
                return enSaveStatus;
            }
            if (oAcctPayable.ChkNameExist())
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
                    SqlCommand dbCommandAcctPayable = oAcctPayable.SaveCommand();
                    db.ExecuteNonQuery(dbCommandAcctPayable, transaction);

                    if (strSaveType == "ADDS")
                    {
                        oCommand = db.GetStoredProcCommand("AcctPayablePaymentAddNew") as SqlCommand;
                        db.AddInParameter(oCommand, "AcctPayable", SqlDbType.BigInt, int.Parse(db.GetParameterValue(dbCommandAcctPayable, "TransNo").ToString()));

                    }
                    else if (strSaveType == "EDIT")
                    {
                        
                        oCommand = db.GetStoredProcCommand("AcctPayablePaymentEdit") as SqlCommand;
                        db.AddInParameter(oCommand, "AcctPayable", SqlDbType.BigInt, intAcctPayable);

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
                    db.AddInParameter(oCommand, "PVNo", SqlDbType.VarChar, strPVNo.Trim());
                    db.AddInParameter(oCommand, "PayableAcct", SqlDbType.VarChar, strPayableAcct.Trim());
                    db.AddInParameter(oCommand, "CashAcct", SqlDbType.VarChar, strCashAcct.Trim());
                    db.AddInParameter(oCommand, "Branch", SqlDbType.VarChar, strBranch.Trim());
                    db.AddInParameter(oCommand, "AmountPaid", SqlDbType.Money, decAmountPaid);
                    db.ExecuteNonQuery(oCommand, transaction);


                    if (strSaveType == "ADDS")
                    {
                        AcctPayableItem oAcctPayableItem = new AcctPayableItem();
                        oAcctPayableItem.AcctPayable = int.Parse(db.GetParameterValue(dbCommandAcctPayable, "TransNo").ToString());
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

        #region Check Trans No Exist
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("AcctPayablePaymentChkTransNoExistUnPosted") as SqlCommand;
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
                SqlCommand oCommand = db.GetStoredProcCommand("AcctPayablePaymentChkTransNoExistPosted") as SqlCommand;
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
                dbCommand = db.GetStoredProcCommand("AcctPayablePaymentSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayablePaymentSelectAllPosted") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Payment
        public DataSet GetAllPayment(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayablePaymentPaymentSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayablePaymentPaymentSelectAllPosted") as SqlCommand;
            }
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
                dbCommand = db.GetStoredProcCommand("AcctPayablePaymentPaymentOnlySelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayablePaymentPaymentOnlySelectAllPosted") as SqlCommand;
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
                dbCommand = db.GetStoredProcCommand("AcctPayablePaymentSelectUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayablePaymentSelectPosted") as SqlCommand;
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
                strPVNo = thisRow[0]["PVNo"].ToString();
                strCashAcct = thisRow[0]["CashAcct"].ToString();
                strPayableAcct = thisRow[0]["PayableAcct"].ToString();
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
        public bool GetAcctPayablePayment(DataGeneral.PostStatus TransStatus)
        {
            bool blnStatus = false;
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayablePaymentSelectUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayablePaymentSelectPosted") as SqlCommand;
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
                strPVNo = thisRow[0]["PVNo"].ToString();
                strCashAcct = thisRow[0]["CashAcct"].ToString();
                strPayableAcct = thisRow[0]["PayableAcct"].ToString();
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
        public bool GetAcctPayablePaymentOnly(DataGeneral.PostStatus TransStatus)
        {
            bool blnStatus = false;
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayablePaymentOnlySelectUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayablePaymentOnlySelectPosted") as SqlCommand;
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
                strPVNo = thisRow[0]["PVNo"].ToString();
                strCashAcct = thisRow[0]["CashAcct"].ToString();
                strPayableAcct = thisRow[0]["PayableAcct"].ToString();
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
            SqlCommand dbCommand = db.GetStoredProcCommand("AcctPayablePaymentSelectPostSource") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo);
            return db.ExecuteScalar(dbCommand).ToString();
        }
        #endregion

        

        #region Update Only Reversal and Return A Command
        public SqlCommand UpDateRevCommand()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AcctPayablePaymentUpdateRev") as SqlCommand;
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
            SqlCommand oCommand = db.GetStoredProcCommand("AcctPayablePaymentDelete") as SqlCommand;
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
            SqlCommand oCommand = db.GetStoredProcCommand("AcctPayablePaymentDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Get Last AcctPayable Payment
        public string GetLastAcctPayablePayment(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayablePaymentSelectLastPaymentUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayablePaymentSelectLastPaymentPosted") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "AcctPayable", SqlDbType.BigInt, intAcctPayable);
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
        public decimal GetTotalAcctPayablePayment(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayablePaymentSelectTotalPaymentUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayablePaymentSelectTotalPaymentPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.All)
            {
                dbCommand = db.GetStoredProcCommand("AcctPayablePaymentSelectTotalPaymentNoRev") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "AcctPayable", SqlDbType.BigInt, intAcctPayable);
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
