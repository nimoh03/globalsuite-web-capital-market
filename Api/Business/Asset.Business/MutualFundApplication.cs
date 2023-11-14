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

namespace Asset.Business
{
    public class MutualFundApplication
    {
        #region Declaration
        private DateTime datEffDate, datDateOfBirth;
        private long lngTransNo, lngTransNoRev,lngMutualFund;
        private decimal decAmount;
        private bool blnResident, blnResident2, blnPosted, blnReversed;
        private string strProductId,strCustomerId, strMutualFundAccount, strApplicantType;
        private string strApplicantName, strApplicantName2, strOccupation, strOccupation2, strPhoneNumber, strPhoneNumber2;
        private string strNextOfKin, strNextOfKin2, strAddress, strAddress2, strEmail, strEmail2, strPassportNumber;
        private string strMotherMaidenName, strBankName, strBankName2, strBankName3, strCustomerAccount, strCustomerAccount2;
        private string strCompanyName, strTypeOfBusiness, strRCNo, strContactPerson, strInvestorCategory;
        private string strSaveType;
        #endregion

        #region Properties
        public long TransNo
        {
            set { lngTransNo = value; }
            get { return lngTransNo; }
        }
        public long TransNoRev
        {
            set { lngTransNoRev = value; }
            get { return lngTransNoRev; }
        }
        public long MutualFund
        {
            set { lngMutualFund = value; }
            get { return lngMutualFund; }
        }
        public DateTime EffDate
        {
            set { datEffDate = value; }
            get { return datEffDate; }
        }
        public DateTime DateOfBirth
        {
            set { datDateOfBirth = value; }
            get { return datDateOfBirth; }
        }
        public decimal Amount
        {
            set { decAmount = value; }
            get { return decAmount; }
        }
        public bool Resident
        {
            set { blnResident = value; }
            get { return blnResident; }
        }
        public bool Resident2
        {
            set { blnResident2 = value; }
            get { return blnResident2; }
        }
        public bool Posted
        {
            set { blnPosted = value; }
            get { return blnPosted; }
        }
        public bool Reversed
        {
            set { blnReversed = value; }
            get { return blnReversed; }
        }

