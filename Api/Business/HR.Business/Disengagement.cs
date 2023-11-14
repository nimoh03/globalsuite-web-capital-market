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
    public class Disengagement
    {
        #region Declarations
        private string strTransNo,strEmpNo;
        private string strDisengageType, strReason;
        private DateTime datDisengageDate;

        private string strUserID;
        private string strSaveType;
        #endregion

        #region Properties
        public DateTime DisengageDate
        {
            set { datDisengageDate = value; }
            get { return datDisengageDate; }
        }
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
       
        public string EmpNo
        {
            set { strEmpNo = value; }
            get { return strEmpNo; }
        }
        public string DisengageType
        {
            set { strDisengageType = value; }
            get { return strDisengageType; }
        }
        public string Reason
        {
            set { strReason = value; }
            get { return strReason; }
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
                dbCommand = db.GetStoredProcCommand("DisengagementAdd") as SqlCommand;
            }
            else if (strSaveType.Trim() == "EDIT")
            {
                dbCommand = db.GetStoredProcCommand("DisengagementEdit") as SqlCommand;
            }

            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "EmpNo", SqlDbType.VarChar, strEmpNo.Trim());
            db.AddInParameter(dbCommand, "DisengageType", SqlDbType.VarChar, strDisengageType.Trim());

            if (datDisengageDate != DateTime.MinValue)
            {
                db.AddInParameter(dbCommand, "DisengageDate", SqlDbType.DateTime, datDisengageDate);

            }
            else
            {
                db.AddInParameter(dbCommand, "DisengageDate", SqlDbType.DateTime, SqlDateTime.Null);

            }

            db.AddInParameter(dbCommand, "Reason", SqlDbType.VarChar, strReason.Trim());
            
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
                oCommand = db.GetStoredProcCommand("DisengagementChkTransNoExist") as SqlCommand;
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

        #region Get Disengagement
        public bool GetDisengagement()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DisengagementSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                strEmpNo = thisRow[0]["EmpNo"].ToString();
                strDisengageType = thisRow[0]["DisengageType"].ToString();
                if (thisRow[0]["DisengageDate"].ToString() == "" || thisRow[0]["DisengageDate"].ToString() == null)
                {
                    datDisengageDate = DateTime.MinValue;
                }
                else
                {
                    datDisengageDate = DateTime.ParseExact(thisRow[0]["DisengageDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                }
                strReason = thisRow[0]["Reason"].ToString();
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
            SqlCommand dbCommand = db.GetStoredProcCommand("DisengagementSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public DataGeneral.SaveStatus Delete()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("DisengagementDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion
    }
}
