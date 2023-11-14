using System.Threading.Tasks;
using BaseUtility.Business;
using CapitalMarket.Business;
using GlobalSuite.Core.Helpers;

namespace GlobalSuite.Core.CapitalMarket
{
    public partial class TradingService 
    {
        public async Task<ResponseResult> TradeValuation(Valuation oValuation)
        {
            return await Task.Run(() =>
            {
                oValuation.UserID = GeneralFunc.UserName.ToUpper();
                oValuation.Cdeal = "N";
                oValuation.SaveType = Constants.SaveType.ADDS;
                var oSaveStatus = oValuation.Save();
                return oSaveStatus == DataGeneral.SaveStatus.Saved ? ResponseResult.Success() : ResponseResult.Error("Stock valuation cannot be saved.");
            });
        }
    }
}