using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ElectricPineapple;

namespace ElectricPineapple.Controllers
{
    public class CVGSUsersController : Controller
    {
        private CVGSEntities db = new CVGSEntities();

        // GET: CVGSUsers
        public ActionResult Index()
        {
            var cVGSUsers = db.CVGSUsers.Include(c => c.Province1).Include(c => c.UserType1);
            return View(cVGSUsers.ToList());
        }

        // GET: CVGSUsers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CVGSUser cVGSUser = db.CVGSUsers.Find(id);
            if (cVGSUser == null)
            {
                return HttpNotFound();
            }
            return View(cVGSUser);
        }

        // GET: CVGSUsers/Create
        public ActionResult Create()
        {
            ViewBag.province = new SelectList(db.Provinces, "provinceCode", "province1");
            ViewBag.userType = new SelectList(db.UserTypes, "typeID", "typeID");
            return View();
        }

        // POST: CVGSUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "userID,firstName,lastName,userName,email,address,city,province,phone,password,gender,recievePromotions,profileInfo,userType,userLink")] CVGSUser cVGSUser)
        {
            if (ModelState.IsValid)
            {
                db.CVGSUsers.Add(cVGSUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.province = new SelectList(db.Provinces, "provinceCode", "province1", cVGSUser.province);
            ViewBag.userType = new SelectList(db.UserTypes, "typeID", "typeID", cVGSUser.userType);
            return View(cVGSUser);
        }

        // GET: CVGSUsers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CVGSUser cVGSUser = db.CVGSUsers.Find(id);
            if (cVGSUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.province = new SelectList(db.Provinces, "provinceCode", "province1", cVGSUser.province);
            ViewBag.userType = new SelectList(db.UserTypes, "typeID", "typeID", cVGSUser.userType);
            return View(cVGSUser);
        }

        // POST: CVGSUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "userID,firstName,lastName,userName,email,address,city,province,phone,password,gender,recievePromotions,profileInfo,userType,userLink")] CVGSUser cVGSUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cVGSUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.province = new SelectList(db.Provinces, "provinceCode", "province1", cVGSUser.province);
            ViewBag.userType = new SelectList(db.UserTypes, "typeID", "typeID", cVGSUser.userType);
            return View(cVGSUser);
        }

        // GET: CVGSUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CVGSUser cVGSUser = db.CVGSUsers.Find(id);
            if (cVGSUser == null)
            {
                return HttpNotFound();
            }
            return View(cVGSUser);
        }

        // POST: CVGSUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CVGSUser cVGSUser = db.CVGSUsers.Find(id);
            db.CVGSUsers.Remove(cVGSUser);
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



        [Authorize(Roles = "Admin")]
        public ActionResult AdminEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CVGSUser cVGSUser = db.CVGSUsers.Find(id);
            if (cVGSUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.province = new SelectList(db.Provinces, "provinceCode", "province1", cVGSUser.province);
            ViewBag.userType = new SelectList(db.UserTypes, "typeID", "typeID", cVGSUser.userType);
            return View(cVGSUser);
        }

        // POST: CVGSUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdminEdit([Bind(Include = "userID,firstName,lastName,userName,email,address,city,province,phone,password,gender,recievePromotions,profileInfo,userType,userLink")] CVGSUser cVGSUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cVGSUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.province = new SelectList(db.Provinces, "provinceCode", "province1", cVGSUser.province);
            ViewBag.userType = new SelectList(db.UserTypes, "typeID", "typeID", cVGSUser.userType);
            return View(cVGSUser);
        }
    }
}
