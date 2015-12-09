using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElectricPineapple.Controllers
{
    public class HomeController : Controller
    {
        CVGSEntities db = new CVGSEntities();

        public ActionResult Index()
        {
            var events = db.Events;
            ViewData["event"] = events.ToList();
            return View();
        }

        public ActionResult GameCatalogue()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult NewPageTemplate()
        {
            return View();
        }
        public ActionResult Game()
        {
            return View();
        }
    }
}