using System;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class KYCExemption
    {
        #region Declarations
        private Int64 intTransNo;
        private string strCustAId;
        private Int64 intKYCDocTypeId;
        private string strSaveType;
        #endregion

        #region Properties

        public Int64 TransNo
        {
            set { intTransNo = value; }
            get { return intTransNo; }
        }
        public string CustAID
        {
            set { strCustAId = value; }
            get { return strCustAId; }
        }
        public Int64 KYCDocTypeId
        {
            set { intKYCDocTypeId = value; }
            get { return intKYCDocTypeId; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        #endregion

        #region Save Return Command
        public SqlCommand SaveCommand()
        {
            SqlCommand dbCommand = null;
            if (strSaveType.Trim() == "ADDS")
            {
                if (ChkNameExist())
                {
                    throw new Exception("Cannot Exclude Document Type For Customer. KYC Document Excluded Already For This Customer");
                }
            }
            if (strSaveType.Trim() == "EDIT" && ChkTransNoExist())
            { }
            else
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                dbCommand = db.GetStoredProcCommand("KYCExemptionAddNew") as SqlCommand;
                db.AddOutParameter(dbCommand, "TransNo", SqlDbType.VarChar, 8);
                db.AddInParameter(dbCommand, "KYCDocTypeId", SqlDbType.BigInt, intKYCDocTypeId);
                db.AddInParameter(dbCommand, "CustAId", SqlDbType.VarChar, strCustAId.Trim());
                db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            }
            return dbCommand;
        }
        #endregion


        #region Check TransNo Exist
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = null;
                oCommand = db.GetStoredProcCommand("KYCExemptionSelectByKYCDOcTypeIdAndCustomerId") as SqlCommand;
                db.AddInParameter(oCommand, "KYCDocTypeId", SqlDbType.BigInt, intKYCDocTypeId);
                db.AddInParameter(oCommand, "CustAId", SqlDbType.VarChar, strCustAId.Trim());
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

        #region Check Name Exist
        public bool ChkNameExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("KYCExemptionChkNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            db.AddInParameter(oCommand, "KYCDocTypeId", SqlDbType.BigInt, intKYCDocTypeId);
            db.AddInParameter(oCommand, "CustAId", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(oCommand, "SaveType", SqlDbType.VarChar, strSaveType.Trim());
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

        
        #region Remove KYC Exemption Return Command
        public SqlCommand RemoveKYCExemptionCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("KYCExemptionRemoveByCustomerIdAndKYCDocTypeId") as SqlCommand;
            db.AddInParameter(dbCommand, "CustAId", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(dbCommand, "KYCDocTypeId", SqlDbType.BigInt, intKYCDocTypeId);
            return dbCommand;
        }
        #endregion

        #region Get By CustomerId
        public DataSet GetKYCExemptionByCustomerId()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("KYCExemptionSelectByCustomerId") as SqlCommand;
            db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Check By CustomerId And KYCDocTypeId
        public bool CheckKYCExemptionExistByCustomerIdAndKYCDocTypeId()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("KYCExemptionSelectByCustomerIdAndKYCDocTypeId") as SqlCommand;
            db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(dbCommand, "KYCDocTypeId", SqlDbType.BigInt, intKYCDocTypeId);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            if(oDS.Tables[0].Rows.Count == 1)
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion
    }
}

