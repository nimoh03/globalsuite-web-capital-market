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
    public class TechReconstruction
    {
        #region Declarations
        private string strTransNo, strTransNoRev;
        private DateTime datEffDate;
        private float fltForEvery, fltConvertUnit;
        private string strStockCode,strNewStockName, strDescription;
        private bool blnChangeStockName,blnPosted, blnReversed;
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
        public float ForEvery
        {
            set { fltForEvery = value; }
            get { return fltForEvery; }
        }
        public float ConvertUnit
        {
            set { fltConvertUnit = value; }
            get { return fltConvertUnit; }
        }
        public DateTime EffDate
        {
            set { datEffDate = value; }
            get { return datEffDate; }
        }
        public string StockCode
        {
            set { strStockCode = value; }
            get { return strStockCode; }
        }
        public string NewStockName
        {
            set { strNewStockName = value; }
            get { return strNewStockName; }
        }
        public string Description
        {
            set { strDescription = value; }
            get { return strDescription; }
        }
        public bool ChangeStockName
        {
            set { blnChangeStockName = value; }
            get { return blnChangeStockName; }
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
                        oCommand = db.GetStoredProcCommand("TechReconstructionAdd") as SqlCommand;
                    }
                    else if (strSaveType == "EDIT")
                    {
                        oCommand = db.GetStoredProcCommand("TechReconstructionEdit") as SqlCommand;
                    }
                    db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
                    db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, datEffDate);
                    db.AddInParameter(oCommand, "ForEvery", SqlDbType.Float, fltForEvery);
                    db.AddInParameter(oCommand, "ConvertUnit", SqlDbType.Float, fltConvertUnit);
                    db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
                    db.AddInParameter(oCommand, "NewStockName", SqlDbType.VarChar, strNewStockName);
                    db.AddInParameter(oCommand, "Description", SqlDbType.VarChar, strDescription.Trim());
                    db.AddInParameter(oCommand, "ChangeStockName", SqlDbType.Bit, blnChangeStockName);
                    db.AddInParameter(oCommand, "Posted", SqlDbType.Bit, blnPosted);
                    db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, blnReversed);
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
                SqlCommand oCommand = db.GetStoredProcCommand("TechReconstructionChkTransNoExistUnPosted") as SqlCommand;
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

        #region Get Technical Reconstruction
        public bool GetTechReconstruction(DataGeneral.PostStatus TransStatus)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("TechReconstructionSelectUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("TechReconstructionSelectPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("TechReconstructionSelectReversed") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                datEffDate = DateTime.Parse(thisRow[0]["EffDate"].ToString());
                strStockCode = thisRow[0]["StockCode"].ToString();
                strNewStockName = thisRow[0]["NewStockName"].ToString();
                strDescription = thisRow[0]["Description"].ToString();
                fltForEvery = float.Parse(thisRow[0]["ForEvery"].ToString());
                fltConvertUnit = float.Parse(thisRow[0]["ConvertUnit"].ToString());
                blnChangeStockName = bool.Parse(thisRow[0]["ChangeStockName"].ToString());
                blnPosted = bool.Parse(thisRow[0]["Posted"].ToString());
                blnReversed = bool.Parse(thisRow[0]["Reversed"].ToString());
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
                dbCommand = db.GetStoredProcCommand("TechReconstructionSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("TechReconstructionSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("TechReconstructionSelectAllReversed") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Technical Reconstruction
        public DataSet GetTechReconstructionDue()
        {
            StkParam oStkParam = new StkParam();
            Company oCompany = new Company();
            DataSet oDS = null;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            if (oCompany.UpdatePortfolioDate(datEffDate))
            {
                SqlCommand dbCommand = null;
                dbCommand = db.GetStoredProcCommand("TechReconstructionSelectDue") as SqlCommand;
                db.AddInParameter(dbCommand, "ForEvery", SqlDbType.Float, fltForEvery);
                db.AddInParameter(dbCommand, "ConvertUnit", SqlDbType.Float, fltConvertUnit);
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
            SqlCommand oCommand = db.GetStoredProcCommand("TechReconstructionDelete") as SqlCommand;
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
            SqlCommand oCommand = db.GetStoredProcCommand("TechReconstructionUpdateRev") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, blnReversed);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.ToUpper());
            return oCommand;
        }
        #endregion

        #region Delete Reversal and Return A Command
        public SqlCommand DeleteReversalCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("TechReconstructionDeleteReversal") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNoRev.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.ToUpper());
            return oCommand;
        }
        #endregion
    }
}
