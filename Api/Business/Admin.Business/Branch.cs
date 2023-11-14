using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using GL.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace Admin.Business
    {
    public class Branch
    {
        #region Declarations
        private string strTransNo, strName;
        private string strAddress1, strAddress2, strPhone;
        private string strDefaultBranch,strShortCode,strSaveType;
        private string strIsDefaultBranch, strCommission, strTrading, strCommissionIncome;
        private bool blnJointHeadOffice;
        #endregion

        #region Properties

        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public string Name
        {
            set { strName = value; }
            get { return strName; }
        }

        public string Address1
        {
            set { strAddress1 = value; }
            get { return strAddress1; }
        }
        public string Address2
        {
            set { strAddress2 = value; }
            get { return strAddress2; }
        }
        public string Phone
        {
            set { strPhone = value; }
            get { return strPhone; }
        }

        public string DefaultBranch
        {
            set { strDefaultBranch = value; }
            get 
            {
                GLParam oGLParam = new GLParam();
                oGLParam.Type = "BRANCHVIEWONLY";
                string strBranchViewOnly = oGLParam.CheckParameter();

                if (strBranchViewOnly.Trim() == "NO")
                {
                    DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                    SqlCommand dbCommand = db.GetStoredProcCommand("BranchGetDefaultBranch") as SqlCommand;
                    strDefaultBranch = db.ExecuteScalar(dbCommand) != null ? (string)db.ExecuteScalar(dbCommand) : "";
                }
                else
                {
                    User oUser = new User();
                    oUser.UserNameAccount = GeneralFunc.UserName;
                    strDefaultBranch = oUser.GetBranchId();
                }
                return strDefaultBranch;
                
            }
        }

        public string DefaultBranchCustomer
        {
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("BranchGetDefaultBranch") as SqlCommand;
                strDefaultBranch = db.ExecuteScalar(dbCommand) != null ? (string)db.ExecuteScalar(dbCommand) : "";
                return strDefaultBranch;
            }
        }

        public string ShortCode
        {
            set { strShortCode = value; }
            get { return strShortCode; }
        }

        public string IsDefaultBranch
        {
            set { strIsDefaultBranch = value; }
            get { return strIsDefaultBranch; }
        }
        public string Commission
        {
            set { strCommission = value; }
            get { return strCommission; }
        }
        public string Trading
        {
            set { strTrading = value; }
            get { return strTrading; }
        }
        public string CommissionIncome
        {
            set { strCommissionIncome = value; }
            get { return strCommissionIncome; }
        }
        public bool JointHeadOffice
        {
            set { blnJointHeadOffice = value; }
            get { return blnJointHeadOffice; }
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
                if (strSaveType == "ADDS")
                {
                    enSaveStatus = DataGeneral.SaveStatus.NameExistAdd;
                }
                else if (strSaveType == "EDIT")
                {
                    enSaveStatus = DataGeneral.SaveStatus.NameExistEdit;
                }
                return enSaveStatus;
            }
            if (strIsDefaultBranch.Trim() == "Y")
            {
                if (ChkDefaultBranchExist())
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
            SqlCommand oCommand = null;
            if (strSaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("BranchAdd") as SqlCommand;
                db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "BRANCH");

            }
            else if (strSaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("BranchEdit") as SqlCommand;

            }
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "Name", SqlDbType.VarChar, strName.ToUpper().Trim());
            db.AddInParameter(oCommand, "Address1", SqlDbType.VarChar, strAddress1.Trim());
            db.AddInParameter(oCommand, "Address2", SqlDbType.VarChar, strAddress2.Trim());
            db.AddInParameter(oCommand, "Phone", SqlDbType.VarChar, strPhone.Trim());
            db.AddInParameter(oCommand, "DefaultBranch", SqlDbType.VarChar, strIsDefaultBranch.Trim());
            db.AddInParameter(oCommand, "ShortCode", SqlDbType.VarChar, strShortCode.Trim().ToUpper());
            db.AddInParameter(oCommand, "Commission", SqlDbType.VarChar, strCommission.Trim());
            db.AddInParameter(oCommand, "Trading", SqlDbType.VarChar, strTrading.Trim());
            db.AddInParameter(oCommand, "CommissionIncome", SqlDbType.VarChar, strCommissionIncome.Trim());
            db.AddInParameter(oCommand, "JointHeadOffice", SqlDbType.Bit, blnJointHeadOffice);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar,GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;

        }
        #endregion

        #region Check Trans No Exist
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("BranchChkTransNoExist") as SqlCommand;
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
            SqlCommand oCommand = db.GetStoredProcCommand("BranchChkNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.Char, strTransNo.Trim());
            db.AddInParameter(oCommand, "Name", SqlDbType.VarChar, strName.Trim().ToUpper());
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

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BranchSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Exclude Head Office
        public DataSet GetAllExcludeDefault()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BranchSelectAllExcludeDefault") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get
        public bool GetBranch()
        {
            bool blnStatus = false;

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BranchSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strName = thisRow[0]["Branname"].ToString();
                strAddress1 = thisRow[0]["Address1"].ToString();
                strAddress2 = thisRow[0]["Address2"].ToString();
                strPhone = thisRow[0]["Phone"].ToString();
                strIsDefaultBranch = thisRow[0]["DefaultBranch"].ToString();
                strShortCode = thisRow[0]["ShortCode"].ToString();
                strCommission = thisRow[0]["Commission"] != null ? thisRow[0]["Commission"].ToString() : "";
                strTrading = thisRow[0]["Trading"] != null ? thisRow[0]["Trading"].ToString() : "";
                strCommissionIncome = thisRow[0]["CommissionIncome"] != null ? thisRow[0]["CommissionIncome"].ToString() : "";
                blnJointHeadOffice = thisRow[0]["JointHeadOffice"] != null && thisRow[0]["JointHeadOffice"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["JointHeadOffice"].ToString()) : false;
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get Branch Name
        public string GetBranchName(string strBranchCode)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BranchSelectName") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strBranchCode.Trim());
            var strReturnBranchName = db.ExecuteScalar(dbCommand);
            return strReturnBranchName != null ? strReturnBranchName.ToString() : "";
        }
        #endregion

        #region Check Default Branch Exist
        public bool ChkDefaultBranchExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BranchChkDefaultBranchExist") as SqlCommand;
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

        #region Get Is Joint Head Office
        public bool GetIsJointHeadOffice()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BranchSelectJointHeadOffice") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            var varReturn = db.ExecuteScalar(dbCommand);
            return varReturn != null && varReturn.ToString().Trim() != "" ? bool.Parse(varReturn.ToString()) : false;
        }
        #endregion

        
    }
}
