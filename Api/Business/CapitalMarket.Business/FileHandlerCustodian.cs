using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Globalization;
using System.Configuration;
using GL.Business;

namespace CapitalMarket.Business
{
    public class FileHandlerCustodian
    {
        #region Declartion
        private DataSet datBargainSlipDataSet;
        #endregion

        #region Properties
        public DataSet BargainSlipDataSet
        {
            get { return datBargainSlipDataSet; }
        }
        #endregion

        #region Reading Text File
        public int ReadTextFile(string filePath, string strUserName, string strBKCustodianCode)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "CUSTODIAN";
            string strCustodianAcct = oGLParam.CheckParameter();

            StreamReader filestream = null;
            try
            {
                IFormatProvider format = new CultureInfo("en-GB");

                int intStatus = 0;

                //char[] textdelimiter = new char[] { ' ' };
                string strLine = "";
                string strLineCustChk = "";
                try
                {
                    filestream = File.OpenText(filePath);
                }
                catch
                {
                    intStatus = 2;
                    return intStatus;
                }
                ProductAcct oProductAcct = new ProductAcct();
                StkParam oStkParam = new StkParam();
                Stock oStockChk = new Stock();
                Broker oBrokerChk = new Broker();
                BargainTransCustodian oBargainTrans = new BargainTransCustodian();
                BargainSlipCustodian oBargainSlip = new BargainSlipCustodian();
                CustMissing oCustMissing = new CustMissing();
                StockMissing oStockMissing = new StockMissing();
                BrokerMissing oBrokerMissing = new BrokerMissing();
                oBargainTrans.CustodianCode = strBKCustodianCode;
                oBargainSlip.CustodianCode = strBKCustodianCode;
                if (!oBargainTrans.DeleteAll())
                {
                    filestream.Close();
                    filestream.Dispose();
                    intStatus = 3;
                    return intStatus;
                }
                if (!oBargainSlip.DeleteAll())
                {
                    filestream.Close();
                    filestream.Dispose();
                    intStatus = 3;
                    return intStatus;
                }

                if (!oCustMissing.DeleteAll())
                {
                    filestream.Close();
                    filestream.Dispose();
                    intStatus = 4;
                    return intStatus;
                }
                if (!oStockMissing.DeleteAll())
                {
                    filestream.Close();
                    filestream.Dispose();
                    intStatus = 8;
                    return intStatus;
                }
                while (strLineCustChk != null)
                {
                    strLineCustChk = filestream.ReadLine();
                    string strChkCustExist;
                    if ((strLineCustChk != "" && strLineCustChk != null))
                    {
                        //maybe 10 and 8 for tight file
                        //Anchoria
                        //oProductAcct.CsCsAcct = strLineCustChk.Trim().Substring(9, 10).Trim();
                        if (strCustodianAcct.Trim() == "YES")
                        {
                            oProductAcct.AccessCode = strLineCustChk.Trim().Substring(9, 10).Trim();
                            oProductAcct.ProductCode = oStkParam.Product;
                            strChkCustExist = oProductAcct.GetCustNoGivenAccessCode();
                        }
                        else
                        {
                            oProductAcct.CsCsAcct = strLineCustChk.Trim().Substring(9, 10).Trim();
                            oProductAcct.ProductCode = oStkParam.Product;
                            strChkCustExist = oProductAcct.GetCustNoGivenCsCsAcct();
                        }
                        
                        if ((strChkCustExist == "") || (strChkCustExist == null))
                        {
                            oCustMissing.Surname = strLineCustChk.Trim().Substring(65, 41).Trim();
                            if (strCustodianAcct.Trim() == "YES")
                            {
                                oCustMissing.CsCsAcct = strLineCustChk.Trim().Substring(9, 10).Trim();
                            }
                            else
                            {
                                oCustMissing.CsCsAcct = strLineCustChk.Trim().Substring(9, 10).Trim();
                            }
                            string strMissingDate = strLineCustChk.Trim().Substring(121, 9).Trim();
                            oCustMissing.ValDate = DateTime.ParseExact(strMissingDate.Substring(6, 2) + "/" + strMissingDate.Substring(4, 2) + "/" + strMissingDate.Substring(0, 4), "dd/MM/yyyy", format);
                            oCustMissing.UserID = strUserName;
                            if (!oCustMissing.Add())
                            {
                                filestream.Close();
                                filestream.Dispose();
                                intStatus = 5;
                                return intStatus;
                            }
                        }

                        oStockChk.SecCode = strLineCustChk.Trim().Substring(28, 17).Trim();
                        if (!oStockChk.GetStockOther())
                        {
                            oStockMissing.SecCode = strLineCustChk.Trim().Substring(28, 17).Trim();
                            oStockMissing.UserID = strUserName;
                            if (!oStockMissing.Add())
                            {
                                filestream.Close();
                                filestream.Dispose();
                                intStatus = 9;
                                return intStatus;
                            }
                        }
                        oBrokerChk.Code = strLineCustChk.Trim().Substring(105, 5).Trim();
                        if (!oBrokerChk.GetBrokerOther())
                        {
                            oBrokerMissing.SecCode = strLineCustChk.Trim().Substring(105, 5).Trim();
                            oBrokerMissing.UserID = strUserName;
                            if (!oBrokerMissing.Add())
                            {
                                filestream.Close();
                                filestream.Dispose();
                                intStatus = 11;
                                return intStatus;
                            }
                        }
                    }
                }
                DataSet dataChkCustMissing = oCustMissing.GetAll();
                System.Data.DataTable thisTable = dataChkCustMissing.Tables[0];
                System.Data.DataRow[] thisRow = thisTable.Select();
                if (thisRow.Length == 0)
                {
                    DataSet dataChkStockMissing = oStockMissing.GetAll();
                    System.Data.DataTable thisTableStockMiss = dataChkStockMissing.Tables[0];
                    System.Data.DataRow[] thisRowStockMiss = thisTableStockMiss.Select();
                    if (thisRowStockMiss.Length == 0)
                    {
                        DataSet dataChkBrokerMissing = oBrokerMissing.GetAll();
                        System.Data.DataTable thisTableBrokerMiss = dataChkBrokerMissing.Tables[0];
                        System.Data.DataRow[] thisRowBrokerMiss = thisTableBrokerMiss.Select();
                        if (thisRowBrokerMiss.Length == 0)
                        {

                        }
                        else
                        {
                            filestream.Close();
                            filestream.Dispose();
                            intStatus = 12;
                            return intStatus;
                        }
                    }
                    else
                    {
                        filestream.Close();
                        filestream.Dispose();
                        intStatus = 10;
                        return intStatus;
                    }
                }
                else
                {
                    filestream.Close();
                    filestream.Dispose();

                    intStatus = 6;
                    return intStatus;
                }

                filestream.Close();
                filestream.Dispose();

                filestream = File.OpenText(filePath);

                while (strLine != null)
                {
                    strLine = filestream.ReadLine();
                    if ((strLine != "" && strLine != null))
                    {
                        //string [] splitout = strLine.Split(textdelimiter); 

                        oBargainTrans.Bslip = strLine.Trim().Substring(20, 8).Trim();
                        string strDiskDate = strLine.Trim().Substring(121, 9).Trim();
                        oBargainTrans.iDate = DateTime.ParseExact(strDiskDate.Substring(6, 2) + "/" + strDiskDate.Substring(4, 2) + "/" + strDiskDate.Substring(0, 4), "dd/MM/yyyy", format);
                        oBargainTrans.StockCode = strLine.Trim().Substring(28, 17).Trim();
                        oBargainTrans.Units = int.Parse(strLine.Trim().Substring(41,16).Trim());
                        oBargainTrans.UnitPrice = Decimal.Parse(strLine.Trim().Substring(57, 8).Trim());
                        oBargainTrans.Consideration = long.Parse(strLine.Trim().Substring(41, 16).Trim()) * Decimal.Parse(strLine.Trim().Substring(57, 8).Trim());
                        oBargainTrans.StockType = "3";
                        Company oCompany = new Company();
                        if (strLine.Trim().Substring(6, 2).Trim() == "1")
                        {
                            oBargainTrans.BoughtBy = oCompany.MemberCode;
                            oBargainTrans.Buy_Sold_Ind = "B";
                            oBargainTrans.SoldBy = strLine.Trim().Substring(105, 5).Trim();
                        }
                        else if (strLine.Trim().Substring(6, 2).Trim() == "2")
                        {
                            oBargainTrans.SoldBy = oCompany.MemberCode;
                            oBargainTrans.Buy_Sold_Ind = "S";
                            oBargainTrans.BoughtBy = strLine.Trim().Substring(105, 5).Trim();
                        }
                        
                        //Difference Between The Normal Bargain Trans
                        oBargainTrans.CrossD = "N";

                        oBargainTrans.AcctNo = strLine.Trim().Substring(9, 10).Trim();
                        if (strCustodianAcct.Trim() == "YES")
                        {
                            oProductAcct.AccessCode = strLine.Trim().Substring(9, 10).Trim();
                            oBargainTrans.CustNo = oProductAcct.GetCustNoGivenAccessCode().Trim();
                        }
                        else
                        {
                            oProductAcct.CsCsAcct = strLine.Trim().Substring(9, 10).Trim();
                            oBargainTrans.CustNo = oProductAcct.GetCustNoGivenCsCsAcct().Trim();
                        }
                        oBargainTrans.CustodianCode = strBKCustodianCode;

                        if (!oBargainTrans.Add())
                        {
                            filestream.Close();
                            filestream.Dispose();
                            intStatus = 7;
                            return intStatus;
                        }
                    }
                }
                oBargainTrans.SaveToBargainSlip();
                //oBargainTrans.SaveToBargainSlipCrossDeal();
                //oBargainSlip.ProcessCrossDeal();
                //oBargainSlip.ProcessUnEqualCrossDeal();
                //if (!oBargainSlip.CheckCrossDealProcess())
                //{
                //    throw new Exception("Cross Deal Transactions Were Not Processed Properly, Please Check");
                //}
                filestream.Close();
                filestream.Dispose();



                datBargainSlipDataSet = oBargainSlip.GetAll();
                intStatus = 1;
                return intStatus;
            }
            catch (Exception err)
            {
                throw err;
            }
            finally
            {
                if (filestream != null)
                {
                    filestream.Close();
                    filestream.Dispose();
                }
            }
        }
        #endregion
    }
}
