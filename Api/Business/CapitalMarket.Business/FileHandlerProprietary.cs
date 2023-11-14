using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Globalization;
using System.Configuration;
using System.Linq;
using GL.Business;

namespace CapitalMarket.Business
{
    public class FileHandlerProprietary
    {
        #region Declartion
        private DataSet datBargainSlipDataSet;
        public string ErrorMessaging { get; set; }
        #endregion

        #region Properties
        public DataSet BargainSlipDataSet
        {
            get { return datBargainSlipDataSet; }
        }
        #endregion

        #region Reading Text File
        public bool ReadTextFile(string filePath, out List<string> oCustMissings, out List<string> oStockMissings, out List<string> oBrokerMissings)
        {
            bool blnStatus = false;
            oCustMissings = null;
            oStockMissings = null;
            oBrokerMissings = null;
            IFormatProvider format = new CultureInfo("en-GB");
            try
            {
                List<TradeFile> oTradeFiles = new List<TradeFile>();
                using (StreamReader sr = File.OpenText(filePath))
                {
                    string strline;
                    while ((strline = sr.ReadLine()) != null)
                    {
                        TradeFile oTradeFile = new TradeFile();
                        oTradeFile.BuySell = strline.Trim().Substring(8, 2).Trim();
                        string strDiskDate = strline.Trim().Substring(10, 9).Trim();
                        oTradeFile.TransactionDate = DateTime.ParseExact(strDiskDate.Substring(6, 2) + "/" + strDiskDate.Substring(4, 2) + "/" + strDiskDate.Substring(0, 4), "dd/MM/yyyy", format);
                        oTradeFile.SecurityName = strline.Trim().Substring(19, 17).Trim();
                        oTradeFile.Quantity = long.Parse(strline.Trim().Substring(36, 16).Trim());
                        oTradeFile.Price = decimal.Parse(strline.Trim().Substring(52, 14).Trim());
                        oTradeFile.Consideration = oTradeFile.Quantity * oTradeFile.Price;
                        oTradeFile.CscsAccount = strline.Trim().Substring(66, 10).Trim();
                        oTradeFile.TicketNumber = strline.Trim().Substring(121, 10).Trim();
                        oTradeFile.CounterParty = strline.Trim().Substring(116, 5).Trim();
                        oTradeFile.CustomerName = strline.Trim().Substring(76, 38).Trim();
                        oTradeFiles.Add(oTradeFile);
                    }
                }

                ProductAcct oProductAcct = new ProductAcct();
                Stock oStock = new Stock();
                Broker oBroker = new Broker();

                StkParam oStkParam = new StkParam();
                BargainTransProp oBargainTransProp = new BargainTransProp();
                BargainSlipProp oBargainSlipProp = new BargainSlipProp();
                oBargainTransProp.DeleteAll();
                oBargainSlipProp.DeleteAll();

                IEnumerable<string> oFileTradeCscsAccounts = from s in oTradeFiles
                                                             select s.CscsAccount;
                IEnumerable<string> oProductAccts = oProductAcct.GetAllByProduct(oStkParam.Product, "ALL").Tables[0].AsEnumerable().Select(dataRow => dataRow.Field<string>("CsCs Account").Trim());
                IEnumerable<string> oProductAcctMissings = oFileTradeCscsAccounts.Except(oProductAccts);
                IEnumerable<CustomerDetail> oMissingCustomerDetails = from s in oProductAcctMissings
                                                                      join t in oTradeFiles on s equals t.CscsAccount
                                                                      select new CustomerDetail
                                                                      {
                                                                          CscsAccount = s,
                                                                          CustomerName = t.CustomerName
                                                                      };
                oCustMissings = (from s in oMissingCustomerDetails
                                 select s.CscsAccount + " " + s.CustomerName).ToList();

                IEnumerable<string> oFileTradeBroker = from s in oTradeFiles
                                                       select s.CounterParty;
                IEnumerable<string> oBrokers = oBroker.GetAll().Tables[0].AsEnumerable().Select(dataRow => dataRow.Field<string>("Code"));
                oBrokerMissings = oFileTradeBroker.Except(oBrokers).ToList();

                IEnumerable<string> oFileTradeSecurityNames = from s in oTradeFiles
                                                              select s.SecurityName;
                IEnumerable<string> oStockCodes = oStock.GetAll().Tables[0].AsEnumerable().Select(dataRow => dataRow.Field<string>("Seccode"));
                oStockMissings = oFileTradeSecurityNames.Except(oStockCodes).ToList();

                if (oCustMissings.Count > 0 || oBrokerMissings.Count > 0 || oStockMissings.Count > 0)
                {
                    return blnStatus;
                }

                oProductAcct.ProductCode = oStkParam.Product;
                Company oCompany = new Company();
                foreach (TradeFile oTradeFileRow in oTradeFiles)
                {
                    oBargainTransProp.Bslip = oTradeFileRow.TicketNumber;
                    oBargainTransProp.iDate = oTradeFileRow.TransactionDate;
                    oBargainTransProp.StockCode = oTradeFileRow.SecurityName;
                    oBargainTransProp.Units = oTradeFileRow.Quantity;
                    oBargainTransProp.UnitPrice = oTradeFileRow.Price;
                    oBargainTransProp.Consideration = oTradeFileRow.Consideration;
                    oBargainTransProp.StockType = "3";
                    if (oTradeFileRow.BuySell == "1")
                    {
                        oBargainTransProp.BoughtBy = oCompany.MemberCode;
                        oBargainTransProp.Buy_Sold_Ind = "B";
                        oBargainTransProp.SoldBy = oTradeFileRow.CounterParty;
                    }
                    else if (oTradeFileRow.BuySell == "2")
                    {
                        oBargainTransProp.SoldBy = oCompany.MemberCode;
                        oBargainTransProp.Buy_Sold_Ind = "S";
                        oBargainTransProp.BoughtBy = oTradeFileRow.CounterParty;
                    }
                    
                    oBargainTransProp.CrossD = "N";
                    
                    oBargainTransProp.AcctNo = oTradeFileRow.CscsAccount;
                    oProductAcct.CsCsAcct = oTradeFileRow.CscsAccount;
                    oBargainTransProp.CustNo = oProductAcct.GetCustNoGivenCsCsAcct().Trim();
                    oBargainTransProp.Add();
                }

                oBargainTransProp.SaveToBargainSlipProp();
                //oBargainTransProp.SaveToBargainSlipPropCrossDeal();
                //oBargainSlip.ProcessCrossDeal();
                //oBargainSlip.ProcessUnEqualCrossDeal();
                //oBargainSlip.UpdateCrossType();
                //if (!oBargainSlip.CheckCrossDealProcess())
                //{
                //    throw new Exception("Cross Deal Transactions Were Not Processed Properly, Please Check");
                //}
                datBargainSlipDataSet = oBargainSlipProp.GetAll();
                blnStatus = true;
            }
            catch (Exception err)
            {
                ErrorMessaging = err.Message;
            }
            return blnStatus;
        }
        #endregion

