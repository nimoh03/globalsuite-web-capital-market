using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace BaseUtility.Business
{
    public class SMSEngine
    {
        #region Declaration

        //SMS
        //private string SenderUrl = "http://www.integration.defturesms.com/addon/smsservice/sendsms.aspx?";
        private string SenderUrl = "https://app.multitexter.com/v2/app/sms?";
        private string strSendingSMSText;

        //Email
        private string strBodyText, strSubject, strFileName;

        #endregion

        #region Properties

        
        //SMS
        
        public string SendingSMSText
        {
            set { strSendingSMSText = value; }
            get { return strSendingSMSText; }
        }
       

        //Email
        public string BodyText
        {
            set { strBodyText = value; }
            get { return strBodyText; }
        }
        public string Subject
        {
            set { strSubject = value; }
            get { return strSubject; }
        }
        public string FileName
        {
            set { strFileName = value; }
            get { return strFileName; }
        }
        
        #endregion

        #region Send SMS Message
        public void SendSMSMessage(string strCustomerSMS)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("SMSAlert") as SqlCommand;
            db.AddInParameter(dbCommand, "SenderUrl", SqlDbType.NVarChar, SenderUrl);
            db.AddInParameter(dbCommand, "SendingSMSText", SqlDbType.NVarChar, strSendingSMSText);
            db.AddInParameter(dbCommand, "CustomerSMS", SqlDbType.NVarChar, strCustomerSMS);
            db.AddOutParameter(dbCommand, "SMSReturnMessage", SqlDbType.NVarChar, 20);
            db.ExecuteNonQuery(dbCommand);
            string SMSStatusResult = db.GetParameterValue(dbCommand, "SMSReturnMessage").ToString();
        }
        #endregion

        #region Send Email
        public void SendEmail(string strCustomerEmail)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("EmailAlert") as SqlCommand;
            db.AddInParameter(dbCommand, "Subject", SqlDbType.VarChar, strSubject.Trim());
            db.AddInParameter(dbCommand, "FileName", SqlDbType.VarChar, strFileName);
            db.AddInParameter(dbCommand, "BodyText", SqlDbType.VarChar, BodyText);
            db.AddInParameter(dbCommand, "CustomerEmail", SqlDbType.VarChar, strCustomerEmail);
            db.ExecuteNonQuery(dbCommand);
        }
        #endregion

        #region Send Email In Code
        public void SendEmailInCode(string strUserEmail,string strUserFullName,string strMessage,string strSubject)
        {
            try
            {
                SmtpClient mySmtpClient = new SmtpClient("webmail.globalseam.com");

                // set smtp-client with basicAuthentication
                mySmtpClient.UseDefaultCredentials = false;
                System.Net.NetworkCredential basicAuthenticationInfo = new
                   System.Net.NetworkCredential("seun@globalseam.com", @"Ayobami1974@@");
                mySmtpClient.Credentials = basicAuthenticationInfo;

                // add from,to mailaddresses
                MailAddress from = new MailAddress("seun@globalseam.com", "TestFromName");
                MailAddress to = new MailAddress(strUserEmail, strUserFullName);
                MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

                // add ReplyTo
                MailAddress replyTo = new MailAddress("seun@globalseam.com");
                myMail.ReplyToList.Add(replyTo);

                // set subject and encoding
                myMail.Subject = strSubject;
                myMail.SubjectEncoding = System.Text.Encoding.UTF8;

                // set body-message and encoding
                myMail.Body = "<b>" + strMessage + "</b>.";
                myMail.BodyEncoding = System.Text.Encoding.UTF8;
                // text or html
                myMail.IsBodyHtml = true;

                mySmtpClient.Send(myMail);
            }

            catch (SmtpException ex)
            {
                throw new ApplicationException
                  ("SmtpException has occured: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Get Successful Message
        public DataSet GetSuccessfullMessage(DateTime datDateFrom,DateTime datDateTo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("EmailSelectSuccessful") as SqlCommand;
            db.AddInParameter(dbCommand, "DateFrom", SqlDbType.DateTime, datDateFrom);
            db.AddInParameter(dbCommand, "DateTo", SqlDbType.DateTime, datDateTo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Unsent Message
        public DataSet GetUnsentMessage(DateTime datDateFrom, DateTime datDateTo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("EmailSelectUnsent") as SqlCommand;
            db.AddInParameter(dbCommand, "DateFrom", SqlDbType.DateTime, datDateFrom);
            db.AddInParameter(dbCommand, "DateTo", SqlDbType.DateTime, datDateTo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Successful Message
        public DataSet GetFailedMessage(DateTime datDateFrom, DateTime datDateTo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("EmailSelectFailed") as SqlCommand;
            db.AddInParameter(dbCommand, "DateFrom", SqlDbType.DateTime, datDateFrom);
            db.AddInParameter(dbCommand, "DateTo", SqlDbType.DateTime, datDateTo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion


    }
}
