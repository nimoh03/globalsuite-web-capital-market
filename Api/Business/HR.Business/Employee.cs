using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using System.Configuration;
using Admin.Business;
using BaseUtility.Business;
using GL.Business;

namespace HR.Business
{
    public class Employee
    {
        #region Declarations
        private string strTransNo, strSurname, strFirstName;
        private string strOtherName;
        private string strAddr1, strAddr2;
        private string strReligion, strQualification, strCountry;
        private string strDeficiency, strTelephone,strSalaryRateType;
        private int intState;
        private DateTime datDOB, datLicExpDate;
        private byte[] imgSignature;
        private byte[] imgPhoto;

        private int intOccupation, intEmployeeNoSortByNetAmount;
        private string strOccupLevel;
        private int intPensionMngr;
        private string strPensionNo, strBankName, strBankAC, strHMOType, strBranch;
        private DateTime datResumeDate, datRetireDate;
        private decimal decBasicSalary;
        private List<SalaryStruct> lstSalStruct;
        private bool blnDoNotGenerateSalary;
        private bool blnDoNotChargeNHF;
        private bool blnDoNotChargeNSITF;
        public string ProductId { get; set; }
        public string CustomerId { get; set; }

        private string strUserID;
        private string strSaveType;

        #endregion

        #region Properties
       
        public DateTime DOB
        {
            set { datDOB = value; }
            get { return datDOB; }
        }

        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public int EmployeeNoSortByNetAmount
        {
            set { intEmployeeNoSortByNetAmount = value; }
            get { return intEmployeeNoSortByNetAmount; }
        }
        
        public string Surname
        {
            set { strSurname = value; }
            get { return strSurname; }
        }
        public string FirstName
        {
            set { strFirstName = value; }
            get { return strFirstName; }
        }
        public string OtherName
        {
            set { strOtherName = value; }
            get { return strOtherName; }
        }
        public string Addr1
        {
            set { strAddr1 = value; }
            get { return strAddr1; }
        }
        public string Addr2
        {
            set { strAddr2 = value; }
            get { return strAddr2; }
        }
        public int State
        {
            set { intState = value; }
            get { return intState; }
        }
       
        public string Religion
        {
            set { strReligion = value; }
            get { return strReligion; }
        }
        
        public string Country
        {
            set { strCountry = value; }
            get { return strCountry; }
        }
        public string Qualification
        {
            set { strQualification = value; }
            get { return strQualification; }
        }
        public string Telephone
        {
            set { strTelephone = value; }
            get { return strTelephone; }
        }
        public string Deficiency
        {
            set { strDeficiency = value; }
            get { return strDeficiency; }
        }
        public DateTime LicExpDate
        {
            set { datLicExpDate = value; }
            get { return datLicExpDate; }
        }
        public byte[] Photo
        {
            set { imgPhoto = value; }
            get { return imgPhoto; }
        }
        public byte[] Signature
        {
            set { imgSignature = value; }
            get { return imgSignature; }
        }
        public DateTime ResumeDate
        {
            set { datResumeDate = value; }
            get { return datResumeDate; }
        }
        public DateTime RetireDate
        {
            set { datRetireDate = value; }
            get { return datRetireDate; }
        }
        public int Occupation
        {
            set { intOccupation = value; }
            get { return intOccupation; }
        }
        public string OccupLevel
        {
            set { strOccupLevel = value; }
            get { return strOccupLevel; }
        }

        public int PensionMngr
        {
            set { intPensionMngr = value; }
            get { return intPensionMngr; }
        }
        public string PensionNo
        {
            set { strPensionNo = value; }
            get { return strPensionNo; }
        }

