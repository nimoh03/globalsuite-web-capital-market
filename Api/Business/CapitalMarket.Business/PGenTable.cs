using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace CapitalMarket.Business
{
    public class PGenTable
    {
        #region Declaration
        private string strBcscs, strScscs, strBnse, strSnse;
        private string strBstamp, strSstamp, strBvat, strSvat;
        private string strBcncomm, strScncomm, strBsec, strSsec;
        private string strTradeBank, strNASDTradingBank, strDirectCashSettleAcct, strTradestock;
        private string strRecievable, strPayable;
        private string strCapGain, strOffer,strBondOffer;
        private string strShInv,strXHeader, strXFooter;
        private string strSuspense;
        private string strOffBank;
        public string strStkAcct, strBrokPay, strInvestment, strNASDAccount,strInvestmentNASDAC, strInvestmentBondAC;

        private string strReserve;
        private Int32 intEomGrace;
        private string strOverDrawn,strStatutoryControl, strAgComm;
        private decimal decMinBalance;
        private Int32 intAccessNo, intAccessMax;
        private string strMagTrad;
        private string strEODRunFlag;
        private string strVerification;
        private decimal decVeriFeeBranch, decVeriFeeHead;
        private string strEOYRunBefore, strTradeBankBank, strCapGainContra;
        private string strDefAcctRep, strDefMDName;
        private string strNASDBSec, strNASDSSec, strNASDBCSCS, strNASDSCSCS, strNASDBComm, strNASDSComm,strNASDSNASD, strNASDBNASD,strNASDBStamp, strNASDSStamp, strNASDBVAT, strNASDSVAT;
        public string PropTradeBank { set; get; }
        #endregion

        #region Properties

        public string Bcscs
        {
            set { strBcscs = value; }
            get { return strBcscs; }
        }

        public string Scscs
        {
            set { strScscs = value; }
            get { return strScscs; }
        }

        public string Bnse
        {
            set { strBnse = value; }
            get { return strBnse; }
        }

        public string Snse
        {
            set { strSnse = value; }
            get { return strSnse; }
        }

        public string Bstamp
        {
            set { strBstamp = value; }
            get { return strBstamp; }
        }

        public string Sstamp
        {
            set { strSstamp = value; }
            get { return strSstamp; }
        }

        public string Bvat
        {
            set { strBvat = value; }
            get { return strBvat; }
        }

        public string Svat
        {
            set { strSvat = value; }
            get { return strSvat; }
        }

        public string Bcncomm
        {
            set { strBcncomm = value; }
            get { return strBcncomm; }
        }

        public string Scncomm
        {
            set { strScncomm = value; }
            get { return strScncomm; }
        }

        public string Bsec
        {
            set { strBsec = value; }
            get { return strBsec; }
        }

        public string Ssec
        {
            set { strSsec = value; }
            get { return strSsec; }
        }

        public string TradeBank
        {
            set { strTradeBank = value; }
            get { return strTradeBank; }
        }

        public string CapGainContra
        {
            set { strCapGainContra = value; }
            get { return strCapGainContra; }
        }
        
        public string NASDTradingBank
        {
            set { strNASDTradingBank = value; }
            get { return strNASDTradingBank; }
        }

        public string DirectCashSettleAcct
        {
            set { strDirectCashSettleAcct = value; }
            get { return strDirectCashSettleAcct; }
        }
        
        public string Tradestock
        {
            set { strTradestock = value; }
            get { return strTradestock; }
        }

        public string Recievable
        {
            set { strRecievable = value; }
            get { return strRecievable; }
        }

        public string Payable
        {
            set { strPayable = value; }
            get { return strPayable; }
        }

        public string CapGain
        {
            set { strCapGain = value; }
            get { return strCapGain; }
        }

        public string Offer
        {
            set { strOffer = value; }
            get { return strOffer; }
        }

        public string BondOffer
        {
            set { strBondOffer = value; }
            get { return strBondOffer; }
        }

        public string ShInv
        {
            set { strShInv = value; }
            get { return strShInv; }
        }
       
        public string XHeader
        {
            set { strXHeader = value; }
            get { return strXHeader; }
        }

        public string XFooter
        {
            set { strXFooter = value; }
            get { return strXFooter; }
        }

        

        public string Suspense
        {
            set { strSuspense = value; }
            get { return strSuspense; }
        }


        public string OffBank
        {
            set { strOffBank = value; }
            get { return strOffBank; }
        }

        


        public string Reserve
        {
            set { strReserve = value; }
            get { return strReserve; }
        }
        public Int32 EomGrace
        {
            set { intEomGrace = value; }
            get { return intEomGrace; }
        }

        public string OverDrawn
        {
            set { strOverDrawn = value; }
            get { return strOverDrawn; }
        }
        public string StatutoryControl
        {
            set { strStatutoryControl = value; }
            get { return strStatutoryControl; }
        }
        public string AgComm
        {
            set { strAgComm = value; }
            get { return strAgComm; }
        }
        public decimal MinBalance
        {
            set { decMinBalance = value; }
            get { return decMinBalance; }
        }
        public Int32 AccessNo
        {
            set { intAccessNo = value; }
            get { return intAccessNo; }
        }
        public Int32 AccessMax
        {
            set { intAccessMax = value; }
            get { return intAccessMax; }
        }

        public string MagTrad
        {
            set { strMagTrad = value; }
            get { return strMagTrad; }
        }


        public string EODRunFlag
        {
            set { strEODRunFlag = value; }
            get { return strEODRunFlag; }
        }

        public string Verification
        {
            set { strVerification = value; }
            get { return strVerification; }
        }

        public decimal VeriFeeBranch
        {
            set { decVeriFeeBranch = value; }
            get { return decVeriFeeBranch; }
        }
        public decimal VeriFeeHead
        {
            set { decVeriFeeHead = value; }
            get { return decVeriFeeHead; }
        }
        public string EOYRunBefore
        {
            set { strEOYRunBefore = value; }
            get { return strEOYRunBefore; }
        }
        

        public string PostStartDate
        {
            get
            {
                //DatabaseFactory.SetDatabaseProviderFactory(new DatabaseProviderFactory());
                //DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;

                 
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                //Database db = factory.Create("GlobalSuitedb");

                //DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("PAGentableSelectPostStartDate") as SqlCommand;
                db.AddOutParameter(oCommand, "@PostStartDate", SqlDbType.VarChar, 10);
                db.ExecuteNonQuery(oCommand);
                if (db.GetParameterValue(oCommand, "@PostStartDate") == null || db.GetParameterValue(oCommand, "@PostStartDate").ToString() == "")
                {
                    return "";
                }
                else
                {
                    return db.GetParameterValue(oCommand, "@PostStartDate").ToString();
                }
            }
        }


        public string TradeBankBank
        {
            set { strTradeBankBank = value; }
            get { return strTradeBankBank; }
        }

        public string DefAcctRep
        {
            set { strDefAcctRep = value; }
            get { return strDefAcctRep; }
        }
        public string DefMDName
        {
            set { strDefMDName = value; }
            get { return strDefMDName; }
        }

        public string NASDBSec
        {
            set { strNASDBSec = value; }
            get { return strNASDBSec; }
        }
        public string NASDSSec
        {
            set { strNASDSSec = value; }
            get { return strNASDSSec; }
        }
        public string NASDBCSCS
        {
            set { strNASDBCSCS = value; }
            get { return strNASDBCSCS; }
        }
        public string NASDSCSCS
        {
            set { strNASDSCSCS = value; }
            get { return strNASDSCSCS; }
        }
        public string NASDBComm
        {
            set { strNASDBComm = value; }
            get { return strNASDBComm; }
        }
        public string NASDSComm
        {
            set { strNASDSComm = value; }
            get { return strNASDSComm; }
        } 
        public string NASDSNASD
        {
            set { strNASDSNASD = value; }
            get { return strNASDSNASD; }
        }
        public string NASDBNASD
        {
            set { strNASDBNASD = value; }
            get { return strNASDBNASD; }
        }
        public string NASDBStamp
        {
            set { strNASDBStamp = value; }
            get { return strNASDBStamp; }
        }
        public string NASDSStamp
        {
            set { strNASDSStamp = value; }
            get { return strNASDSStamp; }
        }

        public string NASDBVAT
        {
            set { strNASDBVAT = value; }
            get { return strNASDBVAT; }
        }
        public string NASDSVAT
        {
            set { strNASDSVAT = value; }
            get { return strNASDSVAT; }
        }
        #endregion

        #region Add New PGenTable For Capital Market
        public bool AddCapitalMarket()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            //DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PGenTableAddCapMarket") as SqlCommand;
            db.AddInParameter(oCommand, "Bcscs", SqlDbType.VarChar, strBcscs.Trim());
            db.AddInParameter(oCommand, "Scscs", SqlDbType.VarChar, strScscs.Trim());
            db.AddInParameter(oCommand, "Bnse", SqlDbType.VarChar, strBnse.Trim());
            db.AddInParameter(oCommand, "Snse", SqlDbType.VarChar, strSnse.Trim());
            db.AddInParameter(oCommand, "Bstamp", SqlDbType.VarChar, strBstamp.Trim());
            db.AddInParameter(oCommand, "Sstamp", SqlDbType.VarChar, strSstamp.Trim());
            db.AddInParameter(oCommand, "Bvat", SqlDbType.VarChar, strBvat.Trim());
            db.AddInParameter(oCommand, "Svat", SqlDbType.VarChar, strSvat.Trim());
            db.AddInParameter(oCommand, "Bcncomm", SqlDbType.VarChar, strBcncomm.Trim());
            db.AddInParameter(oCommand, "Scncomm", SqlDbType.VarChar, strScncomm.Trim());
            db.AddInParameter(oCommand, "Bsec", SqlDbType.VarChar, strBsec.Trim());
            db.AddInParameter(oCommand, "Ssec", SqlDbType.VarChar, strSsec.Trim());
            db.AddInParameter(oCommand, "TradBank", SqlDbType.VarChar, strTradeBank.Trim());
            db.AddInParameter(oCommand, "PropTradeBank", SqlDbType.VarChar, PropTradeBank.Trim());
            db.AddInParameter(oCommand, "NASDTradBank", SqlDbType.VarChar, strNASDTradingBank.Trim());
            db.AddInParameter(oCommand, "DirectCashSettleAcct", SqlDbType.VarChar, strDirectCashSettleAcct.Trim());
            db.AddInParameter(oCommand, "CapGain", SqlDbType.VarChar, strCapGain.Trim());
            db.AddInParameter(oCommand, "Offer", SqlDbType.VarChar, strOffer.Trim());
            db.AddInParameter(oCommand, "BondOffer", SqlDbType.VarChar, strBondOffer.Trim());
            db.AddInParameter(oCommand, "ShInv", SqlDbType.VarChar, strShInv.Trim());
            db.AddInParameter(oCommand, "StkAcct", SqlDbType.VarChar, strStkAcct.Trim());
            db.AddInParameter(oCommand, "OffBank", SqlDbType.VarChar, strOffBank.Trim());
            db.AddInParameter(oCommand, "BrokPay", SqlDbType.VarChar, strBrokPay.Trim());
            db.AddInParameter(oCommand, "Investment", SqlDbType.VarChar, strInvestment.Trim());
            db.AddInParameter(oCommand, "NASDAccount", SqlDbType.VarChar, strNASDAccount.Trim());
            db.AddInParameter(oCommand, "InvestmentNASDAC", SqlDbType.VarChar, strInvestmentNASDAC.Trim());
            db.AddInParameter(oCommand, "InvestmentBondAC", SqlDbType.VarChar, strInvestmentBondAC.Trim());
            db.AddInParameter(oCommand, "OverDrawn", SqlDbType.VarChar, strOverDrawn.Trim());
            db.AddInParameter(oCommand, "StatutoryControl", SqlDbType.VarChar, strStatutoryControl.Trim());
            db.AddInParameter(oCommand, "AgComm", SqlDbType.VarChar, strAgComm.Trim());
            db.AddInParameter(oCommand, "TradBankBank", SqlDbType.VarChar, strTradeBankBank.Trim());
            db.AddInParameter(oCommand, "CapGainContra", SqlDbType.VarChar, strCapGainContra.Trim());
            db.AddInParameter(oCommand, "Verification", SqlDbType.VarChar, strVerification.Trim());
            db.AddInParameter(oCommand, "VeriFeeHead", SqlDbType.Decimal, decVeriFeeHead);
            db.AddInParameter(oCommand, "VeriFeeBranch", SqlDbType.Decimal, decVeriFeeBranch);
            db.AddInParameter(oCommand, "NASDBSec", SqlDbType.VarChar, strNASDBSec);
            db.AddInParameter(oCommand, "NASDSSec", SqlDbType.VarChar, strNASDSSec);
            db.AddInParameter(oCommand, "NASDBCSCS", SqlDbType.VarChar, strNASDBCSCS);
            db.AddInParameter(oCommand, "NASDSCSCS", SqlDbType.VarChar, strNASDSCSCS);
            db.AddInParameter(oCommand, "NASDBComm", SqlDbType.VarChar, strNASDBComm);
            db.AddInParameter(oCommand, "NASDSComm", SqlDbType.VarChar, strNASDSComm);
            db.AddInParameter(oCommand, "NASDSNASD", SqlDbType.VarChar, strNASDSNASD);
            db.AddInParameter(oCommand, "NASDBNASD", SqlDbType.VarChar, strNASDBNASD);
            db.AddInParameter(oCommand, "NASDBStamp", SqlDbType.VarChar, strNASDBStamp);
            db.AddInParameter(oCommand, "NASDSStamp", SqlDbType.VarChar, strNASDSStamp);
            db.AddInParameter(oCommand, "NASDBVAT", SqlDbType.VarChar, strNASDBVAT);
            db.AddInParameter(oCommand, "NASDSVAT", SqlDbType.VarChar, strNASDSVAT);
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;

        }
        #endregion

        #region Get PGenTable Info
        public bool GetPGenTable()
        {
            bool blnStatus = false;
             
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            //DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PGenTableSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strBcscs = thisRow[0]["Bcscs"].ToString();
                strScscs = thisRow[0]["Scscs"].ToString();
                strBnse = thisRow[0]["Bnse"].ToString();
                strSnse = thisRow[0]["Snse"].ToString();
                strBstamp = thisRow[0]["Bstamp"].ToString();
                strSstamp = thisRow[0]["Sstamp"].ToString();
                strBvat = thisRow[0]["Bvat"].ToString();
                strSvat = thisRow[0]["Svat"].ToString();
                strBcncomm = thisRow[0]["Bcncomm"].ToString();
                strScncomm = thisRow[0]["Scncomm"].ToString();
                strBsec = thisRow[0]["Bsec"].ToString();
                strSsec = thisRow[0]["Ssec"].ToString();
                strTradeBank = thisRow[0]["ATradBank"].ToString();
                PropTradeBank = thisRow[0]["PropTradeBank"] != null ? thisRow[0]["PropTradeBank"].ToString() : "";
                strNASDTradingBank = thisRow[0]["ANASDTradBank"] != null ? thisRow[0]["ANASDTradBank"].ToString() : "";
                strDirectCashSettleAcct = thisRow[0]["DirectCashSettleAcct"] != null ? thisRow[0]["DirectCashSettleAcct"].ToString() : "";
                strTradestock = thisRow[0]["ATradStock"].ToString();
                strPayable = thisRow[0]["APayable"].ToString();
                strCapGain = thisRow[0]["ACapGain"].ToString();
                strOffer = thisRow[0]["AOffer"].ToString();
                strBondOffer = thisRow[0]["BondOffer"].ToString();
                strShInv = thisRow[0]["AShInv"].ToString();
                strStkAcct = thisRow[0]["PADepAcct"].ToString();
                strXHeader = thisRow[0]["XHeader"].ToString();
                strXFooter = thisRow[0]["XFooter"].ToString();
                strSuspense = thisRow[0]["ASuspense"].ToString();
                strTradeBankBank = thisRow[0]["ATradBankBank"].ToString();
                strCapGainContra = thisRow[0]["ACapGainContra"] != null ? thisRow[0]["ACapGainContra"].ToString() : "";
                strOffBank = thisRow[0]["AOffBank"].ToString();
                strBrokPay = thisRow[0]["PABrokPay"].ToString();
                strInvestment = thisRow[0]["PInvestmentAC"].ToString();
                strNASDAccount = thisRow[0]["NASDAccount"].ToString();
                strInvestmentNASDAC = thisRow[0]["PInvestmentNASDAC"].ToString();
                strInvestmentBondAC = thisRow[0]["PInvestmentBondAC"].ToString();
                strReserve = thisRow[0]["AReserve"].ToString();
                if (thisRow[0]["EomGrace"].ToString() == "")
                {
                    intEomGrace = 0;
                }
                else
                {
                    intEomGrace = int.Parse(thisRow[0]["EomGrace"].ToString());
                }
                strOverDrawn = thisRow[0]["AOverDrawn"].ToString();
                strStatutoryControl = thisRow[0]["StatutoryControl"].ToString();
                strAgComm = thisRow[0]["AAgComm"].ToString();
                if (thisRow[0]["MinBalance"].ToString() == "")
                {
                    decMinBalance = 0;
                }
                else
                {
                    decMinBalance = decimal.Parse(thisRow[0]["MinBalance"].ToString());
                }
                if (thisRow[0]["AccessNo"].ToString() == "")
                {
                    intAccessNo = 0;
                }
                else
                {
                    intAccessNo = int.Parse(thisRow[0]["AccessNo"].ToString());
                }
                if (thisRow[0]["AccessMax"].ToString() == "")
                {
                    intAccessMax = 0;
                }
                else
                {
                    intAccessMax = int.Parse(thisRow[0]["AccessMax"].ToString());
                }

                strEODRunFlag = thisRow[0]["EODRunFlag"].ToString();
                strVerification = thisRow[0]["AVerification"].ToString();
                if (thisRow[0]["VeriFeeHead"].ToString() == "")
                {
                    decVeriFeeHead = 0;
                }
                else
                {
                    decVeriFeeHead = decimal.Parse(thisRow[0]["VeriFeeHead"].ToString());
                }
                if (thisRow[0]["VeriFeeBranch"].ToString() == "")
                {
                    decVeriFeeBranch = 0;
                }
                else
                {
                    decVeriFeeBranch = decimal.Parse(thisRow[0]["VeriFeeBranch"].ToString());
                }
                strEOYRunBefore = thisRow[0]["EOYRunBefore"].ToString();
                strNASDBSec = thisRow[0]["NASDBSec"] != null ? thisRow[0]["NASDBSec"].ToString() : "";
                strNASDSSec = thisRow[0]["NASDSSec"] != null ? thisRow[0]["NASDSSec"].ToString() : "";
                strNASDBCSCS = thisRow[0]["NASDBCSCS"] != null ? thisRow[0]["NASDBCSCS"].ToString() : "";
                strNASDSCSCS = thisRow[0]["NASDSCSCS"] != null ? thisRow[0]["NASDSCSCS"].ToString() : "";
                strNASDBComm = thisRow[0]["NASDBComm"] != null ? thisRow[0]["NASDBComm"].ToString() : "";
                strNASDSComm = thisRow[0]["NASDSComm"] != null ? thisRow[0]["NASDSComm"].ToString() : "";
                strNASDSNASD = thisRow[0]["NASDSNASD"] != null ? thisRow[0]["NASDSNASD"].ToString() : "";
                strNASDBNASD = thisRow[0]["NASDBNASD"] != null ? thisRow[0]["NASDBNASD"].ToString() : "";
                strNASDBStamp = thisRow[0]["NASDBStamp"] != null ? thisRow[0]["NASDBStamp"].ToString() : "";
                strNASDSStamp = thisRow[0]["NASDSStamp"] != null ? thisRow[0]["NASDSStamp"].ToString() : "";
                strNASDBVAT = thisRow[0]["NASDBVAT"] != null ? thisRow[0]["NASDBVAT"].ToString() : "";
                strNASDSVAT = thisRow[0]["NASDSVAT"] != null ? thisRow[0]["NASDSVAT"].ToString() : "";

                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion
        
    }
}