        #region Reading Text File Remote
        public bool ReadTextFileRemote(string filePath, out List<string> oCustMissings, out List<string> oStockMissings, out List<string> oBrokerMissings)
        {
            bool blnStatus = false;
            oCustMissings = null;
            oStockMissings = null;
            oBrokerMissings = null;
            IFormatProvider format = new CultureInfo("en-GB");
            try
            {
                Company oCompany = new Company();
                List<TradeFile> oTradeFiles = new List<TradeFile>();
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string strline;
                    string[] details = null;
                    char[] removedata = new char[] { '"' };
                    while ((strline = sr.ReadLine()) != null)
                    {
                        if (strline != "")
                        {
                            details = strline.Split(',');
                            if (details[0] == null || details[0].Trim(removedata) == "Board")
                            { }
                            else
                            {

                                TradeFile oTradeFile = new TradeFile();
                                //string strDiskDate = details[7].Trim(removedata).Substring(0, 10);
                                //oTradeFile.TransactionDate = DateTime.ParseExact(strDiskDate.Substring(6, 2) + "/" + strDiskDate.Substring(4, 2) + "/" + strDiskDate.Substring(0, 4), "dd/MM/yyyy", format);
                                //oTradeFile.TransactionDate = DateTime.ParseExact(strDiskDate.Substring(3, 2) + "/" + strDiskDate.Substring(0, 2) + "/" + strDiskDate.Substring(6, 4), "dd/MM/yyyy", format);
                                string strDiskDate = details[7].Trim(removedata).Substring(0, 8);
                                oTradeFile.TransactionDate = DateTime.ParseExact(strDiskDate.Substring(6, 2) + "/" + strDiskDate.Substring(4, 2) + "/" + strDiskDate.Substring(0, 4), "dd/MM/yyyy", format);
                                oTradeFile.SecurityName = details[1].Trim(removedata);
                                oTradeFile.Quantity = Decimal.ToInt64(Decimal.Parse(details[11].Trim(removedata)));
                                oTradeFile.Price = Decimal.Parse(details[10].Trim(removedata));
                                oTradeFile.Consideration = Decimal.ToInt64(Decimal.Parse(details[11].Trim(removedata))) * Decimal.Parse(details[10].Trim(removedata));
                                if (details[3] != null && details[3].Trim(removedata).Trim() != "" &&
                                    (details[5] == null || details[5].Trim(removedata).Trim() == ""))
                                {
                                    oTradeFile.TicketNumber = details[8].Trim(removedata);
                                    oTradeFile.BuySell = "B";
                                    oTradeFile.CounterParty = "GLOBALSEAM";
                                    oTradeFile.CscsAccount = details[3].Trim(removedata).TrimStart('0');
                                }
                                else if (details[5] != null && details[5].Trim(removedata).Trim() != "" &&
                                    (details[3] == null || details[3].Trim(removedata).Trim() == ""))
                                {
                                    oTradeFile.TicketNumber = details[9].Trim(removedata);
                                    oTradeFile.CounterParty = "GLOBALSEAM";
                                    oTradeFile.BuySell = "S";
                                    oTradeFile.CscsAccount = details[5].Trim(removedata).TrimStart('0');
                                }
                                else
                                {
                                    oTradeFile.TicketNumber = details[8].Trim(removedata);
                                    oTradeFile.BuySell = "B";
                                    oTradeFile.CounterParty = oCompany.MemberCode;
                                    oTradeFile.CscsAccount = details[3].Trim(removedata).TrimStart('0');


                                    TradeFile oTradeFileSecond = new TradeFile();
                                    oTradeFileSecond.TransactionDate = DateTime.ParseExact(strDiskDate.Substring(6, 2) + "/" + strDiskDate.Substring(4, 2) + "/" + strDiskDate.Substring(0, 4), "dd/MM/yyyy", format);
                                    oTradeFileSecond.SecurityName = details[1].Trim(removedata);
                                    oTradeFileSecond.Quantity = Decimal.ToInt64(Decimal.Parse(details[11].Trim(removedata)));
                                    oTradeFileSecond.Price = Decimal.Parse(details[10].Trim(removedata));
                                    oTradeFileSecond.Consideration = Decimal.ToInt64(Decimal.Parse(details[11].Trim(removedata))) * Decimal.Parse(details[10].Trim(removedata));
                                    oTradeFileSecond.TicketNumber = details[9].Trim(removedata);
                                    oTradeFileSecond.CounterParty = oCompany.MemberCode;
                                    oTradeFileSecond.BuySell = "S";
                                    oTradeFileSecond.CscsAccount = details[5].Trim(removedata).TrimStart('0');
                                    oTradeFiles.Add(oTradeFileSecond);
                                }
                                oTradeFiles.Add(oTradeFile);
                            }
                        }
                    }
                }

                ProductAcct oProductAcct = new ProductAcct();
                Stock oStock = new Stock();
                Broker oBroker = new Broker();

                StkParam oStkParam = new StkParam();
                BargainTrans oBargainTransProp = new BargainTrans();
                BargainSlip oBargainSlip = new BargainSlip();

                oBargainTransProp.DeleteAll();
                oBargainSlip.DeleteAll();

                IEnumerable<string> oFileTradeCscsAccounts = from s in oTradeFiles
                                                             select s.CscsAccount;
                IEnumerable<string> oProductAccts = oProductAcct.GetAllByProduct(oStkParam.Product, "ALL").Tables[0].AsEnumerable().Select(dataRow => dataRow.Field<string>("CsCs Account"));
                IEnumerable<string> oProductAcctMissings = oFileTradeCscsAccounts.Except(oProductAccts);
                IEnumerable<CustomerDetail> oMissingCustomerDetails = from s in oProductAcctMissings
                                                                      join t in oTradeFiles on s equals t.CscsAccount
                                                                      select new CustomerDetail
                                                                      {
                                                                          CscsAccount = s,
                                                                          CustomerName = t.CustomerName
                                                                      };
                oCustMissings = (from s in oMissingCustomerDetails
                                 select s.CscsAccount + " " + s.CustomerName).ToList();

                IEnumerable<string> oFileTradeBroker = from s in oTradeFiles
                                                       select s.CounterParty;
                IEnumerable<string> oBrokers = oBroker.GetAll().Tables[0].AsEnumerable().Select(dataRow => dataRow.Field<string>("Code"));
                oBrokerMissings = oFileTradeBroker.Except(oBrokers).ToList();

                IEnumerable<string> oFileTradeSecurityNames = from s in oTradeFiles
                                                              select s.SecurityName;
                IEnumerable<string> oStockCodes = oStock.GetAll().Tables[0].AsEnumerable().Select(dataRow => dataRow.Field<string>("Seccode"));
                oStockMissings = oFileTradeSecurityNames.Except(oStockCodes).ToList();

                if (oCustMissings.Count > 0 || oBrokerMissings.Count > 0 || oStockMissings.Count > 0)
                {
                    return blnStatus;
                }

                oProductAcct.ProductCode = oStkParam.Product;
                foreach (TradeFile oTradeFileRow in oTradeFiles)
                {
                    oBargainTransProp.Bslip = oTradeFileRow.TicketNumber;
                    oBargainTransProp.iDate = oTradeFileRow.TransactionDate;
                    oBargainTransProp.StockCode = oTradeFileRow.SecurityName;
                    oBargainTransProp.Units = oTradeFileRow.Quantity;
                    oBargainTransProp.UnitPrice = oTradeFileRow.Price;
                    oBargainTransProp.Consideration = oTradeFileRow.Consideration;
                    oBargainTransProp.StockType = "3";
                    if (oTradeFileRow.BuySell.Trim() == "B")
                    {
                        oBargainTransProp.BoughtBy = oCompany.MemberCode;
                        oBargainTransProp.Buy_Sold_Ind = "B";
                        oBargainTransProp.SoldBy = oTradeFileRow.CounterParty;
                    }
                    else if (oTradeFileRow.BuySell.Trim() == "S")
                    {
                        oBargainTransProp.SoldBy = oCompany.MemberCode;
                        oBargainTransProp.Buy_Sold_Ind = "S";
                        oBargainTransProp.BoughtBy = oTradeFileRow.CounterParty;
                    }
                    
                    oBargainTransProp.CrossD = "N";
                    
                    oBargainTransProp.AcctNo = oTradeFileRow.CscsAccount; ;
                    oProductAcct.CsCsAcct = oTradeFileRow.CscsAccount; ;
                    oBargainTransProp.CustNo = oProductAcct.GetCustNoGivenCsCsAcct().Trim();
                    oBargainTransProp.Add();
                }
                oBargainTransProp.SaveToBargainSlip();
                //oBargainTransProp.SaveToBargainSlipCrossDeal();
                //oBargainSlip.ProcessCrossDeal();
                //oBargainSlip.ProcessUnEqualCrossDeal();
                //oBargainSlip.UpdateCrossType();
                //if (!oBargainSlip.CheckCrossDealProcess())
                //{
                //    throw new Exception("Cross Deal Transactions Were Not Processed Properly, Please Check");
                //}
                datBargainSlipDataSet = oBargainSlip.GetAll();
                blnStatus = true;
            }
            catch (Exception err)
            {
                ErrorMessaging = err.Message;
            }
            return blnStatus;
        }
        #endregion

