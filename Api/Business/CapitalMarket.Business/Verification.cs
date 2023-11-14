using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.SqlTypes;
using System.Globalization;
using System.Configuration;
using BaseUtility.Business;
using CustomerManagement.Business;
using GL.Business;

namespace CapitalMarket.Business
{
    public class Verification
    {
        #region Declarations
        private string strCode, strBatchNo, strCodeForAlert;
        private string strCustNo, strSurname, strFirstname, strOthername, strStockcode;
        private DateTime datEffdate, datEffdateTo, datTxnDate, datTxnDateTo;
        private int intUnits;
        private string strCertno, strCertacc, strReceiptno;
        private bool blnVerificationStatus, blnUnVerificationStatus, blnJBookCross,blnIsDematerialization;
        private DateTime datDateVerified, datDateSent, datDateRec;
        private string strSecurityStatus, strLodgementReason, strCertificateNumber;
        private string strComments, strUserId, strBonus;
        private decimal decUnitPrice, decConsideration, decBestPrice;
        private bool blnCustNotify, blnCertificateCollected;
        private string strEPrint, strSaveType, strCrossBuyer, strStartingTransNumber;
        private List<Verification> lstVerificationCert;
        public bool FullDematerialization { set; get; }

        #endregion

        #region Properties
        public string Code
        {
            set { strCode = value; }
            get { return strCode; }
        }
        public string BatchNo
        {
            set { strBatchNo = value; }
            get { return strBatchNo; }
        }
        public string CodeForAlert
        {
            set { strCodeForAlert = value; }
            get { return strCodeForAlert; }
        }
        public string CustNo
        {
            set { strCustNo = value; }
            get { return strCustNo; }
        }
        public string Surname
        {
            set { strSurname = value; }
            get { return strSurname; }
        }
        public string Firstname
        {
            set { strFirstname = value; }
            get { return strFirstname; }
        }
        public string Othername
        {
            set { strOthername = value; }
            get { return strOthername; }
        }
        public string Stockcode
        {
            set { strStockcode = value; }
            get { return strStockcode; }
        }
        public DateTime Effdate
        {
            set { datEffdate = value; }
            get { return datEffdate; }
        }
        public DateTime EffdateTo
        {
            set { datEffdateTo = value; }
            get { return datEffdateTo; }
        }
        public DateTime TxnDate
        {
            set { datTxnDate = value; }
            get { return datTxnDate; }
        }
        public DateTime TxnDateTo
        {
            set { datTxnDateTo = value; }
            get { return datTxnDateTo; }
        }
        public int Units
        {
            set { intUnits = value; }
            get { return intUnits; }
        }
        public string Certno
        {
            set { strCertno = value; }
            get { return strCertno; }
        }
        public string Certacc
        {
            set { strCertacc = value; }
            get { return strCertacc; }
        }
        public string Receiptno
        {
            set { strReceiptno = value; }
            get { return strReceiptno; }
        }

        public bool VerificationStatus
        {
            set { blnVerificationStatus = value; }
            get { return blnVerificationStatus; }
        }
        public bool UnVerificationStatus
        {
            set { blnUnVerificationStatus = value; }
            get { return blnUnVerificationStatus; }
        }
        public bool JBookCross
        {
            set { blnJBookCross = value; }
            get { return blnJBookCross; }
        }
        public bool IsDematerialization
        {
            set { blnIsDematerialization = value; }
            get { return blnIsDematerialization; }
        }
        
        public DateTime DateVerified
        {
            set { datDateVerified = value; }
            get { return datDateVerified; }
        }
        public DateTime DateSent
        {
            set { datDateSent = value; }
            get { return datDateSent; }
        }
        public DateTime DateRec
        {
            set { datDateRec = value; }
            get { return datDateRec; }
        }
        public string SecurityStatus
        {
            set { strSecurityStatus = value; }
            get { return strSecurityStatus; }
        }
        public string LodgementReason
        {
            set { strLodgementReason = value; }
            get { return strLodgementReason; }
        }
        public string CertificateNumber
        {
            set { strCertificateNumber = value; }
            get { return strCertificateNumber; }
        }
        public string Comments
        {
            set { strComments = value; }
            get { return strComments; }
        }
        public string UserId
        {
            set { strUserId = value; }
            get { return strUserId; }
        }
        public string Bonus
        {
            set { strBonus = value; }
            get { return strBonus; }
        }
        public decimal UnitPrice
        {
            set { decUnitPrice = value; }
            get { return decUnitPrice; }
        }
        public decimal Consideration
        {
            set { decConsideration = value; }
            get { return decConsideration; }
        }
        public decimal BestPrice
        {
            set { decBestPrice = value; }
            get { return decBestPrice; }
        }
        public bool CustNotify
        {
            set { blnCustNotify = value; }
            get { return blnCustNotify; }
        }
        public bool CertificateCollected
        {
            set { blnCertificateCollected = value; }
            get { return blnCertificateCollected; }
        }
        public string EPrint
        {
            set { strEPrint = value; }
            get { return strEPrint; }
        }
       
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        public string CrossBuyer
        {
            set { strCrossBuyer = value; }
            get { return strCrossBuyer; }
        }
        public string StartingTransNumber
        {
            set { strStartingTransNumber = value; }
            get { return strStartingTransNumber; }
        }
        public List<Verification> VerificationCert
        {
            set { lstVerificationCert = value; }
            get { return lstVerificationCert; }
        }
        #endregion

        public Verification()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region Enum
        public enum SaveStatus
        {
            Nothing, NotExist, Saved, Verified,
            CsCsNoNotExist, BoxLoadNoPrice, ReturnDateMissing,
            VerifiedReturnCheckMissing, VerifiedReturnCheckSameTime,
            EffDateInFuture, CustCrossMissing, DuplicateCertNo
        }
        #endregion

        #region Save
        public SaveStatus Save()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");

            SaveStatus enSaveStatus = SaveStatus.Nothing;
            if (datDateVerified == DateTime.MinValue)
            {
                if ((blnVerificationStatus) || (blnUnVerificationStatus) || (FullDematerialization))
                {
                    enSaveStatus = SaveStatus.ReturnDateMissing;
                    return enSaveStatus;
                }
            }
            if (datDateVerified != DateTime.MinValue)
            {
                if ((!blnVerificationStatus) && (!blnUnVerificationStatus) && (!FullDematerialization))
                {
                    enSaveStatus = SaveStatus.VerifiedReturnCheckMissing;
                    return enSaveStatus;
                }
            }
            
            if ((blnVerificationStatus) && (blnUnVerificationStatus))
            {
                enSaveStatus = SaveStatus.VerifiedReturnCheckSameTime;
                return enSaveStatus;
            }
            if ((blnVerificationStatus) && (FullDematerialization))
            {
                enSaveStatus = SaveStatus.VerifiedReturnCheckSameTime;
                return enSaveStatus;
            }
            
