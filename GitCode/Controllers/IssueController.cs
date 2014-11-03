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
    public class IssueController : Controller
    {
        private GitCodeContext db = new GitCodeContext();

        //
        // GET: /Issue/

        public ActionResult Index()
        {
            List<Issue> allIssues = db.Issues.ToList();
            List<Issue> currentIssuesRepo = allIssues.Where(i => i.Repository.User == PagePropertiesModel.user && i.Repository.Project == PagePropertiesModel.project).ToList();
            return View(currentIssuesRepo);
        }

        //
        // GET: /Issue/Details/5

        public ActionResult Details(int id = 0)
        {
            Issue issue = db.Issues.Find(id);
            if (issue == null)
            {
                return HttpNotFound();
            }
            return View(issue);
        }

        //
        // GET: /Issue/Create

        public ActionResult Create()
        {
            ViewBag.RepositoryId = new SelectList(db.Repositories, "RepositoryId", "User");
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username");
            return View();
        }

        //
        // POST: /Issue/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Issue issue)
        {
            if (ModelState.IsValid)
            {
                User user = db.Users.Find(WebSecurity.CurrentUserId);
                issue.User = user;
                issue.UserId = user.UserId;

                List<Repository> allRepo = db.Repositories.ToList();
                issue.Repository = allRepo.Where(r => r.User == PagePropertiesModel.user && r.Project == PagePropertiesModel.project).FirstOrDefault();
                issue.RepositoryId = issue.Repository.RepositoryId;

                issue.When = DateTime.Now;
                issue.IsClear = false;

                db.Issues.Add(issue);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RepositoryId = new SelectList(db.Repositories, "RepositoryId", "User", issue.RepositoryId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username", issue.UserId);
            return View(issue);
        }

        //
        // GET: /Issue/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Issue issue = db.Issues.Find(id);
            if (issue == null)
            {
                return HttpNotFound();
            }
            ViewBag.RepositoryId = new SelectList(db.Repositories, "RepositoryId", "User", issue.RepositoryId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username", issue.UserId);
            return View(issue);
        }

        //
        // POST: /Issue/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Issue issue)
        {
            if (ModelState.IsValid)
            {
                db.Entry(issue).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RepositoryId = new SelectList(db.Repositories, "RepositoryId", "User", issue.RepositoryId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username", issue.UserId);
            return View(issue);
        }

        //
        // GET: /Issue/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Issue issue = db.Issues.Find(id);
            if (issue == null)
            {
                return HttpNotFound();
            }
            return View(issue);
        }

        //
        // POST: /Issue/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Issue issue = db.Issues.Find(id);
            db.Issues.Remove(issue);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult _Comment(Comment comment, string path)
        {
            Dictionary<string,string> urlInfo = urlSplitter(path);
            if (ModelState.IsValid)
            {
                List<Issue> allIssue = db.Issues.ToList();
                Issue issueSelected = allIssue.Where(i => i.Repository.User == PagePropertiesModel.user && i.Repository.Project == PagePropertiesModel.project && i.IssueId == Int32.Parse(urlInfo["issueID"])).FirstOrDefault();

                comment.Issue = issueSelected;
                comment.IssueId = issueSelected.IssueId;
                comment.User = db.Users.Find(WebSecurity.CurrentUserId);
                comment.UserId = comment.User.UserId;
                comment.When = DateTime.Now.ToString();
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = urlInfo["issueID"] });
            }

            ViewBag.IssueId = new SelectList(db.Issues, "IssueId", "Title", comment.IssueId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username", comment.UserId);
            return View(comment);
        }

        private Dictionary<string, string> urlSplitter(string path)
        {
            string[] splittedUrl = path.Split('/');
            Dictionary<string, string> urlInfo = new Dictionary<string, string>();

            if (splittedUrl.Count() > 1)
            {
                urlInfo.Add("user", splittedUrl[0]);
                urlInfo.Add("project", splittedUrl[1]);
                urlInfo.Add("path", splittedUrl[0] + @"\" + splittedUrl[1]);
            }
            if (splittedUrl.Count() > 2)
                urlInfo.Add("issueID", splittedUrl[2]);

            return urlInfo;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}