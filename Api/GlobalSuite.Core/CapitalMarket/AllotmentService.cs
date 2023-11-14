using System.Threading.Tasks;
using BaseUtility.Business;
using CapitalMarket.Business;
using GlobalSuite.Core.Helpers;

namespace GlobalSuite.Core.CapitalMarket
{
    public partial class TradingService
    {
        public async Task<ResponseResult> AllotmentBuy(Allotment oAllotment)
        {
            oAllotment.UserId = GeneralFunc.UserName.ToUpper();
            oAllotment.Buy_sold_Ind = char.Parse("B");
            oAllotment.Cdeal = char.Parse("N");
            oAllotment.Posted = false;
            oAllotment.Reversed = false;
            oAllotment.CDSellTrans = "";
            oAllotment.OtherCust = "";
            oAllotment.MarginCode = ""; 
            oAllotment.CrossType = "";
            oAllotment.PrintFlag = 'N';
            oAllotment.SaveType = Constants.SaveType.ADDS;
            return await Task.Run(() =>
            {
                var saveStatus = oAllotment.Save();
                return saveStatus != DataGeneral.SaveStatus.Saved ? ResponseResult.Error(saveStatus.ToString()) : ResponseResult.Success();
            });
        }
        public async Task<ResponseResult> AllotmentSell(Allotment oAllotment)
        {
            oAllotment.UserId = GeneralFunc.UserName.ToUpper();
            oAllotment.Buy_sold_Ind = char.Parse("S");
            oAllotment.Cdeal = char.Parse("N");
            oAllotment.Posted = false;
            oAllotment.Reversed = false;
            oAllotment.CDSellTrans = "";
            oAllotment.OtherCust = "";
            oAllotment.MarginCode = ""; 
            oAllotment.CrossType = "";
            oAllotment.PrintFlag = 'N';
            oAllotment.SaveType = Constants.SaveType.ADDS;
            return await Task.Run(() =>
            {
              // TODO Validate Portfolio
                var saveStatus = oAllotment.Save();
                return saveStatus != DataGeneral.SaveStatus.Saved ? ResponseResult.Error(saveStatus.ToString()) : ResponseResult.Success();
            });
        }
        public async Task<ResponseResult> AllotmentCrossDeal(Allotment oAllotmentBuy, Allotment oAllotmentSale)
        {
            oAllotmentBuy.UserId = GeneralFunc.UserName.ToUpper();
            oAllotmentBuy.Buy_sold_Ind = char.Parse("B");
            oAllotmentBuy.Cdeal = char.Parse("Y");
            oAllotmentBuy.Posted = false;
            oAllotmentBuy.Reversed = false;
            oAllotmentBuy.CDSellTrans = "";
            oAllotmentBuy.MarginCode = "";
            oAllotmentBuy.PrintFlag = 'N';
            oAllotmentBuy.SaveType = Constants.SaveType.ADDS;
            return await Task.Run(() =>
            {
                var oAllotmentCross = new Allotment
                {
                    TransNoRev = oAllotmentBuy.TransNoRev,
                    SaveType = Constants.SaveType.ADDS
                };
                if (string.IsNullOrEmpty(oAllotmentBuy.CrossType)) return ResponseResult.Error("Cross Type is not set");
                if (string.IsNullOrEmpty(oAllotmentSale.SoldBy)) return ResponseResult.Error("Sold By is not set");
                if (string.IsNullOrEmpty(oAllotmentSale.BoughtBy)) return ResponseResult.Error("Bought By is not set");
                var saveStatus = oAllotmentCross.SaveCross(oAllotmentBuy,oAllotmentSale);
                return saveStatus != DataGeneral.SaveStatus.Saved ? ResponseResult.Error(saveStatus.ToString()) : ResponseResult.Success();
            });
        }
    }
}