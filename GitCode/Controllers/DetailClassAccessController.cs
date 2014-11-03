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
    public class DetailClassAccessController : Controller
    {
        private GitCodeContext db = new GitCodeContext();

        //
        // GET: /DetailClassAccess/

        public ActionResult Index()
        {
            var detailclassaccesses = db.DetailClassAccesses.Include(d => d.User).Include(d => d.Class);
            return View(detailclassaccesses.ToList());
        }

        //
        // GET: /DetailClassAccess/Details/5

        public ActionResult Details(int id = 0)
        {
            DetailClassAccess detailclassaccess = db.DetailClassAccesses.Find(id);
            if (detailclassaccess == null)
            {
                return HttpNotFound();
            }
            return View(detailclassaccess);
        }

        //
        // GET: /DetailClassAccess/Create

        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username");
            ViewBag.ClassId = new SelectList(db.Classes, "ClassId", "Name");
            return View();
        }

        //
        // POST: /DetailClassAccess/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DetailClassAccess detailclassaccess)
        {
            var detailClassAccessObject = from dca in db.DetailClassAccesses
                                          select dca;
            if (ModelState.IsValid)
            {
                if (!db.DetailClassAccesses.ToList().Where(dca => dca.UserId == detailclassaccess.UserId && dca.ClassId == detailclassaccess.ClassId).Any())
                {
                    db.DetailClassAccesses.Add(detailclassaccess);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "User already exist in Class.");
                }
            }

            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username", detailclassaccess.UserId);
            ViewBag.ClassId = new SelectList(db.Classes, "ClassId", "Name", detailclassaccess.ClassId);
            return View(detailclassaccess);
        }

        //
        // GET: /DetailClassAccess/Edit/5

        public ActionResult Edit(int id = 0)
        {
            DetailClassAccess detailclassaccess = db.DetailClassAccesses.Find(id);
            if (detailclassaccess == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username", detailclassaccess.UserId);
            ViewBag.ClassId = new SelectList(db.Classes, "ClassId", "Name", detailclassaccess.ClassId);
            return View(detailclassaccess);
        }

        //
        // POST: /DetailClassAccess/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DetailClassAccess detailclassaccess)
        {
            if (ModelState.IsValid)
            {
                db.Entry(detailclassaccess).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username", detailclassaccess.UserId);
            ViewBag.ClassId = new SelectList(db.Classes, "ClassId", "Name", detailclassaccess.ClassId);
            return View(detailclassaccess);
        }

        //
        // GET: /DetailClassAccess/Delete/5

        public ActionResult Delete(string idclass)
        {
            string[] splitter = idclass.Split('/');
            int classid = Int32.Parse(splitter[1]);
            int userid = Int32.Parse(splitter[0]);
            DetailClassAccess detailclassaccess = db.DetailClassAccesses.Where(dc => dc.ClassId == classid && dc.UserId == userid).FirstOrDefault();
            if (detailclassaccess == null)
            {
                return HttpNotFound();
            }
            return View(detailclassaccess);
        }

        //
        // POST: /DetailClassAccess/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string idclass)
        {
            string[] splitter = idclass.Split('/');
            int classid = Int32.Parse(splitter[1]);
            int userid = Int32.Parse(splitter[0]);
            DetailClassAccess detailclassaccess = db.DetailClassAccesses.Where(dc => dc.ClassId == classid && dc.UserId == userid).FirstOrDefault();
            
            db.DetailClassAccesses.Remove(detailclassaccess);
            db.SaveChanges();
            return RedirectToAction("Details", "Class", new { id = classid });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}