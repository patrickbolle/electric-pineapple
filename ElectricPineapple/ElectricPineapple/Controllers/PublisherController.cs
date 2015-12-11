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
    public class PublisherController : Controller
    {
        private CVGSEntities db = new CVGSEntities();

        // GET: Publisher
        public ActionResult Index()
        {
            return View(db.Publishers.ToList());
        }

        // GET: Publisher/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Publisher publisher = db.Publishers.Find(id);
            if (publisher == null)
            {
                return HttpNotFound();
            }

            var games = db.Games.Include(g => g.ESRBRating1).Include(g => g.Publisher1).Include(g => g.Platform1).Include(g => g.Genre1).Where(n => n.Publisher1.publisherID == publisher.publisherID);
            ViewData["publisherGames"] = games.ToList();

            return View(publisher);
        }

        // GET: Publisher/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Publisher/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "publisherID,publisher1")] Publisher publisher, HttpPostedFileBase publisherImage)
        {
            if (ModelState.IsValid)
            {
                if (publisherImage != null)
                {
                    var fileName = Path.GetFileName(publisherImage.FileName);
                    if (fileName.Length > 30)
                    {
                        fileName = fileName.Substring(0, 25) + fileName.Substring(fileName.Length - 5, 5);
                    }
                    var path = Path.Combine(Server.MapPath("~/Content/images/publishers"), fileName);
                    publisherImage.SaveAs(path);
                    publisher.imagePath = fileName;
                }

                db.Publishers.Add(publisher);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(publisher);
        }

        // GET: Publisher/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Publisher publisher = db.Publishers.Find(id);
            if (publisher == null)
            {
                return HttpNotFound();
            }
            return View(publisher);
        }

        // POST: Publisher/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "publisherID,publisher1")] Publisher publisher, HttpPostedFileBase publisherImage)
        {
            if (ModelState.IsValid)
            {
                var currentPublisher = db.Publishers.Where(p => p.publisherID == publisher.publisherID).First();
                if (currentPublisher == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    currentPublisher.publisher1 = publisher.publisher1;
                }

                if(publisherImage != null)
                {
                    var fileName = Path.GetFileName(publisherImage.FileName);
                    if (fileName.Length > 30)
                    {
                        fileName = fileName.Substring(0, 25) + fileName.Substring(fileName.Length - 5, 5);
                    }
                    var path = Path.Combine(Server.MapPath("~/Content/images/publishers"), fileName);
                    publisherImage.SaveAs(path);
                    currentPublisher.imagePath = fileName;
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(publisher);
        }

        // GET: Publisher/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Publisher publisher = db.Publishers.Find(id);
            if (publisher == null)
            {
                return HttpNotFound();
            }
            return View(publisher);
        }

        // POST: Publisher/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Publisher publisher = db.Publishers.Find(id);
            db.Publishers.Remove(publisher);
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