        #region Read CSCS No
        public string ReadCSCSNo(string strColSeven, string strColThree)
        {
            string strReturn;
            if (strColSeven.Trim().Substring(1, 4) == "0000")
            {
                strReturn = strColSeven.Trim().Substring(5);
            }
            else if (strColSeven.Trim().Substring(1, 3) == "000")
            {
                strReturn = strColSeven.Trim().Substring(4);
            }
            else if (strColSeven.Trim().Substring(1, 2) == "00")
            {
                strReturn = strColSeven.Trim().Substring(3);
            }
            else if (strColSeven.Trim().Substring(1, 1) == "0")
            {
                strReturn = strColSeven.Trim().Substring(2);
            }
            else
            {
                strReturn = strColThree.Trim();
            }
            return strReturn;
        }
        #endregion

        #region Remove Number
        public string RemoveNumber(string strWord)
        {
            strWord.Replace("1", ""); strWord.Replace("2", ""); strWord.Replace("3", "");
            strWord.Replace("4", ""); strWord.Replace("5", ""); strWord.Replace("6", "");
            strWord.Replace("7", ""); strWord.Replace("8", ""); strWord.Replace("9", "");
            strWord.Replace("0", "");
            return strWord;
        }
        #endregion

        public class TradeFile
        {
            public string BuySell { get; set; }
            public DateTime TransactionDate { get; set; }
            public string SecurityName { get; set; }
            public long Quantity { get; set; }
            public decimal Price { get; set; }
            public decimal Consideration { get; set; }
            public string CscsAccount { get; set; }
            public string TicketNumber { get; set; }
            public string CounterParty { get; set; }
            public string CustomerName { get; set; }
        }

        public class CustomerDetail
        {
            public string CscsAccount { get; set; }
            public string CustomerName { get; set; }
        }
    }
}
