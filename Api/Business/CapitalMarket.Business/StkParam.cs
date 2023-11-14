using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace CapitalMarket.Business
{
    public class StkParam
    {
        #region Declaration
        private string strProduct, strProductBrokPay, strProductInvestment, strProductNASDAccount, strProductInvestmentNASD, strProductInvestmentBond;
        private decimal decBCsCs,decSMSAlertCSCSB, decScscs,decSMSAlertCSCSS, decBnse, decSnse, decBstamp, decSstamp, decBvat, decSvat;
        private decimal decBcncomm, decScncomm, decBsec, decSsec, decBothCharge, decSothCharge;
        private string strUpLoadDone, strCustAcctUpLoadDone;
        private decimal decMinSecB, decMinVatB, decMinCommB, decMinNSceB, decMinCscsB;
        private decimal decMinStampB, decMinSecS, decMinVatS, decMinCommS, decMinSceS;
        private decimal decMinCscsS, decMinStampS;
        private string strVatComm, strStampVat;
        private DateTime datEODDate, datPayRollDate, datEOYDate;
        private Int32 intZero;
        private DateTime datEOMDate;
        private decimal decBcscsVat,decSMSAlertCSCSVATB, decScscsVat,decSMSAlertCSCSVATS, decBnseVat, decSnseVat, decBsecVat;
        private decimal decSsecVat, decMinCscsVATB, decMinCscsVatS, decMinNseVatB;
        private decimal decMinNseVatS, decMinSecVatB, decMinSecVatS, decOrderPriceMarkup;
        private Int32 intAllot, intStock, intSettleDay, intDropJobOrderDayNumber;
        private decimal decInterestRate;
        private string strInterestPeriod,strCommType;
        private Decimal decNASDBSec, decNASDSSec, decNASDBCSCS, decNASDSCSCS, decNASDBComm, decNASDSComm, decNASDBSMSCharge, decNASDSSMSCharge, decNASDSNASD, decNASDBNASD, decNASDBStamp, decNASDSStamp;
        private decimal decNASDBCommVAT, decNASDSCommVAT, decNASDBNASDVAT, decNASDSNASDVAT, decNASDBSECVAT, decNASDSSECVAT;
        private decimal decNASDBCSCSVAT, decNASDSCSCSVAT, decNASDBAlertVAT, decNASDSAlertVAT;
        private Decimal decNASDBCommMin, decNASDSCommMin;
        private decimal decBondSECBuy, decBondCommissionBuy, decBondNSEBuy, decBondCSCSBuy, decBondContractStampBuy, decBondSMSAlertBuy, decBondSMSAlertVATBuy;
        private decimal decBondSECSell, decBondCommissionSell, decBondNSESell, decBondCSCSSell, decBondContractStampSell, decBondSMSAlertSell, decBondSMSAlertVATSell;
        private Decimal decBondCommissionMinimumBuy, decBondCommissionMinimumSell;
        private string strNASDMemberCode;
        
        private DataGeneral.StockInstrumentType datgenStockInstrument;
        #endregion

        #region Properties

        public string Product
        {
            set { strProduct = value; }
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("PAGentableSelectStockAcctMaster") as SqlCommand;
                strProduct = (string)db.ExecuteScalar(dbCommand).ToString();
                return strProduct;
            }
        }

        public string ProductBrokPay
        {
            set { strProductBrokPay = value; }
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("PAGentableSelectAgentBrokage") as SqlCommand;
                strProductBrokPay = (string)db.ExecuteScalar(dbCommand).ToString();
                return strProductBrokPay;
            }
        }

        public string ProductInvestment
        {
            set { strProductInvestment = value; }
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("PAGentableSelectInvestment") as SqlCommand;
                strProductInvestment = (string)db.ExecuteScalar(dbCommand).ToString();
                return strProductInvestment;
            }
        }

        public string ProductNASDAccount
        {
            set { strProductNASDAccount = value; }
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("PAGentableSelectNASDAccount") as SqlCommand;
                strProductInvestment = (string)db.ExecuteScalar(dbCommand).ToString();
                return strProductInvestment;
            }
        }

        public string ProductInvestmentNASD
        {
            set { strProductInvestmentNASD = value; }
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("PAGentableSelectInvestmentNASD") as SqlCommand;
                strProductInvestmentNASD = (string)db.ExecuteScalar(dbCommand).ToString();
                return strProductInvestmentNASD;
            }
        }

        public string ProductInvestmentBond
        {
            set { strProductInvestmentBond = value; }
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("PAGentableSelectInvestmentBond") as SqlCommand;
                strProductInvestmentBond = (string)db.ExecuteScalar(dbCommand).ToString();
                return strProductInvestmentBond;
            }
        }

        public decimal OrderPriceMarkup
        {
            set { decOrderPriceMarkup = value; }
            get {return decOrderPriceMarkup; }
        }
        
        public decimal BCsCs
        {
            set { decBCsCs = value; }
            get { if (datgenStockInstrument == DataGeneral.StockInstrumentType.BOND)
                { return decBondCSCSBuy; }
                else if (datgenStockInstrument == DataGeneral.StockInstrumentType.NASD)
                { return decNASDBCSCS; }
                else
                { return decBCsCs; } }

            }

         public decimal SMSAlertCSCSB
        {
            set { decSMSAlertCSCSB = value; }
            get
            {
                if (datgenStockInstrument == DataGeneral.StockInstrumentType.BOND)
                { return decBondSMSAlertBuy; }
                else if (datgenStockInstrument == DataGeneral.StockInstrumentType.NASD)
                { return decNASDBSMSCharge; }
                else
                { return decSMSAlertCSCSB; }
            }
        }

        public decimal Scscs
        {
            set { decScscs = value; }
            get
            {
                if (datgenStockInstrument == DataGeneral.StockInstrumentType.BOND)
                { return decBondCSCSSell; }
                else if (datgenStockInstrument == DataGeneral.StockInstrumentType.NASD)
                { return decNASDSCSCS; }
                else
                { return decScscs; }
            }
        }
        public decimal SMSAlertCSCSS
        {
            set { decSMSAlertCSCSS = value; }
            get { 
                if (datgenStockInstrument == DataGeneral.StockInstrumentType.BOND)
                { return decBondSMSAlertSell; }
                else if (datgenStockInstrument == DataGeneral.StockInstrumentType.NASD)
                { return decNASDSSMSCharge; }
                else
                { return decSMSAlertCSCSS; }
            }
        }
        public decimal Bnse
        {
            set { decBnse = value; }
            get
            {
                if (datgenStockInstrument == DataGeneral.StockInstrumentType.BOND)
                { return decBondNSEBuy; }
                else if (datgenStockInstrument == DataGeneral.StockInstrumentType.NASD)
                { return decNASDBNASD; }
                else
                { return decBnse; }
            }
        }
        public decimal Snse
        {
            set { decSnse = value; }
            
            get
            {
                if (datgenStockInstrument == DataGeneral.StockInstrumentType.BOND)
                { return decBondNSESell; }
                else if (datgenStockInstrument == DataGeneral.StockInstrumentType.NASD)
                { return decNASDSNASD; }
                else
                { return decSnse; }
            }
        }
        public decimal Bstamp
        {
            set { decBstamp = value; }
            
            get
            {
                if (datgenStockInstrument == DataGeneral.StockInstrumentType.BOND)
                { return decBondContractStampBuy; }
                else if (datgenStockInstrument == DataGeneral.StockInstrumentType.NASD)
                { return decNASDBStamp; }
                else
                { return decBstamp; }
            }
        }
        public decimal Sstamp
        {
            set { decSstamp = value; }
            
            get
            {
                if (datgenStockInstrument == DataGeneral.StockInstrumentType.BOND)
                { return decBondContractStampSell; }
                else if (datgenStockInstrument == DataGeneral.StockInstrumentType.NASD)
                { return decNASDSStamp; }
                else
                { return decSstamp; }
            }
        }
        public decimal Bvat
        {
            set { decBvat = value; }
            
            get
            {
                if (datgenStockInstrument == DataGeneral.StockInstrumentType.BOND)
                { return decBvat; }
                else if (datgenStockInstrument == DataGeneral.StockInstrumentType.NASD)
                { return decNASDBCommVAT; }
                else
                { return decBvat; }
            }
        }
        public decimal Svat
        {
            set { decSvat = value; }
            
            get
            {
                if (datgenStockInstrument == DataGeneral.StockInstrumentType.BOND)
                { return decSvat; }
                else if (datgenStockInstrument == DataGeneral.StockInstrumentType.NASD)
                { return decNASDSCommVAT; }
                else
                { return decSvat; }
            }
        }
        public decimal Bcncomm
        {
            set { decBcncomm = value; }
            
            get
            {
                if (datgenStockInstrument == DataGeneral.StockInstrumentType.BOND)
                { return decBondCommissionBuy; }
                else if (datgenStockInstrument == DataGeneral.StockInstrumentType.NASD)
                { return decNASDBComm; }
                else
                { return decBcncomm; }
            }
        }
        public decimal Scncomm
        {
            set { decScncomm = value; }
            get
            {
                if (datgenStockInstrument == DataGeneral.StockInstrumentType.BOND)
                { return decBondCommissionSell; }
                else if (datgenStockInstrument == DataGeneral.StockInstrumentType.NASD)
                { return decNASDSComm; }
                else
                { return decScncomm; }
            }
        }
        public decimal Bsec
        {
            set { decBsec = value; }
            
            get
            {
                if (datgenStockInstrument == DataGeneral.StockInstrumentType.BOND)
                { return decBondSECBuy; }
                else if (datgenStockInstrument == DataGeneral.StockInstrumentType.NASD)
                { return decNASDBSec; }
                else
                { return decBsec; }
            }
        }
        public decimal Ssec
        {
            set { decSsec = value; }
            get
            {
                if (datgenStockInstrument == DataGeneral.StockInstrumentType.BOND)
                { return decBondSECSell; }
                else if (datgenStockInstrument == DataGeneral.StockInstrumentType.NASD)
                { return decNASDSSec; }
                else
                { return decSsec; }
            }
        }
        public decimal BothCharge
        {
            set { decBothCharge = value; }
            get { return datgenStockInstrument != DataGeneral.StockInstrumentType.BOND ? decBothCharge : decBothCharge; }
        }
        public decimal SothCharge
        {
            set { decSothCharge = value; }
            get { return datgenStockInstrument != DataGeneral.StockInstrumentType.BOND ? decSothCharge : decSothCharge; }
        }
        public string UpLoadDone
        {
            set { strUpLoadDone = value; }
            get { return strUpLoadDone; }
        }
        public string CustAcctUpLoadDone
        {
            set { strCustAcctUpLoadDone = value; }
            get { return strCustAcctUpLoadDone; }
        }
        public decimal MinSecB
        {
            set { decMinSecB = value; }
            get { return datgenStockInstrument != DataGeneral.StockInstrumentType.BOND ? decMinSecB : decMinSecB; }
        }
        public decimal MinVatB
        {
            set { decMinVatB = value; }
            get { return datgenStockInstrument != DataGeneral.StockInstrumentType.BOND ? decMinVatB : decMinVatB; }
        }
        public decimal MinCommB
        {
            set { decMinCommB = value; }
            get
            {
                if (datgenStockInstrument == DataGeneral.StockInstrumentType.BOND)
                { return decMinCommB; }
                else if (datgenStockInstrument == DataGeneral.StockInstrumentType.NASD)
                { return decNASDBCommMin; }
                else
                { return decMinCommB; }
            }
        }
        public decimal MinNSceB
        {
            set { decMinNSceB = value; }
            get { return datgenStockInstrument != DataGeneral.StockInstrumentType.BOND ? decMinNSceB : decMinNSceB; }
        }
        public decimal MinCscsB
        {
            set { decMinCscsB = value; }
            get { return datgenStockInstrument != DataGeneral.StockInstrumentType.BOND ? decMinCscsB : decMinCscsB; }
        }
        public decimal MinStampB
        {
            set { decMinStampB = value; }
            get { return datgenStockInstrument != DataGeneral.StockInstrumentType.BOND ? decMinStampB : decMinStampB; }
        }
        public decimal MinSecS
        {
            set { decMinSecS = value; }
            get { return datgenStockInstrument != DataGeneral.StockInstrumentType.BOND ? decMinSecS : decMinSecS; }
        }
        public decimal MinVatS
        {
            set { decMinVatS = value; }
            get { return datgenStockInstrument != DataGeneral.StockInstrumentType.BOND ? decMinVatS : decMinVatS; }
        }
        public decimal MinCommS
        {
            set { decMinCommS = value; }
            get
            {
                if (datgenStockInstrument == DataGeneral.StockInstrumentType.BOND)
                { return decMinCommS; }
                else if (datgenStockInstrument == DataGeneral.StockInstrumentType.NASD)
                { return decNASDSCommMin; }
                else
                { return decMinCommS; }
            }
        }
        public decimal MinSceS
        {
            set { decMinSceS = value; }
            get { return datgenStockInstrument != DataGeneral.StockInstrumentType.BOND ? decMinSceS : decMinSceS; }
        }
        public decimal MinCscsS
        {
            set { decMinCscsS = value; }
            get { return datgenStockInstrument != DataGeneral.StockInstrumentType.BOND ? decMinCscsS : decMinCscsS; }
        }
        public decimal MinStampS
        {
            set { decMinStampS = value; }
            get { return datgenStockInstrument != DataGeneral.StockInstrumentType.BOND ? decMinStampS : decMinStampS; }
        }
        public string VatComm
        {
            set { strVatComm = value; }
            get { return strVatComm; }
        }
        public string StampVat
        {
            set { strStampVat = value; }
            get { return strStampVat; }
        }
        public DateTime EODDate
        {
            set { datEODDate = value; }
            get { return datEODDate; }
        }
        public DateTime PayRollDate
        {
            set { datPayRollDate = value; }
            get { return datPayRollDate; }
        }
        public DateTime EOYDate
        {
            set { datEOYDate = value; }
            get { return datEOYDate; }
        }
        public Int32 Zero
        {
            set { intZero = value; }
            get { return intZero; }
        }
        public DateTime EOMDate
        {
            set { datEOMDate = value; }
            get { return datEOMDate; }
        }
        public decimal BcscsVat
        {
            set { decBcscsVat = value; }
            get
            {
                if (datgenStockInstrument == DataGeneral.StockInstrumentType.BOND)
                { return decBcscsVat; }
                else if (datgenStockInstrument == DataGeneral.StockInstrumentType.NASD)
                { return decNASDBCSCSVAT; }
                else
                { return decBcscsVat; }
            }
        }
        public decimal SMSAlertCSCSVATB
        {
            set { decSMSAlertCSCSVATB = value; }
            
            get
            {
                if (datgenStockInstrument == DataGeneral.StockInstrumentType.BOND)
                { return decBondSMSAlertVATBuy; }
                else if (datgenStockInstrument == DataGeneral.StockInstrumentType.NASD)
                { return decNASDBAlertVAT; }
                else
                { return decSMSAlertCSCSVATB; }
            }
        }
        public decimal ScscsVat
        {
            set { decScscsVat = value; }
            get
            {
                if (datgenStockInstrument == DataGeneral.StockInstrumentType.BOND)
                { return decScscsVat; }
                else if (datgenStockInstrument == DataGeneral.StockInstrumentType.NASD)
                { return decNASDSCSCSVAT; }
                else
                { return decScscsVat; }
            }
        }
        public decimal SMSAlertCSCSVATS
        {
            set { decSMSAlertCSCSVATS = value; }
            
            get
            {
                if (datgenStockInstrument == DataGeneral.StockInstrumentType.BOND)
                { return decBondSMSAlertVATSell; }
                else if (datgenStockInstrument == DataGeneral.StockInstrumentType.NASD)
                { return decNASDSAlertVAT; }
                else
                { return decSMSAlertCSCSVATS; }
            }
        }
        public decimal BnseVat
        {
            set { decBnseVat = value; }
            
            get
            {
                if (datgenStockInstrument == DataGeneral.StockInstrumentType.BOND)
                { return decBnseVat; }
                else if (datgenStockInstrument == DataGeneral.StockInstrumentType.NASD)
                { return decNASDBNASDVAT; }
                else
                { return decBnseVat; }
            }
        }
        public decimal SnseVat
        {
            set { decSnseVat = value; }
            
            get
            {
                if (datgenStockInstrument == DataGeneral.StockInstrumentType.BOND)
                { return decSnseVat; }
                else if (datgenStockInstrument == DataGeneral.StockInstrumentType.NASD)
                { return decNASDSNASDVAT; }
                else
                { return decSnseVat; }
            }
        }
        public decimal BsecVat
        {
            set { decBsecVat = value; }
            
            get
            {
                if (datgenStockInstrument == DataGeneral.StockInstrumentType.BOND)
                { return decBsecVat; }
                else if (datgenStockInstrument == DataGeneral.StockInstrumentType.NASD)
                { return decNASDBSECVAT; }
                else
                { return decBsecVat; }
            }
        }
        public decimal SsecVat
        {
            set { decSsecVat = value; }
            
            get
            {
                if (datgenStockInstrument == DataGeneral.StockInstrumentType.BOND)
                { return decSsecVat; }
                else if (datgenStockInstrument == DataGeneral.StockInstrumentType.NASD)
                { return decNASDSSECVAT; }
                else
                { return decSsecVat; }
            }
        }
        public decimal MinCscsVATB
        {
            set { decMinCscsVATB = value; }
            get { return datgenStockInstrument != DataGeneral.StockInstrumentType.BOND ? decMinCscsVATB : decMinCscsVATB; }
        }
        public decimal MinCscsVatS
        {
            set { decMinCscsVatS = value; }
            get { return datgenStockInstrument != DataGeneral.StockInstrumentType.BOND ? decMinCscsVatS : decMinCscsVatS; }
        }
        public decimal MinNseVatB
        {
            set { decMinNseVatB = value; }
            get { return datgenStockInstrument != DataGeneral.StockInstrumentType.BOND ? decMinNseVatB : decMinNseVatB; }
        }
        public decimal MinNseVatS
        {
            set { decMinNseVatS = value; }
            get { return datgenStockInstrument != DataGeneral.StockInstrumentType.BOND ? decMinNseVatS : decMinNseVatS; }
        }
        public decimal MinSecVatB
        {
            set { decMinSecVatB = value; }
            get { return datgenStockInstrument != DataGeneral.StockInstrumentType.BOND ? decMinSecVatB : decMinSecVatB; }
        }
        public decimal MinSecVatS
        {
            set { decMinSecVatS = value; }
            get { return datgenStockInstrument != DataGeneral.StockInstrumentType.BOND ? decMinSecVatS : decMinSecVatS; }
        }
        public Int32 Allot
        {
            set { intAllot = value; }
            get { return intAllot; }
        }
        public Int32 Stock
        {
            set { intStock = value; }
            get { return intStock; }
        }
        public Int32 SettleDay
        {
            set { intSettleDay = value; }
            get { return intSettleDay; }
        }
        public Int32 DropJobOrderDayNumber
        {
            set { intDropJobOrderDayNumber = value; }
            get { return intDropJobOrderDayNumber; }
        }
        public decimal InterestRate
        {
            set { decInterestRate = value; }
            get { return decInterestRate; }
        }
        public string InterestPeriod
        {
            set { strInterestPeriod = value; }
            get { return strInterestPeriod; }
        }
        
        public string CommType
        {
            set { strCommType = value; }
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("StkbPGentableSelectCommType") as SqlCommand;
                strCommType = (string)db.ExecuteScalar(dbCommand).ToString();
                return strCommType;
            }
        }

        public Decimal NASDBSec
        {
            set { decNASDBSec = value; }
            get { return decNASDBSec; }
        }
        public Decimal NASDSSec
        {
            set { decNASDSSec = value; }
            get { return decNASDSSec; }
        }
        public Decimal NASDBCSCS
        {
            set { decNASDBCSCS = value; }
            get { return decNASDBCSCS; }
        }
        public Decimal NASDSCSCS
        {
            set { decNASDSCSCS = value; }
            get { return decNASDSCSCS; }
        }
        public Decimal NASDBComm
        {
            set { decNASDBComm = value; }
            get { return decNASDBComm; }
        }
        public Decimal NASDSComm
        {
            set { decNASDSComm = value; }
            get { return decNASDSComm; }
        }
        public decimal NASDBSMSCharge
        {
            set { decNASDBSMSCharge = value; }
            get { return decNASDBSMSCharge; }
        }
        public Decimal NASDSSMSCharge
        {
            set { decNASDSSMSCharge = value; }
            get { return decNASDSSMSCharge; }
        }
        public Decimal NASDSNASD
        {
            set { decNASDSNASD = value; }
            get { return decNASDSNASD; }
        }
        public Decimal NASDBNASD
        {
            set { decNASDBNASD = value; }
            get { return decNASDBNASD; }
        }
        public Decimal NASDBStamp
        {
            set { decNASDBStamp = value; }
            get { return decNASDBStamp; }
        }
        public Decimal NASDSStamp
        {
            set { decNASDSStamp = value; }
            get { return decNASDSStamp; }
        }
        public Decimal NASDBCommMin
        {
            set { decNASDBCommMin = value; }
            get { return decNASDBCommMin; }
        }
        public Decimal NASDSCommMin
        {
            set { decNASDSCommMin = value; }
            get { return decNASDSCommMin; }
        }
        public Decimal NASDBCommVAT
        {
            set { decNASDBCommVAT = value; }
            get { return decNASDBCommVAT; }
        }
        public Decimal NASDSCommVAT
        {
            set { decNASDSCommVAT = value; }
            get { return decNASDSCommVAT; }
        }
        public Decimal NASDBNASDVAT
        {
            set { decNASDBNASDVAT = value; }
            get { return decNASDBNASDVAT; }
        }
        public Decimal NASDSNASDVAT
        {
            set { decNASDSNASDVAT = value; }
            get { return decNASDSNASDVAT; }
        }
        public Decimal NASDBSECVAT
        {
            set { decNASDBSECVAT = value; }
            get { return decNASDBSECVAT; }
        }
        public Decimal NASDSSECVAT
        {
            set { decNASDSSECVAT = value; }
            get { return decNASDSSECVAT; }
        }
        public Decimal NASDBCSCSVAT
        {
            set { decNASDBCSCSVAT = value; }
            get { return decNASDBCSCSVAT; }
        }
        public Decimal NASDSCSCSVAT
        {
            set { decNASDSCSCSVAT = value; }
            get { return decNASDSCSCSVAT; }
        }
        public Decimal NASDBAlertVAT
        {
            set { decNASDBAlertVAT = value; }
            get { return decNASDBAlertVAT; }
        }
        public Decimal NASDSAlertVAT
        {
            set { decNASDSAlertVAT = value; }
            get { return decNASDSAlertVAT; }
        }
        public decimal BondSECBuy
        {
            set { decBondSECBuy = value; }
            get { return decBondSECBuy; }
        }
        public decimal BondSECSell
        {
            set { decBondSECSell = value; }
            get { return decBondSECSell; }
        }
        public decimal BondCommissionBuy
        {
            set { decBondCommissionBuy = value; }
            get { return decBondCommissionBuy; }
        }
        public decimal BondCommissionSell
        {
            set { decBondCommissionSell = value; }
            get { return decBondCommissionSell; }
        }
        public decimal BondNSEBuy
        {
            set { decBondNSEBuy = value; }
            get { return decBondNSEBuy; }
        }
        public decimal BondNSESell
        {
            set { decBondNSESell = value; }
            get { return decBondNSESell; }
        }
        public decimal BondCSCSBuy
        {
            set { decBondCSCSBuy = value; }
            get { return decBondCSCSBuy; }
        }
        public decimal BondCSCSSell
        {
            set { decBondCSCSSell = value; }
            get { return decBondCSCSSell; }
        }
        public decimal BondContractStampBuy
        {
            set { decBondContractStampBuy = value; }
            get { return decBondContractStampBuy; }
        }
        public decimal BondContractStampSell
        {
            set { decBondContractStampSell = value; }
            get { return decBondContractStampSell; }
        }
        public decimal BondSMSAlertBuy
        {
            set { decBondSMSAlertBuy = value; }
            get { return decBondSMSAlertBuy; }
        }
        public decimal BondSMSAlertSell
        {
            set { decBondSMSAlertSell = value; }
            get { return decBondSMSAlertSell; }
        }
        public decimal BondSMSAlertVATBuy
        {
            set { decBondSMSAlertVATBuy = value; }
            get { return decBondSMSAlertVATBuy; }
        }
        public decimal BondSMSAlertVATSell
        {
            set { decBondSMSAlertVATSell = value;}
            get { return decBondSMSAlertVATSell; }
        }
        public decimal BondCommissionMinimumBuy
        {
            set { decBondCommissionMinimumBuy = value; }
            get { return decBondCommissionMinimumBuy; }
        }
        public decimal BondCommissionMinimumSell
        {
            set { decBondCommissionMinimumSell = value; }
            get { return decBondCommissionMinimumSell; }
        }
        public DataGeneral.StockInstrumentType StockInstrument
        {
            set { datgenStockInstrument = value; }
            get { return datgenStockInstrument; }
        }

        public string NASDMemberCode
        {
            set { strNASDMemberCode = value; }
            get { return strNASDMemberCode; }
        }
        #endregion

        #region Add New StkbPGenTable
        public bool Add()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("StkbPGentableAdd") as SqlCommand;
            db.AddInParameter(oCommand, "BCsCs", SqlDbType.Decimal, decBCsCs);
            db.AddInParameter(oCommand, "Scscs", SqlDbType.Decimal, decScscs);
            db.AddInParameter(oCommand, "Bnse", SqlDbType.Decimal, decBnse);
            db.AddInParameter(oCommand, "Snse", SqlDbType.Decimal, decSnse);
            db.AddInParameter(oCommand, "Bstamp", SqlDbType.Decimal, decBstamp);
            db.AddInParameter(oCommand, "Sstamp", SqlDbType.Decimal, decSstamp);
            db.AddInParameter(oCommand, "Bvat", SqlDbType.Decimal, decBvat);
            db.AddInParameter(oCommand, "Svat", SqlDbType.Decimal, decSvat);
            db.AddInParameter(oCommand, "Bcncomm", SqlDbType.Decimal, decBcncomm);
            db.AddInParameter(oCommand, "Scncomm", SqlDbType.Decimal, decScncomm);
            db.AddInParameter(oCommand, "Bsec", SqlDbType.Decimal, decBsec);
            db.AddInParameter(oCommand, "Ssec", SqlDbType.Decimal, decSsec);
            db.AddInParameter(oCommand, "BothCharge", SqlDbType.Decimal, decBothCharge);
            db.AddInParameter(oCommand, "SothCharge", SqlDbType.Decimal, decSothCharge);
            db.AddInParameter(oCommand, "UpLoadDone", SqlDbType.VarChar, strUpLoadDone);
            db.AddInParameter(oCommand, "CustAcctUpLoadDone", SqlDbType.VarChar, strCustAcctUpLoadDone);
            db.AddInParameter(oCommand, "MinNSeB", SqlDbType.Decimal, decMinNSceB);
            db.AddInParameter(oCommand, "MinSecB", SqlDbType.Decimal, decMinSecB);
            db.AddInParameter(oCommand, "MinCscsB", SqlDbType.Decimal, decMinCscsB);
            db.AddInParameter(oCommand, "MinVatB", SqlDbType.Decimal, decMinVatB);
            db.AddInParameter(oCommand, "MinCommB", SqlDbType.Decimal, decMinCommB);
            db.AddInParameter(oCommand, "MinStampB", SqlDbType.Decimal, decMinStampB);
            db.AddInParameter(oCommand, "MinNseS", SqlDbType.Decimal, decMinSceS);
            db.AddInParameter(oCommand, "MinSecS", SqlDbType.Decimal, decMinSecS);
            db.AddInParameter(oCommand, "MinCscsS", SqlDbType.Decimal, decMinCscsS);
            db.AddInParameter(oCommand, "MinVatS", SqlDbType.Decimal, decMinVatS);
            db.AddInParameter(oCommand, "MinCommS", SqlDbType.Decimal, decMinCommS);
            db.AddInParameter(oCommand, "MinStampS", SqlDbType.Decimal, decMinStampS);
            db.AddInParameter(oCommand, "BcscsVat", SqlDbType.Decimal, decBcscsVat);
            db.AddInParameter(oCommand, "ScscsVat", SqlDbType.Decimal, decScscsVat);
            db.AddInParameter(oCommand, "BnseVat", SqlDbType.Decimal, decBnseVat);
            db.AddInParameter(oCommand, "SnseVat", SqlDbType.Decimal, decSnseVat);
            db.AddInParameter(oCommand, "BsecVat", SqlDbType.Decimal, decBsecVat);
            db.AddInParameter(oCommand, "SsecVat", SqlDbType.Decimal, decSsecVat);
            db.AddInParameter(oCommand, "MinCscsVATB", SqlDbType.Decimal, decMinCscsVATB);
            db.AddInParameter(oCommand, "MinCscsVatS", SqlDbType.Decimal, decMinCscsVatS);
            db.AddInParameter(oCommand, "MinNseVatB", SqlDbType.Decimal, decMinNseVatB);
            db.AddInParameter(oCommand, "MinNseVatS", SqlDbType.Decimal, decMinNseVatS);
            db.AddInParameter(oCommand, "MinSEcVatB", SqlDbType.Decimal, decMinSecVatB);
            db.AddInParameter(oCommand, "MinSecVatS", SqlDbType.Decimal, decMinSecVatS);
            db.AddInParameter(oCommand, "Allot", SqlDbType.Int, decSvat);
            db.AddInParameter(oCommand, "Stock", SqlDbType.Int, intStock);
            db.AddInParameter(oCommand, "SettleDay", SqlDbType.Int, decSvat);
            db.AddInParameter(oCommand, "InterestRate", SqlDbType.Decimal, decInterestRate);
            db.AddInParameter(oCommand, "InterestPeriod", SqlDbType.VarChar, strInterestPeriod);
            db.AddInParameter(oCommand, "CommType", SqlDbType.VarChar, strCommType.Trim());
            db.AddInParameter(oCommand, "DropJobOrderDayNumber", SqlDbType.Int, intDropJobOrderDayNumber);
            db.AddInParameter(oCommand, "OrderPriceMarkup", SqlDbType.Decimal, decOrderPriceMarkup);
            db.AddInParameter(oCommand, "SMSAlertCSCSB", SqlDbType.Money, decSMSAlertCSCSB);
            db.AddInParameter(oCommand, "SMSAlertCSCSS", SqlDbType.Money, decSMSAlertCSCSS);
            db.AddInParameter(oCommand, "SMSAlertCSCSVATB", SqlDbType.Decimal, decSMSAlertCSCSVATB);
            db.AddInParameter(oCommand, "SMSAlertCSCSVATS", SqlDbType.Decimal, decSMSAlertCSCSVATS);
            db.AddInParameter(oCommand, "NASDBSec", SqlDbType.Decimal, decNASDBSec);
            db.AddInParameter(oCommand, "NASDSSec", SqlDbType.Decimal, decNASDSSec);
            db.AddInParameter(oCommand, "NASDBCSCS", SqlDbType.Decimal, decNASDBCSCS);
            db.AddInParameter(oCommand, "NASDSCSCS", SqlDbType.Decimal, decNASDSCSCS);
            db.AddInParameter(oCommand, "NASDBComm", SqlDbType.Decimal, decNASDBComm);
            db.AddInParameter(oCommand, "NASDSComm", SqlDbType.Decimal, decNASDSComm);
            db.AddInParameter(oCommand, "NASDBSMSCharge", SqlDbType.Decimal, decNASDBSMSCharge);
            db.AddInParameter(oCommand, "NASDSSMSCharge", SqlDbType.Decimal, decNASDSSMSCharge);
            db.AddInParameter(oCommand, "NASDSNASD", SqlDbType.Decimal, decNASDSNASD);
            db.AddInParameter(oCommand, "NASDBNASD", SqlDbType.Decimal, decNASDBNASD);
            db.AddInParameter(oCommand, "NASDBSECVAT", SqlDbType.Decimal, decNASDBSECVAT);
            db.AddInParameter(oCommand, "NASDSSECVAT", SqlDbType.Decimal, decNASDSSECVAT);
            db.AddInParameter(oCommand, "NASDBCSCSVAT", SqlDbType.Decimal, decNASDBCSCSVAT);
            db.AddInParameter(oCommand, "NASDSCSCSVAT", SqlDbType.Decimal, decNASDSCSCSVAT);
            db.AddInParameter(oCommand, "NASDBCommVAT", SqlDbType.Decimal, decNASDBCommVAT);
            db.AddInParameter(oCommand, "NASDSCommVAT", SqlDbType.Decimal, decNASDSCommVAT);
            db.AddInParameter(oCommand, "NASDBAlertVAT", SqlDbType.Decimal, decNASDBAlertVAT);
            db.AddInParameter(oCommand, "NASDSAlertVAT", SqlDbType.Decimal, decNASDSAlertVAT);
            db.AddInParameter(oCommand, "NASDSNASDVAT", SqlDbType.Decimal, decNASDSNASDVAT);
            db.AddInParameter(oCommand, "NASDBNASDVAT", SqlDbType.Decimal, decNASDBNASDVAT);
            db.AddInParameter(oCommand, "NASDBStamp", SqlDbType.Decimal, decNASDBStamp);
            db.AddInParameter(oCommand, "NASDSStamp", SqlDbType.Decimal, decNASDSStamp);
            db.AddInParameter(oCommand, "NASDBCommMin", SqlDbType.Decimal, decNASDBCommMin);
            db.AddInParameter(oCommand, "NASDSCommMin", SqlDbType.Decimal, decNASDSCommMin);
            db.AddInParameter(oCommand, "BondSECBuy", SqlDbType.Decimal, decBondSECBuy);
            db.AddInParameter(oCommand, "BondCommissionBuy", SqlDbType.Decimal, decBondCommissionBuy);
            db.AddInParameter(oCommand, "BondNSEBuy", SqlDbType.Decimal, decBondNSEBuy);
            db.AddInParameter(oCommand, "BondCSCSBuy", SqlDbType.Decimal, decBondCSCSBuy);
            db.AddInParameter(oCommand, "BondContractStampBuy", SqlDbType.Decimal, decBondContractStampBuy);
            db.AddInParameter(oCommand, "BondSMSAlertBuy", SqlDbType.Decimal, decBondSMSAlertBuy);
            db.AddInParameter(oCommand, "BondSMSAlertVATBuy", SqlDbType.Decimal, decBondSMSAlertVATBuy);
            db.AddInParameter(oCommand, "BondSECSell", SqlDbType.Decimal, decBondSECSell);
            db.AddInParameter(oCommand, "BondCommissionSell", SqlDbType.Decimal, decBondCommissionSell);
            db.AddInParameter(oCommand, "BondNSESell", SqlDbType.Decimal, decBondNSESell);
            db.AddInParameter(oCommand, "BondCSCSSell", SqlDbType.Decimal, decBondCSCSSell);
            db.AddInParameter(oCommand, "BondContractStampSell", SqlDbType.Decimal, decBondContractStampSell);
            db.AddInParameter(oCommand, "BondSMSAlertSell", SqlDbType.Decimal, decBondSMSAlertSell);
            db.AddInParameter(oCommand, "BondSMSAlertVATSell", SqlDbType.Decimal, decBondSMSAlertVATSell);
            db.AddInParameter(oCommand, "NASDMemberCode", SqlDbType.VarChar, strNASDMemberCode);
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
            
        }
        #endregion

        #region Get StkPGenTable Info
        public bool GetStkbPGenTable()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("StkPGenTableSelectByAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                if (thisRow[0]["BCsCs"].ToString() == "")
                {
                    decBCsCs = 0;
                }
                else
                {
                    decBCsCs = decimal.Parse(thisRow[0]["BCsCs"].ToString());
                }
                if (thisRow[0]["Scscs"].ToString() == "")
                {
                    decScscs = 0;
                }
                else
                {
                    decScscs = decimal.Parse(thisRow[0]["Scscs"].ToString());
                }
                if (thisRow[0]["Bnse"].ToString() == "")
                {
                    decBnse = 0;
                }
                else
                {
                    decBnse = decimal.Parse(thisRow[0]["Bnse"].ToString());
                }
                if (thisRow[0]["Snse"].ToString() == "")
                {
                    decSnse = 0;
                }
                else
                {
                    decSnse = decimal.Parse(thisRow[0]["Snse"].ToString());
                }
                if (thisRow[0]["Bstamp"].ToString() == "")
                {
                    decBstamp = 0;
                }
                else
                {
                    decBstamp = decimal.Parse(thisRow[0]["Bstamp"].ToString());
                }
                if (thisRow[0]["Sstamp"].ToString() == "")
                {
                    decSstamp = 0;
                }
                else
                {
                    decSstamp = decimal.Parse(thisRow[0]["Sstamp"].ToString());
                }
                if (thisRow[0]["Bvat"].ToString() == "")
                {
                    decBvat = 0;
                }
                else
                {
                    decBvat = decimal.Parse(thisRow[0]["Bvat"].ToString());
                }
                if (thisRow[0]["Svat"].ToString() == "")
                {
                    decSvat = 0;
                }
                else
                {
                    decSvat = decimal.Parse(thisRow[0]["Svat"].ToString());
                }
                if (thisRow[0]["Bcncomm"].ToString() == "")
                {
                    decBcncomm = 0;
                }
                else
                {
                    decBcncomm = decimal.Parse(thisRow[0]["Bcncomm"].ToString());
                }
                if (thisRow[0]["Scncomm"].ToString() == "")
                {
                    decScncomm = 0;
                }
                else
                {
                    decScncomm = decimal.Parse(thisRow[0]["Scncomm"].ToString());
                }
                if (thisRow[0]["Bsec"].ToString() == "")
                {
                    decBsec = 0;
                }
                else
                {
                    decBsec = decimal.Parse(thisRow[0]["Bsec"].ToString());
                }
                if (thisRow[0]["Ssec"].ToString() == "")
                {
                    decSsec = 0;
                }
                else
                {
                    decSsec = decimal.Parse(thisRow[0]["Ssec"].ToString());
                }

                if (thisRow[0]["BothCharge"].ToString() == "")
                {
                    decBothCharge = 0;
                }
                else
                {
                    decBothCharge = decimal.Parse(thisRow[0]["BothCharge"].ToString());
                }
                if (thisRow[0]["SothCharge"].ToString() == "")
                {
                    decSothCharge = 0;
                }
                else
                {
                    decSothCharge = decimal.Parse(thisRow[0]["SothCharge"].ToString());
                }
                strUpLoadDone = thisRow[0]["UpLoadDone"].ToString();
                strCustAcctUpLoadDone = thisRow[0]["CustAcctUpLoadDone"].ToString();
                if (thisRow[0]["MinSecB"].ToString() == "")
                {
                    decMinSecB = 0;
                }
                else
                {
                    decMinSecB = decimal.Parse(thisRow[0]["MinSecB"].ToString());
                }
                if (thisRow[0]["MinVatB"].ToString() == "")
                {
                    decMinVatB = 0;
                }
                else
                {
                    decMinVatB = decimal.Parse(thisRow[0]["MinVatB"].ToString());
                }
                if (thisRow[0]["MinCommB"].ToString() == "")
                {
                    decMinCommB = 0;
                }
                else
                {
                    decMinCommB = decimal.Parse(thisRow[0]["MinCommB"].ToString());
                }
                if (thisRow[0]["MinNSeB"].ToString() == "")
                {
                    decMinNSceB = 0;
                }
                else
                {
                    decMinNSceB = decimal.Parse(thisRow[0]["MinNSeB"].ToString());
                }
                if (thisRow[0]["MinCscsB"].ToString() == "")
                {
                    decMinCscsB = 0;
                }
                else
                {
                    decMinCscsB = decimal.Parse(thisRow[0]["MinCscsB"].ToString());
                }
                if (thisRow[0]["MinStampB"].ToString() == "")
                {
                    decMinStampB = 0;
                }
                else
                {
                    decMinStampB = decimal.Parse(thisRow[0]["MinStampB"].ToString());
                }
                if (thisRow[0]["MinSecS"].ToString() == "")
                {
                    decMinSecS = 0;
                }
                else
                {
                    decMinSecS = decimal.Parse(thisRow[0]["MinSecS"].ToString());
                }
                if (thisRow[0]["MinVatS"].ToString() == "")
                {
                    decMinVatS = 0;
                }
                else
                {
                    decMinVatS = decimal.Parse(thisRow[0]["MinVatS"].ToString());
                }
                if (thisRow[0]["MinCommS"].ToString() == "")
                {
                    decMinCommS = 0;
                }
                else
                {
                    decMinCommS = decimal.Parse(thisRow[0]["MinCommS"].ToString());
                }
                if (thisRow[0]["MinNseS"].ToString() == "")
                {
                    decMinSceS = 0;
                }
                else
                {
                    decMinSceS = decimal.Parse(thisRow[0]["MinNseS"].ToString());
                }
                if (thisRow[0]["MinCscsS"].ToString() == "")
                {
                    decMinCscsS = 0;
                }
                else
                {
                    decMinCscsS = decimal.Parse(thisRow[0]["MinCscsS"].ToString());
                }
                if (thisRow[0]["MinStampS"].ToString() == "")
                {
                    decMinStampS = 0;
                }
                else
                {
                    decMinStampS = decimal.Parse(thisRow[0]["MinStampS"].ToString());
                }
                
                strVatComm = thisRow[0]["VatComm"].ToString();
                strStampVat = thisRow[0]["StampVat"].ToString();
                if (thisRow[0]["EODDate"].ToString() == "")
                {
                    datEODDate = DateTime.MinValue;
                }
                else
                {
                    datEODDate = DateTime.Parse(thisRow[0]["EODDate"].ToString());
                }
                if (thisRow[0]["Zero"].ToString() == "")
                {
                    intZero = 0;
                }
                else
                {
                    intZero = int.Parse(thisRow[0]["Zero"].ToString());
                }
                if (thisRow[0]["EOMDate"].ToString() == "")
                {
                    datEOMDate = DateTime.MinValue;
                }
                else
                {
                    datEOMDate = DateTime.Parse(thisRow[0]["EOMDate"].ToString());
                }
                if (thisRow[0]["BcscsVat"].ToString() == "")
                {
                    decBcscsVat = 0;
                }
                else
                {
                    decBcscsVat = decimal.Parse(thisRow[0]["BcscsVat"].ToString());
                }
                if (thisRow[0]["ScscsVat"].ToString() == "")
                {
                    decScscsVat = 0;
                }
                else
                {
                    decScscsVat = decimal.Parse(thisRow[0]["ScscsVat"].ToString());
                }
                if (thisRow[0]["BnseVat"].ToString() == "")
                {
                    decBnseVat = 0;
                }
                else
                {
                    decBnseVat = decimal.Parse(thisRow[0]["BnseVat"].ToString());
                }
                if (thisRow[0]["SnseVat"].ToString() == "")
                {
                    decSnseVat = 0;
                }
                else
                {
                    decSnseVat = decimal.Parse(thisRow[0]["SnseVat"].ToString());
                }
                if (thisRow[0]["BsecVat"].ToString() == "")
                {
                    decBsecVat = 0;
                }
                else
                {
                    decBsecVat = decimal.Parse(thisRow[0]["BsecVat"].ToString());
                }
                if (thisRow[0]["SsecVat"].ToString() == "")
                {
                    decSsecVat = 0;
                }
                else
                {
                    decSsecVat = decimal.Parse(thisRow[0]["SsecVat"].ToString());
                }
                if (thisRow[0]["MinCscsVATB"].ToString() == "")
                {
                    decMinCscsVATB = 0;
                }
                else
                {
                    decMinCscsVATB = decimal.Parse(thisRow[0]["MinCscsVATB"].ToString());
                }
                if (thisRow[0]["MinCscsVatS"].ToString() == "")
                {
                    decMinCscsVatS = 0;
                }
                else
                {
                    decMinCscsVatS = decimal.Parse(thisRow[0]["MinCscsVatS"].ToString());
                }
                if (thisRow[0]["MinNseVatB"].ToString() == "")
                {
                    decMinNseVatB = 0;
                }
                else
                {
                    decMinNseVatB = decimal.Parse(thisRow[0]["MinNseVatB"].ToString());
                }
                if (thisRow[0]["MinNseVatS"].ToString() == "")
                {
                    decMinNseVatS = 0;
                }
                else
                {
                    decMinNseVatS = decimal.Parse(thisRow[0]["MinNseVatS"].ToString());
                }
                if (thisRow[0]["MinSEcVatS"].ToString() == "")
                {
                    decMinSecVatS = 0;
                }
                else
                {
                    decMinSecVatS = decimal.Parse(thisRow[0]["MinSEcVatS"].ToString());
                }
                if (thisRow[0]["MinSecVatB"].ToString() == "")
                {
                    decMinSecVatB = 0;
                }
                else
                {
                    decMinSecVatB = decimal.Parse(thisRow[0]["MinSecVatB"].ToString());
                }

                if (thisRow[0]["Allot"].ToString() == "")
                {
                    intAllot = 0;
                }
                else
                {
                    intAllot = int.Parse(thisRow[0]["Allot"].ToString());
                }
                if (thisRow[0]["Stock"].ToString() == "")
                {
                    intStock = 0;
                }
                else
                {
                    intStock = int.Parse(thisRow[0]["Stock"].ToString());
                }
                if (thisRow[0]["SettleDay"].ToString() == "")
                {
                    intSettleDay = 0;
                }
                else
                {
                    intSettleDay = int.Parse(thisRow[0]["SettleDay"].ToString());
                }
                if (thisRow[0]["InterestRate"].ToString() == "")
                {
                    decInterestRate = 0;
                }
                else
                {
                    decInterestRate = decimal.Parse(thisRow[0]["InterestRate"].ToString());
                }
                strInterestPeriod = thisRow[0]["InterestPeriod"].ToString();
                strCommType = thisRow[0]["CommType"].ToString();
                if (thisRow[0]["SMSAlertCSCSB"].ToString() == "")
                {
                    decSMSAlertCSCSB = 0;
                }
                else
                {
                    decSMSAlertCSCSB = decimal.Parse(thisRow[0]["SMSAlertCSCSB"].ToString());
                }
                if (thisRow[0]["SMSAlertCSCSS"].ToString() == "")
                {
                    decSMSAlertCSCSS = 0;
                }
                else
                {
                    decSMSAlertCSCSS = decimal.Parse(thisRow[0]["SMSAlertCSCSS"].ToString());
                }
                if (thisRow[0]["SMSAlertCSCSVATB"].ToString() == "")
                {
                    decSMSAlertCSCSVATB = 0;
                }
                else
                {
                    decSMSAlertCSCSVATB = decimal.Parse(thisRow[0]["SMSAlertCSCSVATB"].ToString());
                }
                if (thisRow[0]["SMSAlertCSCSVATS"].ToString() == "")
                {
                    decSMSAlertCSCSVATS = 0;
                }
                else
                {
                    decSMSAlertCSCSVATS = decimal.Parse(thisRow[0]["SMSAlertCSCSVATS"].ToString());
                }
                intDropJobOrderDayNumber = thisRow[0]["DropJobOrderDayNumber"] != null && thisRow[0]["DropJobOrderDayNumber"].ToString().Trim() != "" ? int.Parse(thisRow[0]["DropJobOrderDayNumber"].ToString()) : 0;
                decOrderPriceMarkup = thisRow[0]["OrderPriceMarkup"] != null && thisRow[0]["OrderPriceMarkup"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["OrderPriceMarkup"].ToString()) : 0;
                decNASDBSec = thisRow[0]["NASDBSec"] != null && thisRow[0]["NASDBSec"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["NASDBSec"].ToString()) : 0;
                decNASDSSec = thisRow[0]["NASDSSec"] != null && thisRow[0]["NASDSSec"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["NASDSSec"].ToString()) : 0;
                decNASDBCSCS = thisRow[0]["NASDBCSCS"] != null && thisRow[0]["NASDBCSCS"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["NASDBCSCS"].ToString()) : 0;
                decNASDSCSCS = thisRow[0]["NASDSCSCS"] != null && thisRow[0]["NASDSCSCS"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["NASDSCSCS"].ToString()) : 0;
                decNASDBComm = thisRow[0]["NASDBComm"] != null && thisRow[0]["NASDBComm"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["NASDBComm"].ToString()) : 0;
                decNASDSComm = thisRow[0]["NASDSComm"] != null && thisRow[0]["NASDSComm"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["NASDSComm"].ToString()) : 0;
                decNASDBSMSCharge = thisRow[0]["NASDBSMSCharge"] != null && thisRow[0]["NASDBSMSCharge"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["NASDBSMSCharge"].ToString()) : 0;
                decNASDSSMSCharge = thisRow[0]["NASDSSMSCharge"] != null && thisRow[0]["NASDSSMSCharge"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["NASDSSMSCharge"].ToString()) : 0;
                decNASDSNASD = thisRow[0]["NASDSNASD"] != null && thisRow[0]["NASDSNASD"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["NASDSNASD"].ToString()) : 0;
                decNASDBNASD = thisRow[0]["NASDBNASD"] != null && thisRow[0]["NASDBNASD"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["NASDBNASD"].ToString()) : 0;
                decNASDBSECVAT = thisRow[0]["NASDBSECVAT"] != null && thisRow[0]["NASDBSECVAT"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["NASDBSECVAT"].ToString()) : 0;
                decNASDSSECVAT = thisRow[0]["NASDSSECVAT"] != null && thisRow[0]["NASDSSECVAT"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["NASDSSECVAT"].ToString()) : 0;
                decNASDBCSCSVAT = thisRow[0]["NASDBCSCSVAT"] != null && thisRow[0]["NASDBCSCSVAT"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["NASDBCSCSVAT"].ToString()) : 0;
                decNASDSCSCSVAT = thisRow[0]["NASDSCSCSVAT"] != null && thisRow[0]["NASDSCSCSVAT"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["NASDSCSCSVAT"].ToString()) : 0;
                decNASDBCommVAT = thisRow[0]["NASDBCommVAT"] != null && thisRow[0]["NASDBCommVAT"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["NASDBCommVAT"].ToString()) : 0;
                decNASDSCommVAT = thisRow[0]["NASDSCommVAT"] != null && thisRow[0]["NASDSCommVAT"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["NASDSCommVAT"].ToString()) : 0;
                decNASDBAlertVAT = thisRow[0]["NASDBAlertVAT"] != null && thisRow[0]["NASDBAlertVAT"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["NASDBAlertVAT"].ToString()) : 0;
                decNASDSAlertVAT = thisRow[0]["NASDSAlertVAT"] != null && thisRow[0]["NASDSAlertVAT"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["NASDSAlertVAT"].ToString()) : 0;
                decNASDSNASDVAT = thisRow[0]["NASDSNASDVAT"] != null && thisRow[0]["NASDSNASDVAT"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["NASDSNASDVAT"].ToString()) : 0;
                decNASDBNASDVAT = thisRow[0]["NASDBNASDVAT"] != null && thisRow[0]["NASDBNASDVAT"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["NASDBNASDVAT"].ToString()) : 0;
                decNASDBStamp = thisRow[0]["NASDBStamp"] != null && thisRow[0]["NASDBStamp"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["NASDBStamp"].ToString()) : 0;
                decNASDSStamp = thisRow[0]["NASDSStamp"] != null && thisRow[0]["NASDSStamp"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["NASDSStamp"].ToString()) : 0;
                decNASDBCommMin = thisRow[0]["NASDBCommMin"] != null && thisRow[0]["NASDBCommMin"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["NASDBCommMin"].ToString()) : 0;
                decNASDSCommMin = thisRow[0]["NASDSCommMin"] != null && thisRow[0]["NASDSCommMin"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["NASDSCommMin"].ToString()) : 0;

                decBondSECBuy = thisRow[0]["BondSECBuy"] != null && thisRow[0]["BondSECBuy"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["BondSECBuy"].ToString()) : 0;
                decBondSECSell = thisRow[0]["BondSECBuy"] != null && thisRow[0]["BondSECBuy"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["BondSECBuy"].ToString()) : 0;
                decBondCSCSBuy = thisRow[0]["BondCSCSBuy"] != null && thisRow[0]["BondCSCSBuy"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["BondCSCSBuy"].ToString()) : 0;
                decBondCSCSSell = thisRow[0]["BondCSCSSell"] != null && thisRow[0]["BondCSCSSell"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["BondCSCSSell"].ToString()) : 0;
                decBondCommissionBuy = thisRow[0]["BondCommissionBuy"] != null && thisRow[0]["BondCommissionBuy"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["BondCommissionBuy"].ToString()) : 0;
                decBondCommissionSell = thisRow[0]["BondCommissionSell"] != null && thisRow[0]["BondCommissionSell"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["BondCommissionSell"].ToString()) : 0;
                decBondSMSAlertBuy = thisRow[0]["BondSMSAlertBuy"] != null && thisRow[0]["BondSMSAlertBuy"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["BondSMSAlertBuy"].ToString()) : 0;
                decBondSMSAlertSell = thisRow[0]["BondSMSAlertSell"] != null && thisRow[0]["BondSMSAlertSell"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["BondSMSAlertSell"].ToString()) : 0;
                decBondSMSAlertVATBuy = thisRow[0]["BondSMSAlertVATBuy"] != null && thisRow[0]["BondSMSAlertVATBuy"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["BondSMSAlertVATBuy"].ToString()) : 0;
                decBondSMSAlertVATSell = thisRow[0]["BondSMSAlertVATSell"] != null && thisRow[0]["BondSMSAlertVATSell"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["BondSMSAlertVATSell"].ToString()) : 0;
                decBondNSEBuy = thisRow[0]["BondNSEBuy"] != null && thisRow[0]["BondNSEBuy"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["BondNSEBuy"].ToString()) : 0;
                decBondNSESell = thisRow[0]["BondNSESell"] != null && thisRow[0]["BondNSESell"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["BondNSESell"].ToString()) : 0;
                decBondContractStampBuy = thisRow[0]["BondContractStampBuy"] != null && thisRow[0]["BondContractStampBuy"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["BondContractStampBuy"].ToString()) : 0;
                decBondContractStampSell = thisRow[0]["BondContractStampSell"] != null && thisRow[0]["BondContractStampSell"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["BondContractStampSell"].ToString()) : 0;
                //decBondCommissionMinimumBuy = thisRow[0]["BondCommissionMinimumBuy"] != null && thisRow[0]["BondCommissionMinimumBuy"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["BondCommissionMinimumBuy"].ToString()) : 0;
                //decBondCommissionMinimumSell = thisRow[0]["BondCommissionMinimumSell"] != null && thisRow[0]["BondCommissionMinimumSell"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["BondCommissionMinimumSell"].ToString()) : 0;
                strNASDMemberCode = thisRow[0]["NASDMemberCode"] != null ? thisRow[0]["NASDMemberCode"].ToString() : "";

                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get Sale Fee
        public decimal GetSaleFee()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("StkPGenTableSelectByAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                
                if (thisRow[0]["Scscs"].ToString() == "")
                {
                    decScscs = 0;
                }
                else
                {
                    decScscs = decimal.Parse(thisRow[0]["Scscs"].ToString());
                }
                
                if (thisRow[0]["Snse"].ToString() == "")
                {
                    decSnse = 0;
                }
                else
                {
                    decSnse = decimal.Parse(thisRow[0]["Snse"].ToString());
                }
                
                if (thisRow[0]["Sstamp"].ToString() == "")
                {
                    decSstamp = 0;
                }
                else
                {
                    decSstamp = decimal.Parse(thisRow[0]["Sstamp"].ToString());
                }
                
                if (thisRow[0]["Svat"].ToString() == "")
                {
                    decSvat = 0;
                }
                else
                {
                    decSvat = decimal.Parse(thisRow[0]["Svat"].ToString());
                }
                if (thisRow[0]["Scncomm"].ToString() == "")
                {
                    decScncomm = 0;
                }
                else
                {
                    decScncomm = decimal.Parse(thisRow[0]["Scncomm"].ToString());
                }
                
                if (thisRow[0]["Ssec"].ToString() == "")
                {
                    decSsec = 0;
                }
                else
                {
                    decSsec = decimal.Parse(thisRow[0]["Ssec"].ToString());
                }                
                if (thisRow[0]["ScscsVat"].ToString() == "")
                {
                    decScscsVat = 0;
                }
                else
                {
                    decScscsVat = decimal.Parse(thisRow[0]["ScscsVat"].ToString());
                }
                
                if (thisRow[0]["SnseVat"].ToString() == "")
                {
                    decSnseVat = 0;
                }
                else
                {
                    decSnseVat = decimal.Parse(thisRow[0]["SnseVat"].ToString());
                }
                
                if (thisRow[0]["SsecVat"].ToString() == "")
                {
                    decSsecVat = 0;
                }
                else
                {
                    decSsecVat = decimal.Parse(thisRow[0]["SsecVat"].ToString());
                }
                
                if (thisRow[0]["SMSAlertCSCSS"].ToString() == "")
                {
                    decSMSAlertCSCSS = 0;
                }
                else
                {
                    decSMSAlertCSCSS = decimal.Parse(thisRow[0]["SMSAlertCSCSS"].ToString());
                }
                
                if (thisRow[0]["SMSAlertCSCSVATS"].ToString() == "")
                {
                    decSMSAlertCSCSVATS = 0;
                }
                else
                {
                    decSMSAlertCSCSVATS = decimal.Parse(thisRow[0]["SMSAlertCSCSVATS"].ToString());
                }
                return decScscs + decSnse + decSstamp + decScncomm + decSsec +
                        ((decScscsVat * decScscs) / 100) + ((decSnseVat * decSnse) / 100) + ((decSvat * decScncomm) / 100) + ((decSsecVat * decSvat) / 100);
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get All Fee
        public decimal GetAllFee()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("StkPGenTableSelectByAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {

                if (thisRow[0]["BCsCs"].ToString() == "")
                {
                    decBCsCs = 0;
                }
                else
                {
                    decBCsCs = decimal.Parse(thisRow[0]["BCsCs"].ToString());
                }
                if (thisRow[0]["Bnse"].ToString() == "")
                {
                    decBnse = 0;
                }
                else
                {
                    decBnse = decimal.Parse(thisRow[0]["Bnse"].ToString());
                }
                if (thisRow[0]["Bstamp"].ToString() == "")
                {
                    decBstamp = 0;
                }
                else
                {
                    decBstamp = decimal.Parse(thisRow[0]["Bstamp"].ToString());
                }
                if (thisRow[0]["Bvat"].ToString() == "")
                {
                    decBvat = 0;
                }
                else
                {
                    decBvat = decimal.Parse(thisRow[0]["Bvat"].ToString());
                }
                if (thisRow[0]["Bcncomm"].ToString() == "")
                {
                    decBcncomm = 0;
                }
                else
                {
                    decBcncomm = decimal.Parse(thisRow[0]["Bcncomm"].ToString());
                }
                if (thisRow[0]["Bsec"].ToString() == "")
                {
                    decBsec = 0;
                }
                else
                {
                    decBsec = decimal.Parse(thisRow[0]["Bsec"].ToString());
                }
                if (thisRow[0]["BcscsVat"].ToString() == "")
                {
                    decBcscsVat = 0;
                }
                else
                {
                    decBcscsVat = decimal.Parse(thisRow[0]["BcscsVat"].ToString());
                }
                if (thisRow[0]["BnseVat"].ToString() == "")
                {
                    decBnseVat = 0;
                }
                else
                {
                    decBnseVat = decimal.Parse(thisRow[0]["BnseVat"].ToString());
                }
                if (thisRow[0]["BsecVat"].ToString() == "")
                {
                    decBsecVat = 0;
                }
                else
                {
                    decBsecVat = decimal.Parse(thisRow[0]["BsecVat"].ToString());
                }
                if (thisRow[0]["SMSAlertCSCSB"].ToString() == "")
                {
                    decSMSAlertCSCSB = 0;
                }
                else
                {
                    decSMSAlertCSCSB = decimal.Parse(thisRow[0]["SMSAlertCSCSB"].ToString());
                }
                if (thisRow[0]["SMSAlertCSCSVATB"].ToString() == "")
                {
                    decSMSAlertCSCSVATB = 0;
                }
                else
                {
                    decSMSAlertCSCSVATB = decimal.Parse(thisRow[0]["SMSAlertCSCSVATB"].ToString());
                }
               






                if (thisRow[0]["Scscs"].ToString() == "")
                {
                    decScscs = 0;
                }
                else
                {
                    decScscs = decimal.Parse(thisRow[0]["Scscs"].ToString());
                }

                if (thisRow[0]["Snse"].ToString() == "")
                {
                    decSnse = 0;
                }
                else
                {
                    decSnse = decimal.Parse(thisRow[0]["Snse"].ToString());
                }

                if (thisRow[0]["Sstamp"].ToString() == "")
                {
                    decSstamp = 0;
                }
                else
                {
                    decSstamp = decimal.Parse(thisRow[0]["Sstamp"].ToString());
                }

                if (thisRow[0]["Svat"].ToString() == "")
                {
                    decSvat = 0;
                }
                else
                {
                    decSvat = decimal.Parse(thisRow[0]["Svat"].ToString());
                }
                if (thisRow[0]["Scncomm"].ToString() == "")
                {
                    decScncomm = 0;
                }
                else
                {
                    decScncomm = decimal.Parse(thisRow[0]["Scncomm"].ToString());
                }

                if (thisRow[0]["Ssec"].ToString() == "")
                {
                    decSsec = 0;
                }
                else
                {
                    decSsec = decimal.Parse(thisRow[0]["Ssec"].ToString());
                }
                if (thisRow[0]["ScscsVat"].ToString() == "")
                {
                    decScscsVat = 0;
                }
                else
                {
                    decScscsVat = decimal.Parse(thisRow[0]["ScscsVat"].ToString());
                }

                if (thisRow[0]["SnseVat"].ToString() == "")
                {
                    decSnseVat = 0;
                }
                else
                {
                    decSnseVat = decimal.Parse(thisRow[0]["SnseVat"].ToString());
                }

                if (thisRow[0]["SsecVat"].ToString() == "")
                {
                    decSsecVat = 0;
                }
                else
                {
                    decSsecVat = decimal.Parse(thisRow[0]["SsecVat"].ToString());
                }

                if (thisRow[0]["SMSAlertCSCSS"].ToString() == "")
                {
                    decSMSAlertCSCSS = 0;
                }
                else
                {
                    decSMSAlertCSCSS = decimal.Parse(thisRow[0]["SMSAlertCSCSS"].ToString());
                }

                if (thisRow[0]["SMSAlertCSCSVATS"].ToString() == "")
                {
                    decSMSAlertCSCSVATS = 0;
                }
                else
                {
                    decSMSAlertCSCSVATS = decimal.Parse(thisRow[0]["SMSAlertCSCSVATS"].ToString());
                }
                return decScscs + decSnse + decSstamp  + decScncomm + decSsec +
                       ((decScscsVat * decScscs) / 100) + ((decSnseVat * decSnse) / 100) + ((decSvat * decScncomm) / 100) + ((decSsecVat * decSvat) / 100) +
                       decBCsCs + decBnse + decBstamp  + decBcncomm + decBsec +
                       ((decBcscsVat * decBCsCs) / 100) + ((decBnseVat * decBnse) / 100) + ((decBvat * decBcncomm) / 100) + ((decBsecVat * decBvat) / 100);
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Sale SMS Amount Fee
        public decimal GetSaleSMSAmountFee(int intNumberOfTransaction)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("StkPGenTableSelectByAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                if (thisRow[0]["SMSAlertCSCSS"].ToString() == "")
                {
                    decSMSAlertCSCSS = 0;
                }
                else
                {
                    decSMSAlertCSCSS = decimal.Parse(thisRow[0]["SMSAlertCSCSS"].ToString());
                }

                if (thisRow[0]["SMSAlertCSCSVATS"].ToString() == "")
                {
                    decSMSAlertCSCSVATS = 0;
                }
                else
                {
                    decSMSAlertCSCSVATS = decimal.Parse(thisRow[0]["SMSAlertCSCSVATS"].ToString());
                }
                return (decSMSAlertCSCSS * intNumberOfTransaction) + (((decSMSAlertCSCSS * intNumberOfTransaction) * decSMSAlertCSCSVATS) / 100);
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Order Price Markup
        public decimal GetOrderPriceMarkup()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("StkPGenTableSelectOrderPriceMarkup") as SqlCommand;
            var varOrderPriceMarkup = db.ExecuteScalar(dbCommand);
            if (varOrderPriceMarkup != null && varOrderPriceMarkup.ToString().Trim() != "")
            {
                return Convert.ToDecimal(varOrderPriceMarkup);
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Purchase Fee
        public decimal GetPurchaseFee()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("StkPGenTableSelectByAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                decBCsCs = thisRow[0]["Bcscs"] != null && thisRow[0]["Bcscs"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["Bcscs"].ToString()) : 0;
                decBnse = thisRow[0]["Bnse"] != null && thisRow[0]["Bnse"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["Bnse"].ToString()) : 0;
                decBstamp = thisRow[0]["Bstamp"] != null && thisRow[0]["Bstamp"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["Bstamp"].ToString()) : 0;
                decBvat = thisRow[0]["Bvat"] != null && thisRow[0]["Bvat"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["Bvat"].ToString()) : 0;
                decBcncomm = thisRow[0]["Bcncomm"] != null && thisRow[0]["Bcncomm"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["Bcncomm"].ToString()) : 0;
                decBsec = thisRow[0]["Bsec"] != null && thisRow[0]["Bsec"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["Bsec"].ToString()) : 0;
                decBcscsVat = thisRow[0]["BcscsVat"] != null && thisRow[0]["BcscsVat"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["BcscsVat"].ToString()) : 0;
                decBnseVat = thisRow[0]["BnseVat"] != null && thisRow[0]["BnseVat"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["BnseVat"].ToString()) : 0;
                decBsecVat = thisRow[0]["BsecVat"] != null && thisRow[0]["BsecVat"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["BsecVat"].ToString()) : 0;
                decSMSAlertCSCSB = thisRow[0]["SMSAlertCSCSB"] != null && thisRow[0]["SMSAlertCSCSB"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["SMSAlertCSCSB"].ToString()) : 0;
                decSMSAlertCSCSVATB = thisRow[0]["SMSAlertCSCSVATB"] != null && thisRow[0]["SMSAlertCSCSVATB"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["SMSAlertCSCSVATB"].ToString()) : 0;

                return decBCsCs + decBnse + decBstamp + decBcncomm + decBsec +
                        ((decBcscsVat * decBCsCs) / 100) + ((decBnseVat * decBnse) / 100) + ((decBvat * decBcncomm) / 100) + ((decBsecVat * decBvat) / 100);

            }
            else
            {
                return 0;
            }
        }
        #endregion

    }
}
