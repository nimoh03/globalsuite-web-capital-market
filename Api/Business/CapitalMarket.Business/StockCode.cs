using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

/// <summary>
/// Summary description for Stock
/// </summary>
/// 
namespace CapitalMarket.Business
{
    public class Stock
    {
        #region Declaration
        private string strSecName, strSecCode, strIndustry, strRegistrar, strInstruType;
        private string strCscsCode, strKeepStockCode;
        private decimal decNominalValue;
        private long lngIntialOutShare;
        private bool blnDelistedByNSE;
        #endregion

        #region Properties
        public string SecName
        {
            set { strSecName = value; }
            get { return strSecName; }
        }
        public string SecCode
        {
            set { strSecCode = value; }
            get { return strSecCode; }
        }
        public string KeepStockCode
        {
            set { strKeepStockCode = value; }
            get { return strKeepStockCode; }
        }
        public string Industry
        {
            set { strIndustry = value; }
            get { return strIndustry; }
        }
        public string Registrar
        {
            set { strRegistrar = value; }
            get { return strRegistrar; }
        }
        public string InstruType
        {
            set { strInstruType = value; }
            get { return strInstruType; }
        }
        
        
        
        public string CscsCode
        {
            set { strCscsCode = value; }
            get { return strCscsCode; }
        }

        public decimal NominalValue
        {
            set { decNominalValue = value; }
            get { return decNominalValue; }
        }

        public long IntialOutShare
        {
            set { lngIntialOutShare = value; }
            get { return lngIntialOutShare; }
        }

        public bool DelistedByNSE
        {
            set { blnDelistedByNSE = value; }
            get { return blnDelistedByNSE; }
        }

        #endregion

        public Stock()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region Enum
        public enum SaveStatus { Nothing, NotExist, Saved }
        #endregion

        #region Add New Stock
        public bool Add()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("StockAdd") as SqlCommand;
            db.AddInParameter(oCommand, "SecName", SqlDbType.VarChar, strSecName.ToUpper());
            db.AddInParameter(oCommand, "SecCode", SqlDbType.VarChar, strSecCode.ToUpper());
            db.AddInParameter(oCommand, "Industry", SqlDbType.VarChar, strIndustry);
            db.AddInParameter(oCommand, "Registrar", SqlDbType.VarChar, strRegistrar);
            db.AddInParameter(oCommand, "InstruType", SqlDbType.VarChar, strInstruType);
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.AddInParameter(oCommand, "CscsCode", SqlDbType.VarChar, strCscsCode);
            db.AddInParameter(oCommand, "NominalValue", SqlDbType.Money, decNominalValue);
            db.AddInParameter(oCommand, "IntialOutShare", SqlDbType.BigInt, lngIntialOutShare);
            db.AddInParameter(oCommand, "DelistedByNSE", SqlDbType.Bit, blnDelistedByNSE);
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "STOCK");
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;

