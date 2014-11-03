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
using GitCode.Git;
using GitCode.Data;
using System.IO;

namespace GitCode.Controllers
{
    public class TeamController : Controller
    {
        private GitCodeContext db = new GitCodeContext();
        private TransferContent transferContent = new TransferContent();
        private RepositoryController repositoryService = new RepositoryController();
        //
        // GET: /Team/

        //public ActionResult Index()
        //{

        //    return View(db.Teams.ToList());
        //}
        public ActionResult Index()
        {
            var teamInDT = from dt in db.DetailTeams
                           where dt.UserId == WebSecurity.CurrentUserId
                           select dt.Team;

            return View(teamInDT.ToList());
        }
        //
        // GET: /Team/Details/5

        public ActionResult Details(int id = 0)
        {
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        //
        // GET: /Team/Create

        public ActionResult Create(string input)
        {
            return View();
        }

        //
        // POST: /Team/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Team team, string classID = "")
        {
            var teamObject = from t in db.Teams
                              select t;

            if (ModelState.IsValid)
            {
                RepositoryController repoService = new RepositoryController();
                if (!teamObject.ToList().Where(t => t.Name == team.Name).Any() && repoService.createUserDirectory(team.Name) && !db.Users.Where(u=> u.Username == team.Name).Any())
                {
                    db.Teams.Add(team);
                    db.SaveChanges();
                    User user = db.Users.Find(WebSecurity.CurrentUserId);
                    FirstCreate(team, user);

                    if (classID != "")
                    {
                        DetailClass detailClass = new DetailClass();
                        DetailClassController classService = new DetailClassController();

                        detailClass.ClassId = Int32.Parse(classID);
                        detailClass.TeamId = team.TeamId;

                        classService.Create(detailClass);
                    }

                    return RedirectToAction("Details", "DetailTeam", new { id = team.TeamId , idclass = classID });
                }
                else
                {
                    ModelState.AddModelError("", "Team name already exist.");
                }
            }

            return View(team);
        }

        //Add team maker
        public void FirstCreate(Team team, User user)
        {
            DetailTeam detailTeam = new DetailTeam();
            detailTeam.TeamId = team.TeamId;
            detailTeam.UserId = user.UserId;
            detailTeam.IsOwner = true;
            db.DetailTeams.Add(detailTeam);
            db.SaveChanges();
        }

        //
        // POST: /Team/Edit/5
        public ActionResult Edit(int id = 0)
        {
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Team team)
        {
            if (team != null)
            {
                db.Entry(team).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(team);
        }

        //
        // GET: /Team/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        //
        // POST: /Team/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Team team = db.Teams.Find(id);
            List<Repository> repodelete = new List<Repository>();
            repodelete = db.Repositories.Where(r => r.User == team.Name).ToList();
            foreach (Repository repo in repodelete)
            {
                db.Repositories.Remove(repo);
            }
            db.Teams.Remove(team);
            db.SaveChanges();
            Directory.Delete(transferContent.getRepoDirectory() + @"\" + team.Name,true);
            
            return RedirectToAction("Index");
        }

        public ActionResult TeamPerformance(int teamID = 0,int repoID = 0)
        {
            List<Repository> teamRepo = db.Repositories.Where(r => r.TeamId == teamID).ToList();
            if (teamRepo.Any())
            {
                Repository choosedRepository = new Repository();

                if (repoID != 0)
                    choosedRepository = teamRepo.Find(t => t.RepositoryId == repoID);
                else
                    choosedRepository = teamRepo.FirstOrDefault();

                var contributor = repositoryService.getContributor(choosedRepository.User + @"/" + choosedRepository.Project, "master");
                var max = contributor.Select(c => c.commitContrib).OrderByDescending(c => c).FirstOrDefault();
                ViewBag.MaxCommit = max != null ? max : 0 ;
                max = contributor.Select(c => c.featureContrib).OrderByDescending(c => c).FirstOrDefault();
                ViewBag.MaxFeature = max != null ? max : 0;
                ViewBag.CurrentTeam = teamID;
                ViewBag.RecentCommit = recentCommits(choosedRepository);
                ViewBag.Contributor = contributor;
            }
            return View(teamRepo);
        }

        public List<CommitModel> recentCommits(Repository repo)
        {
            List<CommitModel> commitActivity = new List<CommitModel>();
            GitService gitService = new GitService();

            try
            {
                commitActivity = gitService.getCommitsInfo(transferContent.getRepoDirectory() + @"\" + repo.User + @"\" + repo.Project, "master").ToList();
            }
            catch(Exception e)
            {
                commitActivity = null;
            }
            return commitActivity;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}