using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class DirectorDocumentTracking
    {
        #region Declarations
        private string strTransNo;
        private string strDirectorId;
        private Int64 intKYCDocTypeId;
        private byte[] imgDocumentPhoto;
        private string strDocumentFileName;
        private string strIdentificationType, strIdentificationNo;
        private DateTime datExpiryDate;
        private string strSaveType;
        IFormatProvider format = new CultureInfo("en-GB");
        #endregion

        #region Properties

        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public string DirectorId
        {
            set { strDirectorId = value; }
            get { return strDirectorId; }
        }
        public byte[] DocumentPhoto
        {
            set { imgDocumentPhoto = value; }
            get { return imgDocumentPhoto; }
        }
        public string DocumentFileName
        {
            set { strDocumentFileName = value; }
            get { return strDocumentFileName; }
        }
        public Int64 KYCDocTypeId
        {
            set { intKYCDocTypeId = value; }
            get { return intKYCDocTypeId; }
        }
        public string IdentificationType
        {
            set { strIdentificationType = value; }
            get { return strIdentificationType; }
        }
        public string IdentificationNo
        {
            set { strIdentificationNo = value; }
            get { return strIdentificationNo; }
        }
        
        public DateTime ExpiryDate
        {
            set { datExpiryDate = value; }
            get { return datExpiryDate; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
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

            if (ChkDocumentTypeNameExist())
            {
                enSaveStatus = DataGeneral.SaveStatus.HeadOfficeExistAdd;
                return enSaveStatus;
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (strSaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("DirectorDocumentTrackingAdd") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, "");
            }
            else if (strSaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("DirectorDocumentTrackingEdit") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo);
            }
            db.AddInParameter(oCommand, "DirectorId", SqlDbType.VarChar, strDirectorId.Trim());
            db.AddInParameter(oCommand, "DocumentPhoto", SqlDbType.Image, imgDocumentPhoto);
            db.AddInParameter(oCommand, "DocumentFileName", SqlDbType.VarChar, strDocumentFileName.Trim());
            db.AddInParameter(oCommand, "KYCDocTypeId", SqlDbType.BigInt, intKYCDocTypeId);
            db.AddInParameter(oCommand, "IdentificationType", SqlDbType.VarChar, strIdentificationType.Trim());
            db.AddInParameter(oCommand, "IdentificationNo", SqlDbType.VarChar, strIdentificationNo.Trim());
            if (datExpiryDate != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "ExpiryDate", SqlDbType.DateTime, datExpiryDate);
            }
            else
            {
                db.AddInParameter(oCommand, "ExpiryDate", SqlDbType.DateTime, null);
            }
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar,GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;

            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorDocumentTrackingDelete") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.ExecuteNonQuery(dbCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Check Name Exist
        public bool ChkNameExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("DirectorDocumentTrackingChkNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "DirectorId", SqlDbType.VarChar, strDirectorId.Trim());
            db.AddInParameter(oCommand, "Name", SqlDbType.VarChar,strDocumentFileName.Trim());
            db.AddInParameter(oCommand, "SaveType", SqlDbType.VarChar, strSaveType);
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

        #region Check Document Type Name Exist
        public bool ChkDocumentTypeNameExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("DirectorDocumentTrackingChkDocumentTypeNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "DirectorId", SqlDbType.VarChar, strDirectorId.Trim());
            db.AddInParameter(oCommand, "KYCDocTypeId", SqlDbType.BigInt, intKYCDocTypeId);
            db.AddInParameter(oCommand, "SaveType", SqlDbType.VarChar, strSaveType);
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

        #region Get Director Document By Director
        public DataSet GetDirectorDocumentByDirector()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorDocumentTrackingSelectByDirector") as SqlCommand;
            db.AddInParameter(dbCommand, "DirectorId", SqlDbType.VarChar, strDirectorId.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Director Document By Director List
        public DataSet GetDirectorDocumentByDirectorList(int DirectorTypeId)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorDocumentTrackingSelectByDirectorList") as SqlCommand;
            db.AddInParameter(dbCommand, "DirectorId", SqlDbType.VarChar, strDirectorId.Trim());
            db.AddInParameter(dbCommand, "DirectorTypeId", SqlDbType.Int,DirectorTypeId);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;

        }
        #endregion

        #region Get Director Document Image By FileName and Director
        public byte[] GetDirectorDocumentImageByFileNameandDirector()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorDocumentTrackingSelectImageByFileNameDirector") as SqlCommand;
            db.AddInParameter(dbCommand, "DirectorId", SqlDbType.VarChar, strDirectorId.Trim());
            db.AddInParameter(dbCommand, "DocumentFileName", SqlDbType.VarChar, strDocumentFileName.Trim());
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

        #region Get Director Document Type By FileName and Director
        public string GetDirectorDocumentTypeByFileNameandDirector()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorDocumentTrackingSelectTypeByFileNameDirector") as SqlCommand;
            db.AddInParameter(dbCommand, "DirectorId", SqlDbType.VarChar, strDirectorId.Trim());
            db.AddInParameter(dbCommand, "DocumentFileName", SqlDbType.VarChar, strDocumentFileName.Trim());
            var strDocumentTypeReturn = db.ExecuteScalar(dbCommand);
            return strDocumentTypeReturn != null ? strDocumentTypeReturn.ToString() : "";
        }
        #endregion

        #region Get Director Document
        public bool GetDirectorDocument()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorDocumentTrackingSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strDirectorId = thisRow[0]["DirectorId"].ToString();
                intKYCDocTypeId = Convert.ToInt64(thisRow[0]["DocumentType"]);
                strDocumentFileName = thisRow[0]["DocumentFileName"] != null ? thisRow[0]["DocumentFileName"].ToString() : "";
                imgDocumentPhoto = thisRow[0]["DocumentPhoto"] != null ? (byte[])thisRow[0]["DocumentPhoto"] : null;
                strIdentificationType = thisRow[0]["IdentificationType"] != null ? thisRow[0]["IdentificationType"].ToString() : "";
                strIdentificationNo = thisRow[0]["IdentificationNo"] != null ? thisRow[0]["IdentificationNo"].ToString() : "";
                datExpiryDate = thisRow[0]["ExpiryDate"] != null && thisRow[0]["ExpiryDate"].ToString().Trim() != "" ? DateTime.ParseExact(thisRow[0]["ExpiryDate"].ToString().Substring(0, 10), "dd/MM/yyyy", format) : DateTime.MinValue;
                blnStatus = true;
            }
            
            return blnStatus;
           
        }
        #endregion

        

        #region Delete Document Same Database
        public bool DeleteDocumentSameDatabase()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorDocumentDelete") as SqlCommand;
            db.AddInParameter(dbCommand, "DirectorId", SqlDbType.VarChar, strDirectorId.Trim());
            db.ExecuteNonQuery(dbCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Save Document Same Database
        public void SaveDocumentSameDatabase()
        {
           
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("DirectorDocumentAdd") as SqlCommand;
            db.AddInParameter(oCommand, "DirectorId", SqlDbType.VarChar, strDirectorId.Trim());
            db.AddInParameter(oCommand, "DocumentPhoto", SqlDbType.Image, imgDocumentPhoto);
            db.AddInParameter(oCommand, "DocumentFileName", SqlDbType.VarChar, strDocumentFileName.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        #region Get All Expired Means Of Id
        public DataSet GetAllExpiredMeansOfId(DateTime datCurrentDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorDocumentTrackingSelectExpiredMeansOfId") as SqlCommand;
            db.AddInParameter(dbCommand, "DateToCheck", SqlDbType.DateTime, datCurrentDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Update Director Name
        public void UpdateDirectorName(string strDirectorNo,bool blnAddPhotoAndSignature)
        {
            AppSettingsReader oAppSettingsReader = new AppSettingsReader();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            DatabaseProviderFactory factoryGlobalSuite = new DatabaseProviderFactory(); SqlDatabase dbGlobalSuite = factoryGlobalSuite.Create("GlobalSuitedb") as SqlDatabase;

            GLParam oGLParam = new GLParam();
            oGLParam.Type = "IMAGEDATAINDIFFLOCATION";
            if (oGLParam.CheckParameter().Trim() != "YES")
            {
                SqlCommand dbCommand;
                if (blnAddPhotoAndSignature)
                {
                    dbCommand = db.GetStoredProcCommand("DirectorDocumentTrackingUpdateDirectorName") as SqlCommand;
                }
                else
                {
                    dbCommand = db.GetStoredProcCommand("DirectorDocumentTrackingUpdateDirectorNameWithOutPhotoSignature") as SqlCommand;
                }
                db.AddInParameter(dbCommand, "DirectorId", SqlDbType.VarChar, strDirectorNo.Trim());
                db.AddInParameter(dbCommand, "DatabaseName", SqlDbType.VarChar, oAppSettingsReader.GetValue("DatabaseName", typeof(string)).ToString());
                db.ExecuteNonQuery(dbCommand);
            }
            else
            {
                SqlCommand dbCommandDeleteDirectorName = db.GetStoredProcCommand("DirectorDocumentTrackingDeleteDirectorName") as SqlCommand;
                db.ExecuteNonQuery(dbCommandDeleteDirectorName);
                SqlCommand dbCommandDirector;
                if (blnAddPhotoAndSignature)
                {
                    dbCommandDirector = dbGlobalSuite.GetStoredProcCommand("DirectorSelectWithDirectorType") as SqlCommand;
                }
                else
                {
                    dbCommandDirector = dbGlobalSuite.GetStoredProcCommand("DirectorSelectWithDirectorTypeWithOutPhotoSignature") as SqlCommand;
                }
                dbGlobalSuite.AddInParameter(dbCommandDirector, "DirectorId", SqlDbType.VarChar, strDirectorNo.Trim());
                DataTable oDTblDirectorByClientType = dbGlobalSuite.ExecuteDataSet(dbCommandDirector).Tables[0];

                using (SqlConnection connection = db.CreateConnection() as SqlConnection)
                {
                    connection.Open();
                    using (var bulkCopy = new SqlBulkCopy(connection))
                    {
                        foreach (DataColumn col in oDTblDirectorByClientType.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }

                        bulkCopy.BulkCopyTimeout = 600;
                        bulkCopy.DestinationTableName = "DirectorName";
                        bulkCopy.WriteToServer(oDTblDirectorByClientType);
                    }
                }

            }
        }
        #endregion

        #region Update Director Name All
        public void UpdateDirectorNameAll(bool blnAddPhotoAndSignature)
        {
            AppSettingsReader oAppSettingsReader = new AppSettingsReader();

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase; 
            DatabaseProviderFactory factoryGlobalSuite = new DatabaseProviderFactory(); SqlDatabase dbGlobalSuite = factoryGlobalSuite.Create("GlobalSuitedb") as SqlDatabase;

            GLParam oGLParam = new GLParam();
            oGLParam.Type = "IMAGEDATAINDIFFLOCATION";
            if (oGLParam.CheckParameter().Trim() != "YES")
            {
                SqlCommand dbCommand;
                if (blnAddPhotoAndSignature)
                {
                    dbCommand = db.GetStoredProcCommand("DirectorDocumentTrackingUpdateDirectorNameAll") as SqlCommand;
                }
                else
                {
                    dbCommand = db.GetStoredProcCommand("DirectorDocumentTrackingUpdateDirectorNameAllWithoutPhotoSignature") as SqlCommand;
                }
                db.AddInParameter(dbCommand, "DatabaseName", SqlDbType.VarChar, oAppSettingsReader.GetValue("DatabaseName", typeof(string)).ToString());
                db.ExecuteNonQuery(dbCommand);
            }
            else
            {
                SqlCommand dbCommandDeleteDirectorName = db.GetStoredProcCommand("DirectorDocumentTrackingDeleteDirectorName") as SqlCommand;
                db.ExecuteNonQuery(dbCommandDeleteDirectorName);

                SqlCommand dbCommandDirector;
                if (blnAddPhotoAndSignature)
                {
                    dbCommandDirector = dbGlobalSuite.GetStoredProcCommand("DirectorSelectAllWithDirectorType") as SqlCommand;
                }
                else
                {
                    dbCommandDirector = dbGlobalSuite.GetStoredProcCommand("DirectorSelectAllWithDirectorTypeWithOutPhotoSignature") as SqlCommand;
                }
                DataTable oDTblDirectorByClientType = dbGlobalSuite.ExecuteDataSet(dbCommandDirector).Tables[0];


                using (SqlConnection connection = db.CreateConnection() as SqlConnection)
                {
                    connection.Open();
                    using (var bulkCopy = new SqlBulkCopy(connection))
                    {
                        foreach (DataColumn col in oDTblDirectorByClientType.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }
                        bulkCopy.BulkCopyTimeout = 600;
                        bulkCopy.DestinationTableName = "DirectorName";
                        bulkCopy.WriteToServer(oDTblDirectorByClientType);
                    }
                }

            }
        }
        #endregion

        #region Check KYCDocTypeId And Director Type Id Exist
        public bool ChkKYCDocTypeIdAndDirectorTypeIdExist(Int32 DirectorTypeId)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("DirectorDocumentTrackingChkKYCDocTypeIdAndDirectorTypeIdExist") as SqlCommand;
            db.AddInParameter(oCommand, "KYCDocTypeId", SqlDbType.BigInt, intKYCDocTypeId);
            db.AddInParameter(oCommand, "DirectorTypeId", SqlDbType.Int, DirectorTypeId);
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

        #region Get Director Document  ByKYCDocTypeId And DirectorId
        public bool GetDirectorDocumentByKYCDocTypeIdAndDirectorId()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("DirectorDocumentTrackingSelectByKYCDocTypeIdAndDirectorId") as SqlCommand;
            db.AddInParameter(oCommand, "KYCDocTypeId", SqlDbType.BigInt, intKYCDocTypeId);
            db.AddInParameter(oCommand, "DirectorId", SqlDbType.VarChar, DirectorId);
            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strDirectorId = thisRow[0]["DirectorId"].ToString();
                intKYCDocTypeId = Convert.ToInt64(thisRow[0]["DocumentType"]);
                strDocumentFileName = thisRow[0]["DocumentFileName"] != null ? thisRow[0]["DocumentFileName"].ToString() : "";
                imgDocumentPhoto = thisRow[0]["DocumentPhoto"] != null ? (byte[])thisRow[0]["DocumentPhoto"] : null;
                strIdentificationType = thisRow[0]["IdentificationType"] != null ? thisRow[0]["IdentificationType"].ToString() : "";
                strIdentificationNo = thisRow[0]["IdentificationNo"] != null ? thisRow[0]["IdentificationNo"].ToString() : "";
                datExpiryDate = thisRow[0]["ExpiryDate"] != null && thisRow[0]["ExpiryDate"].ToString().Trim() != "" ? DateTime.ParseExact(thisRow[0]["ExpiryDate"].ToString().Substring(0, 10), "dd/MM/yyyy", format) : DateTime.MinValue;
                blnStatus = true;
            }

            return blnStatus;

        }
        #endregion

        #region Check KYCDocTypeId And Director Id Exist
        public bool ChkKYCDocTypeIdAndDirectorIdExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("DirectorDocumentTrackingChkKYCDocTypeIdAndDirectorIdExist") as SqlCommand;
            db.AddInParameter(oCommand, "KYCDocTypeId", SqlDbType.BigInt, intKYCDocTypeId);
            db.AddInParameter(oCommand, "DirectorId", SqlDbType.VarChar, DirectorId);
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

        #region Get All By KYC Document Type
        public DataSet GetAllByKYCDocType()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DirectorDocumentTrackingSelectAllByKYCDocType") as SqlCommand;
            db.AddInParameter(dbCommand, "KYCDocTypeId", SqlDbType.BigInt, intKYCDocTypeId);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

    }
}
