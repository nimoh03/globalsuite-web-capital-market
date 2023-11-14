using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Admin.Business;
using BaseUtility.Business;
using CustomerManagement.Business;
using GL.Business;
using GlobalSuite.Core.Accounting.Models;
using GlobalSuite.Core.Helpers;

namespace GlobalSuite.Core.Accounting
{
    public partial class AccountingService
    {
        public async Task<List<Product>> GetProducts( )
        {
            return await Task.Run(() =>
            {
                var oProduct = new Product();
                var ds = oProduct.GetAll();
                var oList = new List<Product>();
                foreach (DataRow oRow in ds.Tables[0].Rows)
                {
                    oList.Add(new Product
                    {
                        TransNo = $"{oRow["ProductCode"]}",
                        Description = $"{oRow["ProductName"]}",
                        GLAcct = $"{oRow["GLAcct"]}",
                        ModName = $"{oRow["ModuleName"]}",
                        ProductClass = $"{oRow["ProductClass"]}",
                        ProductType = Convert.ToInt32(oRow["ProductType"]),
                        DefaultProduct = $"{oRow["DefaultProduct"]}"=="Y",
                    });
                   
                }

                return oList;
            });
        }
        public async Task<List<ProductClass>> GetAllProductClass( )
        {
            return await Task.Run(() =>
            {
                var oProduct = new ProductClass();
                var ds = oProduct.GetAll();
                return ds.Tables[0].ToList<ProductClass>();
            });
        }
 public async Task<List<ProductType>> GetAllProductTypes( )
        {
            return await Task.Run(() =>
            {
                var oProduct = new ProductType();
                var ds = oProduct.GetAll();
                return ds.Tables[0].ToList<ProductType>();
            });
        }

    
    }
}