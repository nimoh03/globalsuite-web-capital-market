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
    public class HMORateDetail
    {
        #region Declarations
        private string strTransNo;
        private int intHMORate;
        private string strHMOType;
        private decimal decAmount;
        private string strUserID;
        private string strSaveType;
        #endregion

        #region Properties
        
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        
        public int HMORate
        {
            set { intHMORate = value; }
            get { return intHMORate; }
        }
        public string HMOType
        {
            set { strHMOType = value; }
            get { return strHMOType; }
        }
        public decimal Amount
        {
            set { decAmount = value; }
            get { return decAmount; }
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

        #region Add Command
        public SqlCommand AddCommand(int oHMORate)
        {
            if (!ChkTransNoExist())
            {
                throw new Exception("Transaction Number Does Not Exist");
            }
            if (ChkNameExist())
            {
                throw new Exception("HMO Type For This HMO Already Exist");
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strSaveType.Trim() == "ADDS")
            {
                dbCommand = db.GetStoredProcCommand("HMORateDetailAdd") as SqlCommand;
                db.AddInParameter(dbCommand, "HMORate", SqlDbType.Int, oHMORate);
            }
            else if (strSaveType.Trim() == "EDIT")
            {
                dbCommand = db.GetStoredProcCommand("HMORateDetailEdit") as SqlCommand;
                db.AddInParameter(dbCommand, "HMORate", SqlDbType.Int, intHMORate);
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "HMOType", SqlDbType.VarChar, strHMOType.Trim());
            db.AddInParameter(dbCommand, "Amount", SqlDbType.Decimal, decAmount);
            db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return dbCommand;
        }
        #endregion

        #region Get HMO Rate Detail
        public bool GetHMORateDetail()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("HMORateDetailSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                intHMORate = int.Parse(thisRow[0]["HMORate"].ToString());
                strHMOType = thisRow[0]["HMOType"].ToString();
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString());
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        

        #region Get HMO Rate Detail By HMORate and HMOType
        public bool GetHMORateDetailByHMORateHMOType()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("HMORateDetailSelectByHMORateHMOType") as SqlCommand;
            db.AddInParameter(dbCommand, "HMORate", SqlDbType.Int, intHMORate);
            db.AddInParameter(dbCommand, "HMOType", SqlDbType.VarChar, strHMOType);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                intHMORate = int.Parse(thisRow[0]["HMORate"].ToString());
                strHMOType = thisRow[0]["HMOType"].ToString();
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString());
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
                oCommand = db.GetStoredProcCommand("HMORateDetailChkTransNoExist") as SqlCommand;
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
            SqlCommand oCommand = db.GetStoredProcCommand("HMORateDetailChkNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.Char, strTransNo.Trim());
            db.AddInParameter(oCommand, "HMORate", SqlDbType.Int, intHMORate);
            db.AddInParameter(oCommand, "HMOType", SqlDbType.VarChar, strHMOType);
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
    }
}
