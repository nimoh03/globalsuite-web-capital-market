using System;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class KYCCompulsoryCustExemption
    {
        #region Declarations
        private Int64 intTransNo;
        private string strCustAId;
        private string strCustomerFieldId;
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
        public string CustomerFieldId
        {
            set { strCustomerFieldId = value; }
            get { return strCustomerFieldId; }
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
                    throw new Exception("Cannot Exclude Compulsory Customer Data For Customer. Compulsory Customer Data Already Excluded For This Customer");
                }
            }
            if (strSaveType.Trim() == "EDIT" && ChkTransNoExist())
            { }
            else
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                dbCommand = db.GetStoredProcCommand("KYCCompulsoryCustExemptionAddNew") as SqlCommand;
                db.AddOutParameter(dbCommand, "TransNo", SqlDbType.VarChar, 8);
                db.AddInParameter(dbCommand, "CustomerFieldId", SqlDbType.VarChar,strCustomerFieldId);
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
                oCommand = db.GetStoredProcCommand("KYCCompulsoryCustExemptionSelectByCustomerIdAndCustomerFieldId") as SqlCommand;
                db.AddInParameter(oCommand, "CustomerFieldId", SqlDbType.VarChar, strCustomerFieldId.Trim());
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
            SqlCommand oCommand = db.GetStoredProcCommand("KYCCompulsoryCustExemptionChkNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            db.AddInParameter(oCommand, "CustomerFieldId", SqlDbType.VarChar, strCustomerFieldId.Trim());
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


        #region Remove KYC Compulsory Customer Exemption Return Command
        public SqlCommand RemoveKYCCompulsoryCustExemptionCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("KYCCompulsoryCustExemptionRemoveByCustomerIdAndCustomerFieldId") as SqlCommand;
            db.AddInParameter(dbCommand, "CustAId", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(dbCommand, "CustomerFieldId", SqlDbType.VarChar, strCustomerFieldId.Trim());
            return dbCommand;
        }
        #endregion

        #region Get By CustomerId
        public DataSet GetKYCCompulsoryCustExemptionByCustomerId()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("KYCCompulsoryCustExemptionSelectByCustomerId") as SqlCommand;
            db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Check By CustomerId And KYCDocTypeId
        public bool CheckKYCCompulsoryCustExemptionExistByCustomerIdAndCustomerFieldId()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("KYCCompulsoryCustExemptionSelectByCustomerIdAndCustomerFieldId") as SqlCommand;
            db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(dbCommand, "CustomerFieldId", SqlDbType.VarChar, strCustomerFieldId.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            if (oDS.Tables[0].Rows.Count == 1)
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion
    }
}

