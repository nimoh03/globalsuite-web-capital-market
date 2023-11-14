using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

/// Summary description for Broker
/// </summary>
/// 
namespace CapitalMarket.Business
{
    public class Broker
    {
        #region Declaration
        private string strTransno, strCode, strDescription, strAddr1, strAddr2;
        private string strAddr3, strTel, strFax, strEmail, strUserID, strPortManager;
        private DateTime datTxnDate;
        #endregion

        #region Properties
        public string Transno
        {
            set { strTransno = value; }
            get { return strTransno; }
        }
        public string Code
        {
            set { strCode = value; }
            get { return strCode; }
        }
        public string Description
        {
            set { strDescription = value; }
            get { return strDescription; }
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
        public string Addr3
        {
            set { strAddr3 = value; }
            get { return strAddr3; }
        }
        public string Tel
        {
            set { strTel = value; }
            get { return strTel; }
        }
        public string Fax
        {
            set { strFax = value; }
            get { return strFax; }
        }
        public string Email
        {
            set { strEmail = value; }
            get { return strEmail; }
        }
        public string UserID
        {
            set { strUserID = value; }
            get { return strUserID; }
        }
        public string PortManager
        {
            set { strPortManager = value; }
            get { return strPortManager; }
        }

        public DateTime TxnDate
        {
            set { datTxnDate = value; }
            get { return datTxnDate; }
        }
        #endregion

        public Broker()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region Enum
        public enum SaveStatus { Nothing, NotExist, Saved }
        #endregion

        #region Add New Broker
        public bool Add()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BrokerAddNew") as SqlCommand;
            db.AddInParameter(dbCommand, "Transno", SqlDbType.VarChar, strTransno);
            db.AddInParameter(dbCommand, "Code", SqlDbType.VarChar, strCode);
            db.AddInParameter(dbCommand, "Description", SqlDbType.VarChar, strDescription);
            db.AddInParameter(dbCommand, "Addr1", SqlDbType.VarChar, strAddr1);
            db.AddInParameter(dbCommand, "Addr2", SqlDbType.VarChar, strAddr2);
            db.AddInParameter(dbCommand, "Addr3", SqlDbType.VarChar, strAddr3);
            db.AddInParameter(dbCommand, "Tel", SqlDbType.VarChar, strTel);
            db.AddInParameter(dbCommand, "Fax", SqlDbType.VarChar, strFax);
            db.AddInParameter(dbCommand, "Email", SqlDbType.VarChar, strEmail);
            db.AddInParameter(dbCommand, "PortManager", SqlDbType.VarChar, "");
            db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, strUserID);
            db.AddInParameter(dbCommand, "TableName", SqlDbType.VarChar, "BROKER");
            db.ExecuteNonQuery(dbCommand);
            blnStatus = true;
            return blnStatus;

        }
        #endregion

