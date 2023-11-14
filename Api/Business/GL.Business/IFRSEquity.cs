using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class IFRSEquity
    {
        #region Declaration
        private Int64 intTransNo,intIFRSItem;
        private string strEquityItem;
        private decimal decAmount;
        #endregion

        #region Properties
        public Int64 TransNo
        {
            set { intTransNo = value; }
            get { return intTransNo; }
        }
        public Int64 IFRSItem
        {
            set { intIFRSItem = value; }
            get { return intIFRSItem; }
        }
        public string EquityItem
        {
            set { strEquityItem = value; }
            get { return strEquityItem; }
        }
        public decimal Amount
        {
            set { decAmount = value; }
            get { return decAmount; }
        }
        #endregion

        #region Delete All
        public void DeleteAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("IFRSEquityDeleteAll") as SqlCommand;
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        #region Save Each Amount
        public void SaveEachAmount()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("IFRSEquityAddNew") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, 0);
            db.AddInParameter(oCommand, "IFRSItem", SqlDbType.BigInt, intIFRSItem);
            db.AddInParameter(oCommand, "EquityItem", SqlDbType.VarChar, strEquityItem);
            db.AddInParameter(oCommand, "Amount", SqlDbType.Money, decAmount);
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        #region Save All Amount
        public void SaveAllAmount(string strIFRSReportPeriod, DateTime datEntryDate, DateTime datFirstDatePrevious, DateTime datFirstDatePreviousPrevious, DateTime datPrevious)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            Account oAccount = new Account();
            GLParam oGLParam = new GLParam();
            DataSet dsChildren;
            DataSet dsEquityItem;
            bool blnAsAtStatusFlag;

            DeleteAll();
            dsEquityItem = oGLParam.GetAllOrderByCode("IFRSEQUITY");
            
                IFRSAnnual oIFRSAnnual = new IFRSAnnual();
                oIFRSAnnual.ReportType = "SOCIE";
                dsChildren = oIFRSAnnual.GetAllChild();
            

            foreach (DataRow oRow in dsChildren.Tables[0].Rows)
            {
                foreach (DataRow oRowEquityItem in dsEquityItem.Tables[0].Rows)
                {
                    decAmount = 0;
                    blnAsAtStatusFlag = oRowEquityItem["Code"].ToString().Trim() == "1"
                                        || oRowEquityItem["Code"].ToString().Trim() == "5" 
                                        || oRowEquityItem["Code"].ToString().Trim() == "7" 
                                        || oRowEquityItem["Code"].ToString().Trim() == "11" ? true : false;
                    intIFRSItem = long.Parse(oRow["TransNo"].ToString());
                    strEquityItem = oRowEquityItem["Code"].ToString();
                    if (strIFRSReportPeriod.Trim() == "ANNUAL")
                    {
                        oAccount.SOCIEAnnual = long.Parse(oRow["TransNo"].ToString());

                        if (oRowEquityItem["Code"].ToString().Trim() == "2" || oRowEquityItem["Code"].ToString().Trim() == "3" ||
                            oRowEquityItem["Code"].ToString().Trim() == "4")
                        {
                            if ((oRow["ReportPosition2"].ToString().Trim() != "1") && oRowEquityItem["Code"].ToString().Trim() == "4")
                            { decAmount = 0;}
                            else if ((oRow["ReportPosition2"].ToString().Trim() != "2") && oRowEquityItem["Code"].ToString().Trim() == "3")
                            { decAmount = 0;}
                            else if ((oRow["ReportPosition2"].ToString().Trim() != "3" && oRow["ReportPosition2"].ToString().Trim() != "4") && oRowEquityItem["Code"].ToString().Trim() == "2")
                            { decAmount = 0; }
                            else
                            {
                                decAmount = oAccount.GetTotalAccountBalancesGivenSOCIE(datFirstDatePrevious.AddDays(1), datEntryDate, blnAsAtStatusFlag);
                            }
                        }
                        else if (oRowEquityItem["Code"].ToString().Trim() == "8" || oRowEquityItem["Code"].ToString().Trim() == "9" ||
                            oRowEquityItem["Code"].ToString().Trim() == "10")
                        {
                            if ((oRow["ReportPosition2"].ToString().Trim() != "1") && oRowEquityItem["Code"].ToString().Trim() == "10")
                            { decAmount = 0;}
                            else if ((oRow["ReportPosition2"].ToString().Trim() != "2") && oRowEquityItem["Code"].ToString().Trim() == "9")
                            { decAmount = 0;}
                            else if ((oRow["ReportPosition2"].ToString().Trim() != "3" && oRow["ReportPosition2"].ToString().Trim() != "4") && oRowEquityItem["Code"].ToString().Trim() == "8")
                             { decAmount = 0;}
                             else
                             {
                                 decAmount = oAccount.GetTotalAccountBalancesGivenSOCIE(datFirstDatePreviousPrevious.AddDays(1), datPrevious, blnAsAtStatusFlag);
                             }
                        }
                        else if (oRowEquityItem["Code"].ToString().Trim() == "1" || oRowEquityItem["Code"].ToString().Trim() == "5" ||
                            oRowEquityItem["Code"].ToString().Trim() == "7" || oRowEquityItem["Code"].ToString().Trim() == "11")
                        {
                            if (oRowEquityItem["Code"].ToString().Trim() == "1")
                            {
                                decAmount = oAccount.GetTotalAccountBalancesGivenSOCIE(datFirstDatePrevious, datFirstDatePrevious, blnAsAtStatusFlag);
                            }
                            else if (oRowEquityItem["Code"].ToString().Trim() == "5")
                            {
                                decAmount = oAccount.GetTotalAccountBalancesGivenSOCIE(datEntryDate, datEntryDate, blnAsAtStatusFlag);
                            }
                            else if (oRowEquityItem["Code"].ToString().Trim() == "7")
                            {
                                decAmount = oAccount.GetTotalAccountBalancesGivenSOCIE(datFirstDatePreviousPrevious, datFirstDatePreviousPrevious, blnAsAtStatusFlag);
                            }
                            else
                            {
                                decAmount = oAccount.GetTotalAccountBalancesGivenSOCIE(datPrevious, datPrevious, blnAsAtStatusFlag);
                            }
                        }
                    }
                    SaveEachAmount();
                }
            }

        }
        #endregion
    }
}
