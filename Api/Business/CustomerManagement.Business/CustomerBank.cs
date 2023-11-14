using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using BaseUtility.Business;

namespace CustomerManagement.Business
{
    public class CustomerBank
    {
        #region Declarations
        private string strCustAId;
        private Int64 lngBankId;
        private string strBankName;
        private string strBankAccountNo,  strBankAddress,strSaveType;
        private DateTime datBankDateOpen;
        #endregion

        #region Properties

        public string CustAID
        {
            set { strCustAId = value; }
            get { return strCustAId; }
        }
        public Int64 BankId
        {
            set { lngBankId = value; }
            get { return lngBankId; }
        }
        public string BankName
        {
            set { strBankName = value; }
            get { return strBankName; }
        }
        public string BankAccountNo
        {
            set { strBankAccountNo = value; }
            get { return strBankAccountNo; }
        }
        public DateTime BankDateOpen
        {
            set { datBankDateOpen = value; }
            get { return datBankDateOpen; }
        }
        public string BankAddress
        {
            set { strBankAddress = value; }
            get { return strBankAddress; }
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
                oCommand = db.GetStoredProcCommand("CustomerBankAdd") as SqlCommand;
            }
            else if (strSaveType == "EDIT")
            {
                if (lngBankId != 0)
                {
                    oCommand = db.GetStoredProcCommand("CustomerBankEdit") as SqlCommand;
                }
                else
                {
                    oCommand = db.GetStoredProcCommand("CustomerBankAdd") as SqlCommand;
                }
            }
            db.AddInParameter(oCommand, "BankId", SqlDbType.BigInt, lngBankId);
            db.AddInParameter(oCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(oCommand, "BankName", SqlDbType.VarChar, strBankName.Trim());
            db.AddInParameter(oCommand, "BankAccountNo", SqlDbType.VarChar, strBankAccountNo.Trim());
            if (datBankDateOpen != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "BankDateOpen", SqlDbType.DateTime, datBankDateOpen);
            }
            else
            {
                db.AddInParameter(oCommand, "BankDateOpen", SqlDbType.DateTime, SqlDateTime.Null);
            }
            db.AddInParameter(oCommand, "BankAddress", SqlDbType.VarChar, strBankAddress.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Get
        public DataSet GetCustomerBankByCustomerId()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustomerBankSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion
    }
}
