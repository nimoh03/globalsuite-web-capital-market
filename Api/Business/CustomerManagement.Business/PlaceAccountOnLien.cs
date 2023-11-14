using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;

namespace CustomerManagement.Business
{
    public class PlaceAccountOnLien
    {
        #region Declarations
        private long lngTransNo;
        private string strCustomerNo, strReason;
        private string strSaveType;
        #endregion

        #region Properties
        public long TransNo
        {
            set { lngTransNo = value; }
            get { return lngTransNo; }
        }
        public string CustomerNo
        {
            set { strCustomerNo = value; }
            get { return strCustomerNo; }
        }
        public string Reason
        {
            set { strReason = value; }
            get { return strReason; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        #endregion

        #region Save
        public bool Save()
        {
            bool blnStatus = false;
            if (!ChkPlaceAccountOnLienExist())
            {
                throw new Exception("Cannot Save Place Account On Lien Does Not Exist");
            }

            if (ChkCustomerExist())
            {
                throw new Exception("Cannot Save Customer Number Already Exist");
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (strSaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("PlaceAccountOnLienAdd") as SqlCommand;
            }
            else if (strSaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("PlaceAccountOnLienEdit") as SqlCommand;
            }
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt,lngTransNo);
            db.AddInParameter(oCommand, "CustomerNo", SqlDbType.VarChar,strCustomerNo.Trim());
            db.AddInParameter(oCommand, "Reason", SqlDbType.VarChar,strReason.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Check Place Account On Lien Exist
        public bool ChkPlaceAccountOnLienExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("PlaceAccountOnLienChkTransNoExist") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, lngTransNo);
                DataSet oDs = db.ExecuteDataSet(oCommand);
                if (oDs.Tables[0].Rows.Count > 0)
                {
                    blnStatus = true;
                }
            }
            else if (strSaveType == "ADDS")
            {
                blnStatus = true;
            }

            return blnStatus;
        }
        #endregion

        #region Check Customer Exist Exist
        public bool ChkCustomerExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PlaceAccountOnLienChkCustomerExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, lngTransNo);
            db.AddInParameter(oCommand, "Name", SqlDbType.VarChar, strCustomerNo.Trim());
            db.AddInParameter(oCommand, "SaveType", SqlDbType.VarChar, strSaveType.Trim());
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            
            return blnStatus;
        }
        #endregion

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PlaceAccountOnLienSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get 
        public bool GetPlaceAccountOnLien()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PlaceAccountOnLienSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, lngTransNo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strCustomerNo = thisRow[0]["CustomerNo"].ToString();
                strReason = thisRow[0]["Reason"].ToString();
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Get By Customer
        public bool GetPlaceAccountOnLienByCustomer()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PlaceAccountOnLienSelectByCustomer") as SqlCommand;
            db.AddInParameter(dbCommand, "CustomerNo", SqlDbType.VarChar, strCustomerNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                lngTransNo = long.Parse(thisRow[0]["TransNo"].ToString());
                strReason = thisRow[0]["Reason"].ToString();
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PlaceAccountOnLienDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, lngTransNo);
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion
        
    }
}
