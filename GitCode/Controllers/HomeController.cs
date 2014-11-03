using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GitCode.Data;
using GitCode.DAL;
namespace GitCode.Controllers
{
    public class HomeController : Controller
    {
        private TransferContent transContent = new TransferContent();
        private GitCodeContext db = new GitCodeContext();
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            
            ViewBag.Message = transContent.getAbout();

            return View(db.GitCodeBasses.FirstOrDefault());
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
