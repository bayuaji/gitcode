using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LibGit2Sharp;
using GitCode.Models;
using GitCode.DAL;
using GitCode.Data;
using System.IO;
using WebMatrix.WebData;
using GitCode.Git;
using System.Net;

namespace GitCode.Controllers
{
    public class RepositoryController : Controller
    {
        private GitCodeContext db = new GitCodeContext();
        private TransferContent transferContent = new TransferContent();
        private DetailRepositoryController detailRepository = new DetailRepositoryController();
        private GitService gitService = new GitService();
        private Dictionary<string, string> urlInfo = new Dictionary<string, string>();
        //
        // GET: /Repository/
        public RepositoryController()
        { }
        #region ViewController
        public ActionResult Index()
        {
            PagePropertiesModel.projectName = null;
            if (WebSecurity.CurrentUserName != "")
            {
                List<Team> teamRepo = new List<Team>();
                List<Models.Repository> contribRepo = new List<Models.Repository>();

                contribRepo = db.DetailRepositories.Where(dr => dr.UserId == WebSecurity.CurrentUserId).Select(dr => dr.Repository).ToList();
                
                teamRepo = db.DetailTeams.Where(dt => dt.UserId == WebSecurity.CurrentUserId).Select(dt => dt.Team).ToList();
                foreach(Team team in teamRepo)
                {
                    contribRepo.AddRange(team.Repositories.ToList());
                }
                
                //contribRepo.AddRange(privateRepo);
                //db.Repositories.Where(r => r.User == WebSecurity.CurrentUserName).ToList().AddRange(privateRepo);
                if(db.Users.Where(u=>u.UserId == WebSecurity.CurrentUserId && u.Role == "administrator" ).Any())
                    ViewBag.isAdmin = "true";

                ViewData["PrivateRepository"] = contribRepo.Select(r=>r).Distinct().ToList();
                //ViewData["PrivateRepository"] = db.Repositories.Where(r => r.User == WebSecurity.CurrentUserName).ToList();
            }
            ViewData["PublicRepository"] = db.Repositories.Where(r=>r.IsPublic==true).ToList();
            return View();
        }

        public ActionResult MyRepository()
        {
            List<Team> TeamRepo = new List<Team>();
            List<Models.Repository> myTeamRepo = new List<Models.Repository>();
            List<Models.Repository> contribTeamRepo = new List<Models.Repository>();

            List<Models.Repository> myRepo = new List<Models.Repository>();
            List<Models.Repository> contribRepo = new List<Models.Repository>();

            myRepo = db.DetailRepositories.Where(dr => dr.UserId == WebSecurity.CurrentUserId && dr.IsOwner == true).Select(dr => dr.Repository).ToList();
            contribRepo = db.DetailRepositories.Where(dr => dr.UserId == WebSecurity.CurrentUserId && dr.IsOwner == false).Select(dr => dr.Repository).ToList();

            TeamRepo = db.DetailTeams.Where(dt => dt.UserId == WebSecurity.CurrentUserId && dt.IsOwner == true).Select(dt => dt.Team).ToList();
            foreach (Team team in TeamRepo)
            {
                myTeamRepo.AddRange(team.Repositories.ToList());
            }
            if (db.Users.Where(u => u.UserId == WebSecurity.CurrentUserId && u.Role == "administrator").Any())
                ViewBag.isAdmin = "true";
            TeamRepo = db.DetailTeams.Where(dt => dt.UserId == WebSecurity.CurrentUserId && dt.IsOwner == false).Select(dt => dt.Team).ToList();
            foreach (Team team in TeamRepo)
            {
                contribTeamRepo.AddRange(team.Repositories.ToList());
            }

            myRepo.AddRange(myTeamRepo);
            contribRepo.AddRange(contribTeamRepo);

            ViewBag.OwnRepo = myRepo.Distinct().ToList();
            ViewBag.ContribRepo = contribRepo.Distinct().ToList();

            return View();
        }

