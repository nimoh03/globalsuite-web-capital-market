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
    public class PortfolioInternalUpdate
    {
        #region Declaration
        private string strTransNo, strCustomerId, strStockCode,strDescription;
        private DateTime datEffDate;
        private int intQuantity;
        private float fltUnitPrice;
        private bool blnPosted, blnReversed,blnAddOrDeduct;
        private string strSaveType;
        #endregion

        #region Properties

        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public DateTime EffDate
        {
            set { datEffDate = value; }
            get { return datEffDate; }
        }

        public string CustomerId
        {
            set { strCustomerId = value; }
            get { return strCustomerId; }
        }
        public string StockCode
        {
            set { strStockCode = value; }
            get { return strStockCode; }
        }

        public string Description
        {
            set { strDescription = value; }
            get { return strDescription; }
        }

        public int Quantity
        {
            set { intQuantity = value; }
            get { return intQuantity; }
        }
        public float UnitPrice
        {
            set { fltUnitPrice = value; }
            get { return fltUnitPrice; }
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
        public bool AddOrDeduct
        {
            set { blnAddOrDeduct = value; }
            get { return blnAddOrDeduct; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        #endregion

        #region Check Trans No Exist
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("PortfolioInternalUpdateChkTransNoExistUnPosted") as SqlCommand;
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

        #region Check Stock Enough Exist
        public bool ChkStockEnoughExist()
        {
            bool blnStatus = false;
            Portfolio oPortfolio = new Portfolio();
            oPortfolio.CustomerAcct = strCustomerId;
            oPortfolio.StockCode = strStockCode;
            oPortfolio.PurchaseDate = datEffDate;
            if (oPortfolio.GetNetHolding() >= intQuantity)
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
            if (!blnAddOrDeduct)
            {
                if (!ChkStockEnoughExist())
                {
                    enSaveStatus = DataGeneral.SaveStatus.NameExistAdd;
                    return enSaveStatus;
                }
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (strSaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("PortfolioInternalUpdateAdd") as SqlCommand;
            }
            else if (strSaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("PortfolioInternalUpdateEdit") as SqlCommand;
            }
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, datEffDate);
            db.AddInParameter(oCommand, "Customer", SqlDbType.VarChar, strCustomerId.Trim());
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(oCommand, "Description", SqlDbType.VarChar,strDescription.Trim());
            db.AddInParameter(oCommand, "Quantity", SqlDbType.Int, intQuantity);
            db.AddInParameter(oCommand, "UnitPrice", SqlDbType.Float, fltUnitPrice);
            db.AddInParameter(oCommand, "Posted", SqlDbType.Bit, blnPosted);
            db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, blnReversed);
            db.AddInParameter(oCommand, "AddOrDeduct", SqlDbType.Bit, blnAddOrDeduct);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;

        }
        #endregion

        #region Get
        public bool GetPortfolioInternalUpdate(DataGeneral.PostStatus TransStatus)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("PortfolioInternalUpdateSelectUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PortfolioInternalUpdateSelectPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("PortfolioInternalUpdateSelectReversed") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                datEffDate = DateTime.ParseExact(thisRow[0]["EffDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                strCustomerId = thisRow[0]["CustomerId"].ToString();
                strStockCode = thisRow[0]["StockCode"].ToString();
                strDescription = thisRow[0]["Description"].ToString();
                intQuantity = int.Parse(thisRow[0]["Units"].ToString());
                fltUnitPrice = float.Parse(thisRow[0]["UnitPrice"].ToString());
                blnPosted = bool.Parse(thisRow[0]["Posted"].ToString());
                blnReversed = bool.Parse(thisRow[0]["Reversed"].ToString());
                blnAddOrDeduct = bool.Parse(thisRow[0]["AddOrDeduct"].ToString());
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
                dbCommand = db.GetStoredProcCommand("PortfolioInternalUpdateSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PortfolioInternalUpdateSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("PortfolioInternalUpdateSelectAllReversed") as SqlCommand;
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
            SqlCommand oCommand = db.GetStoredProcCommand("PortfolioInternalUpdateDelete") as SqlCommand;
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
            SqlCommand oCommand = db.GetStoredProcCommand("PortfolioInternalUpdateUpdateRev") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion
    }
}
