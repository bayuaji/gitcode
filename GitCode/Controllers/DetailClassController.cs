using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GitCode.Models;
using GitCode.DAL;
using System.Data;

namespace GitCode.Controllers
{
    public class DetailClassController : Controller
    {
        private GitCodeContext db = new GitCodeContext();

        //
        // GET: /DetailClass/

        public ActionResult Index()
        {
            var detailclasses = db.DetailClasses.Include(d => d.Team).Include(d => d.Class);
            return View(detailclasses.ToList());
        }

        //
        // GET: /DetailClass/Details/5

        public ActionResult Details(int id = 0)
        {
            DetailClass detailclass = db.DetailClasses.Find(id);
            if (detailclass == null)
            {
                return HttpNotFound();
            }
            return View(detailclass);
        }

        //
        // GET: /DetailClass/Create

        /*public ActionResult Create()
        {
            
            ViewBag.TeamId = new SelectList(db.Teams, "TeamId", "Name");
            ViewBag.ClassId = new SelectList(db.Classes, "ClassId", "Name");
                        
            return View();
        }*/

        //
        // POST: /DetailClass/Create
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]*/
        //add admin to every team
        public void Create(DetailClass detailclass)
        {
            //admin of class
            var listClassAdmin = from ca in db.DetailClassAccesses
                                 where ca.ClassId == detailclass.ClassId && ca.UserAccess=="admin"
                                 select ca;
            User classAdmin = listClassAdmin.ToList()[0].User;
            //list user in team
            var userTeam = from dt in db.DetailTeams
                           where dt.TeamId == detailclass.TeamId
                           select dt.User;

            //all detail class
            var detailClass = from dc in db.DetailClasses
                              select dc;
            if (ModelState.IsValid)
            {
                //check if exist
                if(!detailClass.ToList().Where(t=>t.TeamId==detailclass.TeamId && t.ClassId==detailclass.ClassId).Any())
                {
                    //check if admin already in team
                    if(!userTeam.Where(ca => ca.UserId == classAdmin.UserId).Any())
                    {
                        //add admin to team in detailclass
                        DetailTeam detailTeam = new DetailTeam();
                        detailTeam.Team = detailclass.Team;
                        detailTeam.TeamId = detailclass.TeamId;
                        detailTeam.User = classAdmin;
                        detailTeam.UserId = classAdmin.UserId;
                        detailTeam.Role = "Lecturer";

                        db.DetailTeams.Add(detailTeam);
                        db.SaveChanges();
                    }

                    db.DetailClasses.Add(detailclass);
                    db.SaveChanges();
                    //return RedirectToAction("Index","Class");
                }
                else
                {
                    ModelState.AddModelError("", "Team already exist in class.");
                }
            }

            ViewBag.TeamId = new SelectList(db.Teams, "TeamId", "Name", detailclass.TeamId);
            ViewBag.ClassId = new SelectList(db.Classes, "ClassId", "Name", detailclass.ClassId);
            //return View(detailclass);
        }

        public ActionResult CreateTeamInClass()
        {
            return View();
        }
        //
        // GET: /DetailClass/Edit/5

        public ActionResult Edit(int id = 0)
        {
            DetailClass detailclass = db.DetailClasses.Find(id);
            if (detailclass == null)
            {
                return HttpNotFound();
            }
            ViewBag.TeamId = new SelectList(db.Teams, "TeamId", "Name", detailclass.TeamId);
            ViewBag.ClassId = new SelectList(db.Classes, "ClassId", "Name", detailclass.ClassId);
            return View(detailclass);
        }

        //
        // POST: /DetailClass/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DetailClass detailclass)
        {
            if (ModelState.IsValid)
            {
                db.Entry(detailclass).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TeamId = new SelectList(db.Teams, "TeamId", "Name", detailclass.TeamId);
            ViewBag.ClassId = new SelectList(db.Classes, "ClassId", "Name", detailclass.ClassId);
            return View(detailclass);
        }

        //
        // GET: /DetailClass/Delete/5

        public ActionResult Delete(int id = 0)
        {
            DetailClass detailclass = db.DetailClasses.Find(id);
            if (detailclass == null)
            {
                return HttpNotFound();
            }
            return View(detailclass);
        }

        //
        // POST: /DetailClass/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DetailClass detailclass = db.DetailClasses.Find(id);
            db.DetailClasses.Remove(detailclass);
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