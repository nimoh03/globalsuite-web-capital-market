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
    public class FileHandlerNASD
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
                        oTradeFile.NASDAccount = strline.Trim().Substring(66, 10).Trim();
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
                BargainTransNASD oBargainTransNASD = new BargainTransNASD();
                BargainSlipNASD oBargainSlipNASD = new BargainSlipNASD();
                oBargainTransNASD.DeleteAll();
                oBargainSlipNASD.DeleteAll();

                IEnumerable<string> oFileTradeCscsAccounts = from s in oTradeFiles
                                                             select s.NASDAccount;
                IEnumerable<string> oProductAccts = oProductAcct.GetAllByProduct(oStkParam.Product, "ALL").Tables[0].AsEnumerable().Select(dataRow => dataRow.Field<string>("NASD Account"));
                IEnumerable<string> oProductAcctMissings = oFileTradeCscsAccounts.Except(oProductAccts);
                IEnumerable<CustomerDetail> oMissingCustomerDetails = from s in oProductAcctMissings
                                                                      join t in oTradeFiles on s equals t.NASDAccount
                                                                      select new CustomerDetail
                                                                      {
                                                                          NASDAccount = s,
                                                                          CustomerName = t.CustomerName
                                                                      };
                oCustMissings = (from s in oMissingCustomerDetails
                                 select s.NASDAccount + " " + s.CustomerName).ToList();

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

                oStkParam.GetStkbPGenTable();
                oProductAcct.ProductCode = oStkParam.Product;
                foreach (TradeFile oTradeFileRow in oTradeFiles)
                {
                    oBargainTransNASD.Bslip = oTradeFileRow.TicketNumber;
                    oBargainTransNASD.iDate = oTradeFileRow.TransactionDate;
                    oBargainTransNASD.StockCode = oTradeFileRow.SecurityName;
                    oBargainTransNASD.Units = oTradeFileRow.Quantity;
                    oBargainTransNASD.UnitPrice = oTradeFileRow.Price;
                    oBargainTransNASD.Consideration = oTradeFileRow.Consideration;
                    oBargainTransNASD.StockType = "3";
                    if (oTradeFileRow.BuySell == "1")
                    {
                        oBargainTransNASD.BoughtBy = oStkParam.NASDMemberCode;
                        oBargainTransNASD.Buy_Sold_Ind = "B";
                        oBargainTransNASD.SoldBy = oTradeFileRow.CounterParty;
                    }
                    else if (oTradeFileRow.BuySell == "2")
                    {
                        oBargainTransNASD.SoldBy = oStkParam.NASDMemberCode;
                        oBargainTransNASD.Buy_Sold_Ind = "S";
                        oBargainTransNASD.BoughtBy = oTradeFileRow.CounterParty;
                    }
                    if (oBargainTransNASD.BoughtBy.Trim() == oBargainTransNASD.SoldBy.Trim())
                    {
                        oBargainTransNASD.CrossD = "Y";
                    }
                    else
                    {
                        oBargainTransNASD.CrossD = "N";
                    }
                    oBargainTransNASD.NASDAcctNo = oTradeFileRow.NASDAccount;
                    oProductAcct.NASDCsCsAcct = oTradeFileRow.NASDAccount;
                    oBargainTransNASD.CustNo = oProductAcct.GetCustNoGivenNASDAcct().Trim();
                    oBargainTransNASD.Add();
                }

                oBargainTransNASD.SaveToBargainSlipNASD();
                oBargainTransNASD.SaveToBargainSlipNASDCrossDeal();
                oBargainSlipNASD.ProcessCrossDeal();
                oBargainSlipNASD.ProcessUnEqualCrossDeal();
                oBargainSlipNASD.UpdateCrossType();
                if (!oBargainSlipNASD.CheckCrossDealProcess())
                {
                    throw new Exception("Cross Deal Transactions Were Not Processed Properly, Please Check");
                }
                datBargainSlipDataSet = oBargainSlipNASD.GetAll();
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
                StkParam oStkParam = new StkParam();
                oStkParam.GetStkbPGenTable();
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
                                string strDiskDate = details[7].Trim(removedata).Substring(0, 8);
                                oTradeFile.TransactionDate = DateTime.ParseExact(strDiskDate.Substring(6, 2) + "/" + strDiskDate.Substring(4, 2) + "/" + strDiskDate.Substring(0, 4), "dd/MM/yyyy", format);
                                oTradeFile.SecurityName = details[1].Trim(removedata);
                                oTradeFile.Quantity = int.Parse(details[11].Trim(removedata));
                                oTradeFile.Price = Decimal.Parse(details[10].Trim(removedata));
                                oTradeFile.Consideration = long.Parse(details[11].Trim(removedata)) * Decimal.Parse(details[10].Trim(removedata));
                                if (details[3] != null && details[3].Trim(removedata) != "" &&
                                    (details[5] == null || details[5].Trim(removedata) == ""))
                                {
                                    oTradeFile.TicketNumber = details[8].Trim(removedata);
                                    oTradeFile.BuySell = "B";
                                    oTradeFile.CounterParty = "GLOBALSEAM";
                                    oTradeFile.NASDAccount = details[3].Trim(removedata).TrimStart('0');
                                }
                                else if (details[5] != null && details[5].Trim(removedata) != "" &&
                                    (details[3] == null || details[3].Trim(removedata) == ""))
                                {
                                    oTradeFile.TicketNumber = details[9].Trim(removedata);
                                    oTradeFile.CounterParty = "GLOBALSEAM";
                                    oTradeFile.BuySell = "S";
                                    oTradeFile.NASDAccount = details[5].Trim(removedata).TrimStart('0');
                                }
                                else
                                {
                                    oTradeFile.TicketNumber = details[8].Trim(removedata);
                                    oTradeFile.BuySell = "B";
                                    oTradeFile.CounterParty = oStkParam.NASDMemberCode; 
                                    oTradeFile.NASDAccount = details[3].Trim(removedata).TrimStart('0');
                                    oTradeFiles.Add(oTradeFile);

                                    oTradeFile.TicketNumber = details[9].Trim(removedata);
                                    oTradeFile.CounterParty = oStkParam.NASDMemberCode;
                                    oTradeFile.BuySell = "S";
                                    oTradeFile.NASDAccount = details[5].Trim(removedata).TrimStart('0');
                                }
                                oTradeFiles.Add(oTradeFile);
                            }
                        }
                    }
                }

                ProductAcct oProductAcct = new ProductAcct();
                Stock oStock = new Stock();
                Broker oBroker = new Broker();

                BargainTransNASD oBargainTransNASD = new BargainTransNASD();
                BargainSlipNASD oBargainSlipNASD = new BargainSlipNASD();

                oBargainTransNASD.DeleteAll();
                oBargainSlipNASD.DeleteAll();

                IEnumerable<string> oFileTradeCscsAccounts = from s in oTradeFiles
                                                             select s.NASDAccount;
                IEnumerable<string> oProductAccts = oProductAcct.GetAllByProduct(oStkParam.Product, "ALL").Tables[0].AsEnumerable().Select(dataRow => dataRow.Field<string>("NASD Account"));
                IEnumerable<string> oProductAcctMissings = oFileTradeCscsAccounts.Except(oProductAccts);
                IEnumerable<CustomerDetail> oMissingCustomerDetails = from s in oProductAcctMissings
                                                                      join t in oTradeFiles on s equals t.NASDAccount
                                                                      select new CustomerDetail
                                                                      {
                                                                          NASDAccount = s,
                                                                          CustomerName = t.CustomerName
                                                                      };
                oCustMissings = (from s in oMissingCustomerDetails
                                 select s.NASDAccount + " " + s.CustomerName).ToList();

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
                    oBargainTransNASD.Bslip = oTradeFileRow.TicketNumber;
                    oBargainTransNASD.iDate = oTradeFileRow.TransactionDate;
                    oBargainTransNASD.StockCode = oTradeFileRow.SecurityName;
                    oBargainTransNASD.Units = oTradeFileRow.Quantity;
                    oBargainTransNASD.UnitPrice = oTradeFileRow.Price;
                    oBargainTransNASD.Consideration = oTradeFileRow.Consideration;
                    oBargainTransNASD.StockType = "3";
                    if (oTradeFileRow.BuySell == "1")
                    {
                        oBargainTransNASD.BoughtBy = oStkParam.NASDMemberCode;
                        oBargainTransNASD.Buy_Sold_Ind = "B";
                        oBargainTransNASD.SoldBy = oTradeFileRow.CounterParty;
                    }
                    else if (oTradeFileRow.BuySell == "2")
                    {
                        oBargainTransNASD.SoldBy = oStkParam.NASDMemberCode;
                        oBargainTransNASD.Buy_Sold_Ind = "S";
                        oBargainTransNASD.BoughtBy = oTradeFileRow.CounterParty;
                    }
                    if (oBargainTransNASD.BoughtBy.Trim() == oBargainTransNASD.SoldBy.Trim())
                    {
                        oBargainTransNASD.CrossD = "Y";
                    }
                    else
                    {
                        oBargainTransNASD.CrossD = "N";
                    }
                    oBargainTransNASD.NASDAcctNo = oTradeFileRow.NASDAccount; ;
                    oProductAcct.NASDCsCsAcct = oTradeFileRow.NASDAccount; ;
                    oBargainTransNASD.CustNo = oProductAcct.GetCustNoGivenNASDAcct().Trim();
                    oBargainTransNASD.Add();
                }
                oBargainTransNASD.SaveToBargainSlipNASD();
                oBargainTransNASD.SaveToBargainSlipNASDCrossDeal();
                oBargainSlipNASD.ProcessCrossDeal();
                oBargainSlipNASD.ProcessUnEqualCrossDeal();
                oBargainSlipNASD.UpdateCrossType();
                if (!oBargainSlipNASD.CheckCrossDealProcess())
                {
                    throw new Exception("Cross Deal Transactions Were Not Processed Properly, Please Check");
                }
                datBargainSlipDataSet = oBargainSlipNASD.GetAll();
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
            public string NASDAccount { get; set; }
            public string TicketNumber { get; set; }
            public string CounterParty { get; set; }
            public string CustomerName { get; set; }
        }

        public class CustomerDetail
        {
            public string NASDAccount { get; set; }
            public string CustomerName { get; set; }
        }
    }
}

