using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace CustomerManagement.Business
{
    public class CustomerType
    {
        #region Declaration
        private string strTransNo;
        private string strCustomerTypeName;
        private bool blnIsDefault;
        private bool blnIsInstitutional;
        private bool blnIsJointAccount;
        private string strSaveType;

        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public string CustomerTypeName
        {
            set { strCustomerTypeName = value; }
            get { return strCustomerTypeName; }
        }
        public bool IsDefault
        {
            set { blnIsDefault = value; }
            get { return blnIsDefault; }
        }

        public bool IsInstitutional
        {
            set { blnIsInstitutional = value; }
            get { return blnIsInstitutional; }
        }
        public bool IsJointAccount
        {
            set { blnIsJointAccount = value; }
            get { return blnIsJointAccount; }
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
            if (!ChkTransNoExist())
            {
                enSaveStatus = DataGeneral.SaveStatus.NotExist;
                return enSaveStatus;
            }
            if (ChkNameExist())
            {
                enSaveStatus = DataGeneral.SaveStatus.NameExistAdd;
                return enSaveStatus;
            }

            if (blnIsDefault)
            {
                if (ChkIsDefaultCustomerTypeExist())
                {
                    if (strSaveType == "ADDS")
                    {
                        enSaveStatus = DataGeneral.SaveStatus.HeadOfficeExistAdd;
                    }
                    else if (strSaveType == "EDIT")
                    {
                        enSaveStatus = DataGeneral.SaveStatus.HeadOfficeExistEdit;
                    }
                    return enSaveStatus;
                }
            }

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strSaveType.Trim() == "ADDS")
            {
                dbCommand = db.GetStoredProcCommand("CustomerTypeAddNew") as SqlCommand;
            }
            else if (strSaveType.Trim() == "EDIT")
            {
                dbCommand = db.GetStoredProcCommand("CustomerTypeEdit") as SqlCommand;
            }

            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "CustomerTypeName", SqlDbType.VarChar, strCustomerTypeName.Trim().ToUpper());
            db.AddInParameter(dbCommand, "IsDefault", SqlDbType.Bit, blnIsDefault);
            db.AddInParameter(dbCommand, "IsInstitutional", SqlDbType.Bit, blnIsInstitutional);
            db.AddInParameter(dbCommand, "IsJointAccount", SqlDbType.Bit, blnIsJointAccount);
            db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(dbCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
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
                SqlCommand oCommand = null;
                oCommand = db.GetStoredProcCommand("CustomerTypeChkTransNoExist") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
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
            SqlCommand oCommand = db.GetStoredProcCommand("CustomerTypeChkNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.Char, strTransNo.Trim());
            db.AddInParameter(oCommand, "Name", SqlDbType.VarChar, strCustomerTypeName.Trim().ToUpper());
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

        #region Check Is Default Customer Type Exist
        public bool ChkIsDefaultCustomerTypeExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("CustomerTypeChkIsDefaultExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
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

        #region Get Base CustomerType
        public int GetIsDefaultCustomerType()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustomerTypeSelectIsDefault") as SqlCommand;
            var varReturn = db.ExecuteScalar(dbCommand);
            return varReturn != null && varReturn.ToString().Trim() != "" ? int.Parse(varReturn.ToString()) : 0;
        }
        #endregion

        #region Check Is Joint Account
        public bool ChkIsJointAccount()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustomerTypeSelectIsJointAccount") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            var varReturn = db.ExecuteScalar(dbCommand);
            return varReturn != null && varReturn.ToString().Trim() != "" ? bool.Parse(varReturn.ToString()) : false;
        }
        #endregion
        
        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustomerTypeSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion
        
        #region Get All Return List
        public List<CustomerType> GetAllReturnList()
        {
            List<CustomerType> lstCustomerType = new List<CustomerType>();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustomerTypeSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            foreach(DataRow oRow in oDS.Tables[0].Rows)
            {
                CustomerType oCustomerType = new CustomerType();
                oCustomerType.strTransNo = oRow["TransNo"].ToString();
                oCustomerType.strCustomerTypeName = oRow["CustomerTypeName"].ToString();
                lstCustomerType.Add(oCustomerType);
            }
            return lstCustomerType;
        }
        #endregion
        
        #region Get CustomerType
        public bool GetCustomerType()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustomerTypeSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                strCustomerTypeName = thisRow[0]["CustomerTypeName"].ToString();
                blnIsDefault = bool.Parse(thisRow[0]["IsDefault"] != null && thisRow[0]["IsDefault"].ToString().Trim() != "" ? thisRow[0]["IsDefault"].ToString() : "False");
                blnIsInstitutional = bool.Parse(thisRow[0]["IsInstitutional"] != null && thisRow[0]["IsInstitutional"].ToString().Trim() != "" ? thisRow[0]["IsInstitutional"].ToString() : "False");
                blnIsJointAccount = bool.Parse(thisRow[0]["IsJointAccount"] != null && thisRow[0]["IsJointAccount"].ToString().Trim() != "" ? thisRow[0]["IsJointAccount"].ToString() : "False");
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
