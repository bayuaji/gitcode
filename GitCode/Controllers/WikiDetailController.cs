using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GitCode.Models;
using GitCode.DAL;
using System.IO;

namespace GitCode.Controllers
{
    public class WikiDetailController : Controller
    {
        private GitCodeContext db = new GitCodeContext();

        //
        // GET: /WikiDetail/

        public ActionResult Index()
        {
            var wikidetails = db.WikiDetails.Include(w => w.Wiki);
            return View(wikidetails.ToList());
        }

        //
        // GET: /WikiDetail/Details/5

        public ActionResult Details(int id = 0)
        {
            WikiDetail wikidetail = db.WikiDetails.Find(id);
            if (wikidetail == null)
            {
                return HttpNotFound();
            }
            return View(wikidetail);
        }

        //
        // GET: /WikiDetail/Create

        

        public ActionResult Create()
        {
            ViewBag.WikiId = new SelectList(db.Wikis, "WikiId", "Title");
            return View();
        }

        //
        // POST: /WikiDetail/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WikiDetail wikidetail)
        {
            if (ModelState.IsValid)
            {
                db.WikiDetails.Add(wikidetail);
                db.SaveChanges();
                return RedirectToAction("Tree", "Repository", new { path = GitCode.Models.PagePropertiesModel.projectName });
            }

            ViewBag.WikiId = new SelectList(db.Wikis, "WikiId", "Title", wikidetail.WikiId);
            return View(wikidetail);
        }

        //
        // GET: /WikiDetail/Edit/5

        public ActionResult Edit(int id = 0)
        {
            WikiDetail wikidetail = db.WikiDetails.Find(id);
            if (wikidetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.WikiId = new SelectList(db.Wikis, "WikiId", "Title", wikidetail.WikiId);
            return View(wikidetail);
        }

        //
        // POST: /WikiDetail/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(WikiDetail wikidetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(wikidetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.WikiId = new SelectList(db.Wikis, "WikiId", "Title", wikidetail.WikiId);
            return View(wikidetail);
        }

        //
        // GET: /WikiDetail/Delete/5

        public ActionResult Delete(int id = 0)
        {
            WikiDetail wikidetail = db.WikiDetails.Find(id);
            if (wikidetail == null)
            {
                return HttpNotFound();
            }
            return View(wikidetail);
        }

        //
        // POST: /WikiDetail/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WikiDetail wikidetail = db.WikiDetails.Find(id);
            db.WikiDetails.Remove(wikidetail);
            db.SaveChanges();
            return RedirectToAction("Tree", "Repository", new { path = GitCode.Models.PagePropertiesModel.projectName });
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            WikiDetail wikidetail = new WikiDetail();
            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/uploads"), fileName);
                file.SaveAs(path);

                wikidetail.Wiki = getCurrentWiki();
                wikidetail.WikiId = wikidetail.Wiki.WikiId;
                wikidetail.ImagePath = Path.Combine("~/uploads", fileName);
                db.WikiDetails.Add(wikidetail);
                db.SaveChanges();
            }

            return RedirectToAction("Tree", "Repository", new { path = GitCode.Models.PagePropertiesModel.projectName });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private Wiki getCurrentWiki()
        {
            List<Wiki> wikies = db.Wikis.ToList();

            return wikies.Where(w => w.Repository.User == PagePropertiesModel.user && w.Repository.Project == PagePropertiesModel.project).FirstOrDefault();
        }
    }
}