        public string ProductId
        {
            set { strProductId = value; }
            get { return strProductId; }
        }
        public string CustomerId
        {
            set { strCustomerId = value; }
            get { return strCustomerId; }
        }
        public string MutualFundAccount
        {
            set { strMutualFundAccount = value; }
            get { return strMutualFundAccount; }
        }
        public string ApplicantType
        {
            set { strApplicantType = value; }
            get { return strApplicantType; }
        }
        public string ApplicantName
        {
            set { strApplicantName = value; }
            get { return strApplicantName; }
        }
        public string ApplicantName2
        {
            set { strApplicantName2 = value; }
            get { return strApplicantName2; }
        }
        public string Occupation
        {
            set { strOccupation = value; }
            get { return strOccupation; }
        }
        public string Occupation2
        {
            set { strOccupation2 = value; }
            get { return strOccupation2; }
        }
        public string PhoneNumber
        {
            set { strPhoneNumber = value; }
            get { return strPhoneNumber; }
        }
        public string PhoneNumber2
        {
            set { strPhoneNumber2 = value; }
            get { return strPhoneNumber2; }
        }
        public string NextOfKin
        {
            set { strNextOfKin = value; }
            get { return strNextOfKin; }
        }
        public string NextOfKin2
        {
            set { strNextOfKin2 = value; }
            get { return strNextOfKin2; }
        }
        public string Address
        {
            set { strAddress = value; }
            get { return strAddress; }
        }
        public string Address2
        {
            set { strAddress2 = value; }
            get { return strAddress2; }
        }
        public string Email
        {
            set { strEmail = value; }
            get { return strEmail; }
        }
        public string Email2
        {
            set { strEmail2 = value; }
            get { return strEmail2; }
        }
        public string PassportNumber
        {
            set { strPassportNumber = value; }
            get { return strPassportNumber; }
        }
        public string MotherMaidenName
        {
            set { strMotherMaidenName = value; }
            get { return strMotherMaidenName; }
        }
        public string BankName
        {
            set { strBankName = value; }
            get { return strBankName; }
        }
        public string BankName2
        {
            set { strBankName2 = value; }
            get { return strBankName2; }
        }
        public string BankName3
        {
            set { strBankName3 = value; }
            get { return strBankName3; }
        }
        public string CustomerAccount
        {
            set { strCustomerAccount = value; }
            get { return strCustomerAccount; }
        }
        public string CustomerAccount2
        {
            set { strCustomerAccount2 = value; }
            get { return strCustomerAccount2; }
        }
        public string CompanyName
        {
            set { strCompanyName = value; }
            get { return strCompanyName; }
        }
        public string TypeOfBusiness
        {
            set { strTypeOfBusiness = value; }
            get { return strTypeOfBusiness; }
        }
        public string RCNo
        {
            set { strRCNo = value; }
            get { return strRCNo; }
        }
        public string ContactPerson
        {
            set { strContactPerson = value; }
            get { return strContactPerson; }
        }
        public string InvestorCategory
        {
            set { strInvestorCategory = value; }
            get { return strInvestorCategory; }
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
            if (!ChkTransNoExist(DataGeneral.PostStatus.UnPosted))
            {
                enSaveStatus = DataGeneral.SaveStatus.NotExist;
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
                        dbCommand = db.GetStoredProcCommand("MutualFundApplicationAddNew") as SqlCommand;
                    }
                    else if (strSaveType.Trim() == "EDIT")
                    {
                        dbCommand = db.GetStoredProcCommand("MutualFundApplicationEdit") as SqlCommand;
                    }
                    db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, lngTransNo);
                    db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime, datEffDate);
                    db.AddInParameter(dbCommand, "DateOfBirth", SqlDbType.DateTime, datDateOfBirth != DateTime.MinValue ?  datDateOfBirth : SqlDateTime.Null);
                    db.AddInParameter(dbCommand, "MutualFund", SqlDbType.BigInt, lngMutualFund);
                    db.AddInParameter(dbCommand, "Amount", SqlDbType.Decimal, decAmount);
                    db.AddInParameter(dbCommand, "Resident", SqlDbType.Bit, blnResident);
                    db.AddInParameter(dbCommand, "Resident2", SqlDbType.Bit, blnResident2);
                    db.AddInParameter(dbCommand, "ProductId", SqlDbType.VarChar, strProductId);
                    db.AddInParameter(dbCommand, "CustomerId", SqlDbType.VarChar, strCustomerId);
                    db.AddInParameter(dbCommand, "MutualFundAccount", SqlDbType.VarChar, strMutualFundAccount);
                    db.AddInParameter(dbCommand, "ApplicantType", SqlDbType.VarChar, strApplicantType);
                    db.AddInParameter(dbCommand, "ApplicantName", SqlDbType.VarChar, strApplicantName);
                    db.AddInParameter(dbCommand, "ApplicantName2", SqlDbType.VarChar, strApplicantName2);
                    db.AddInParameter(dbCommand, "Occupation", SqlDbType.VarChar, strOccupation);
                    db.AddInParameter(dbCommand, "Occupation2", SqlDbType.VarChar, strOccupation2);
                    db.AddInParameter(dbCommand, "PhoneNumber", SqlDbType.VarChar, strPhoneNumber);
                    db.AddInParameter(dbCommand, "PhoneNumber2", SqlDbType.VarChar, strPhoneNumber2);
                    db.AddInParameter(dbCommand, "NextOfKin", SqlDbType.VarChar, strNextOfKin);
                    db.AddInParameter(dbCommand, "NextOfKin2", SqlDbType.VarChar, strNextOfKin2);
                    db.AddInParameter(dbCommand, "Address", SqlDbType.VarChar, strAddress);
                    db.AddInParameter(dbCommand, "Address2", SqlDbType.VarChar, strAddress2);
                    db.AddInParameter(dbCommand, "Email", SqlDbType.VarChar, strEmail);
                    db.AddInParameter(dbCommand, "Email2", SqlDbType.VarChar, strEmail2);
                    db.AddInParameter(dbCommand, "PassportNumber", SqlDbType.VarChar, strPassportNumber);
                    db.AddInParameter(dbCommand, "MotherMaidenName", SqlDbType.VarChar, strMotherMaidenName);
                    db.AddInParameter(dbCommand, "BankName", SqlDbType.VarChar, strBankName);
                    db.AddInParameter(dbCommand, "BankName2", SqlDbType.VarChar, strBankName2);
                    db.AddInParameter(dbCommand, "BankName3", SqlDbType.VarChar, strBankName3);
                    db.AddInParameter(dbCommand, "CustomerAccount", SqlDbType.VarChar, strCustomerAccount);
                    db.AddInParameter(dbCommand, "CustomerAccount2", SqlDbType.VarChar, strCustomerAccount2);
                    db.AddInParameter(dbCommand, "CompanyName", SqlDbType.VarChar, strCompanyName);
                    db.AddInParameter(dbCommand, "TypeOfBusiness", SqlDbType.VarChar, strTypeOfBusiness);
                    db.AddInParameter(dbCommand, "RCNo", SqlDbType.VarChar, strRCNo);
                    db.AddInParameter(dbCommand, "ContactPerson", SqlDbType.VarChar, strContactPerson);
                    db.AddInParameter(dbCommand, "InvestorCategory", SqlDbType.VarChar, strInvestorCategory);
                    db.AddInParameter(dbCommand, "Posted", SqlDbType.Bit, false);
                    db.AddInParameter(dbCommand, "Reversed", SqlDbType.Bit, false);
                    db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    db.ExecuteNonQuery(dbCommand, transaction);
                    if (lngTransNoRev != 0)
                    {
                        SqlCommand dbCommandDeleteReversal = DeleteReversalCommand();
                        db.ExecuteNonQuery(dbCommandDeleteReversal, transaction);
                    }
                    transaction.Commit();
                    enSaveStatus = DataGeneral.SaveStatus.Saved;
                }
                catch (Exception err)
                {
                    transaction.Rollback();
                    throw new Exception(err.Message);
                }
            }
            return enSaveStatus;
        }
        #endregion

        #region Check TransNo Exist
        public bool ChkTransNoExist(DataGeneral.PostStatus ePostStatus)
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = null;
                if (ePostStatus == DataGeneral.PostStatus.UnPosted)
                {
                    oCommand = db.GetStoredProcCommand("AllotmentChkTransNoExistUnPosted") as SqlCommand;
                }
                db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt,lngTransNo);
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


        
        #region Get MutualFund Application
        public bool GetMutualFundApplication(DataGeneral.PostStatus ePostStatus)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");

            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (ePostStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("MutualFundApplicationSelectUnPosted") as SqlCommand;
            }
            else if (ePostStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("MutualFundApplicationSelectPosted") as SqlCommand;
            }
            else if (ePostStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("MutualFundApplicationSelectReversed") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, lngTransNo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {

                datEffDate = DateTime.Parse(thisRow[0]["EffDate"].ToString());
                datDateOfBirth = DateTime.Parse(thisRow[0]["DateOfBirth"].ToString());
                lngMutualFund = long.Parse(thisRow[0]["MutualFund"].ToString());
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString());
                blnResident = bool.Parse(thisRow[0]["Resident"].ToString());
                blnResident2 = bool.Parse(thisRow[0]["Resident2"].ToString());
                strProductId = thisRow[0]["ProductId"].ToString();
                strCustomerId = thisRow[0]["CustomerId"].ToString();
                strMutualFundAccount = thisRow[0]["MutualFundAccount"].ToString();
                strApplicantType = thisRow[0]["ApplicantType"].ToString();
                strApplicantName = thisRow[0]["ApplicantName"].ToString();
                strApplicantName2 = thisRow[0]["ApplicantName2"].ToString();
                strOccupation = thisRow[0]["Occupation"].ToString();
                strOccupation2 = thisRow[0]["Occupation2"].ToString();
                strPhoneNumber = thisRow[0]["PhoneNumber"].ToString();
                strPhoneNumber2 = thisRow[0]["PhoneNumber2"].ToString();
                strNextOfKin = thisRow[0]["NextOfKin"].ToString();
                strNextOfKin2 = thisRow[0]["NextOfKin2"].ToString();
                strAddress = thisRow[0]["Address"].ToString();
                strAddress2 = thisRow[0]["Address2"].ToString();
                strEmail = thisRow[0]["Email"].ToString();
                strEmail2 = thisRow[0]["Email2"].ToString();
                strPassportNumber = thisRow[0]["PassportNumber"].ToString();
                strMotherMaidenName = thisRow[0]["MotherMaidenName"].ToString();
                strBankName = thisRow[0]["BankName"].ToString();
                strBankName2 = thisRow[0]["BankName2"].ToString();
                strBankName3 = thisRow[0]["BankName3"].ToString();
                strCustomerAccount = thisRow[0]["CustomerAccount"].ToString();
                strCustomerAccount2 = thisRow[0]["CustomerAccount2"].ToString();
                strCompanyName = thisRow[0]["CompanyName"].ToString();
                strTypeOfBusiness = thisRow[0]["TypeOfBusiness"].ToString();
                strRCNo = thisRow[0]["RCNo"].ToString();
                strContactPerson = thisRow[0]["ContactPerson"].ToString();
                strInvestorCategory = thisRow[0]["InvestorCategory"].ToString();
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
        public DataSet GetAll(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("MutualFundApplicationSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("MutualFundApplicationSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("MutualFundApplicationSelectAllReversed") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("MutualFundApplicationSelectAllPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("MutualFundApplicationSelectAllUnPostedAsc") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("MutualFundApplicationDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, lngTransNo);
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Get Total Number Of Returns For A Particular MutualFund Offer Real
        public decimal GetTotalMutualFundApp()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("MutualFundApplicationSelectByMutualFundCodeReturnSumAmount") as SqlCommand;
            db.AddInParameter(dbCommand, "MutualFund", SqlDbType.BigInt,lngMutualFund);
            db.AddOutParameter(dbCommand, "TotalAmountApplied", SqlDbType.Money, 8);
            db.ExecuteNonQuery(dbCommand);
            return decimal.Parse(db.GetParameterValue(dbCommand, "TotalAmountApplied").ToString());
        }
        #endregion

        #region Delete Reversal and Return A Command
        public SqlCommand DeleteReversalCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("MutualFundApplicationDeleteReversal") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.BigInt, lngTransNoRev);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.ToUpper());
            return oCommand;
        }
        #endregion
    }
}
