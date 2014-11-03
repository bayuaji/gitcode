using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GitCode.Models;
using GitCode.DAL;
using WebMatrix.WebData;
using System.Data;

namespace GitCode.Controllers
{
    public class ClassController : Controller
    {
        private GitCodeContext db = new GitCodeContext();

        //
        // GET: /Class/

        public ActionResult Index()
        {
            //get list team of login user
            var listTeam = from dt in db.DetailTeams
                               where dt.UserId == WebSecurity.CurrentUserId
                               select dt.Team;
            //get list Class of login user in detailClass
            var detailClass = from dc in db.DetailClasses
                           from dt in listTeam
                           where dt.TeamId == dc.TeamId
                           select dc.Class;
            //get list Class of login user in detailClassAccess
            var detailClassAccess = from dca in db.DetailClassAccesses
                                    where dca.UserId == WebSecurity.CurrentUserId
                                    select dca.Class;
            int isi = detailClass.Count();
            //combine class in detailClass & detailClassAccess
            if (detailClassAccess.ToList().Count > 0)
            {
                detailClassAccess.ToList().AddRange(detailClass.ToList());
                return View(detailClassAccess.Distinct().ToList());
            }
            else
            {
                detailClass.ToList().AddRange(detailClassAccess.ToList());
                return View(detailClass.Distinct().ToList());
            }
        }

        //
        // GET: /Class/Details/5
        // get class detail with every user and every team in that class

        public ActionResult Details(int id = 0)
        {
            DetailAccessDetailClassModel dadcObject = new DetailAccessDetailClassModel();
            List<Team> detailTeamAccess = db.DetailClasses.Where(dc => dc.ClassId == id).Select(dc => dc.Team).Distinct().ToList();
            List<User> detailUserAccess = db.DetailClassAccesses.Where(dca => dca.ClassId == id).Select(dca => dca.User).Distinct().ToList();
            //all user from class access and team
            if (detailUserAccess != null)
            {
                ViewBag.AllAccessedTeam = detailTeamAccess;
                ViewBag.AllAccessedUser = detailUserAccess;
            }
            ViewBag.className = db.Classes.Find(id).Name;
            ViewBag.classId = db.Classes.Find(id).ClassId;

            if (db.DetailClassAccesses.Where(dc => dc.UserId == WebSecurity.CurrentUserId && dc.UserAccess == "admin" && dc.ClassId == id).Any())
                ViewBag.isLecturer = "true";
            else
                ViewBag.isLecturer = "false";

            if (!db.Classes.Where(dc =>dc.ClassId == id).Any())
            {
                return HttpNotFound();
            }
            return View(dadcObject);
        }

        public ActionResult AddUser(string username, int ClassId = 0)
        {
            User user = db.Users.Where(u => u.Username == username).FirstOrDefault();
            DetailClassAccess detailClassAccess = new DetailClassAccess
            {
                UserAccess = "member",
                UserId = user.UserId,
                ClassId = ClassId,
                User = user,
                Class = db.Classes.Find(ClassId)
            };

            db.DetailClassAccesses.Add(detailClassAccess);
            db.SaveChanges();

            return RedirectToAction("Details", "Class", new { id = ClassId });
        }

        //
        // GET: /Class/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Class/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Class classes)
        {
            var classObject = from c in db.Classes
                              select c;

            if (ModelState.IsValid)
            {
                if(!classObject.ToList().Where(c=>c.Name==classes.Name).Any())
                {
                    db.Classes.Add(classes);
                    db.SaveChanges();

                    //Add automatic user in detailclass
                    DetailClassAccess dca = new DetailClassAccess();
                    dca.User = db.Users.Find(WebSecurity.CurrentUserId);
                    dca.UserAccess = "admin";
                    dca.Class = classes;
                    dca.ClassId = classes.ClassId;
                    dca.UserId = db.Users.Find(WebSecurity.CurrentUserId).UserId;
                    db.DetailClassAccesses.Add(dca);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Class name already exist.");
                }
                
            }

            return View(classes);
        }

        //
        // GET: /Class/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Class classes = db.Classes.Find(id);
            if (classes == null)
            {
                return HttpNotFound();
            }
            return View(classes);
        }

        //
        // POST: /Class/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Class classes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(classes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(classes);
        }

        //
        // GET: /Class/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Class classes = db.Classes.Find(id);
            if (classes == null)
            {
                return HttpNotFound();
            }
            return View(classes);
        }

        //
        // POST: /Class/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Class classes = db.Classes.Find(id);
            db.Classes.Remove(classes);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}