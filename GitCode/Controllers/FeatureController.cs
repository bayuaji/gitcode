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
    public class FeatureController : Controller
    {
        private GitCodeContext db = new GitCodeContext();

        //
        // GET: /Feature/

        public ActionResult Index(string path)
        {
            var features = db.Features.Where(f=>f.Repository.User == PagePropertiesModel.user && f.Repository.Project == PagePropertiesModel.project);
            return View(features.ToList());
        }

        //
        // GET: /Feature/Details/5

        public ActionResult Details(int id = 0)
        {
            Feature feature = db.Features.Find(id);
            if (feature == null)
            {
                return HttpNotFound();
            }
            return View(feature);
        }

        //
        // GET: /Feature/Create

        public ActionResult Create()
        {
            ViewBag.RepositoryRepositoryId = new SelectList(db.Repositories, "RepositoryId", "User");
            return View();
        }

        //
        // POST: /Feature/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Feature feature)
        {
            if (ModelState.IsValid)
            {
                List<Repository> allRepo = db.Repositories.ToList();

                feature.Repository = allRepo.Where(r => r.User == PagePropertiesModel.user && r.Project == PagePropertiesModel.project).FirstOrDefault();
                feature.RepositoryRepositoryId = feature.Repository.RepositoryId;
                
                db.Features.Add(feature);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RepositoryRepositoryId = new SelectList(db.Repositories, "RepositoryId", "User", feature.RepositoryRepositoryId);
            return View(feature);
        }

        //
        // GET: /Feature/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Feature feature = db.Features.Find(id);
            if (feature == null)
            {
                return HttpNotFound();
            }
            ViewBag.RepositoryRepositoryId = new SelectList(db.Repositories, "RepositoryId", "User", feature.RepositoryRepositoryId);
            return View(feature);
        }

        //
        // POST: /Feature/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Feature feature)
        {
            if (ModelState.IsValid)
            {
                db.Entry(feature).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RepositoryRepositoryId = new SelectList(db.Repositories, "RepositoryId", "User", feature.RepositoryRepositoryId);
            return View(feature);
        }

        //
        // GET: /Feature/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Feature feature = db.Features.Find(id);
            if (feature == null)
            {
                return HttpNotFound();
            }
            return View(feature);
        }

        //
        // POST: /Feature/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Feature feature = db.Features.Find(id);
            db.Features.Remove(feature);
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