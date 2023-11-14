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
using GL.Business;

namespace Asset.Business
{
    public class TBillApp
    {
        #region Declarations
        private string strTransNo, strCustomer, strProduct, strTBillSettlementBankAccount, strBankChargeCode;
        private DateTime datEffDate;
        private decimal decFaceValue, decDiscountedValue, decUpFrontInterestValue, decNetUpFrontInterestValue, decKeepUpFrontInterestValue;
        private decimal decBrokerCommission, decVATBrokerCommission, decBankCharges, decCustodianFee, decIntialFaceValue, decBankChargeCodeBankCharge;
        private int intTenor;
        private float fltDiscount, fltBrokerCommissionRate, fltVATBrokerCommissionRate, fltBankChargeCodeCustodianCharge;
        private bool blnUpFrontInterestFlag, blnChargeBankCharge;
        private string strSaveType;
        #endregion

        #region Properties

        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
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

        public string TBillSettlementBankAccount
        {
            set { strTBillSettlementBankAccount = value; }
            get { return strTBillSettlementBankAccount; }
        }
        public string BankChargeCode
        {
            set { strBankChargeCode = value; }
            get { return strBankChargeCode; }
        }
        public DateTime EffDate
        {
            set { datEffDate = value; }
            get { return datEffDate; }
        }

        public decimal FaceValue
        {
            set { decFaceValue = value; }
            get { return decFaceValue; }
        }
        public decimal DiscountedValue
        {
            set { decDiscountedValue = value; }
            get { return decDiscountedValue; }
        }
        public decimal UpFrontInterestValue
        {
            set { decUpFrontInterestValue = value; }
            get { return decUpFrontInterestValue; }
        }
        public decimal NetUpFrontInterestValue
        {
            set { decNetUpFrontInterestValue = value; }
            get { return decNetUpFrontInterestValue; }
        }
        public decimal KeepUpFrontInterestValue
        {
            set { decKeepUpFrontInterestValue = value; }
            get { return decKeepUpFrontInterestValue; }
        }
        public decimal BrokerCommission
        {
            set { decBrokerCommission = value; }
            get { return decBrokerCommission; }
        }
        public decimal VATBrokerCommission
        {
            set { decVATBrokerCommission = value; }
            get { return decVATBrokerCommission; }
        }
        public decimal BankCharges
        {
            set { decBankCharges = value; }
            get { return decBankCharges; }
        }
        public decimal CustodianFee
        {
            set { decCustodianFee = value; }
            get { return decCustodianFee; }
        }
        public decimal IntialFaceValue
        {
            set { decIntialFaceValue = value; }
            get { return decIntialFaceValue; }
        }
        public decimal BankChargeCodeBankCharge
        {
            set { decBankChargeCodeBankCharge = value; }
            get { return decBankChargeCodeBankCharge; }
        }
        public int Tenor
        {
            set { intTenor = value; }
            get { return intTenor; }
        }
        public float Discount
        {
            set { fltDiscount = value; }
            get { return fltDiscount; }
        }
        public float BrokerCommissionRate
        {
            set { fltBrokerCommissionRate = value; }
            get { return fltBrokerCommissionRate; }
        }
        public float VATBrokerCommissionRate
        {
            set { fltVATBrokerCommissionRate = value; }
            get { return fltVATBrokerCommissionRate; }
        }
        public float BankChargeCodeCustodianCharge
        {
            set { fltBankChargeCodeCustodianCharge = value; }
            get { return fltBankChargeCodeCustodianCharge; }
        }
        public bool UpFrontInterestFlag
        {
            set { blnUpFrontInterestFlag = value; }
            get { return blnUpFrontInterestFlag; }
        }

