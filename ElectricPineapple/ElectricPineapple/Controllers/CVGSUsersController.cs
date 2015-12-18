using System;
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
            var userId = User.Identity.GetUserId();
            CVGSUser user = db.CVGSUsers.Where(u => u.userLink == userId).First();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CVGSUser cVGSUser = db.CVGSUsers.Find(id);
            if (cVGSUser == null)
            {
                return HttpNotFound();
            }

            //Creates a list of games that are on the users wishlist
            List<Game> wishlist = new List<Game>();
            foreach (Game_User item in db.Game_User)
            {
                if (item.CVGSUser == cVGSUser)
                {
                    wishlist.Add(item.Game);
                }
            }

            bool friends = false;
            //If the users are friends, or the user is viewing their own profile, show the wishlist (on view)
            if(db.Friends.Where(b => b.UserID == user.userID && b.FriendID == cVGSUser.userID).Count() > 0 || id == user.userID)
            {
                friends = true;
            }

            ViewData["userWishlist"] = wishlist;
            ViewData["areFriends"] = friends;

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
        [Authorize(Roles="Admin")]
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
        [Authorize(Roles = "Admin")]
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

        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchResults(string SearchText)
        {
            //Search for a user
            ViewData["SearchTerm"] = SearchText;
            var users = db.CVGSUsers.Where(a => a.userName.Contains(SearchText) || a.email.Contains(SearchText));
            return View(users.ToList());
        }

        //[HttpPost]
        public ActionResult AddFriend(int? id)
        {
            CVGSUser friend = db.CVGSUsers.Find(id);

            var userId = User.Identity.GetUserId();
            CVGSUser user = db.CVGSUsers.Where(u => u.userLink == userId).First();

            //Links the two users
            Friend newfriendship = new Friend();
            newfriendship.FriendID = friend.userID;
            newfriendship.UserID = user.userID;
            db.Friends.Add(newfriendship);
            
            db.SaveChanges();

            var AllFriends = db.Friends.Where(a => a.UserID == user.userID);

            List<CVGSUser> friends = new List<CVGSUser>();
            //Gets a list of all the users friends
            foreach (Friend item in AllFriends)
            {
                friends.Add(db.CVGSUsers.Find(item.FriendID));
            }
            
            return View("FriendsList", friends.ToList());
        }

        public ActionResult FriendsList()
        {
            var userId = User.Identity.GetUserId();
            CVGSUser user = db.CVGSUsers.Where(u => u.userLink == userId).First();

            var AllFriends = db.Friends.Where(a => a.UserID == user.userID).Distinct<Friend>();

            List<CVGSUser> friends = new List<CVGSUser>();
            //Creates a list of all the users friends
            foreach (Friend item in AllFriends)
            {
                friends.Add(db.CVGSUsers.Find(item.FriendID));
            }

            return View(friends.ToList());
        }
    }
}
