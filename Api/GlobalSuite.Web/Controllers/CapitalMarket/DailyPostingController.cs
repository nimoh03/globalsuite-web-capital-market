using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using BaseUtility.Business;
using GL.Business;

namespace GlobalSuite.Web.Controllers.CapitalMarket
{

    public partial class CapitalMarketController : BaseController
    {
        [HttpPost, Route(GlobalSuiteRoutes.CapitalMarketRoutes.TradingRoutes.DailyPosting)]
        public async Task<IHttpActionResult> Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            var files = HttpContext.Current.Request.Files;
            if (files.Count <= 0) return BadRequest("No Files found.");
            var form = HttpContext.Current.Request.Form;
            var dateStr = form["date"];
            var date = dateStr.ToDate();
            var allowedExtensions = new string[] { ".txt", ".csv" };
            var file = files[0];
            if (file == null) return BadRequest("Invalid file uploaded.");
            var ext = Path.GetExtension(file.FileName);
            if (!allowedExtensions.Contains(ext))
                return BadRequest($"Invalid file extension. Only {allowedExtensions} allowed. ");
            var oCompany = new Company();
            var path = HttpContext.Current.Server.MapPath($"~/GlobalSuiteFolder/{oCompany.MemberCode.Trim()}{ext}");
            file.SaveAs(path);
            var result = await _tradingService.DailyTradePosting(date, path);
            if (!result.IsSuccess) return BadRequest(result.ToString());

            return Ok();
        }

    }
}