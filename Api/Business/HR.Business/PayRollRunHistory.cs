using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.SqlTypes;
using System.Globalization;
using Admin.Business;
using BaseUtility.Business;
using GL.Business;

namespace HR.Business
{
    public class PayRollRunHistory
    {
        #region Declarations
        private DateTime datPayRollDate;
        private string strUserId;
        #endregion

        #region Properties

        public DateTime PayRollDate
        {
            set { datPayRollDate = value; }
            get { return datPayRollDate; }
        }
        public string UserId
        {
            set { strUserId = value; }
            get { return strUserId; }
        }


        #endregion

        #region Save
        public SqlCommand Save()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PayRollRunHistoryAdd") as SqlCommand;
            db.AddInParameter(dbCommand, "PayRollDate", SqlDbType.DateTime, datPayRollDate);
            db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return dbCommand;

        }
        #endregion

        #region Get All
        public DataSet GetAll(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PayRollRunHistorySelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("PayRollRunHistorySelectAllReversed") as SqlCommand;
                db.AddInParameter(dbCommand, "PayRollDate", SqlDbType.DateTime, datPayRollDate);
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PayRollRunHistorySelectAllPostedAsc") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get
        public bool GetPayRollRunHistory(DataGeneral.PostStatus TransStatus)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            SqlCommand dbCommand = null;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PayRollRunHistorySelectPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("PayRollRunHistorySelectReversed") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "PayRollDate", SqlDbType.DateTime, datPayRollDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                datPayRollDate = DateTime.ParseExact(thisRow[0]["PayRollDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Update Reversal and Return A Command
        public SqlCommand UpDateRevCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PayRollRunHistoryUpdateRev") as SqlCommand;
            db.AddInParameter(oCommand, "PayRollDate", SqlDbType.DateTime, datPayRollDate);
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName);
            return oCommand;
        }
        #endregion

        #region Update Post and Return A Command
        public SqlCommand UpDatePostCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PayRollRunHistoryUpdatePost") as SqlCommand;
            db.AddInParameter(oCommand, "PayRollDate", SqlDbType.DateTime, datPayRollDate);
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName);
            return oCommand;
        }
        #endregion

        #region Check PayRoll Date Exist
        public bool ChkPayRollDateExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PayRollRunHistoryChkPayRollDateExist") as SqlCommand;
            db.AddInParameter(oCommand, "PayRollDate", SqlDbType.DateTime, datPayRollDate);
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Check PayRoll Date Exist Posted
        public bool ChkPayRollDateExistPosted()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PayRollRunHistoryChkPayRollDateExistPosted") as SqlCommand;
            db.AddInParameter(oCommand, "PayRollDate", SqlDbType.DateTime, datPayRollDate);
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Check Greater Equal PayRoll Date Exist
        public bool ChkGreaterEqualPayRollDate()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PayRollRunHistoryChkGreaterEqualPayRollDate") as SqlCommand;
            db.AddInParameter(oCommand, "PayRollDate", SqlDbType.DateTime, datPayRollDate);
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Run
        public DataGeneral.SaveStatus Run(bool blnIsReposted)
        {
            #region Properties
            string strAddMonthLastDay;
            string strComputeTaxableAllowanceOnly;
            string strPostToIndiviualAccount;
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "ADDMONTHLASTDAY";
            strAddMonthLastDay = oGLParam.CheckParameter().Trim();
            oGLParam.Type = "COMPUTETAXABLEALLOW";
            strComputeTaxableAllowanceOnly = oGLParam.CheckParameter().Trim();
            oGLParam.Type = "PAYROLLPOSTINDVACCT";
            strPostToIndiviualAccount = oGLParam.CheckParameter().Trim();
            IFormatProvider format = new CultureInfo("en-GB");
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            #endregion

            #region Get HR Parameter Values
            HRParam oHRParam = new HRParam();
            if (!oHRParam.GetHRGentable())
            {
                throw new Exception("HR Parameter Setup Does Not Exist");
            }
            #endregion

            #region Check PayRoll Date Constraints
            DateTime datPayRollDateReturn = oHRParam.PayRollDate;
            if (datPayRollDateReturn == DateTime.MinValue)
            {
                throw new Exception("Error:PayRoll Parameter Table Not Set.Please Setup It Up In The HR Parameter Setup");
            }

            datPayRollDate = datPayRollDateReturn; 
            if (ChkPayRollDateExistPosted())
            {
                enSaveStatus = DataGeneral.SaveStatus.NameExistAdd;
                return enSaveStatus;
            }
            //if (ChkGreaterEqualPayRollDate())
            //{
            //      string strErrMessage;
            //    strErrMessage = "Cannot Save, PayRoll Date Less Than Or Equal To PayRoll Dates Already Processed";
            //    throw new Exception(strErrMessage);
            //}
            #endregion

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    #region Declare Objects and assign default values with Jnumber Setup
                    Branch oBranch = new Branch();
                    PayRollProcess oPayRollProcess = new PayRollProcess();
                    Employee oEmployee = new Employee();
                    SalaryStruct oSalaryStruct = new SalaryStruct();
                    AllowDeduct oAllowDeduct = new AllowDeduct();
                    string strEmployeeFullName = "";
                    string strEmployeeProductId = "";
                    string strEmployeeCustomerId = "";
                    decimal decTotalStaffGrossAmount = 0;
                    decimal decTotalStaffPensionAmountStaff = 0;
                    decimal decTotalStaffPensionAmountEmployer = 0;
                    decimal decTotalStaffTaxAmount = 0;
                    decimal decTotalStaffLoanAmount = 0;
                    decimal decTotalNHTFAmount = 0;

                    decimal decActualStaffBasicSalaryAmount = 0;
                    decimal decStaffPensionItemAmount = 0;
                    decimal decStaffTaxableItemAmount = 0;
                    decimal decStaffAllowanceItemAmount = 0;
                    decimal decStaffDeductionItemAmount = 0;

                    string strJnumberNext = "";
                    SqlCommand oCommandJnumber = db.GetStoredProcCommand("JnumberAdd") as SqlCommand;
                    db.AddOutParameter(oCommandJnumber, "Jnumber", SqlDbType.BigInt, 8);
                    db.ExecuteNonQuery(oCommandJnumber, transaction);
                    strJnumberNext = db.GetParameterValue(oCommandJnumber, "Jnumber").ToString();
                    #endregion

                    AcctGL oGL = new AcctGL();
                    Product oProduct = new Product();
                    foreach (DataRow oRow in oEmployee.GetAllToGenerateSalary().Tables[0].Rows)
                    {
                        #region Default INDIVIDUAL Basic,Pension,Tax,All,Ded variable to zero
                        decActualStaffBasicSalaryAmount = 0;
                        decStaffPensionItemAmount = 0;
                        decStaffTaxableItemAmount = 0;
                        decStaffAllowanceItemAmount = 0;
                        decStaffDeductionItemAmount = 0;
                        strEmployeeFullName = oEmployee.GetEmployeeName(oRow["TransNo"].ToString()).Trim();
                        if(oEmployee.GetEmployeeCustomerIdProductId(oRow["TransNo"].ToString()))
                        {
                            strEmployeeProductId = oEmployee.ProductId;
                            strEmployeeCustomerId = oEmployee.CustomerId;
                        }
                        #endregion

                        #region Set Basic Salary
                        if (oRow["BasicSalary"] != null && oRow["BasicSalary"].ToString().Trim() != "")
                        {
                            if (oRow["SalaryRateType"].ToString().Trim() == "Y")
                            {
                                decActualStaffBasicSalaryAmount = decimal.Parse(oRow["BasicSalary"].ToString()) / 12;
                            }
                            else
                            {
                                decActualStaffBasicSalaryAmount = decimal.Parse(oRow["BasicSalary"].ToString());
                            }
                        }
                        #endregion

                        #region Adding All Allowances And Deductions For The Staff In PayRollProcee Table
                        
                        oSalaryStruct.Employee = oRow["TransNo"].ToString();
                        
                        foreach (DataRow oRowSub in oSalaryStruct.GetAllByEmployeeWithoutStatutoryDeduction().Tables[0].Rows)
                        {
                            #region Check Allowance Or Deduction Item In Salary Structure Exist
                            oAllowDeduct.TransNo = oRowSub["AllDed"].ToString();
                            if (!oAllowDeduct.GetAllowDeduct())
                            {
                                throw new Exception("PayRoll Item Does Not Exist");
                            }
                            #endregion

                            #region Set All/Ded,Amount,Employee,PayRollDate,SysRef to PAYROLLPROCESS Class Property Values AND Add All/Ded Amount To INDIVIDUAL STAFF ALLOWANCE OR DEDUCTION Variable
                            oPayRollProcess.AllDed = int.Parse(oRowSub["AllDed"].ToString());
                            if (oRowSub["RateType"].ToString().Trim() == "Y")
                            {
                                oPayRollProcess.Amount = decimal.Parse(oRowSub["Amount"].ToString()) / 12;
                            }
                            else
                            {
                                oPayRollProcess.Amount = decimal.Parse(oRowSub["Amount"].ToString());
                            }
                            if (oAllowDeduct.AllDed.Trim() == "A")
                            {
                                oPayRollProcess.AllDedType = "A";
                                oPayRollProcess.ItemType = "ALLOW";
                                decStaffAllowanceItemAmount = decStaffAllowanceItemAmount + oPayRollProcess.Amount;
                            }
                            else if (oAllowDeduct.AllDed.Trim() == "D")
                            {
                                oPayRollProcess.AllDedType = "D";
                                oPayRollProcess.ItemType = "DEDUCT";
                                decStaffDeductionItemAmount = decStaffDeductionItemAmount + oPayRollProcess.Amount;
                            }
                            else
                            {
                                throw new Exception("PayRoll Item Type Does Not Exist");
                            }
                            oPayRollProcess.Employee = oRow["TransNo"].ToString();
                            oPayRollProcess.PayRollDate = datPayRollDateReturn;
                            oPayRollProcess.SysRef = "PAY-" + datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oPayRollProcess.SaveType = "ADDS";
                            #endregion

                            #region Add All/Ded Amt To TOTAL GROSS And INDIVIDUAL PENSION,TAX Variable Save To PAYROLLPROCESS Table
                            if (oPayRollProcess.Amount > 0)
                            {
                                if (!oAllowDeduct.AnnualPayment)
                                {
                                    if (oAllowDeduct.AllDed.Trim() == "A")
                                    {
                                        decTotalStaffGrossAmount = decTotalStaffGrossAmount + oPayRollProcess.Amount;
                                    }
                                    else
                                    {
                                        decTotalStaffGrossAmount = decTotalStaffGrossAmount - oPayRollProcess.Amount;
                                    }
                                    decTotalStaffGrossAmount = Math.Round(decTotalStaffGrossAmount, 2);
                                }
                                if (oAllowDeduct.Pension)
                                {
                                    decStaffPensionItemAmount = decStaffPensionItemAmount + oPayRollProcess.Amount;
                                    decStaffPensionItemAmount = Math.Round(decStaffPensionItemAmount, 2);
                                }
                                if (oAllowDeduct.Taxable)
                                {
                                    decStaffTaxableItemAmount = decStaffTaxableItemAmount + oPayRollProcess.Amount;
                                    decStaffTaxableItemAmount = Math.Round(decStaffTaxableItemAmount, 2);
                                }
                                SqlCommand dbCommandPayRollProcess = oPayRollProcess.AddCommand();
                                db.ExecuteNonQuery(dbCommandPayRollProcess, transaction);

                                if ((strPostToIndiviualAccount.Trim() == "YES") && oPayRollProcess.Amount > 0)
                                {
                                    if (oAllowDeduct.AllDed.Trim() == "A")
                                    {
                                        oGL.EffectiveDate = datPayRollDateReturn;
                                        oGL.AccountID = "";
                                        oGL.MasterID = oAllowDeduct.AccountId;
                                        oGL.AcctRef = "";
                                        oGL.Credit = 0;
                                        oGL.Debit = oPayRollProcess.Amount;
                                        oGL.Debcred = "D";
                                        oGL.Desciption = GeneralFunc.GetMonthName(datPayRollDateReturn.Month) + " " + datPayRollDateReturn.Year.ToString() + " " + oAllowDeduct.Descrip.Trim() + " For " + strEmployeeFullName;
                                        oGL.TransType = "PAYROLL";
                                        oGL.SysRef = "PAY-" + datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                                        oGL.Ref01 = datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                                        oGL.Reverse = "N";
                                        oGL.Jnumber = strJnumberNext;
                                        oGL.Branch = oBranch.DefaultBranch;
                                        oGL.FeeType = "EMPALLDEDSING";
                                        oGL.Ref02 = ""; oGL.Chqno = ""; oGL.RecAcctMaster = oGL.MasterID; oGL.RecAcct = ""; oGL.AcctRefSecond = ""; oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                                        SqlCommand dbCommandStaffLoanSalaryPayable = oGL.AddCommand();
                                        db.ExecuteNonQuery(dbCommandStaffLoanSalaryPayable, transaction);
                                    }
                                    else
                                    {
                                        oGL.EffectiveDate = datPayRollDateReturn;
                                        oGL.AccountID = "";
                                        oGL.MasterID = oAllowDeduct.AccountId;
                                        oGL.AcctRef = "";
                                        oGL.Credit = oPayRollProcess.Amount;
                                        oGL.Debit = 0;
                                        oGL.Debcred = "C";
                                        oGL.Desciption = GeneralFunc.GetMonthName(datPayRollDateReturn.Month) + " " + datPayRollDateReturn.Year.ToString() + " " + oAllowDeduct.Descrip.Trim() + " For " + strEmployeeFullName;
                                        oGL.TransType = "PAYROLL";
                                        oGL.SysRef = "PAY-" + datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                                        oGL.Ref01 = datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                                        oGL.Reverse = "N";
                                        oGL.Jnumber = strJnumberNext;
                                        oGL.Branch = oBranch.DefaultBranch;
                                        oGL.FeeType = "EMPALLDEDSING";
                                        oGL.Ref02 = ""; oGL.Chqno = ""; oGL.RecAcctMaster = oGL.MasterID; oGL.RecAcct = ""; oGL.AcctRefSecond = ""; oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                                        SqlCommand dbCommandStaffLoanSalaryPayable = oGL.AddCommand();
                                        db.ExecuteNonQuery(dbCommandStaffLoanSalaryPayable, transaction);
                                    }
                                }
                            }
                            #endregion
                        }
                        #endregion

                        #region  Add Basic Salary To PayRollProcee And Add Basic to TOTAL Gross
                        oPayRollProcess.AllDed = 99996;
                        oPayRollProcess.AllDedType = "A";
                        oPayRollProcess.ItemType = "BASIC";
                        oPayRollProcess.Amount = decActualStaffBasicSalaryAmount;
                        oPayRollProcess.Employee = oRow["TransNo"].ToString();
                        oPayRollProcess.PayRollDate = datPayRollDateReturn;
                        oPayRollProcess.SysRef = "PAY-" + datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                        oPayRollProcess.SaveType = "ADDS";
                        if (decActualStaffBasicSalaryAmount > 0)
                        {
                            decTotalStaffGrossAmount = decTotalStaffGrossAmount + decActualStaffBasicSalaryAmount;

                            SqlCommand dbCommandBasic = oPayRollProcess.AddCommand();
                            db.ExecuteNonQuery(dbCommandBasic, transaction);
                        }

                        if ((strPostToIndiviualAccount.Trim() == "YES") && decActualStaffBasicSalaryAmount > 0)
                        {
                            oGL.EffectiveDate = datPayRollDateReturn;
                            oGL.AccountID = "";
                            oGL.MasterID = oHRParam.SalExpenseAC;
                            oGL.AcctRef = "";
                            oGL.Credit = 0;
                            oGL.Debit = decActualStaffBasicSalaryAmount;
                            oGL.Debcred = "D";
                            oGL.Desciption = GeneralFunc.GetMonthName(datPayRollDateReturn.Month) + " " + datPayRollDateReturn.Year.ToString() + " Basic Salary For " + strEmployeeFullName;
                            oGL.TransType = "PAYROLL";
                            oGL.SysRef = "PAY-"  + datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oGL.Ref01 = datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oGL.Reverse = "N";
                            oGL.Jnumber = strJnumberNext;
                            oGL.Branch = oBranch.DefaultBranch;
                            oGL.FeeType = "EMPBASICPAYSING";
                            oGL.Ref02 = ""; oGL.Chqno = ""; oGL.RecAcctMaster = oGL.MasterID; oGL.RecAcct = ""; oGL.AcctRefSecond = ""; oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                            SqlCommand dbCommandStaffLoanSalaryPayable = oGL.AddCommand();
                            db.ExecuteNonQuery(dbCommandStaffLoanSalaryPayable, transaction);
                        }
                        #endregion

                        #region Add PENSION EMPLOYEE To PayRollProcess,Calulate Pension Amt Of Employee,Employer And Add to TOTAL Pension Employee var Only
                        decimal decActualPensionAmount = 0;
                        decimal decActualPensionAmountEmployer = 0;
                        oEmployee.TransNo = oRow["TransNo"].ToString();
                        if (oEmployee.DoNotChargeNSITF)
                        { }
                        else
                        {
                            if (!oAllowDeduct.ChkNSITFExist())
                            {
                                decActualPensionAmount = ((decActualStaffBasicSalaryAmount + decStaffPensionItemAmount) * decimal.Parse(oHRParam.EmployeePension.ToString())) / decimal.Parse("100");
                                decActualPensionAmount = Math.Round(decActualPensionAmount, 2);
                                decActualPensionAmountEmployer = ((decActualStaffBasicSalaryAmount + decStaffPensionItemAmount) * decimal.Parse(oHRParam.EmployerPension.ToString())) / decimal.Parse("100");
                                decActualPensionAmountEmployer = Math.Round(decActualPensionAmountEmployer, 2);
                            }
                            else
                            {
                                decActualPensionAmount = oSalaryStruct.GetNSITFByEmployee();
                                decActualPensionAmount = Math.Round(decActualPensionAmount, 2);
                                //decActualPensionAmountEmployer = oSalaryStruct.GetNSITFByEmployer();
                                //decActualPensionAmountEmployer = Math.Round(decActualPensionAmount, 2);
                            }
                            oPayRollProcess.AllDed = 99992;
                            oPayRollProcess.AllDedType = "D";
                            oPayRollProcess.ItemType = "PENEM";
                            oPayRollProcess.Amount = decActualPensionAmount;
                            oPayRollProcess.Employee = oRow["TransNo"].ToString();
                            oPayRollProcess.PayRollDate = datPayRollDateReturn;
                            oPayRollProcess.SysRef = "PAY-"  + datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oPayRollProcess.SaveType = "ADDS";
                            if (decActualPensionAmount > 0)
                            {
                                decTotalStaffPensionAmountStaff = decTotalStaffPensionAmountStaff + decActualPensionAmount;
                                SqlCommand dbCommandPensionStaff = oPayRollProcess.AddCommand();
                                db.ExecuteNonQuery(dbCommandPensionStaff, transaction);
                            }

                            if ((strPostToIndiviualAccount.Trim() == "YES") && decActualPensionAmount > 0)
                            {
                                oGL.EffectiveDate = datPayRollDateReturn;
                                oGL.AccountID = "";
                                oGL.MasterID = oHRParam.PensionAC;
                                oGL.AcctRef = "";
                                oGL.Credit = decActualPensionAmount;
                                oGL.Debit = 0;
                                oGL.Debcred = "C";
                                oGL.Desciption = GeneralFunc.GetMonthName(datPayRollDateReturn.Month) + " " + datPayRollDateReturn.Year.ToString() +  " Pension For " + strEmployeeFullName;
                                oGL.TransType = "PAYROLL";
                                oGL.SysRef = "PAY-" +  datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                                oGL.Ref01 = datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                                oGL.Reverse = "N";
                                oGL.Jnumber = strJnumberNext;
                                oGL.Branch = oBranch.DefaultBranch;
                                oGL.FeeType = "EMPPENPAYSING";
                                oGL.Ref02 = ""; oGL.Chqno = ""; oGL.RecAcctMaster = oGL.MasterID; oGL.RecAcct = ""; oGL.AcctRefSecond = ""; oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                                SqlCommand dbCommandStaffLoanSalaryPayable = oGL.AddCommand();
                                db.ExecuteNonQuery(dbCommandStaffLoanSalaryPayable, transaction);
                            }
                        }
                        #endregion

                        #region Add PENSION EMPLOYER To PayRollProcess And Add to TOTAL Pension Employer
                        oPayRollProcess.AllDed = 99993;
                        oPayRollProcess.AllDedType = "D";
                        oPayRollProcess.ItemType = "PENER";
                        oPayRollProcess.Amount = decActualPensionAmountEmployer;
                        oPayRollProcess.Employee = oRow["TransNo"].ToString();
                        oPayRollProcess.PayRollDate = datPayRollDateReturn;
                        oPayRollProcess.SysRef = "PAY-" + datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                        oPayRollProcess.SaveType = "ADDS";
                        if (decActualPensionAmountEmployer > 0)
                        {
                            decTotalStaffPensionAmountEmployer = decTotalStaffPensionAmountEmployer + decActualPensionAmountEmployer;
                            SqlCommand dbCommandPensionEmployer = oPayRollProcess.AddCommand();
                            db.ExecuteNonQuery(dbCommandPensionEmployer, transaction);
                        }

                        if ((strPostToIndiviualAccount.Trim() == "YES") && decActualPensionAmountEmployer > 0)
                        {
                            oGL.EffectiveDate = datPayRollDateReturn;
                            oGL.AccountID = "";
                            oGL.MasterID = oHRParam.PensionEmployerAC;
                            oGL.AcctRef = "";
                            oGL.Credit = decActualPensionAmountEmployer;
                            oGL.Debit = 0;
                            oGL.Debcred = "C";
                            oGL.Desciption = GeneralFunc.GetMonthName(datPayRollDateReturn.Month) + " " + datPayRollDateReturn.Year.ToString() +  " Pen Employer For " + strEmployeeFullName;
                            oGL.TransType = "PAYROLL";
                            oGL.SysRef = "PAY-" +  datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oGL.Ref01 = datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oGL.Reverse = "N";
                            oGL.Jnumber = strJnumberNext;
                            oGL.Branch = oBranch.DefaultBranch;
                            oGL.FeeType = "EMPPENEMPSING";
                            oGL.Ref02 = ""; oGL.Chqno = ""; oGL.RecAcctMaster = oGL.MasterID; oGL.RecAcct = ""; oGL.AcctRefSecond = ""; oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                            SqlCommand dbCommandPensionEmployerPerEmployee = oGL.AddCommand();
                            db.ExecuteNonQuery(dbCommandPensionEmployerPerEmployee, transaction);

                            oGL.EffectiveDate = datPayRollDateReturn;
                            oGL.AccountID = "";
                            oGL.MasterID = oHRParam.PensionEmployerExpAC;
                            oGL.AcctRef = "";
                            oGL.Credit = 0;
                            oGL.Debit = decActualPensionAmountEmployer;
                            oGL.Debcred = "D";
                            oGL.Desciption = GeneralFunc.GetMonthName(datPayRollDateReturn.Month) + " " + datPayRollDateReturn.Year.ToString() +  " Pen Employer For " + strEmployeeFullName;
                            oGL.TransType = "PAYROLL";
                            oGL.SysRef = "PAY-" + datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oGL.Ref01 = datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oGL.Reverse = "N";
                            oGL.Jnumber = strJnumberNext;
                            oGL.Branch = oBranch.DefaultBranch;
                            oGL.FeeType = "EMPPENEMPEXPSING";
                            oGL.Ref02 = ""; oGL.Chqno = ""; oGL.RecAcctMaster = oGL.MasterID; oGL.RecAcct = ""; oGL.AcctRefSecond = ""; oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                            SqlCommand dbCommandPensionEmployerExpensePerEmployee = oGL.AddCommand();
                            db.ExecuteNonQuery(dbCommandPensionEmployerExpensePerEmployee, transaction);
                        }
                        #endregion

                        #region Add NHF To PayRollProcess,Calculate NHF Amount And Add to TOTAL NHF
                        decimal decNHTFAmount = 0;
                        oEmployee.TransNo = oRow["TransNo"].ToString();
                        if (oEmployee.DoNotChargeNHF)
                        { }
                        else
                        {
                            if (!oAllowDeduct.ChkNHTFExist())
                            {
                                oEmployee.TransNo = oRow["TransNo"].ToString();
                                if (!oEmployee.DoNotChargeNHF)
                                {
                                    decNHTFAmount = (decActualStaffBasicSalaryAmount * decimal.Parse(oHRParam.NHF.ToString())) / decimal.Parse("100");
                                    decNHTFAmount = Math.Round(decNHTFAmount, 2);
                                }
                                else
                                {
                                    decNHTFAmount = 0;
                                }
                            }
                            else
                            {
                                decNHTFAmount = oSalaryStruct.GetNHTFByEmployee();
                                decNHTFAmount = Math.Round(decNHTFAmount, 2);
                            }
                            oPayRollProcess.AllDed = 99997;
                            oPayRollProcess.AllDedType = "D";
                            oPayRollProcess.ItemType = "NHTF";
                            oPayRollProcess.Amount = decNHTFAmount;
                            oPayRollProcess.Employee = oRow["TransNo"].ToString();
                            oPayRollProcess.PayRollDate = datPayRollDateReturn;
                            oPayRollProcess.SysRef = "PAY-" + datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oPayRollProcess.SaveType = "ADDS";
                            if (decNHTFAmount > 0)
                            {
                                decTotalNHTFAmount = decTotalNHTFAmount + decNHTFAmount;
                                SqlCommand dbCommandNHTF = oPayRollProcess.AddCommand();
                                db.ExecuteNonQuery(dbCommandNHTF, transaction);
                            }

                            if ((strPostToIndiviualAccount.Trim() == "YES") && decNHTFAmount > 0)
                            {
                                oGL.EffectiveDate = datPayRollDateReturn;
                                oGL.AccountID = "";
                                oGL.MasterID = oHRParam.NHFAccount;
                                oGL.AcctRef = "";
                                oGL.Credit = decNHTFAmount;
                                oGL.Debit = 0;
                                oGL.Debcred = "C";
                                oGL.Desciption = GeneralFunc.GetMonthName(datPayRollDateReturn.Month) + " " + datPayRollDateReturn.Year.ToString() +  " NHF For " + strEmployeeFullName;
                                oGL.TransType = "PAYROLL";
                                oGL.SysRef = "PAY-" + datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                                oGL.Ref01 = datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                                oGL.Reverse = "N";
                                oGL.Jnumber = strJnumberNext;
                                oGL.Branch = oBranch.DefaultBranch;
                                oGL.FeeType = "EMPNHFPAYSING";
                                oGL.Ref02 = ""; oGL.Chqno = ""; oGL.RecAcctMaster = oGL.MasterID; oGL.RecAcct = ""; oGL.AcctRefSecond = ""; oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                                SqlCommand dbCommandStaffLoanSalaryPayable = oGL.AddCommand();
                                db.ExecuteNonQuery(dbCommandStaffLoanSalaryPayable, transaction);
                            }
                        }
                        #endregion

                        #region Add PAYE To PayRollProcess,Calculate TAX amount And Add to TOTAL TAX
                        decimal decStaffTaxAmount = 0;
                        if (!oAllowDeduct.ChkPAYEExist())
                        {
                            decimal decEarnedIncome = 0;
                            decimal decTotalDeductionForTax = 0;
                            decimal decCalculatedConRelief = 0;
                            if (strComputeTaxableAllowanceOnly.Trim() == "YES")
                            {
                                decimal yearbasic = decActualStaffBasicSalaryAmount * 12;
                                decimal taxyear = decStaffTaxableItemAmount * 12;
                                decEarnedIncome = (decActualStaffBasicSalaryAmount * 12) + (decStaffTaxableItemAmount * 12);
                            }
                            else
                            {
                                decEarnedIncome = (decActualStaffBasicSalaryAmount * 12) + (decStaffAllowanceItemAmount * 12);
                            }
                            decEarnedIncome = Math.Round(decEarnedIncome, 2);

                            decTotalDeductionForTax = (decActualPensionAmount * 12) + (decNHTFAmount * 12) + (decStaffDeductionItemAmount * 12);
                            decTotalDeductionForTax = Math.Round(decTotalDeductionForTax, 2);

                            decimal decConFixedPercentage = (decimal.Parse(oHRParam.ConReliefFixedPercentage.ToString()) * decEarnedIncome) / decimal.Parse("100");
                            decConFixedPercentage = Math.Round(decConFixedPercentage, 2);

                            decimal decConVariablePerentage = (decimal.Parse(oHRParam.ConReliefVariablePercentage.ToString()) * decEarnedIncome) / decimal.Parse("100");
                            decConVariablePerentage = Math.Round(decConVariablePerentage, 2);

                            if (decConVariablePerentage > oHRParam.ConReliefVariableAmount)
                            {
                                decCalculatedConRelief = decConVariablePerentage;
                            }
                            else
                            {
                                decCalculatedConRelief = oHRParam.ConReliefVariableAmount;
                            }
                            decimal decNetAmountToDeductTax = decEarnedIncome - (decConFixedPercentage + decCalculatedConRelief + decTotalDeductionForTax);
                            decNetAmountToDeductTax = Math.Round(decNetAmountToDeductTax, 2);

                            decStaffTaxAmount = ComputeTax(decNetAmountToDeductTax);
                            decStaffTaxAmount = Math.Round(decStaffTaxAmount, 2);
                        }
                        else
                        {
                            decStaffTaxAmount = oSalaryStruct.GetPAYEByEmployee();
                            decStaffTaxAmount = Math.Round(decStaffTaxAmount, 2);
                        }

                        oPayRollProcess.AllDed = 99991;
                        oPayRollProcess.AllDedType = "D";
                        oPayRollProcess.ItemType = "PAYE";
                        oPayRollProcess.Amount = decStaffTaxAmount;
                        oPayRollProcess.Employee = oRow["TransNo"].ToString();
                        oPayRollProcess.PayRollDate = datPayRollDateReturn;
                        oPayRollProcess.SysRef = "PAY-" + datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                        oPayRollProcess.SaveType = "ADDS";
                        if (decStaffTaxAmount > 0)
                        {
                            decTotalStaffTaxAmount = decTotalStaffTaxAmount + decStaffTaxAmount;

                            SqlCommand dbCommandPaye = oPayRollProcess.AddCommand();
                            db.ExecuteNonQuery(dbCommandPaye, transaction);
                        }

                        if ((strPostToIndiviualAccount.Trim() == "YES") && decStaffTaxAmount > 0)
                        {
                            oGL.EffectiveDate = datPayRollDateReturn;
                            oGL.AccountID = "";
                            oGL.MasterID = oHRParam.PAYEAC;
                            oGL.AcctRef = "";
                            oGL.Credit = decStaffTaxAmount;
                            oGL.Debit = 0;
                            oGL.Debcred = "C";
                            oGL.Desciption = GeneralFunc.GetMonthName(datPayRollDateReturn.Month) + " " + datPayRollDateReturn.Year.ToString() +  " Tax For " + strEmployeeFullName;
                            oGL.TransType = "PAYROLL";
                            oGL.SysRef = "PAY-" + datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oGL.Ref01 = datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oGL.Reverse = "N";
                            oGL.Jnumber = strJnumberNext;
                            oGL.Branch = oBranch.DefaultBranch;
                            oGL.FeeType = "EMPTAXPAYSING";
                            oGL.Ref02 = ""; oGL.Chqno = ""; oGL.RecAcctMaster = oGL.MasterID; oGL.RecAcct = ""; oGL.AcctRefSecond = ""; oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                            SqlCommand dbCommandStaffLoanSalaryPayable = oGL.AddCommand();
                            db.ExecuteNonQuery(dbCommandStaffLoanSalaryPayable, transaction);
                        }

                        #endregion

                        #region Add STAFF LOAN To PayRollProcess,Calculate Loan Amount And Post To GL Salary Payable and Staff Account
                        decimal decStaffLoanAmount = 0;
                        AcctGL oAcctGLStaffPayable = new AcctGL();
                        AcctGL oAcctGLForLoan = new AcctGL();
                        StaffLoan oStaffLoan = new StaffLoan();
                        Product oProductLoan = new Product();
                        string strLoanProductAccount;

                        if (!oAllowDeduct.ChkAdvanceExist())
                        {
                            foreach (DataRow oRowAllLoan in oStaffLoan.GetAllLoanByStaffAndDate(oRow["TransNo"].ToString(), datPayRollDateReturn).Tables[0].Rows)
                            {
                                if ((Math.Round(oAcctGLForLoan.GetStaffLoanBalance(long.Parse(oRowAllLoan["TransNo"].ToString()), decimal.Parse(oRowAllLoan["TotalAmount"].ToString()),datPayRollDate), 2) - decimal.Parse(oRowAllLoan["MonthlyDeductionAmount"].ToString())) >= 0)
                                {
                                    oProductLoan.TransNo = oRowAllLoan["ProductId"].ToString();
                                    strLoanProductAccount = oProductLoan.GetProductGLAcct();

                                    oAcctGLStaffPayable.EffectiveDate = datPayRollDateReturn;
                                    oAcctGLStaffPayable.AccountID = oRowAllLoan["CustomerId"].ToString();
                                    oAcctGLStaffPayable.MasterID = strLoanProductAccount;
                                    oAcctGLStaffPayable.AcctRef = oRowAllLoan["ProductId"].ToString(); 
                                    oAcctGLStaffPayable.Credit = Math.Round(decimal.Parse(oRowAllLoan["MonthlyDeductionAmount"].ToString()),2);
                                    oAcctGLStaffPayable.Debit = 0;
                                    oAcctGLStaffPayable.Debcred = "C";
                                    oAcctGLStaffPayable.Desciption = GeneralFunc.GetMonthName(datPayRollDateReturn.Month) + " " + datPayRollDateReturn.Year.ToString() + " Loan Repayment for " + strEmployeeFullName;
                                    oAcctGLStaffPayable.TransType = "PAYROLL";
                                    oAcctGLStaffPayable.SysRef = "PAY-" + datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                                    oAcctGLStaffPayable.Ref01 = oRowAllLoan["TransNo"].ToString();
                                    oAcctGLStaffPayable.Reverse = "N";
                                    oAcctGLStaffPayable.Jnumber = strJnumberNext;
                                    oAcctGLStaffPayable.Branch = oBranch.DefaultBranch;
                                    oAcctGLStaffPayable.FeeType = "EMPLOANSTAFF";
                                    oAcctGLStaffPayable.Ref02 = ""; oAcctGLStaffPayable.Chqno = ""; oAcctGLStaffPayable.RecAcctMaster = strLoanProductAccount; oAcctGLStaffPayable.RecAcct = oRowAllLoan["CustomerId"].ToString(); oAcctGLStaffPayable.AcctRefSecond = oRowAllLoan["ProductId"].ToString(); oAcctGLStaffPayable.InstrumentType = DataGeneral.GLInstrumentType.C;
                                    SqlCommand dbCommandStaffLoan = oAcctGLStaffPayable.AddCommand();
                                    db.ExecuteNonQuery(dbCommandStaffLoan, transaction);

                                    decStaffLoanAmount = decStaffLoanAmount + decimal.Parse(oRowAllLoan["MonthlyDeductionAmount"].ToString());
                                    decStaffLoanAmount = Math.Round(decStaffLoanAmount, 2);

                                    decTotalStaffLoanAmount = decTotalStaffLoanAmount + decimal.Parse(oRowAllLoan["MonthlyDeductionAmount"].ToString());
                                    decTotalStaffLoanAmount = Math.Round(decTotalStaffLoanAmount, 2);
                                }
                            }
                        }
                        else
                        {
                            decStaffLoanAmount = oSalaryStruct.GetAdvanceByEmployee();
                            decStaffLoanAmount = Math.Round(decStaffLoanAmount, 2);

                            if (decStaffLoanAmount > 0)
                            {
                                oProduct.TransNo = strEmployeeProductId;
                                oAcctGLStaffPayable.EffectiveDate = datPayRollDateReturn;
                                oAcctGLStaffPayable.AccountID = strEmployeeCustomerId;
                                oAcctGLStaffPayable.MasterID = oProduct.GetProductGLAcct();
                                oAcctGLStaffPayable.AcctRef = strEmployeeProductId;
                                oAcctGLStaffPayable.Credit = decStaffLoanAmount;
                                oAcctGLStaffPayable.Debit = 0;
                                oAcctGLStaffPayable.Debcred = "C";
                                oAcctGLStaffPayable.Desciption = GeneralFunc.GetMonthName(datPayRollDateReturn.Month) + " " + datPayRollDateReturn.Year.ToString() + " Loan Repayment For " + strEmployeeFullName;
                                oAcctGLStaffPayable.TransType = "PAYROLL";
                                oAcctGLStaffPayable.SysRef = "PAY-" + datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                                oAcctGLStaffPayable.Ref01 = datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                                oAcctGLStaffPayable.Reverse = "N";
                                oAcctGLStaffPayable.Jnumber = strJnumberNext;
                                oAcctGLStaffPayable.Branch = oBranch.DefaultBranch;
                                oAcctGLStaffPayable.FeeType = "EMPLOANSTAFFSING";
                                oAcctGLStaffPayable.Ref02 = ""; oAcctGLStaffPayable.Chqno = ""; oAcctGLStaffPayable.RecAcctMaster = oAcctGLStaffPayable.MasterID; oAcctGLStaffPayable.RecAcct = strEmployeeCustomerId; oAcctGLStaffPayable.AcctRefSecond = strEmployeeProductId; oAcctGLStaffPayable.InstrumentType = DataGeneral.GLInstrumentType.C;
                                SqlCommand dbCommandStaffLoan = oAcctGLStaffPayable.AddCommand();
                                db.ExecuteNonQuery(dbCommandStaffLoan, transaction);
                            }
                        }
                        oPayRollProcess.AllDed = 99995;
                        oPayRollProcess.AllDedType = "D";
                        oPayRollProcess.ItemType = "SLOAN";
                        oPayRollProcess.Amount = decStaffLoanAmount;
                        oPayRollProcess.Employee = oRow["TransNo"].ToString();
                        oPayRollProcess.PayRollDate = datPayRollDateReturn;
                        oPayRollProcess.SysRef = "PAY-" + datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                        oPayRollProcess.SaveType = "ADDS";
                        if (decStaffLoanAmount > 0)
                        {
                            SqlCommand dbCommandEmployeeStaffLoan = oPayRollProcess.AddCommand();
                            db.ExecuteNonQuery(dbCommandEmployeeStaffLoan, transaction);
                        }

                        //if ((strPostToIndiviualAccount.Trim() == "YES") && decStaffLoanAmount > 0)
                        //{
                        //    oGL.EffectiveDate = datPayRollDateReturn;
                        //    oGL.AccountID = "";
                        //    oGL.MasterID = oHRParam.StaffLoanAC;
                        //    oGL.AcctRef = "";
                        //    oGL.Credit = 0;
                        //    oGL.Debit = decStaffLoanAmount;
                        //    oGL.Debcred = "D";
                        //    oGL.Desciption = GeneralFunc.GetMonthName(datPayRollDateReturn.Month) + " " + datPayRollDateReturn.Year.ToString() + " Loan Repayment For " + strEmployeeFullName;
                        //    oGL.TransType = "PAYROLL";
                        //    oGL.SysRef = "PAY-" + datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                        //    oGL.Ref01 = datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                        //    oGL.Reverse = "N";
                        //    oGL.Jnumber = strJnumberNext;
                        //    oGL.Branch = oBranch.DefaultBranch;
                        //    oGL.FeeType = "EMPLOANPAYSING";
                        //    oGL.Ref02 = ""; oGL.Chqno = ""; oGL.RecAcctMaster = oGL.MasterID; oGL.RecAcct = ""; oGL.AcctRefSecond = ""; oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                        //    SqlCommand dbCommandStaffLoanSalaryPayable = oGL.AddCommand();
                        //    db.ExecuteNonQuery(dbCommandStaffLoanSalaryPayable, transaction);
                        //}
                        #endregion

                        #region Post To Employee Account Net Salary
                        if ((strPostToIndiviualAccount.Trim() == "YES") && 
                            ((((decActualStaffBasicSalaryAmount + decStaffAllowanceItemAmount) - decStaffDeductionItemAmount) - (decStaffTaxAmount + decActualPensionAmount + decNHTFAmount + decStaffLoanAmount)) > 0))
                        {
                            
                            oGL.EffectiveDate = datPayRollDateReturn;
                            oProduct.TransNo = strEmployeeProductId;
                            oGL.AccountID = strEmployeeCustomerId;
                            oGL.MasterID = oProduct.GetProductGLAcct();
                            oGL.AcctRef = strEmployeeProductId;
                            oGL.Credit = (((decActualStaffBasicSalaryAmount + decStaffAllowanceItemAmount) - decStaffDeductionItemAmount) - (decStaffTaxAmount + decActualPensionAmount + decNHTFAmount + decStaffLoanAmount));
                            oGL.Debit = 0;
                            oGL.Debcred = "C";
                            oGL.Desciption = GeneralFunc.GetMonthName(datPayRollDateReturn.Month) + " " + datPayRollDateReturn.Year.ToString() + " Net Salary For " + oEmployee.GetEmployeeName(oRow["TransNo"].ToString()).Trim();
                            oGL.TransType = "PAYROLL";
                            oGL.SysRef = "PAY-" + datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oGL.Ref01 = datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oGL.Reverse = "N";
                            oGL.Jnumber = strJnumberNext;
                            oGL.Branch = oBranch.DefaultBranch;
                            oGL.FeeType = "EMPSTAFFPAYSING";
                            oGL.Ref02 = ""; oGL.Chqno = ""; oGL.RecAcctMaster = oGL.MasterID; oGL.RecAcct = oGL.AccountID; oGL.AcctRefSecond = oGL.AcctRef; oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                            SqlCommand dbCommandStaffLoanSalaryPayable = oGL.AddCommand();
                            db.ExecuteNonQuery(dbCommandStaffLoanSalaryPayable, transaction);
                        }
                        #endregion
                    }

                    #region Post GL Table For Summation Amount To Account

                    if (strPostToIndiviualAccount.Trim() != "YES")
                    {
                        //if (decTotalStaffLoanAmount > 0)
                        //{
                        //    oGL.EffectiveDate = datPayRollDateReturn;
                        //    oGL.AccountID = "";
                        //    oGL.MasterID = oHRParam.SalPayableAC;
                        //    oGL.AcctRef = "";
                        //    oGL.Credit = 0;
                        //    oGL.Debit = decTotalStaffLoanAmount;
                        //    oGL.Debcred = "D";
                        //    oGL.Desciption = "Total Loan Deduction of " + GeneralFunc.GetMonthName(datPayRollDateReturn.Month) + " " + datPayRollDateReturn.Year.ToString();
                        //    oGL.TransType = "PAYROLL";
                        //    oGL.SysRef = "PAY-" +  datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                        //    oGL.Ref01 = datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                        //    oGL.Reverse = "N";
                        //    oGL.Jnumber = strJnumberNext;
                        //    oGL.Branch = oBranch.DefaultBranch;
                        //    oGL.FeeType = "EMPLOANPAY";
                        //    oGL.Ref02 = ""; oGL.Chqno = ""; oGL.RecAcctMaster = oGL.MasterID; oGL.RecAcct = ""; oGL.AcctRefSecond = ""; oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                        //    SqlCommand dbCommandStaffLoanSalaryPayable = oGL.AddCommand();
                        //    db.ExecuteNonQuery(dbCommandStaffLoanSalaryPayable, transaction);
                        //}

                        if (decTotalStaffGrossAmount > 0)
                        {
                            oGL.EffectiveDate = datPayRollDateReturn;
                            oGL.AccountID = "";
                            oGL.MasterID = oHRParam.SalExpenseAC;
                            oGL.AcctRef = "";
                            oGL.Credit = 0;
                            oGL.Debit = decTotalStaffGrossAmount;
                            oGL.Debcred = "D";
                            oGL.Desciption = "Total Gross Salary For Month Of " + GeneralFunc.GetMonthName(datPayRollDateReturn.Month);
                            oGL.TransType = "PAYROLL";
                            oGL.SysRef = "PAY-" + datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oGL.Ref01 = datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oGL.Reverse = "N";
                            oGL.Jnumber = strJnumberNext;
                            oGL.FeeType = "EMPGROSS";
                            oGL.Branch = oBranch.DefaultBranch;
                            oGL.Ref02 = ""; oGL.Chqno = ""; oGL.RecAcctMaster = oGL.MasterID; oGL.RecAcct = ""; oGL.AcctRefSecond = ""; oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                            SqlCommand dbCommandEmployeeGross = oGL.AddCommand();
                            db.ExecuteNonQuery(dbCommandEmployeeGross, transaction);
                        }

                        if (decTotalStaffTaxAmount > 0)
                        {
                            oGL.EffectiveDate = datPayRollDateReturn;
                            oGL.AccountID = "";
                            oGL.MasterID = oHRParam.PAYEAC;
                            oGL.AcctRef = "";
                            oGL.Credit = decTotalStaffTaxAmount;
                            oGL.Debit = 0;
                            oGL.Debcred = "C";
                            oGL.Desciption = "Total PAYE For The Month of " + GeneralFunc.GetMonthName(datPayRollDateReturn.Month);
                            oGL.TransType = "PAYROLL";
                            oGL.SysRef = "PAY-" + datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oGL.Ref01 = datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oGL.Reverse = "N";
                            oGL.Jnumber = strJnumberNext;
                            oGL.Branch = oBranch.DefaultBranch;
                            oGL.FeeType = "PAYE";
                            oGL.Ref02 = ""; oGL.Chqno = ""; oGL.RecAcctMaster = oGL.MasterID; oGL.RecAcct = ""; oGL.AcctRefSecond = ""; oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                            SqlCommand dbCommandPAYEAC = oGL.AddCommand();
                            db.ExecuteNonQuery(dbCommandPAYEAC, transaction);
                        }


                        if (decTotalStaffPensionAmountStaff > 0)
                        {
                            oGL.EffectiveDate = datPayRollDateReturn;
                            oGL.AccountID = "";
                            oGL.MasterID = oHRParam.PensionAC;
                            oGL.AcctRef = "";
                            oGL.Credit = decTotalStaffPensionAmountStaff;
                            oGL.Debit = 0;
                            oGL.Debcred = "C";
                            oGL.Desciption = "Total Employee Contr. For The Month of " + GeneralFunc.GetMonthName(datPayRollDateReturn.Month);
                            oGL.TransType = "PAYROLL";
                            oGL.SysRef = "PAY-" + datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oGL.Ref01 = datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oGL.Reverse = "N";
                            oGL.Jnumber = strJnumberNext;
                            oGL.Branch = oBranch.DefaultBranch;
                            oGL.FeeType = "EMPPEN";
                            oGL.Ref02 = ""; oGL.Chqno = ""; oGL.RecAcctMaster = oGL.MasterID; oGL.RecAcct = ""; oGL.AcctRefSecond = ""; oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                            SqlCommand dbCommandEmployeePensionAC = oGL.AddCommand();
                            db.ExecuteNonQuery(dbCommandEmployeePensionAC, transaction);
                        }

                        if (decTotalStaffPensionAmountEmployer > 0)
                        {
                            oGL.EffectiveDate = datPayRollDateReturn;
                            oGL.AccountID = "";
                            oGL.MasterID = oHRParam.PensionEmployerAC;
                            oGL.AcctRef = "";
                            oGL.Credit = decTotalStaffPensionAmountEmployer;
                            oGL.Debit = 0;
                            oGL.Debcred = "C";
                            oGL.Desciption = "Total Employer Contr For The Month of " + GeneralFunc.GetMonthName(datPayRollDateReturn.Month);
                            oGL.TransType = "PAYROLL";
                            oGL.SysRef = "PAY-" + datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oGL.Ref01 = datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oGL.Reverse = "N";
                            oGL.Jnumber = strJnumberNext;
                            oGL.Branch = oBranch.DefaultBranch;
                            oGL.FeeType = "EMPERPENPEN";
                            oGL.Ref02 = ""; oGL.Chqno = ""; oGL.RecAcctMaster = oGL.MasterID; oGL.RecAcct = ""; oGL.AcctRefSecond = ""; oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                            SqlCommand dbCommandEmployerPensionAC = oGL.AddCommand();
                            db.ExecuteNonQuery(dbCommandEmployerPensionAC, transaction);

                            oGL.EffectiveDate = datPayRollDateReturn;
                            oGL.AccountID = "";
                            oGL.MasterID = oHRParam.PensionEmployerExpAC;
                            oGL.AcctRef = "";
                            oGL.Credit = 0;
                            oGL.Debit = decTotalStaffPensionAmountEmployer;
                            oGL.Debcred = "D";
                            oGL.Desciption = "Total Employer Contr For The Month Of " + GeneralFunc.GetMonthName(datPayRollDateReturn.Month);
                            oGL.TransType = "PAYROLL";
                            oGL.SysRef = "PAY-" + datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oGL.Ref01 = datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oGL.Reverse = "N";
                            oGL.Jnumber = strJnumberNext;
                            oGL.Branch = oBranch.DefaultBranch;
                            oGL.FeeType = "EMPERPENEXP";
                            oGL.Ref02 = ""; oGL.Chqno = ""; oGL.RecAcctMaster = oGL.MasterID; oGL.RecAcct = ""; oGL.AcctRefSecond = ""; oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                            SqlCommand dbCommandEmployerPenssionCompany = oGL.AddCommand();
                            db.ExecuteNonQuery(dbCommandEmployerPenssionCompany, transaction);
                        }

                        if (decTotalNHTFAmount > 0)
                        {
                            oGL.EffectiveDate = datPayRollDateReturn;
                            oGL.AccountID = "";
                            oGL.MasterID = oHRParam.NHFAccount;
                            oGL.AcctRef = "";
                            oGL.Credit = decTotalNHTFAmount;
                            oGL.Debit = 0;
                            oGL.Debcred = "C";
                            oGL.Desciption = "Total NHF For The Month of " + GeneralFunc.GetMonthName(datPayRollDateReturn.Month);
                            oGL.TransType = "PAYROLL";
                            oGL.SysRef = "PAY-" + datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oGL.Ref01 = datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oGL.Reverse = "N";
                            oGL.Jnumber = strJnumberNext;
                            oGL.Branch = oBranch.DefaultBranch;
                            oGL.FeeType = "EMPNHF";
                            oGL.Ref02 = ""; oGL.Chqno = ""; oGL.RecAcctMaster = oGL.MasterID; oGL.RecAcct = ""; oGL.AcctRefSecond = ""; oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                            SqlCommand dbCommandNHFAC = oGL.AddCommand();
                            db.ExecuteNonQuery(dbCommandNHFAC, transaction);
                        }

                        if ((decTotalStaffGrossAmount - (decTotalStaffTaxAmount + decTotalStaffPensionAmountStaff + decTotalNHTFAmount + decTotalStaffLoanAmount)) > 0)
                        {
                            oGL.EffectiveDate = datPayRollDateReturn;
                            oGL.AccountID = "";
                            oGL.MasterID = oHRParam.SalPayableAC;
                            oGL.AcctRef = "";
                            oGL.Debit = 0;
                            oGL.Credit = decTotalStaffGrossAmount - (decTotalStaffTaxAmount + decTotalStaffPensionAmountStaff + decTotalNHTFAmount);
                            oGL.Debcred = "C";
                            oGL.Desciption = "Net Salary Payable For Month Of " + GeneralFunc.GetMonthName(datPayRollDateReturn.Month);
                            oGL.TransType = "PAYROLL";
                            oGL.SysRef = "PAY-" + datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oGL.Ref01 = datPayRollDateReturn.Day.ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Month.ToString().ToString().PadLeft(2, char.Parse("0")) + datPayRollDateReturn.Year.ToString();
                            oGL.Reverse = "N";
                            oGL.Jnumber = strJnumberNext;
                            oGL.FeeType = "EMPNETSAL";
                            oGL.Branch = oBranch.DefaultBranch;
                            oGL.Ref02 = ""; oGL.Chqno = ""; oGL.RecAcctMaster = oGL.MasterID; oGL.RecAcct = ""; oGL.AcctRefSecond = ""; oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                            SqlCommand dbCommandEmployeeGross = oGL.AddCommand();
                            db.ExecuteNonQuery(dbCommandEmployeeGross, transaction);
                        }
                    }
                    #endregion

                    #region Update HR Parameter Table With Next Date
                    if (!blnIsReposted)
                    {
                        DateTime datNewNextDate;
                        if (strAddMonthLastDay.Trim() == "YES")
                        {
                            string strNextDateJoin = GeneralFunc.GetMonthLastDay(DateTime.ParseExact(datPayRollDateReturn.AddMonths(1).ToString().Substring(0, 10), "dd/MM/yyyy", format).Month, DateTime.ParseExact(datPayRollDateReturn.AddMonths(1).ToString().Substring(0, 10), "dd/MM/yyyy", format).Year) + "/" +
                            DateTime.ParseExact(datPayRollDateReturn.AddMonths(1).ToString().Substring(0, 10), "dd/MM/yyyy", format).Month.ToString().PadLeft(2, char.Parse("0")) + "/" +
                            DateTime.ParseExact(datPayRollDateReturn.AddMonths(1).ToString().Substring(0, 10), "dd/MM/yyyy", format).Year.ToString();
                            datNewNextDate = DateTime.ParseExact(strNextDateJoin.Substring(0, 10), "dd/MM/yyyy", format);
                        }
                        else
                        {
                            datNewNextDate = datPayRollDateReturn.AddMonths(1);
                        }
                        SqlCommand dbCommandHRParamUpdatePayRollDate = oHRParam.UpdatePayRollDate(datNewNextDate);
                        db.ExecuteNonQuery(dbCommandHRParamUpdatePayRollDate, transaction);
                    }
                    #endregion

                    #region Add Or Edit Record To PayRollRunHistory Table
                    if (ChkPayRollDateExist())
                    {
                        SqlCommand dbCommandPayRollRunHistoryUpdatePost = UpDatePostCommand();
                        db.ExecuteNonQuery(dbCommandPayRollRunHistoryUpdatePost, transaction);
                    }
                    else
                    {
                        SqlCommand dbCommandPayRollRunHistory = Save();
                        db.ExecuteNonQuery(dbCommandPayRollRunHistory, transaction);
                    }

                    #endregion

                    transaction.Commit();
                    enSaveStatus = DataGeneral.SaveStatus.Saved;
                    return enSaveStatus;
                }
                catch (Exception err)
                {
                    transaction.Rollback();
                    throw err;
                }
            }
        }
        #endregion

        #region Compute Tax
        public decimal ComputeTax(decimal decTaxedAmount)
        {
            decimal decTaxAmountLeft = 0;
            decimal decCalculatedResult = 0;
            decimal decTaxResult = 0;
            int intRecordNumber = 0;
            int intNumberDone = 0;

            HRParam oHRParam = new HRParam();
            TaxTable oTaxTable = new TaxTable();
            DataSet oDsTax = oTaxTable.GetAll();
            if (oHRParam.GetHRGentable())
            {
                if (oHRParam.TaxRateType.Trim() == "T")
                {
                    if (oDsTax.Tables[0].Rows.Count <= 0)
                    {
                        throw new Exception("Cannot Calculate Tax, Tax Table Table Is Empty");
                    }

                    intRecordNumber = oTaxTable.GetTaxTableMaxTransNo();
                    foreach (DataRow oTaxRow in oDsTax.Tables[0].Rows)
                    {
                        intNumberDone = intNumberDone + 1;
                        if ((decTaxedAmount - decTaxAmountLeft) > (decimal.Parse(oTaxRow["AdditionalAmount"].ToString()) - decimal.Parse(oTaxRow["Amount"].ToString())))
                        {
                            if (intNumberDone == intRecordNumber)
                            {
                                decCalculatedResult = (((decTaxedAmount - decTaxAmountLeft) * (decimal.Parse(oTaxRow["Rate"].ToString()))) / 100);
                                decTaxResult = decTaxResult + decCalculatedResult;
                                break;
                            }
                            else
                            {
                                decCalculatedResult = (((decimal.Parse(oTaxRow["AdditionalAmount"].ToString()) - decimal.Parse(oTaxRow["Amount"].ToString())) * (decimal.Parse(oTaxRow["Rate"].ToString()))) / 100);
                                decTaxResult = decTaxResult + decCalculatedResult;
                                decTaxAmountLeft = decTaxAmountLeft + (decimal.Parse(oTaxRow["AdditionalAmount"].ToString()) - decimal.Parse(oTaxRow["Amount"].ToString()));
                            }

                        }
                        else 
                        {
                            decCalculatedResult = (((decTaxedAmount - decTaxAmountLeft) * (decimal.Parse(oTaxRow["Rate"].ToString()))) / 100);
                            decTaxResult = decTaxResult + decCalculatedResult;
                            break;
                        }
                    }
                }
                else
                {
                    decTaxResult = (decTaxedAmount * decimal.Parse(oHRParam.TaxFixedRate.ToString())) / 100;
                }
            }
            decTaxResult = decTaxResult / 12;
            return decTaxResult;
        }
        #endregion

        #region Get Last PayRoll Date
        public DateTime GetLastPayRollDate()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PayRollRunHistorySelectLastDatePosted") as SqlCommand;
            var datReturnDate = db.ExecuteScalar(dbCommand);
            return datReturnDate != null && datReturnDate.ToString().Trim() != "" ? DateTime.ParseExact(datReturnDate.ToString().Substring(0, 10), "dd/MM/yyyy", format) : DateTime.MinValue;
        }
        #endregion
    }
}
