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
    public class AllowDeduct
    {
        #region Declaration
        private string strTransNo;
        private string strDescrip;
        private string strAllDed;
        private string strPayPeriod;
        private string strPayMonth;
        private string strUserID;
        private string strSaveType;
        private bool blnPension;
        private bool blnTaxable,blnAnnualPayment;
        private int intPrintPos;
        private string strStatutoryDeduction;
        public string AccountId { get; set; }
        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public string Descrip
        {
            set { strDescrip = value; }
            get { return strDescrip; }
        }
        public string AllDed
        {
            set { strAllDed = value; }
            get { return strAllDed; }
        }
        public string PayPeriod
        {
            set { strPayPeriod = value; }
            get { return strPayPeriod; }
        }
        public string PayMonth
        {
            set { strPayMonth = value; }
            get { return strPayMonth; }
        }
        public bool Pension
        {
            set { blnPension = value; }
            get { return blnPension; }
        }

        public bool AnnualPayment
        {
            set { blnAnnualPayment = value; }
            get { return blnAnnualPayment; }
        }
        public bool Taxable
        {
            set { blnTaxable = value; }
            get { return blnTaxable; }
        }
        public int PrintPos
        {
            set { intPrintPos = value; }
            get { return intPrintPos; }
        }
        public string StatutoryDeduction
        {
            set { strStatutoryDeduction = value; }
            get { return strStatutoryDeduction; }
        }
        public string UserID
        {
            set { strUserID = value; }
            get { return strUserID; }
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
            if (ChkPrintPosExist() && strStatutoryDeduction.Trim() == "")
            {
                enSaveStatus = DataGeneral.SaveStatus.NameExistEdit;
                return enSaveStatus;
            }

            if (ChkAllowanceDeductionLimit())
            {
                enSaveStatus = DataGeneral.SaveStatus.AccountIdExistAdd;
                return enSaveStatus;
            }

            if (ChkStatutoryDeductionExist())
            {
                enSaveStatus = DataGeneral.SaveStatus.DuplicateRef;
                return enSaveStatus;
            }

            if (strAllDed.Trim() == "A" && strStatutoryDeduction.Trim() != "")
            {
                enSaveStatus = DataGeneral.SaveStatus.AccountIdExistEdit;
                return enSaveStatus;
            }

            if (strStatutoryDeduction.Trim() != "" && intPrintPos > 0 )
            {
                enSaveStatus = DataGeneral.SaveStatus.HeadOfficeExistAdd;
                return enSaveStatus;
            }

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strSaveType.Trim() == "ADDS")
            {
                dbCommand = db.GetStoredProcCommand("AllDedAdd") as SqlCommand;
            }
            else if (strSaveType.Trim() == "EDIT")
            {
                dbCommand = db.GetStoredProcCommand("AllDedEdit") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "Descrip", SqlDbType.VarChar, strDescrip.ToUpper().Trim());
            db.AddInParameter(dbCommand, "AllDed", SqlDbType.VarChar, strAllDed.Trim());
            db.AddInParameter(dbCommand, "PayPeriod", SqlDbType.VarChar, strPayPeriod.Trim());
            db.AddInParameter(dbCommand, "PayMonth", SqlDbType.VarChar, strPayMonth.ToUpper().Trim());
            db.AddInParameter(dbCommand, "Pension", SqlDbType.Bit, blnPension);
            db.AddInParameter(dbCommand, "Taxable", SqlDbType.Bit, blnTaxable);
            db.AddInParameter(dbCommand, "AnnualPayment", SqlDbType.Bit, blnAnnualPayment);
            db.AddInParameter(dbCommand, "PrintPos", SqlDbType.Int, intPrintPos);
            db.AddInParameter(dbCommand, "StatutoryDeduction", SqlDbType.VarChar, strStatutoryDeduction.Trim());
            db.AddInParameter(dbCommand, "AccountId", SqlDbType.VarChar, AccountId);
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
                oCommand = db.GetStoredProcCommand("AllDedChkTransNoExist") as SqlCommand;
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

        #region Get All
        public DataSet GetAll(string strOrderBy, string strAllDedType)
        {
            //B- For Both, A- For Allowances, D- Deductions
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strAllDedType == "B")
            {
                if (strOrderBy.Trim() == "NAME")
                {
                    dbCommand = db.GetStoredProcCommand("AllDedSelectAllOrderByName") as SqlCommand;
                }
                else
                {
                    dbCommand = db.GetStoredProcCommand("AllDedSelectAll") as SqlCommand;
                }
            }
            else if (strAllDedType == "A")
            {
                dbCommand = db.GetStoredProcCommand("AllDedSelectAllAllow") as SqlCommand;
            }
            else if (strAllDedType == "D")
            {
                dbCommand = db.GetStoredProcCommand("AllDedSelectAllDeduct") as SqlCommand;
            }

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Variable Allowance\Deductions
        public DataSet GetAllVarAllDed()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllDedSelectAllVarAllDed") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get
        public bool GetAllowDeduct()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllDedSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                strDescrip = thisRow[0]["Descrip"].ToString();
                strAllDed = thisRow[0]["AllDed"].ToString();
                strPayPeriod = thisRow[0]["PayPeriod"].ToString();
                strPayMonth = thisRow[0]["PayMonth"].ToString();
                blnPension = bool.Parse(thisRow[0]["Pension"].ToString());
                blnTaxable = bool.Parse(thisRow[0]["Taxable"].ToString());
                blnAnnualPayment = thisRow[0]["AnnualPayment"] != null && thisRow[0]["AnnualPayment"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["AnnualPayment"].ToString()) : false;
                intPrintPos = thisRow[0]["PrintPos"] != null && thisRow[0]["PrintPos"].ToString().Trim() != "" ? int.Parse(thisRow[0]["PrintPos"].ToString()) : 0;
                strStatutoryDeduction = thisRow[0]["StatutoryDeduction"] != null ? thisRow[0]["StatutoryDeduction"].ToString().Trim() : "";
                AccountId = thisRow[0]["AccountId"] != null ? thisRow[0]["AccountId"].ToString() : "";
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
            SqlCommand oCommand = db.GetStoredProcCommand("AllDedChkNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "Descrip", SqlDbType.VarChar, strDescrip.Trim().ToUpper());
            db.AddInParameter(oCommand, "SaveType", SqlDbType.VarChar, strSaveType.Trim());
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            
            return blnStatus;
        }
        #endregion

        #region Check Print Position Exist
        public bool ChkPrintPosExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AllDedChkPrintPosExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "PrintPos", SqlDbType.Int, intPrintPos);
            db.AddInParameter(oCommand, "AllDed", SqlDbType.VarChar, strAllDed.Trim());
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


        #region Check Statutory Deduction Exist
        public bool ChkStatutoryDeductionExist()
        {
            bool blnStatus = false;
            if (strStatutoryDeduction.Trim() != "")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("AllDedChkStatutoryDeductionExist") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
                db.AddInParameter(oCommand, "StatutoryDeduction", SqlDbType.VarChar, strStatutoryDeduction.Trim());
                db.AddInParameter(oCommand, "SaveType", SqlDbType.VarChar, strSaveType.Trim());
                DataSet oDs = db.ExecuteDataSet(oCommand);
                if (oDs.Tables[0].Rows.Count > 0)
                {
                    blnStatus = true;
                }
            }
            return blnStatus;
        }
        #endregion

        #region Check Allowance Deduction Limit
        public bool ChkAllowanceDeductionLimit()
        {
            bool blnStatus = false;
            if (strSaveType == "ADDS" && strStatutoryDeduction.Trim() != "")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("AllDedSelectCount") as SqlCommand;
                db.AddInParameter(oCommand, "AllDed", SqlDbType.VarChar, strAllDed.Trim());
                var varAllDedNumber = db.ExecuteScalar(oCommand);
                if (varAllDedNumber != null && varAllDedNumber.ToString().Trim() != "")
                {
                    if (strAllDed.Trim() == "A" && int.Parse(varAllDedNumber.ToString()) >= 10)
                    {
                        blnStatus = true;
                    }
                    if (strAllDed.Trim() == "D" && int.Parse(varAllDedNumber.ToString()) >= 1)
                    {
                        blnStatus = true;
                    }
                }
            }
            return blnStatus;
        }
        #endregion

        #region Delete
        public DataGeneral.SaveStatus Delete()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            if (strPayPeriod.Trim() == "V")
            {
                if (ChkAllowAddExist())
                {
                    enSaveStatus = DataGeneral.SaveStatus.NameExistAdd;
                    return enSaveStatus;
                }
            }
            else
            {
                if (ChkAllowRateExist())
                {
                    enSaveStatus = DataGeneral.SaveStatus.NameExistEdit;
                    return enSaveStatus;
                }
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AllDedDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Get Allowance/Deduction Name
        public string GetAllowDeductName()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllDedSelectName") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                return thisRow[0]["Descrip"].ToString();
            }
            else
            {
                return "";
            }
        }
        #endregion


        #region Get Allowance Name Given Print Position
        public string GetAllowDeductAllowanceNamePrintPos()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllDedSelectAllowanceNameGivenPrintPos") as SqlCommand;
            db.AddInParameter(dbCommand, "PrintPos", SqlDbType.Int, intPrintPos);
            if (db.ExecuteScalar(dbCommand) != null)
            {
                return db.ExecuteScalar(dbCommand).ToString();
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region Get Deduction Name Given Print Position
        public string GetAllowDeductDeductionNamePrintPos()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllDedSelectDeductionNameGivenPrintPos") as SqlCommand;
            db.AddInParameter(dbCommand, "PrintPos", SqlDbType.Int, intPrintPos);
            if (db.ExecuteScalar(dbCommand) != null)
            {
                return db.ExecuteScalar(dbCommand).ToString();
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region Check Allowance Rate Exist
        public bool ChkAllowRateExist()
        {
            bool blnStatus = false;
            AllDedRateDetail oAllDedRateDetail = new AllDedRateDetail();
            SalaryStruct oSalaryStruct = new SalaryStruct();
            oAllDedRateDetail.AllDed = int.Parse(strTransNo);
            oSalaryStruct.AllDed = int.Parse(strTransNo);
            if (oAllDedRateDetail.GetAllDedRateDetailByAllDed().Tables[0].Rows.Count > 0 ||
                oSalaryStruct.GetAllByAllDed().Tables[0].Rows.Count > 0)
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

        #region Check Allowance Addition Exist
        public bool ChkAllowAddExist()
        {
            bool blnStatus = false;
            AllowAddition oAllowAddition = new AllowAddition();
            oAllowAddition.AllDed = int.Parse(strTransNo);
            if (oAllowAddition.GetAllByAllDed().Tables[0].Rows.Count > 0)
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

        #region Check PAYE Exist
        public bool ChkPAYEExist()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AllDedChkPAYEExist") as SqlCommand;
            DataSet oDs = db.ExecuteDataSet(oCommand);
            return oDs.Tables[0].Rows.Count > 0 ? true : false;
        }
        #endregion

        #region Check NHTF Exist
        public bool ChkNHTFExist()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AllDedChkNHTFExist") as SqlCommand;
            DataSet oDs = db.ExecuteDataSet(oCommand);
            return oDs.Tables[0].Rows.Count > 0 ? true : false;
        }
        #endregion

        #region Check NSITF Exist
        public bool ChkNSITFExist()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AllDedChkNSITFExist") as SqlCommand;
            DataSet oDs = db.ExecuteDataSet(oCommand);
            return oDs.Tables[0].Rows.Count > 0 ? true : false;
        }
        #endregion

        #region Check Advance Exist
        public bool ChkAdvanceExist()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AllDedChkAdvanceExist") as SqlCommand;
            DataSet oDs = db.ExecuteDataSet(oCommand);
            return oDs.Tables[0].Rows.Count > 0 ? true : false;
        }
        #endregion
    }
}