        #region Edit Broker
        public bool Edit()
        {

            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BrokerEdit") as SqlCommand;
            db.AddInParameter(dbCommand, "Transno", SqlDbType.VarChar, strTransno);
            db.AddInParameter(dbCommand, "Code", SqlDbType.VarChar, strCode);
            db.AddInParameter(dbCommand, "Description", SqlDbType.VarChar, strDescription);
            db.AddInParameter(dbCommand, "Addr1", SqlDbType.VarChar, strAddr1);
            db.AddInParameter(dbCommand, "Addr2", SqlDbType.VarChar, strAddr2);
            db.AddInParameter(dbCommand, "Addr3", SqlDbType.VarChar, strAddr3);
            db.AddInParameter(dbCommand, "Tel", SqlDbType.VarChar, strTel);
            db.AddInParameter(dbCommand, "Fax", SqlDbType.VarChar, strFax);
            db.AddInParameter(dbCommand, "Email", SqlDbType.VarChar, strEmail);
            db.AddInParameter(dbCommand, "PortManager", SqlDbType.VarChar, "");
            db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, strUserID);
            
            
                db.ExecuteNonQuery(dbCommand);
                blnStatus = true;
                return blnStatus;
            
        }
        #endregion

        #region Get Broker 
        public bool GetBroker()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BrokerSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "Transno", SqlDbType.VarChar, strTransno.Trim());
           
                DataSet oDS = db.ExecuteDataSet(dbCommand);
                DataTable thisTable = oDS.Tables[0];
                DataRow[] thisRow = thisTable.Select();
                if (thisRow.Length == 1)
                {
                    strTransno = thisRow[0]["Transno"].ToString();
                    strCode = thisRow[0]["Code"].ToString();
                    strDescription = thisRow[0]["Description"].ToString();
                    strAddr1 = thisRow[0]["Addr1"].ToString();
                    strAddr2 = thisRow[0]["Addr2"].ToString();
                    strAddr3 = thisRow[0]["Addr3"].ToString();
                    strTel = thisRow[0]["Tel"].ToString();
                    strFax = thisRow[0]["Fax"].ToString();
                    strEmail = thisRow[0]["Email"].ToString();
                    
                    blnStatus = true;
                }
                else
                {
                    blnStatus = false;
                }
            

            return blnStatus;
        }
        #endregion

        #region Get Broker Other
        public bool GetBrokerOther()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BrokerSelectByCode") as SqlCommand;
            db.AddInParameter(dbCommand, "Code", SqlDbType.VarChar, strCode.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransno = thisRow[0]["Transno"].ToString();
                strCode = thisRow[0]["Code"].ToString();
                strDescription = thisRow[0]["Description"].ToString();
                strAddr1 = thisRow[0]["Addr1"].ToString();
                strAddr2 = thisRow[0]["Addr2"].ToString();
                strAddr3 = thisRow[0]["Addr3"].ToString();
                strTel = thisRow[0]["Tel"].ToString();
                strFax = thisRow[0]["Fax"].ToString();
                strEmail = thisRow[0]["Email"].ToString();

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
            SqlCommand dbCommand = db.GetStoredProcCommand("BrokerSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Broker List
        public DataSet GetStockBrokerList(string strCustomer, DateTime datFrom, DateTime datTo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("rpt_BrokerList") as SqlCommand;
            db.AddInParameter(dbCommand, "Customer", SqlDbType.VarChar, strCustomer);
            db.AddInParameter(dbCommand, "From", SqlDbType.DateTime, datFrom);
            db.AddInParameter(dbCommand, "To", SqlDbType.DateTime, datTo);

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BrokerDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransno);
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCode);
            db.AddInParameter(oCommand, "Description", SqlDbType.VarChar, strDescription);
            db.AddInParameter(oCommand, "Addr1", SqlDbType.VarChar, strAddr1);
            db.AddInParameter(oCommand, "Addr2", SqlDbType.VarChar, strAddr2);
            db.AddInParameter(oCommand, "Addr3", SqlDbType.VarChar, strAddr3);
            db.AddInParameter(oCommand, "Tel", SqlDbType.VarChar, strTel);
            db.AddInParameter(oCommand, "Fax", SqlDbType.VarChar, strFax);
            db.AddInParameter(oCommand, "Email", SqlDbType.VarChar, strEmail);
            db.AddInParameter(oCommand, "PortManager", SqlDbType.VarChar, "");
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, strUserID);


            db.ExecuteNonQuery(oCommand);
            blnStatus = true;

            return blnStatus;
        }
        #endregion

        #region Check That Broker Code Already Exist For Existing Record
        public bool BrokerCodeExist()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BrokerSelectByCodeExist") as SqlCommand;
            db.AddInParameter(dbCommand, "Code", SqlDbType.VarChar, strCode.Trim());
            db.AddOutParameter(dbCommand, "CodeExist", SqlDbType.VarChar, 1);
            db.AddInParameter(dbCommand, "Transno", SqlDbType.VarChar, strTransno.Trim());
            db.ExecuteNonQuery(dbCommand);
            if (db.GetParameterValue(dbCommand, "CodeExist").ToString().Trim() == "0")
            {
                blnStatus = false;
            }
            else
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Check That Broker Code Already Exist For New Record
        public bool BrokerCodeExistNoTransNo()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BrokerSelectByCodeExistNoTransno") as SqlCommand;
            db.AddInParameter(dbCommand, "Code", SqlDbType.VarChar, strCode.Trim());
            db.AddOutParameter(dbCommand, "CodeExist", SqlDbType.VarChar, 1);
            db.ExecuteNonQuery(dbCommand);
            if (db.GetParameterValue(dbCommand, "CodeExist").ToString().Trim() == "0")
            {
                blnStatus = false;
            }
            else
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Check That Broker Name Already Exist For Existing Record
        public bool BrokerNameExist()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BrokerSelectByDescriptionExist") as SqlCommand;
            db.AddInParameter(dbCommand, "Description", SqlDbType.VarChar, strDescription.Trim());
            db.AddOutParameter(dbCommand, "DescriptionExist", SqlDbType.VarChar, 1);
            db.AddInParameter(dbCommand, "Transno", SqlDbType.VarChar, strTransno.Trim());
            db.ExecuteNonQuery(dbCommand);
            if (db.GetParameterValue(dbCommand, "DescriptionExist").ToString().Trim() == "0")
            {
                blnStatus = false;
            }
            else
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Check That Broker Name Already Exist For New Record
        public bool BrokerNameExistNoTransNo()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BrokerSelectByDescriptionExistNoTransno") as SqlCommand;
            db.AddInParameter(dbCommand, "Description", SqlDbType.VarChar, strDescription.Trim());
            db.AddOutParameter(dbCommand, "DescriptionExist", SqlDbType.VarChar, 1);
            db.ExecuteNonQuery(dbCommand);
            if (db.GetParameterValue(dbCommand, "DescriptionExist").ToString().Trim() == "0")
            {
                blnStatus = false;
            }
            else
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Check That Portfolio Manager Already Exist For Existing Record
        public bool PortMangerExist()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BrokerSelectByPortManagerExist") as SqlCommand;
            db.AddOutParameter(dbCommand, "PortManagerExist", SqlDbType.VarChar, 1);
            db.AddInParameter(dbCommand, "Transno", SqlDbType.VarChar, strTransno.Trim());
            db.ExecuteNonQuery(dbCommand);
            if (db.GetParameterValue(dbCommand, "PortManagerExist").ToString().Trim() == "0")
            {
                blnStatus = false;
            }
            else
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Check That Portfolio Manager Already Exist For New Record
        public bool PortMangerExistNoTransno()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BrokerSelectByPortManagerExistNoTransno") as SqlCommand;
            db.AddOutParameter(dbCommand, "PortManagerExist", SqlDbType.VarChar, 1);
            db.ExecuteNonQuery(dbCommand);
            if (db.GetParameterValue(dbCommand, "PortManagerExist").ToString().Trim() == "0")
            {
                blnStatus = false;
            }
            else
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion



    }
}
