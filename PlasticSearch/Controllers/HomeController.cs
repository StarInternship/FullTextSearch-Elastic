using System.Web.Mvc;

namespace PlasticSearch.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Search(string query)
        {
            return Json(SearchController.Instance.Search(query));
        }

        [HttpPost]
        public long IsReady()
        {
            return SearchController.Instance.GetIsReady();
        }
    }
}