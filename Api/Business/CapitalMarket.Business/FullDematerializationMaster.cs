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
    public class FullDematerializationMaster
    {
        #region Declarations
        public Int64 TransNo { set; get; }
        public DateTime Effdate { set; get; }
        public string CertVerificationNo { set; get; }
        public bool IsWithCertificate { set; get; }
        public string CustomerId { set; get; }
        public string SaveType { set; get; }
        #endregion


        public FullDematerializationMaster()
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
                oCommand = db.GetStoredProcCommand("FullDematerializationMasterAddNew") as SqlCommand;
                db.AddOutParameter(oCommand, "TransNo", SqlDbType.BigInt, 10);
            }
            else if (SaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("FullDematerializationMasterEdit") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt,TransNo);
            }
            db.AddInParameter(oCommand, "CustomerId", SqlDbType.VarChar, CustomerId.Trim());
            db.AddInParameter(oCommand, "CertVerificationNo", SqlDbType.VarChar, CertVerificationNo.Trim());
            db.AddInParameter(oCommand, "IsWithCertificate", SqlDbType.Bit, IsWithCertificate);
            db.AddInParameter(oCommand, "Effdate", SqlDbType.DateTime, Effdate);
            
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
                SqlCommand oCommand = db.GetStoredProcCommand("FullDematerializationMasterChkTransNoExist") as SqlCommand;
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

        #region Get All FullDematerializationMaster
        public DataSet GetAll()
        {
           

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            
                dbCommand = db.GetStoredProcCommand("FullDematerializationMasterSelectAll") as SqlCommand;
            

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All FullDematerialization By Master
        public DataSet GetAllDematByMaster()
        {


            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("FullDematerializationSelectAllByMaster") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, TransNo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;


        }
        #endregion

        #region Get All Full Demat
        public DataSet GetAllFullDemat()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("FullDematerializationMasterSelectAllFullDematerialization") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get FullDematerializationMaster
        public bool GetFullDematerializationMaster()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("FullDematerializationMasterSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "Code", SqlDbType.BigInt, TransNo);
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
                CustomerId = thisRow[0]["CustomerId"].ToString();
                CertVerificationNo = thisRow[0]["CertVerificationNo"].ToString();
                IsWithCertificate = thisRow[0]["IsWithCertificate"] != null && thisRow[0]["IsWithCertificate"].ToString().Trim() != "" ? Convert.ToBoolean(thisRow[0]["IsWithCertificate"]): false;
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

