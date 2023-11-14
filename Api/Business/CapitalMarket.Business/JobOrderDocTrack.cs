using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using BaseUtility.Business;

namespace CapitalMarket.Business
{
    public class JobOrderDocTrack
    {
        #region Properties
        public Int64 TransNo { get; set; }
        public string CustomerNo { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string DocumentID { get; set; }
        public byte[] DocumentPhoto { get; set; }
        public string JobOrderId { get; set; }
        public string Remark { get; set; }
        public string ImageFileName { get; set; }
        public string SaveType { get; set; }
        IFormatProvider format = new CultureInfo("en-GB");
        #endregion

        #region Save
        public DataGeneral.SaveStatus Save()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            if (ChkNameExist())
            {
                enSaveStatus = DataGeneral.SaveStatus.NameExistAdd;
                return enSaveStatus;
            }

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (SaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("JobOrderDocTrackAdd") as SqlCommand;
            }
            else if (SaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("JobOrderDocTrackEdit") as SqlCommand;
            }
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, TransNo);
            db.AddInParameter(oCommand, "EffectiveDate", SqlDbType.DateTime, EffectiveDate);
            db.AddInParameter(oCommand, "CustomerNo", SqlDbType.VarChar, CustomerNo.Trim());
            db.AddInParameter(oCommand, "DocumentPhoto", SqlDbType.Image, DocumentPhoto);
            db.AddInParameter(oCommand, "DocumentID", SqlDbType.VarChar, DocumentID.Trim());
            db.AddInParameter(oCommand, "JobOrderId", SqlDbType.VarChar, JobOrderId.Trim());
            db.AddInParameter(oCommand, "Remark", SqlDbType.VarChar, Remark);
            db.AddInParameter(oCommand, "ImageFileName", SqlDbType.VarChar, ImageFileName.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Check Name Exist
        public bool ChkNameExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("JobOrderDocTrackChkNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, TransNo);
            db.AddInParameter(oCommand, "CustomerNo", SqlDbType.VarChar, CustomerNo.Trim());
            db.AddInParameter(oCommand, "DocumentID", SqlDbType.VarChar,DocumentID.Trim());
            db.AddInParameter(oCommand, "SaveType", SqlDbType.VarChar, SaveType);
            DataSet oDs = db.ExecuteDataSet(oCommand);
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

        #region Get Job Order Document Tracking
        public bool GetJobOrderDocTrack()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("JobOrderDocTrackSelect") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, TransNo);
            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                EffectiveDate = thisRow[0]["EffectiveDate"] != null && thisRow[0]["EffectiveDate"].ToString().Trim() != "" ? DateTime.ParseExact(thisRow[0]["EffectiveDate"].ToString().Substring(0, 10), "dd/MM/yyyy", format) : DateTime.MinValue;
                CustomerNo = thisRow[0]["CustomerId"].ToString();
                DocumentID = thisRow[0]["DocumentID"] != null ? thisRow[0]["DocumentID"].ToString() : "";
                DocumentPhoto = thisRow[0]["DocumentPhoto"] != null ? (byte[])thisRow[0]["DocumentPhoto"] : null;
                JobOrderId = thisRow[0]["JobOrderId"].ToString();
                ImageFileName = thisRow[0]["ImageFileName"].ToString();
                Remark = thisRow[0]["Remark"] != null ? (string)thisRow[0]["Remark"] : "";
                blnStatus = true;
            }

            return blnStatus;

        }
        #endregion

        #region Get All Job Order Document Tracking
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("JobOrderDocTrackSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion

        #region Delete Document Same Database
        public bool DeleteDocumentSameDatabase()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobOrderDocDelete") as SqlCommand;
            db.AddInParameter(dbCommand, "CustomerId", SqlDbType.VarChar, CustomerNo.Trim());
            db.ExecuteNonQuery(dbCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Get Customer Document Image By FileName and Customer
        public byte[] GetCustomerDocumentImageByFileNameandCustomer()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobOrderDocTrackSelectImageByFileNameCustomer") as SqlCommand;
            db.AddInParameter(dbCommand, "CustomerNo", SqlDbType.VarChar, CustomerNo.Trim());
            db.AddInParameter(dbCommand, "DocumentID", SqlDbType.VarChar, DocumentID.Trim());
            if (db.ExecuteScalar(dbCommand) != null && db.ExecuteScalar(dbCommand).ToString().Trim() != "")
            {
                return (byte[])(db.ExecuteScalar(dbCommand));
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Get By Customer And Date
        public DataSet GetByCustomerAndDate(DateTime datDateTo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobOrderDocTrackSelectByCustomerAndDate") as SqlCommand;
            db.AddInParameter(dbCommand, "CustomerNo", SqlDbType.VarChar, CustomerNo.Trim());
            db.AddInParameter(dbCommand, "FromDate", SqlDbType.DateTime, EffectiveDate);
            db.AddInParameter(dbCommand, "ToDate", SqlDbType.DateTime, datDateTo);
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region Get By Job Order Id
        public bool GetByJobOrderId()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobOrderDocTrackSelectByJobOrderId") as SqlCommand;
            db.AddInParameter(dbCommand, "JobOrderId", SqlDbType.VarChar, JobOrderId.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                EffectiveDate = thisRow[0]["EffectiveDate"] != null && thisRow[0]["EffectiveDate"].ToString().Trim() != "" ? DateTime.ParseExact(thisRow[0]["EffectiveDate"].ToString().Substring(0, 10), "dd/MM/yyyy", format) : DateTime.MinValue;
                CustomerNo = thisRow[0]["CustomerId"].ToString();
                DocumentID = thisRow[0]["DocumentID"] != null ? thisRow[0]["DocumentID"].ToString() : "";
                DocumentPhoto = thisRow[0]["DocumentPhoto"] != null ? (byte[])thisRow[0]["DocumentPhoto"] : null;
                JobOrderId = thisRow[0]["JobOrderId"].ToString();
                ImageFileName = thisRow[0]["ImageFileName"].ToString();
                Remark = thisRow[0]["Remark"] != null ? (string)thisRow[0]["Remark"] : "";
                blnStatus = true;
            }

            return blnStatus;
        }
        #endregion

        #region Save Document Same Database
        public void SaveDocumentSameDatabase()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("JobOrderDocAdd") as SqlCommand;
            db.AddInParameter(oCommand, "CustomerNo", SqlDbType.VarChar, CustomerNo.Trim());
            db.AddInParameter(oCommand, "DocumentPhoto", SqlDbType.Image, DocumentPhoto);
            db.AddInParameter(oCommand, "DocumentID", SqlDbType.VarChar, DocumentID.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
        }
        #endregion
    }
}
