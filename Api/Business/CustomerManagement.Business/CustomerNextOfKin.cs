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
    public class CustomerNextOfKin
    {
        #region Declarations
        private string strCustAId;
        private string strNextKinName, strNextKinAddress, strNextKinPhone, strNextKinRelationship, strEmailAddress;
        private string strSaveType;
        #endregion

        #region Properties
        public string CustAID
        {
            set { strCustAId = value; }
            get { return strCustAId; }
        }
        public string NextKinName
        {
            set { strNextKinName = value; }
            get { return strNextKinName; }
        }
        public string NextKinAddress
        {
            set { strNextKinAddress = value; }
            get { return strNextKinAddress; }
        }
        public string NextKinPhone
        {
            set { strNextKinPhone = value; }
            get { return strNextKinPhone; }
        }
        public string NextKinRelationship
        {
            set { strNextKinRelationship = value; }
            get { return strNextKinRelationship; }
        }
        public string EmailAddress
        {
            set { strEmailAddress = value; }
            get { return strEmailAddress; }
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
                oCommand = db.GetStoredProcCommand("CustomerNextOfKinAdd") as SqlCommand;
            }
            else if (strSaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("CustomerNextOfKinEdit") as SqlCommand;
            }
            db.AddInParameter(oCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(oCommand, "NextKinName", SqlDbType.VarChar, strNextKinName.Trim());
            db.AddInParameter(oCommand, "NextKinAddress", SqlDbType.VarChar, strNextKinAddress.Trim());
            db.AddInParameter(oCommand, "NextKinPhone", SqlDbType.VarChar, strNextKinPhone.Trim());
            db.AddInParameter(oCommand, "NextKinRelationship", SqlDbType.VarChar, strNextKinRelationship.Trim());
            db.AddInParameter(oCommand, "EmailAddress", SqlDbType.VarChar, strEmailAddress.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Get
        public bool GetCustomerNextOfKin()
        {
            bool blnStatus = false;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustomerNextOfKinSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strCustAId = thisRow[0]["CustAID"].ToString();
                strNextKinName = thisRow[0]["NextKinName"] != null ? thisRow[0]["NextKinName"].ToString() : "";
                strNextKinAddress = thisRow[0]["NextKinAddress"] != null ? thisRow[0]["NextKinAddress"].ToString() : "";
                strNextKinPhone = thisRow[0]["NextKinPhone"] != null ? thisRow[0]["NextKinPhone"].ToString() :"";
                strNextKinRelationship = thisRow[0]["NextKinRelationship"] != null ? thisRow[0]["NextKinRelationship"].ToString() : "";
                strEmailAddress = thisRow[0]["EmailAddress"] != null ? thisRow[0]["EmailAddress"].ToString() : "";
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion
    }
}
