using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GitCode.Models;
using GitCode.DAL;

namespace GitCode.Controllers
{
    public class cobController : Controller
    {
        private GitCodeContext db = new GitCodeContext();

        //
        // GET: /cob/

        public ActionResult Index()
        {
            var detailteams = db.DetailTeams.Include(d => d.User).Include(d => d.Team);
            return View(detailteams.ToList());
        }

        //
        // GET: /cob/Details/5

        public ActionResult Details(int id = 0)
        {
            DetailTeam detailteam = db.DetailTeams.Find(id);
            if (detailteam == null)
            {
                return HttpNotFound();
            }
            return View(detailteam);
        }

        //
        // GET: /cob/Create

        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username");
            ViewBag.TeamId = new SelectList(db.Teams, "TeamId", "Name");
            return View();
        }

        //
        // POST: /cob/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DetailTeam detailteam)
        {
            if (ModelState.IsValid)
            {
                db.DetailTeams.Add(detailteam);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username", detailteam.UserId);
            ViewBag.TeamId = new SelectList(db.Teams, "TeamId", "Name", detailteam.TeamId);
            return View(detailteam);
        }

        //
        // GET: /cob/Edit/5

        public ActionResult Edit(int id = 0)
        {
            DetailTeam detailteam = db.DetailTeams.Find(id);
            if (detailteam == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username", detailteam.UserId);
            ViewBag.TeamId = new SelectList(db.Teams, "TeamId", "Name", detailteam.TeamId);
            return View(detailteam);
        }

        //
        // POST: /cob/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DetailTeam detailteam)
        {
            if (ModelState.IsValid)
            {
                db.Entry(detailteam).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username", detailteam.UserId);
            ViewBag.TeamId = new SelectList(db.Teams, "TeamId", "Name", detailteam.TeamId);
            return View(detailteam);
        }

        //
        // GET: /cob/Delete/5

        public ActionResult Delete(int id = 0)
        {
            DetailTeam detailteam = db.DetailTeams.Find(id);
            if (detailteam == null)
            {
                return HttpNotFound();
            }
            return View(detailteam);
        }

        //
        // POST: /cob/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DetailTeam detailteam = db.DetailTeams.Find(id);
            db.DetailTeams.Remove(detailteam);
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