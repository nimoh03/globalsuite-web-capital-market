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

namespace CapitalMarket.Business
{
    public class FullDematerialization
    {
        #region Declarations
        public Int64 TransNo { set; get; }
        public Int64 FullDematerializationMasterId { set; get; }
        public DateTime Effdate { set; get; }
        public string StockCode { set; get; }
        public Int64 Units { set; get; }
        public string AccountNumber { set; get; }
        public string CertificateNumber { set; get; }
        public bool IsFromRegistrar { set; get; }
        public DateTime DateVerified { set; get; }
        public DateTime DateSent { set; get; }
        public DateTime DateRec { set; get; }
        public string SaveType { set; get; }
        #endregion


        public FullDematerialization()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region Save Return Command
        public SqlCommand SaveCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (SaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("FullDematerializationAddNew") as SqlCommand;
            }
            else if (SaveType == "EDIT")
            {
                if (TransNo == 0)
                {
                    oCommand = db.GetStoredProcCommand("FullDematerializationAddNew") as SqlCommand;
                }
                else
                {
                    oCommand = db.GetStoredProcCommand("FullDematerializationEdit") as SqlCommand;
                }
            }
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, TransNo);
            db.AddInParameter(oCommand, "FullDematerializationMasterId", SqlDbType.BigInt, FullDematerializationMasterId);
            db.AddInParameter(oCommand, "Stockcode", SqlDbType.VarChar, StockCode.Trim());
            db.AddInParameter(oCommand, "Units", SqlDbType.BigInt, Units);
            db.AddInParameter(oCommand, "CertificateNumber", SqlDbType.VarChar, CertificateNumber.Trim());
            db.AddInParameter(oCommand, "AccountNumber", SqlDbType.VarChar, AccountNumber.Trim());
            db.AddInParameter(oCommand, "IsFromRegistrar", SqlDbType.Bit, IsFromRegistrar);
            if (DateVerified != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "Effdate", SqlDbType.DateTime, Effdate);
            }
            else
            {
                db.AddInParameter(oCommand, "Effdate", SqlDbType.DateTime, SqlDateTime.Null);
            }
            if (DateSent != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "DateSent", SqlDbType.DateTime, DateSent);
            }
            else
            {
                db.AddInParameter(oCommand, "DateSent", SqlDbType.DateTime, SqlDateTime.Null);
            }
            if (DateRec != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "DateRec", SqlDbType.DateTime, DateRec);
            }
            else
            {
                db.AddInParameter(oCommand, "DateRec", SqlDbType.DateTime, SqlDateTime.Null);
            }
            if (DateVerified != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "DateVerified", SqlDbType.DateTime, DateVerified);
            }
            else
            {
                db.AddInParameter(oCommand, "DateVerified", SqlDbType.DateTime, SqlDateTime.Null);
            }
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
            return oCommand;
        }
        #endregion

        #region Check TransNo Exist
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;
            if (SaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("FullDematerializationChkTransNoExist") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, TransNo);
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
            else if (SaveType == "ADDS")
            {
                blnStatus = true;
            }

            return blnStatus;
        }
        #endregion

        #region Get All FullDematerialization
        public DataSet GetAll()
        {


            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("FullDematerializationSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;


        }
        #endregion

        #region Get All Full Dematerialization
        public DataSet GetAllFullDemat()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("FullDematerializationSelectAllFullDematerialization") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get FullDematerialization
        public bool GetFullDematerialization()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("FullDematerializationSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "Code", SqlDbType.VarChar, TransNo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                if (thisRow[0]["EffDate"] == null || thisRow[0]["EffDate"].ToString().Trim() == "")
                {
                    Effdate = DateTime.MinValue;
                }
                else
                {
                    Effdate = DateTime.Parse(thisRow[0]["EffDate"].ToString());
                }
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

