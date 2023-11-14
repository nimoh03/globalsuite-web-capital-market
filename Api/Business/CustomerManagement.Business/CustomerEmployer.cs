using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using BaseUtility.Business;

namespace CustomerManagement.Business
{
    public class CustomerEmployer
    {
        #region Declarations
        private string strCustAId;
        private string strEmployerName, strJobDescription, strEmployerAddress1, strEmployerAddress2;
        private string strEmployerCity, strEmployerWorkPhone, strEmployerEmail;
        private string strSaveType;
        #endregion

        #region Properties
        public string CustAID
        {
            set { strCustAId = value; }
            get { return strCustAId; }
        }
        public string EmployerName
        {
            set { strEmployerName = value; }
            get { return strEmployerName; }
        }
        public string JobDescription
        {
            set { strJobDescription = value; }
            get { return strJobDescription; }
        }
        public string EmployerAddress1
        {
            set { strEmployerAddress1 = value; }
            get { return strEmployerAddress1; }
        }
        public string EmployerAddress2
        {
            set { strEmployerAddress2 = value; }
            get { return strEmployerAddress2; }
        }
        public string EmployerCity
        {
            set { strEmployerCity = value; }
            get { return strEmployerCity; }
        }
        public string EmployerWorkPhone
        {
            set { strEmployerWorkPhone = value; }
            get { return strEmployerWorkPhone; }
        }
        public string EmployerEmail
        {
            set { strEmployerEmail = value; }
            get { return strEmployerEmail; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        #endregion

        #region Save And Return Command
        public SqlCommand SaveCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (strSaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("CustomerEmployerAdd") as SqlCommand;
            }
            else if (strSaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("CustomerEmployerEdit") as SqlCommand;
            }
            db.AddInParameter(oCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(oCommand, "EmployerName", SqlDbType.VarChar, strEmployerName);
            db.AddInParameter(oCommand, "JobDescription", SqlDbType.VarChar, strJobDescription);
            db.AddInParameter(oCommand, "EmployerAddress1", SqlDbType.VarChar, strEmployerAddress1);
            db.AddInParameter(oCommand, "EmployerAddress2", SqlDbType.VarChar, strEmployerAddress2);
            db.AddInParameter(oCommand, "EmployerCity", SqlDbType.VarChar, strEmployerCity);
            db.AddInParameter(oCommand, "EmployerWorkPhone", SqlDbType.VarChar, strEmployerWorkPhone);
            db.AddInParameter(oCommand, "EmployerEmail", SqlDbType.VarChar, strEmployerEmail);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Get
        public bool GetCustomerEmployer()
        {
            bool blnStatus = false;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustomerEmployerSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strCustAId = thisRow[0]["CustAID"].ToString();
                strEmployerName = thisRow[0]["EmployerName"].ToString();
                strJobDescription = thisRow[0]["JobDescription"].ToString();
                strEmployerAddress1 = thisRow[0]["EmployerAddress1"].ToString();
                strEmployerAddress2 = thisRow[0]["EmployerAddress2"].ToString();
                strEmployerCity = thisRow[0]["EmployerCity"].ToString();
                strEmployerWorkPhone = thisRow[0]["EmployerWorkPhone"].ToString();
                strEmployerEmail = thisRow[0]["EmployerEmail"].ToString();
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion
    }
}
