using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseUtility.Business;
using GL.Business;
using GlobalSuite.Core.Admin.Models;
using GlobalSuite.Core.Caching;
using GlobalSuite.Core.Helpers;

namespace GlobalSuite.Core.Admin
{
    public partial class AdminService :BaseService, IAdminService
    {
        private readonly Caching<Param> _caching = new Caching<Param>();

        private async Task<List<Param>> GetAllParams(string tableName)
        {
            return await Task.Run(() =>
            {
                var items = _caching.GetAll($"{tableName}_all");
                if (items != null && items.Any()) return items;
                var oMParam = new mParam();
                var dt = oMParam.GetAll(tableName, "NO");
                items= dt.Tables[0].ToList<Param>();
                _caching.Add($"{tableName}_all",items);

                return items;
            });

        }
        private async Task<string> GetTransactionNoFromBucket(string tableName)
        {
            return await Task.Run(() =>
            {
                var oBucket = new Bucket();
                return oBucket.GetNextBucketNoNonIdentity(tableName.ToUpper())
                    .PadLeft(4, char.Parse("0"));
            });
        }

        private async Task<ResponseResult> SaveParam(string tableName, string description)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    var table = tableName.Trim().ToUpper();
                    var oMParam = new mParam
                    {
                        TransNo = await GetTransactionNoFromBucket(table),
                        Descrip = description,
                        TableName = table,
                        SaveType = Constants.SaveType.ADDS
                    };
                    _caching.Remove($"{tableName}_all");
                    var oSaveStatus = oMParam.Save();
                    switch (oSaveStatus)
                    {
                        case DataGeneral.SaveStatus.NotExist:
                            return ResponseResult.Error("Cannot Edit, Title Code Does Not Exist");
                        case DataGeneral.SaveStatus.NameExistAdd:
                            return ResponseResult.Error("Error Saving Title,Title Already Exist.");
                        default:
                            return ResponseResult.Success();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    return ResponseResult.Error(ex.Message);
                }
                
            });
        }
     }
}
