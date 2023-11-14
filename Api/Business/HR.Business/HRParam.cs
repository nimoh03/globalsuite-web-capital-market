using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using BaseUtility.Business;

namespace HR.Business
{
    public class HRParam
    {
        #region Declarations
        private float fltEmployeePension, fltEmployerPension, fltTaxFixedRate, fltNHF, fltConReliefVariablePercentage,fltConReliefFixedPercentage, fltMinimumTaxPercentage;
        private string strPAYEAC, strPensionAC, strPensionEmployerAC,strNHFAccount;
        private string strSalExpenseAC, strSalPayableAC, strPensionEmployerExpAC,strStaffLoanProduct, strStaffLoanAC;
        private string strTaxRateType;
        private DateTime datPayRollDate;
        private decimal decConReliefVariableAmount;
        
        #endregion

        #region Properties
        public float EmployeePension
        {
            set { fltEmployeePension = value; }
            get { return fltEmployeePension; }
        }
        public float EmployerPension
        {
            set { fltEmployerPension = value; }
            get { return fltEmployerPension; }
        }
        public float TaxFixedRate
        {
            set { fltTaxFixedRate = value; }
            get { return fltTaxFixedRate; }
        }
        public float NHF
        {
            set { fltNHF = value; }
            get { return fltNHF; }
        }
        public float ConReliefVariablePercentage
        {
            set { fltConReliefVariablePercentage = value; }
            get { return fltConReliefVariablePercentage; }
        }
        public float MinimumTaxPercentage
        {
            set { fltMinimumTaxPercentage = value; }
            get { return fltMinimumTaxPercentage; }
        }
        public string PAYEAC
        {
            set { strPAYEAC = value; }
            get { return strPAYEAC; }
        }
        public string PensionAC
        {
            set { strPensionAC = value; }
            get { return strPensionAC; }
        }
        public string PensionEmployerAC
        {
            set { strPensionEmployerAC = value; }
            get { return strPensionEmployerAC; }
        }
        public string NHFAccount
        {
            set { strNHFAccount = value; }
            get { return strNHFAccount; }
        }
        public string SalExpenseAC
        {
            set { strSalExpenseAC = value; }
            get { return strSalExpenseAC; }
        }
        public string SalPayableAC
        {
            set { strSalPayableAC = value; }
            get { return strSalPayableAC; }
        }
       
        public string TaxRateType
        {
            set { strTaxRateType = value; }
            get { return strTaxRateType; }
        }
        public DateTime PayRollDate
        {
            set { datPayRollDate = value; }
            get
            {
                IFormatProvider format = new CultureInfo("en-GB");
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("HRGentableSelectPayRollDate") as SqlCommand;
                var datReturnDate = db.ExecuteScalar(dbCommand);
                return datReturnDate != null && datReturnDate.ToString().Trim() != "" ? DateTime.ParseExact(datReturnDate.ToString().Substring(0, 10), "dd/MM/yyyy", format) : DateTime.MinValue;
            }
        }
        public decimal ConReliefVariableAmount
        {
            set { decConReliefVariableAmount = value; }
            get { return decConReliefVariableAmount; }
        }
        public float ConReliefFixedPercentage
        {
            set { fltConReliefFixedPercentage = value; }
            get { return fltConReliefFixedPercentage; }
        }
        public string PensionEmployerExpAC
        {
            set { strPensionEmployerExpAC = value; }
            get { return strPensionEmployerExpAC; }
        }

        public string StaffLoanAC
        {
            set { strStaffLoanAC = value; }
            get { return strStaffLoanAC; }
        }

        #endregion

