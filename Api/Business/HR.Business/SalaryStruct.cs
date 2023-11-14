using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace HR.Business
{
    public class SalaryStruct
    {
        #region Declaration
        private string strTransNo;
        private string strEmployee;
        private int intAllDed;
        private string strRateType;
        private float fltRate;
        private decimal decAmount;
        private string strUserID;

        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        
        public string Employee
        {
            set { strEmployee = value; }
            get { return strEmployee; }
        }
        public int AllDed
        {
            set { intAllDed = value; }
            get { return intAllDed; }
        }
        public string RateType
        {
            set { strRateType = value; }
            get { return strRateType; }
        }
        public float Rate
        {
            set { fltRate = value; }
            get { return fltRate; }
        }
        public decimal Amount
        {
            set { decAmount = value; }
            get { return decAmount; }
        }
        public string UserID
        {
            set { strUserID = value; }
            get { return strUserID; }
        }
        
        #endregion

        #region Add Command
        public SqlCommand AddCommand()
        {
            if (ChkNameExist())
            {
                throw new Exception("Salary Structure For This Employee Already Exist");
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;

                dbCommand = db.GetStoredProcCommand("SalaryStructAdd") as SqlCommand;

            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo);
            db.AddInParameter(dbCommand, "Employee", SqlDbType.VarChar, strEmployee.Trim());
            db.AddInParameter(dbCommand, "AllDed", SqlDbType.Int, intAllDed);
            db.AddInParameter(dbCommand, "RateType", SqlDbType.VarChar, strRateType.Trim());
            db.AddInParameter(dbCommand, "Rate", SqlDbType.Real, fltRate);
            db.AddInParameter(dbCommand, "Amount", SqlDbType.Decimal, decAmount);
            db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return dbCommand;
        }
        #endregion

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("SalaryStructSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By AllDed Rate Type
        public DataSet GetAll(string strAllDedRateType)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strAllDedRateType == "A")
            {
                dbCommand = db.GetStoredProcCommand("SalaryStructSelectAllAllow") as SqlCommand;
            }
            else
            {
                dbCommand = db.GetStoredProcCommand("SalaryStructSelectAllDedRateuct") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By Employee
        public DataSet GetAllByEmployee()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("SalaryStructSelectEmployee") as SqlCommand;
            db.AddInParameter(dbCommand, "Employee", SqlDbType.VarChar, strEmployee.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By Employee Without Statutory Deductions
        public DataSet GetAllByEmployeeWithoutStatutoryDeduction()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("SalaryStructSelectEmployeeWithoutStatutoryDeduction") as SqlCommand;
            db.AddInParameter(dbCommand, "Employee", SqlDbType.VarChar, strEmployee.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By Employee And AllDed
        public DataSet GetAllByEmployeeAllDed()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("SalaryStructSelectEmployeeAllDed") as SqlCommand;
            db.AddInParameter(dbCommand, "Employee", SqlDbType.VarChar, strEmployee.Trim());
            db.AddInParameter(dbCommand, "AllDed", SqlDbType.Int,intAllDed);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By AllDed
        public DataSet GetAllByAllDed()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("SalaryStructSelectAllDed") as SqlCommand;
            db.AddInParameter(dbCommand, "AllDed", SqlDbType.Int, intAllDed);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Salary Structure
        public bool GetSalaryStruct()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("SalaryStructSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                strEmployee = thisRow[0]["Employee"].ToString();
                strRateType = thisRow[0]["RateType"].ToString();
                fltRate = float.Parse(thisRow[0]["Rate"].ToString());
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString());

                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Check Name Exist
        public bool ChkNameExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("SalaryStructChkNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.Char, strTransNo);
            db.AddInParameter(oCommand, "Employee", SqlDbType.VarChar, strEmployee.Trim());
            db.AddInParameter(oCommand, "AllDed", SqlDbType.Int, intAllDed);
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

        #region Delete
        public DataGeneral.SaveStatus Delete()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("SalaryStructDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Delete By Employee Return Command
        public SqlCommand DeleteByEmployeeReturnCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("SalaryStructDeleteByEmployee") as SqlCommand;
            db.AddInParameter(oCommand, "Employee", SqlDbType.VarChar, strEmployee.Trim());
            return oCommand;
        }
        #endregion

        #region Delete All Return Command
        public SqlCommand DeleteAllReturnCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("SalaryStructDeleteAll") as SqlCommand;
            return oCommand;
        }
        #endregion

        #region Get PAYE By Employee
        public decimal GetPAYEByEmployee()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("SalaryStructSelectEmployeePAYE") as SqlCommand;
            db.AddInParameter(dbCommand, "Employee", SqlDbType.VarChar, strEmployee.Trim());
            var varAmount =  db.ExecuteScalar(dbCommand);
            return varAmount != null && varAmount.ToString().Trim() != "" ? decimal.Parse(varAmount.ToString()) : 0;
        }
        #endregion

        #region Get NHTF By Employee
        public decimal GetNHTFByEmployee()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("SalaryStructSelectEmployeeNHTF") as SqlCommand;
            db.AddInParameter(dbCommand, "Employee", SqlDbType.VarChar, strEmployee.Trim());
            var varAmount = db.ExecuteScalar(dbCommand);
            return varAmount != null && varAmount.ToString().Trim() != "" ? decimal.Parse(varAmount.ToString()) : 0;
        }
        #endregion

        #region Get NSITF By Employee
        public decimal GetNSITFByEmployee()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("SalaryStructSelectEmployeeNSITF") as SqlCommand;
            db.AddInParameter(dbCommand, "Employee", SqlDbType.VarChar, strEmployee.Trim());
            var varAmount = db.ExecuteScalar(dbCommand);
            return varAmount != null && varAmount.ToString().Trim() != "" ? decimal.Parse(varAmount.ToString()) : 0;
        }
        #endregion

        #region Get Advance By Employee
        public decimal GetAdvanceByEmployee()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("SalaryStructSelectEmployeeAdvance") as SqlCommand;
            db.AddInParameter(dbCommand, "Employee", SqlDbType.VarChar, strEmployee.Trim());
            var varAmount = db.ExecuteScalar(dbCommand);
            return varAmount != null && varAmount.ToString().Trim() != "" ? decimal.Parse(varAmount.ToString()) : 0;
        }
        #endregion
    }
}
