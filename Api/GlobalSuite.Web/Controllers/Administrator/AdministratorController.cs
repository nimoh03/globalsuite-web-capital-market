using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using BaseUtility.Business;
using GlobalSuite.Core;
using GlobalSuite.Core.Admin;
using GlobalSuite.Core.Helpers;

namespace GlobalSuite.Web.Controllers
{
    public partial class AdministratorController:BaseController
    {
        private readonly IAdminService _adminService;
        private readonly IMapper _mapper;
        public AdministratorController(IAdminService adminService, IMapper mapper)
        {
            _adminService = adminService;
            _mapper = mapper;
        }
        
        /// <summary>
        /// Posting Statuses
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AdministratorRoutes.MaintainRoutes.PostingStatus)]
        [System.Web.Mvc.OutputCache(CacheProfile = Constants.Caching.Cache1Day)]
        [ResponseType(typeof(EnumObject))]
        public IHttpActionResult GetStatuses()
        {
            var statuses = EnumHelper.ToEnumArray<DataGeneral.PostStatus>();

            return Ok(statuses);
        }
        
        /// <summary>
        /// Allotment Types
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AdministratorRoutes.MaintainRoutes.AllotmentTypes)]
        [System.Web.Mvc.OutputCache(CacheProfile = Constants.Caching.Cache1Day)]
        [ResponseType(typeof(EnumObject))]
        public IHttpActionResult AllotmentTypes()
        {
            var types = EnumHelper.ToEnumArray<DataGeneral.AllotmentType>();

            return Ok(types);
        }
        /// <summary>
        /// GL Instrument Types
        /// C -- Cash Q -- Clearing Cheque H -- Cash Cheque N -- NIBSS R -- TRANSFER
        /// U-- Cheque S--Internal Customer Transfer O-- Others V-- Dividend M- Commission
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AdministratorRoutes.MaintainRoutes.GlInstrumentTypes)]
        [System.Web.Mvc.OutputCache(CacheProfile = Constants.Caching.Cache1Day)]
        [ResponseType(typeof(EnumObject))]
        public IHttpActionResult GlInstrumentTypes()
        {
            var values = Enum.GetValues(typeof(DataGeneral.GLInstrumentType));
            var response = (from DataGeneral.GLInstrumentType value in values 
                let description = value.GetDescription()
                select new EnumObject { Id = (int)value, Name = description }).ToList();
            return Ok(response);
        }
        
        /// <summary>
        /// Interest Types
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AdministratorRoutes.MaintainRoutes.InterestTypes)]
        [System.Web.Mvc.OutputCache(CacheProfile = Constants.Caching.Cache1Day)]
        [ResponseType(typeof(EnumObject))]
        public IHttpActionResult InterestTypes()
        {
            var types = EnumHelper.ToEnumArray<DataGeneral.InterestType>();

            return Ok(types);
        }
        /// <summary>
        /// Interest Types
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AdministratorRoutes.MaintainRoutes.InterestPeriods)]
        [System.Web.Mvc.OutputCache(CacheProfile = Constants.Caching.Cache1Day)]
        [ResponseType(typeof(EnumObject))]
        public IHttpActionResult InterestPeriods()
        {
            var types = EnumHelper.ToEnumArray<DataGeneral.InterestPeriod>();

            return Ok(types);
        } 
        
        /// <summary>
        /// Stock Instrument Types
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AdministratorRoutes.MaintainRoutes.StockInstrumentTypes)]
        [System.Web.Mvc.OutputCache(CacheProfile = Constants.Caching.Cache1Day)]
        [ResponseType(typeof(EnumObject))]
        public IHttpActionResult StockInstrumentTypes()
        {
            var types = EnumHelper.ToEnumArray<DataGeneral.StockInstrumentType>();

            return Ok(types);
        }
        
        /// <summary>
        /// Customer Type Codes
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AdministratorRoutes.MaintainRoutes.CustomerTypeCodes)]
        [System.Web.Mvc.OutputCache(CacheProfile = Constants.Caching.Cache1Day)]
        [ResponseType(typeof(EnumObject))]
        public IHttpActionResult CustomerTypeCode()
        {
            var types = EnumHelper.ToEnumArray<DataGeneral.CustomerTypeCode>();

            return Ok(types);
        }
    }
}