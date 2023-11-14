using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

/// <summary>
/// Summary description for Sector
/// </summary>
/// 
namespace CapitalMarket.Business
{
    public class Sector
    {
        #region Declaration
        private string strIndcode;
        private string strIndname;
        private string strUserId;
        private int intSectorHeader;
        #endregion

        #region Properties
        public string Indcode
        {
            set { strIndcode = value; }
            get { return strIndcode; }
        }
        public string Indname
        {
            set { strIndname = value; }
            get { return strIndname; }
        }

        public string UserId
        {
            set { strUserId = value; }
            get { return strUserId; }
        }

        public int SectorHeader
        {
            set { intSectorHeader = value; }
            get { return intSectorHeader; }
        }
        #endregion

        public Sector()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region Add New Sector
        public bool Add()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("SectorAddNew") as SqlCommand;

            db.AddInParameter(oCommand, "Indcode", SqlDbType.VarChar, strIndcode.Trim());
            db.AddInParameter(oCommand, "Indname", SqlDbType.VarChar, strIndname.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.AddInParameter(oCommand, "SectorHeader", SqlDbType.Int, intSectorHeader);
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "SECTOR");
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Edit Sector
        public bool Edit()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand;
            oCommand = db.GetStoredProcCommand("SectorEdit") as SqlCommand;
            db.AddInParameter(oCommand, "Indcode", SqlDbType.VarChar, strIndcode.Trim());
            db.AddInParameter(oCommand, "Indname", SqlDbType.VarChar, strIndname.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.AddInParameter(oCommand, "SectorHeader", SqlDbType.Int, intSectorHeader);
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;

        }
        #endregion

        #region Get Sector
        public bool GetSector()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("SectorSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "Indcode", SqlDbType.VarChar, strIndcode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strIndcode = thisRow[0]["Indcode"].ToString();
                strIndname = thisRow[0]["Indname"].ToString();
                intSectorHeader = thisRow[0]["SectorHeader"] != null && thisRow[0]["SectorHeader"].ToString().Trim() != "" ? int.Parse(thisRow[0]["SectorHeader"].ToString()) : 0;

                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }

            return blnStatus;
        }
        #endregion

        #region Get All Sector
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("SectorSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("SectorDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Indcode", SqlDbType.VarChar, strIndcode.Trim());
            db.AddInParameter(oCommand, "Indname", SqlDbType.VarChar, strIndname.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());



            db.ExecuteNonQuery(oCommand);
            blnStatus = true;


            return blnStatus;
        }
        #endregion

        #region Check That Sector Name Already Exist For Existing Record
        public bool SectorNameExist()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("SectorSelectByIndNameExist") as SqlCommand;
            db.AddInParameter(dbCommand, "Indname", SqlDbType.VarChar, strIndname.Trim());
            db.AddOutParameter(dbCommand, "NameExist", SqlDbType.VarChar, 1);
            db.AddInParameter(dbCommand, "Transno", SqlDbType.VarChar, strIndcode.Trim());
            db.ExecuteNonQuery(dbCommand);
            if (db.GetParameterValue(dbCommand, "NameExist").ToString().Trim() == "0")
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

        #region Check That Sector Name Already Exist For New Record
        public bool SectorNameExistNoTransNo()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("SectorSelectByIndNameExistNoTransNo") as SqlCommand;
            db.AddInParameter(dbCommand, "Indname", SqlDbType.VarChar, strIndname.Trim());
            db.AddOutParameter(dbCommand, "NameExist", SqlDbType.VarChar, 1);
            db.ExecuteNonQuery(dbCommand);
            if (db.GetParameterValue(dbCommand, "NameExist").ToString().Trim() == "0")
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

        #region Get All Sector By Sector Header
        public DataSet GetAllBySectorHeader()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("SectorSelectAllBySectorHeader") as SqlCommand;
            db.AddInParameter(dbCommand, "SectorHeader", SqlDbType.Int, intSectorHeader);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion
    }
}
