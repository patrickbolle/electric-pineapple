﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ElectricPineapple;
using System.Security.Claims;
using Microsoft.AspNet.Identity;

namespace ElectricPineapple.Controllers
{
    public class ShippingAddressesController : Controller
    {
        private CVGSEntities db = new CVGSEntities();

        // GET: ShippingAddresses
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            CVGSUser user = db.CVGSUsers.Where(u => u.userLink == userId).First();

            //Show shipping addresses that belong to the user
            List<ShippingAddress> userShippingAddresses = new List<ShippingAddress>();
            foreach (ShippingAddress item in db.ShippingAddresses)
            {
                if (item.CVGSUsers.Contains(user))
                {
                    userShippingAddresses.Add(item);
                }
            }
            return View(userShippingAddresses);
        }

        // GET: ShippingAddresses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShippingAddress shippingAddress = db.ShippingAddresses.Find(id);
            if (shippingAddress == null)
            {
                return HttpNotFound();
            }
            return View(shippingAddress);
        }

        // GET: ShippingAddresses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ShippingAddresses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "addressID,address,City,Province,Country,Postal_Code")] ShippingAddress shippingAddress)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                CVGSUser user = db.CVGSUsers.Where(u => u.userLink == userId).First();

                //Add shipping address to the user
                user.ShippingAddresses.Add(shippingAddress);
                db.ShippingAddresses.Add(shippingAddress);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(shippingAddress);
        }

        // GET: ShippingAddresses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShippingAddress shippingAddress = db.ShippingAddresses.Find(id);
            if (shippingAddress == null)
            {
                return HttpNotFound();
            }
            return View(shippingAddress);
        }

        // POST: ShippingAddresses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "addressID,address,City,Province,Country,Postal_Code")] ShippingAddress shippingAddress)
        {
            if (ModelState.IsValid)
            {
                db.Entry(shippingAddress).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(shippingAddress);
        }

        // GET: ShippingAddresses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShippingAddress shippingAddress = db.ShippingAddresses.Find(id);
            if (shippingAddress == null)
            {
                return HttpNotFound();
            }
            return View(shippingAddress);
        }

        // POST: ShippingAddresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ShippingAddress shippingAddress = db.ShippingAddresses.Find(id);
            db.ShippingAddresses.Remove(shippingAddress);
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
