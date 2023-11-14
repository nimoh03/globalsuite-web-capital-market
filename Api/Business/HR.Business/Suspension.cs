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
    public class Suspension
    {
        #region Declarations
        private string strTransNo, strEmployee, strReason;
        private bool blnWithoutPay;
        private DateTime datSuspensionDate;

        private string strUserId;
        private string strSaveType;
        #endregion

        #region Properties
        public DateTime SuspensionDate
        {
            set { datSuspensionDate = value; }
            get { return datSuspensionDate; }
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
        public string Reason
        {
            set { strReason = value; }
            get { return strReason; }
        }
        public bool WithoutPay
        {
            set { blnWithoutPay = value; }
            get { return blnWithoutPay; }
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
                dbCommand = db.GetStoredProcCommand("SuspensionAdd") as SqlCommand;
            }
            else if (strSaveType.Trim() == "EDIT")
            {
                dbCommand = db.GetStoredProcCommand("SuspensionEdit") as SqlCommand;
            }

            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "Employee", SqlDbType.VarChar, strEmployee.Trim());
            db.AddInParameter(dbCommand, "Reason", SqlDbType.VarChar, strReason.Trim());
            db.AddInParameter(dbCommand, "WithoutPay", SqlDbType.Bit, blnWithoutPay);
            if (datSuspensionDate != DateTime.MinValue)
            {
                db.AddInParameter(dbCommand, "SuspensionDate", SqlDbType.DateTime, datSuspensionDate);
            }
            else
            {
                db.AddInParameter(dbCommand, "SuspensionDate", SqlDbType.DateTime, SqlDateTime.Null);
            }
            db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
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
                oCommand = db.GetStoredProcCommand("SuspensionChkTransNoExist") as SqlCommand;
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

        #region Get Suspension
        public bool GetSuspension()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("SuspensionSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                strEmployee = thisRow[0]["Employee"].ToString();
                strReason = thisRow[0]["Reason"].ToString();
                blnWithoutPay = bool.Parse(thisRow[0]["WithoutPay"].ToString());
                if (thisRow[0]["SuspensionDate"].ToString() == "" || thisRow[0]["SuspensionDate"].ToString() == null)
                {
                    datSuspensionDate = DateTime.MinValue;
                }
                else
                {
                    datSuspensionDate = DateTime.ParseExact(thisRow[0]["SuspensionDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                }
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
            SqlCommand dbCommand = db.GetStoredProcCommand("SuspensionSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public DataGeneral.SaveStatus Delete()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("SuspensionDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion
    }
}
