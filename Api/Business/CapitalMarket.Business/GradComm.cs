using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

/// <summary>
/// Summary description for GradComm
/// </summary>
/// 
namespace CapitalMarket.Business
{
    public class GradComm
    {
        #region Declaration
        private string strTransno, strUserId;
        private decimal decperc;
        private decimal decAmount;
        #endregion

        #region Properties
        public string Transno
        {
            set { strTransno = value; }
            get { return strTransno; }
        }
        public decimal perc
        {
            set { decperc = value; }
            get { return decperc; }
        }
        public decimal Amount
        {
            set { decAmount = value; }
            get { return decAmount; }
        }
        public string UserId
        {
            set { strUserId = value; }
            get { return strUserId; }
        }
        #endregion

        public GradComm()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region Enum
        public enum SaveStatus { Nothing, NotExist, Saved }
        #endregion

        #region Add New GradComm
        public bool Add()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand;
            dbCommand = db.GetStoredProcCommand("GradCommAddNew") as SqlCommand;
            db.AddInParameter(dbCommand, "Transno", SqlDbType.VarChar, strTransno.Trim());
            db.AddInParameter(dbCommand, "perc", SqlDbType.Decimal, decperc);
            db.AddInParameter(dbCommand, "Amount", SqlDbType.Money, decAmount);
            db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.AddInParameter(dbCommand, "TableName", SqlDbType.VarChar, "GRADCOMM");
            db.ExecuteNonQuery(dbCommand);
            blnStatus = true;
            return blnStatus;

        }
        #endregion

        #region Edit GradComm
        public bool Edit()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand;
            dbCommand = db.GetStoredProcCommand("GradCommEdit") as SqlCommand;
            db.AddInParameter(dbCommand, "Code", SqlDbType.VarChar, strTransno.Trim());
            db.AddInParameter(dbCommand, "perc", SqlDbType.Decimal, decperc);
            db.AddInParameter(dbCommand, "Amount", SqlDbType.Decimal, decAmount);
            db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            
            db.ExecuteNonQuery(dbCommand);
            blnStatus = true;
            return blnStatus;
            
        }
        #endregion

        #region Get Graduated Commission Info
        public bool GetGradComm()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GradCommSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "Transno", SqlDbType.VarChar, strTransno.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransno = thisRow[0]["Transno"].ToString();
                decperc = decimal.Parse(thisRow[0]["perc"].ToString());
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString());
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Get All Graduated Commission Order By Trans No
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GradCommSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GradCommDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strTransno.Trim());
            db.AddInParameter(oCommand, "perc", SqlDbType.Decimal, decperc);
            db.AddInParameter(oCommand, "Amount", SqlDbType.Decimal, decAmount);
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());




            db.ExecuteNonQuery(oCommand);
            blnStatus = true;

            return blnStatus;
        }
        #endregion

        #region Get Graduated Maximum TransNo 
        public int GetGradCommMaxTransNo()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GradCommSelectMaxTransNo") as SqlCommand;
            return int.Parse(db.ExecuteScalar(dbCommand).ToString());
        }
        #endregion
    }
}