            return blnStatus;
        }
        #endregion

        #region Edit Stock
        public bool Edit()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("StockEdit") as SqlCommand;
            db.AddInParameter(oCommand, "SecName", SqlDbType.VarChar, strSecName.ToUpper());
            db.AddInParameter(oCommand, "SecCode", SqlDbType.VarChar, strSecCode.ToUpper());
            db.AddInParameter(oCommand, "KeepStockCode", SqlDbType.VarChar, strKeepStockCode.Trim());
            db.AddInParameter(oCommand, "Industry", SqlDbType.VarChar, strIndustry);
            db.AddInParameter(oCommand, "Registrar", SqlDbType.VarChar, strRegistrar);
            db.AddInParameter(oCommand, "InstruType", SqlDbType.VarChar, strInstruType);
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.AddInParameter(oCommand, "CscsCode", SqlDbType.VarChar, strCscsCode);
            db.AddInParameter(oCommand, "NominalValue", SqlDbType.Money, decNominalValue);
            db.AddInParameter(oCommand, "IntialOutShare", SqlDbType.BigInt, lngIntialOutShare);
            db.AddInParameter(oCommand, "DelistedByNSE", SqlDbType.Bit, blnDelistedByNSE);
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Get Stock Info For Stock Form
        public bool GetStock()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("StockSelectBySecCode") as SqlCommand;
            db.AddInParameter(dbCommand, "SecCode", SqlDbType.VarChar, strKeepStockCode.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strSecName = thisRow[0]["SecName"].ToString();
                strSecCode = thisRow[0]["SecCode"].ToString();
                strIndustry = thisRow[0]["Industry"] != null ? thisRow[0]["Industry"].ToString() : "";
                strRegistrar = thisRow[0]["Registrar"] != null ? thisRow[0]["Registrar"].ToString() : "";
                strInstruType = thisRow[0]["InstruType"].ToString();
                if (thisRow[0]["CscsCode"] == null)
                {
                    strCscsCode = "";
                }
                else
                {
                    strCscsCode = thisRow[0]["CscsCode"].ToString();
                }
               
                decNominalValue = thisRow[0]["NominalValue"] != null && thisRow[0]["NominalValue"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["NominalValue"].ToString()) : 0;
                lngIntialOutShare = thisRow[0]["IntialOutShare"] != null && thisRow[0]["IntialOutShare"].ToString().Trim() != "" ? long.Parse(thisRow[0]["IntialOutShare"].ToString()) : 0;
                blnDelistedByNSE = thisRow[0]["DelistedByNSE"] != null && thisRow[0]["DelistedByNSE"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["DelistedByNSE"].ToString()) : false;
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }

            return blnStatus;
        }
        #endregion


        #region Get Stock Info For Other Form
        public bool GetStockOther()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("StockSelectBySecCode") as SqlCommand;
            db.AddInParameter(dbCommand, "SecCode", SqlDbType.VarChar, strSecCode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strSecName = thisRow[0]["SecName"].ToString();
                strSecCode = thisRow[0]["SecCode"].ToString();
                strIndustry = thisRow[0]["Industry"].ToString();
                strRegistrar = thisRow[0]["Registrar"].ToString();
                strInstruType = thisRow[0]["InstruType"].ToString();
                if (thisRow[0]["CscsCode"] == null)
                {
                    strCscsCode = "";
                }
                else
                {
                    strCscsCode = thisRow[0]["CscsCode"].ToString();
                }
               
                decNominalValue = thisRow[0]["NominalValue"] != null && thisRow[0]["NominalValue"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["NominalValue"].ToString()) : 0;
                lngIntialOutShare = thisRow[0]["IntialOutShare"] != null && thisRow[0]["IntialOutShare"].ToString().Trim() != "" ? long.Parse(thisRow[0]["IntialOutShare"].ToString()) : 0;
                blnDelistedByNSE = thisRow[0]["DelistedByNSE"] != null && thisRow[0]["DelistedByNSE"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["DelistedByNSE"].ToString()) : false;
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Get Customer No Given The CsCs Account Number
        public string GetStockCodeGivenStockCode()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("StockSelectByStockCodeReturnStockCode", strSecCode) as SqlCommand;
            return (string)db.ExecuteScalar(dbCommand);
        }
        #endregion

        #region Get All Stock Order By Trans No
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("StockSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All NSE Stock Order By Trans No
        public DataSet GetAllNSE()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("StockSelectAllNSE") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

       

        #region Get All NASD Stock Order By Trans No
        public DataSet GetAllNASD()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("StockSelectAllNASD") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All NSE Stock Not Delisted Order By Trans No
        public DataSet GetAllNSENoBondNotDelisted()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("StockSelectAllNSENoBondNotDelisted") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("StockDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strKeepStockCode.Trim());
            db.AddInParameter(oCommand, "Secname", SqlDbType.VarChar, strSecName.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Check That Stock Code Already Exist For Existing Record
        public bool StockCodeExist()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("StockSelectBySecCodeExist") as SqlCommand;
            db.AddInParameter(dbCommand, "SecCode", SqlDbType.VarChar, strSecCode.Trim());
            db.AddOutParameter(dbCommand, "CodeExist", SqlDbType.VarChar, 1);
            db.AddInParameter(dbCommand, "Transno", SqlDbType.VarChar, strKeepStockCode.Trim());
            db.ExecuteNonQuery(dbCommand);
            if (db.GetParameterValue(dbCommand, "CodeExist").ToString().Trim() == "0")
            {
                blnStatus = false;
            }
            else
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Check That Stock Code Already Exist For New Record
        public bool StockCodeExistNoTransNo()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("StockSelectBySecCodeExistNoTransno") as SqlCommand;
            db.AddInParameter(dbCommand, "SecCode", SqlDbType.VarChar, strSecCode.Trim());
            db.AddOutParameter(dbCommand, "CodeExist", SqlDbType.VarChar, 1);
            db.ExecuteNonQuery(dbCommand);
            if (db.GetParameterValue(dbCommand, "CodeExist").ToString().Trim() == "0")
            {
                blnStatus = false;
            }
            else
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Check That Stock Name Already Exist For Existing Record
        public bool StockNameExist()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("StockSelectBySecNameExist") as SqlCommand;
            db.AddInParameter(dbCommand, "Secname", SqlDbType.VarChar, strSecName.Trim());
            db.AddOutParameter(dbCommand, "NameExist", SqlDbType.VarChar, 1);
            db.AddInParameter(dbCommand, "Transno", SqlDbType.VarChar, strKeepStockCode.Trim());
            db.ExecuteNonQuery(dbCommand);
            if (db.GetParameterValue(dbCommand, "NameExist").ToString().Trim() == "0")
            {
                blnStatus = false;
            }
            else
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Check That Stock Name Already Exist For New Record
        public bool StockNameExistNoTransNo()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("StockSelectBySecNameExistNoTransno") as SqlCommand;
            db.AddInParameter(dbCommand, "Secname", SqlDbType.VarChar, strSecName.Trim());
            db.AddOutParameter(dbCommand, "NameExist", SqlDbType.VarChar, 1);
            db.ExecuteNonQuery(dbCommand);
            if (db.GetParameterValue(dbCommand, "NameExist").ToString().Trim() == "0")
            {
                blnStatus = false;
            }
            else
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Get Stock DataSet Given StockCode
        public DataSet GetStockSelectByStockCodeShowingDetails()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("StockSelectByStockCodeShowDetails") as SqlCommand;
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strSecCode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion


        #region Get Stock Instrument Type Using Stockcode
        public DataGeneral.StockInstrumentType GetInstrumentTypeUsingStockCode()
        {
            DataGeneral.StockInstrumentType oStockInstrumentType = DataGeneral.StockInstrumentType.OTHERS;
            GetStockOther();
            if (strInstruType.Trim() == "0001")
            {
                oStockInstrumentType = DataGeneral.StockInstrumentType.QUOTEDEQUITY;
            }
            else if (strInstruType.Trim() == "0013")
            {
                oStockInstrumentType = DataGeneral.StockInstrumentType.NASD;
            }
            else if (strInstruType.Trim() == "0003")
            {
                oStockInstrumentType = DataGeneral.StockInstrumentType.BOND;
            }
            return oStockInstrumentType;
        }
        #endregion

        #region Change Stock Code In Transaction
        public void ChangeStockCodeInTransaction(string strStockCodeOld,string strStockCodeNew)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("StockChangeStockCodeInTransaction") as SqlCommand;
            db.AddInParameter(dbCommand, "StockCodeOld", SqlDbType.VarChar, strStockCodeOld.Trim());
            db.AddInParameter(dbCommand, "StockCodeNew", SqlDbType.VarChar, strStockCodeNew.Trim());
            db.ExecuteNonQuery(dbCommand);
        }
        #endregion
        


    }

}

