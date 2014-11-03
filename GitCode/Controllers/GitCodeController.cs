using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GitCode.Models;
using GitCode.DAL;
using System.Data.Entity;

namespace GitCode.Controllers
{
    public class GitCodeController : Controller
    {
        private GitCodeContext db = new GitCodeContext();
        //
        // GET: /GitCode/

        public ActionResult Setting()
        {
            if (db.GitCodeBasses.Any())
            {
                return View(db.GitCodeBasses.FirstOrDefault());
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Setting(GitCodeBase gitCodeSetting)
        {
            if (ModelState.IsValid)
            {
                if (db.GitCodeBasses.Any())
                {
                    gitCodeSetting.GitCodeBaseId = 1;
                    db.Entry(gitCodeSetting).State = EntityState.Modified;
                }
                else
                {
                    if (gitCodeSetting.AboutContent.Contains('<'))
                    {
                        String myEncodedString;
                        // Encode the string.
                        myEncodedString = HttpUtility.HtmlEncode(gitCodeSetting.AboutContent);
                    }
                    db.GitCodeBasses.Add(gitCodeSetting);
                }
                db.SaveChanges();
                return RedirectToAction("Index","Repository");
            }
            return View();
        }
    }
}
