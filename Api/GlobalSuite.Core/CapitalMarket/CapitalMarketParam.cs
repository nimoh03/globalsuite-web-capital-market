using System.Threading.Tasks;
using CapitalMarket.Business;
using GL.Business;
using GlobalSuite.Core.Helpers;

namespace GlobalSuite.Core.CapitalMarket
{
    public partial class TradingService
    {
        public async Task<ResponseResult> AddCapitalMarketParam( string memberCode, StkParam oStkbPGenTable, PGenTable oPGenTable)
        {
            var oCompany = new Company
            {
                MemberCode = memberCode
            };
            return await Task.Run(() =>
            {
                var stkParamSaved = oStkbPGenTable.Add();
                if (stkParamSaved)
                {
                    if (oPGenTable.AddCapitalMarket())
                    {
                        if (!oCompany.SaveMemberCode())
                            return ResponseResult.Error("Error In Saving Member Code");
                    }
                    else
                    {
                        return ResponseResult.Error("Error In Adding Accounts Capital Market Parameter");
                    }
                }
                else
                {
                    return ResponseResult.Error("Error In Adding Fees Capital Market Parameter");
                }
                return ResponseResult.Success();
            });
        }
    }
}