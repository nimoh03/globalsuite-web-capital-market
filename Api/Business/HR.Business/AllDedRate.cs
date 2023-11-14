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
    public class AllDedRate
    {
        #region Declaration
        private string strTransNo;
        private int intOccupation;
        private string strOccupLevel;
        private decimal decBasicSalary;
        private string strBasicSalaryPayPeriod;
        private decimal decGrossSalary;
        private string strGrossSalaryPayPeriod;
        private float fltBasicSalaryPer;
        private List<AllDedRateDetail> lstAllDedRateDetailList;
        private string strUserID;
        private string strSaveType;

        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        
        public int Occupation
        {
            set { intOccupation = value; }
            get { return intOccupation; }
        }
        public string OccupLevel
        {
            set { strOccupLevel = value; }
            get { return strOccupLevel; }
        }
        public decimal BasicSalary
        {
            set { decBasicSalary = value; }
            get { return decBasicSalary; }
        }
        public string BasicSalaryPayPeriod
        {
            set { strBasicSalaryPayPeriod = value; }
            get { return strBasicSalaryPayPeriod; }
        }

        public decimal GrossSalary
        {
            set { decGrossSalary = value; }
            get { return decGrossSalary; }
        }
        public string GrossSalaryPayPeriod
        {
            set { strGrossSalaryPayPeriod = value; }
            get { return strGrossSalaryPayPeriod; }
        }
        public float BasicSalaryPer
        {
            set { fltBasicSalaryPer = value; }
            get { return fltBasicSalaryPer; }
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

        public List<AllDedRateDetail> AllDedRateDetailList
        {
            set { lstAllDedRateDetailList = value; }
            get { return lstAllDedRateDetailList; }
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
            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    SqlCommand dbCommand = null;
                    if (strSaveType.Trim() == "ADDS")
                    {
                        dbCommand = db.GetStoredProcCommand("AllDedRateAdd") as SqlCommand;
                        db.AddOutParameter(dbCommand, "TransNo", SqlDbType.VarChar, 10);
                    }
                    else if (strSaveType.Trim() == "EDIT")
                    {
                        dbCommand = db.GetStoredProcCommand("AllDedRateEdit") as SqlCommand;
                        db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar,strTransNo.Trim());
                    }
                    db.AddInParameter(dbCommand, "Occupation", SqlDbType.Int, intOccupation);
                    db.AddInParameter(dbCommand, "OccupLevel", SqlDbType.VarChar, strOccupLevel.Trim());
                    db.AddInParameter(dbCommand, "BasicSalary", SqlDbType.Decimal, decBasicSalary);
                    db.AddInParameter(dbCommand, "BasicSalaryPayPeriod", SqlDbType.VarChar, strBasicSalaryPayPeriod);
                    db.AddInParameter(dbCommand, "GrossSalary", SqlDbType.Decimal, decGrossSalary);
                    db.AddInParameter(dbCommand, "GrossSalaryPayPeriod", SqlDbType.VarChar, strGrossSalaryPayPeriod);
                    db.AddInParameter(dbCommand, "BasicSalaryPer", SqlDbType.Real, fltBasicSalaryPer);
                    db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    db.ExecuteNonQuery(dbCommand, transaction);
                    foreach (AllDedRateDetail oAllDedRateDetail in lstAllDedRateDetailList)
                    {
                        SqlCommand dbCommandAllDedRateDetail;
                        if (strSaveType.Trim() == "ADDS")
                        {
                            dbCommandAllDedRateDetail = oAllDedRateDetail.AddCommand(int.Parse(db.GetParameterValue(dbCommand, "TransNo").ToString()));
                            db.ExecuteNonQuery(dbCommandAllDedRateDetail, transaction);
                        }
                        else if (strSaveType.Trim() == "EDIT")
                        {
                            dbCommandAllDedRateDetail = oAllDedRateDetail.EditCommand(int.Parse(strTransNo));
                            db.ExecuteNonQuery(dbCommandAllDedRateDetail, transaction);

                        }
                    }
                    if (strSaveType.Trim() == "EDIT")
                    {
                        AllDedRateDetail oAllDedRateDetailSelect = new AllDedRateDetail();
                        AllDedRateDetail oAllDedRateDetailDelete = new AllDedRateDetail();
                        oAllDedRateDetailSelect.AllDedRate = int.Parse(strTransNo);
                        foreach (DataRow oRowDetail in oAllDedRateDetailSelect.GetAllDedRateDetailsByAllDedRate().Tables[0].Rows)
                        {
                            if (!ChkDetailContainInList(int.Parse(oRowDetail["AllDedRate"].ToString()), int.Parse(oRowDetail["AllDed"].ToString())))
                            {
                                oAllDedRateDetailDelete.AllDedRate =int.Parse(oRowDetail["AllDedRate"].ToString());
                                oAllDedRateDetailDelete.AllDed=int.Parse(oRowDetail["AllDed"].ToString());
                                SqlCommand dbCommandAllDedRateDetailDelete = oAllDedRateDetailDelete.DeleteByAllDedRateAndAllDedReturnCommand();
                                db.ExecuteNonQuery(dbCommandAllDedRateDetailDelete, transaction);
                            }
                        }
                    }
                    transaction.Commit();
                    enSaveStatus = DataGeneral.SaveStatus.Saved;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
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
                oCommand = db.GetStoredProcCommand("AllDedRateChkTransNoExist") as SqlCommand;
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

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllDedRateSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        
        #region Get 
        public bool GetAllDedRate()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllDedRateSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                intOccupation = int.Parse(thisRow[0]["Occupation"].ToString());
                strOccupLevel = thisRow[0]["OccLevel"].ToString();
                decBasicSalary = thisRow[0]["BasicSalary"] != null && thisRow[0]["BasicSalary"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["BasicSalary"].ToString()) : 0;
                strBasicSalaryPayPeriod = thisRow[0]["BasicSalaryPayPeriod"] != null ?  thisRow[0]["BasicSalaryPayPeriod"].ToString() : "";
                decGrossSalary = thisRow[0]["GrossSalary"] != null && thisRow[0]["GrossSalary"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["GrossSalary"].ToString()) : 0;
                strGrossSalaryPayPeriod = thisRow[0]["GrossSalaryPayPeriod"] != null ? thisRow[0]["GrossSalaryPayPeriod"].ToString() : "";
                fltBasicSalaryPer = thisRow[0]["BasicSalaryPer"] != null && thisRow[0]["BasicSalaryPer"].ToString().Trim() != "" ? float.Parse(thisRow[0]["BasicSalaryPer"].ToString()) : 0;

                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get AllDedRate By Occupation And OccupLevel
        public bool GetAllDedRateByOccupationOccupLevel()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllDedRateSelectByOccupationOccupLevel") as SqlCommand;
            db.AddInParameter(dbCommand, "Occupation", SqlDbType.Int, intOccupation);
            db.AddInParameter(dbCommand, "OccupLevel", SqlDbType.VarChar, strOccupLevel.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                intOccupation = int.Parse(thisRow[0]["Occupation"].ToString());
                strOccupLevel = thisRow[0]["OccLevel"].ToString();
                decBasicSalary = decimal.Parse(thisRow[0]["BasicSalary"].ToString());
                strBasicSalaryPayPeriod = thisRow[0]["BasicSalaryPayPeriod"].ToString();
                decGrossSalary = decimal.Parse(thisRow[0]["GrossSalary"] != null && thisRow[0]["GrossSalary"].ToString().Trim() != "" ? thisRow[0]["GrossSalary"].ToString() : "0");
                strGrossSalaryPayPeriod = thisRow[0]["GrossSalaryPayPeriod"] != null ? thisRow[0]["GrossSalaryPayPeriod"].ToString() : "";
                fltBasicSalaryPer = float.Parse(thisRow[0]["BasicSalaryPer"] != null && thisRow[0]["BasicSalaryPer"].ToString().Trim() != "" ? thisRow[0]["BasicSalaryPer"].ToString() : "0");
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Check Name Exist
        public bool ChkNameExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AllDedRateChkNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "Occupation", SqlDbType.Int, intOccupation);
            db.AddInParameter(oCommand, "OccupLevel", SqlDbType.VarChar, strOccupLevel.Trim());
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

        #region Delete
        public DataGeneral.SaveStatus Delete()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("OccupationDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Check If AllDedRate Details Is In The List
        public bool ChkDetailContainInList(int intAllDedRateValue, int intAllDedValue)
        {
            bool blnStatus = false;
            foreach (AllDedRateDetail oAllDedRateDetailsItem in lstAllDedRateDetailList)
            {
                if (oAllDedRateDetailsItem.AllDedRate == intAllDedRateValue && oAllDedRateDetailsItem.AllDed == intAllDedValue)
                {
                    blnStatus = true;
                    break;
                }
            }
            return blnStatus;
        }
        #endregion
    }
}
