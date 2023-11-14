using System.Threading.Tasks;
using CapitalMarket.Business;
using GL.Business;
using GlobalSuite.Core.Helpers;

namespace GlobalSuite.Core.CapitalMarket
{
    public partial class TradingService
    {
        public async Task<ResponseResult> AddCustomerStock(ProductAcct oProductAcct)
        {

            return await Task.Run(() =>
            {
                var oStkParam=new StkParam();
                oProductAcct.ProductCode = oStkParam.Product;
                oProductAcct.ProductCodeAgent = oStkParam.ProductBrokPay;
                oProductAcct.ProductCodeInvestment = oStkParam.ProductInvestment;
                oProductAcct.ProductCodeNASD = oStkParam.ProductNASDAccount;
                var isBoxLoad = oProductAcct.BoxLoad == "Y";
                var isAgentInd = oProductAcct.AgentInd == "Y";
                if (isBoxLoad && isAgentInd)
                    return ResponseResult.Error(
                        "Customer Cannot Be Both An Investment and Agent Account, Please Choose Only One");
                if (isAgentInd && oProductAcct.CustAID != oProductAcct.Agent ||
                    string.IsNullOrWhiteSpace(oProductAcct.Agent))
                    return ResponseResult.Error("Error Saving Customer, Agent Not Equal To Customer Account Or Empty");

                if (!isAgentInd && oProductAcct.AgentComm != 0)
                    return ResponseResult.Error("Cannot Specify Agent Commission Customer Is Not An Agent Account");


                oProductAcct.SaveType = Constants.SaveType.ADDS;

                var oGlParam = new GLParam
                {
                    Type = "CUSTODIAN"
                };
                var strCustodianAcct = oGlParam.CheckParameter();
                if (strCustodianAcct.Trim() == "YES" && oProductAcct.IsCustodian && oProductAcct.ChkAccessCodeExist())
                    return ResponseResult.Error(
                        "Cannot Create Stockbroking Custodian Account! Access Code Already Exist");

                var oSaveStatus = oProductAcct.Save(ProductAcct.ProductType.StockBroking);
                switch (oSaveStatus)
                {
                    case ProductAcct.SaveStatus.Saved:
                        return ResponseResult.Success("Customer Account Saved Successfully!");
                    case ProductAcct.SaveStatus.NotExist:
                        return ResponseResult.Error("Cannot Edit Account Customer Does Not Exist");
                    case ProductAcct.SaveStatus.CsCsAcctExistAdd:
                        return ResponseResult.Error("Error In Adding New Customer,CsCs Account Number Already Exist");
                    case ProductAcct.SaveStatus.CsCsAcctExistEdit:
                        return ResponseResult.Error("Error In Editing Customer,CsCs Account Number Already Exist");
                    case ProductAcct.SaveStatus.CsCsRegExistAdd:
                        return ResponseResult.Error(
                            "Error In Adding New Customer,CsCs Clearing House Number Already Exist");
                    case ProductAcct.SaveStatus.CsCsRegExistEdit:
                        return ResponseResult.Error(
                            "Error In Editing Customer,CsCs Clearing House Number Already Exist");
                    case ProductAcct.SaveStatus.NASDCsCsAcctExistAdd:
                        return ResponseResult.Error(
                            "Error In Adding New Customer,NASD CsCs Account Number Already Exist");
                    case ProductAcct.SaveStatus.NASDCsCsAcctExistEdit:
                        return ResponseResult.Error("Error In Editing Customer,NASD CsCs Account Number Already Exist");
                    case ProductAcct.SaveStatus.AgentExist:
                        return ResponseResult.Error(
                            "Cannot Create Investment Account,Agent Account Exist For Customer");
                    case ProductAcct.SaveStatus.AttachedToAgentExist:
                        return ResponseResult.Error("Cannot Create Investment Account,Agent Attached To Customer");
                    case ProductAcct.SaveStatus.BoxLoadExist:
                        return ResponseResult.Error(
                            "Cannot Create Agent Account,Investment Account Exist For Customer");
                    case ProductAcct.SaveStatus.BoxLoadUnCheck:
                        return ResponseResult.Error(
                            "Cannot Modify Investment Account Customer Already Has A Investment Account Posted To");
                    case ProductAcct.SaveStatus.AgentUnCheck:
                        return ResponseResult.Error(
                            "Cannot Modify Agent Account Customer Already Has An Agent Account Posted To");
                    case ProductAcct.SaveStatus.NotSaved:
                        return ResponseResult.Error("Error In Creating Stock Account For Customer");
                    case ProductAcct.SaveStatus.AttachedToAgent:
                        return ResponseResult.Error("Investment Account Cannot Be Attached To An Agent Account");
                    case ProductAcct.SaveStatus.AgentCommIncluded:
                        return ResponseResult.Error("Investment Account Cannot Have Agent Commission Percentage");
                    case ProductAcct.SaveStatus.AccountIdExistAdd:
                        return ResponseResult.Error("StockBroking Account For This Customer Already Created");
                    default:
                        return ResponseResult.Error();
                }
            });
        }

        public async Task<ResponseResult> DeleteCustomerStock(string code)
        {
            return await Task.Run(() =>
            {
                var oStkParam = new StkParam();
                var oProductAcct = new ProductAcct
                {
                    TransNo = code
                };
                if (oProductAcct.GetProductAcct())
                {
                    var oGl = new AcctGL
                    {
                        MasterID = oProductAcct.ProductCode,
                        AccountID = oProductAcct.CustAID
                    };
                    if (!oGl.CheckTransExistCustomer())
                    {
                        oGl.MasterID = oStkParam.ProductBrokPay;
                        oGl.AccountID = oProductAcct.CustAID;
                        if (!oGl.CheckTransExistCustomer())
                        {
                            if (!oProductAcct.ChkAgentHasCustomerAttached(oProductAcct.CustAID, oStkParam.Product))
                            {
                                if (oProductAcct.Delete())
                                {
                                    oProductAcct.ProductCode = oStkParam.ProductBrokPay;
                                    oProductAcct.GetCustomerByCustId();
                                    if (oProductAcct.Delete())
                                        return ResponseResult.Success();
                                }
                                else
                                    return ResponseResult.Error("Error In Deleting Customer Account");
                            }
                            else
                                return ResponseResult.Error(
                                    "Cannot Delete Agent Account Customers Already Attached, Try UnAttaching The Customers And Saving The Change");
                        }
                        else
                            return ResponseResult.Error(
                                "Cannot Delete Customer Agent Account, GL Transactions Exist For This Customer Agent Account");
                    }
                    else
                        return ResponseResult.Error(
                            "Cannot Delete Customer Account, GL Transactions Exist For This Customer Account");
                }
                else
                    return ResponseResult.Error("Customer Account Does Not Exist.");

                return ResponseResult.Error();
            });
        }
    }
}