        public ActionResult Setting(string path)
        {
            urlInfo = urlSplitter(path);
            string user = urlInfo["user"];
            string project = urlInfo["project"];
            var repo = db.Repositories.Where(r => r.User == user && r.Project == project ).ToList();
            Models.Repository repositoryChoosed = repo.FirstOrDefault();
            if (db.Users.Where(u => u.UserId == WebSecurity.CurrentUserId && u.Role == "administrator").Any())
                ViewBag.isAdmin = "true";
            List<User> userExistInCollaborator = repositoryChoosed.DetailRepositories.Select(dr => dr.User).ToList();
            Team Team = repositoryChoosed.Team;

            //userExistInCollaborator.AddRange(userInTeam);
            ViewBag.TeamId = new SelectList(db.DetailTeams.Where(dt => dt.UserId == WebSecurity.CurrentUserId).Select(dt => dt.Team).Distinct(), "TeamId", "Name");
            ViewBag.Collaborator = userExistInCollaborator.Distinct().ToList();
            ViewBag.Team = Team;

            return View(repositoryChoosed);
        }

        [HttpPost]
        public ActionResult addContributor(string username, int RepositoryId = 0, string User = "", string Project = "")
        {
            DetailRepository detailRepository = new DetailRepository
            {
                IsOwner = false,
                Repository = db.Repositories.Find(RepositoryId),
                RepositoryId = RepositoryId,
                User = db.Users.Where(u => u.Username == username).FirstOrDefault(),
                UserId = db.Users.Where(u => u.Username == username).Select(u => u.UserId).FirstOrDefault()
            };
            db.DetailRepositories.Add(detailRepository);
            db.SaveChanges();
            return RedirectToAction("Setting", new { path = User +"/"+ Project });
        }

        [HttpPost]
        public ActionResult deleteContributor(int RepositoryId = 0, int UserId = 0)
        {
            DetailRepository detailrepo = db.DetailRepositories.Where(dr => dr.RepositoryId == RepositoryId && dr.UserId == UserId).FirstOrDefault();
            return RedirectToAction("Delete", "DetailRepository", new { id = detailrepo.DetailRepositoryId });
        }
        //
        // GET: /Repository/Details/5
        public ActionResult Details(int id = 0)
        {
            GitCode.Models.Repository repository = db.Repositories.Find(id);
            if (repository == null)
            {
                return HttpNotFound();
            }
            return View(repository);
        }

        //
        // GET: /Repository/Create

        public ActionResult Create()
        {
            ViewBag.TeamId = new SelectList(db.DetailTeams.Where(dt => dt.UserId == WebSecurity.CurrentUserId).Select(dt => dt.Team).Distinct(), "TeamId", "Name");
            return View();
        }

        //
        // POST: /Repository/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GitCode.Models.Repository repository)
        {
            if (db.Users.Where(u => u.UserId == WebSecurity.CurrentUserId && u.Role == "administrator").Any())
                ViewBag.isAdmin = "true";
            if (repository.TeamId == null)
                repository.User = WebSecurity.CurrentUserName;
            else
                repository.User = db.Teams.Find(repository.TeamId).Name;

            if (ModelState.IsValid)
            {
                bool successAddRepo = addNewRepoData(repository); //add to repo db
                if (successAddRepo)
                {
                    string path = createRepoDirectory(repository.User, repository.Project); //create repo folder
                    bool successAddDetailRepo;
                    if (repository.TeamId == null)
                    {
                        var detailRepo = new DetailRepository
                        {
                            User = db.Users.Find(WebSecurity.CurrentUserId),
                            Repository = getRepo(repository.User, repository.Project),
                            UserId = WebSecurity.CurrentUserId,
                            RepositoryId = getRepo(repository.User, repository.Project).RepositoryId,
                            IsOwner = true,
                        };

                        successAddDetailRepo = addDetailRepo(detailRepo); //add detailRepo owner
                    }
                    else
                    {
                        User teamOwner = db.Teams.Find(repository.TeamId).DetailTeams.Where(dt => dt.IsOwner == true).Select(t => t.User).FirstOrDefault();
                        var detailRepo = new DetailRepository
                        {
                            User = db.Users.Find(teamOwner.UserId),
                            Repository = getRepo(repository.User, repository.Project),
                            UserId = teamOwner.UserId,
                            RepositoryId = getRepo(repository.User, repository.Project).RepositoryId,
                            IsOwner = true,
                        };

                        successAddDetailRepo = addDetailRepo(detailRepo);
                    }

                    if (successAddDetailRepo)
                    {
                        bool gitInit = gitService.CreateRepository(path);
                        if (gitInit)
                            return RedirectToAction("Index");
                    }
                }
            }

            ViewBag.TeamId = new SelectList(db.Teams, "TeamId", "Name", repository.TeamId);
            return View(repository);
        }

