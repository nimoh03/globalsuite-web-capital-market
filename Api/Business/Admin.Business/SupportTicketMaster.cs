using System;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace Admin.Business
{
    public class SupportTicketMaster
    {
        #region Properties
        public Int64 SupportTicketMasterId { get; set; }
        public string IssueTitle { get; set; }
        public string IssueModule { get; set; }
        public bool IssueReplied { get; set; }
        public string SaveType { get; set; }
        #endregion

        #region Save Return Command
        public SqlCommand SaveCommand()
        {
            if (!ChkSupportTicketMasterIdExist())
            {
                throw new Exception("Issue Master Id Does Not Exist");
            }
            
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (SaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("SupportTicketMasterAdd") as SqlCommand;
            }
            else if (SaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("SupportTicketMasterEdit") as SqlCommand;
            }
            db.AddInParameter(oCommand, "SupportTicketMasterId", SqlDbType.BigInt, SupportTicketMasterId);
            db.AddInParameter(oCommand, "IssueTitle", SqlDbType.NVarChar, IssueTitle.Trim().ToUpper());
            db.AddInParameter(oCommand, "IssueModule", SqlDbType.VarChar, IssueModule.Trim());
            db.AddInParameter(oCommand, "IssueReplied", SqlDbType.Bit, IssueReplied);
            db.AddInParameter(oCommand, "UserId", SqlDbType.NVarChar, GeneralFunc.UserName);
            return oCommand;
        }
        #endregion

        #region Check Support Ticket Master Id Exist
        public bool ChkSupportTicketMasterIdExist()
        {
            bool blnStatus = false;
            if (SaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = null;
                oCommand = db.GetStoredProcCommand("SupportTicketMasterChkTransNoExist") as SqlCommand;
                db.AddInParameter(oCommand, "Transno", SqlDbType.BigInt, SupportTicketMasterId);
                DataSet oDs = db.ExecuteDataSet(oCommand);
                if (oDs.Tables[0].Rows.Count > 0)
                {
                    blnStatus = true;
                }
            }
            else if (SaveType == "ADDS")
            {
                blnStatus = true;
            }

            return blnStatus;
        }
        #endregion

    }
}
