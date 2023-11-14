using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using BaseUtility.Business;

namespace CapitalMarket.Business
{
    public class PublicOffer
    {
        #region Declarations
        private string strTxn, strOfferName, strStockCode, strRCStockCode;
        private decimal decSubUnits, decUnitPrice;
        private float fltNominalPrice;
        private DateTime datOpenDate, datCloseDate;
        private string strNameIssueHouse, strRCIssueHouse, strNameIssueHouse2, strRCIssueHouse2;
        private string strNameRegistrar, strRegistrarAddress1, strRegistrarAddress2, strRegistrarAddress3;
        private char charOfferType;
        private string strInputBy,strSaveType;
        private DateTime datHeldDate;
        private bool blnRemitComplete;
        private string strAmount1, strAmount2;
        private string strOfferAcctMas, strOfferAcctSub,strBranch;
        #endregion

        #region Properties
        public float NominalPrice
        {
            set { fltNominalPrice = value; }
            get { return fltNominalPrice; }
        }
        public string Amount2
        {
            set { strAmount2 = value; }
            get { return strAmount2; }
        }
        public string Amount1
        {
            set { strAmount1 = value; }
            get { return strAmount1; }
        }
        public bool RemitComplete
        {
            set { blnRemitComplete = value; }
            get { return blnRemitComplete; }
        }
        public DateTime HeldDate
        {
            set { datHeldDate = value; }
            get { return datHeldDate; }
        }
        
