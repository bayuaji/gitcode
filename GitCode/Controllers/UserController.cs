using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GitCode.Models;
using GitCode.DAL;
using WebMatrix.WebData;
using GitCode.Filters;
using Microsoft.Web.WebPages.OAuth;
using System.Web.Security;
using System.Data;
using System.Net.Mail;
using System.IO;
using GitCode.Data;

namespace GitCode.Controllers
{
    public class UserController : Controller
    {
        private GitCodeContext db = new GitCodeContext();
        private TransferContent transferContent = new TransferContent();
        //
        // GET: /User/

        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        //
        // GET: /User/Details/5

        public ActionResult Details(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // GET: /User/Create

        public bool sendEmail(string sendTo)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("hermangroho@gmail.com");
                mail.To.Add(sendTo);
                mail.Subject = "Test Mail";
                mail.Body = "This is for testing SMTP mail from GMAIL";

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("hermangroho@gmail.com", "HermanHerman");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /User/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            var userAll = from u in db.Users
                       select u;
            if (ModelState.IsValid)
            {
                try
                {
                    if(!userAll.Where(ua=>ua.Email==user.Email).Any())
                    {
                        RepositoryController repoService = new RepositoryController();
                        if (repoService.createUserDirectory(user.Username) )
                        {
                            //if (sendEmail(user.Email)) //send mail
                            if(true)
                            {
                                WebSecurity.Logout();
                                WebSecurity.CreateUserAndAccount(user.Username, user.Password);
                                user.Role = "user";
                                db.Users.Add(user);
                                db.SaveChanges();
                            }
                            else
                            {
                                ModelState.AddModelError("", "E-mail confirmation have a problem. Please register again.");
                                return View(user);
                            }
                        }

                        return RedirectToAction("Index", "Repository");
                    }
                    else
                    {
                        ModelState.AddModelError("", "E-mail address already exists. Please enter a different e-mail address.");
                    }
                    
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            return View(user);
        }

        //
        // GET: /User/Edit/5

        public ActionResult Edit(int id = 0)
        {
            User user = db.Users.Find(WebSecurity.CurrentUserId);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        //
        // GET: /User/Delete/5

        public ActionResult Delete(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /User/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            List<Repository> repouser = db.Repositories.Where(r => r.User == user.Username).ToList();
                if(repouser.Any())
                {
                    foreach (Repository repo in repouser)
                    {
                        List<Issue> issue = db.Issues.Where(i => i.RepositoryId == repo.RepositoryId).ToList();
                        {
                            if (issue.Any())
                            {
                                foreach (Issue issueDel in issue)
                                {
                                    List<Comment> comment = db.Comments.Where(c => c.IssueId == issueDel.IssueId).ToList();
                                    if (comment.Any())
                                    {
                                        foreach (Comment comdel in comment)
                                        {
                                            db.Comments.Remove(comdel);

                                        }
                                    }
                                    db.Issues.Remove(issueDel);
                                }
                            }
                        }
                        db.Repositories.Remove(repo); 
                    }
                }
            Directory.Delete(transferContent.getRepoDirectory() + @"\" + user.Username, true);
            ((SimpleMembershipProvider)Membership.Provider).DeleteAccount(user.Username); // deletes record from webpages_Membership table
            ((SimpleMembershipProvider)Membership.Provider).DeleteUser(user.Username, true);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        // POST: /User/Login
        public ActionResult LoginUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginUser(User model, string returnUrl)
        {
            if (WebSecurity.Login(model.Username, model.Password))
            {
                if(db.Users.Where(u => u.Username == model.Username && u.Role == "administrator").Any())
                    GitCode.Models.PagePropertiesModel.isAdmin = true;
                else
                    GitCode.Models.PagePropertiesModel.isAdmin = false;
                return RedirectToAction("Index", "Repository");
            }
            else
            {
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
                return View(model);
            }
        }

        //
        // POST: /User/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();
            GitCode.Models.PagePropertiesModel.isAdmin = false;

            return RedirectToAction("Index", "Repository");
        }

        //
        // POST : /User/ChangePassword()
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string before, string after, string confirmafter)
        {
            User user = db.Users.Find(WebSecurity.CurrentUserId);
            if(after != confirmafter)
            {
                ModelState.AddModelError("", "Password is not match.");
                return View(user);
            }
            else
            {
                if (WebSecurity.ChangePassword(WebSecurity.CurrentUserName, before, after))
                {
                    user.Password = after;
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    WebSecurity.Logout();
                    WebSecurity.Login(WebSecurity.CurrentUserName, after);
                    ModelState.AddModelError("", "Your password has been changed.");
                    return View(user);
                }
                else
                {
                    ModelState.AddModelError("", "Password is incorrect.");
                    return View(user);
                }
            }            
        }

        public ActionResult AllUser()
        {
            var allUser = db.Users.Select(u=>u);
            return View(allUser);
        }

        //Error Message
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
    }
}