using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class FX_AssetType
    {
        #region Declarations
        private string strTransNo, strDescrip, strUsefulLife;
        private string strFXAssAcct, strAccDepAcct, strExpAcct, strDispAcct,strShortCode;
        private float fltDepreciationRate;
        private int intPrintPos;
        private string strUserID;
        private string strSaveType;
        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public string Descrip
        {
            set { strDescrip = value; }
            get { return strDescrip; }
        }
        public float DepreciationRate
        {
            set { fltDepreciationRate = value; }
            get { return fltDepreciationRate; }
        }
        public string FXAssAcct
        {
            set { strFXAssAcct = value; }
            get { return strFXAssAcct; }
        }
        public string AccDepAcct
        {
            set { strAccDepAcct = value; }
            get { return strAccDepAcct; }
        }
        public string ExpAcct
        {
            set { strExpAcct = value; }
            get { return strExpAcct; }
        }
        public string DispAcct
        {
            set { strDispAcct = value; }
            get { return strDispAcct; }
        }
        public string UsefulLife
        {
            set { strUsefulLife = value; }
            get { return strUsefulLife; }
        }

        public string ShortCode
        {
            set { strShortCode = value; }
            get { return strShortCode; }
        }

        public int PrintPos
        {
            set { intPrintPos = value; }
            get { return intPrintPos; }
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
                dbCommand = db.GetStoredProcCommand("FX_AssetTypeAddNew") as SqlCommand;
            }
            else if (strSaveType.Trim() == "EDIT")
            {
                dbCommand = db.GetStoredProcCommand("FX_AssetTypeEdit") as SqlCommand;
            }

            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "Descrip", SqlDbType.VarChar, strDescrip.Trim().ToUpper());
            db.AddInParameter(dbCommand, "DepreciationRate", SqlDbType.VarChar, fltDepreciationRate);
            db.AddInParameter(dbCommand, "FXAssAcct", SqlDbType.VarChar, strFXAssAcct.Trim());
            db.AddInParameter(dbCommand, "AccDepAcct", SqlDbType.VarChar, strAccDepAcct.Trim());
            db.AddInParameter(dbCommand, "ExpAcct", SqlDbType.VarChar, strExpAcct.Trim());
            db.AddInParameter(dbCommand, "DispAcct", SqlDbType.VarChar, strDispAcct.Trim());
            db.AddInParameter(dbCommand, "UsefulLife", SqlDbType.VarChar, strUsefulLife.Trim());
            db.AddInParameter(dbCommand, "ShortCode", SqlDbType.VarChar, strShortCode.Trim());
            db.AddInParameter(dbCommand, "PrintPos", SqlDbType.Int, intPrintPos);
            db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
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
                oCommand = db.GetStoredProcCommand("FX_AssetTypeChkTransNoExist") as SqlCommand;
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

        #region Get AssetType
        public bool GetFX_AssetType()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("FX_AssetTypeSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                strDescrip = thisRow[0]["Descrip"].ToString();
                fltDepreciationRate = float.Parse(thisRow[0]["DepreciationRate"].ToString());
                strFXAssAcct = thisRow[0]["FXAssAcct"].ToString();
                strAccDepAcct = thisRow[0]["AccDepAcct"].ToString();
                strExpAcct = thisRow[0]["ExpAcct"].ToString();
                strDispAcct = thisRow[0]["DispAcct"].ToString();
                strUsefulLife = thisRow[0]["UsefulLife"].ToString();
                strShortCode = thisRow[0]["ShortCode"].ToString();
                intPrintPos = int.Parse(thisRow[0]["PrintPos"].ToString());
                strUserID = thisRow[0]["UserID"].ToString();
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
        public DataSet GetAll(string strOrderBy)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strOrderBy.Trim() == "NAME")
            {
                dbCommand = db.GetStoredProcCommand("FX_AssetTypeSelectAllOrderByName") as SqlCommand;
            }
            else
            {
                dbCommand = db.GetStoredProcCommand("FX_AssetTypeSelectAll") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion



        #region Delete
        public DataGeneral.SaveStatus Delete()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("FX_AssetTypeDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Get Asset Type Name Given Print Position
        public string GetAssetTypeNameGivenPrintPos()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("FX_AssetTypeSelectNameGivenPrintPos") as SqlCommand;
            db.AddInParameter(dbCommand, "PrintPos", SqlDbType.Int, intPrintPos);
            if (db.ExecuteScalar(dbCommand) != null)
            {
                return db.ExecuteScalar(dbCommand).ToString();
            }
            else
            {
                return "";
            }
        }
        #endregion
    }
}