        //
        // GET: /Repository/Edit/5

        public ActionResult Edit(int id = 0)
        {
            GitCode.Models.Repository repository = db.Repositories.Find(id);
            if (repository == null)
            {
                return HttpNotFound();
            }
            ViewBag.TeamId = new SelectList(db.Teams, "TeamId", "Name", repository.TeamId);
            return View(repository);
        }

        //
        // POST: /Repository/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GitCode.Models.Repository repository)
        {
            if (ModelState.IsValid)
            {
                db.Entry(repository).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TeamId = new SelectList(db.Teams, "TeamId", "Name", repository.TeamId);
            return View(repository);
        }

        public ActionResult _Branch(string path)
        {
            
            return View();
        }

        public ActionResult Branch(string path)
        {
            if (db.Users.Where(u => u.UserId == WebSecurity.CurrentUserId && u.Role == "administrator").Any())
                ViewBag.isAdmin = "true";
            urlInfo = urlSplitter(path);

            List<BranchModel> branchList = gitService.getBranch(transferContent.getRepoDirectory() + @"\" + urlInfo["path"]);

            //ViewData["Project"] = urlInfo["path"];
            return View(branchList);
        }

        public ActionResult Commit(string path)
        {
            if (db.Users.Where(u => u.UserId == WebSecurity.CurrentUserId && u.Role == "administrator").Any())
                ViewBag.isAdmin = "true";
            urlInfo = urlSplitter(path);
            IEnumerable<CommitModel> commitInfo = new List<CommitModel>();
            //get all commit
            if(!urlInfo.ContainsKey("sha"))
                commitInfo = gitService.getCommitsInfo(transferContent.getRepoDirectory() + @"\" + urlInfo["path"], urlInfo["branch"]);
            else
                commitInfo = gitService.getAllCommitByChoosen(urlInfo["branch"], transferContent.getRepoDirectory() + @"\" + urlInfo["path"], urlInfo["sha"]);

            //get all branch
            List<BranchModel> branchList = gitService.getBranch(transferContent.getRepoDirectory() + @"\" + urlInfo["path"]);

            List<CommitModel> commitInBranch = new List<CommitModel>();
            //get commit in every branch
            foreach(var branch in branchList)
            {
                commitInBranch.Add(gitService.getCommitBy(branch.BranchName, transferContent.getRepoDirectory()+@"\"+urlInfo["path"]));
            }

            ViewData["CommitListBy"] = commitInBranch;
            ViewData["Branch"] = branchList.Select(b => b.BranchName);
            ViewData["CurrentBranch"] = urlInfo["branch"];

            return View(commitInfo);
        }
        public ActionResult CommitDetail(string path)
        {
            if (db.Users.Where(u => u.UserId == WebSecurity.CurrentUserId && u.Role == "administrator").Any())
                ViewBag.isAdmin = "true";
            urlInfo = urlSplitter(path);
            CommitModel commitInfo = gitService.getCommitsInfo(transferContent.getRepoDirectory() + @"\" + urlInfo["path"], urlInfo["branch"], "detail").FirstOrDefault(ci => ci.SHA == urlInfo["sha"]);

            return View(commitInfo);
        }
        //
        // GET: /Repository/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (db.Users.Where(u => u.UserId == WebSecurity.CurrentUserId && u.Role == "administrator").Any())
                ViewBag.isAdmin = "true";
            GitCode.Models.Repository repository = db.Repositories.Find(id);
            if (repository == null)
            {
                return HttpNotFound();
            }
            return View(repository);
        }

        //
        // POST: /Repository/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GitCode.Models.Repository repository = db.Repositories.Find(id);
            db.Repositories.Remove(repository);
            db.SaveChanges();
            Directory.Delete(transferContent.getRepoDirectory()+@"\"+ repository.User + @"\" + repository.Project, true);
            return RedirectToAction("MyRepository");
        }

        public ActionResult Feature(string path)
        {
            urlInfo = urlSplitter(path);
            IEnumerable<CommitModel> commitInfoByFeature;
            IEnumerable<CommitModel> commitInfo;
            //get all commit
            commitInfo = gitService.getCommitsInfo(transferContent.getRepoDirectory() + @"\" + urlInfo["path"], urlInfo["branch"]);

            if (commitInfo != null)
                commitInfoByFeature = commitInfo.Where(ci => ci.Feature == urlInfo["feature"]);
            else
                commitInfoByFeature = null;

            ViewData["CurrentBranch"] = urlInfo["branch"];
            ViewBag.FeatureName = getFeatureBy(urlInfo["user"],urlInfo["project"], Int32.Parse(urlInfo["feature"]));
            return View(commitInfoByFeature);
        }

        public ActionResult Tree(string path)
        {
            urlInfo = urlSplitter(path);

            List<BranchModel> branchList = new List<BranchModel>();
            PagePropertiesModel.projectName = urlInfo["user"]+@"/"+urlInfo["project"];
            PagePropertiesModel.user = urlInfo["user"];
            PagePropertiesModel.project = urlInfo["project"];
            List<CommitModel> allCommit = new List<CommitModel>();
            List<TagModel> tagList = new List<TagModel>();

            //get all commit
            //commitInfo = gitService.getCommitsInfo(transferContent.getRepoDirectory() + @"\" + urlInfo["path"]).FirstOrDefault();
            //allCommit = getAllInformation(path).ToList();
            if (urlInfo.ContainsKey("sha"))
            {
                allCommit = gitService.getAllCommitByChoosen(urlInfo["branch"], transferContent.getRepoDirectory() + @"\" + urlInfo["path"], urlInfo["sha"]);
                ViewData["CommitElement"] = allCommit.Count();
                ViewData["ContributorElement"] = allCommit.Select(ac => ac.Author.Name).Distinct().Count();
                ViewData["SHA"] = urlInfo["sha"];
            }
            else
            {
                try
                {
                    allCommit = gitService.getCommitsInfo(transferContent.getRepoDirectory() + @"\" + urlInfo["path"], urlInfo["branch"]).ToList();
                    ViewData["CommitElement"] = allCommit.Count();
                    ViewData["ContributorElement"] = allCommit.Select(ac => ac.Author.Name).Distinct().Count();
                }
                catch (Exception e)
                {
                    ViewData["CommitElement"] = 0;
                    ViewData["ContributorElement"] = 0;
                    return View();
                }
            }
            //get all branch
                branchList = gitService.getBranch(transferContent.getRepoDirectory() + @"\" + urlInfo["path"]);
            if(branchList !=null)    
            ViewData["Branch"] = branchList.Select(b => b.BranchName);
                //get all tag
                tagList = gitService.getAllTag(transferContent.getRepoDirectory() + @"\" + urlInfo["path"]);
            if(tagList !=null)
                ViewData["Tag"] = tagList.Select(t => t.name);

            ViewData["GitPath"] = urlInfo["path"].Replace('\\', '/');
            ViewData["CurrentBranch"] = urlInfo["branch"];
            ViewData["BranchElement"] = branchList.Count();
            //ViewData["Path"] = urlInfo["path"];

            return View(allCommit.FirstOrDefault());
        }

        public ActionResult Tag(string path)
        {
            urlInfo = urlSplitter(path);

            List<TagModel> allTag = gitService.getAllTag(transferContent.getRepoDirectory() + @"\" + urlInfo["path"]);

            ViewData["GitPath"] = urlInfo["path"].Replace('\\', '/');
            return View(allTag);
        }

        public ActionResult Archive(string path)
        {
            urlInfo = urlSplitter(path);
            using (var git = new GitService(transferContent.getRepoDirectory() + @"\" + urlInfo["path"]))
            {
                string cacheFile;
                if(urlInfo.ContainsKey("sha"))
                    cacheFile = git.getCommitFile(urlInfo["branch"], transferContent.getRepoDirectory() + @"\" + urlInfo["path"], urlInfo["sha"]);
                else
                cacheFile = git.getCommitFile(urlInfo["branch"],transferContent.getRepoDirectory() + @"\" +urlInfo["path"] );

                if (cacheFile == null)
                    throw new HttpException((int)HttpStatusCode.NotFound, string.Empty);

                return File(@transferContent.getCacheDirectory()+@"\"+cacheFile, "application/zip", cacheFile + ".zip");
            }
        }
        public ActionResult Contributor(string path)
        {
            urlInfo = urlSplitter(path);
            List<ContributorModel> allContributor = getContributor(urlInfo["user"]+"/"+urlInfo["project"], urlInfo["branch"]);

            return View(allContributor);
        }

        #endregion

        #region RepositoryService
        public List<ContributorModel> getContributor(string path, string branch)
        {
            List<ContributorModel> allContributor = new List<ContributorModel>();
            List<string> contributorEmail = new List<string>();
            List<CommitModel> allCommit = new List<CommitModel>();
            List<CommitModel> allCommitByAuthor = new List<CommitModel>();

            allCommit = getAllInformation(path+@"/"+branch);
            contributorEmail = allCommit.Select(c => c.Author.Email).Distinct().ToList();

            foreach (string contributor in contributorEmail)
            {
                ContributorModel newContributor = new ContributorModel
                {
                    contributorName = allCommit.Where(ac => ac.Author.Email == contributor).Select(ac => ac.Author.Name).Distinct().FirstOrDefault(),
                    contributorEmail = contributor,
                    commitContrib = allCommit.Where(ac => ac.Author.Email == contributor).ToList().Count(),
                    featureContrib = allCommit.Where(ac => ac.Author.Email == contributor && ac.Feature != "").Select(ac => ac.Feature).Distinct().Count()
                };
                allContributor.Add(newContributor);
            }

            return allContributor;
        }

        private List<CommitModel> getAllInformation(string path)
        {
            urlInfo = urlSplitter(path);
            CommitModel commitInfo = null;
            List<CommitModel> allCommit = new List<CommitModel>();
            //get all commit
            //commitInfo = gitService.getCommitsInfo(transferContent.getRepoDirectory() + @"\" + urlInfo["path"]).FirstOrDefault();
            if (!urlInfo.ContainsKey("sha"))
            {
                commitInfo = gitService.getCommitBy(urlInfo["branch"], transferContent.getRepoDirectory() + @"\" + urlInfo["path"]);
                if(commitInfo !=null)
                allCommit = gitService.getAllCommitByChoosen(urlInfo["branch"], transferContent.getRepoDirectory() + @"\" + urlInfo["path"], commitInfo.SHA);
            }
            else
            {
                allCommit = gitService.getAllCommitByChoosen(urlInfo["branch"], transferContent.getRepoDirectory() + @"\" + urlInfo["path"], urlInfo["sha"]);
                ViewData["SHA"] = urlInfo["sha"];
            }

            return allCommit;
        }
        private string createRepo(string directoryName, string user)
        {
            string message = "";
            string repoOwner = @transferContent.getRepoDirectory() + @"\" + @WebSecurity.CurrentUserName;
            string fullPath = "";

            if (!Directory.Exists(@repoOwner))
            {
                try
                {
                    Directory.CreateDirectory(@repoOwner);
                }
                catch
                {
                    Directory.Delete(@repoOwner);
                }
            }

            fullPath = repoOwner + @"\" + directoryName;

            if (!Directory.Exists(@fullPath))
            {
                Directory.CreateDirectory(@fullPath);
                message = fullPath;
            }

            return message;
        }

        public bool createUserDirectory(string user)
        {
            bool value = false;
            string userDirectory = @transferContent.getRepoDirectory() + @"\" + user;

            if (!Directory.Exists(@userDirectory))
            {
                try
                {
                    Directory.CreateDirectory(@userDirectory);
                    value = true;
                }
                catch
                {
                    Directory.Delete(@userDirectory);
                    value = false;
                }
            }
            else
                if(!db.Users.Where(u=>u.Username == user).Any() && !db.Teams.Where(t => t.Name == user).Any())
            {
                value = true;
            }

            return value;
        }

        public string createRepoDirectory(string user, string repository)
        {
            string value = "";
            string repoDirectory = @transferContent.getRepoDirectory() + @"\" + user+@"\"+repository;

            if (!Directory.Exists(@repoDirectory))
            {
                try
                {
                    Directory.CreateDirectory(@repoDirectory);
                    value = repoDirectory;
                }
                catch
                {
                    Directory.Delete(@repoDirectory);
                    value = "";
                }
            }

            return value;
        }

        private Dictionary<string,string> urlSplitter(string path)
        {
            string[] splittedUrl = path.Split('/');
            Dictionary<string, string> urlInfo = new Dictionary<string, string>();

            if (splittedUrl.Count() > 1)
            {
                urlInfo.Add("user", splittedUrl[0]);
                urlInfo.Add("project", splittedUrl[1]);
                urlInfo.Add("path", splittedUrl[0] + @"\" + splittedUrl[1]);
            }
            if(splittedUrl.Count() > 2)
                urlInfo.Add("branch", splittedUrl[2]);
            if (splittedUrl.Count() > 3)
            {
                urlInfo.Add("sha", splittedUrl[3]);
                urlInfo.Add("feature", splittedUrl[3]);
            }
            if(splittedUrl.Count() > 4)
                urlInfo["feature"] = splittedUrl[4];

            return urlInfo;
        }
        #endregion

        #region RepoData
        public IEnumerable<GitCode.Models.Repository> myOwnRepo()
        {
            var detailRepository = db.DetailRepositories.Where(re => re.UserId == WebSecurity.CurrentUserId).ToList();
            var ownRepo = from dr in detailRepository
                       select dr.Repository;
            return ownRepo;
        }
        public bool addNewRepoData(GitCode.Models.Repository repoDetail)
        {
            bool message = false;
            var repoAll = from r in db.Repositories
                          select r;

            if (!repoAll.Where(ua => (ua.User == repoDetail.User) && ua.Project == repoDetail.Project).Any())
            {
                if(repoDetail.User == null)
                repoDetail.User = db.Users.Find(WebSecurity.CurrentUserId).Username;

                db.Repositories.Add(repoDetail);
                db.SaveChanges();
                message = true;
            }
            return message;
        }
        public GitCode.Models.Repository getRepo(string user, string project)
        {
            GitCode.Models.Repository repo = db.Repositories.Where(r => (r.User == user) && r.Project == project).ToList().First();

            return repo;
        }

        public bool publicCanRead(string user, string project)
        {
            bool value = false;

            value = db.Repositories.Where(r => (r.User == user) && r.Project == project).ToList().First().IsPublicRead;

            return value;
        }

        public bool publicCanWrite(string user, string project)
        {
            bool value = false;

            value = db.Repositories.Where(r => (r.User == user) && r.Project == project).ToList().First().IsPublicWrite;

            return value;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteCountry(string term)
        {
            var result3 = db.Repositories.Where(r => r.IsPublic == true).Select(r=>r.User+"/"+r.Project);
            return Json(result3, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AutoCompleteAllUser(string term)
        {
            var result = db.Users.Select(u=>u.Username);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public bool userCanWrite(string user, string project)
        {
            bool value = false;

            List<User> userExistInCollaborator = db.DetailRepositories.Where(dr => dr.Repository.Project == project && dr.User.Username == user).Select(dr=>dr.User).ToList();
            List<Team> userTeam = db.DetailTeams.Where(dt => dt.User.Username == user).Select(dt => dt.Team).ToList();
            List<Models.Repository> repoTeam = new List<Models.Repository>();
            foreach(Team team in userTeam)
            {
                List<Models.Repository> repo = team.Repositories.ToList();
                repoTeam.AddRange(repo);
            }

            if (repoTeam.Where(rt => rt.Project == project).Any() || userExistInCollaborator.Any())
                return true;

            return value;
        }
        #endregion

        #region DetailRepoData

        public bool addDetailRepo(DetailRepository detailRepo)
        {
            bool message = false;
            var detailRepoAll = from r in db.DetailRepositories
                          select r;

            if (!detailRepoAll.Where(dra => (dra.UserId == detailRepo.UserId) && dra.RepositoryId == detailRepo.RepositoryId).Any())
            {
                db.DetailRepositories.Add(detailRepo);
                db.SaveChanges();
                message = true;
            }
            return message;
        }

        #endregion

        #region FeatureRepo

        public string getFeatureBy(string user, string project, int fitureNumber)
        {
            string featureName;

            featureName = db.Repositories.Where(r => r.User == user && r.Project == project).First().Features.ToList()[fitureNumber - 1].Detail;

            return featureName;
        }

        #endregion
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}