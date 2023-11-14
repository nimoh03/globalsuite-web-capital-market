using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using BaseUtility.Business;

namespace CapitalMarket.Business
{
    public class PortfolioRemoveAll
    {
        #region Properties
        public string TransNo { set; get; }
        public string CustomerId { set; get; }
        public DateTime EffDate { set; get; }
        public bool Posted { set; get; }
        public bool Reversed { set; get; }
        public string SaveType { set; get; }
       
        #endregion

        

        #region Check Trans No Exist
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;
            if (SaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("PortfolioRemoveAllChkTransNoExistUnPosted") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, TransNo.Trim());
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
            if (SaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("PortfolioRemoveAllAdd") as SqlCommand;
            }
            else if (SaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("PortfolioRemoveAllEdit") as SqlCommand;
            }
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, TransNo.Trim());
            db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, EffDate);
            db.AddInParameter(oCommand, "CustomerId", SqlDbType.VarChar, CustomerId.Trim());
            db.AddInParameter(oCommand, "Posted", SqlDbType.Bit, Posted);
            db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, Reversed);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;

        }
        #endregion

        #region Get
        public bool GetPortfolioRemoveAll(DataGeneral.PostStatus TransStatus)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("PortfolioRemoveAllSelectUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PortfolioRemoveAllSelectPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("PortfolioRemoveAllSelectReversed") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, TransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                EffDate = DateTime.ParseExact(thisRow[0]["EffDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                CustomerId = thisRow[0]["CustomerId"].ToString();
                Posted = bool.Parse(thisRow[0]["Posted"].ToString());
                Reversed = bool.Parse(thisRow[0]["Reversed"].ToString());
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
                dbCommand = db.GetStoredProcCommand("PortfolioRemoveAllSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PortfolioRemoveAllSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("PortfolioRemoveAllSelectAllReversed") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PortfolioRemoveAllDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, TransNo.Trim());
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
            SqlCommand oCommand = db.GetStoredProcCommand("PortfolioRemoveAllUpdateRev") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, TransNo.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion
    }
}
