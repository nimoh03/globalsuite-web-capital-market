using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using CapitalMarket.Business;
using GL.Business;
using GlobalSuite.Core.Helpers;
using Microsoft.Office.Interop.Excel;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using DataTable = System.Data.DataTable;

namespace GlobalSuite.Core.CapitalMarket
{
    public partial class TradingService 
    {
        public async Task<ResponseResult> PostCsCsStockPositionFoxPro(DateTime date)
        {
            Worksheet workSheet;
            Workbook workBook;
            return await Task.Run(() =>
            {
                try
                {

                    var oCompany = new Company();
                    var oProductAcct = new ProductAcct();
                    var oStkParam = new StkParam();
                    var oStock = new Stock();
                    var dtCustMissing = new System.Data.DataTable("CUSTMISSING");
                    var dtStockMissing = new System.Data.DataTable("STOCKMISSING");

                    var dtCustomer = new System.Data.DataTable("CSCSSTOCKPOSITION");

                    DataColumn myDataColumn;
                    DataColumn myDataColumnCustMiss;
                    DataColumn myDataColumnStockMiss;

                    myDataColumn = new DataColumn();
                    myDataColumn.DataType = Type.GetType("System.String");
                    myDataColumn.ColumnName = "CODENAME";
                    dtCustomer.Columns.Add(myDataColumn);


                    myDataColumn = new DataColumn();
                    myDataColumn.DataType = Type.GetType("System.String");
                    myDataColumn.ColumnName = "CUSTNAME";
                    dtCustomer.Columns.Add(myDataColumn);
                    myDataColumnCustMiss = new DataColumn();
                    myDataColumnCustMiss.DataType = Type.GetType("System.String");
                    myDataColumnCustMiss.ColumnName = "CUSTNAME";
                    dtCustMissing.Columns.Add(myDataColumnCustMiss);

                    myDataColumn = new DataColumn();
                    myDataColumn.DataType = Type.GetType("System.String");
                    myDataColumn.ColumnName = "CSCSACCT";
                    dtCustomer.Columns.Add(myDataColumn);
                    myDataColumnCustMiss = new DataColumn();
                    myDataColumnCustMiss.DataType = Type.GetType("System.String");
                    myDataColumnCustMiss.ColumnName = "CSCSACCT";
                    dtCustMissing.Columns.Add(myDataColumnCustMiss);

                    myDataColumn = new DataColumn();
                    myDataColumn.DataType = Type.GetType("System.String");
                    myDataColumn.ColumnName = "STOCK";
                    dtCustomer.Columns.Add(myDataColumn);
                    myDataColumnStockMiss = new DataColumn();
                    myDataColumnStockMiss.DataType = Type.GetType("System.String");
                    myDataColumnStockMiss.ColumnName = "STOCK";
                    dtStockMissing.Columns.Add(myDataColumnStockMiss);

                    myDataColumn = new DataColumn();
                    myDataColumn.DataType = Type.GetType("System.Int64");
                    myDataColumn.ColumnName = "PENDINGUNIT";
                    dtCustomer.Columns.Add(myDataColumn);

                    myDataColumn = new DataColumn();
                    myDataColumn.DataType = Type.GetType("System.Int64");
                    myDataColumn.ColumnName = "AVAILABLEUNIT";
                    dtCustomer.Columns.Add(myDataColumn);




                    var ipath = GetMissingFilePath("CSCSPOSITIONFOXPRO", oCompany.MemberCode, ".xlsx");
                    // @"C:\GlobalSuiteFolder\" + oCompany.MemberCode.Trim() + "CSCSPOSITIONFOXPRO.xlsx";
                    _Application app = new Application();

                    workBook = app.Workbooks.Open(ipath, 0, true, 5, "", "", true, XlPlatform.xlWindows,
                        "\t", false, false, 0, true, 1, 0);
                    workSheet = (Worksheet)workBook.ActiveSheet;

                    long index = 0;
                    object colIndex1 = 1;
                    object colIndex2 = 2;
                    object colIndex3 = 3;
                    object colIndex4 = 4;
                    object colIndex5 = 5;
                    object colIndex6 = 6;
                    object colIndex7 = 7;

                    DataRow oRowCsCsAcct;

                    var range = workSheet.UsedRange;

                    for (index = 1; index <= range.Rows.Count + 2; ++index)
                    {
                        if (((Range)workSheet.Cells[index, colIndex1]).Value2 != null &&
                            (string)(((Range)workSheet.Cells[index, colIndex1]).Value2).ToString().Trim() != "")
                        {
                            if (((Range)workSheet.Cells[index, 1]).Value2.ToString().Trim() != "MEMBER")
                            {
                                oRowCsCsAcct = dtCustomer.NewRow();
                                if (((Range)workSheet.Cells[index, 1]).Value2 != null)
                                {
                                    oRowCsCsAcct["CODENAME"] = ((Range)workSheet.Cells[index, 1]).Value2.ToString();
                                }
                                else
                                {
                                    oRowCsCsAcct["CODENAME"] = "";
                                }

                                if (((Range)workSheet.Cells[index, 3]).Value2 != null)
                                {
                                    oRowCsCsAcct["CUSTNAME"] = ((Range)workSheet.Cells[index, 3]).Value2.ToString();
                                }
                                else
                                {
                                    oRowCsCsAcct["CUSTNAME"] = "";
                                }

                                if (((Range)workSheet.Cells[index, 2]).Value2 != null)
                                {
                                    oRowCsCsAcct["CSCSACCT"] = ((Range)workSheet.Cells[index, 2]).Value2.ToString();
                                }
                                else
                                {
                                    oRowCsCsAcct["CSCSACCT"] = "";
                                }

                                if (((Range)workSheet.Cells[index, 5]).Value2 != null)
                                {
                                    oRowCsCsAcct["STOCK"] = ((Range)workSheet.Cells[index, 5]).Value2.ToString();
                                }
                                else
                                {
                                    oRowCsCsAcct["STOCK"] = "";
                                }

                                if (((Range)workSheet.Cells[index, 7]).Value2 != null)
                                {
                                    oRowCsCsAcct["PENDINGUNIT"] = ((Range)workSheet.Cells[index, 7]).Value2.ToString();
                                }
                                else
                                {
                                    oRowCsCsAcct["PENDINGUNIT"] = "";
                                }

                                if (((Range)workSheet.Cells[index, 6]).Value2 != null)
                                {
                                    oRowCsCsAcct["AVAILABLEUNIT"] =
                                        ((Range)workSheet.Cells[index, 6]).Value2.ToString();
                                }
                                else
                                {
                                    oRowCsCsAcct["AVAILABLEUNIT"] = "";
                                }

                                dtCustomer.Rows.Add(oRowCsCsAcct);
                            }
                        }
                    }

                    var strRetCustId = "";
                    var strRetStockCode = "";
                    long lngNumberDone = 0;
                    oProductAcct.ProductCode = oStkParam.Product;
                    foreach (DataRow oRow in dtCustomer.Rows)
                    {
                        lngNumberDone = lngNumberDone + 1;
                        //"Checking for missing customer A/C and securities.Please wait......" + lngNumberDone.ToString() + " Of " + dtCustomer.Rows.Count.ToString();

                        oProductAcct.CsCsAcct = oRow["CSCSACCT"].ToString();
                        strRetCustId = oProductAcct.GetCustNoGivenCsCsAcct();
                        if (strRetCustId == null || strRetCustId.Trim() == "")
                        {
                            oRowCsCsAcct = dtCustMissing.NewRow();
                            oRowCsCsAcct["CSCSACCT"] = oRow["CSCSACCT"];
                            oRowCsCsAcct["CUSTNAME"] = oRow["CUSTNAME"];
                            dtCustMissing.Rows.Add(oRowCsCsAcct);
                            continue;
                        }

                        oStock.SecCode = oRow["STOCK"].ToString();
                        strRetStockCode = oStock.GetStockCodeGivenStockCode();
                        if (strRetStockCode == null || strRetStockCode.Trim() == "")
                        {
                            oRowCsCsAcct = dtStockMissing.NewRow();
                            oRowCsCsAcct["STOCK"] = oRow["STOCK"];
                            dtStockMissing.Rows.Add(oRowCsCsAcct);
                            continue;
                        }
                    }

                    if (dtStockMissing.Rows.Count > 0 || dtCustMissing.Rows.Count > 0)
                        return ResponseResult.Error(
                            ShowMissingItem(dtCustMissing, dtStockMissing, oCompany.MemberCode));



                    var oPortfolioUploadDate = new PortfolioUploadDate
                    {
                        UploadDate = date.ToExact()
                    };
                    if (oPortfolioUploadDate.GetPortfolioUploadDate())
                        return ResponseResult.Error("Posting Aborted! Portfolio Upload For The Date Selected Already Posted");

                    //TODO Return Message Start Posting 
                    // lblStatusMessage.Text = "Loading Customer Stock Portfolio.Please wait......";
                    // lblStatusMessage.Refresh();
                    // progressBar1.Value = progressBar1.Minimum;
                    // progressBar1.Maximum = dtCustomer.Rows.Count;
                    lngNumberDone = 0;

                    var factory = new DatabaseProviderFactory();
                    var db = factory.Create("GlobalSuitedb") as SqlDatabase;
                    using (var connection = db.CreateConnection() as SqlConnection)
                    {
                        connection.Open();
                        var transaction = connection.BeginTransaction();
                        try
                        {
                            var oPortfolioFoxPro = new PortfolioFoxPro();
                            oProductAcct.ProductCode = oStkParam.Product;
                            foreach (DataRow oRow in dtCustomer.Rows)
                            {
                                lngNumberDone = lngNumberDone + 1;
                                // lblStatusMessage.Text = "Loading Customer Stock Portfolio.Please wait......" +
                                //                         lngNumberDone.ToString() + " Of " +
                                //                         dtCustomer.Rows.Count.ToString();

                                if (long.Parse(oRow["AVAILABLEUNIT"].ToString()) > 0)
                                {

                                    oProductAcct.CsCsAcct = oRow["CSCSACCT"].ToString();
                                    oPortfolioFoxPro.EffDate = date.ToExact();
                                    oPortfolioFoxPro.CustNo = oProductAcct.GetCustNoGivenCsCsAcct();
                                    oPortfolioFoxPro.StockCode = oRow["STOCK"].ToString();
                                    oPortfolioFoxPro.PendingUnit = long.Parse(oRow["PENDINGUNIT"].ToString());
                                    oPortfolioFoxPro.AvailableUnit = long.Parse(oRow["AVAILABLEUNIT"].ToString());
                                    var dbCommandPortFoxPro = oPortfolioFoxPro.AddCommand();
                                    db.ExecuteNonQuery(dbCommandPortFoxPro, transaction);
                                }
                            }

                            oPortfolioUploadDate.UploadDate = date.ToExact();
                            var dbCommandUploadDate = oPortfolioUploadDate.AddCommand();
                            db.ExecuteNonQuery(dbCommandUploadDate, transaction);

                            transaction.Commit();
                            return ResponseResult.Success("CSCS Stock Holding For FoxPro Report Upload Successful");
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.Message, ex);
                            transaction?.Rollback();
                            return ResponseResult.Error("Error In Uploading CSCS Stock Holding For FoxPro Report " +
                                                        ex.Message.Trim());
                        }
                    }
                }


                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    return ResponseResult.Error("Error In Uploading CSCS Stock Holding For FoxPro Report " +
                                                ex.Message.Trim());
                }
            });
        }

        public async Task<ResponseResult> SendPortfolioToEmailCsCs(SendPortfolioToEmail oSendPortfolioToEmail)
        {
          return  await Task.Run(() => oSendPortfolioToEmail.Add() 
              ? ResponseResult.Success("Send Customer Portfolio To Email Successful") 
              : ResponseResult.Error("Error Sending Customer Portfolio To Email"));
        }

        private string ShowMissingItem(DataTable oDt, DataTable oDt2, string companyMemberCode)
        {
            var strMissingItem = "";
            var i = 0;
            if (oDt.Rows.Count != 0)
            {
                strMissingItem = "Customer Account Missing " + Environment.NewLine;
                foreach (DataRow dr in oDt.Rows)
                {
                    foreach (DataColumn dc in oDt.Columns)
                    {
                        if (i == 0)
                            strMissingItem += dr[dc];
                        else
                            strMissingItem = strMissingItem + " , " + dr[dc];
                        i++;
                    }

                    i = 0;
                    strMissingItem = strMissingItem + Environment.NewLine;
                }
            }

            //Stock Part
            if (oDt2.Rows.Count != 0)
            {
                strMissingItem = strMissingItem + " Stock Code Missing " + Environment.NewLine;
                i = 0;
                foreach (DataRow dr in oDt2.Rows)
                {
                    foreach (DataColumn dc in oDt2.Columns)
                    {
                        if (i == 0)
                            strMissingItem += dr[dc];
                        else
                            strMissingItem = strMissingItem + " , " + dr[dc];
                        i++;
                    }

                    i = 0;
                    strMissingItem = strMissingItem + Environment.NewLine;
                }
            }

            var missFilePath = GetMissingFilePath("AccountingMissing", companyMemberCode);
            System.IO.File.WriteAllText(missFilePath, strMissingItem);
             
                // "Cannot Upload Customer CSCS Stock Position Holding! Customer Account Or Stock Code Is Missing" +
                //     ".Check Account Missing Text File";
                return strMissingItem;

        }

    }
}