        public string InputBy
        {
            set { strInputBy = value; }
            get { return strInputBy; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        public char OfferType
        {
            set { charOfferType = value; }
            get { return charOfferType; }
        }
        public string RegistrarAddress3
        {
            set { strRegistrarAddress3 = value; }
            get { return strRegistrarAddress3; }
        }
        public string RegistrarAddress2
        {
            set { strRegistrarAddress2 = value; }
            get { return strRegistrarAddress2; }
        }
        public string RegistrarAddress1
        {
            set { strRegistrarAddress1 = value; }
            get { return strRegistrarAddress1; }
        }
        public string NameRegistrar
        {
            set { strNameRegistrar = value; }
            get { return strNameRegistrar; }
        }
        public string RCIssueHouse2
        {
            set { strRCIssueHouse2 = value; }
            get { return strRCIssueHouse2; }
        }
        public string NameIssueHouse2
        {
            set { strNameIssueHouse2 = value; }
            get { return strNameIssueHouse2; }
        }
        public string RCIssueHouse
        {
            set { strRCIssueHouse = value; }
            get { return strRCIssueHouse; }
        }
        public string NameIssueHouse
        {
            set { strNameIssueHouse = value; }
            get { return strNameIssueHouse; }
        }
        public decimal UnitPrice
        {
            set { decUnitPrice = value; }
            get { return decUnitPrice; }
        }
        public decimal SubUnits
        {
            set { decSubUnits = value; }
            get { return decSubUnits; }
        }
        public DateTime CloseDate
        {
            set { datCloseDate = value; }
            get { return datCloseDate; }
        }
        public DateTime OpenDate
        {
            set { datOpenDate = value; }
            get { return datOpenDate; }
        }
        public string RCStockCode
        {
            set { strRCStockCode = value; }
            get { return strRCStockCode; }
        }
        public string StockCode
        {
            set { strStockCode = value; }
            get { return strStockCode; }
        }

        public string OfferName
        {
            set { strOfferName = value; }
            get { return strOfferName; }
        }
        public string Txn
        {
            set { strTxn = value; }
            get { return strTxn; }
        }
        public string OfferAcctMas
        {
            set { strOfferAcctMas = value; }
            get { return strOfferAcctMas; }
        }
        public string OfferAcctSub
        {
            set { strOfferAcctSub = value; }
            get { return strOfferAcctSub; }
        }
        public string Branch
        {
            set { strBranch = value; }
            get { return strBranch; }
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
            SqlCommand dbCommand = null;
            if (strSaveType == "ADDS")
            {
                dbCommand = db.GetStoredProcCommand("OfferAddNew") as SqlCommand;
                db.AddInParameter(dbCommand, "TableName", SqlDbType.VarChar, "OFFER");
            }
            else if (strSaveType == "EDIT")
            {
                dbCommand = db.GetStoredProcCommand("OfferEdit") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "Txn#", SqlDbType.VarChar, strTxn);
            db.AddInParameter(dbCommand, "OfferName", SqlDbType.VarChar, strOfferName.Trim().ToUpper());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(dbCommand, "RC#StockCode", SqlDbType.VarChar, strRCStockCode);
            db.AddInParameter(dbCommand, "SubUnits", SqlDbType.Decimal, decSubUnits);
            db.AddInParameter(dbCommand, "UnitPrice", SqlDbType.Real, decUnitPrice);
            db.AddInParameter(dbCommand, "NominalPrice", SqlDbType.Float, fltNominalPrice);
            db.AddInParameter(dbCommand, "OpenDate", SqlDbType.DateTime, datOpenDate);
            db.AddInParameter(dbCommand, "CloseDate", SqlDbType.DateTime, datCloseDate);
            db.AddInParameter(dbCommand, "NameIssueHouse", SqlDbType.VarChar, strNameIssueHouse.ToUpper());
            db.AddInParameter(dbCommand, "RC#IssueHouse", SqlDbType.VarChar, strRCIssueHouse);
            db.AddInParameter(dbCommand, "NameIssueHouse2", SqlDbType.VarChar, strNameIssueHouse2.ToUpper());
            db.AddInParameter(dbCommand, "RC#IssueHouse2", SqlDbType.VarChar, strRCIssueHouse2);
            db.AddInParameter(dbCommand, "NameRegistrar", SqlDbType.VarChar, strNameRegistrar.ToUpper());
            db.AddInParameter(dbCommand, "RegistrarAddress1", SqlDbType.VarChar, strRegistrarAddress1.ToUpper());
            db.AddInParameter(dbCommand, "RegistrarAddress2", SqlDbType.VarChar, strRegistrarAddress2.ToUpper());
            db.AddInParameter(dbCommand, "RegistrarAddress3", SqlDbType.VarChar, strRegistrarAddress3.ToUpper());
            db.AddInParameter(dbCommand, "OfferType", SqlDbType.Char, charOfferType);
            db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            if (datHeldDate != DateTime.MinValue)
            {
                db.AddInParameter(dbCommand, "HeldDate", SqlDbType.DateTime, datHeldDate);
            }
            else
            {
                db.AddInParameter(dbCommand, "HeldDate", SqlDbType.DateTime, SqlDateTime.Null);
            }
            db.AddInParameter(dbCommand, "Amount1", SqlDbType.VarChar, strAmount1);
            db.AddInParameter(dbCommand, "Amount2", SqlDbType.VarChar, strAmount2);
            db.AddInParameter(dbCommand, "OfferAcct", SqlDbType.VarChar, strOfferAcctSub.Trim()); 
            
            db.ExecuteNonQuery(dbCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;

        }
        #endregion

        #region Get Public Offer
        public bool GetPublicOffer()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");

            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("OfferSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "Txn", SqlDbType.VarChar, strTxn);
            
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTxn = thisRow[0]["Txn#"].ToString();
                strOfferName = thisRow[0]["OfferName"].ToString();
                datOpenDate = DateTime.Parse(thisRow[0]["OpenDate"].ToString());
                datCloseDate = DateTime.Parse(thisRow[0]["CloseDate"].ToString());
                strStockCode = thisRow[0]["StockCode"].ToString();
                strRCStockCode = thisRow[0]["RC#StockCode"].ToString();
                decSubUnits = decimal.Parse(thisRow[0]["SubUnits"].ToString());
                decUnitPrice = decimal.Parse(thisRow[0]["UnitPrice"].ToString());

                strNameIssueHouse = thisRow[0]["NameIssueHouse"].ToString();
                strRCIssueHouse = thisRow[0]["RC#IssueHouse"].ToString();
                strNameIssueHouse2 = thisRow[0]["NameIssueHouse2"].ToString();
                strRCIssueHouse2 = thisRow[0]["RC#IssueHouse2"].ToString();
                strNameRegistrar = thisRow[0]["NameRegistrar"].ToString();
                strRegistrarAddress1 = thisRow[0]["RegistrarAddress1"].ToString();
                strRegistrarAddress2 = thisRow[0]["RegistrarAddress2"].ToString();
                strRegistrarAddress3 = thisRow[0]["RegistrarAddress3"].ToString();
                charOfferType = char.Parse(thisRow[0]["OfferType"].ToString());
                strInputBy = thisRow[0]["InputBy"].ToString();
                if (thisRow[0]["HeldDate"].ToString() == "" || thisRow[0]["HeldDate"].ToString() == null)
                {
                    datHeldDate = DateTime.MinValue;
                }
                else
                {
                    datHeldDate = DateTime.Parse(thisRow[0]["HeldDate"].ToString());
                }

                blnRemitComplete = bool.Parse(thisRow[0]["RemitComplete"].ToString());
                strAmount1 = thisRow[0]["Amount1"].ToString();
                strAmount2 = thisRow[0]["Amount2"].ToString();
                fltNominalPrice = float.Parse(thisRow[0]["NominalPrice"].ToString());
                strOfferAcctSub = thisRow[0]["OfferAcctSub"].ToString();

                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Check TransNo Exist
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("OfferChkTransNoExist") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTxn.Trim());
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

        #region Get UnRemitted Public Offer
        public DataSet GetUnRemitted()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("OfferSelectUnRemitted") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Public Offer 
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("OfferSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("OfferDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strTxn.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Check That Public Offer Is UnRemitted
        public bool PublicOfferIsUnRemitted()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PublicOfferSelectByUnRemittedSelectByID") as SqlCommand;
            db.AddInParameter(dbCommand, "Txn", SqlDbType.VarChar, strTxn.Trim());
            db.AddOutParameter(dbCommand, "CodeExist", SqlDbType.VarChar, 1);
            db.ExecuteNonQuery(dbCommand);
            if (db.GetParameterValue(dbCommand, "CodeExist").ToString().Trim() == "1")
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

        #region Get All Public Offer Closure Date
        public DataSet GetAllPublicOfferClosureDateForAlertOnly(DateTime datPublicOfferClosureDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("OfferSelectAllClosureDate") as SqlCommand;
            db.AddInParameter(dbCommand, "PublicOfferClosureDate", SqlDbType.DateTime, datPublicOfferClosureDate);
            db.AddInParameter(dbCommand, "NumberOfDaysToStop", SqlDbType.Int, 30);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion
    }
}