        public string BankName
        {
            set { strBankName = value; }
            get { return strBankName; }
        }
        public string BankAC
        {
            set { strBankAC = value; }
            get { return strBankAC; }
        }
        public string HMOType
        {
            set { strHMOType = value; }
            get { return strHMOType; }
        }
        public string Branch
        {
            set { strBranch = value; }
            get { return strBranch; }
        }
        public string SalaryRateType
        {
            set { strSalaryRateType = value; }
            get { return strSalaryRateType; }
        }
        public decimal BasicSalary
        {
            set { decBasicSalary = value; }
            get { return decBasicSalary; }
        }
        public List<SalaryStruct> SalStruct
        {
            set { lstSalStruct = value; }
            get { return lstSalStruct; }
        }
        public bool DoNotGenerateSalary
        {
            set { blnDoNotGenerateSalary = value; }
            get { return blnDoNotGenerateSalary; }
        }
        

        public bool DoNotChargeNHF
        {
            set { blnDoNotChargeNHF = value; }
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("EmployeeSelectDoNotChargeNHF") as SqlCommand;
                db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
                var varDoNotChargeNHF = db.ExecuteScalar(dbCommand);
                return varDoNotChargeNHF != null && varDoNotChargeNHF.ToString().Trim() != "" ? bool.Parse(varDoNotChargeNHF.ToString()) : false;

            }
        }

