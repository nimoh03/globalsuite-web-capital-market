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
    public class InstrumentType
    {
        #region Declaration
        private string strInstrucode;
        private string strInstruname;
        private Int64 intAssetClass;
        public string SpecializedType { set; get; }
        #endregion

        #region Properties
        public string Instrucode
        {
            set { strInstrucode = value; }
            get { return strInstrucode; }
        }
        public string Instruname
        {
            set { strInstruname = value; }
            get { return strInstruname; }
        }
        public Int64 AssetClass
        {
            set { intAssetClass = value; }
            get { return intAssetClass; }
        }
        
        #endregion

        public InstrumentType()
        {
        }

        #region Enum
        public enum SaveStatus { Nothing, NotExist, Saved }
        #endregion

        #region Add New Instrument Type
        public bool Add()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("InstrumentTypeAddNew") as SqlCommand;
            db.AddInParameter(oCommand, "Instrucode", SqlDbType.VarChar, strInstrucode.Trim());
            db.AddInParameter(oCommand, "Instruname", SqlDbType.VarChar, strInstruname.Trim().ToUpper());
            db.AddInParameter(oCommand, "AssetClass", SqlDbType.BigInt, intAssetClass);
            db.AddInParameter(oCommand, "SpecializedType", SqlDbType.VarChar, SpecializedType.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "INSTRUMENT");
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
            
        }
        #endregion

        #region Edit New Instrument Type
        public bool Edit()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("InstrumentTypeEdit") as SqlCommand;
            db.AddInParameter(oCommand, "Instrucode", SqlDbType.VarChar, strInstrucode.Trim());
            db.AddInParameter(oCommand, "Instruname", SqlDbType.VarChar, strInstruname.Trim().ToUpper());
            db.AddInParameter(oCommand, "AssetClass", SqlDbType.BigInt, intAssetClass);
            db.AddInParameter(oCommand, "SpecializedType", SqlDbType.VarChar, SpecializedType.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;

        }
        #endregion

        #region Get All Instrument Type Order By  ID
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("InstrumentTypeSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Instrument Type 
        public bool GetInstrumentType()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("InstrumentTypeSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "Instrucode", SqlDbType.VarChar, strInstrucode);

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strInstrucode = thisRow[0]["Instrucode"].ToString();
                strInstruname = thisRow[0]["Instruname"].ToString();
                intAssetClass = Convert.ToInt64(thisRow[0]["AssetClass"]);
                SpecializedType = thisRow[0]["SpecializedType"] != null ? thisRow[0]["SpecializedType"].ToString() : "";
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("InstrumentTypeDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Instrucode", SqlDbType.VarChar, strInstrucode.Trim());
            db.AddInParameter(oCommand, "Instruname", SqlDbType.VarChar, strInstruname.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Check That Instrument Type Name Already Exist For Existing Record
        public bool InstrumentTypeNameExist()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("InstrumentTypeSelectByInstruNameExist") as SqlCommand;
            db.AddInParameter(dbCommand, "Instruname", SqlDbType.VarChar, strInstruname.Trim());
            db.AddOutParameter(dbCommand, "NameExist", SqlDbType.VarChar, 1);
            db.AddInParameter(dbCommand, "Transno", SqlDbType.VarChar, strInstrucode.Trim());
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

        #region Check That Instrument Type Name Already Exist For New Record
        public bool InstrumentTypeNameExistNoTransNo()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("InstrumentTypeSelectByInstruNameExistNoTransno") as SqlCommand;
            db.AddInParameter(dbCommand, "Instruname", SqlDbType.VarChar, strInstruname.Trim());
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

       

    }
}
