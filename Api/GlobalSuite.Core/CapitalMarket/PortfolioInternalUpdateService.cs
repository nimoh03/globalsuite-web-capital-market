using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Admin.Business;
using BaseUtility.Business;
using CapitalMarket.Business;
using GL.Business;
using GlobalSuite.Core.Helpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GlobalSuite.Core.CapitalMarket
{
    public partial class TradingService 
    {
        public async Task<ResponseResult> PortfolioCreateInternalUpdate(PortfolioInternalUpdate oPortfolioInternalUpdate) => 
            await Task.Run(() => Save(oPortfolioInternalUpdate, Constants.SaveType.ADDS, true));

        public async Task<ResponseResult> PortfolioUpdateInternalUpdate(PortfolioInternalUpdate oPortfolioInternalUpdate)
            => await Task.Run(() => Save(oPortfolioInternalUpdate, Constants.SaveType.EDIT, false));

        private static async Task<ResponseResult> Save(PortfolioInternalUpdate oPortfolioInternalUpdate, string mode, bool addOrUpdate)
        {
            return await Task.Run(() =>
            {
                oPortfolioInternalUpdate.Posted = false;
                oPortfolioInternalUpdate.Reversed = false;
                oPortfolioInternalUpdate.SaveType = mode;
                oPortfolioInternalUpdate.AddOrDeduct = addOrUpdate;
                var oSaveStatus = oPortfolioInternalUpdate.Save();
                switch (oSaveStatus)
                {
                    case DataGeneral.SaveStatus.Saved when oPortfolioInternalUpdate.SaveType.Trim() == Constants.SaveType.ADDS:
                        return ResponseResult.Error("Portfolio Balance Update(Add/Deduct) Added Successfully");
                    case DataGeneral.SaveStatus.Saved:
                        return ResponseResult.Error( "Portfolio Balance Update(Add/Deduct) Modified Successfully");
                    case DataGeneral.SaveStatus.NotExist:
                        return ResponseResult.Error("Cannot Update. Portfolio Balance Update(Add/Deduct) Does Not Exist");
                    case DataGeneral.SaveStatus.NameExistAdd:
                        var oPortfolio = new Portfolio
                        {
                            CustomerAcct = oPortfolioInternalUpdate.CustomerId,
                            StockCode = oPortfolioInternalUpdate.StockCode,
                            PurchaseDate = oPortfolioInternalUpdate.EffDate.ToExact()
                        };
                        return ResponseResult.Error("Error In Saving Portfolio Balance Update(Add/Deduct)," +
                                                    "Customer Does Not Have Enough Stock To Transfer.Portfolio Amount Is " 
                                                    + oPortfolio.GetNetHolding().ToString("D") + " units");
                    default:
                        return ResponseResult.Error();
                }
            });
        }
        public async Task<ResponseResult> PostPortfolioInternalUpdate(string code)
        {
            return await Task.Run(() =>
            {
                var oPortfolioInternalUpdate = new PortfolioInternalUpdate
                {
                    TransNo = code.Trim()
                };
                var strUserName = GeneralFunc.UserName;
                 if (oPortfolioInternalUpdate.GetPortfolioInternalUpdate(DataGeneral.PostStatus.UnPosted))
            {
                var oPortfolio = new Portfolio
                {
                    CustomerAcct = oPortfolioInternalUpdate.CustomerId,
                    StockCode = oPortfolioInternalUpdate.StockCode,
                    PurchaseDate = oPortfolioInternalUpdate.EffDate
                };
                if (!oPortfolioInternalUpdate.AddOrDeduct)
                {
                    if (oPortfolio.GetNetHolding() < oPortfolioInternalUpdate.Quantity)
                        return ResponseResult.Error("Error In Posting Portfolio Transfer,Customer Does Not Have Enough Stock To Transfer.Portfolio Amount Is " + oPortfolio.GetNetHolding().ToString("d") + " units");
                }
                 
                if (oPortfolioInternalUpdate.EffDate > GeneralFunc.GetTodayDate())
                    return ResponseResult.Error( "Cannot Post Add/Deduct Portfolio Holding Balance,Transfer Date In The Future");
                
                    var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
                    using (var connection = db.CreateConnection() as SqlConnection)
                    {
                        connection.Open();
                        var transaction = connection.BeginTransaction();
                        try
                        {
                            var oPort = new Portfolio
                            {
                                PurchaseDate = oPortfolioInternalUpdate.EffDate,
                                CustomerAcct = oPortfolioInternalUpdate.CustomerId,
                                StockCode = oPortfolioInternalUpdate.StockCode,
                                Units = oPortfolioInternalUpdate.Quantity,
                                UnitPrice = oPortfolioInternalUpdate.UnitPrice,
                                ActualUnitCost = oPortfolioInternalUpdate.UnitPrice,
                                Ref01 = oPortfolioInternalUpdate.TransNo

                            };
                            oPort.TotalCost = Decimal.Parse(oPort.ActualUnitCost.ToString()) * oPort.Units;
                            if (oPortfolioInternalUpdate.AddOrDeduct)
                            {
                                oPort.DebCred = "C";
                            }
                            else
                            {
                                oPort.DebCred = "D";
                            }
                            oPort.TransDesc = oPortfolioInternalUpdate.Description + " " + oPortfolioInternalUpdate.Quantity.ToString().Trim() + " " + " @ " + oPortfolioInternalUpdate.UnitPrice.ToString().Trim();
                            oPort.TransType = "PORTIUPD";
                            oPort.SysRef = "PORTIUP-" + oPortfolioInternalUpdate.TransNo;
                            oPort.MarginCode = "";
                            var dbCommandPortTotal = oPort.AddCommand();
                            db.ExecuteNonQuery(dbCommandPortTotal, transaction);

                            var oStock = new Stock();
                            var oStkParam = new StkParam();
                            
                            DataGeneral.StockInstrumentType oStockInstrumentType;
                            oStock.SecCode = oPortfolioInternalUpdate.StockCode.ToString().Trim();
                            oStockInstrumentType = oStock.GetInstrumentTypeUsingStockCode();
                            
                            var oProductAcct = new ProductAcct();
                            var oProduct = new Product();
                            var oGLParam = new GLParam();
                            var oBranch = new Branch();
                            oProductAcct.ProductCode = oStkParam.Product;
                            oProductAcct.CustAID = oPortfolioInternalUpdate.CustomerId;
                            if (oProductAcct.GetBoxLoadStatus() && oPort.TotalCost > 0)
                            {
                                var strJnumberNext = "";
                                var oCommandJnumber = db.GetStoredProcCommand("JnumberAdd") as SqlCommand;
                                db.AddOutParameter(oCommandJnumber, "Jnumber", SqlDbType.BigInt, 8);
                                db.ExecuteNonQuery(oCommandJnumber, transaction);
                                strJnumberNext = db.GetParameterValue(oCommandJnumber, "Jnumber").ToString();

                                var oGL = new AcctGL
                                {
                                    EffectiveDate = oPortfolioInternalUpdate.EffDate
                                };
                                if (oStockInstrumentType == DataGeneral.StockInstrumentType.NASD)
                                {
                                    oProduct.TransNo = oStkParam.ProductInvestmentNASD;
                                    oGL.MasterID = oProduct.GetProductGLAcct();
                                    oGL.AcctRef = oStkParam.ProductInvestmentNASD;
                                }
                                else
                                {
                                    oProduct.TransNo = oStkParam.ProductInvestment;
                                    oGL.MasterID = oProduct.GetProductGLAcct();
                                    oGL.AcctRef = oStkParam.ProductInvestment;
                                }
                                oGL.AccountID = oPortfolioInternalUpdate.CustomerId;
                                if (oPortfolioInternalUpdate.AddOrDeduct)
                                {
                                    oGL.Credit = 0;
                                    oGL.Debit = Math.Round(oPort.TotalCost, 2);
                                    oGL.Debcred = "D";
                                    oGL.Desciption = "Stock Addition: " + oPortfolioInternalUpdate.Quantity.ToString().Trim() + " " + oPortfolioInternalUpdate.StockCode.ToString().Trim() + " @ " + oPortfolioInternalUpdate.UnitPrice.ToString("n").Trim();
                                }
                                else
                                {
                                    oGL.Credit = Math.Round(oPort.TotalCost, 2); 
                                    oGL.Debit = 0;
                                    oGL.Debcred = "C";
                                    oGL.Desciption = "Stock Deduction: " + oPortfolioInternalUpdate.Quantity.ToString().Trim() + " " + oPortfolioInternalUpdate.StockCode.ToString().Trim() + " @ " + oPortfolioInternalUpdate.UnitPrice.ToString("n").Trim();
                                }
                                oGL.TransType = "PORTIUPD";
                                oGL.SysRef = "PORTIUP" + "-" + oPortfolioInternalUpdate.TransNo;
                                oGL.Ref01 = oPortfolioInternalUpdate.TransNo;
                                oGL.Chqno = "";
                                oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                                oGL.Ref02 = oPortfolioInternalUpdate.CustomerId;
                                oGL.RecAcctMaster = oGLParam.GLOpenAcct;
                                oGL.RecAcct = "";
                                oGL.AcctRefSecond = "";
                                oGL.Reverse = "N";
                                oGL.Jnumber = strJnumberNext;
                                oGL.Reverse = "N";
                                oGL.Branch = oBranch.DefaultBranch;
                                oGL.FeeType = "";
                                var dbCommandCustAccount = oGL.AddCommand();
                                db.ExecuteNonQuery(dbCommandCustAccount, transaction);

                                oGL.EffectiveDate = oPortfolioInternalUpdate.EffDate;
                                oGL.MasterID = oGLParam.GLOpenAcct;
                                oGL.AccountID = "";
                                oGL.AcctRef = "";
                                if (oPortfolioInternalUpdate.AddOrDeduct)
                                {
                                    oGL.Credit = Math.Round(oPort.TotalCost, 2);
                                    oGL.Debit = 0;
                                    oGL.Debcred = "C";
                                    oGL.Desciption = "Stock Addition: " + oPortfolioInternalUpdate.Quantity.ToString().Trim() + " " + oPortfolioInternalUpdate.StockCode.ToString().Trim() + " @ " + oPortfolioInternalUpdate.UnitPrice.ToString("n").Trim();
                                }
                                else
                                {
                                    oGL.Debit = Math.Round(oPort.TotalCost, 2);
                                    oGL.Credit = 0;
                                    oGL.Debcred = "D";
                                    oGL.Desciption = "Stock Deduction: " + oPortfolioInternalUpdate.Quantity.ToString().Trim() + " " + oPortfolioInternalUpdate.StockCode.ToString().Trim() + " @ " + oPortfolioInternalUpdate.UnitPrice.ToString("n").Trim();
                                }
                                oGL.TransType = "PORTIUPD";
                                oGL.SysRef = "PORTIUP" + "-" + oPortfolioInternalUpdate.TransNo;
                                oGL.Ref01 = oPortfolioInternalUpdate.TransNo;
                                oGL.Chqno = "";
                                oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                                oGL.Ref02 = oPortfolioInternalUpdate.CustomerId;
                                if (oStockInstrumentType == DataGeneral.StockInstrumentType.NASD)
                                {
                                    oProduct.TransNo = oStkParam.ProductInvestmentNASD;
                                    oGL.RecAcctMaster = oProduct.GetProductGLAcct();
                                    oGL.AcctRefSecond = oStkParam.ProductInvestmentNASD;
                                }
                                else
                                {
                                    oProduct.TransNo = oStkParam.ProductInvestment;
                                    oGL.RecAcctMaster = oProduct.GetProductGLAcct();
                                    oGL.AcctRefSecond = oStkParam.ProductInvestment;
                                }

                                oGL.RecAcct = oPortfolioInternalUpdate.CustomerId;
                                oGL.Reverse = "N";
                                oGL.Jnumber = strJnumberNext;
                                oGL.Reverse = "N";
                                oGL.Branch = oBranch.DefaultBranch;
                                oGL.FeeType = "";
                                var dbCommandOpenGLAcct = oGL.AddCommand();
                                db.ExecuteNonQuery(dbCommandOpenGLAcct, transaction);
                            }


                            var oCommandUpdatePost = new SqlCommand();
                            oCommandUpdatePost = db.GetStoredProcCommand("PortfolioInternalUpdateUpdatePost") as SqlCommand;
                            db.AddInParameter(oCommandUpdatePost, "Code", SqlDbType.VarChar, oPortfolioInternalUpdate.TransNo.Trim());
                            db.AddInParameter(oCommandUpdatePost, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                            db.ExecuteNonQuery(oCommandUpdatePost, transaction);
                            transaction.Commit();
                            return ResponseResult.Success();
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.Message, ex);
                            transaction?.Rollback();
                            return ResponseResult.Error(ex.Message);
                        }

                    }
                
            }
                return ResponseResult.Error("Error In Posting,Add/Deduct Portfolio Holding Balance Does Not Exist");
            });
        }
    }
}