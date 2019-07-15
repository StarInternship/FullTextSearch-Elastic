using System;
using System.Collections.Generic;
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
        public string Search(string query)
        {
            return "not found";
        }

        [HttpPost]
        public int Preprocess()
        {
            Thread.Sleep(3000);
            return 3000;
        }
    }
}