            if (datEffdate > DateTime.Now)
            {
                enSaveStatus = SaveStatus.EffDateInFuture;
                return enSaveStatus;
            }
            if (blnJBookCross)
            {
                if (strCrossBuyer.Trim() == "" || strCrossBuyer == null)
                {
                    enSaveStatus = SaveStatus.CustCrossMissing;
                    return enSaveStatus;
                }
            }
            if (!ChkTransNoExist())
            {
                enSaveStatus = SaveStatus.NotExist;
                return enSaveStatus;
            }
            if (ChkCertVerified())
            {
                enSaveStatus = SaveStatus.Verified;
                return enSaveStatus;
            }
            if (CertNoExist())
            {
                enSaveStatus = SaveStatus.DuplicateCertNo;
                return enSaveStatus;
            }
            if (blnVerificationStatus)
            {
                if (!ChkCustomerCSCSNoExist())
                {
                    enSaveStatus = SaveStatus.CsCsNoNotExist;
                }
            }
            if (strBonus == "N")
            {
                if (!ChkBoxLoadHasPrice())
                {
                    enSaveStatus = SaveStatus.BoxLoadNoPrice;
                }
            }
            StkParam oStkParam = new StkParam();
            string strCertificateNo = "";
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    SqlCommand oCommand = null;
                    Verification oVerificationChkRegOrRet = new Verification();
                    if (strSaveType == "ADDS")
                    {
                        oCommand = db.GetStoredProcCommand("VerificationAddNew") as SqlCommand;
                        db.AddOutParameter(oCommand, "Code", SqlDbType.VarChar, 10);
                        db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "CERTREGISTER");
                    }
                    else if (strSaveType == "EDIT")
                    {
                        oVerificationChkRegOrRet.GetRegularAndReturned(strCode.Trim());
                        if (oVerificationChkRegOrRet.UnVerificationStatus && (blnUnVerificationStatus == false))
                        {
                            strSaveType = "ADDS";
                            oCommand = db.GetStoredProcCommand("VerificationAddNew") as SqlCommand;
                            db.AddOutParameter(oCommand, "Code", SqlDbType.VarChar, 10);
                            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "CERTREGISTER");
                        }
                        else
                        {
                            oCommand = db.GetStoredProcCommand("VerificationEdit") as SqlCommand;
                            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCode.Trim());
                        }
                    }
                    db.AddInParameter(oCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
                    db.AddInParameter(oCommand, "Surname", SqlDbType.VarChar, strSurname.Trim());
                    db.AddInParameter(oCommand, "Firstname", SqlDbType.VarChar, strFirstname.Trim());
                    db.AddInParameter(oCommand, "Othername", SqlDbType.VarChar, strOthername.Trim());
                    db.AddInParameter(oCommand, "Stockcode", SqlDbType.VarChar, strStockcode.Trim());
                    db.AddInParameter(oCommand, "Effdate", SqlDbType.DateTime, datEffdate);
                    db.AddInParameter(oCommand, "Units", SqlDbType.Int, intUnits);
                    db.AddInParameter(oCommand, "Certno", SqlDbType.VarChar, strCertno.Trim());
                    db.AddInParameter(oCommand, "Certacc", SqlDbType.VarChar, strCertacc.Trim());
                    db.AddInParameter(oCommand, "VerificationStatus", SqlDbType.Bit, blnVerificationStatus);
                    db.AddInParameter(oCommand, "UnVerificationStatus", SqlDbType.Bit, blnUnVerificationStatus);
                    db.AddInParameter(oCommand, "FullDematerialization", SqlDbType.Bit, FullDematerialization);
                    db.AddInParameter(oCommand, "JBookCross", SqlDbType.Bit, blnJBookCross);
                    if (datDateVerified != DateTime.MinValue)
                    {
                        db.AddInParameter(oCommand, "DateVerified", SqlDbType.DateTime, datDateVerified);
                    }
                    else
                    {
                        db.AddInParameter(oCommand, "DateVerified", SqlDbType.DateTime, SqlDateTime.Null);
                    }
                    if (datDateSent != DateTime.MinValue)
                    {
                        db.AddInParameter(oCommand, "DateSent", SqlDbType.DateTime, datDateSent);
                    }
                    else
                    {
                        db.AddInParameter(oCommand, "DateSent", SqlDbType.DateTime, SqlDateTime.Null);
                    }
                    db.AddInParameter(oCommand, "LodgementReason", SqlDbType.VarChar, strLodgementReason.Trim());
                    db.AddInParameter(oCommand, "Comments", SqlDbType.NText, strComments.Trim());
                    db.AddInParameter(oCommand, "UnitPrice", SqlDbType.Money, decUnitPrice);
                    if (datDateRec != DateTime.MinValue)
                    {
                        db.AddInParameter(oCommand, "DateRec", SqlDbType.DateTime, datDateRec);
                    }
                    else
                    {
                        db.AddInParameter(oCommand, "DateRec", SqlDbType.DateTime, SqlDateTime.Null);
                    }
                    db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId.Trim());
                    db.AddInParameter(oCommand, "CustNotify", SqlDbType.Bit, blnCustNotify);
                    db.AddInParameter(oCommand, "CertificateCollected", SqlDbType.Bit, blnCertificateCollected);
                    db.AddInParameter(oCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());
                    db.AddInParameter(oCommand, "CrossBuyer", SqlDbType.VarChar, strCrossBuyer.Trim());
                    db.AddInParameter(oCommand, "Product", SqlDbType.VarChar, oStkParam.Product.Trim());
                    db.AddInParameter(oCommand, "BatchNo", SqlDbType.VarChar, strBatchNo.Trim());
                    db.AddInParameter(oCommand, "IsDematerialization", SqlDbType.Bit, blnIsDematerialization);
                    db.ExecuteNonQuery(oCommand, transaction);
                    if (strSaveType == "ADDS")
                    {
                        strCertificateNo = db.GetParameterValue(oCommand, "Code").ToString().Trim();
                    }
                    else if (strSaveType == "EDIT")
                    {
                        strCertificateNo = strCode.Trim();
                    }
                    if (oVerificationChkRegOrRet.UnVerificationStatus && (blnUnVerificationStatus == false))
                    {
                        SqlCommand dbCommandSaveStartingNumber = oVerificationChkRegOrRet.SaveStartingNumberCommand(strCertificateNo);
                        db.ExecuteNonQuery(dbCommandSaveStartingNumber, transaction);
                    }
                    if (blnVerificationStatus)
                    {
                        Portfolio oPort = new Portfolio();
                        oPort.PurchaseDate = datDateVerified;
                        oPort.CustomerAcct = strCustNo;
                        oPort.StockCode = strStockcode;
                        oPort.Units = intUnits;
                        oPort.UnitPrice = float.Parse(decUnitPrice.ToString());
                        oPort.ActualUnitCost = float.Parse(decUnitPrice.ToString());
                        oPort.TotalCost = intUnits * decUnitPrice;
                        if (strLodgementReason.Trim() == "SALE")
                        {
                            oPort.SysRef = "CESA-" + "-" + strCertificateNo;
                        }
                        else if (strLodgementReason.Trim() == "HOUSE")
                        {
                            oPort.SysRef = "HOUS-" + "-" + strCertificateNo;
                        }
                        else if (strLodgementReason.Trim() == "COLL")
                        {
                            oPort.SysRef = "COLL-" + "-" + strCertificateNo;
                        }
                        else if (strLodgementReason.Trim() == "NYFS")
                        {
                            oPort.SysRef = "NYFS-" + "-" + strCertificateNo;
                        }
                        else if (strLodgementReason.Trim() == "INDEM")
                        {
                            oPort.SysRef = "INDEM-" + "-" + strCertificateNo;
                        }
                        oPort.TransType = "CERT";
                        oPort.Ref01 = strCertificateNo;
                        oPort.TransDesc = "Certificate Depository";
                        oPort.DebCred = "C";
                        oPort.MarginCode = "";
                        SqlCommand dbCommandPort = oPort.AddCommand();
                        db.ExecuteNonQuery(dbCommandPort, transaction);
                    }
                    if (strLodgementReason.Trim() == "SALE" && blnVerificationStatus)
                    {
                        JobOrder oJobOrder = new JobOrder();
                        oJobOrder.Code = strCode;
                        oJobOrder.JB_ID = strCertificateNo;
                        oJobOrder.EffectiveDate = datDateVerified;
                        oJobOrder.CustNo = strCustNo;
                        oJobOrder.StockCode = strStockcode;
                        oJobOrder.Units = intUnits;
                        oJobOrder.UnitPrice = decUnitPrice;
                        oJobOrder.Instructions = "Verified Certificate";
                        oJobOrder.Amount = 0;
                        oJobOrder.AmtProc = 0;
                        oJobOrder.Balance = intUnits;
                        oJobOrder.CustBalance = 0;
                        oJobOrder.Broker = "";
                        oJobOrder.InputFrom = "CERTREGISTER";
                        if (blnJBookCross)
                        {
                            oJobOrder.CustNo_CD = strCrossBuyer;
                        }
                        else
                        {
                            oJobOrder.CustNo_CD = "";
                        }

                        if (blnJBookCross)
                        {
                            oJobOrder.TxnType = 2;
                        }
                        else
                        {
                            oJobOrder.TxnType = 1;
                        }
                        oJobOrder.Posted = "N";
                        oJobOrder.Reversed = "N";
                        SqlCommand dbCommandJobOrder = oJobOrder.AddCommand();
                        db.ExecuteNonQuery(dbCommandJobOrder, transaction);
                    }
                    transaction.Commit();
                    strCodeForAlert = strCertificateNo;
                    enSaveStatus = SaveStatus.Saved;
                    return enSaveStatus;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
                finally
                {
                    connection.Close();
                }
            }


        }
        #endregion

        #region Save With Batch
        public SaveStatus SaveWithBatch()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");

            SaveStatus enSaveStatus = SaveStatus.Nothing;
            if (datDateVerified == DateTime.MinValue)
            {
                if ((blnVerificationStatus) || (blnUnVerificationStatus) || (FullDematerialization))
                {
                    enSaveStatus = SaveStatus.ReturnDateMissing;
                    return enSaveStatus;
                }
            }
            if (datDateVerified != DateTime.MinValue)
            {
                if ((!blnVerificationStatus) && (!blnUnVerificationStatus) && (!FullDematerialization))
                {
                    enSaveStatus = SaveStatus.VerifiedReturnCheckMissing;
                    return enSaveStatus;
                }
            }
            if ((blnVerificationStatus) && (blnUnVerificationStatus) && (FullDematerialization))
            {
                enSaveStatus = SaveStatus.VerifiedReturnCheckSameTime;
                return enSaveStatus;
            }
            if ((blnVerificationStatus) && (blnUnVerificationStatus))
            {
                enSaveStatus = SaveStatus.VerifiedReturnCheckSameTime;
                return enSaveStatus;
            }
            if ((blnVerificationStatus) && (FullDematerialization))
            {
                enSaveStatus = SaveStatus.VerifiedReturnCheckSameTime;
                return enSaveStatus;
            }
            if ((blnUnVerificationStatus) && (FullDematerialization))
            {
                enSaveStatus = SaveStatus.VerifiedReturnCheckSameTime;
                return enSaveStatus;
            }
            if (datEffdate > DateTime.Now)
            {
                enSaveStatus = SaveStatus.EffDateInFuture;
                return enSaveStatus;
            }
            if (blnJBookCross)
            {
                if (strCrossBuyer.Trim() == "" || strCrossBuyer == null)
                {
                    enSaveStatus = SaveStatus.CustCrossMissing;
                    return enSaveStatus;
                }

            }

            if (!ChkTransNoExist())
            {
                enSaveStatus = SaveStatus.NotExist;
                return enSaveStatus;
            }
            if (ChkCertVerified())
            {
                enSaveStatus = SaveStatus.Verified;
                return enSaveStatus;
            }
            if (CertNoExist())
            {
                enSaveStatus = SaveStatus.DuplicateCertNo;
                return enSaveStatus;
            }
            if (blnVerificationStatus)
            {
                if (!ChkCustomerCSCSNoExist())
                {
                    enSaveStatus = SaveStatus.CsCsNoNotExist;
                }
            }
            if (strBonus == "N")
            {
                if (!ChkBoxLoadHasPrice())
                {
                    enSaveStatus = SaveStatus.BoxLoadNoPrice;
                }
            }
            StkParam oStkParam = new StkParam();
            string strCertificateNo = "";
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    Verification oVerificationChkRegOrRet = new Verification();
                    SqlCommand oCommand = null;
                    if (strSaveType == "ADDS")
                    {
                        oCommand = db.GetStoredProcCommand("VerificationAddNewWithBatch") as SqlCommand;
                        db.AddOutParameter(oCommand, "Code", SqlDbType.VarChar, 10);
                        db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "CERTREGISTER");

                    }
                    else if (strSaveType == "EDIT")
                    {
                        
                        oVerificationChkRegOrRet.GetRegularAndReturned(strCode.Trim());
                        if (oVerificationChkRegOrRet.UnVerificationStatus && (blnUnVerificationStatus == false))
                        {
                            strSaveType = "ADDS";
                            oCommand = db.GetStoredProcCommand("VerificationAddNewWithBatch") as SqlCommand;
                            db.AddOutParameter(oCommand, "Code", SqlDbType.VarChar, 10);
                            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "CERTREGISTER");
                        }
                        else
                        {
                            oCommand = db.GetStoredProcCommand("VerificationEdit") as SqlCommand;
                            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCode.Trim());
                        }
                    }
                    db.AddInParameter(oCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
                    db.AddInParameter(oCommand, "Surname", SqlDbType.VarChar, strSurname.Trim());
                    db.AddInParameter(oCommand, "Firstname", SqlDbType.VarChar, strFirstname.Trim());
                    db.AddInParameter(oCommand, "Othername", SqlDbType.VarChar, strOthername.Trim());
                    db.AddInParameter(oCommand, "Stockcode", SqlDbType.VarChar, strStockcode.Trim());
                    db.AddInParameter(oCommand, "Effdate", SqlDbType.DateTime, datEffdate);
                    db.AddInParameter(oCommand, "Units", SqlDbType.Int, intUnits);
                    db.AddInParameter(oCommand, "Certno", SqlDbType.VarChar, strCertno.Trim());
                    db.AddInParameter(oCommand, "Certacc", SqlDbType.VarChar, strCertacc.Trim());
                    db.AddInParameter(oCommand, "VerificationStatus", SqlDbType.Bit, blnVerificationStatus);
                    db.AddInParameter(oCommand, "UnVerificationStatus", SqlDbType.Bit, blnUnVerificationStatus);
                    db.AddInParameter(oCommand, "FullDematerialization", SqlDbType.Bit, FullDematerialization);
                    db.AddInParameter(oCommand, "JBookCross", SqlDbType.Bit, blnJBookCross);
                    if (datDateVerified != DateTime.MinValue)
                    {
                        db.AddInParameter(oCommand, "DateVerified", SqlDbType.DateTime, datDateVerified);

                    }
                    else
                    {
                        db.AddInParameter(oCommand, "DateVerified", SqlDbType.DateTime, SqlDateTime.Null);

                    }
                    if (datDateSent != DateTime.MinValue)
                    {
                        db.AddInParameter(oCommand, "DateSent", SqlDbType.DateTime, datDateSent);
                    }
                    else
                    {
                        db.AddInParameter(oCommand, "DateSent", SqlDbType.DateTime, SqlDateTime.Null);
                    }
                    db.AddInParameter(oCommand, "LodgementReason", SqlDbType.VarChar, strLodgementReason.Trim());

                    db.AddInParameter(oCommand, "Comments", SqlDbType.NText, strComments.Trim());

                    db.AddInParameter(oCommand, "UnitPrice", SqlDbType.Money, decUnitPrice);

                    if (datDateRec != DateTime.MinValue)
                    {
                        db.AddInParameter(oCommand, "DateRec", SqlDbType.DateTime, datDateRec);

                    }
                    else
                    {
                        db.AddInParameter(oCommand, "DateRec", SqlDbType.DateTime, SqlDateTime.Null);

                    }
                    db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId.Trim());
                    db.AddInParameter(oCommand, "CustNotify", SqlDbType.Bit, blnCustNotify);
                    db.AddInParameter(oCommand, "CertificateCollected", SqlDbType.Bit, blnCertificateCollected);
                    db.AddInParameter(oCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());
                    db.AddInParameter(oCommand, "CrossBuyer", SqlDbType.VarChar, strCrossBuyer.Trim());
                    db.AddInParameter(oCommand, "Product", SqlDbType.VarChar, oStkParam.Product.Trim());
                    db.AddInParameter(oCommand, "BatchNo", SqlDbType.VarChar, strBatchNo.Trim());
                    db.ExecuteNonQuery(oCommand, transaction);
                    if (strSaveType == "ADDS")
                    {
                        strCertificateNo = db.GetParameterValue(oCommand, "Code").ToString().Trim();
                    }
                    else if (strSaveType == "EDIT")
                    {
                        strCertificateNo = strCode.Trim();
                    }
                    if (oVerificationChkRegOrRet.UnVerificationStatus && (blnUnVerificationStatus == false))
                    {
                        SqlCommand dbCommandSaveStartingNumber = oVerificationChkRegOrRet.SaveStartingNumberCommand(strCertificateNo);
                        db.ExecuteNonQuery(dbCommandSaveStartingNumber, transaction);
                    }
                    if (blnVerificationStatus)
                    {
                        Portfolio oPort = new Portfolio();
                        oPort.PurchaseDate = datDateVerified;
                        oPort.CustomerAcct = strCustNo;
                        oPort.StockCode = strStockcode;
                        oPort.Units = intUnits;
                        oPort.UnitPrice = float.Parse(decUnitPrice.ToString());
                        oPort.ActualUnitCost = float.Parse(decUnitPrice.ToString());
                        oPort.TotalCost = intUnits * decUnitPrice;
                        if (strLodgementReason.Trim() == "SALE")
                        {
                            oPort.SysRef = "CESA-" + "-" + strCertificateNo;
                        }
                        else if (strLodgementReason.Trim() == "HOUSE")
                        {
                            oPort.SysRef = "HOUS-" + "-" + strCertificateNo;
                        }
                        else if (strLodgementReason.Trim() == "COLL")
                        {
                            oPort.SysRef = "COLL-" + "-" + strCertificateNo;
                        }
                        else if (strLodgementReason.Trim() == "NYFS")
                        {
                            oPort.SysRef = "NYFS-" + "-" + strCertificateNo;
                        }
                        else if (strLodgementReason.Trim() == "INDEM")
                        {
                            oPort.SysRef = "INDEM-" + "-" + strCertificateNo;
                        }
                        oPort.TransType = "CERT";
                        oPort.Ref01 = strCertificateNo;
                        oPort.TransDesc = "Certificate Depository";
                        oPort.DebCred = "C";
                        oPort.MarginCode = "";
                        SqlCommand dbCommandPort = oPort.AddCommand();
                        db.ExecuteNonQuery(dbCommandPort, transaction);
                    }
                    if (strLodgementReason.Trim() == "SALE" && blnVerificationStatus)
                    {
                        JobOrder oJobOrder = new JobOrder();
                        oJobOrder.Code = strCode;
                        oJobOrder.JB_ID = strCertificateNo;
                        oJobOrder.EffectiveDate = datDateVerified;
                        oJobOrder.CustNo = strCustNo;
                        oJobOrder.StockCode = strStockcode;
                        oJobOrder.Units = intUnits;
                        oJobOrder.UnitPrice = decUnitPrice;
                        oJobOrder.Instructions = "Verified Certificate";
                        oJobOrder.Amount = 0;
                        oJobOrder.AmtProc = 0;
                        oJobOrder.Balance = intUnits;
                        oJobOrder.CustBalance = 0;
                        oJobOrder.Broker = "";
                        oJobOrder.InputFrom = "CERTREGISTER";
                        if (blnJBookCross)
                        {
                            oJobOrder.CustNo_CD = strCrossBuyer;
                        }
                        else
                        {
                            oJobOrder.CustNo_CD = "";
                        }

                        if (blnJBookCross)
                        {
                            oJobOrder.TxnType = 2;
                        }
                        else
                        {
                            oJobOrder.TxnType = 1;
                        }
                        oJobOrder.Posted = "N";
                        oJobOrder.Reversed = "N";
                        SqlCommand dbCommandJobOrder = oJobOrder.AddCommand();
                        db.ExecuteNonQuery(dbCommandJobOrder, transaction);

                    }
                    transaction.Commit();
                    strCodeForAlert = strCertificateNo;
                    enSaveStatus = SaveStatus.Saved;
                    return enSaveStatus;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
                finally
                {
                    connection.Close();
                }
            }


        }
        #endregion

        #region Save Multi
        public SaveStatus SaveMulti()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            SaveStatus enSaveStatus = SaveStatus.Nothing;
            if (lstVerificationCert.Count > 0)
            {
                foreach (Verification oVerifyCert in lstVerificationCert)
                {
                    strCode = oVerifyCert.Code;
                    strCertno = oVerifyCert.Certno;
                    strCustNo = oVerifyCert.CustNo;
                    strStockcode = oVerifyCert.Stockcode;
                    strSaveType = oVerifyCert.SaveType;

                    if (oVerifyCert.DateVerified == DateTime.MinValue)
                    {
                        if ((oVerifyCert.VerificationStatus) || (oVerifyCert.UnVerificationStatus) || (oVerifyCert.FullDematerialization))
                        {
                            enSaveStatus = SaveStatus.ReturnDateMissing;
                            return enSaveStatus;
                        }
                    }
                    if (oVerifyCert.DateVerified != DateTime.MinValue)
                    {
                        if ((!oVerifyCert.VerificationStatus) && (!oVerifyCert.UnVerificationStatus) && (!oVerifyCert.FullDematerialization))
                        {
                            enSaveStatus = SaveStatus.VerifiedReturnCheckMissing;
                            return enSaveStatus;
                        }
                    }
                    if ((oVerifyCert.VerificationStatus) && (oVerifyCert.UnVerificationStatus) && (oVerifyCert.FullDematerialization))
                    {
                        enSaveStatus = SaveStatus.VerifiedReturnCheckSameTime;
                        return enSaveStatus;
                    }
                    if ((oVerifyCert.VerificationStatus) && (oVerifyCert.UnVerificationStatus))
                    {
                        enSaveStatus = SaveStatus.VerifiedReturnCheckSameTime;
                        return enSaveStatus;
                    }
                    if ((oVerifyCert.VerificationStatus) && (oVerifyCert.FullDematerialization))
                    {
                        enSaveStatus = SaveStatus.VerifiedReturnCheckSameTime;
                        return enSaveStatus;
                    }
                    if ((oVerifyCert.UnVerificationStatus) && (oVerifyCert.FullDematerialization))
                    {
                        enSaveStatus = SaveStatus.VerifiedReturnCheckSameTime;
                        return enSaveStatus;
                    }
                    if (oVerifyCert.Effdate > DateTime.Now)
                    {
                        enSaveStatus = SaveStatus.EffDateInFuture;
                        return enSaveStatus;
                    }
                    if (oVerifyCert.JBookCross)
                    {
                        if (oVerifyCert.CrossBuyer.Trim() == "" || oVerifyCert.CrossBuyer == null)
                        {
                            enSaveStatus = SaveStatus.CustCrossMissing;
                            return enSaveStatus;
                        }

                    }

                    if (!ChkTransNoExist())
                    {
                        enSaveStatus = SaveStatus.NotExist;
                        return enSaveStatus;
                    }
                    if (ChkCertVerified())
                    {
                        enSaveStatus = SaveStatus.Verified;
                        return enSaveStatus;
                    }
                    if (CertNoExist())
                    {
                        enSaveStatus = SaveStatus.DuplicateCertNo;
                        return enSaveStatus;
                    }
                    if (oVerifyCert.VerificationStatus)
                    {
                        if (!ChkCustomerCSCSNoExist())
                        {
                            enSaveStatus = SaveStatus.CsCsNoNotExist;
                        }
                    }
                    if (oVerifyCert.Bonus == "N")
                    {
                        if (!ChkBoxLoadHasPrice())
                        {
                            enSaveStatus = SaveStatus.BoxLoadNoPrice;
                        }
                    }
                }
                StkParam oStkParam = new StkParam();
                string strCertificateNo = "";
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                using (SqlConnection connection = db.CreateConnection() as SqlConnection)
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    try
                    {
                        foreach (Verification oVerifyCert in lstVerificationCert)
                        {
                            strCode = oVerifyCert.Code;
                            strCertno = oVerifyCert.Certno;
                            strCustNo = oVerifyCert.CustNo;
                            strStockcode = oVerifyCert.Stockcode;
                            strSaveType = oVerifyCert.SaveType;
                            SqlCommand oCommand = null;
                            if (oVerifyCert.SaveType == "ADDS")
                            {
                                oCommand = db.GetStoredProcCommand("VerificationAddNew") as SqlCommand;
                                db.AddOutParameter(oCommand, "Code", SqlDbType.VarChar, 10);
                                db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "CERTREGISTER");
                            }
                            else if (oVerifyCert.SaveType == "EDIT")
                            {
                                oCommand = db.GetStoredProcCommand("VerificationEdit") as SqlCommand;
                                db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, oVerifyCert.Code.Trim());
                            }
                            db.AddInParameter(oCommand, "CustNo", SqlDbType.VarChar, oVerifyCert.CustNo.Trim());
                            db.AddInParameter(oCommand, "Surname", SqlDbType.VarChar, oVerifyCert.Surname.Trim());
                            db.AddInParameter(oCommand, "Firstname", SqlDbType.VarChar, oVerifyCert.Firstname.Trim());
                            db.AddInParameter(oCommand, "Othername", SqlDbType.VarChar, oVerifyCert.Othername.Trim());
                            db.AddInParameter(oCommand, "Stockcode", SqlDbType.VarChar, oVerifyCert.Stockcode.Trim());
                            db.AddInParameter(oCommand, "Effdate", SqlDbType.DateTime, oVerifyCert.Effdate);
                            db.AddInParameter(oCommand, "Units", SqlDbType.Int, oVerifyCert.Units);
                            db.AddInParameter(oCommand, "Certno", SqlDbType.VarChar, oVerifyCert.Certno.Trim());
                            db.AddInParameter(oCommand, "Certacc", SqlDbType.VarChar, oVerifyCert.Certacc.Trim());
                            db.AddInParameter(oCommand, "VerificationStatus", SqlDbType.Bit, oVerifyCert.VerificationStatus);
                            db.AddInParameter(oCommand, "UnVerificationStatus", SqlDbType.Bit, oVerifyCert.UnVerificationStatus);
                            db.AddInParameter(oCommand, "FullDematerialization", SqlDbType.Bit, oVerifyCert.FullDematerialization);

                            db.AddInParameter(oCommand, "JBookCross", SqlDbType.Bit, oVerifyCert.JBookCross);
                            if (oVerifyCert.DateVerified != DateTime.MinValue)
                            {
                                db.AddInParameter(oCommand, "DateVerified", SqlDbType.DateTime, oVerifyCert.DateVerified);

                            }
                            else
                            {
                                db.AddInParameter(oCommand, "DateVerified", SqlDbType.DateTime, SqlDateTime.Null);

                            }
                            if (oVerifyCert.DateSent != DateTime.MinValue)
                            {
                                db.AddInParameter(oCommand, "DateSent", SqlDbType.DateTime, oVerifyCert.DateSent);
                            }
                            else
                            {
                                db.AddInParameter(oCommand, "DateSent", SqlDbType.DateTime, SqlDateTime.Null);
                            }
                            db.AddInParameter(oCommand, "LodgementReason", SqlDbType.VarChar, oVerifyCert.LodgementReason.Trim());

                            db.AddInParameter(oCommand, "Comments", SqlDbType.NText, oVerifyCert.Comments.Trim());

                            db.AddInParameter(oCommand, "UnitPrice", SqlDbType.Money, oVerifyCert.UnitPrice);

                            if (oVerifyCert.DateRec != DateTime.MinValue)
                            {
                                db.AddInParameter(oCommand, "DateRec", SqlDbType.DateTime, oVerifyCert.DateRec);

                            }
                            else
                            {
                                db.AddInParameter(oCommand, "DateRec", SqlDbType.DateTime, SqlDateTime.Null);

                            }
                            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                            db.AddInParameter(oCommand, "CustNotify", SqlDbType.Bit, oVerifyCert.CustNotify);
                            db.AddInParameter(oCommand, "CertificateCollected", SqlDbType.Bit, oVerifyCert.CertificateCollected);
                            db.AddInParameter(oCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());
                            db.AddInParameter(oCommand, "CrossBuyer", SqlDbType.VarChar, oVerifyCert.CrossBuyer.Trim());
                            db.AddInParameter(oCommand, "Product", SqlDbType.VarChar, oStkParam.Product.Trim());
                            db.AddInParameter(oCommand, "BatchNo", SqlDbType.VarChar, oVerifyCert.BatchNo.Trim());
                            db.ExecuteNonQuery(oCommand, transaction);
                            if (oVerifyCert.SaveType == "ADDS")
                            {
                                strCertificateNo = db.GetParameterValue(oCommand, "Code").ToString().Trim();
                            }
                            else if (oVerifyCert.SaveType == "EDIT")
                            {
                                strCertificateNo = oVerifyCert.Code.Trim();
                            }

                            if (oVerifyCert.VerificationStatus)
                            {
                                Portfolio oPort = new Portfolio();
                                oPort.PurchaseDate = oVerifyCert.DateVerified;
                                oPort.CustomerAcct = oVerifyCert.CustNo;
                                oPort.StockCode = oVerifyCert.Stockcode;
                                oPort.Units = oVerifyCert.Units;
                                oPort.UnitPrice = float.Parse(oVerifyCert.UnitPrice.ToString());
                                oPort.ActualUnitCost = float.Parse(oVerifyCert.UnitPrice.ToString());
                                oPort.TotalCost = oVerifyCert.Units * oVerifyCert.UnitPrice;
                                if (oVerifyCert.LodgementReason.Trim() == "SALE")
                                {
                                    oPort.SysRef = "CESA-" + "-" + strCertificateNo;
                                }
                                else if (oVerifyCert.LodgementReason.Trim() == "HOUSE")
                                {
                                    oPort.SysRef = "HOUS-" + "-" + strCertificateNo;
                                }
                                else if (oVerifyCert.LodgementReason.Trim() == "COLL")
                                {
                                    oPort.SysRef = "COLL-" + "-" + strCertificateNo;
                                }
                                else if (oVerifyCert.LodgementReason.Trim() == "NYFS")
                                {
                                    oPort.SysRef = "NYFS-" + "-" + strCertificateNo;
                                }
                                else if (strLodgementReason.Trim() == "INDEM")
                                {
                                    oPort.SysRef = "INDEM-" + "-" + strCertificateNo;
                                }
                                oPort.TransType = "CERT";
                                oPort.Ref01 = strCertificateNo;
                                oPort.TransDesc = "Certificate Depository";
                                oPort.DebCred = "C";
                                oPort.MarginCode = "";
                                SqlCommand dbCommandPort = oPort.AddCommand();
                                db.ExecuteNonQuery(dbCommandPort, transaction);
                            }
                            if (oVerifyCert.LodgementReason.Trim() == "SALE" && oVerifyCert.VerificationStatus)
                            {
                                JobOrder oJobOrder = new JobOrder();
                                oJobOrder.Code = oVerifyCert.Code;
                                oJobOrder.JB_ID = strCertificateNo;
                                oJobOrder.EffectiveDate = oVerifyCert.DateVerified;
                                oJobOrder.CustNo = oVerifyCert.CustNo;
                                oJobOrder.StockCode = oVerifyCert.Stockcode;
                                oJobOrder.Units = oVerifyCert.Units;
                                oJobOrder.UnitPrice = oVerifyCert.UnitPrice;
                                oJobOrder.Instructions = "Verified Certificate";
                                oJobOrder.Amount = 0;
                                oJobOrder.AmtProc = 0;
                                oJobOrder.Balance = oVerifyCert.Units;
                                oJobOrder.CustBalance = 0;
                                oJobOrder.Broker = "";
                                oJobOrder.InputFrom = "CERTREGISTER";
                                if (oVerifyCert.JBookCross)
                                {
                                    oJobOrder.CustNo_CD = oVerifyCert.CrossBuyer;
                                }
                                else
                                {
                                    oJobOrder.CustNo_CD = "";
                                }

                                if (oVerifyCert.JBookCross)
                                {
                                    oJobOrder.TxnType = 2;
                                }
                                else
                                {
                                    oJobOrder.TxnType = 1;
                                }
                                oJobOrder.Posted = "N";
                                oJobOrder.Reversed = "N";
                                SqlCommand dbCommandJobOrder = oJobOrder.AddCommand();
                                db.ExecuteNonQuery(dbCommandJobOrder, transaction);

                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            enSaveStatus = Verification.SaveStatus.Saved;
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
                SqlCommand oCommand = db.GetStoredProcCommand("VerificationChkTransNoExist") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strCode.Trim());
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

        #region Get All Verification
        public DataSet GetAll(DataGeneral.PostStatus TransStatus)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("VerificationSelectAll") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("VerificationSelectAllUnPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("VerificationSelectAllPosted") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "Code", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockcode.Trim());
            db.AddInParameter(dbCommand, "Certno", SqlDbType.VarChar, strCertno.Trim());
            db.AddInParameter(dbCommand, "Certacc", SqlDbType.VarChar, strCertacc.Trim());
            db.AddInParameter(dbCommand, "Customer", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, strUserId.Trim().ToUpper());
            if (datEffdate != DateTime.MinValue)
            {
                db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime, datEffdate);
            }
            else
            {
                db.AddInParameter(dbCommand, "Effdate", SqlDbType.DateTime, SqlDateTime.Null);
            }
            db.AddInParameter(dbCommand, "Unit", SqlDbType.BigInt, intUnits);
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;


        }
        #endregion

        #region Get All Full Dematerialization
        public DataSet GetAllFullDemat()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("VerificationSelectAllFullDematerialization") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Full Dematerialization
        public DataSet GetAllHoldingForFullDemat()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("VerificationSelectAllHoldingForFullDematerialization") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Verification Transactions By Entry Date
        public DataSet GetAllGivenEntryDate(DataGeneral.PostStatus TransStatus)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllGivenEntryDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllGivenEntryDatePostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllGivenEntryDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllGivenEntryDateUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "EntryDate", SqlDbType.DateTime, datTxnDate);
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Verification Transactions By Effective Date
        public DataSet GetAllGivenEffDate(DataGeneral.PostStatus TransStatus)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllGivenEffDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllGivenEffDatePostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllGivenEffDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllGivenEffDateUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime,datEffdate);
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Verification Transactions By Transaction No
        public DataSet GetAllGivenTxnNo(DataGeneral.PostStatus TransStatus, string strTxnFrom, string strTxnTo)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllGivenTxnNoPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllGivenTxnNoPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllGivenTxnNoUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllGivenTxnNoUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TxnFrom", SqlDbType.VarChar, strTxnFrom.Trim());
            db.AddInParameter(dbCommand, "TxnTo", SqlDbType.VarChar, strTxnTo.Trim());
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Verification For Customer StockCode And Date
        public DataSet GetAllForCustStkDate()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("VerificationSelectAllForCustStkDate") as SqlCommand;
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockcode.Trim());
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime, datEffdate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;


        }
        #endregion

        #region Get All Verification For Customer And Date
        public DataSet GetAllForCustDate()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("VerificationSelectAllForCustDate") as SqlCommand;
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime, datEffdate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;


        }
        #endregion
        

        #region Check Certificate Is Verified
        public bool ChkCertVerified()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("VerificationSelectByTransNoReturnVerificationStatus") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strCode.Trim());

                blnStatus = (bool)db.ExecuteScalar(oCommand);
            }
            else if (strSaveType == "ADDS")
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Check Customer CSCS Number Exist
        public bool ChkCustomerCSCSNoExist()
        {
            bool blnStatus = false;
            StkParam oStkParam = new StkParam();
            ProductAcct oProductAcct = new ProductAcct();
            oProductAcct.CustAID = strCustNo.Trim();
            oProductAcct.ProductCode = oStkParam.Product;
            if (oProductAcct.GetCSCSNo())
            {
                if (oProductAcct.CsCsAcct.Trim() == "" || oProductAcct.CsCsAcct == null)
                {
                    blnStatus = false;
                }
                else
                {
                    blnStatus = true;
                }
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Check Box Load Customer Has Price
        public bool ChkBoxLoadHasPrice()
        {
            bool blnStatus = false;
            StkParam oStkParam = new StkParam();
            ProductAcct oProductAcct = new ProductAcct();
            oProductAcct.CustAID = strCustNo.Trim();
            oProductAcct.ProductCode = oStkParam.Product;
            if (oProductAcct.GetBoxLoadStatus())
            {
                if (decUnitPrice == 0)
                {
                    blnStatus = false;
                }
                else
                {
                    blnStatus = true;
                }
            }
            else
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Add Return Command
        public SqlCommand AddCommand()
        {
            StkParam oStkParam = new StkParam();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            oCommand = db.GetStoredProcCommand("VerificationAddNew") as SqlCommand;
            db.AddOutParameter(oCommand, "Code", SqlDbType.VarChar, 10);
            db.AddInParameter(oCommand, "CustNo", SqlDbType.VarChar, strCustNo);
            db.AddInParameter(oCommand, "Surname", SqlDbType.VarChar, strSurname);
            db.AddInParameter(oCommand, "Firstname", SqlDbType.VarChar, strFirstname);
            db.AddInParameter(oCommand, "Othername", SqlDbType.VarChar, strOthername);
            db.AddInParameter(oCommand, "Stockcode", SqlDbType.VarChar, strStockcode);
            if (datEffdate != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "Effdate", SqlDbType.DateTime, datEffdate);
            }
            else
            {
                db.AddInParameter(oCommand, "Effdate", SqlDbType.DateTime, SqlDateTime.Null);
            }
            db.AddInParameter(oCommand, "Units", SqlDbType.Int, intUnits);
            db.AddInParameter(oCommand, "Certno", SqlDbType.VarChar, strCertno);
            db.AddInParameter(oCommand, "Certacc", SqlDbType.VarChar, strCertacc);
            db.AddInParameter(oCommand, "VerificationStatus", SqlDbType.Bit, blnVerificationStatus);
            db.AddInParameter(oCommand, "UnVerificationStatus", SqlDbType.Bit, blnUnVerificationStatus);
            db.AddInParameter(oCommand, "JBookCross", SqlDbType.Bit, blnJBookCross);
            if (datDateVerified != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "DateVerified", SqlDbType.DateTime, datDateVerified);
            }
            else
            {
                db.AddInParameter(oCommand, "DateVerified", SqlDbType.DateTime, SqlDateTime.Null);
            }
            if (datDateSent != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "DateSent", SqlDbType.DateTime, datDateSent);
            }
            else
            {
                db.AddInParameter(oCommand, "DateSent", SqlDbType.DateTime, SqlDateTime.Null);
            }
            db.AddInParameter(oCommand, "LodgementReason", SqlDbType.VarChar, strLodgementReason);
            db.AddInParameter(oCommand, "Comments", SqlDbType.NText, strComments);
            db.AddInParameter(oCommand, "UnitPrice", SqlDbType.Money, decUnitPrice);

            if (datDateRec != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "DateRec", SqlDbType.DateTime, datDateRec);
            }
            else
            {
                db.AddInParameter(oCommand, "DateRec", SqlDbType.DateTime, SqlDateTime.Null);
            }
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId.Trim());
            db.AddInParameter(oCommand, "CustNotify", SqlDbType.Bit, blnCustNotify);
            db.AddInParameter(oCommand, "CertificateCollected", SqlDbType.Bit, blnCertificateCollected);
            db.AddInParameter(oCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());
            db.AddInParameter(oCommand, "CrossBuyer", SqlDbType.VarChar, strCrossBuyer.Trim());
            db.AddInParameter(oCommand, "Product", SqlDbType.VarChar, oStkParam.Product.Trim());
            db.AddInParameter(oCommand, "BatchNo", SqlDbType.VarChar, strBatchNo.Trim());
            db.AddInParameter(oCommand, "IsDematerialization", SqlDbType.Bit, blnIsDematerialization);
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "CERTREGISTER");
            return oCommand;

        }
        #endregion






        #region Edit Certificate Verification Status and DateReturn Only and Return A Command
        public SqlCommand EditStatusAndDateReturnCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("VerificationEditStatusAndDateReturn") as SqlCommand;

            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(oCommand, "VerificationStatus", SqlDbType.Bit, blnVerificationStatus);
            db.AddInParameter(oCommand, "UnVerificationStatus", SqlDbType.Bit, blnUnVerificationStatus);
            if (datDateVerified != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "DateVerified", SqlDbType.DateTime, datDateVerified);

            }
            else
            {
                db.AddInParameter(oCommand, "DateVerified", SqlDbType.DateTime, SqlDateTime.Null);

            }


            return oCommand;

        }
        #endregion

        #region Get Verification
        public bool GetVerification(DataGeneral.PostStatus TransStatus)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.All)
            {
                dbCommand = db.GetStoredProcCommand("VerificationSelect") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("VerificationSelectPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("VerificationSelectUnPosted") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "Code", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strCode = thisRow[0]["TransNo"].ToString();
                if ( thisRow[0]["EffDate"] == null || thisRow[0]["EffDate"].ToString().Trim() == "")
                {
                    datEffdate = DateTime.MinValue;
                }
                else
                {
                    datEffdate = DateTime.Parse(thisRow[0]["EffDate"].ToString());
                }
                if ( thisRow[0]["DateSent"] == null || thisRow[0]["DateSent"].ToString().Trim() == "")
                {
                    datDateSent = DateTime.MinValue;
                }
                else
                {
                    datDateSent = DateTime.Parse(thisRow[0]["DateSent"].ToString());
                }
                if ( thisRow[0]["DateVerified"] == null || thisRow[0]["DateVerified"].ToString().Trim() == "")
                {
                    datDateVerified = DateTime.MinValue;
                }
                else
                {
                    datDateVerified = DateTime.Parse(thisRow[0]["DateVerified"].ToString());
                }
                if ( thisRow[0]["DateRec"] == null || thisRow[0]["DateRec"].ToString().Trim() == "")
                {
                    datDateRec = DateTime.MinValue;
                }
                else
                {
                    datDateRec = DateTime.Parse(thisRow[0]["DateRec"].ToString());
                }

                strCustNo = thisRow[0]["CustNo"].ToString();
                strSurname = thisRow[0]["Surname"].ToString();
                strOthername = thisRow[0]["Othername"].ToString();
                strFirstname = thisRow[0]["Firstname"].ToString();
                strStockcode = thisRow[0]["Stockcode"].ToString();
                intUnits = int.Parse(thisRow[0]["Units"].ToString());
                strCertacc = thisRow[0]["Certacc"].ToString();
                strCertno = thisRow[0]["Certno"].ToString();
                decUnitPrice = decimal.Parse(thisRow[0]["UnitPrice"].ToString());
                blnVerificationStatus = bool.Parse(thisRow[0]["VerificationStatus"].ToString());
                blnUnVerificationStatus = bool.Parse(thisRow[0]["UnVerificationStatus"].ToString());
                blnJBookCross = bool.Parse(thisRow[0]["JBookCross"].ToString());
                strLodgementReason = thisRow[0]["LodgementReason"].ToString();
                strComments = thisRow[0]["Comments"].ToString();

                blnCustNotify = bool.Parse(thisRow[0]["CustNotify"].ToString());
                blnCertificateCollected = thisRow[0]["CertificateCollected"] != null && thisRow[0]["CertificateCollected"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["CertificateCollected"].ToString()) : false;
                strBonus = thisRow[0]["Bonus"].ToString();
                strCrossBuyer = thisRow[0]["CrossBuyer"].ToString();
                strUserId = thisRow[0]["UserId"].ToString();
                strBatchNo = thisRow[0]["BatchNo"] != null &&  thisRow[0]["BatchNo"].ToString().Trim() != "" ? thisRow[0]["BatchNo"].ToString() : "";
                strStartingTransNumber = thisRow[0]["StartingTransNumber"] != null && thisRow[0]["StartingTransNumber"].ToString().Trim() != "" ? thisRow[0]["StartingTransNumber"].ToString() : "";
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }


            return blnStatus;
        }
        #endregion

        //#region Get Certificate Verification
        //public bool GetVerification()
        //{
        //    bool blnStatus = false;
        //    //Create the connection
        //    SqlConnection oCon = new SqlConnection(ConfigurationManager.ConnectionStrings["GlobalSuitedb"].ToString());
        //    string strSQL = "SELECT * from Stkb_JobbingBook WHERE [Transno] =" + strCode;

        //    //Create the data adapter
        //    SqlDataAdapter oDA = new SqlDataAdapter(strSQL, oCon);

        //    //Create a data set
        //    DataSet oDS = new DataSet();

        //    //Set the select command
        //    oDA.SelectCommand.CommandText = strSQL;

        //    oDA.Fill(oDS, "Verification");

        //    try
        //    {
        //        DataTable thisTable = oDS.Tables["Verification"];
        //        DataRow[] thisRow = thisTable.Select();
        //        if (thisRow.Length == 1)
        //        {
        //            strCode = thisRow[0]["TransNo"].ToString();
        //            if (thisRow[0]["EffDate"].ToString() == "" || thisRow[0]["EffDate"].ToString() == null)
        //            {
        //                datEffdate = DateTime.MinValue;
        //            }
        //            else
        //            {
        //                datEffdate = DateTime.Parse(thisRow[0]["EffDate"].ToString());
        //            }
        //            if (thisRow[0]["DateSent"].ToString() == "" || thisRow[0]["DateSent"].ToString() == null)
        //            {
        //                datDateSent = DateTime.MinValue;
        //            }
        //            else
        //            {
        //                datDateSent = DateTime.Parse(thisRow[0]["DateSent"].ToString());
        //            }
        //            if (thisRow[0]["DateVerified"].ToString() == "" || thisRow[0]["DateVerified"].ToString() == null)
        //            {
        //                datDateVerified = DateTime.MinValue;
        //            }
        //            else
        //            {
        //                datDateVerified = DateTime.Parse(thisRow[0]["DateVerified"].ToString());
        //            }
        //            if (thisRow[0]["DateRec"].ToString() == "" || thisRow[0]["DateRec"].ToString() == null)
        //            {
        //                datDateRec = DateTime.MinValue;
        //            }
        //            else
        //            {
        //                datDateRec = DateTime.Parse(thisRow[0]["DateRec"].ToString());
        //            }

        //            strCustNo = thisRow[0]["CustNo"].ToString();
        //            strSurname = thisRow[0]["Surname"].ToString();
        //            strOthername = thisRow[0]["Othername"].ToString();
        //            strFirstname = thisRow[0]["Firstname"].ToString();
        //            strStockcode = thisRow[0]["Stockcode"].ToString();
        //            intUnits = int.Parse(thisRow[0]["Units"].ToString());
        //            strCertacc = thisRow[0]["Certacc"].ToString();
        //            strCertno = thisRow[0]["Certno"].ToString();
        //            decUnitPrice = decimal.Parse(thisRow[0]["UnitPrice"].ToString());
        //            blnVerificationStatus = bool.Parse(thisRow[0]["VerificationStatus"].ToString());
        //            blnUnVerificationStatus = bool.Parse(thisRow[0]["UnVerificationStatus"].ToString());
        //            blnJBookCross = bool.Parse(thisRow[0]["JBookCross"].ToString());
        //            strLodgementReason = thisRow[0]["LodgementReason"].ToString();
        //            strComments = thisRow[0]["Comments"].ToString();

        //            blnCustNotify = bool.Parse(thisRow[0]["CustNotify"].ToString());
        //            blnCertificateCollected = bool.Parse(thisRow[0]["CertificateCollected"].ToString());
        //            strCrossBuyer = thisRow[0]["CrossBuyer"].ToString();
        //            strUserId = thisRow[0]["UserId"].ToString();
        //            blnStatus = true;
        //        }
        //        else
        //        {
        //            blnStatus = false;
        //        }
        //    }
        //    catch (InvalidCastException e)
        //    {
        //        blnStatus = false;
        //        strSurname = e.Message;
        //    }
        //    return blnStatus;
        //}
        //#endregion

        #region Get All Certificate Deposits Sorted By Trans No From Biggest
        public DataSet GetAllCert(string strUserBranch)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("VerificationSelectByAllOrderByID") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockcode.Trim());
            db.AddInParameter(oCommand, "Certno", SqlDbType.VarChar, strCertno.Trim());
            db.AddInParameter(oCommand, "Certacc", SqlDbType.VarChar, strCertacc.Trim());
            db.AddInParameter(oCommand, "Customer", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId.Trim());
            if (datEffdate != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, datEffdate);
            }
            else
            {
                db.AddInParameter(oCommand, "Effdate", SqlDbType.DateTime, SqlDateTime.Null);
            }
            db.AddInParameter(oCommand, "Unit", SqlDbType.BigInt, intUnits);
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion

        

        #region Get Certificate Script Receipt
        public DataSet GetScript(string strCustId, DateTime strTo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ScriptGet") as SqlCommand;
            db.AddInParameter(dbCommand, "strCustId", SqlDbType.Char, strCustId);
            db.AddInParameter(dbCommand, "strTo", SqlDbType.DateTime, strTo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion


        #region Get Verification Report
        public DataSet GetVerificationReport(string strCustomer, DateTime datFrom, DateTime datTo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("rpt_JobbingBook") as SqlCommand;
            db.AddInParameter(dbCommand, "Customer", SqlDbType.VarChar, strCustomer);
            db.AddInParameter(dbCommand, "From", SqlDbType.DateTime, datFrom);
            db.AddInParameter(dbCommand, "To", SqlDbType.DateTime, datTo);

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Verification Register All Report
        public DataSet GetVerificationRegisterAll(string strCustomer, DateTime datFrom, DateTime datTo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("rpt_JobbingBook") as SqlCommand;
            db.AddInParameter(dbCommand, "Customer", SqlDbType.VarChar, strCustomer);
            db.AddInParameter(dbCommand, "From", SqlDbType.DateTime, datFrom);
            db.AddInParameter(dbCommand, "To", SqlDbType.DateTime, datTo);

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Verification Letter Report
        public DataSet GetVerificationLetter(string strCustomer, DateTime datFrom, DateTime datTo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("rpt_JobbingBook") as SqlCommand;
            db.AddInParameter(dbCommand, "Customer", SqlDbType.VarChar, strCustomer);
            db.AddInParameter(dbCommand, "From", SqlDbType.DateTime, datFrom);
            db.AddInParameter(dbCommand, "To", SqlDbType.DateTime, datTo);

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("VerificationDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId.Trim());

            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Get All UnPosted Certifcates Order By ID Desc
        public DataSet GetUnPostedDesc()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("VerificationSelectByUnPostedOrderByIDDesc") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockcode.Trim());
            db.AddInParameter(oCommand, "Certno", SqlDbType.VarChar, strCertno.Trim());
            db.AddInParameter(oCommand, "Certacc", SqlDbType.VarChar, strCertacc.Trim());
            db.AddInParameter(oCommand, "Customer", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId.Trim());
            if (datEffdate != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, datEffdate);
            }
            else
            {
                db.AddInParameter(oCommand, "Effdate", SqlDbType.DateTime, SqlDateTime.Null);
            }
            db.AddInParameter(oCommand, "Unit", SqlDbType.BigInt, intUnits);
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion

        #region Get All UnPosted Certifcates Order By ID Asc
        public DataSet GetUnPostedAsc()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("VerificationSelectByUnPostedOrderByIDAsc") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockcode.Trim());
            db.AddInParameter(oCommand, "Certno", SqlDbType.VarChar, strCertno.Trim());
            db.AddInParameter(oCommand, "Certacc", SqlDbType.VarChar, strCertacc.Trim());
            db.AddInParameter(oCommand, "Customer", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId.Trim());
            if (datEffdate != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, datEffdate);
            }
            else
            {
                db.AddInParameter(oCommand, "Effdate", SqlDbType.DateTime, SqlDateTime.Null);
            }
            db.AddInParameter(oCommand, "Unit", SqlDbType.BigInt, intUnits);
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion

        #region Get All Verified Certifcates Order By ID Desc
        public DataSet GetVerifiedAllDesc()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("VerificationSelectByVerifiedOrderByIDDesc") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockcode.Trim());
            db.AddInParameter(oCommand, "Certno", SqlDbType.VarChar, strCertno.Trim());
            db.AddInParameter(oCommand, "Certacc", SqlDbType.VarChar, strCertacc.Trim());
            db.AddInParameter(oCommand, "Customer", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId.Trim());
            if (datEffdate != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, datEffdate);
            }
            else
            {
                db.AddInParameter(oCommand, "Effdate", SqlDbType.DateTime, SqlDateTime.Null);
            }
            db.AddInParameter(oCommand, "Unit", SqlDbType.BigInt, intUnits);
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion

        #region Get All Verified Certifcates Order By ID Asc
        public DataSet GetVerifiedAllAsc()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("VerificationSelectByVerifiedOrderByIDAsc") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockcode.Trim());
            db.AddInParameter(oCommand, "Certno", SqlDbType.VarChar, strCertno.Trim());
            db.AddInParameter(oCommand, "Certacc", SqlDbType.VarChar, strCertacc.Trim());
            db.AddInParameter(oCommand, "Customer", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId.Trim());
            if (datEffdate != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, datEffdate);
            }
            else
            {
                db.AddInParameter(oCommand, "Effdate", SqlDbType.DateTime, SqlDateTime.Null);
            }
            db.AddInParameter(oCommand, "Unit", SqlDbType.BigInt, intUnits);
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion

        #region Get All Irregular Certifcates Order By ID Desc
        public DataSet GetIrregularAllDesc()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("VerificationSelectByIrregularOrderByIDDesc") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockcode.Trim());
            db.AddInParameter(oCommand, "Certno", SqlDbType.VarChar, strCertno.Trim());
            db.AddInParameter(oCommand, "Certacc", SqlDbType.VarChar, strCertacc.Trim());
            db.AddInParameter(oCommand, "Customer", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId.Trim());
            if (datEffdate != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, datEffdate);
            }
            else
            {
                db.AddInParameter(oCommand, "Effdate", SqlDbType.DateTime, SqlDateTime.Null);
            }
            db.AddInParameter(oCommand, "Unit", SqlDbType.BigInt, intUnits);
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion

        #region Get All Irregular Certifcates Order By ID Asc
        public DataSet GetIrregularAllAsc()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("VerificationSelectByIrregularOrderByIDAsc") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockcode.Trim());
            db.AddInParameter(oCommand, "Certno", SqlDbType.VarChar, strCertno.Trim());
            db.AddInParameter(oCommand, "Certacc", SqlDbType.VarChar, strCertacc.Trim());
            db.AddInParameter(oCommand, "Customer", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId.Trim());
            if (datEffdate != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, datEffdate);
            }
            else
            {
                db.AddInParameter(oCommand, "Effdate", SqlDbType.DateTime, SqlDateTime.Null);
            }
            db.AddInParameter(oCommand, "Unit", SqlDbType.BigInt, intUnits);
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion

        #region Check That Certificate Number Already Exist For Existing Record
        public bool CertNoExist()
        {
            bool blnStatus = false;
            if (strCertno != null && strCertno.Trim() != "")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("VerificationSelectByCertNoExist") as SqlCommand;
                db.AddInParameter(dbCommand, "Transno", SqlDbType.VarChar, strCode.Trim());
                db.AddInParameter(dbCommand, "CertNo", SqlDbType.VarChar, strCertno.Trim());
                db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockcode.Trim());
                db.AddInParameter(dbCommand, "SaveType", SqlDbType.VarChar, strSaveType.Trim());
                DataSet oDs = db.ExecuteDataSet(dbCommand);
                if (oDs.Tables[0].Rows.Count > 0)
                {
                    blnStatus = true;
                }
            }
            return blnStatus;
        }
        #endregion

        #region Check That Certificate Number Already Exist For New Record And Recall Information
        public bool CertNoExistNoTransNoWithRecall()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("VerificationSelectByCertNoExistNoTransnoWithRecall") as SqlCommand;
            db.AddInParameter(dbCommand, "CertNo", SqlDbType.VarChar, strCertno.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockcode.Trim());

            try
            {

                DataSet oDS = db.ExecuteDataSet(dbCommand);
                DataTable thisTable = oDS.Tables[0];
                DataRow[] thisRow = thisTable.Select();
                if (thisRow.Length == 1)
                {
                    strCode = thisRow[0]["TransNo"].ToString();
                    if (thisRow[0]["EffDate"].ToString() == "" || thisRow[0]["EffDate"].ToString() == null)
                    {
                        datEffdate = DateTime.MinValue;
                    }
                    else
                    {
                        datEffdate = DateTime.Parse(thisRow[0]["EffDate"].ToString());
                    }
                    if (thisRow[0]["DateSent"].ToString() == "" || thisRow[0]["DateSent"].ToString() == null)
                    {
                        datDateSent = DateTime.MinValue;
                    }
                    else
                    {
                        datDateSent = DateTime.Parse(thisRow[0]["DateSent"].ToString());
                    }
                    if (thisRow[0]["DateVerified"].ToString() == "" || thisRow[0]["DateVerified"].ToString() == null)
                    {
                        datDateVerified = DateTime.MinValue;
                    }
                    else
                    {
                        datDateVerified = DateTime.Parse(thisRow[0]["DateVerified"].ToString());
                    }
                    if (thisRow[0]["DateRec"].ToString() == "" || thisRow[0]["DateRec"].ToString() == null)
                    {
                        datDateRec = DateTime.MinValue;
                    }
                    else
                    {
                        datDateRec = DateTime.Parse(thisRow[0]["DateRec"].ToString());
                    }

                    strCustNo = thisRow[0]["CustNo"].ToString();
                    strSurname = thisRow[0]["Surname"].ToString();
                    strOthername = thisRow[0]["Othername"].ToString();
                    strFirstname = thisRow[0]["Firstname"].ToString();
                    strStockcode = thisRow[0]["Stockcode"].ToString();
                    intUnits = int.Parse(thisRow[0]["Units"].ToString());
                    strCertacc = thisRow[0]["Certacc"].ToString();
                    strCertno = thisRow[0]["Certno"].ToString();
                    decUnitPrice = decimal.Parse(thisRow[0]["UnitPrice"].ToString());
                    blnVerificationStatus = bool.Parse(thisRow[0]["VerificationStatus"].ToString());
                    blnUnVerificationStatus = bool.Parse(thisRow[0]["UnVerificationStatus"].ToString());
                    blnJBookCross = bool.Parse(thisRow[0]["JBookCross"].ToString());
                    strLodgementReason = thisRow[0]["LodgementReason"].ToString();
                    strComments = thisRow[0]["Comments"].ToString();
                    blnCustNotify = bool.Parse(thisRow[0]["CustNotify"].ToString());
                    blnCertificateCollected = bool.Parse(thisRow[0]["CertificateCollected"].ToString());
                    strCrossBuyer = thisRow[0]["CrossBuyer"].ToString();
                    strUserId = thisRow[0]["UserId"].ToString();
                    blnStatus = true;
                }
                else
                {
                    strCode = "";
                    datEffdate = DateTime.MinValue;
                    datDateSent = DateTime.MinValue;
                    datDateVerified = DateTime.MinValue;
                    datDateRec = DateTime.MinValue;
                    strCustNo = "";
                    strSurname = "";
                    strOthername = "";
                    strFirstname = "";
                    strStockcode = "";
                    intUnits = 0;
                    strCertacc = "";
                    strCertno = "";
                    decUnitPrice = 0;
                    blnVerificationStatus = false;
                    blnUnVerificationStatus = false;
                    blnJBookCross = false;
                    strLodgementReason = "";
                    strComments = "";
                    blnCustNotify = false;
                    blnCertificateCollected = false;
                    strCrossBuyer = "";
                    strUserId = "";
                    blnStatus = false;
                }
            }
            catch
            {
                blnStatus = false;

            }

            return blnStatus;
        }
        #endregion

        #region Edit Certificate Verification Status By Transaction Number and Return A Command
        public SqlCommand EditVerifiedByTransNoCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("VerificationEditVerifiedByTransNo") as SqlCommand;
            db.AddInParameter(dbCommand, "Code", SqlDbType.VarChar,strCode.Trim());
            db.AddInParameter(dbCommand, "DateVerified", SqlDbType.DateTime, datDateVerified);
            return dbCommand;
        }
        #endregion

        #region Edit Certificate Verification Status By CustNo,Stock,CertNo For Regular To  and Return A Command
        public SqlCommand EditVerifiedByCustStockCertCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("VerificationEditVerifiedByCustStockCert") as SqlCommand;
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(dbCommand, "Stockcode", SqlDbType.VarChar, strStockcode.Trim());
            db.AddInParameter(dbCommand, "Certno", SqlDbType.VarChar, strCertno.Trim());
            db.AddInParameter(dbCommand, "DateVerified", SqlDbType.DateTime, datDateVerified);
            return dbCommand;
        }
        #endregion

        #region Edit Certificate Verification Status By CustNo,Stock,CertNo For Regular To and Return A Command
        public SqlCommand EditVerifiedCertNoByCustStockCertCommand(string strNewCertNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("VerificationEditVerifiedCertNoByCustStockCert") as SqlCommand;
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(dbCommand, "Stockcode", SqlDbType.VarChar, strStockcode.Trim());
            db.AddInParameter(dbCommand, "Certno", SqlDbType.VarChar, strCertno.Trim());
            db.AddInParameter(dbCommand, "CertnoNew", SqlDbType.VarChar, strNewCertNumber.Trim());
            db.AddInParameter(dbCommand, "DateVerified", SqlDbType.DateTime, datDateVerified);
            return dbCommand;

        }
        #endregion

        #region Get All Returned Irregular Certifcates Order By ID Asc
        public DataSet GetRetIrregularAllAsc()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("VerificationSelectByRetIrregularOrderByIDAsc") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Edit Certificate Verification Status For Irregular Certificate  and Return A Command
        public SqlCommand EditRetIrregularCommand(DateTime datReVerifyDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("VerificationEditRetIrregular") as SqlCommand;

            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCode.Trim());
            if (datReVerifyDate != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "ReVerifydate", SqlDbType.DateTime, datReVerifyDate);

            }
            else
            {
                db.AddInParameter(oCommand, "ReVerifydate", SqlDbType.DateTime, SqlDateTime.Null);

            }

            if (datDateVerified != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "DateVerified", SqlDbType.DateTime, datDateVerified);

            }
            else
            {
                db.AddInParameter(oCommand, "DateVerified", SqlDbType.DateTime, SqlDateTime.Null);

            }
            if (datDateSent != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "DateSent", SqlDbType.DateTime, datDateSent);
            }
            else
            {
                db.AddInParameter(oCommand, "DateSent", SqlDbType.DateTime, SqlDateTime.Null);
            }

            if (datDateRec != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "DateRec", SqlDbType.DateTime, datDateRec);

            }
            else
            {
                db.AddInParameter(oCommand, "DateRec", SqlDbType.DateTime, SqlDateTime.Null);

            }
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId.Trim());

            return oCommand;

        }
        #endregion

        #region Get Certificate Holding Reason
        public DataSet GetHoldingFor()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ParamHoldingSelectAll") as SqlCommand;
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region Get All UnPosted Certificates Selected By Customer and StockCode Order By Trans ID Desc
        public DataSet GetUnPostedGivenCustStock()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("VerificationSelectByUnPostedGivenCustStockOrderByIDDesc") as SqlCommand;
            db.AddInParameter(dbCommand, "Customer", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockcode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Check Cert Exist by CustNo,Stock and Cert and Return Irregular and Verified
        public bool ChkCertExistByCustStockCert()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("VerificationChkCertExistByCustStockCert") as SqlCommand;
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(dbCommand, "Stockcode", SqlDbType.VarChar, strStockcode.Trim());
            db.AddInParameter(dbCommand, "Certno", SqlDbType.VarChar, strCertno.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strCode = thisRow[0]["TransNo"].ToString();
                if (thisRow[0]["VerificationStatus"] != null)
                {
                    blnVerificationStatus = bool.Parse(thisRow[0]["VerificationStatus"].ToString());
                }
                else
                {
                    blnVerificationStatus = false;
                }
                if (thisRow[0]["UnVerificationStatus"] != null)
                {
                    blnUnVerificationStatus = bool.Parse(thisRow[0]["UnVerificationStatus"].ToString());
                }
                else
                {
                    blnUnVerificationStatus = false;
                }
                decUnitPrice = decimal.Parse(thisRow[0]["UnitPrice"].ToString());
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Check Cert Exist by CustNo,Stock and Cert Not Returned
        public bool ChkCertExistByCustStockCertNotReturned()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("VerificationChkCertExistByCustStockCertNotReturned") as SqlCommand;
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(dbCommand, "Stockcode", SqlDbType.VarChar, strStockcode.Trim());
            db.AddInParameter(dbCommand, "Certno", SqlDbType.VarChar, strCertno.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strCode = thisRow[0]["TransNo"].ToString();
                decUnitPrice = decimal.Parse(thisRow[0]["UnitPrice"].ToString());
                blnStatus = true;
            }
            //else if (thisRow.Length > 1)
            //{
            //    throw new Exception("Check File CUST AC: " + strCustNo.Trim()
            //    + " Security: " + strStockcode.Trim() + " Cert No: " + strCertno.Trim()
            //    + " is double in GlobalSuite Software file.Manually DELETE THE EXISTING UNVERIFIED certificate of this transaction from the Certificate Verification Form for a successful upload");
            //}
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Check Cert Exist by CustNo,Stock and Unit Not Returned Return Cert No
        public bool ChkCertExistByCustStockUnitNotReturned()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("VerificationChkCertExistByCustStockUnitNotReturned") as SqlCommand;
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(dbCommand, "Stockcode", SqlDbType.VarChar, strStockcode.Trim());
            db.AddInParameter(dbCommand, "Units", SqlDbType.BigInt,intUnits);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strCode = thisRow[0]["TransNo"].ToString();
                strCertno = thisRow[0]["Certno"].ToString();
                decUnitPrice = decimal.Parse(thisRow[0]["UnitPrice"].ToString());
                blnStatus = true;
            }
            //else if (thisRow.Length > 1)
            //{
            //    throw new Exception("Check File CUST AC: " + strCustNo.Trim()
            //    + " Security: " + strStockcode.Trim() + " Unit: " + intUnits.ToString()
            //    + " is double in GlobalSuite Software file.Manually DELETE THE EXISTING UNVERIFIED certificate of this transaction from the Certificate Verification Form for a successful upload");
            //}
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Check Status Is Sent
        public bool ChkStatusIsSent(string strStatus,string strRecordNo)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strStatus.Trim() == "DEPOSIT")
            {
                dbCommand = db.GetStoredProcCommand("VerificationChkStatusIsSentDeposit") as SqlCommand;
            }
            else if (strStatus.Trim() == "RECEIVED")
            {
                dbCommand = db.GetStoredProcCommand("VerificationChkStatusIsSentReceived") as SqlCommand;
            }
            else if (strStatus.Trim() == "IRREGULAR")
            {
                dbCommand = db.GetStoredProcCommand("VerificationChkStatusIsSentIrregular") as SqlCommand;
            }
            else if (strStatus.Trim() == "VERIFIED")
            {
                dbCommand = db.GetStoredProcCommand("VerificationChkStatusIsSentVerified") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strRecordNo.Trim());
            if (db.ExecuteScalar(dbCommand) != null)
            {
                if (db.ExecuteScalar(dbCommand).ToString() != "")
                {
                    blnStatus = bool.Parse(db.ExecuteScalar(dbCommand).ToString());
                }
                else
                {
                    blnStatus = false;
                }
            }
            else
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Update Status Is Sent
        public void UpdateStatusIsSent(string strStatus, string strRecordNo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strStatus.Trim() == "DEPOSIT")
            {
                dbCommand = db.GetStoredProcCommand("VerificationUpdateStatusIsSentDeposit") as SqlCommand;
            }
            else if (strStatus.Trim() == "RECEIVED")
            {
                dbCommand = db.GetStoredProcCommand("VerificationUpdateStatusIsSentReceived") as SqlCommand;
            }
            else if (strStatus.Trim() == "IRREGULAR")
            {
                dbCommand = db.GetStoredProcCommand("VerificationUpdateStatusIsSentIrregular") as SqlCommand;
            }
            else if (strStatus.Trim() == "VERIFIED")
            {
                dbCommand = db.GetStoredProcCommand("VerificationUpdateStatusIsSentVerified") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strRecordNo.Trim());
            db.ExecuteNonQuery(dbCommand).ToString();
        }
        #endregion

        #region Get All Certifcates By Cust Stock And Date
        public DataSet GetAllCertByCustStockDate()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("VerificationSelectCustStockDate") as SqlCommand;
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockcode.Trim());
            db.AddInParameter(oCommand, "Customer", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, datEffdate);
            db.AddInParameter(oCommand, "EffDateTo", SqlDbType.DateTime, datDateSent);
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion

        #region Save Amount To Word
        public void SaveAmountWord()
        {
            NumberToWords oNumberToWords = new NumberToWords();
            foreach (DataRow oRow in GetAllCertByCustStockDate().Tables[0].Rows)
            {
                string strAmountWord = oNumberToWords.ConvertToWordNoOnly(oRow["Units"].ToString());
                SaveIndividualAmountWord(oRow["TransNo"].ToString(), strAmountWord);

            }
        }
        #endregion

        #region Save Individual Amount To Word
        public void SaveIndividualAmountWord(string strCodeNumber,string strAmountInWord)
        {
            
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            oCommand = db.GetStoredProcCommand("VerificationSaveAmountWord") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCodeNumber.Trim());
            db.AddInParameter(oCommand, "AmountWord", SqlDbType.VarChar, strAmountInWord.Trim());
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        #region Get All Certifcates For Transfer Form
        public DataSet GetAllCertForTransferForm(string strStockRegistrar)
        {
            StkParam oStkParam = new StkParam();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("VerificationSelectForTransferForm") as SqlCommand;
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockcode.Trim());
            db.AddInParameter(oCommand, "Customer", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(oCommand, "Registrar", SqlDbType.VarChar, strStockRegistrar.Trim());
            db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, datEffdate);
            db.AddInParameter(oCommand, "EffDateTo", SqlDbType.DateTime, datEffdateTo);
            db.AddInParameter(oCommand, "ProductCode", SqlDbType.VarChar,oStkParam.Product.Trim());
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion

        #region Get All Cert For CDF Report
        public DataSet GetAllCertForCDFReport(string strRegistrarId)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("VerificationSelectAllForCDFReport") as SqlCommand;
            db.AddInParameter(dbCommand, "Customer", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockcode.Trim());
            db.AddInParameter(dbCommand, "Registrar", SqlDbType.VarChar, strRegistrarId.Trim());
            db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime, datEffdate);
            db.AddInParameter(dbCommand, "EffDateTo", SqlDbType.DateTime, datEffdateTo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Cert For CDF Excel
        public DataSet GetAllCertForCDFExcelGenerate(string strRegistrarId)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("VerificationSelectAllForCDFExcelGenerate") as SqlCommand;
            db.AddInParameter(dbCommand, "Customer", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockcode.Trim());
            db.AddInParameter(dbCommand, "Registrar", SqlDbType.VarChar, strRegistrarId.Trim());
            db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime, datEffdate);
            db.AddInParameter(dbCommand, "EffDateTo", SqlDbType.DateTime, datEffdateTo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Regular And Returned
        public void GetRegularAndReturned(string strTransactionNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("VerificationSelectRegularAndReturned") as SqlCommand;
            db.AddInParameter(dbCommand, "Code", SqlDbType.VarChar, strTransactionNumber.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                blnVerificationStatus = bool.Parse(thisRow[0]["VerificationStatus"].ToString());
                blnUnVerificationStatus = bool.Parse(thisRow[0]["UnVerificationStatus"].ToString());
                strStartingTransNumber = thisRow[0]["StartingTransNumber"] != null && thisRow[0]["StartingTransNumber"].ToString().Trim() != "" ? thisRow[0]["StartingTransNumber"].ToString() : thisRow[0]["TransNo"].ToString();
            }
            else
            {
                blnVerificationStatus = false;
                blnUnVerificationStatus = false;
                strStartingTransNumber = "";
            }
        }
        #endregion

        #region Save Starting Number Return Command
        public SqlCommand SaveStartingNumberCommand(string strTransactionNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("VerificationSaveStartingNumber") as SqlCommand;
            db.AddInParameter(dbCommand, "Code", SqlDbType.VarChar, strTransactionNumber.Trim());
            db.AddInParameter(dbCommand, "StartingTransNumber", SqlDbType.VarChar, strStartingTransNumber.Trim());
            return dbCommand;
        }
        #endregion
        

    }

}