        public bool ChargeBankCharge
        {
            set { blnChargeBankCharge = value; }
            get { return blnChargeBankCharge; }
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
            if (DateFunction.ChkDateIsFuture(datEffDate))
            {
                enSaveStatus = DataGeneral.SaveStatus.FutureDate;
                return enSaveStatus;
            }
            if (!ChkTransNoExist(DataGeneral.PostStatus.UnPosted))
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
                        oCommand = db.GetStoredProcCommand("TBillAppAddNew") as SqlCommand;
                    }
                    else if (strSaveType == "EDIT")
                    {
                        oCommand = db.GetStoredProcCommand("TBillAppEdit") as SqlCommand;
                    }
                    db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
                    db.AddInParameter(oCommand, "Product", SqlDbType.VarChar, strProduct.Trim());
                    db.AddInParameter(oCommand, "Customer", SqlDbType.VarChar, strCustomer.Trim());
                    db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, datEffDate);
                    db.AddInParameter(oCommand, "FaceValue", SqlDbType.Money, decFaceValue);
                    db.AddInParameter(oCommand, "DiscountedValue", SqlDbType.Money, decDiscountedValue);
                    db.AddInParameter(oCommand, "UpFrontInterestValue", SqlDbType.Money, decUpFrontInterestValue);
                    db.AddInParameter(oCommand, "BrokerCommission", SqlDbType.Money, decBrokerCommission);
                    db.AddInParameter(oCommand, "VATBrokerCommission", SqlDbType.Money, decVATBrokerCommission);
                    db.AddInParameter(oCommand, "BankCharge", SqlDbType.Money, decBankCharges);
                    db.AddInParameter(oCommand, "CustodianFee", SqlDbType.Money, decCustodianFee);
                    db.AddInParameter(oCommand, "Tenor", SqlDbType.Int, intTenor);
                    db.AddInParameter(oCommand, "Discount", SqlDbType.Float, fltDiscount);
                    db.AddInParameter(oCommand, "UpFrontInterestFlag", SqlDbType.Bit, blnUpFrontInterestFlag);
                    db.AddInParameter(oCommand, "NetUpFrontInterestValue", SqlDbType.Money, decNetUpFrontInterestValue);
                    db.AddInParameter(oCommand, "KeepUpFrontInterestValue", SqlDbType.Money, decKeepUpFrontInterestValue);
                    db.AddInParameter(oCommand, "TBillSettlementBankAccount", SqlDbType.VarChar, strTBillSettlementBankAccount.Trim());
                    db.AddInParameter(oCommand, "BrokerCommissionRate", SqlDbType.Real, fltBrokerCommissionRate);
                    db.AddInParameter(oCommand, "VATBrokerCommissionRate", SqlDbType.Real, fltVATBrokerCommissionRate);
                    db.AddInParameter(oCommand, "IntialFaceValue", SqlDbType.Money, decIntialFaceValue);
                    db.AddInParameter(oCommand, "BankChargeCode", SqlDbType.VarChar,strBankChargeCode.Trim());
                    db.AddInParameter(oCommand, "BankChargeCodeBankCharge", SqlDbType.Money, decBankChargeCodeBankCharge);
                    db.AddInParameter(oCommand, "BankChargeCodeCustodianCharge", SqlDbType.Float, fltBankChargeCodeCustodianCharge);
                    db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName);
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

        #region Get
        public bool GetTBillApp(DataGeneral.PostStatus TranStatus)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TranStatus == DataGeneral.PostStatus.UnPosted)
            {

                dbCommand = db.GetStoredProcCommand("TBillAppSelectUnPosted") as SqlCommand;
            }
            else if (TranStatus == DataGeneral.PostStatus.Posted)
            {

                dbCommand = db.GetStoredProcCommand("TBillAppSelectPosted") as SqlCommand;
            }
            else if (TranStatus == DataGeneral.PostStatus.Reversed)
            {

                dbCommand = db.GetStoredProcCommand("TBillAppSelectReversed") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strCustomer = thisRow[0]["Customer"].ToString();
                strProduct = thisRow[0]["Product"].ToString();
                datEffDate = DateTime.ParseExact(thisRow[0]["EffDate"].ToString().Substring(0, 10), "dd/MM/yyyy", format);
                decFaceValue = decimal.Parse(thisRow[0]["FaceValue"].ToString());
                decDiscountedValue = decimal.Parse(thisRow[0]["DiscountedValue"].ToString());
                decUpFrontInterestValue = decimal.Parse(thisRow[0]["UpFrontInterestValue"].ToString());
                decBrokerCommission = decimal.Parse(thisRow[0]["BrokerCommission"].ToString());
                decVATBrokerCommission = thisRow[0]["VATBrokerCommission"] != null && thisRow[0]["VATBrokerCommission"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["VATBrokerCommission"].ToString().Trim()) : 0;
                decBankCharges = decimal.Parse(thisRow[0]["BankCharges"].ToString());
                decCustodianFee = decimal.Parse(thisRow[0]["CustodianFee"].ToString());
                intTenor = int.Parse(thisRow[0]["Tenor"].ToString());
                fltDiscount = float.Parse(thisRow[0]["Discount"].ToString());
                blnUpFrontInterestFlag = bool.Parse(thisRow[0]["UpFrontInterestFlag"].ToString());
                decNetUpFrontInterestValue = decimal.Parse(thisRow[0]["NetUpFrontInterestValue"].ToString());
                decKeepUpFrontInterestValue = decimal.Parse(thisRow[0]["KeepUpFrontInterestValue"].ToString());
                strTBillSettlementBankAccount = thisRow[0]["TBillSettlementBankAccount"] != null ? thisRow[0]["TBillSettlementBankAccount"].ToString().Trim() : "";
                fltBrokerCommissionRate = thisRow[0]["BrokerCommissionRate"] != null && thisRow[0]["BrokerCommissionRate"].ToString().Trim() != "" ? float.Parse(thisRow[0]["BrokerCommissionRate"].ToString().Trim()) : 0;
                fltVATBrokerCommissionRate = thisRow[0]["VATBrokerCommissionRate"] != null && thisRow[0]["VATBrokerCommissionRate"].ToString().Trim() != "" ? float.Parse(thisRow[0]["VATBrokerCommissionRate"].ToString().Trim()) : 0;
                decIntialFaceValue = thisRow[0]["IntialFaceValue"] != null && thisRow[0]["IntialFaceValue"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["IntialFaceValue"].ToString()) : 0;
                strBankChargeCode = thisRow[0]["BankChargeCode"] != null ? thisRow[0]["BankChargeCode"].ToString().Trim() : "";
                fltBankChargeCodeCustodianCharge = thisRow[0]["BankChargeCodeCustodianCharge"] != null && thisRow[0]["BankChargeCodeCustodianCharge"].ToString().Trim() != "" ? float.Parse(thisRow[0]["BankChargeCodeCustodianCharge"].ToString().Trim()) : 0;
                decBankChargeCodeBankCharge = thisRow[0]["BankChargeCodeBankCharge"] != null && thisRow[0]["BankChargeCodeBankCharge"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["BankChargeCodeBankCharge"].ToString()) : 0;
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
                dbCommand = db.GetStoredProcCommand("TBillAppSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("TBillAppSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("TBillAppSelectAllReversed") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Check TransNo Exist
        public bool ChkTransNoExist(DataGeneral.PostStatus ePostStatus)
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = null;
                if (ePostStatus == DataGeneral.PostStatus.UnPosted)
                {
                    oCommand = db.GetStoredProcCommand("TBillAppChkTransNoExistUnPosted") as SqlCommand;
                }
                else if (ePostStatus == DataGeneral.PostStatus.Posted)
                {
                    oCommand = db.GetStoredProcCommand("TBillAppChkTransNoExistPosted") as SqlCommand;
                }
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

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("TBillAppDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Update Only Reversal and Return A Command
        public SqlCommand UpDateRevCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("TBillAppUpdateRev") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Compute Discounted Value
        public decimal ComputeDiscountedValue()
        {
            decimal decCalDiscountAndTenor = (decimal.Parse(fltDiscount.ToString()) * decimal.Parse(intTenor.ToString())) / (365 * 100);
            decimal decCalMinusFromOne = 1 - decCalDiscountAndTenor;
            decimal decDiscountedValueCompute = decFaceValue * decCalMinusFromOne;
            decDiscountedValueCompute = Math.Round(decDiscountedValueCompute, 2);
            return decDiscountedValueCompute;
        }
        #endregion

        #region Compute Upfront Interest Value
        public decimal ComputeUpfrontInterestValue()
        {
            decimal decUpfrontInterestValueCompute = (decFaceValue * decimal.Parse(fltDiscount.ToString()) * decimal.Parse(intTenor.ToString())) / (365 * 100);
            decUpfrontInterestValueCompute = Math.Round(decUpfrontInterestValueCompute, 2);
            return decUpfrontInterestValueCompute;
        }
        #endregion

        #region Compute Broker Commission
        public decimal ComputeBrokerCommission(float fltBrokerCommissionParameter)
        {
            decimal decBrokerCommissionCompute = (decFaceValue * decimal.Parse(fltBrokerCommissionParameter.ToString()) * decimal.Parse(intTenor.ToString())) / (365 * 100);
            decBrokerCommissionCompute = Math.Round(decBrokerCommissionCompute, 2);
            return decBrokerCommissionCompute;
        }
        #endregion

        #region Compute VAT Broker Commission
        public decimal ComputeVATBrokerCommission(decimal decBrokerCommission, float fltVATBrokerCommissionParameter)
        {
            decimal decVATBrokerCommissionCompute = (decBrokerCommission * decimal.Parse(fltVATBrokerCommissionParameter.ToString())) / 100;
            decVATBrokerCommissionCompute = Math.Round(decVATBrokerCommissionCompute, 2);
            return decVATBrokerCommissionCompute;
        }
        #endregion

        #region Compute Custodian Fee
        public decimal ComputeCustodianFee(float fltCustodianFeeParameter)
        {
            decimal decCustodianFeeCompute = (decFaceValue * decimal.Parse(fltCustodianFeeParameter.ToString()) * decimal.Parse(intTenor.ToString())) / (365 * 100);
            decCustodianFeeCompute = Math.Round(decCustodianFeeCompute, 2);
            return decCustodianFeeCompute;
        }
        #endregion
    }
}

