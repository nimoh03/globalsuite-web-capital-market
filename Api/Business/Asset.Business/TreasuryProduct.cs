using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace Asset.Business
{
    public class TreasuryProduct
    {
        #region Properties
        public String ProductCode { get; set; }
        public String ProductName { get; set; }
        public String ProductType { get; set; }
        public String GLCode { get; set; }
        public String GLInterestPayable { get; set; }
        public String GLInterestExpensePayable { get; set; }
        public String GLGeneralIncome { get; set; }
        public String GLDepositChargeIncome { get; set; }
        public String GLTerminationChargeIncome { get; set; }
        public String GLAccountOpeningFeeIncome { get; set; }
        public String GLProcessingIncome { get; set; }
        public String GLManagementIncome { get; set; }
        public String GLPassbookChargeIncome { get; set; }
        public float FeeDepositChargeIncome { get; set; }
        public float FeeTerminationChargeIncome { get; set; }
        public Decimal FeeAccountOpeningFeeIncome { get; set; }
        public Decimal FeeProcessingIncome { get; set; }
        public float FeeManagementIncome { get; set; }
        public Decimal FeePassbookChargeIncome { get; set; }
        public Decimal MinIntialDeposit { get; set; }
        public Decimal MinACBalance { get; set; }
        public float InterestLowerLimit { get; set; }
        public float InterestUpperLimit { get; set; }
        public String SaveType { get; set; }
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
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (SaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("TreasuryProductAddNew") as SqlCommand;
                db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "PRODUCT");
            }
            else if (SaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("TreasuryProductEdit") as SqlCommand;
            }
            db.AddInParameter(oCommand, "ProductCode", SqlDbType.Char, ProductCode.Trim());
            db.AddInParameter(oCommand, "ProductType", SqlDbType.Char, ProductType.Trim());
            db.AddInParameter(oCommand, "ProductName", SqlDbType.VarChar, ProductName.Trim().ToUpper());
            db.AddInParameter(oCommand, "GLCode", SqlDbType.VarChar, GLCode.Trim());
            db.AddInParameter(oCommand, "GLInterestPayable", SqlDbType.VarChar, GLInterestPayable.Trim());
            db.AddInParameter(oCommand, "GLInterestExpensePayable", SqlDbType.VarChar, GLInterestExpensePayable.Trim());
            db.AddInParameter(oCommand, "GLGeneralIncome", SqlDbType.VarChar, GLGeneralIncome.Trim());
            db.AddInParameter(oCommand, "GLDepositChargeIncome", SqlDbType.VarChar, GLDepositChargeIncome.Trim());
            db.AddInParameter(oCommand, "GLTerminationChargeIncome", SqlDbType.VarChar, GLTerminationChargeIncome.Trim());
            db.AddInParameter(oCommand, "GLAccountOpeningFeeIncome", SqlDbType.VarChar, GLAccountOpeningFeeIncome.Trim());
            db.AddInParameter(oCommand, "GLProcessingIncome", SqlDbType.VarChar, GLProcessingIncome.Trim());
            db.AddInParameter(oCommand, "GLManagementIncome", SqlDbType.VarChar, GLManagementIncome.Trim());
            db.AddInParameter(oCommand, "GLPassbookChargeIncome", SqlDbType.VarChar, GLPassbookChargeIncome.Trim());
            db.AddInParameter(oCommand, "FeeDepositChargeIncome", SqlDbType.Real, FeeDepositChargeIncome);
            db.AddInParameter(oCommand, "FeeTerminationChargeIncome", SqlDbType.Real, FeeTerminationChargeIncome);
            db.AddInParameter(oCommand, "FeeAccountOpeningFeeIncome", SqlDbType.Decimal, FeeAccountOpeningFeeIncome);
            db.AddInParameter(oCommand, "FeeProcessingIncome", SqlDbType.Decimal, FeeProcessingIncome);
            db.AddInParameter(oCommand, "FeeManagementIncome", SqlDbType.Real, FeeManagementIncome);
            db.AddInParameter(oCommand, "FeePassbookChargeIncome", SqlDbType.Decimal, FeePassbookChargeIncome);
            db.AddInParameter(oCommand, "MinIntialDeposit", SqlDbType.Decimal, MinIntialDeposit);
            db.AddInParameter(oCommand, "MinACBalance", SqlDbType.Decimal, MinACBalance);
            db.AddInParameter(oCommand, "InterestLowerLimit", SqlDbType.Real, InterestLowerLimit);
            db.AddInParameter(oCommand, "InterestUpperLimit", SqlDbType.Real, InterestUpperLimit);
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Check TransNo Exist
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;
            if (SaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("TreasuryProductChkTransNoExist") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, ProductCode);
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

        #region Get
        public bool GetTreasuryProduct()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("TreasuryProductSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, ProductCode);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                ProductName = thisRow[0]["ProductName"].ToString().Trim();
                ProductType = thisRow[0]["ProductType"].ToString().Trim();
                GLCode = thisRow[0]["GLCode"].ToString().Trim();
                GLInterestPayable = thisRow[0]["GLInterestPayable"].ToString().Trim();
                GLInterestExpensePayable = thisRow[0]["GLInterestExpensePayable"].ToString().Trim();
                GLGeneralIncome = thisRow[0]["GLGeneralIncome"].ToString().Trim();
                GLDepositChargeIncome = thisRow[0]["GLDepositChargeIncome"].ToString().Trim();
                GLGeneralIncome = thisRow[0]["GLGeneralIncome"].ToString().Trim();
                GLTerminationChargeIncome = thisRow[0]["GLTerminationChargeIncome"].ToString().Trim();
                GLAccountOpeningFeeIncome = thisRow[0]["GLAccountOpeningFeeIncome"].ToString().Trim();
                GLProcessingIncome = thisRow[0]["GLProcessingIncome"].ToString().Trim();
                GLManagementIncome = thisRow[0]["GLManagementIncome"].ToString().Trim();
                GLPassbookChargeIncome = thisRow[0]["GLPassbookChargeIncome"].ToString().Trim();
                FeeDepositChargeIncome = float.Parse(thisRow[0]["FeeDepositChargeIncome"].ToString());
                FeeTerminationChargeIncome = float.Parse(thisRow[0]["FeeTerminationChargeIncome"].ToString());
                FeeAccountOpeningFeeIncome = Convert.ToDecimal(thisRow[0]["FeeAccountOpeningFeeIncome"]);
                FeeProcessingIncome = Convert.ToDecimal(thisRow[0]["FeeProcessingIncome"]);
                FeeManagementIncome = float.Parse(thisRow[0]["FeeManagementIncome"].ToString());
                FeePassbookChargeIncome = Convert.ToDecimal(thisRow[0]["FeePassbookChargeIncome"]);
                MinIntialDeposit = Convert.ToDecimal(thisRow[0]["MinIntialDeposit"]);
                MinACBalance = Convert.ToDecimal(thisRow[0]["MinACBalance"]);
                InterestLowerLimit = float.Parse(thisRow[0]["InterestLowerLimit"].ToString());
                InterestUpperLimit = float.Parse(thisRow[0]["InterestUpperLimit"].ToString());
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion    

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;

            dbCommand = db.GetStoredProcCommand("TreasuryProductSelectAll") as SqlCommand;

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Treasury Product GL Account
        public string GetTreasuryProductGLAccount()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("TreasuryProductSelectGLAccount") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, ProductCode);
            var varGLAccount = db.ExecuteScalar(dbCommand);
            return varGLAccount != null ? (string)varGLAccount : "";
        }
        #endregion
    }
}
