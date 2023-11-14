using System;
using System.Threading.Tasks;
using CapitalMarket.Business;
using GlobalSuite.Core.Helpers;

namespace GlobalSuite.Core.CapitalMarket
{
    public partial class TradingService
    {
        public async Task<ResponseResult> AddStock(Stock oStock)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (oStock.StockNameExistNoTransNo())
                        return ResponseResult.Error("Cannot Save,Stock Name Already Exist");
                    if (oStock.StockCodeExistNoTransNo())
                        return ResponseResult.Error("Cannot Save,Stock Code Already Exist");

                    if (oStock.GetStock()) return ResponseResult.Error("Stock Code Already Exist.");
                    return oStock.Add() ? ResponseResult.Success() : ResponseResult.Error( "Error In Adding New Stock.");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message,ex);
                    return ResponseResult.Error(ex.Message);
                }
               
            });
        } 
        public async Task<Stock> GetStock(string code)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var oStock = new Stock
                    {
                        KeepStockCode = code
                    };
                    var stockExists = oStock.GetStock();
                    return stockExists ? oStock : null;

                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message,ex);
                    return null;
                }
            });
        } public async Task<ResponseResult> EditStock(Stock oStock)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var stockExists = oStock.GetStock();
                    if (!stockExists) return ResponseResult.Error("Stock cannot be found.");
                    if (oStock.StockNameExist())
                        return ResponseResult.Error("Cannot Edit,Stock Name Already Exist");
                    if (oStock.StockCodeExist())
                        return ResponseResult.Error( "Cannot Edit,Stock Code Already Exist");
                    return oStock.Edit() ? ResponseResult.Success() : ResponseResult.Error( "Error Editing Stock.");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message,ex);
                    return ResponseResult.Error(ex.Message);
                }
            });
        }
        public async Task<ResponseResult> DeleteStock(string code)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var oStock = new Stock
                    {
                        KeepStockCode = code
                    };
                    var stockExists = oStock.GetStock();
                    if (!stockExists) return ResponseResult.Error("Stock Does Not Exist.");
                   
                    return oStock.Delete() ? ResponseResult.Success() : ResponseResult.Error( "Error Deleting Stock.");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message,ex);
                    return ResponseResult.Error(ex.Message);
                }
            });
        }
        
    }
}