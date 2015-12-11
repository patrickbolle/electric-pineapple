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
using System.Security.Claims;

namespace ElectricPineapple.Controllers
{
    public class GameController : Controller
    {
        private CVGSEntities db = new CVGSEntities();

        // GET: Game
        public ActionResult Index()
        {
            var games = db.Games.Include(g => g.ESRBRating1).Include(g => g.Publisher1).Include(g => g.Platform1).Include(g => g.Genre1);
            return View(games.ToList());
        }

        // GET: Game/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game game = db.Games.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }

            var ratings = db.Ratings.Where(r => r.gameID == id);
            if (ratings.Count() > 0)
            {
                var gameRating = 0;

                foreach (Rating item in ratings)
                {
                    gameRating += item.rating1;
                }

                gameRating = gameRating / ratings.Count();

                ViewData["GameRating"] = "Game rating: " + gameRating + "/10"; 
            }
            else
            {
                ViewData["GameRating"] = "Game has no ratings";
            }


            return View(game);
        }

        [HttpPost]
        public ActionResult Details()
        {
            int? id = int.Parse(Request["gameID"]);
            int rating = int.Parse(Request["gameRating"]);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game game = db.Games.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }

            //Gets user ID
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            var userIdValue = "";

            if (userIdClaim != null)
            {
                userIdValue = userIdClaim.Value;
            }

            CVGSUser user = db.CVGSUsers.Where(u => u.userLink == userIdValue).First();

            Rating userRating = null;

            //If the user already has a rating for the game, update that rating
            try
            {
                userRating = db.Ratings.Where(a => a.gameID == id && a.userID == user.userID).First();
            }
            catch
            {            
            }

            if (userRating == null)
            {
                //If the user has not yet rated that game, add a new rating to database
                userRating = new Rating();
                user.Ratings.Add(userRating);
                game.Ratings.Add(userRating);
            }            

            //Set rating
            userRating.rating1 = rating;
            
            db.SaveChanges();

