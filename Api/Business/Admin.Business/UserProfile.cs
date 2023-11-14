using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace Admin.Business
{
    public class UserProfile
    {
        #region Declaration
        //Module
        private bool blnAdmin, blnAsset, blnCapMarket, blnChurch,blnCustomer, blnEstate, blnFund, blnGL, blnHotel, blnHR;
        private bool blnMoneyMkt, blnResearch, blnRiskManagement, blnSchool;
        
        //Transaction
        private bool blnAddNew, blnView, blnEdit, blnPost,blnReverse, blnDelete,blnBackUp, blnCreateAccess;
        private bool blnLateNight, blnMultStation,blnWkEndAccess, blnPrintRep;
        private DateTime datPswExpDate;
        private decimal decTransLimit;
        private int intPassAge;

        //Stock Market
        private bool blnStockParam, blnCapMarketInt, blnMnMerge, blnCustUpload,blnStockUpload;
        private bool blnAutStkPost,  blnPrintInvMerge, blnPrintInvAgent, blnCustComm;
        private bool blnStkMenuPortfolio,blnStkMenuTrade,blnStkMenuJob,blnStkMenuCert;
        private bool blnStkMenuCompRes, blnStkMenuMaintain, blnStkMenuReport, blnJobNoInSufficientBal;
        private bool blnCapitalMarketPost, blnCapitalMarketReverse, blnAssignCustomerDirectCashSettlement;

        //Accounts
        private bool blnSetChart,blnRunYearEnd,blnRunEOM, blnRunEOD;
        private bool blnOverDrawAcct, blnPrintBackStmt, blnPettyCashStaffRequest, blnBackDate, blnForwardDate, blnAccountPost, blnAccountReverse;

        //Customers
        private bool blnCustRecAcc, blnCustActivat, blnCustDeactivat, blnScanPict,blnDelCustTrans,blnCustomerModify, blnAccessAcctIncompleteKYC;

        //Asset Management
        private bool blnAssetManagePost, blnAssetManageReverse;

        //Money Market
        private bool blnMoneyMarketPost, blnMoneyMarketReverse;

        //HR
        private bool blnHRPost, blnHRReverse;


        private string strUserName;
        #endregion

        #region Properties

        #region Modules
        public bool Admin
        {
            set { blnAdmin = value; }
            get { return blnAdmin; }
        }
        public bool Asset
        {
            set { blnAsset = value; }
            get { return blnAsset; }
        }
        public bool CapMarket
        {
            set { blnCapMarket = value; }
            get { return blnCapMarket; }
        }
        public bool Church
        {
            set { blnChurch = value; }
            get { return blnChurch; }
        }
        public bool Customer
        {
            set { blnCustomer = value; }
            get { return blnCustomer; }
        }
        public bool Estate
        {
            set { blnEstate = value; }
            get { return blnEstate; }
        }
        public bool Fund
        {
            set { blnFund = value; }
            get { return blnFund; }
        }
        public bool GL
        {
            set { blnGL = value; }
            get { return blnGL; }
        }
        public bool Hotel
        {
            set { blnHotel = value; }
            get { return blnHotel; }
        }
        public bool HR
        {
            set { blnHR = value; }
            get { return blnHR; }
        }
        public bool MoneyMkt
        {
            set { blnMoneyMkt = value; }
            get { return blnMoneyMkt; }
        }
        public bool Research
        {
            set { blnResearch = value; }
            get { return blnResearch; }
        }
        public bool RiskManagement
        {
            set { blnRiskManagement = value; }
            get { return blnRiskManagement; }
        }
        public bool School
        {
            set { blnSchool = value; }
            get { return blnSchool; }
        }
        #endregion

        #region Transaction
        public bool AddNew
        {
            set { blnAddNew = value; }
            get { return blnAddNew; }
        }
        public bool View
        {
            set { blnView = value; }
            get { return blnView; }
        }
        public bool Edit
        {
            set { blnEdit = value; }
            get { return blnEdit; }
        }
        public bool Post
        {
            set { blnPost = value; }
            get { return blnPost; }
        }
        public bool Reverse
        {
            set { blnReverse = value; }
            get { return blnReverse; }
        }
        public bool Delete
        {
            set { blnDelete = value; }
            get { return blnDelete; }
        }
        public string UserName
        {
            set { strUserName = value; }
            get { return strUserName; }
        }
       
        public bool BackUp
        {
            set { blnBackUp = value; }
            get { return blnBackUp; }
        }
        public bool CreateAccess
        {
            set { blnCreateAccess = value; }
            get { return blnCreateAccess; }
        }
        public bool LateNight
        {
            set { blnLateNight = value; }
            get { return blnLateNight; }
        }
        public bool MultStation
        {
            set { blnMultStation = value; }
            get { return blnMultStation; }
        }
        public bool WkEndAccess
        {
            set { blnWkEndAccess = value; }
            get { return blnWkEndAccess; }
        }
        public bool PrintRep
        {
            set { blnPrintRep = value; }
            get { return blnPrintRep; }
        }
        public DateTime PswExpDate
        {
            set { datPswExpDate = value; }
            get { return datPswExpDate; }
        }
        public decimal TransLimit
        {
            set { decTransLimit = value; }
            get { return decTransLimit; }
        }
        public int PassAge
        {
            set { intPassAge = value; }
            get { return intPassAge; }
        }
        #endregion

        #region Stock Market
        public bool StockParam
        {
            set { blnStockParam = value; }
            get { return blnStockParam; }
        }
        public bool CapMarketInt
        {
            set { blnCapMarketInt = value; }
            get { return blnCapMarketInt; }
        }
       
        public bool AutStkPost
        {
            set { blnAutStkPost = value; }
            get { return blnAutStkPost; }
        }
        
        public bool CustComm
        {
            set { blnCustComm = value; }
            get { return blnCustComm; }
        }
        public bool MnMerge
        {
            set { blnMnMerge = value; }
            get { return blnMnMerge; }
        }
        public bool CustUpload
        {
            set { blnCustUpload = value; }
            get { return blnCustUpload; }
        }
        public bool StockUpload
        {
            set { blnStockUpload = value; }
            get { return blnStockUpload; }
        }
        public bool PrintInvMerge
        {
            set { blnPrintInvMerge = value; }
            get { return blnPrintInvMerge; }
        }
        public bool PrintInvAgent
        {
            set { blnPrintInvAgent = value; }
            get { return blnPrintInvAgent; }
        }
        public bool StkMenuPortfolio
        {
            set { blnStkMenuPortfolio = value; }
            get { return blnStkMenuPortfolio; }
        }
        public bool StkMenuTrade
        {
            set { blnStkMenuTrade = value; }
            get { return blnStkMenuTrade; }
        }
        public bool StkMenuJob
        {
            set { blnStkMenuJob = value; }
            get { return blnStkMenuJob; }
        }
        public bool StkMenuCert
        {
            set { blnStkMenuCert = value; }
            get { return blnStkMenuCert; }
        }
        public bool StkMenuCompRes
        {
            set { blnStkMenuCompRes = value; }
            get { return blnStkMenuCompRes; }
        }
        public bool StkMenuMaintain
        {
            set { blnStkMenuMaintain = value; }
            get { return blnStkMenuMaintain; }
        }
        public bool StkMenuReport
        {
            set { blnStkMenuReport = value; }
            get { return blnStkMenuReport; }
        }
        public bool JobNoInSufficientBal
        {
            set { blnJobNoInSufficientBal = value; }
            get { return blnJobNoInSufficientBal; }
        }
        public bool CapitalMarketPost
        {
            set { blnCapitalMarketPost = value; }
            get { return blnCapitalMarketPost; }
        }
        public bool CapitalMarketReverse
        {
            set { blnCapitalMarketReverse = value; }
            get { return blnCapitalMarketReverse; }
        }

        public bool AssignCustomerDirectCashSettlement
        {
            set { blnAssignCustomerDirectCashSettlement = value; }
            get { return blnAssignCustomerDirectCashSettlement; }
        }
        
        #endregion

        #region Account
        public bool SetChart
        {
            set { blnSetChart = value; }
            get { return blnSetChart; }
        }
        public bool RunYearEnd
        {
            set { blnRunYearEnd = value; }
            get { return blnRunYearEnd; }
        }
        public bool OverDrawAcct
        {
            set { blnOverDrawAcct = value; }
            get { return blnOverDrawAcct; }
        }
        public bool PrintBackStmt
        {
            set { blnPrintBackStmt = value; }
            get { return blnPrintBackStmt; }
        }
        public bool PettyCashStaffRequest
        {
            set { blnPettyCashStaffRequest = value; }
            get { return blnPettyCashStaffRequest; }
        }
        public bool BackDate
        {
            set { blnBackDate = value; }
            get { return blnBackDate; }
        }
        public bool ForwardDate
        {
            set { blnForwardDate = value; }
            get { return blnForwardDate; }
        }
        public bool RunEOM
        {
            set { blnRunEOM = value; }
            get { return blnRunEOM; }
        }
        public bool RunEOD
        {
            set { blnRunEOD = value; }
            get { return blnRunEOD; }
        }
        public bool AccountPost
        {
            set { blnAccountPost = value; }
            get { return blnAccountPost; }
        }
        public bool AccountReverse
        {
            set { blnAccountReverse = value; }
            get { return blnAccountReverse; }
        }
        #endregion

        #region Customer
        public bool CustRecAcc
        {
            set { blnCustRecAcc = value; }
            get { return blnCustRecAcc; }
        }
        public bool CustActivat
        {
            set { blnCustActivat = value; }
            get { return blnCustActivat; }
        }
        public bool CustDeactivat
        {
            set { blnCustDeactivat = value; }
            get { return blnCustDeactivat; }
        }
        public bool DelCustTrans
        {
            set { blnDelCustTrans = value; }
            get { return blnDelCustTrans; }
        }
        public bool ScanPict
        {
            set { blnScanPict = value; }
            get { return blnScanPict; }
        }
        public bool CustomerModify
        {
            set { blnCustomerModify = value; }
            get { return blnCustomerModify; }
        }
        public bool AccessAcctIncompleteKYC
        {
            set { blnAccessAcctIncompleteKYC = value; }
            get { return blnAccessAcctIncompleteKYC; }
        }
        
        #endregion

        #region AssetManagement
        public bool AssetManagePost
        {
            set { blnAssetManagePost = value; }
            get { return blnAssetManagePost; }
        }
        public bool AssetManageReverse
        {
            set { blnAssetManageReverse = value; }
            get { return blnAssetManageReverse; }
        }
        #endregion

        #region MoneyMarket
        public bool MoneyMarketPost
        {
            set { blnMoneyMarketPost = value; }
            get { return blnMoneyMarketPost; }
        }
        public bool MoneyMarketReverse
        {
            set { blnMoneyMarketReverse = value; }
            get { return blnMoneyMarketReverse; }
        }
        #endregion

        #region HR
        public bool HRPost
        {
            set { blnHRPost = value; }
            get { return blnHRPost; }
        }
        public bool HRReverse
        {
            set { blnHRReverse = value; }
            get { return blnHRReverse; }
        }
        #endregion

        #endregion

        #region Get User Profile
        public bool GetUserProfile()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("UserProfileSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "UserName", SqlDbType.VarChar, strUserName.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                //Modules
                blnAdmin = bool.Parse(thisRow[0]["S_R"].ToString() != "" ? thisRow[0]["S_R"].ToString() : "false");
                blnAsset = bool.Parse(thisRow[0]["I_C"].ToString() != "" ? thisRow[0]["I_C"].ToString() : "false");
                blnCapMarket = bool.Parse(thisRow[0]["S_M"].ToString() != "" ? thisRow[0]["S_M"].ToString() : "false");
                blnCustomer = bool.Parse(thisRow[0]["M_A"].ToString() != "" ? thisRow[0]["M_A"].ToString() : "false");
                blnChurch = bool.Parse(thisRow[0]["Church"].ToString() != "" ? thisRow[0]["Church"].ToString() : "false");
                blnEstate = bool.Parse(thisRow[0]["E_S"].ToString() != "" ? thisRow[0]["E_S"].ToString() : "false");
                blnFund = bool.Parse(thisRow[0]["C_L"].ToString() != "" ? thisRow[0]["C_L"].ToString() : "false");
                blnGL = bool.Parse(thisRow[0]["A_C"].ToString() != "" ? thisRow[0]["A_C"].ToString() : "false");
                blnHotel = bool.Parse(thisRow[0]["H_T"].ToString() != "" ? thisRow[0]["H_T"].ToString() : "false");
                blnHR = bool.Parse(thisRow[0]["P_Y"].ToString() != "" ? thisRow[0]["P_Y"].ToString() : "false");
                blnMoneyMkt = bool.Parse(thisRow[0]["M_M"].ToString() != "" ? thisRow[0]["M_M"].ToString() : "false");
                blnResearch = bool.Parse(thisRow[0]["Research"].ToString() != "" ? thisRow[0]["Research"].ToString() : "false");
                blnRiskManagement = bool.Parse(thisRow[0]["RiskManagement"].ToString() != "" ? thisRow[0]["RiskManagement"].ToString() : "false");
                blnSchool = bool.Parse(thisRow[0]["S_H"].ToString() != "" ? thisRow[0]["S_H"].ToString() : "false");
                //blnPortfolio = bool.Parse(thisRow[0]["P_M"].ToString() != "" ? thisRow[0]["P_M"].ToString() : "false");
                
                //Transactions
                blnAddNew = bool.Parse(thisRow[0]["PostStkTrans"].ToString() != "" ? thisRow[0]["PostStkTrans"].ToString() : "false");
                blnView = bool.Parse(thisRow[0]["RevStkTrans"].ToString() != "" ? thisRow[0]["RevStkTrans"].ToString() : "false");
                blnEdit = bool.Parse(thisRow[0]["PostOthTrans"].ToString() != "" ? thisRow[0]["PostOthTrans"].ToString() : "false");
                blnPost = bool.Parse(thisRow[0]["PostJournalTrans"].ToString() != "" ? thisRow[0]["PostJournalTrans"].ToString() : "false");
                blnReverse = bool.Parse(thisRow[0]["RevJournalTrans"].ToString() != "" ? thisRow[0]["RevJournalTrans"].ToString() : "false");
                blnDelete = bool.Parse(thisRow[0]["RevOthTrans"].ToString() != "" ? thisRow[0]["RevOthTrans"].ToString() : "false");
                blnBackUp = bool.Parse(thisRow[0]["BACKUP"].ToString() != "" ? thisRow[0]["BACKUP"].ToString() : "false");
                blnCreateAccess = bool.Parse(thisRow[0]["CREATEACCESS"].ToString() != "" ? thisRow[0]["CREATEACCESS"].ToString() : "false");
                blnLateNight = bool.Parse(thisRow[0]["LATENIGHTWORK"].ToString() != "" ? thisRow[0]["LATENIGHTWORK"].ToString() : "false");
                blnMultStation = bool.Parse(thisRow[0]["MULTIPLESTATION"].ToString() != "" ? thisRow[0]["MULTIPLESTATION"].ToString() : "false");
                blnScanPict = bool.Parse(thisRow[0]["ScanPict"].ToString() != "" ? thisRow[0]["ScanPict"].ToString() : "false");
                blnWkEndAccess = bool.Parse(thisRow[0]["WEEKENDWORK"].ToString() != "" ? thisRow[0]["WEEKENDWORK"].ToString() : "false");
                blnPrintRep = bool.Parse(thisRow[0]["PRINTREP"].ToString() != "" ? thisRow[0]["PRINTREP"].ToString() : "false");
                decTransLimit = decimal.Parse(thisRow[0]["AmtLimit"].ToString());

                //StockMarket
                blnStockParam = bool.Parse(thisRow[0]["MODISTKPARAM"].ToString() != "" ? thisRow[0]["MODISTKPARAM"].ToString() : "false");
                blnCapMarketInt = bool.Parse(thisRow[0]["MODISTKGLFACE"].ToString() != "" ? thisRow[0]["MODISTKGLFACE"].ToString() : "false");
                blnAutStkPost = bool.Parse(thisRow[0]["AUTOPOST"].ToString() != "" ? thisRow[0]["AUTOPOST"].ToString() : "false");
                blnCustComm = bool.Parse(thisRow[0]["EDITCUSTCOMM"].ToString() != "" ? thisRow[0]["EDITCUSTCOMM"].ToString() : "false");
                blnPrintInvMerge = bool.Parse(thisRow[0]["PRTINVMERGE"].ToString() != "" ? thisRow[0]["PRTINVMERGE"].ToString() : "false");
                blnPrintInvAgent = bool.Parse(thisRow[0]["PRTINVAGENT"].ToString() != "" ? thisRow[0]["PRTINVAGENT"].ToString() : "false");
                blnCustUpload = bool.Parse(thisRow[0]["MNCUSTDUPLOAD"].ToString() != "" ? thisRow[0]["MNCUSTDUPLOAD"].ToString() : "false");
                blnStockUpload = bool.Parse(thisRow[0]["MNSTKUPLOAD"].ToString() != "" ? thisRow[0]["MNSTKUPLOAD"].ToString() : "false");
                blnMnMerge = bool.Parse(thisRow[0]["MNMERGE"].ToString() != "" ? thisRow[0]["MNMERGE"].ToString() : "false");
                blnJobNoInSufficientBal = bool.Parse(thisRow[0]["JobNoInSufficientBal"].ToString() != "" ? thisRow[0]["JobNoInSufficientBal"].ToString() : "false");
                blnStkMenuPortfolio = bool.Parse(thisRow[0]["StkMenuPortfolio"].ToString() != "" ? thisRow[0]["StkMenuPortfolio"].ToString() : "false");
                blnStkMenuTrade = bool.Parse(thisRow[0]["StkMenuTrade"].ToString() != "" ? thisRow[0]["StkMenuTrade"].ToString() : "false");
                blnStkMenuJob = bool.Parse(thisRow[0]["StkMenuJob"].ToString() != "" ? thisRow[0]["StkMenuJob"].ToString() : "false");
                blnStkMenuCert = bool.Parse(thisRow[0]["StkMenuCert"].ToString() != "" ? thisRow[0]["StkMenuCert"].ToString() : "false");
                blnStkMenuCompRes = bool.Parse(thisRow[0]["StkMenuCompRes"].ToString() != "" ? thisRow[0]["StkMenuCompRes"].ToString() : "false");
                blnStkMenuMaintain = bool.Parse(thisRow[0]["StkMenuMaintain"].ToString() != "" ? thisRow[0]["StkMenuMaintain"].ToString() : "false");
                blnStkMenuReport = bool.Parse(thisRow[0]["StkMenuReport"].ToString() != "" ? thisRow[0]["StkMenuReport"].ToString() : "false");
                blnCapitalMarketPost = bool.Parse(thisRow[0]["CapitalMarketPost"].ToString() != "" ? thisRow[0]["CapitalMarketPost"].ToString() : "false");
                blnCapitalMarketReverse = bool.Parse(thisRow[0]["CapitalMarketReverse"].ToString() != "" ? thisRow[0]["CapitalMarketReverse"].ToString() : "false");
                blnAssignCustomerDirectCashSettlement = bool.Parse(thisRow[0]["AssignCustomerDirectCashSettlement"].ToString() != "" ? thisRow[0]["AssignCustomerDirectCashSettlement"].ToString() : "false");
                

                //Account
                blnSetChart = bool.Parse(thisRow[0]["SETCHART"].ToString() != "" ? thisRow[0]["SETCHART"].ToString() : "false");
                blnRunYearEnd = bool.Parse(thisRow[0]["RUNYEAREND"].ToString() != "" ? thisRow[0]["RUNYEAREND"].ToString() : "false");
                blnRunEOD = bool.Parse(thisRow[0]["RUNEOM"].ToString() != "" ? thisRow[0]["RUNEOM"].ToString() : "false");
                blnRunEOM = bool.Parse(thisRow[0]["RUNEOD"].ToString() != "" ? thisRow[0]["RUNEOD"].ToString() : "false");
                blnOverDrawAcct = bool.Parse(thisRow[0]["OVERDRAWNACCT"].ToString() != "" ? thisRow[0]["OVERDRAWNACCT"].ToString() : "false");
                blnPrintBackStmt = bool.Parse(thisRow[0]["PRINTBACKSTMT"].ToString() != "" ? thisRow[0]["PRINTBACKSTMT"].ToString() : "false");
                blnPettyCashStaffRequest = bool.Parse(thisRow[0]["PettyCashStaffRequest"].ToString() != "" ? thisRow[0]["PettyCashStaffRequest"].ToString() : "false");
                blnBackDate = bool.Parse(thisRow[0]["BACKDATE"].ToString() != "" ? thisRow[0]["BACKDATE"].ToString() : "false");
                blnForwardDate = bool.Parse(thisRow[0]["FORWARDDATE"].ToString() != "" ? thisRow[0]["FORWARDDATE"].ToString() : "false");
                blnAccountPost = bool.Parse(thisRow[0]["AccountPost"].ToString() != "" ? thisRow[0]["AccountPost"].ToString() : "false");
                blnAccountReverse = bool.Parse(thisRow[0]["AccountReverse"].ToString() != "" ? thisRow[0]["AccountReverse"].ToString() : "false");

                //Customer
                blnCustRecAcc = bool.Parse(thisRow[0]["MODCUST"].ToString() != "" ? thisRow[0]["MODCUST"].ToString() : "false");
                blnCustActivat = bool.Parse(thisRow[0]["MNCUSTACTIVATION"].ToString() != "" ? thisRow[0]["MNCUSTACTIVATION"].ToString() : "false");
                blnCustDeactivat = bool.Parse(thisRow[0]["MNCUSTDEACTIVATION"].ToString() != "" ? thisRow[0]["MNCUSTDEACTIVATION"].ToString() : "false");
                blnDelCustTrans = bool.Parse(thisRow[0]["DELCUSTTRANS"].ToString() != "" ? thisRow[0]["DELCUSTTRANS"].ToString() : "false");
                blnCustomerModify = bool.Parse(thisRow[0]["MNCUSTPRE"].ToString() != "" ? thisRow[0]["MNCUSTPRE"].ToString() : "false");
                blnStatus = true; 

                 //Asset Management
                 blnAssetManagePost = bool.Parse(thisRow[0]["AssetManagePost"].ToString() != "" ? thisRow[0]["AssetManagePost"].ToString() : "false");
                blnAssetManageReverse = bool.Parse(thisRow[0]["AssetManageReverse"].ToString() != "" ? thisRow[0]["AssetManageReverse"].ToString() : "false");

                //Money Market
                blnMoneyMarketPost = bool.Parse(thisRow[0]["MoneyMarketPost"].ToString() != "" ? thisRow[0]["MoneyMarketPost"].ToString() : "false");
                blnMoneyMarketReverse = bool.Parse(thisRow[0]["MoneyMarketReverse"].ToString() != "" ? thisRow[0]["MoneyMarketReverse"].ToString() : "false");

                //HR
                blnHRPost = bool.Parse(thisRow[0]["HRPost"].ToString() != "" ? thisRow[0]["HRPost"].ToString() : "false");
                blnHRReverse = bool.Parse(thisRow[0]["HRReverse"].ToString() != "" ? thisRow[0]["HRReverse"].ToString() : "false");

            }
            else
            {
                User oUser = new User();
                UserLevelProfile oUserLevelProfile = new UserLevelProfile();
                oUser.UserNameAccount = strUserName;
                if (oUser.GetUserNoPassword())
                {
                    oUserLevelProfile.UserLevelName = oUser.Group;
                    if (oUserLevelProfile.GetUserLevelProfile())
                    {
                        AssignUserLevelToUser(oUserLevelProfile);
                        blnStatus = true;
                    }
                }
            }
            return blnStatus;
        }
        #endregion

        #region Assign UserLevel To User
        public void AssignUserLevelToUser(UserLevelProfile profile)
        {
                //Modules
                blnAdmin = profile.Admin;
                blnAsset = profile.Asset;
                blnCapMarket = profile.CapMarket;
                blnCustomer = profile.Customer;
                blnChurch = profile.Church;
                blnEstate = profile.Estate;
                blnFund = profile.Fund;
                blnGL = profile.GL;
                blnHotel = profile.Hotel;
                blnHR = profile.HR;
                blnMoneyMkt = profile.MoneyMkt;
                blnResearch = profile.Research;
                blnRiskManagement = profile.RiskManagement;
                blnSchool = profile.School;
                //blnPortfolio = profile.P_M;

                //Transactions
                blnAddNew = profile.AddNew;
                blnView = profile.View;
                blnEdit = profile.Edit;
                blnPost = profile.Post;
                blnReverse = profile.Reverse;
                blnDelete = profile.Delete;
                blnBackUp = profile.BackUp;
                blnCreateAccess = profile.CreateAccess;
                blnLateNight = profile.LateNight;
                blnMultStation = profile.MultStation;
                blnScanPict = profile.ScanPict;
                blnWkEndAccess = profile.WkEndAccess;
                blnPrintRep = profile.PrintRep;
                decTransLimit = profile.TransLimit;

                //StockMarket
                blnStockParam = profile.StockParam;
                blnCapMarketInt = profile.CapMarketInt;
                blnAutStkPost = profile.AutStkPost;
                blnCustComm = profile.AutStkPost;
                blnPrintInvMerge = profile.PrintInvMerge;
                blnPrintInvAgent = profile.PrintInvAgent;
                blnCustUpload = profile.CustUpload;
                blnStockUpload = profile.StockUpload;
                blnMnMerge = profile.MnMerge;
                blnJobNoInSufficientBal = profile.JobNoInSufficientBal;

                blnStkMenuPortfolio = profile.StkMenuPortfolio;
                blnStkMenuTrade = profile.StkMenuTrade;
                blnStkMenuJob = profile.StkMenuJob;
                blnStkMenuCert = profile.StkMenuCert;
                blnStkMenuCompRes = profile.StkMenuCompRes;
                blnStkMenuMaintain = profile.StkMenuMaintain;
                blnStkMenuReport = profile.StkMenuReport;
                blnAssignCustomerDirectCashSettlement = profile.AssignCustomerDirectCashSettlement;

            //Account
                blnSetChart = profile.SetChart;
                blnRunYearEnd = profile.RunYearEnd;
                blnRunEOD = profile.RunEOD;
                blnRunEOM = profile.RunEOM;
                blnOverDrawAcct = profile.OverDrawAcct;
                blnPrintBackStmt = profile.PrintBackStmt;
                blnPettyCashStaffRequest = profile.PettyCashStaffRequest;
                blnBackDate = profile.BackDate;
                blnForwardDate = profile.ForwardDate;

                //Customer
                blnCustRecAcc = profile.CustRecAcc;
                blnCustActivat = profile.CustActivat;
                blnCustDeactivat = profile.CustDeactivat;
                blnDelCustTrans = profile.DelCustTrans;

            
        }
        #endregion

        #region Save
        public DataGeneral.SaveStatus Save()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    SqlCommand oCommand = null;
                    if (ChkUserProfileExist())
                    {
                        oCommand = db.GetStoredProcCommand("UserProfileEdit") as SqlCommand;
                    }
                    else
                    {
                        oCommand = db.GetStoredProcCommand("UserProfileAddNew") as SqlCommand;
                    }
                    db.AddInParameter(oCommand, "UserName", SqlDbType.VarChar, strUserName.Trim().ToUpper());
                    
                    //Modules
                    db.AddInParameter(oCommand, "Admin", SqlDbType.VarChar, blnAdmin);
                    db.AddInParameter(oCommand, "Asset", SqlDbType.VarChar, blnAsset);
                    db.AddInParameter(oCommand, "CapMarket", SqlDbType.VarChar, blnCapMarket);
                    db.AddInParameter(oCommand, "Church", SqlDbType.VarChar, blnChurch);
                    db.AddInParameter(oCommand, "Customer", SqlDbType.VarChar, blnCustomer);
                    db.AddInParameter(oCommand, "Fund", SqlDbType.VarChar, blnFund);
                    db.AddInParameter(oCommand, "GL", SqlDbType.VarChar, blnGL);
                    db.AddInParameter(oCommand, "HR", SqlDbType.VarChar, blnHR);
                    db.AddInParameter(oCommand, "Inventory", SqlDbType.VarChar, "");
                    db.AddInParameter(oCommand, "MoneyMkt", SqlDbType.VarChar, blnMoneyMkt);
                    db.AddInParameter(oCommand, "School", SqlDbType.VarChar, blnSchool);
                    db.AddInParameter(oCommand, "Hotel", SqlDbType.VarChar, blnHotel);
                    db.AddInParameter(oCommand, "Research", SqlDbType.VarChar, blnResearch);
                    db.AddInParameter(oCommand, "RiskManagement", SqlDbType.VarChar, blnRiskManagement);

                    //Transaction
                    db.AddInParameter(oCommand, "AddNew", SqlDbType.VarChar, blnAddNew);
                    db.AddInParameter(oCommand, "View", SqlDbType.VarChar, blnView);
                    db.AddInParameter(oCommand, "Edit", SqlDbType.VarChar, blnEdit);
                    db.AddInParameter(oCommand, "Post", SqlDbType.VarChar, blnPost);
                    db.AddInParameter(oCommand, "Reverse", SqlDbType.VarChar, blnReverse);
                    db.AddInParameter(oCommand, "Delete", SqlDbType.VarChar, blnDelete);
                    db.AddInParameter(oCommand, "BackUp", SqlDbType.VarChar, blnBackUp);
                    db.AddInParameter(oCommand, "CreateAccess", SqlDbType.VarChar, blnCreateAccess);
                    db.AddInParameter(oCommand, "LateNight", SqlDbType.VarChar, blnLateNight);
                    db.AddInParameter(oCommand, "MultStation", SqlDbType.VarChar, blnMultStation);
                    db.AddInParameter(oCommand, "WkEndAccess", SqlDbType.VarChar, blnWkEndAccess);
                    db.AddInParameter(oCommand, "PrintRep", SqlDbType.VarChar, blnPrintRep);
                    db.AddInParameter(oCommand, "TransLimit", SqlDbType.Money, decTransLimit);
                    if (datPswExpDate != DateTime.MinValue)
                    {
                        db.AddInParameter(oCommand, "ExpDate", SqlDbType.DateTime, datPswExpDate);

                    }
                    else
                    {
                        db.AddInParameter(oCommand, "ExpDate", SqlDbType.DateTime, SqlDateTime.Null);

                    }
                    db.AddInParameter(oCommand, "PassAge", SqlDbType.Int, intPassAge);

                    //Stock Market
                    db.AddInParameter(oCommand, "StockParam", SqlDbType.VarChar, blnStockParam);
                    db.AddInParameter(oCommand, "CapMarketInt", SqlDbType.VarChar, blnCapMarketInt);
                    db.AddInParameter(oCommand, "AutStkPost", SqlDbType.VarChar, blnAutStkPost);
                    db.AddInParameter(oCommand, "CustComm", SqlDbType.VarChar, blnCustComm);
                    db.AddInParameter(oCommand, "MnMerge", SqlDbType.VarChar, blnMnMerge);
                    db.AddInParameter(oCommand, "CustUpload", SqlDbType.VarChar, blnCustUpload);
                    db.AddInParameter(oCommand, "StockUpload", SqlDbType.VarChar, blnStockUpload);
                    db.AddInParameter(oCommand, "PrintInvMerge", SqlDbType.VarChar, blnPrintInvMerge);
                    db.AddInParameter(oCommand, "PrintInvAgent", SqlDbType.VarChar, blnPrintInvAgent);
                    db.AddInParameter(oCommand, "StkMenuPortfolio", SqlDbType.VarChar, blnStkMenuPortfolio);
                    db.AddInParameter(oCommand, "StkMenuTrade", SqlDbType.VarChar, blnStkMenuTrade);
                    db.AddInParameter(oCommand, "StkMenuJob", SqlDbType.VarChar, blnStkMenuJob);
                    db.AddInParameter(oCommand, "StkMenuCert", SqlDbType.VarChar, blnStkMenuCert);
                    db.AddInParameter(oCommand, "StkMenuCompRes", SqlDbType.VarChar, blnStkMenuCompRes);
                    db.AddInParameter(oCommand, "StkMenuMaintain", SqlDbType.VarChar, blnStkMenuMaintain);
                    db.AddInParameter(oCommand, "StkMenuReport", SqlDbType.VarChar, blnStkMenuReport);
                    db.AddInParameter(oCommand, "JobNoInSufficientBal", SqlDbType.VarChar, blnJobNoInSufficientBal);
                    db.AddInParameter(oCommand, "CapitalMarketPost", SqlDbType.VarChar, blnCapitalMarketPost);
                    db.AddInParameter(oCommand, "CapitalMarketReverse", SqlDbType.VarChar, blnCapitalMarketReverse);
                    db.AddInParameter(oCommand, "AssignCustomerDirectCashSettlement", SqlDbType.VarChar, blnAssignCustomerDirectCashSettlement);

                    //Account
                    db.AddInParameter(oCommand, "SetChart", SqlDbType.VarChar, blnSetChart);
                    db.AddInParameter(oCommand, "RunYearEnd", SqlDbType.VarChar, blnRunYearEnd);
                    db.AddInParameter(oCommand, "RunEOD", SqlDbType.VarChar, blnRunEOD);
                    db.AddInParameter(oCommand, "RunEOM", SqlDbType.VarChar, blnRunEOM);
                    db.AddInParameter(oCommand, "OverDrawAcct", SqlDbType.VarChar, blnOverDrawAcct);
                    db.AddInParameter(oCommand, "PrintBackStmt", SqlDbType.VarChar, blnPrintBackStmt);
                    db.AddInParameter(oCommand, "PettyCashStaffRequest", SqlDbType.VarChar, blnPettyCashStaffRequest);
                    db.AddInParameter(oCommand, "BackDate", SqlDbType.VarChar, blnBackDate);
                    db.AddInParameter(oCommand, "ForwardDate", SqlDbType.VarChar, blnForwardDate);
                    db.AddInParameter(oCommand, "AccountPost", SqlDbType.VarChar, blnAccountPost);
                    db.AddInParameter(oCommand, "AccountReverse", SqlDbType.VarChar, blnAccountReverse);

                    //Customer
                    db.AddInParameter(oCommand, "CustRecAcc", SqlDbType.VarChar, blnCustRecAcc);
                    db.AddInParameter(oCommand, "CustActivat", SqlDbType.VarChar, blnCustActivat);
                    db.AddInParameter(oCommand, "CustDeactivat", SqlDbType.VarChar, blnCustDeactivat);
                    db.AddInParameter(oCommand, "DelCustTrans", SqlDbType.VarChar, blnDelCustTrans);
                    db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    db.AddInParameter(oCommand, "ScanPict", SqlDbType.VarChar, blnScanPict);
                    db.AddInParameter(oCommand, "CustomerModify", SqlDbType.VarChar, blnCustomerModify);
                    db.AddInParameter(oCommand, "AccessAcctIncompleteKYC", SqlDbType.VarChar, blnAccessAcctIncompleteKYC);
                    
                    //AssetManagement
                    db.AddInParameter(oCommand, "AssetManagePost", SqlDbType.VarChar, blnAssetManagePost);
                    db.AddInParameter(oCommand, "AssetManageReverse", SqlDbType.VarChar, blnAssetManageReverse);

                    //MoneyMarket
                    db.AddInParameter(oCommand, "MoneyMarketPost", SqlDbType.VarChar, blnMoneyMarketPost);
                    db.AddInParameter(oCommand, "MoneyMarketReverse", SqlDbType.VarChar, blnMoneyMarketReverse);

                    //HR
                    db.AddInParameter(oCommand, "HRPost", SqlDbType.VarChar, blnHRPost);
                    db.AddInParameter(oCommand, "HRReverse", SqlDbType.VarChar, blnHRReverse);

                    db.ExecuteNonQuery(oCommand, transaction);
                    transaction.Commit();
                }
                catch (Exception err)
                {
                    transaction.Rollback();
                    throw new Exception(err.Message);
                }
            }
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Check That User Profile Already Exist
        public bool ChkUserProfileExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("UserProfileSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "UserName", SqlDbType.VarChar, strUserName.Trim());
            DataSet oDs = db.ExecuteDataSet(dbCommand);
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
        
    }
}
