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
    public class AllowAddition
    {
        #region Declarations
        private string strTransNo, strEmployee,strDescription;
        private decimal decAmount;
        private int intAllDed;
        private DateTime datAllDedDate;
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
        public string Description
        {
            set { strDescription = value; }
            get { return strDescription; }
        }
        public decimal Amount
        {
            set { decAmount = value; }
            get { return decAmount; }
        }
        public int AllDed
        {
            set { intAllDed = value; }
            get { return intAllDed; }
        }
        
        public DateTime AllDedDate
        {
            set { datAllDedDate = value; }
            get { return datAllDedDate; }
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
            //if (ChkNameExist())
            //{
            //    enSaveStatus = DataGeneral.SaveStatus.NameExistAdd;
            //    return enSaveStatus;
            //}

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strSaveType.Trim() == "ADDS")
            {
                dbCommand = db.GetStoredProcCommand("AllowAdditionAdd") as SqlCommand;
            }
            else if (strSaveType.Trim() == "EDIT")
            {
                dbCommand = db.GetStoredProcCommand("AllowAdditionEdit") as SqlCommand;
            }

            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            if (datAllDedDate != DateTime.MinValue)
            {
                db.AddInParameter(dbCommand, "AllDedDate", SqlDbType.DateTime, datAllDedDate);
            }
            else
            {
                db.AddInParameter(dbCommand, "AllDedDate", SqlDbType.DateTime, SqlDateTime.Null);
            }
            db.AddInParameter(dbCommand, "Employee", SqlDbType.VarChar, strEmployee);
            db.AddInParameter(dbCommand, "AllDed", SqlDbType.Int, intAllDed);
            db.AddInParameter(dbCommand, "Amount", SqlDbType.VarChar, decAmount);
            db.AddInParameter(dbCommand, "Description", SqlDbType.VarChar, strDescription.Trim());
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
                oCommand = db.GetStoredProcCommand("AllowAdditionChkTransNoExist") as SqlCommand;
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

        #region Get Allow Addition
        public bool GetAllowAddition()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllowAdditionSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                strEmployee = thisRow[0]["Employee"].ToString();
                if (thisRow[0]["AllDedDate"].ToString() == "" || thisRow[0]["AllDedDate"].ToString() == null)
                {
                    datAllDedDate = DateTime.MinValue;
                }
                else
                {
                    datAllDedDate = DateTime.ParseExact(thisRow[0]["AllDedDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                }
                intAllDed = int.Parse(thisRow[0]["AllDed"].ToString());
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString());
                strDescription = thisRow[0]["Description"].ToString();
                
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
            SqlCommand dbCommand = db.GetStoredProcCommand("AllowAdditionSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By Allowance and Deduction Type
        public DataSet GetAllByAllDed()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllowAdditionSelectAllDed") as SqlCommand;
            db.AddInParameter(dbCommand, "AllDed", SqlDbType.Int, intAllDed);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public DataGeneral.SaveStatus Delete()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AllowAdditionDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion
    }
}
