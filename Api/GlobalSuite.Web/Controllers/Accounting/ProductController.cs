using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using GL.Business;
using GlobalSuite.Core.Accounting.Models;
using GlobalSuite.Web.Controllers.Accounting.Dto;

namespace GlobalSuite.Web.Controllers.Accounting
{
    public partial class AccountingController
    {
        /// <summary>
        /// Get All Products
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.Products)]
        [ResponseType(typeof(List<ProductDetailResponse>))]
        public async Task<IHttpActionResult> GetProducts()
        {
            var products = await _accountingService.GetProducts();
            var res = _mapper.Map<List<ProductDetailResponse>>(products);
                return Ok(res);
        } 
        // /// <summary>
        // /// Get All Product Classes
        // /// </summary>
        // /// <returns></returns>
        // [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.Products+"/class")]
        // [ResponseType(typeof(List<ProductClassResponse>))]
        // public async Task<IHttpActionResult> GetProductClass()
        // {
        //     var products = await _accountingService.GetAllProductClass();
        //     var res = _mapper.Map<List<ProductClassResponse>>(products);
        //         return Ok(res);
        // }  
        //  
        // /// <summary>
        // /// Get All Product Types
        // /// </summary>
        // /// <returns></returns>
        // [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.Products+"/type")]
        // [ResponseType(typeof(List<ProductTypeResponse>))]
        // public async Task<IHttpActionResult> GetProductType()
        // {
        //     var products = await _accountingService.GetAllProductTypes();
        //     var res = _mapper.Map<List<ProductTypeResponse>>(products);
        //         return Ok(res);
        // }  
         
    }
}