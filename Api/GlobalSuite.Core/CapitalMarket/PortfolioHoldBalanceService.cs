using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Admin.Business;
using BaseUtility.Business;
using CapitalMarket.Business;
using CustomerManagement.Business;
using GL.Business;
using GlobalSuite.Core.Helpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GlobalSuite.Core.CapitalMarket
{
    public partial class TradingService 
    {
        public async Task<ResponseResult> PortfolioHoldingBalance(PortOpenBal oPortOpenBal)
        {
            return await Task.Run(() =>
            {
                var oStockPGenTable = new StkParam();
                var oProductAcct = new ProductAcct
                {
                    ProductCode = oStockPGenTable.Product,
                    CustAID = oPortOpenBal.CustNo
                };
                if (oProductAcct.AcctDeactivation.Trim() == "Y")
                    return ResponseResult.Error("Cannot Post! Customer Account Deactivated");
                oPortOpenBal.UserId = GeneralFunc.UserName;
                oPortOpenBal.Posted = "N";
                oPortOpenBal.Reversed = "N";
                oPortOpenBal.SaveType = Constants.SaveType.ADDS;
                var oSaveStatus = oPortOpenBal.Save();
                return oSaveStatus == DataGeneral.SaveStatus.Saved ? ResponseResult.Success() : ResponseResult.Error("Holding Balance cannot be saved.");
            });
        }

        public async Task<ResponseResult> PostPortfolioHoldingBalance(string code)
        {
            return await Task.Run(() =>
            {
                var oPortOpenBal = new PortOpenBal
                {
                    TransNo = code.Trim()
                };
                var oCustomer = new Customer();
                if (oPortOpenBal.GetPortOpenBalUnPost())
            {
                var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
                using (var connection = db.CreateConnection() as SqlConnection)
                {
                    connection.Open();
                    var transaction = connection.BeginTransaction();
                    try
                    {
                        var oStock = new Stock();
                        oCustomer.CustAID = oPortOpenBal.CustNo.Trim();
                        if (!oCustomer.GetCustomerName(oPortOpenBal.Product))
                        {
                            throw new Exception("Customer Does Not Exist");
                        }

                        oStock.SecCode = oPortOpenBal.Stockcode.ToString().Trim();
                        var oStockInstrumentType = oStock.GetInstrumentTypeUsingStockCode();
                        if (oStockInstrumentType != DataGeneral.StockInstrumentType.QUOTEDEQUITY &&
                            oStockInstrumentType != DataGeneral.StockInstrumentType.NASD &&
                            oStockInstrumentType != DataGeneral.StockInstrumentType.BOND)
                        {
                            throw new Exception("Security Code Must Be Quoted Equity Or NASD Instrument");
                        }

                        var oPort = new Portfolio
                        {
                            PurchaseDate = oPortOpenBal.EffDate,
                            CustomerAcct = oPortOpenBal.CustNo.ToString(),
                            StockCode = oPortOpenBal.Stockcode.ToString().Trim(),
                            Units = long.Parse(oPortOpenBal.Units.ToString().Trim()),
                            UnitPrice = float.Parse(oPortOpenBal.UnitPrice.ToString()),
                            ActualUnitCost = float.Parse(oPortOpenBal.UnitPrice.ToString())
                        };
                        oPort.TotalCost = oPort.Units * Decimal.Parse(oPort.ActualUnitCost.ToString());
                        oPort.Ref01 = oPortOpenBal.TransNo;
                        oPort.MarginCode = "";
                        oPort.DebCred = "C";
                        oPort.TransDesc = "Add To Stock Portfolio Balance: " + oPortOpenBal.Units.ToString().Trim() + " @ " + oPortOpenBal.UnitPrice.ToString().Trim();
                        oPort.TransType = "OPBL";
                        oPort.SysRef = "OPBL-" + oPortOpenBal.TransNo;
                        var dbCommandPort = oPort.AddCommand();
                        db.ExecuteNonQuery(dbCommandPort, transaction);
                        oPortOpenBal.UserId = GeneralFunc.UserName;
                        var oCommandUpdatePost = oPortOpenBal.UpDatePostCommand();
                        db.ExecuteNonQuery(oCommandUpdatePost, transaction);

                        var oProductAcct = new ProductAcct();
                        var oProduct = new Product();
                        var oGlParam = new GLParam();
                        var oBranch = new Branch();
                        var oStkParamBoxLoad = new StkParam();
                        oProductAcct.ProductCode = oStkParamBoxLoad.Product;
                        oProductAcct.CustAID = oPortOpenBal.CustNo;
                        if (oProductAcct.GetBoxLoadStatus() && oPort.TotalCost > 0 )
                        {
                            var strNumberNext = "";
                            var oCommandJnumber = db.GetStoredProcCommand("JnumberAdd") as SqlCommand;
                            db.AddOutParameter(oCommandJnumber, "Jnumber", SqlDbType.BigInt, 8);
                            db.ExecuteNonQuery(oCommandJnumber, transaction);
                            strNumberNext = db.GetParameterValue(oCommandJnumber, "Jnumber").ToString();

                            var oGl = new AcctGL
                            {
                                EffectiveDate = oPortOpenBal.EffDate
                            };
                            if (oStockInstrumentType == DataGeneral.StockInstrumentType.NASD)
                            {
                                oProduct.TransNo = oStkParamBoxLoad.ProductInvestmentNASD;
                                oGl.MasterID = oProduct.GetProductGLAcct();
                                oGl.AcctRef = oStkParamBoxLoad.ProductInvestmentNASD;
                            }
                            else
                            {
                                oProduct.TransNo = oStkParamBoxLoad.ProductInvestment;
                                oGl.MasterID = oProduct.GetProductGLAcct();
                                oGl.AcctRef = oStkParamBoxLoad.ProductInvestment;
                            }
                            oGl.AccountID = oPortOpenBal.CustNo;
                            oGl.Credit = 0;
                            oGl.Debit = Math.Round(oPort.TotalCost, 2);
                            oGl.Debcred = "D";
                            oGl.Desciption = "Stock Opening Balance: " + oPortOpenBal.Units.ToString().Trim() + " " + oPortOpenBal.Stockcode.ToString().Trim() + " @ " + oPortOpenBal.UnitPrice.ToString().Trim(); 
                            oGl.TransType = "SKOPBAL";
                            oGl.SysRef = "SKOBL" + "-" + oPortOpenBal.TransNo;
                            oGl.Ref01 = oPortOpenBal.TransNo;
                            oGl.Chqno = "";
                            oGl.InstrumentType = DataGeneral.GLInstrumentType.C;
                            oGl.Ref02 = oPortOpenBal.CustNo;
                            oGl.RecAcctMaster = oGlParam.GLOpenAcct;
                            oGl.RecAcct = "";
                            oGl.AcctRefSecond="";
                            oGl.Reverse = "N";
                            oGl.Jnumber = strNumberNext;
                            oGl.Reverse = "N";
                            oGl.Branch = oBranch.DefaultBranch;
                            oGl.FeeType = "";  
                            var dbCommandCustAccount = oGl.AddCommand();
                            db.ExecuteNonQuery(dbCommandCustAccount, transaction);

                            oGl.EffectiveDate = oPortOpenBal.EffDate;
                            oGl.MasterID = oGlParam.GLOpenAcct;
                            oGl.AccountID = "";
                            oGl.AcctRef = "";
                            oGl.Credit = Math.Round(oPort.TotalCost, 2); 
                            oGl.Debit = 0;
                            oGl.Debcred = "C";
                            oGl.Desciption = "Stock Opening Balance: " + oPortOpenBal.Units.ToString().Trim() + " " + oPortOpenBal.Stockcode.ToString().Trim() + " @ " + oPortOpenBal.UnitPrice.ToString().Trim();
                            oGl.TransType = "SKOPBAL";
                            oGl.SysRef = "SKOBL" + "-" + oPortOpenBal.TransNo;
                            oGl.Ref01 = oPortOpenBal.TransNo;
                            oGl.Chqno = "";
                            oGl.InstrumentType = DataGeneral.GLInstrumentType.C;
                            oGl.Ref02 = oPortOpenBal.CustNo;
                            if (oStockInstrumentType == DataGeneral.StockInstrumentType.NASD)
                            {
                                oProduct.TransNo = oStkParamBoxLoad.ProductInvestmentNASD;
                                oGl.RecAcctMaster = oProduct.GetProductGLAcct();
                                oGl.AcctRefSecond = oStkParamBoxLoad.ProductInvestmentNASD;
                            }
                            else
                            {
                                oProduct.TransNo = oStkParamBoxLoad.ProductInvestment;
                                oGl.RecAcctMaster = oProduct.GetProductGLAcct();
                                oGl.AcctRefSecond = oStkParamBoxLoad.ProductInvestment;
                            }
                            
                            oGl.RecAcct = oPortOpenBal.CustNo;
                            oGl.Reverse = "N";
                            oGl.Jnumber = strNumberNext;
                            oGl.Reverse = "N";
                            oGl.Branch = oBranch.DefaultBranch;
                            oGl.FeeType = "";  
                            var dbCommandOpenGlAcct = oGl.AddCommand();
                            db.ExecuteNonQuery(dbCommandOpenGlAcct, transaction);
                        }

                        transaction.Commit();
return ResponseResult.Success();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message, ex);
                        transaction?.Rollback();
                        return ResponseResult.Error("Error In Posting " + ex.Message.Trim());

                    }
                }
            }
                else
               return ResponseResult.Error("Error In Posting, Purchase Allotment Does Not Exist");
            });
        }
    }
}