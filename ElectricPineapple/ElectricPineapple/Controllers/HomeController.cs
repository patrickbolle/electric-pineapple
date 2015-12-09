using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ElectricPineapple;
using System.IO;

namespace ElectricPineapple.Controllers
{

   

    public class HomeController : Controller
    {
        private CVGSEntities db = new CVGSEntities();

        public ActionResult Index()
        {
            var news = db.News.Take(10);
            ViewData["NewsList"] = news.ToList();

            var events = db.Events.Take(10);
            ViewData["EventList"] = events.ToList();


            var games = db.Games.Where(n => n.releaseDate > DateTime.Now).Take(10).OrderBy(g => g.releaseDate);
            return View(games.ToList());
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