using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace CapitalMarket.Business
{
    public class FIXReconcile
    {
        
        #region Save Auto Date With GL Reconcile and Return A Command
        public SqlCommand SaveAutoDateWithGLReconcileCommand(DateTime datAutoDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("FIXReconcileAutoDateWithGLReconcileSave") as SqlCommand;
            db.AddInParameter(oCommand, "AutoDate", SqlDbType.DateTime, datAutoDate);
            return oCommand;
        }
        #endregion

        #region Save Auto Date With Allotment Reconcile and Return A Command
        public SqlCommand SaveAutoDateWithAllotmentReconcileCommand(DateTime datAutoDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("FIXReconcileAutoDateWithAllotmentReconcileSave") as SqlCommand;
            db.AddInParameter(oCommand, "AutoDate", SqlDbType.DateTime, datAutoDate);
            return oCommand;
        }
        #endregion

        #region Save Auto Date With Portfolio Reconcile and Return A Command
        public SqlCommand SaveAutoDateWithPortfolioReconcileCommand(DateTime datAutoDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("FIXReconcileAutoDateWithPortfolioReconcileSave") as SqlCommand;
            db.AddInParameter(oCommand, "AutoDate", SqlDbType.DateTime, datAutoDate);
            return oCommand;
        }
        #endregion

        #region Save Auto Date With Trade File Reconcile and Return A Command
        public SqlCommand SaveAutoDateWithTradeFileReconcileCommand(DateTime datAutoDate,int intNumberOfFIXTrade,int intNumberOfFileTrade,int intNumberOfTradeInserted)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("FIXReconcileAutoDateWithTradeFileReconcileSave") as SqlCommand;
            db.AddInParameter(oCommand, "AutoDate", SqlDbType.DateTime, datAutoDate);
            db.AddInParameter(oCommand, "NumberOfFIXTrade", SqlDbType.Int, intNumberOfFIXTrade);
            db.AddInParameter(oCommand, "NumberOfFileTrade", SqlDbType.Int, intNumberOfFileTrade);
            db.AddInParameter(oCommand, "NumberOfTradeInserted", SqlDbType.Int, intNumberOfTradeInserted);
            return oCommand;
        }
        #endregion

        #region Check GL Reconcile
        public bool ChkGLReconcile(DateTime datAutoDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("FIXReconcileChkGLReconcile") as SqlCommand;
            db.AddInParameter(oCommand, "AutoDate", SqlDbType.DateTime, datAutoDate);
            var varResult = db.ExecuteScalar(oCommand);
            return varResult != null && varResult.ToString().Trim() != "" ? bool.Parse(varResult.ToString()) : false;
        }
        #endregion

        #region Check Allotment Reconcile
        public bool ChkAllotmentReconcile(DateTime datAutoDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("FIXReconcileChkAllotmentReconcile") as SqlCommand;
            db.AddInParameter(oCommand, "AutoDate", SqlDbType.DateTime, datAutoDate);
            var varResult = db.ExecuteScalar(oCommand);
            return varResult != null && varResult.ToString().Trim() != "" ? bool.Parse(varResult.ToString()) : false;
        }
        #endregion

        #region Check Portfolio Reconcile
        public bool ChkPortfolioReconcile(DateTime datAutoDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("FIXReconcileChkPortfolioReconcile") as SqlCommand;
            db.AddInParameter(oCommand, "AutoDate", SqlDbType.DateTime, datAutoDate);
            var varResult = db.ExecuteScalar(oCommand);
            return varResult != null && varResult.ToString().Trim() != "" ? bool.Parse(varResult.ToString()) : false;
        }
        #endregion

        #region Check Trade File Reconcile
        public bool ChkTradeFileReconcile(DateTime datAutoDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("FIXReconcileChkTradeFileReconcile") as SqlCommand;
            db.AddInParameter(oCommand, "AutoDate", SqlDbType.DateTime, datAutoDate);
            var varResult = db.ExecuteScalar(oCommand);
            return varResult != null && varResult.ToString().Trim() != "" ? bool.Parse(varResult.ToString()) : false;
        }
        #endregion

        #region Get All Reconcile
        public DataSet GetAllReconcile(DateTime datAutoDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("FIXReconcileGetAllReconcile") as SqlCommand;
            db.AddInParameter(oCommand, "AutoDate", SqlDbType.DateTime, datAutoDate);
            return db.ExecuteDataSet(oCommand);
        }
        #endregion

        #region Get FIX Date In Acct_GL,Stkb_Sold,Stkb_Portfolio Not In FIX Reconcile And/Or Trade File Not Reconcile
        public DataSet GetFIXTransactionNotInFIXTradeFileReconcile()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("FIXReconcileSelectFIXTransNotInFIXTradeFileReconcile") as SqlCommand;
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion
    }
}
