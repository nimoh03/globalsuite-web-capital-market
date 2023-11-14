using System;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace Admin.Business
{
    public class SupportTicket
    {
        #region Properties
        public Int64 SupportTicketId { get; set; }
        public Int64 SupportTicketMasterId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string IssueDescription { get; set; }
        public string SaveType { get; set; }
        #endregion

        #region Save Return Command
        public SqlCommand SaveCommand()
        {
            if (!ChkSupportTicketIdExist())
            {
                throw new Exception("Issue Id Does Not Exist");
            }

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (SaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("SupportTicketAdd") as SqlCommand;
            }
            else if (SaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("SupportTicketEdit") as SqlCommand;
            }
            db.AddInParameter(oCommand, "SupportTicketMasterId", SqlDbType.BigInt, SupportTicketMasterId);
            db.AddInParameter(oCommand, "SupportTicketId", SqlDbType.BigInt, SupportTicketId);
            db.AddInParameter(oCommand, "TransactionDate", SqlDbType.DateTime, TransactionDate);
            db.AddInParameter(oCommand, "IssueDescription", SqlDbType.NVarChar, IssueDescription);
            db.AddInParameter(oCommand, "UserId", SqlDbType.NVarChar, GeneralFunc.UserName);
            return oCommand;
        }
        #endregion

        #region Check Support Ticket Id Exist
        public bool ChkSupportTicketIdExist()
        {
            bool blnStatus = false;
            if (SaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = null;
                oCommand = db.GetStoredProcCommand("SupportTicketChkTransNoExist") as SqlCommand;
                db.AddInParameter(oCommand, "Transno", SqlDbType.BigInt, SupportTicketId);
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
