using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class FX_AssetItem
    {
        #region Declarations
        private string strTransNo, strTransNoRev, strItemCode, strItemCodeFull, strDescrip;
        private string strBranch, strLocation;
        private decimal decAmount, decAmountPaid,decRealAmount, decSalvageValue;
        private int intAssetType;
        private Int64 intSupplierId;
        private bool blnPosted, blnReversed, blnDoNotPostToGL;
        private DateTime datPurchaseDate;
        private List<FX_AssetItemBank> lstFX_AssetItemBankList;
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
        public string ItemCode
        {
            set { strItemCode = value; }
            get { return strItemCode; }
        }
        public string ItemCodeFull
        {
            set { strItemCodeFull = value; }
            get { return strItemCodeFull; }
        }
        public string Descrip
        {
            set { strDescrip = value; }
            get { return strDescrip; }
        }
       
        public string Branch
        {
            set { strBranch = value; }
            get { return strBranch; }
        }
        public string Location
        {
            set { strLocation = value; }
            get { return strLocation; }
        }
        public decimal Amount
        {
            set { decAmount = value; }
            get { return decAmount; }
        }

        public decimal AmountPaid
        {
            set { decAmountPaid = value; }
            get { return decAmountPaid; }
        }
        public decimal RealAmount
        {
            set { decRealAmount = value; }
            get { return decRealAmount; }
        }
        public decimal SalvageValue
        {
            set { decSalvageValue = value; }
            get { return decSalvageValue; }
        }
        public int AssetType
        {
            set { intAssetType = value; }
            get { return intAssetType; }
        }

        public Int64 SupplierId
        {
            set { intSupplierId = value; }
            get { return intSupplierId; }
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
        public bool DoNotPostToGL
        {
            set { blnDoNotPostToGL = value; }
            get { return blnDoNotPostToGL; }
        }
        
        public DateTime PurchaseDate
        {
            set { datPurchaseDate = value; }
            get { return datPurchaseDate; }
        }
       
        public List<FX_AssetItemBank> FX_AssetItemBankList
        {
            set { lstFX_AssetItemBankList = value; }
            get { return lstFX_AssetItemBankList; }
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
                enSaveStatus = DataGeneral.SaveStatus.NameExistAdd;
                return enSaveStatus;
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {

                    SqlCommand dbCommand = null;
                    if (strSaveType.Trim() == "ADDS")
                    {
                        dbCommand = db.GetStoredProcCommand("FX_AssetItemAddNew") as SqlCommand;
                        db.AddOutParameter(dbCommand, "TransNo", SqlDbType.VarChar, 10);
                    }
                    else if (strSaveType.Trim() == "EDIT")
                    {
                        dbCommand = db.GetStoredProcCommand("FX_AssetItemEdit") as SqlCommand;
                        db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
                    }
                    db.AddInParameter(dbCommand, "ItemCode", SqlDbType.VarChar, strItemCode.Trim());
                    db.AddInParameter(dbCommand, "ItemCodeFull", SqlDbType.VarChar, strItemCodeFull.Trim());
                    db.AddInParameter(dbCommand, "Descrip", SqlDbType.VarChar, strDescrip.Trim());
                    db.AddInParameter(dbCommand, "PurchaseDate", SqlDbType.DateTime, datPurchaseDate);
                    db.AddInParameter(dbCommand, "Branch", SqlDbType.VarChar, strBranch.Trim());
                    db.AddInParameter(dbCommand, "Location", SqlDbType.VarChar, strLocation.Trim());
                    db.AddInParameter(dbCommand, "AssetType", SqlDbType.Int, intAssetType);
                    db.AddInParameter(dbCommand, "Amount", SqlDbType.Decimal, decAmount);
                    db.AddInParameter(dbCommand, "AmountPaid", SqlDbType.Decimal, decAmountPaid);
                    db.AddInParameter(dbCommand, "RealAmount", SqlDbType.Decimal, decRealAmount);
                    db.AddInParameter(dbCommand, "SalvageValue", SqlDbType.Decimal, decSalvageValue);
                    db.AddInParameter(dbCommand, "Posted", SqlDbType.Bit, blnPosted);
                    db.AddInParameter(dbCommand, "Reversed", SqlDbType.Bit, blnReversed);
                    db.AddInParameter(dbCommand, "DoNotPostToGL", SqlDbType.Bit, blnDoNotPostToGL);
                    db.AddInParameter(dbCommand, "SupplierId", SqlDbType.BigInt, intSupplierId);
                    db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    db.ExecuteNonQuery(dbCommand, transaction);
                    if (strSaveType.Trim() == "EDIT")
                    {
                        FX_AssetItemBank oFX_AssetItemBankDelete = new FX_AssetItemBank();
                        oFX_AssetItemBankDelete.AssetItem = int.Parse(strTransNo);
                        SqlCommand dbCommandFX_AssetItemBankDelete = oFX_AssetItemBankDelete.DeleteCommand();
                        db.ExecuteNonQuery(dbCommandFX_AssetItemBankDelete, transaction);

                    }
                    foreach (FX_AssetItemBank oFX_AssetItemBank in lstFX_AssetItemBankList)
                    {
                        SqlCommand dbCommandFX_AssetItemBank;
                        if (strSaveType.Trim() == "ADDS")
                        {
                            oFX_AssetItemBank.AssetItem = int.Parse(db.GetParameterValue(dbCommand, "TransNo").ToString());
                            dbCommandFX_AssetItemBank = oFX_AssetItemBank.AddCommand();
                            db.ExecuteNonQuery(dbCommandFX_AssetItemBank, transaction);
                        }
                        else if (strSaveType.Trim() == "EDIT")
                        {
                            oFX_AssetItemBank.AssetItem = int.Parse(strTransNo);
                            dbCommandFX_AssetItemBank = oFX_AssetItemBank.AddCommand();
                            db.ExecuteNonQuery(dbCommandFX_AssetItemBank, transaction);
                        }
                    }

                    if (strTransNoRev.Trim() != "")
                    {
                        SqlCommand dbCommandDeleteReversal = DeleteReversalCommand();
                        db.ExecuteNonQuery(dbCommandDeleteReversal, transaction);
                    }

                    transaction.Commit();
                    enSaveStatus = DataGeneral.SaveStatus.Saved;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
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
                oCommand = db.GetStoredProcCommand("FX_AssetItemChkTransNoExist") as SqlCommand;
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
            SqlCommand oCommand = db.GetStoredProcCommand("FX_AssetItemChkNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.Char, strTransNo.Trim());
            db.AddInParameter(oCommand, "Descrip", SqlDbType.VarChar, strDescrip.Trim());
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

        #region Get AssetItem
        public bool GetFX_AssetItem(DataGeneral.PostStatus ePostStatus)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (ePostStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("FX_AssetItemSelectUnPosted") as SqlCommand;
            }
            else if (ePostStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("FX_AssetItemSelectPosted") as SqlCommand;
            }
            else if (ePostStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("FX_AssetItemSelectReversed") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strItemCode = thisRow[0]["ItemCode"].ToString();
                strItemCodeFull = thisRow[0]["ItemCodeFull"].ToString();
                strDescrip = thisRow[0]["Descrip"].ToString();
                strBranch = thisRow[0]["Branch"].ToString();
                strLocation = thisRow[0]["Location"].ToString();
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString());
                decAmountPaid = thisRow[0]["AmountPaid"] != null && thisRow[0]["AmountPaid"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["AmountPaid"].ToString()) : 0;
                decRealAmount = thisRow[0]["RealAmount"] != null && thisRow[0]["RealAmount"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["RealAmount"].ToString()) : 0;
                intAssetType = int.Parse(thisRow[0]["AssetType"].ToString());
                intSupplierId = thisRow[0]["SupplierId"] != null && thisRow[0]["SupplierId"].ToString().Trim() != "" ? Int64.Parse(thisRow[0]["SupplierId"].ToString()) : 0;
                decSalvageValue = decimal.Parse(thisRow[0]["SalvageValue"].ToString());
                datPurchaseDate = DateTime.ParseExact(thisRow[0]["PurchaseDate"].ToString().Substring(0,10),"dd/MM/yyyy",format);
                blnPosted = bool.Parse(thisRow[0]["Posted"].ToString());
                blnReversed = bool.Parse(thisRow[0]["Reversed"].ToString());
                blnDoNotPostToGL = thisRow[0]["DoNotPostToGL"] != null && thisRow[0]["DoNotPostToGL"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["DoNotPostToGL"].ToString()) : false;
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
                dbCommand = db.GetStoredProcCommand("FX_AssetItemSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("FX_AssetItemSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("FX_AssetItemSelectAllReversed") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("FX_AssetItemSelectAllPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("FX_AssetItemSelectAllUnPostedAsc") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Disposed
        public DataSet GetAllDisposed(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("FX_AssetItemSelectAllDisposedPosted") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public DataGeneral.SaveStatus Delete()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("FX_AssetItemDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Dispose Item Command
        public SqlCommand DisposeItem()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("FX_AssetItemDisposeItem") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            return dbCommand;
        }
        #endregion

        #region UnDispose Item Command
        public SqlCommand UnDisposeItem()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("FX_AssetItemUnDisposeItem") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            return dbCommand;
        }
        #endregion

      
        #region Get Value Item
        public DataSet GetValueItems()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("FX_AssetItemSelectValueItem") as SqlCommand;
            db.AddInParameter(dbCommand, "PurchaseDate", SqlDbType.DateTime, datPurchaseDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Check If FX_AssetItem Bank Is In The List
        public bool ChkDetailContainInList(int intFX_AssetItemValue)
        {
            bool blnStatus = false;
            foreach (FX_AssetItemBank oFX_AssetItemBanksItem in lstFX_AssetItemBankList)
            {
                if (oFX_AssetItemBanksItem.AssetItem == intFX_AssetItemValue)
                {
                    blnStatus = true;
                    break;
                }
            }
            return blnStatus;
        }
        #endregion

        #region Update Only Reversal and Return A Command
        public SqlCommand UpDateRevCommand()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("FX_AssetItemUpdateRev") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, blnReversed);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Delete Reversal and Return A Command
        public SqlCommand DeleteReversalCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("FX_AssetItemDeleteReversal") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNoRev.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.ToUpper());
            return oCommand;
        }
        #endregion

        #region Get Asset Item Amount
        public decimal GetAmount(string strTransNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("FX_AssetItemSelectAmount") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNumber.Trim());
            var varAmount = db.ExecuteScalar(dbCommand);
            return varAmount != null && varAmount.ToString().Trim() != "" ? decimal.Parse(varAmount.ToString()) : 0;
        }
        #endregion

        #region Get Asset Item Real Amount
        public decimal GetRealAmount(string strTransNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("FX_AssetItemSelectRealAmount") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNumber.Trim());
            var varRealAmount = db.ExecuteScalar(dbCommand);
            return varRealAmount != null && varRealAmount.ToString().Trim() != "" ? decimal.Parse(varRealAmount.ToString()) : 0;
        }
        #endregion
        
    }
}
