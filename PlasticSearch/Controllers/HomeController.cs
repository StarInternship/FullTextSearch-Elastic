using PlasticSearch.Models;
using PlasticSearch.Models.search;
using PlasticSearch.Models.tokenizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
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
            return SearchController.Instance.IsReady();
        }
    }
}