        #region Add
        public bool Add()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    SqlCommand oCommand;
                    if (!ChkRecordExist())
                    {
                        oCommand = db.GetStoredProcCommand("HRGentableAdd") as SqlCommand;
                    }
                    else
                    {
                        oCommand = db.GetStoredProcCommand("HRGentableEdit") as SqlCommand;
                    }
                    db.AddInParameter(oCommand, "EmployeePension", SqlDbType.Real, fltEmployeePension);
                    db.AddInParameter(oCommand, "EmployerPension", SqlDbType.Real, fltEmployerPension);
                    db.AddInParameter(oCommand, "PAYEAC", SqlDbType.VarChar, strPAYEAC.Trim());
                    db.AddInParameter(oCommand, "PensionAC", SqlDbType.VarChar, strPensionAC.Trim());
                    db.AddInParameter(oCommand, "PensionEmployerAC", SqlDbType.VarChar, strPensionEmployerAC.Trim());
                    db.AddInParameter(oCommand, "BasicSalAC", SqlDbType.VarChar,strSalExpenseAC.Trim());
                    db.AddInParameter(oCommand, "StaffLoanAC", SqlDbType.VarChar, strStaffLoanAC.Trim());
                    db.AddInParameter(oCommand, "SalControlAC", SqlDbType.VarChar,strSalPayableAC.Trim());
                    db.AddInParameter(oCommand, "TaxFixedRate", SqlDbType.Real, fltTaxFixedRate);
                    db.AddInParameter(oCommand, "TaxRateType", SqlDbType.VarChar, strTaxRateType.Trim());
                    db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    db.AddInParameter(oCommand, "PayRollDate", SqlDbType.DateTime, datPayRollDate != DateTime.MinValue ? datPayRollDate : SqlDateTime.Null);
                    db.AddInParameter(oCommand, "NHF", SqlDbType.Real, fltNHF);
                    db.AddInParameter(oCommand, "ConReliefVariablePercentage", SqlDbType.Real, fltConReliefVariablePercentage);
                    db.AddInParameter(oCommand, "MinimumTaxPercentage", SqlDbType.Real, fltMinimumTaxPercentage);
                    db.AddInParameter(oCommand, "NHFAccount", SqlDbType.VarChar, strNHFAccount);
                    db.AddInParameter(oCommand, "ConReliefVariableAmount", SqlDbType.Money, decConReliefVariableAmount);
                    db.AddInParameter(oCommand, "ConReliefFixedPercentage", SqlDbType.Real, fltConReliefFixedPercentage);
                    db.AddInParameter(oCommand, "StaffLoanProduct", SqlDbType.VarChar, strStaffLoanProduct.Trim());
                    db.AddInParameter(oCommand, "PensionEmployerExpAC", SqlDbType.VarChar, strPensionEmployerExpAC.Trim());
                    db.ExecuteNonQuery(oCommand,transaction);
                    transaction.Commit();
                    blnStatus = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            return blnStatus;
        }
        #endregion

        #region Get HR GenTable
        public bool GetHRGentable()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("HRGentableSelect") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                
                
                if (thisRow[0]["EmployeePension"] != null && thisRow[0]["EmployeePension"].ToString() != "")
                {
                    fltEmployeePension = float.Parse(thisRow[0]["EmployeePension"].ToString());
                }
                else
                {
                    fltEmployeePension = 0;
                }
                if (thisRow[0]["EmployerPension"] != null && thisRow[0]["EmployerPension"].ToString() != "")
                {
                    fltEmployerPension = float.Parse(thisRow[0]["EmployerPension"].ToString());
                }
                else
                {
                    fltEmployerPension = 0;
                }
                
                if (thisRow[0]["PAYEAC"] != null && thisRow[0]["PAYEAC"].ToString() != "")
                {
                    strPAYEAC = thisRow[0]["PAYEAC"].ToString();
                }
                else
                {
                    strPAYEAC = "";
                }
                if (thisRow[0]["PensionAC"] != null && thisRow[0]["PensionAC"].ToString() != "")
                {
                    strPensionAC = thisRow[0]["PensionAC"].ToString();
                }
                else
                {
                    strPensionAC = "";
                }
                if (thisRow[0]["PensionEmployerAC"] != null && thisRow[0]["PensionEmployerAC"].ToString() != "")
                {
                    strPensionEmployerAC = thisRow[0]["PensionEmployerAC"].ToString();
                }
                else
                {
                    strPensionEmployerAC = "";
                }
               
