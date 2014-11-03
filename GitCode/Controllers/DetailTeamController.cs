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
    public class DetailTeamController : Controller
    {
        private GitCodeContext db = new GitCodeContext();

        //
        // GET: /DetailTeam/

        public ActionResult Index()
        {
            var detailteams = db.DetailTeams.Include(d => d.User).Include(d => d.Team);
            return View(detailteams.ToList());
        }

        //
        // GET: /DetailTeam/Details/5
        /*[HttpPost]
        [ValidateAntiForgeryToken]*/
        public ActionResult Details(int id = 0, int idclass = 0)
        {
            var detailTeam = from dt in db.DetailTeams
                             where dt.TeamId == id
                             select dt;

            if (idclass == 0)
            {
                int idtemp = db.DetailClasses.Where(dc=>dc.TeamId == id).Select(dc=>dc.ClassId).FirstOrDefault();
                idclass = idtemp != 0 ? idtemp : 0;
            }
            else
            {
                ViewBag.UserInClass = db.DetailClassAccesses.Where(dca => dca.ClassId == idclass).Select(dca => dca.User.Username).ToList();
                ViewBag.UserId = new SelectList(db.DetailClassAccesses.Where(dca => dca.ClassId == idclass).Select(dca => dca.User), "UserId", "Username");
            }
            //ViewBag.UserInClass = db.DetailClassAccesses.Where(dca => dca.ClassId == idclass).Select(dca => dca.User);
            ViewBag.ClassId = idclass;
            ViewBag.TeamName = db.Teams.Find(id).Name;
            ViewBag.teamId = db.Teams.Find(id).TeamId;
            if (detailTeam == null)
            {
                return HttpNotFound();
            }
            return View(detailTeam.ToList());
        }

        public JsonResult AutoCompleteClassUser(string term, int idclass)
        {
            List<string> result = new List<string>();
            if (idclass == 0)
            {
                result = db.Users.Select(u => u.Username).ToList();
            }
            else
            {
                result = db.DetailClassAccesses.Where(dca => dca.ClassId == idclass).Select(dca => dca.User.Username).ToList();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /DetailTeam/Create
        /*
        public ActionResult Create(string teamName)
        {
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username");
            ViewBag.TeamName = teamName;
            return View();
        }
        */
        //
        // POST: /DetailTeam/Create
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]*/
        public ActionResult Create(DetailTeam detailteam, string teamName)
        {   
            if (ModelState.IsValid)
            {
                detailteam.Team = db.Teams.Where(t => t.Name == teamName).FirstOrDefault();
                detailteam.TeamId = detailteam.Team.TeamId;

                db.DetailTeams.Add(detailteam);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = detailteam.TeamId });
            }

            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username", detailteam.UserId);
            ViewBag.TeamId = new SelectList(db.Teams, "TeamId", "Name", detailteam.TeamId);
            return View(detailteam);
        }

        [HttpPost]
        public ActionResult AddUser(string username, string teamname)
        {
            Team team = db.Teams.Where(t=>t.Name == teamname).FirstOrDefault();
            User user = db.Users.Where(t=>t.Username == username).FirstOrDefault();
            DetailTeam detailteam = new DetailTeam{
                IsOwner = false,
            Team = team,
            TeamId = team.TeamId,
            User = user,
            UserId = user.UserId,
            };
                db.DetailTeams.Add(detailteam);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = team.TeamId, idclass = 0 });
        }

        [HttpPost]
        public ActionResult EditRole(string role, int idDetail)
        {
            DetailTeam detailteam = db.DetailTeams.Find(idDetail);
            if (detailteam == null)
            {
                return HttpNotFound();
            }
            else
            {
                detailteam.Role = role;
                db.Entry(detailteam).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = detailteam.TeamId, idclass = 0 });
            }
        }

        //
        // GET: /DetailTeam/Edit/5
        [ChildActionOnly]
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
        // POST: /DetailTeam/Edit/5

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
        // GET: /DetailTeam/Delete/5

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
        // POST: /DetailTeam/Delete/5

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