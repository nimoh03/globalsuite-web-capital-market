using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.SqlTypes;
using System.Globalization;
using BaseUtility.Business;

namespace HR.Business
{
    public class EduQualif
    {
        #region Declarations
        private string strTransNo, strEmployee,strSchool, strQualification, strCourse;
        private DateTime datStartDate, datEndDate;

        private string strUserId, strTxnDate, strTxnTime;
        private string strSaveType;
        #endregion

        #region Properties
        public DateTime StartDate
        {
            set { datStartDate = value; }
            get { return datStartDate; }
        }
        public DateTime EndDate
        {
            set { datEndDate = value; }
            get { return datEndDate; }
        }
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
       
        public string School
        {
            set { strSchool = value; }
            get { return strSchool; }
        }
        public string Qualification
        {
            set { strQualification = value; }
            get { return strQualification; }
        }
        public string Course
        {
            set { strCourse = value; }
            get { return strCourse; }
        }
        public string UserId
        {
            set { strUserId = value; }
            get { return strUserId; }
        }
        public string TxnDate
        {
            set { strTxnDate = value; }
            get { return strTxnDate; }
        }
        public string TxnTime
        {
            set { strTxnTime = value; }
            get { return strTxnTime; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        #endregion

        #region Add To Temp Table Return Command
        public SqlCommand AddTempCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("EduQualifTempAdd") as SqlCommand;
            db.AddInParameter(dbCommand, "Employee", SqlDbType.VarChar, strEmployee.Trim());
            db.AddInParameter(dbCommand, "School", SqlDbType.VarChar, strSchool.ToUpper().Trim());
            db.AddInParameter(dbCommand, "Qualification", SqlDbType.VarChar, strQualification.Trim());
            db.AddInParameter(dbCommand, "Course", SqlDbType.VarChar, strCourse.Trim());
            if (datStartDate != DateTime.MinValue)
            {
                db.AddInParameter(dbCommand, "StartDate", SqlDbType.DateTime, datStartDate);

            }
            else
            {
                db.AddInParameter(dbCommand, "StartDate", SqlDbType.DateTime, SqlDateTime.Null);

            }
            if (datEndDate != DateTime.MinValue)
            {
                db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, datEndDate);

            }
            else
            {
                db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, SqlDateTime.Null);

            }
            return dbCommand; 
        }
        #endregion

        #region Add Return Command
        public SqlCommand AddCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("EduQualifAdd") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo);
            db.AddInParameter(dbCommand, "Employee", SqlDbType.VarChar, strEmployee.Trim());
            db.AddInParameter(dbCommand, "School", SqlDbType.VarChar, strSchool.ToUpper().Trim());
            db.AddInParameter(dbCommand, "Qualification", SqlDbType.VarChar, strQualification.Trim());
            db.AddInParameter(dbCommand, "Course", SqlDbType.VarChar, strCourse.Trim());
            if (datStartDate != DateTime.MinValue)
            {
                db.AddInParameter(dbCommand, "StartDate", SqlDbType.DateTime, datStartDate);

            }
            else
            {
                db.AddInParameter(dbCommand, "StartDate", SqlDbType.DateTime, SqlDateTime.Null);

            }
            if (datEndDate != DateTime.MinValue)
            {
                db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, datEndDate);

            }
            else
            {
                db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, SqlDateTime.Null);

            }
            db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return dbCommand;
        }
        #endregion

        #region Delete By Employee Temp Table Return Command
        public SqlCommand DeleteTempByEmployeeCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("EduQualifTempDeleteByEmployee") as SqlCommand;
            db.AddInParameter(dbCommand, "Employee", SqlDbType.VarChar, strEmployee.Trim());
            return dbCommand;
        }
        #endregion

        #region Delete By Employee Return Command
        public SqlCommand DeleteByEmployeeCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("EduQualifDeleteByEmployee") as SqlCommand;
            db.AddInParameter(dbCommand, "Employee", SqlDbType.VarChar, strEmployee.Trim());
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
                oCommand = db.GetStoredProcCommand("EduQualifChkTransNoExist") as SqlCommand;
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

        #region Get Education Qualification
        public bool GetEduQualif()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("EduQualifSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                strEmployee = thisRow[0]["Employee"].ToString();
                strSchool = thisRow[0]["School"].ToString();
                strQualification = thisRow[0]["Qualification"].ToString();
                strCourse = thisRow[0]["Course"].ToString();
                if (thisRow[0]["StartDate"].ToString() == "" || thisRow[0]["StartDate"].ToString() == null)
                {
                    datStartDate = DateTime.MinValue;
                }
                else
                {
                    datStartDate = DateTime.ParseExact(thisRow[0]["StartDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                }

                if (thisRow[0]["EndDate"].ToString() == "" || thisRow[0]["EndDate"].ToString() == null)
                {
                    datStartDate = DateTime.MinValue;
                }
                else
                {
                    datStartDate = DateTime.ParseExact(thisRow[0]["EndDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                }

                strUserId = thisRow[0]["UserId"].ToString();
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
            SqlCommand dbCommand = db.GetStoredProcCommand("EduQualifSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By Employee
        public DataSet GetAllByEmployee(string strOrderBy)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strOrderBy.Trim() == "NAME")
            {
                dbCommand = db.GetStoredProcCommand("EduQualifSelectAllByEmployeeOrderByName") as SqlCommand;
            }
            else
            {
                dbCommand = db.GetStoredProcCommand("EduQualifSelectAllByEmployee") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "Employee", SqlDbType.VarChar, strEmployee.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All In Temp Table By Employee
        public DataSet GetTempAllByEmployee()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;

            dbCommand = db.GetStoredProcCommand("EduQualifTempSelectAllByEmployee") as SqlCommand;

            db.AddInParameter(dbCommand, "Employee", SqlDbType.VarChar, strEmployee.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public DataGeneral.SaveStatus Delete()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("EduQualifDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion
    }
}