            TempData["message"] = "Rating added successfully!";

            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        public ActionResult AddToWishList()
        {
            int id = int.Parse(Request["gameID"]);
            Game game = db.Games.Find(id);

            //Gets user ID
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            var userIdValue = "";

            if (userIdClaim != null)
            {
                userIdValue = userIdClaim.Value;
            }

            CVGSUser user = db.CVGSUsers.Where(u => u.userLink == userIdValue).First();
            Game_User gameUser = null;

            try
            {
                gameUser = db.Game_User.Where(a => a.gameID == id && a.userID == user.userID).First();
            }
            catch
            {
            }

            if (gameUser == null)
            {
                gameUser = new Game_User();
                gameUser.status = 2;
                user.Game_User.Add(gameUser);
                game.Game_User.Add(gameUser);
                db.SaveChanges();
                TempData["message"] = ("Added to wishlist.");
            }
            else
            {
                TempData["message"] = ("Game already in wishlist.");
            }  
            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        public ActionResult AddToCart()
        {
            return null;
            //todo
        }

        // GET: Game/Create
        public ActionResult Create()
        {
            ViewBag.ESRBRating = new SelectList(db.ESRBRatings, "ratingID", "rating");
            ViewBag.publisher = new SelectList(db.Publishers, "publisherID", "publisher1");
            ViewBag.platform = new SelectList(db.Platforms, "platformID", "platform1");
            ViewBag.genre = new SelectList(db.Genres, "genreID", "genre1");
            return View();
        }

        // POST: Game/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "gameID,title,genre,publisher,ESRBRating,releaseDate,price,description,platform")] Game game, HttpPostedFileBase gameCover, HttpPostedFileBase gameScreenshot)
        {
            if (ModelState.IsValid)
            {

                //TO FIX: will crash if file name is too long
                if (gameCover != null)
                {
                    var fileName = Path.GetFileName(gameCover.FileName);
                    if(fileName.Length > 60)
                    {
                        fileName = fileName.Substring(0, 55) + fileName.Substring(fileName.Length - 5, 5);
                    }

                    var path = Path.Combine(Server.MapPath("~/Content/images/box covers"), fileName);
                    gameCover.SaveAs(path);
                    game.coverPath = fileName;
                }


                if (gameScreenshot != null)
                {
                    var fileName = Path.GetFileName(gameScreenshot.FileName);
                    if (fileName.Length > 60)
                    {
                        fileName = fileName.Substring(0, 55) + fileName.Substring(fileName.Length - 5, 5);
                    }
                    var path = Path.Combine(Server.MapPath("~/Content/images/screenshots"), fileName);
                    gameScreenshot.SaveAs(path);
                    game.screenshotPath = fileName;
                }


                db.Games.Add(game);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ESRBRating = new SelectList(db.ESRBRatings, "ratingID", "rating", game.ESRBRating);
            ViewBag.publisher = new SelectList(db.Publishers, "publisherID", "publisher1", game.publisher);
            ViewBag.platform = new SelectList(db.Platforms, "platformID", "platform1", game.platform);
            ViewBag.genre = new SelectList(db.Genres, "genreID", "genre1", game.genre);
            return View(game);
        }

        // GET: Game/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game game = db.Games.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }
            ViewBag.ESRBRating = new SelectList(db.ESRBRatings, "ratingID", "rating", game.ESRBRating);
            ViewBag.publisher = new SelectList(db.Publishers, "publisherID", "publisher1", game.publisher);
            ViewBag.platform = new SelectList(db.Platforms, "platformID", "platform1", game.platform);
            ViewBag.genre = new SelectList(db.Genres, "genreID", "genre1", game.genre);
            return View(game);
        }

        // POST: Game/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "gameID,title,genre,publisher,ESRBRating,releaseDate,price,description,platform")] Game game, HttpPostedFileBase gameCover, HttpPostedFileBase gameScreenshot)
        {
            if (ModelState.IsValid)
            {
                var currentGame = db.Games.Where(g => g.gameID == game.gameID).First();
                if (currentGame == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    currentGame.title = game.title;
                    currentGame.genre = game.genre;
                    currentGame.publisher = game.publisher;
                    currentGame.ESRBRating = game.ESRBRating;
                    currentGame.releaseDate = game.releaseDate;
                    currentGame.price = game.price;
                    currentGame.description = game.description;
                    currentGame.platform = game.platform;
                }


                //TO FIX: will crash if file name is too long
                if (gameCover != null)
                {
                    var fileName = Path.GetFileName(gameCover.FileName);
                    if (fileName.Length > 60)
                    {
                        fileName = fileName.Substring(0, 55) + fileName.Substring(fileName.Length - 5, 5);
                    }
                    var path = Path.Combine(Server.MapPath("~/Content/images/box covers"), fileName);
                    gameCover.SaveAs(path);
                    currentGame.coverPath = fileName;
                }


                if (gameScreenshot != null)
                {
                    var fileName = Path.GetFileName(gameScreenshot.FileName);
                    if (fileName.Length > 60)
                    {
                        fileName = fileName.Substring(0, 55) + fileName.Substring(fileName.Length - 5, 5);
                    }
                    var path = Path.Combine(Server.MapPath("~/Content/images/screenshots"), fileName);
                    gameScreenshot.SaveAs(path);
                    currentGame.screenshotPath = fileName;
                }
                
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ESRBRating = new SelectList(db.ESRBRatings, "ratingID", "rating", game.ESRBRating);
            ViewBag.publisher = new SelectList(db.Publishers, "publisherID", "publisher1", game.publisher);
            ViewBag.platform = new SelectList(db.Platforms, "platformID", "platform1", game.platform);
            ViewBag.genre = new SelectList(db.Genres, "genreID", "genre1", game.genre);
            return View(game);
        }

        // GET: Game/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game game = db.Games.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }
            return View(game);
        }

        // POST: Game/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Game game = db.Games.Find(id);
            db.Games.Remove(game);
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

        [HttpPost]
        public ActionResult Search(string SearchText)
        {
            ViewData["SearchTerm"] = SearchText;
            var games = db.Games.Include(g => g.ESRBRating1).Include(g => g.Publisher1).Include(g => g.Platform1).Include(g => g.Genre1).Where(n => n.title.Contains(SearchText) || n.description.Contains(SearchText) || n.Publisher1.publisher1.Contains(SearchText));
            return View(games.ToList());
        }
    }
}
