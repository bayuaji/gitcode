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
    public class WikiController : Controller
    {
        private GitCodeContext db = new GitCodeContext();
        private Dictionary<string, string> urlInfo = new Dictionary<string, string>();
        //
        // GET: /Wiki/

        public ActionResult Index()
        {
            return View(db.Wikis.ToList());
        }

        //
        // GET: /Wiki/Details/5

        public ActionResult Details(string path)
        {
            urlInfo = urlSplitter(path);
            int id = 0;
            try
            {
                List<Wiki> wikis = db.Wikis.ToList();
                id = wikis.Where(w => w.Repository.User == urlInfo["user"] && w.Repository.Project == urlInfo["project"]).Select(w => w.WikiId).FirstOrDefault();
            }
            catch(Exception e)
            {
            }
            
            Wiki wiki = db.Wikis.Find(id);

            if(wiki != null)
            ViewData["WikiDetail"] = wiki.WikiDetails.ToList();

            return View(wiki);
        }

        //
        // GET: /Wiki/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Wiki/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Wiki wiki)
        {
            if (ModelState.IsValid)
            {
                Repository repository = db.Repositories.Where(r => r.User == PagePropertiesModel.user && r.Project == PagePropertiesModel.project).FirstOrDefault();
                
                wiki.Repository = repository;
                wiki.Title = repository.Project;
                db.Wikis.Add(wiki);
                db.SaveChanges();
                return RedirectToAction("Tree", "Repository", new { path = GitCode.Models.PagePropertiesModel.projectName });
            }

            return View(wiki);
        }

        //
        // GET: /Wiki/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Wiki wiki = db.Wikis.Find(id);
            if (wiki == null)
            {
                return HttpNotFound();
            } 
            
            if (wiki != null)
                ViewData["WikiDetail"] = wiki.WikiDetails.ToList();
            ViewBag.edit = "true";
            return View(wiki);
        }

        //
        // POST: /Wiki/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Wiki wiki)
        {
            if (ModelState.IsValid)
            {
                db.Entry(wiki).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Tree", "Repository", new { path = GitCode.Models.PagePropertiesModel.projectName });
            }
            return View(wiki);
        }

        //
        // GET: /Wiki/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Wiki wiki = db.Wikis.Find(id);
            if (wiki == null)
            {
                return HttpNotFound();
            }
            return View(wiki);
        }

        //
        // POST: /Wiki/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Wiki wiki = db.Wikis.Find(id);
            db.Wikis.Remove(wiki);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /*public ActionResult AllRepoWikiDetail()
        {
            List<Wiki> allWiki = db.Wikis.ToList();
            
            return View(repoWikiDetail);
        }*/

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        #region urlSplitter

        private Dictionary<string,string> urlSplitter(string path)
        {
            Dictionary<string, string> urlInfo = new Dictionary<string, string>();
            string[] splitter = path.Split('/');
            
            urlInfo["user"] = splitter[0];
            urlInfo["project"] = splitter[1];

            return urlInfo;
        }

        #endregion
    }
}