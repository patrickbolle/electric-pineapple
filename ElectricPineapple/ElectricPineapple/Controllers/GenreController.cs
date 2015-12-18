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
    public class GenreController : Controller
    {
        private CVGSEntities db = new CVGSEntities();

        // GET: Genre
        public ActionResult Index()
        {
            return View(db.Genres.ToList());
        }

        // GET: Genre/Details/5

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Genre genre = db.Genres.Find(id);
            if (genre == null)
            {
                return HttpNotFound();
            }

            //var games = db.Games.Where(a => a.genre == id).ToList();
            //ViewBag.stuff = games;

            var games = db.Games.Include(g => g.ESRBRating1).Include(g => g.Publisher1).Include(g => g.Platform1).Include(g => g.Genre1).Where(a => a.genre == id);
            ViewData["gameGenre"] = games.ToList();

            return View(genre);
        }

        // GET: Genre/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Genre/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "genreID,genre1")] Genre genre, HttpPostedFileBase genreImage)
        {
            if (ModelState.IsValid)
            {
                if (genreImage != null)
                {
                    var fileName = Path.GetFileName(genreImage.FileName);
                    if (fileName.Length > 30)
                    {
                        fileName = fileName.Substring(0, 25) + fileName.Substring(fileName.Length - 5, 5);
                    }
                    var path = Path.Combine(Server.MapPath("~/Content/images/genres"), fileName);
                    genreImage.SaveAs(path);
                    genre.imagePath = fileName;
                }

                db.Genres.Add(genre);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(genre);
        }

        // GET: Genre/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Genre genre = db.Genres.Find(id);
            if (genre == null)
            {
                return HttpNotFound();
            }
            return View(genre);
        }

        // POST: Genre/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "genreID,genre1")] Genre genre, HttpPostedFileBase genreImage)
        {
            if (ModelState.IsValid)
            {
                var currentGenre = db.Genres.Where(g => g.genreID == genre.genreID).First();
                if (currentGenre == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    currentGenre.genre1 = genre.genre1;
                }

                if (genreImage != null)
                {
                    var fileName = Path.GetFileName(genreImage.FileName);
                    if (fileName.Length > 30)
                    {
                        fileName = fileName.Substring(0, 25) + fileName.Substring(fileName.Length - 5, 5);
                    }
                    var path = Path.Combine(Server.MapPath("~/Content/images/genres"), fileName);
                    genreImage.SaveAs(path);
                    currentGenre.imagePath = fileName;
                }

                db.SaveChanges();
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(genre);
        }

        // GET: Genre/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Genre genre = db.Genres.Find(id);
            if (genre == null)
            {
                return HttpNotFound();
            }
            return View(genre);
        }

        // POST: Genre/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Genre genre = db.Genres.Find(id);
            db.Genres.Remove(genre);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
