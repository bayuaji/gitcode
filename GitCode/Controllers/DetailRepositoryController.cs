using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GitCode.Models;
using GitCode.DAL;
using WebMatrix.WebData;

namespace GitCode.Controllers
{
    public class DetailRepositoryController : Controller
    {
        private GitCodeContext db = new GitCodeContext();

        //
        // GET: /DetailRepository/

        public DetailRepositoryController()
        {

        }
        public ActionResult Index()
        {
            var detailrepositories = db.DetailRepositories.Include(d => d.User).Include(d => d.Repository);
            return View(detailrepositories.ToList());
        }

        //
        // GET: /DetailRepository/Details/5

        public ActionResult Details(int id = 0)
        {
            DetailRepository detailrepository = db.DetailRepositories.Find(id);
            if (detailrepository == null)
            {
                return HttpNotFound();
            }
            return View(detailrepository);
        }

        //
        // GET: /DetailRepository/Create

        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username");
            ViewBag.RepositoryId = new SelectList(db.Repositories, "RepositoryId", "IsPublic");
            return View();
        }

        //
        // POST: /DetailRepository/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DetailRepository detailrepository)
        {
            if (ModelState.IsValid)
            {
                db.DetailRepositories.Add(detailrepository);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username", detailrepository.UserId);
            ViewBag.RepositoryId = new SelectList(db.Repositories, "RepositoryId", "IsPublic", detailrepository.RepositoryId);
            return View(detailrepository);
        }

        //
        // GET: /DetailRepository/Edit/5

        public ActionResult Edit(int id = 0)
        {
            DetailRepository detailrepository = db.DetailRepositories.Find(id);
            if (detailrepository == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username", detailrepository.UserId);
            ViewBag.RepositoryId = new SelectList(db.Repositories, "RepositoryId", "IsPublic", detailrepository.RepositoryId);
            return View(detailrepository);
        }

        //
        // POST: /DetailRepository/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DetailRepository detailrepository)
        {
            if (ModelState.IsValid)
            {
                db.Entry(detailrepository).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username", detailrepository.UserId);
            ViewBag.RepositoryId = new SelectList(db.Repositories, "RepositoryId", "IsPublic", detailrepository.RepositoryId);
            return View(detailrepository);
        }

        //
        // GET: /DetailRepository/Delete/5

        public ActionResult Delete(int id = 0)
        {
            DetailRepository detailrepository = db.DetailRepositories.Find(id);
            if (detailrepository == null)
            {
                return HttpNotFound();
            }
            return View(detailrepository);
        }

        //
        // POST: /DetailRepository/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DetailRepository detailrepository = db.DetailRepositories.Find(id);
            db.DetailRepositories.Remove(detailrepository);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public bool addNewDetailRepoData(DetailRepository repoDetail)
        {
            bool message = false;
            var detailRepoAll = from dr in db.DetailRepositories
                                select dr;

            if (!detailRepoAll.Where(ua => (ua.UserId == repoDetail.UserId) && ua.RepositoryId == repoDetail.RepositoryId).Any())
            {
                db.DetailRepositories.Add(repoDetail);
                db.SaveChanges();
                message = true;
            }
            return message;
        }
    }
}