using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace CustomerManagement.Business
{
    public class ContactPerson
    {
        #region Properties
        public Int64 ContactPersonId { get; set; }
        public Int64 CustomerId { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string SaveType { get; set; }
        #endregion

        #region Save And Return Command
        public SqlCommand SaveCommand()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (SaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("ContactPersonAdd") as SqlCommand;
            }
            else if (SaveType == "EDIT")
            {
                if (ContactPersonId != 0)
                {
                    oCommand = db.GetStoredProcCommand("ContactPersonEdit") as SqlCommand;
                }
                else
                {
                    oCommand = db.GetStoredProcCommand("ContactPersonAdd") as SqlCommand;
                }
            }
            db.AddInParameter(oCommand, "ContactPersonId", SqlDbType.BigInt, ContactPersonId);
            db.AddInParameter(oCommand, "CustomerId", SqlDbType.BigInt, CustomerId);
            db.AddInParameter(oCommand, "ContactName", SqlDbType.VarChar, ContactName.Trim());
            db.AddInParameter(oCommand, "Phone", SqlDbType.VarChar, Phone.Trim());
            db.AddInParameter(oCommand, "Email", SqlDbType.VarChar, Email.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Get
        public DataSet GetContactPersonByCustomerId()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ContactPersonSelectByCustomerId") as SqlCommand;
            db.AddInParameter(dbCommand, "CustomerId", SqlDbType.BigInt, CustomerId);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion
    }
}
