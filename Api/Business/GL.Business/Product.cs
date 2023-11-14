using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class Product
    {
        #region Declaration
        private string strTransNo, strDescription, strGLAcct, strModName,strSaveType;
        private bool blnDefaultProduct;
        private string strProductClass;
        private int intProductType;
        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public string Description
        {
            set { strDescription = value; }
            get { return strDescription; }
        }
        public string GLAcct
        {
            set { strGLAcct = value; }
            get { return strGLAcct; }
        }
        public string ModName
        {
            set { strModName = value; }
            get { return strModName; }
        }
        
        public bool DefaultProduct
        {
            set { blnDefaultProduct = value; }
            get { return blnDefaultProduct; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }

        public string ProductClass
        {
            set { strProductClass = value; }
            get { return strProductClass; }
        }
        public int ProductType
        {
            set { intProductType = value; }
            get { return intProductType; }
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
            if (ChkGLAccountExist())
            {
                enSaveStatus = DataGeneral.SaveStatus.AccountIdExistAdd;
                return enSaveStatus;
            }

            if (blnDefaultProduct)
            {
                if (ChkDefaultProductExist())
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
            if (strSaveType == "ADDS")
            {
                dbCommand = db.GetStoredProcCommand("ProductAddNew") as SqlCommand;
                db.AddInParameter(dbCommand, "TableName", SqlDbType.VarChar, "PRODUCT");
            }
            else if (strSaveType == "EDIT")
            {
                ProductAcct oProductAcct = new ProductAcct();
                if (oProductAcct.GetAllByProduct(strTransNo,"ALL").Tables[0].Rows.Count == 0)
                {
                    dbCommand = db.GetStoredProcCommand("ProductEdit") as SqlCommand;
                }
                else
                {
                    dbCommand = db.GetStoredProcCommand("ProductEditNotAll") as SqlCommand;
                }
            }
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "ProductName", SqlDbType.VarChar, strDescription.Trim().ToUpper());
            db.AddInParameter(dbCommand, "GLAcct", SqlDbType.VarChar, strGLAcct.Trim());
            db.AddInParameter(dbCommand, "ModuleName", SqlDbType.VarChar, strModName.Trim());
            if (blnDefaultProduct)
            {
                db.AddInParameter(dbCommand, "DefaultProduct", SqlDbType.VarChar, "Y");
            }
            else
            {
                db.AddInParameter(dbCommand, "DefaultProduct", SqlDbType.VarChar, "N");
            }
            db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.AddInParameter(dbCommand, "ProductClass", SqlDbType.VarChar, strProductClass);
            db.AddInParameter(dbCommand, "ProductType", SqlDbType.Int, intProductType);
            db.ExecuteNonQuery(dbCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Get 
        public bool GetProduct()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {

                strTransNo = thisRow[0]["ProductCode"].ToString();
                strDescription = thisRow[0]["ProductName"].ToString();
                strGLAcct = thisRow[0]["GLAcct"].ToString();
                strModName = thisRow[0]["ModuleName"].ToString();
                if (thisRow[0]["DefaultProduct"].ToString().Trim() == "N")
                {
                    blnDefaultProduct = false;
                }
                else
                {
                    blnDefaultProduct = true;
                }
                strProductClass = thisRow[0]["ProductClass"] == null ? "" : thisRow[0]["ProductClass"].ToString();
                intProductType = thisRow[0]["ProductType"] == null || thisRow[0]["ProductType"].ToString().Trim() == ""
                    ? 0 : int.Parse(thisRow[0]["ProductType"].ToString());

                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }


            return blnStatus;
        }
        #endregion

        #region Get By Module
        public bool GetProduct(string strModuleName)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductSelectByModule") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "ModuleName", SqlDbType.VarChar, strModuleName.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["ProductCode"].ToString();
                strDescription = thisRow[0]["ProductName"].ToString();
                strGLAcct = thisRow[0]["GLAcct"].ToString();
                strModName = thisRow[0]["ModuleName"].ToString();
                if (thisRow[0]["DefaultProduct"].ToString().Trim() == "N")
                {
                    blnDefaultProduct = false;
                }
                else
                {
                    blnDefaultProduct = true;
                }
                strProductClass = thisRow[0]["ProductClass"] == null ? "" : thisRow[0]["ProductClass"].ToString();
                intProductType = thisRow[0]["ProductType"] == null || thisRow[0]["ProductType"].ToString().Trim() == ""
                    ? 0 : int.Parse(thisRow[0]["ProductType"].ToString());
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }


            return blnStatus;
        }
        #endregion

        #region Get By ModuleName WIth Product Class
        public bool GetProductByProductClass(string strModuleNameCode,string strProductClassCode)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductSelectByModuleWithProductClass") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "ModuleName", SqlDbType.VarChar, strModuleNameCode.Trim());
            db.AddInParameter(dbCommand, "ProductClass", SqlDbType.VarChar, strProductClassCode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["ProductCode"].ToString();
                strDescription = thisRow[0]["ProductName"].ToString();
                strGLAcct = thisRow[0]["GLAcct"].ToString();
                strModName = thisRow[0]["ModuleName"].ToString();
                if (thisRow[0]["DefaultProduct"].ToString().Trim() == "N")
                {
                    blnDefaultProduct = false;
                }
                else
                {
                    blnDefaultProduct = true;
                }
                strProductClass = thisRow[0]["ProductClass"] == null ? "" : thisRow[0]["ProductClass"].ToString();
                intProductType = thisRow[0]["ProductType"] == null || thisRow[0]["ProductType"].ToString().Trim() == ""
                    ? 0 : int.Parse(thisRow[0]["ProductType"].ToString());
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }


            return blnStatus;
        }
        #endregion

        #region Get All 
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By Module
        public DataSet GetAllByModule(string strModuleName)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductSelectAllByModule") as SqlCommand;
            db.AddInParameter(dbCommand, "ModuleName", SqlDbType.VarChar, strModuleName.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By Product Class
        public DataSet GetAllByModuleWithProductClass(string strModuleName)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductSelectAllByModuleWithProductClass") as SqlCommand;
            db.AddInParameter(dbCommand, "ModuleName", SqlDbType.VarChar, strModuleName.Trim());
            db.AddInParameter(dbCommand, "ProductClass", SqlDbType.VarChar, strProductClass.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By Account
        public DataSet GetAllByAccount(string strAccountId)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductSelectAllByAccountId") as SqlCommand;
            db.AddInParameter(dbCommand, "AccountId", SqlDbType.VarChar, strAccountId.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Product Name
        public string GetProductName()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductSelectProductName") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar,strTransNo.Trim());
            return (string) db.ExecuteScalar(dbCommand);
        }
        #endregion

        #region Get Product Name By Module
        public string GetProductNameByModule(string strModuleName)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductSelectProductNameByModule") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "ModuleName", SqlDbType.VarChar, strModuleName.Trim());
            if (db.ExecuteScalar(dbCommand) != null)
            {
                return (string)db.ExecuteScalar(dbCommand);
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region Get Product GL Account
        public string GetProductGLAcct()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductSelectProductGLAcct") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strTransNo.Trim());
            return (string)db.ExecuteScalar(dbCommand);
        }
        #endregion

        #region Get Product Module 
        public string GetProductModule()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductSelectProductModule") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strTransNo.Trim());
            var varModuleName = db.ExecuteScalar(dbCommand);
            return varModuleName != null ? varModuleName.ToString() : "";

        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ProductDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
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
                SqlCommand oCommand = db.GetStoredProcCommand("ProductChkTransNoExist") as SqlCommand;
                
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
            SqlCommand oCommand = db.GetStoredProcCommand("ProductChkNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "ProductCode", SqlDbType.Char, strTransNo.Trim());
            db.AddInParameter(oCommand, "ProductName", SqlDbType.VarChar, strDescription.Trim().ToUpper());
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

        #region Check GL Account Exist
        public bool ChkGLAccountExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ProductChkGLAccountExist") as SqlCommand;
            db.AddInParameter(oCommand, "ProductCode", SqlDbType.Char, strTransNo.Trim());
            db.AddInParameter(oCommand, "GLAccount", SqlDbType.VarChar, strGLAcct.Trim());
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

        #region Check Default Product Exist
        public bool ChkDefaultProductExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ProductChkDefaultProductExist") as SqlCommand;
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

        #region Get The Product Made Has  Default
        public string GetDefaultProduct()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductGetDefaultProduct") as SqlCommand;
            var varDefault = db.ExecuteScalar(dbCommand);
            return varDefault != null ? varDefault.ToString() : "";
        }
        #endregion

        
        

        #region Check If Loan Product
        public bool ChkIfLoanProduct()
        {
            bool blnStatus = false;
            blnStatus = GetProductByProductClass("FIN","LOAN");
            return blnStatus;
        }
        #endregion

        #region Convert To List
        public IEnumerable<Product> ConvertToList(DataTable dataTable)
        {
            var oLists = new List<Product>();
            foreach (DataRow oRow in dataTable.Rows)
            {
                var oList = new Product
                {
                    TransNo = Convert.ToString(oRow["TransNo"]),
                    Description = Convert.ToString(oRow["Description"])
                };
                oLists.Add(oList);
            }
            return oLists;
        }
        #endregion

    }
}
