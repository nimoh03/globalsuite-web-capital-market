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
    public class PayRollProcess
    {
        #region Declarations
        private string strTransNo, strEmployee;
        private string strSysRef,strAllDedType,strItemType;
        private int intAllDed;
        private decimal decAmount;
        private DateTime datPayRollDate;
        private string strUserId;
        private string strSaveType;
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
        public decimal Amount
        {
            set { decAmount = value; }
            get { return decAmount; }
        }
        public DateTime PayRollDate
        {
            set { datPayRollDate = value; }
            get { return datPayRollDate; }
        }
        public string SysRef
        {
            set { strSysRef = value; }
            get { return strSysRef; }
        }
        public string AllDedType
        {
            set { strAllDedType = value; }
            get { return strAllDedType; }
        }
        public string ItemType
        {
            set { strItemType = value; }
            get { return strItemType; }
        }
        public string UserId
        {
            set { strUserId = value; }
            get { return strUserId; }
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

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strSaveType.Trim() == "ADDS")
            {
                dbCommand = db.GetStoredProcCommand("PayRollProcessAdd") as SqlCommand;
            }
            else if (strSaveType.Trim() == "EDIT")
            {
                dbCommand = db.GetStoredProcCommand("PayRollProcessEdit") as SqlCommand;
            }

            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "Employee", SqlDbType.VarChar, strEmployee.Trim());
            db.AddInParameter(dbCommand, "AllDed", SqlDbType.Int, intAllDed);
            db.AddInParameter(dbCommand, "Amount", SqlDbType.Decimal, decAmount);
            db.AddInParameter(dbCommand, "PayRollDate", SqlDbType.DateTime, datPayRollDate);
            db.AddInParameter(dbCommand, "SysRef", SqlDbType.VarChar, strSysRef.Trim());
            db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, strUserId.Trim());

            db.ExecuteNonQuery(dbCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;

        }
        #endregion

        #region Add And Return Command
        public SqlCommand AddCommand()
        {
            if (!ChkTransNoExist())
            {
                throw new Exception("Transaction Number Does Not Exist");
            }
            //if (ChkNameExist())
            //{
            //    throw new Exception("PayRoll Item(s) Already Posted");
            //}

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            
            dbCommand = db.GetStoredProcCommand("PayRollProcessAdd") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo);
            db.AddInParameter(dbCommand, "Employee", SqlDbType.VarChar, strEmployee.Trim());
            db.AddInParameter(dbCommand, "AllDed", SqlDbType.Int, intAllDed);
            db.AddInParameter(dbCommand, "Amount", SqlDbType.Money, decAmount);
            db.AddInParameter(dbCommand, "PayRollDate", SqlDbType.DateTime, datPayRollDate);
            db.AddInParameter(dbCommand, "SysRef", SqlDbType.VarChar, strSysRef.Trim());
            db.AddInParameter(dbCommand, "AllDedType", SqlDbType.VarChar, strAllDedType.Trim());
            db.AddInParameter(dbCommand, "ItemType", SqlDbType.VarChar, strItemType.Trim());
            db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return dbCommand;

        }
        #endregion

        #region Get PayRoll Process
        public bool GetPayRollProcess()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PayRollProcessSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                strEmployee = thisRow[0]["Employee"].ToString();
                intAllDed = int.Parse(thisRow[0]["AllDed"].ToString());
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString());
                datPayRollDate = DateTime.Parse(thisRow[0]["PayRollDate"].ToString());
                strSysRef = thisRow[0]["SysRef"].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
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
                oCommand = db.GetStoredProcCommand("PayRollProcessChkTransNoExist") as SqlCommand;
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

        #region Check Name Exist
        public bool ChkNameExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PayRollProcessChkNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.Char, strTransNo);
            db.AddInParameter(oCommand, "Employee", SqlDbType.VarChar, strEmployee.Trim());
            db.AddInParameter(oCommand, "AllDed", SqlDbType.Int, intAllDed);
            db.AddInParameter(oCommand, "PayRollDate", SqlDbType.DateTime, datPayRollDate);
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

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PayRollProcessSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public SqlCommand DeleteCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PayRollProcessDelete") as SqlCommand;
            db.AddInParameter(oCommand, "PayRollDate", SqlDbType.DateTime, datPayRollDate);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
            return oCommand;
        }
        #endregion

        #region Get Sort By NetAmount
        public DataSet GetSortByNetAmount()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PayRollProcessSortByNetAmount") as SqlCommand;
            db.AddInParameter(dbCommand, "ReportDate", SqlDbType.DateTime, datPayRollDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion


    }
}
