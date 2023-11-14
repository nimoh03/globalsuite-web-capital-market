using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using BaseUtility.Business;

namespace CapitalMarket.Business
{
    public class DematCertificate
    {
        #region Declarations
        public string MainCertficateNo { set; get; }
        public Int64 DematCertificateNo { set; get; }
        public Int64 Quantity { set; get; }
        public string CertAccountNo { set; get; }
        public string CertRegNo { set; get; }
        public string SaveType { set; get; }
        #endregion

        

        #region Save And Return Command
        public SqlCommand SaveCommand()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (SaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("DematCertificateAdd") as SqlCommand;
            }
            else if (SaveType == "EDIT")
            {
                if (DematCertificateNo != 0)
                {
                    oCommand = db.GetStoredProcCommand("DematCertificateEdit") as SqlCommand;
                }
                else
                {
                    oCommand = db.GetStoredProcCommand("DematCertificateAdd") as SqlCommand;
                }
            }
            db.AddInParameter(oCommand, "DematCertificateNo", SqlDbType.BigInt, DematCertificateNo);
            db.AddInParameter(oCommand, "MainCertficateNo", SqlDbType.VarChar, MainCertficateNo);
            db.AddInParameter(oCommand, "Quantity", SqlDbType.BigInt, Quantity);
            db.AddInParameter(oCommand, "CertAccountNo", SqlDbType.VarChar, CertAccountNo.Trim());
            db.AddInParameter(oCommand, "CertRegNo", SqlDbType.VarChar, CertRegNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Get
        public DataSet GetDematCertificateByMainCertificateNo()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DematCertificateSelectByMainCertificateNo") as SqlCommand;
            db.AddInParameter(dbCommand, "MainCertficateNo", SqlDbType.VarChar, MainCertficateNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion
    }
}
