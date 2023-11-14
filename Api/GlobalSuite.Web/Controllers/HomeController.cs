using System.Web.Mvc;

namespace GlobalSuite.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return RedirectPermanent("/swagger");
        }
    }
}