        public bool DoNotChargeNSITF
        {
            set { blnDoNotChargeNSITF = value; }
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("EmployeeSelectDoNotChargeNSITF") as SqlCommand;
                db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
                var varDoNotChargeNSITF = db.ExecuteScalar(dbCommand);
                return varDoNotChargeNSITF != null && varDoNotChargeNSITF.ToString().Trim() != "" ? bool.Parse(varDoNotChargeNSITF.ToString()) : false;

            }
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
            IFormatProvider format = new CultureInfo("en-GB");
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            if (!ChkTransNoExist())
            {
                enSaveStatus = DataGeneral.SaveStatus.NotExist;
                return enSaveStatus;
            }
            if (intOccupation != 0 && strOccupLevel.Trim() != "")
            {
                AllDedRate oAllDedRate = new AllDedRate();
                oAllDedRate.Occupation = intOccupation;
                oAllDedRate.OccupLevel = strOccupLevel;
                oAllDedRate.TransNo = "";
                oAllDedRate.SaveType = "ADDS";
                if (!oAllDedRate.ChkNameExist())
                {
                    enSaveStatus = DataGeneral.SaveStatus.ProductNotExist;
                    return enSaveStatus;
                }
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            
            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                Branch oBranch = new Branch();
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    SqlCommand dbCommand = null;
                    if (strSaveType.Trim() == "ADDS")
                    {
                        dbCommand = db.GetStoredProcCommand("EmployeeAdd") as SqlCommand;
                        db.AddOutParameter(dbCommand, "TransNo", SqlDbType.VarChar, 10);
                    }
                    else if (strSaveType.Trim() == "EDIT")
                    {
                        dbCommand = db.GetStoredProcCommand("EmployeeEdit") as SqlCommand;
                        db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
                    }

                    db.AddInParameter(dbCommand, "Surname", SqlDbType.VarChar, strSurname.ToUpper().Trim());
                    db.AddInParameter(dbCommand, "OtherName", SqlDbType.VarChar, strOtherName.ToUpper().Trim());
                    db.AddInParameter(dbCommand, "FirstName", SqlDbType.VarChar, strFirstName.ToUpper().Trim());

                    if (datDOB != DateTime.MinValue)
                    {
                        db.AddInParameter(dbCommand, "DOB", SqlDbType.DateTime, datDOB);

                    }
                    else
                    {
                        db.AddInParameter(dbCommand, "DOB", SqlDbType.DateTime, SqlDateTime.Null);

                    }
                    db.AddInParameter(dbCommand, "Addr1", SqlDbType.VarChar, strAddr1.Trim());
                    db.AddInParameter(dbCommand, "Addr2", SqlDbType.VarChar, strAddr2.Trim());
                    db.AddInParameter(dbCommand, "Telephone", SqlDbType.VarChar, strTelephone.Trim());
                    db.AddInParameter(dbCommand, "State", SqlDbType.Int, intState);
                    db.AddInParameter(dbCommand, "Status", SqlDbType.VarChar, "");
                    db.AddInParameter(dbCommand, "Religion", SqlDbType.VarChar, strReligion.Trim());
                    db.AddInParameter(dbCommand, "Country", SqlDbType.VarChar, strCountry.Trim());
                    db.AddInParameter(dbCommand, "Deficiency", SqlDbType.VarChar, strDeficiency.Trim());
                    
                    db.AddInParameter(dbCommand, "Photo", SqlDbType.Image, imgPhoto);
                    db.AddInParameter(dbCommand, "Signature", SqlDbType.Image, imgSignature);

                    if (datResumeDate != DateTime.MinValue)
                    {
                        db.AddInParameter(dbCommand, "ResumeDate", SqlDbType.DateTime, datResumeDate);

                    }
                    else
                    {
                        db.AddInParameter(dbCommand, "ResumeDate", SqlDbType.DateTime, SqlDateTime.Null);

                    }
                    db.AddInParameter(dbCommand, "RetireDate", SqlDbType.DateTime, SqlDateTime.Null);
                    db.AddInParameter(dbCommand, "Occupation", SqlDbType.Int, intOccupation);
                    db.AddInParameter(dbCommand, "OccupLevel", SqlDbType.VarChar, strOccupLevel.Trim());
                    db.AddInParameter(dbCommand, "PensionMngr", SqlDbType.Int, intPensionMngr);
                    db.AddInParameter(dbCommand, "PensionNo", SqlDbType.VarChar, strPensionNo.Trim());
                    db.AddInParameter(dbCommand, "BankName", SqlDbType.VarChar, strBankName.Trim());
                    db.AddInParameter(dbCommand, "BankAC", SqlDbType.VarChar, strBankAC.Trim());
                    db.AddInParameter(dbCommand, "HMOType", SqlDbType.VarChar, strHMOType.Trim());
                    db.AddInParameter(dbCommand, "SalaryRateType", SqlDbType.VarChar, strSalaryRateType.Trim());
                    db.AddInParameter(dbCommand, "BasicSalary", SqlDbType.Decimal, decBasicSalary);
                    db.AddInParameter(dbCommand, "Branch", SqlDbType.VarChar, oBranch.DefaultBranch);
                    db.AddInParameter(dbCommand, "DoNotGenerateSalary", SqlDbType.Bit, blnDoNotGenerateSalary);
                    db.AddInParameter(dbCommand, "DoNotChargeNHF", SqlDbType.Bit, blnDoNotChargeNHF);
                    db.AddInParameter(dbCommand, "DoNotChargeNSITF", SqlDbType.Bit, blnDoNotChargeNSITF);
                    db.AddInParameter(dbCommand, "ProductId", SqlDbType.VarChar, ProductId);
                    db.AddInParameter(dbCommand, "CustomerId", SqlDbType.VarChar, CustomerId);
                    db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
                    db.AddInParameter(dbCommand, "TableName", SqlDbType.VarChar, "EMPLOYEE");
                    db.ExecuteNonQuery(dbCommand, transaction);
                    if (strSaveType == "ADDS")
                    {
                        EduQualif oEduQualif = new EduQualif();
                        oEduQualif.Employee = db.GetParameterValue(dbCommand, "TransNo").ToString();
                        foreach (DataRow oEduRow in oEduQualif.GetTempAllByEmployee().Tables[0].Rows)
                        {
                            oEduQualif.Course = oEduRow[1].ToString();
                            oEduQualif.EndDate = DateTime.ParseExact(oEduRow[3].ToString(), "dd/MM/yyyy", format);
                            oEduQualif.Qualification = oEduRow[3].ToString();
                            oEduQualif.School = oEduRow[3].ToString();
                            oEduQualif.StartDate = DateTime.ParseExact(oEduRow[3].ToString(), "dd/MM/yyyy", format);
                            SqlCommand oAddEdu = oEduQualif.AddCommand();
                            db.ExecuteNonQuery(oAddEdu, transaction);

                        }
                        SqlCommand oDelEduTemp = oEduQualif.DeleteTempByEmployeeCommand();
                        db.ExecuteNonQuery(oDelEduTemp, transaction);
                        WorkExp oWorkExp = new WorkExp();
                        oWorkExp.Employee = db.GetParameterValue(dbCommand, "TransNo").ToString();
                        foreach (DataRow oworkRow in oWorkExp.GetTempAllByEmployee().Tables[0].Rows)
                        {
                            oWorkExp.Addr = oworkRow[1].ToString();
                            oWorkExp.EndDate = DateTime.ParseExact(oworkRow[3].ToString(), "dd/MM/yyyy", format);
                            oWorkExp.Occupation = oworkRow[3].ToString();
                            oWorkExp.Position = oworkRow[3].ToString();
                            oWorkExp.StartDate = DateTime.ParseExact(oworkRow[3].ToString(),"dd/MM/yyyy",format);
                            SqlCommand oAddWork = oWorkExp.AddCommand();
                            db.ExecuteNonQuery(oAddWork, transaction);

                        }
                        SqlCommand oDelworkTemp = oWorkExp.DeleteTempByEmployeeCommand();
                        db.ExecuteNonQuery(oDelworkTemp, transaction);
                    }

                    if (strSaveType.Trim() == "EDIT")
                    {
                        SalaryStruct oSalaryStructDelete = new SalaryStruct();
                        oSalaryStructDelete.Employee = strTransNo; 
                        SqlCommand dbCommandSalaryStructDelete = oSalaryStructDelete.DeleteByEmployeeReturnCommand();
                        db.ExecuteNonQuery(dbCommandSalaryStructDelete, transaction);

                    }
                    foreach (SalaryStruct oSalaryStruct in lstSalStruct)
                    {
                        SqlCommand dbCommandSalaryStruct;
                        if (strSaveType.Trim() == "ADDS")
                        {
                            oSalaryStruct.Employee = db.GetParameterValue(dbCommand, "TransNo").ToString();
                            dbCommandSalaryStruct = oSalaryStruct.AddCommand();
                            db.ExecuteNonQuery(dbCommandSalaryStruct, transaction);
                        }
                        else if (strSaveType.Trim() == "EDIT")
                        {
                            oSalaryStruct.Employee = strTransNo;
                            dbCommandSalaryStruct = oSalaryStruct.AddCommand();
                            db.ExecuteNonQuery(dbCommandSalaryStruct, transaction);
                        }

                    }                    
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
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
                oCommand = db.GetStoredProcCommand("EmployeeChkTransNoExist") as SqlCommand;
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

        #region Get 
        public bool GetEmployee()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("EmployeeSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                strSurname = thisRow[0]["Surname"].ToString();
                strFirstName = thisRow[0]["FirstName"].ToString();
                strOtherName = thisRow[0]["OtherName"].ToString();
                if (thisRow[0]["DOB"] == null || thisRow[0]["DOB"].ToString().Trim() == "")
                {
                    datDOB = DateTime.MinValue;
                }
                else
                {
                    datDOB = DateTime.ParseExact(thisRow[0]["DOB"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                }
                
                strAddr1 = thisRow[0]["Addr1"].ToString();
                strAddr2 = thisRow[0]["Addr2"].ToString();
                strTelephone = thisRow[0]["Telephone"].ToString();
                if (thisRow[0]["State"] != null && thisRow[0]["State"].ToString() != "")
                {
                    intState = int.Parse(thisRow[0]["State"].ToString());
                }
                else
                {
                    intState = 0;
                }
                strReligion = thisRow[0]["Religion"].ToString();
                strCountry = thisRow[0]["Country"].ToString();
                strDeficiency = thisRow[0]["Deficiency"].ToString();
                strQualification = thisRow[0]["Qualification"].ToString();
                if (thisRow[0]["Photo"] != System.DBNull.Value)
                {
                    imgPhoto = (byte[])thisRow[0]["Photo"];
                }
                else
                {
                    imgPhoto = null;
                }
                if (thisRow[0]["Signature"] != System.DBNull.Value)
                {
                    imgSignature = (byte[])thisRow[0]["Signature"];
                }
                else
                {
                    imgSignature = null;
                }

                if (thisRow[0]["ResumeDate"] == null || thisRow[0]["ResumeDate"].ToString().Trim() == "")
                {
                    datResumeDate = DateTime.MinValue;
                }
                else
                {
                    datResumeDate = DateTime.ParseExact(thisRow[0]["ResumeDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                }
                if (thisRow[0]["RetireDate"] == null || thisRow[0]["RetireDate"].ToString().Trim() == "")
                {
                    datRetireDate = DateTime.MinValue;
                }
                else
                {
                    datRetireDate = DateTime.ParseExact(thisRow[0]["RetireDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                }

                if (thisRow[0]["Occupation"] != null && thisRow[0]["Occupation"].ToString() != "")
                {
                    intOccupation = int.Parse(thisRow[0]["Occupation"].ToString());
                }
                else
                {
                    intOccupation = 0;
                }
                strOccupLevel = thisRow[0]["OccLevel"].ToString();
                if (thisRow[0]["PensionMngr"] != null && thisRow[0]["PensionMngr"].ToString() != "")
                {
                    intPensionMngr = int.Parse(thisRow[0]["PensionMngr"].ToString());
                }
                else
                {
                    intPensionMngr = 0;
                }
                strPensionNo = thisRow[0]["PensionNo"].ToString();
                strBankName = thisRow[0]["BankName"].ToString();
                strBankAC = thisRow[0]["BankAC"].ToString();
                strHMOType = thisRow[0]["HMOType"].ToString();
                strSalaryRateType = thisRow[0]["SalaryRateType"].ToString();
                if (thisRow[0]["BasicSalary"] != null && thisRow[0]["BasicSalary"].ToString().Trim() != "")
                {
                    decBasicSalary = decimal.Parse(thisRow[0]["BasicSalary"].ToString());
                }
                else
                {
                    decBasicSalary = 0;
                }
                strBranch = thisRow[0]["Branch"].ToString();
                blnDoNotGenerateSalary = thisRow[0]["DoNotGenerateSalary"] != null && thisRow[0]["DoNotGenerateSalary"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["DoNotGenerateSalary"].ToString()) : false;
                blnDoNotChargeNHF = thisRow[0]["DoNotChargeNHF"] != null && thisRow[0]["DoNotChargeNHF"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["DoNotChargeNHF"].ToString()) : false;
                blnDoNotChargeNSITF = thisRow[0]["DoNotChargeNSITF"] != null && thisRow[0]["DoNotChargeNSITF"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["DoNotChargeNSITF"].ToString()) : false;
                ProductId = thisRow[0]["ProductId"] != null ? thisRow[0]["ProductId"].ToString() : "";
                CustomerId = thisRow[0]["CustomerId"] != null ? thisRow[0]["CustomerId"].ToString() : "";
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
        public DataSet GetAll(string strOrderType)
        {
            var conn = ConfigurationManager.ConnectionStrings["GlobalSuitedb"];
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.Create(conn.Name) as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strOrderType.Trim() != "NAME")
            {
                dbCommand = db.GetStoredProcCommand("EmployeeSelectAll") as SqlCommand;
            }
            else
            {
                dbCommand = db.GetStoredProcCommand("EmployeeSelectAllByName") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion


        #region Get All To Generate Salary
        public DataSet GetAllToGenerateSalary()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("EmployeeSelectAllToGenerateSalary") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All FullName
        public DataSet GetAllByFullName()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;

            dbCommand = db.GetStoredProcCommand("EmployeeSelectAllFullName") as SqlCommand;
           
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public DataGeneral.SaveStatus Delete()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("EmployeeDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Update EmployeeNumberSortByNetAmount
        public void UpdateEmployeeNumberSortByNetAmount()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("EmployeeUpdateEmployeeNoSortByNetAmount") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "EmployeeNoSortByNetAmount", SqlDbType.Int, intEmployeeNoSortByNetAmount);
            db.ExecuteNonQuery(oCommand);
        }
        #endregion


        #region Get All Order By Surname - Search
        public DataSet GetAllBySurnameSearch()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("EmployeeSelectAllBySurnameSearch") as SqlCommand;
            dbCommand = db.GetStoredProcCommand("EmployeeSelectAllBySurnameSearch") as SqlCommand;
            db.AddInParameter(dbCommand, "Surname", SqlDbType.VarChar, strSurname.Trim());
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "OtherName", SqlDbType.VarChar, strOtherName.Trim());
            db.AddInParameter(dbCommand, "FirstName", SqlDbType.VarChar, strFirstName.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Employee Name
        public string GetEmployeeName(string strStaffCode)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("EmployeeSelectEmployeeName") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strStaffCode.Trim());
            var varStaffName = db.ExecuteScalar(oCommand);
            return varStaffName != null ? varStaffName.ToString() : "";
        }
        #endregion

        #region Get Employee CustomerId And ProductId
        public bool GetEmployeeCustomerIdProductId(string strStaffCode)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("EmployeeSelectEmployeeCustomerIdProductId") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strStaffCode.Trim());
            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                ProductId = thisRow[0]["ProductId"] != null ? thisRow[0]["ProductId"].ToString() : "";
                CustomerId = thisRow[0]["CustomerId"] != null ? thisRow[0]["CustomerId"].ToString() : "";
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Get Employee Name With Product
        public bool GetEmployeeName(string strStaffCode, string ProductCode)
        {
            Product oProduct = new Product();
            oProduct.TransNo = ProductCode;
            if (!oProduct.GetProduct())
            {
                throw new Exception("Product Does Not Exist");
            }
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("EmployeeSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strStaffCode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strFirstName = thisRow[0]["FirstName"].ToString();
                strOtherName = thisRow[0]["OtherName"].ToString();
                strSurname = thisRow[0]["Surname"].ToString();
                strBranch = thisRow[0]["Branch"].ToString();
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Update Basic Salary Amount And Period Type Return Command
        public SqlCommand UpdateBasicSalaryAmountAndPeriodTypeReturnCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("EmployeeUpdateBasicSalaryPeriodType") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "SalaryRateType", SqlDbType.VarChar, strSalaryRateType.Trim());
            db.AddInParameter(dbCommand, "BasicSalary", SqlDbType.Decimal, decBasicSalary);
            return dbCommand;
        }
        #endregion

        #region Get All AC Not Created
        public DataSet GetAllACNotCreate(string strProductCode)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("EmployeeSelectAllACNotCreate") as SqlCommand;
            db.AddInParameter(dbCommand, "Product", SqlDbType.VarChar, strProductCode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All AC Not Create Order By Surname - Search
        public DataSet GetAllACNotCreateBySurnameSearch(string strProductCode)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("EmployeeSelectAllACNotCreateBySurnameSearch") as SqlCommand;
            db.AddInParameter(dbCommand, "Surname", SqlDbType.VarChar, strSurname.Trim());
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "OtherName", SqlDbType.VarChar, strOtherName.Trim());
            db.AddInParameter(dbCommand, "FirstName", SqlDbType.VarChar, strFirstName.Trim());
            db.AddInParameter(dbCommand, "Product", SqlDbType.VarChar, strProductCode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

    }
}
