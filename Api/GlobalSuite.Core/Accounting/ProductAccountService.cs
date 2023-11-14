using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Admin.Business;
using BaseUtility.Business;
using CustomerManagement.Business;
using GL.Business;
using GlobalSuite.Core.Accounting.Models;
using GlobalSuite.Core.Helpers;
using DataTableExtensions = GlobalSuite.Core.Helpers.DataTableExtensions;

namespace GlobalSuite.Core.Accounting
{
    public partial class AccountingService
    {
        public async Task<List<ProductAccountResponse>> GetAllSubsidiaryAccounts(string productCode, string branchCode, string keyword=" ")
        {
            return await Task.Run(() =>
            {
                var oProductAcct = new ProductAcct
                {
                    Branch = branchCode,
                    CustAID = string.Empty,
                    CsCsAcct = string.Empty,
                    CsCsReg = string.Empty,
                };
                var ds = oProductAcct.GetAllByProductSearch(productCode, keyword,
                    "ALL","ALL","N","ALL" );
                // var dt = ds.Tables[0].AsEnumerable().Take(50);
                // var oList = new List<ProductAccountResponse>();
                // foreach (var row in dt)
                // {
                //     if($"{row["TransNo"]}"=="1") continue;
                //     var p = DataTableExtensions.GetItem<ProductAccountResponse>(row);
                //     p.CscsAccount = $"{row["CsCs Account"]}";
                //     p.CscsReg = $"{row["CsCs Reg"]}";
                //     oList.Add(p);
                // }
                
                return ds.Tables[0].ToList<ProductAccountResponse>();
            });
        }
    }
}