                if (thisRow[0]["BasicSalAC"] != null && thisRow[0]["BasicSalAC"].ToString() != "")
                {
                    strSalExpenseAC = thisRow[0]["BasicSalAC"].ToString();
                }
                else
                {
                    strSalExpenseAC = "";
                }
                if (thisRow[0]["SalControlAC"] != null && thisRow[0]["SalControlAC"].ToString() != "")
                {
                    strSalPayableAC = thisRow[0]["SalControlAC"].ToString();
                }
                else
                {
                    strSalPayableAC = "";
                }

                strStaffLoanAC = thisRow[0]["StaffLoanAC"] != null ? thisRow[0]["StaffLoanAC"].ToString() : "";

                if (thisRow[0]["TaxFixedRate"] != null && thisRow[0]["TaxFixedRate"].ToString() != "")
                {
                    fltTaxFixedRate = float.Parse(thisRow[0]["TaxFixedRate"].ToString());
                }
                else
                {
                    fltTaxFixedRate = 0;
                }
                if (thisRow[0]["TaxRateType"] != null && thisRow[0]["TaxRateType"].ToString() != "")
                {
                    strTaxRateType = thisRow[0]["TaxRateType"].ToString();
                }
                else
                {
                    strTaxRateType = "";
                }
                if (thisRow[0]["PayRollDate"] != null && thisRow[0]["PayRollDate"].ToString() != "")
                {
                    datPayRollDate = DateTime.ParseExact(thisRow[0]["PayRollDate"].ToString().Substring(0,10),"dd/MM/yyyy",format);
                }
                else
                {
                    datPayRollDate = DateTime.MinValue;
                }

                fltNHF = thisRow[0]["NHF"] != null && thisRow[0]["NHF"].ToString() != "" ? float.Parse(thisRow[0]["NHF"].ToString()) : 0;
                fltConReliefVariablePercentage = thisRow[0]["ConReliefVariablePercentage"] != null && thisRow[0]["ConReliefVariablePercentage"].ToString() != "" ? float.Parse(thisRow[0]["ConReliefVariablePercentage"].ToString()) : 0;
                fltMinimumTaxPercentage = thisRow[0]["MinimumTaxPercentage"] != null && thisRow[0]["MinimumTaxPercentage"].ToString() != "" ? float.Parse(thisRow[0]["MinimumTaxPercentage"].ToString()) : 0;
                strNHFAccount = thisRow[0]["NHFAccount"] != null ? thisRow[0]["NHFAccount"].ToString() : "";
                decConReliefVariableAmount = thisRow[0]["ConReliefVariableAmount"] != null && thisRow[0]["ConReliefVariableAmount"].ToString() != "" ? decimal.Parse(thisRow[0]["ConReliefVariableAmount"].ToString()) : 0;
                fltConReliefFixedPercentage = thisRow[0]["ConReliefFixedPercentage"] != null && thisRow[0]["ConReliefFixedPercentage"].ToString() != "" ? float.Parse(thisRow[0]["ConReliefFixedPercentage"].ToString()) : 0;
                strPensionEmployerExpAC = thisRow[0]["PensionEmployerExpAC"] != null ? thisRow[0]["PensionEmployerExpAC"].ToString() : "";
                strStaffLoanProduct = thisRow[0]["StaffLoanProduct"] != null ? thisRow[0]["StaffLoanProduct"].ToString() : "";

                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Check Record Exist
        public bool ChkRecordExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            oCommand = db.GetStoredProcCommand("HRGentableChkRecordExist") as SqlCommand;
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Update PayRoll Date
        public SqlCommand UpdatePayRollDate(DateTime datNewPayRollDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("HRGentableUpdatePayRollDate") as SqlCommand;
            db.AddInParameter(dbCommand, "PayRollDate", SqlDbType.DateTime,datNewPayRollDate);
            return dbCommand;
        }
        #endregion
    }
}
