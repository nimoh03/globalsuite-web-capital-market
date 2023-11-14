using System;
using System.Threading.Tasks;
using CapitalMarket.Business;
using GlobalSuite.Core.Helpers;

namespace GlobalSuite.Core.CapitalMarket
{
    public partial class TradingService
    {
        public async Task<ResponseResult> AddBroker(Broker oBroker)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (oBroker.BrokerNameExistNoTransNo())
                        return ResponseResult.Error("Cannot Save,Broker Name Already Exist");
                    if (oBroker.BrokerCodeExistNoTransNo())
                        return ResponseResult.Error("Cannot Save,Broker Code Already Exist");

                    if (oBroker.GetBroker()) return ResponseResult.Error("Broker Code Already Exist.");
                    return oBroker.Add() ? ResponseResult.Success() : ResponseResult.Error( "Error In Adding New Broker.");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message,ex);
                    return ResponseResult.Error(ex.Message);
                }
               
            });
        } 
        public async Task<Broker> GetBroker(string code)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var oBroker = new Broker
                    {
                        Transno = code
                    };
                    var brokerExists = oBroker.GetBroker();
                    return brokerExists ? oBroker : null;

                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message,ex);
                    return null;
                }
            });
        } public async Task<ResponseResult> EditBroker(Broker oBroker)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var brokerExists = oBroker.GetBroker();
                    if (!brokerExists) return ResponseResult.Error("Broker cannot be found.");
                    if (oBroker.BrokerNameExist())
                        return ResponseResult.Error("Cannot Edit,Broker Name Already Exist");
                    if (oBroker.BrokerCodeExist())
                        return ResponseResult.Error( "Cannot Edit,Broker Code Already Exist");
                    return oBroker.Edit() ? ResponseResult.Success() : ResponseResult.Error( "Error Editing Broker.");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message,ex);
                    return ResponseResult.Error(ex.Message);
                }
            });
        }
        public async Task<ResponseResult> DeleteBroker(string code)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var oBroker = new Broker
                    {
                        Transno = code
                    };
                    var brokerExists = oBroker.GetBroker();
                    if (!brokerExists) return ResponseResult.Error("Broker Does Not Exist.");
                   
                    return oBroker.Delete() ? ResponseResult.Success() : ResponseResult.Error( "Error Deleting Broker.");
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