using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using BaseUtility.Business;
using GL.Business;

namespace CapitalMarket.Business
{
    public  class BonusOffer
    {
      
        #region Declarations
        private string strTransNo, strTransNoRev;
        private DateTime datBonusOfferDate, datClosureDate, datQualificationDate, datPaymentDate;
        private int intUnit, intEvery;
        private string strStockCode, strComment;
        private string strPosted, strReversed;
        private string strSaveType;
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
        public int  Unit
        {
            set { intUnit = value; }
            get { return intUnit; }
        }
        public int  Every
        {
            set { intEvery = value; }
            get { return intEvery; }
        }
        public DateTime BonusOfferDate
        {
            set { datBonusOfferDate = value; }
            get { return datBonusOfferDate; }
        }
        public DateTime ClosureDate
        {
            set { datClosureDate = value; }
            get { return datClosureDate; }
        }
        public DateTime QualificationDate
        {
            set { datQualificationDate = value; }
            get { return datQualificationDate; }
        }
        public DateTime PaymentDate
        {
            set { datPaymentDate = value; }
            get { return datPaymentDate; }
        }
        public string StockCode
        {
            set { strStockCode = value; }
            get { return strStockCode; }
        }

        public string Comment
        {
            set { strComment = value; }
            get { return strComment; }
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
            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    SqlCommand oCommand = null;
                    if (strSaveType == "ADDS")
                    {
                        oCommand = db.GetStoredProcCommand("BonusTxnsAdd") as SqlCommand;
                        db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "BONUSOFFER");


                    }
                    else if (strSaveType == "EDIT")
                    {
                        oCommand = db.GetStoredProcCommand("BonusTxnsEdit") as SqlCommand;
                    }


                    db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
                    db.AddInParameter(oCommand, "BonusOfferDate", SqlDbType.DateTime, datBonusOfferDate);
                    db.AddInParameter(oCommand, "ClosureDate", SqlDbType.DateTime, datClosureDate);
                    db.AddInParameter(oCommand, "BonusOffer", SqlDbType.Int, intUnit);
                    db.AddInParameter(oCommand, "ForEvery", SqlDbType.Int, intEvery);
                    db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
                    db.AddInParameter(oCommand, "Comment", SqlDbType.VarChar, strComment.Trim());
                    db.AddInParameter(oCommand, "Posted", SqlDbType.VarChar, strPosted.Trim());
                    db.AddInParameter(oCommand, "Reverse", SqlDbType.VarChar, strReversed.Trim());
                    db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    db.ExecuteNonQuery(oCommand, transaction);
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

        #region Check Trans No Exist
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("BonusTxnChkTransNoExistUnPosted") as SqlCommand;
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

        #region Get Bonus Offer
        public bool GetBonusOffer(DataGeneral.PostStatus TransStatus)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("BonusTxnSelectUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("BonusTxnSelectPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("BonusTxnSelectReversed") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar,strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                datBonusOfferDate = DateTime.Parse(thisRow[0]["Date"].ToString());
                datClosureDate = DateTime.Parse(thisRow[0]["ClosureDate"].ToString());
                strStockCode = thisRow[0]["StockCode"].ToString();
                strComment = thisRow[0]["Comments"].ToString();
                intUnit = int.Parse(thisRow[0]["BonusOffer"].ToString());
                intEvery = int.Parse(thisRow[0]["ForEvery"].ToString());
                strPosted = thisRow[0]["Posted"].ToString();
                strReversed = thisRow[0]["Reverse"].ToString();

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
                dbCommand = db.GetStoredProcCommand("BonusTxnSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("BonusTxnSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("BonusTxnSelectAllReversed") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Bonus Due
        public DataSet GetBonusDue()
        {
            StkParam oStkParam = new StkParam();
            Company oCompany = new Company();
            DataSet oDS = null;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            if (oCompany.UpdatePortfolioDate(datClosureDate))
            {
                SqlCommand dbCommand = null;
                dbCommand = db.GetStoredProcCommand("BonusTxnSelectDue") as SqlCommand;
                db.AddInParameter(dbCommand, "BonusUnit", SqlDbType.Int, intUnit);
                db.AddInParameter(dbCommand, "ShareNo", SqlDbType.Int, intEvery);
                db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
                db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, oStkParam.Product.Trim());
                oDS = db.ExecuteDataSet(dbCommand);
            }
            return oDS;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BonusTxnDelete") as SqlCommand;
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
                catch (Exception err)
                {
                    transaction.Rollback();
                    throw new Exception(err.Message);
                }
                connection.Close();
            }
            return blnStatus;
        }
        #endregion

        #region Update Only Reversal and Return A Command
        public SqlCommand UpDateRevCommand()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BonusTxnUpdateRev") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "Reversed", SqlDbType.VarChar, strReversed.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.ToUpper());
            return oCommand;
        }
        #endregion

        #region Delete Reversal and Return A Command
        public SqlCommand DeleteReversalCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BonusTxnDeleteReversal") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNoRev.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.ToUpper());
            return oCommand;
        }
        #endregion
    }
}
