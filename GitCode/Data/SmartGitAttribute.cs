using GitCode.Controllers;
using GitCode.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace GitCode.Filters
{
    public class SmartGitAttribute : SmartAuthorizeAttribute
    {
        private const string AuthKey = "GitCodeGitAuthorize";
        private GitCodeContext db = new GitCodeContext();
        private string project;
        private string verb;
        private RepositoryController reposervice = new RepositoryController();
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            var right = false;
            var userfound = false;
            string password = "";
            string username = "";
            List<string> paramParsing = new List<string>();
            string url = HttpContext.Current.Request.Url.AbsoluteUri;

            //url.Split("")
            //base.OnAuthorization(filterContext);
            var controller = filterContext.Controller as GitController;
            if (controller == null)
                return;

            // git.exe not accept cookies as well as no session available
            var auth = controller.HttpContext.Request.Headers["Authorization"];

            if (!String.IsNullOrEmpty(auth))
            {
                var bytes = Convert.FromBase64String(auth.Substring(6));
                var certificate = Encoding.ASCII.GetString(bytes);
                var index = certificate.IndexOf(':');
                password = certificate.Substring(index + 1);
                username = certificate.Substring(0, index);

                if (username != "" && password != "")
                //var user = controller.MembershipService.Login(username, password);
                if (WebSecurity.Login(username, password))
                {
                    WebSecurity.Login(username, password);
                    userfound = true;
                }
            }

            var projectField = controller.ValueProvider.GetValue("project");
            var serviceField = controller.ValueProvider.GetValue("service");
            var verbField = controller.ValueProvider.GetValue("service");
            //filterContext.Controller.ValueProvider
            var project = projectField == null ? null : projectField.AttemptedValue;
            var service = serviceField == null ? null : serviceField.AttemptedValue;
            
            if (string.IsNullOrEmpty(service) && userfound) // redirect to git browser
            {
                right = true;
            }
            else if (string.Equals(service, "git-receive-pack", StringComparison.OrdinalIgnoreCase) && userfound) // git push
            {
                //right = controller.RepositoryService.CanWriteRepository(project, username);
                right = reposervice.userCanWrite(username, project);
            }
            else if (string.Equals(service, "git-upload-pack", StringComparison.OrdinalIgnoreCase) && userfound ) // git fetch
            {
                //right = controller.RepositoryService.CanReadRepository(project, username);
                right = reposervice.userCanWrite(username, project);
            }

            if (!userfound)
            {
                if (WebSecurity.CurrentUserName == "")
                {
                    filterContext.HttpContext.Response.Clear();
                    //filterContext.HttpContext.Response.StatusDescription = "Unauthorized";
                    filterContext.HttpContext.Response.AddHeader("WWW-Authenticate", "Basic realm=\"Secure Area\"");
                    //filterContext.HttpContext.Response.Write("401, please authenticate");
                    filterContext.HttpContext.Response.StatusCode = 401;
                    filterContext.Result = new EmptyResult();
                    filterContext.HttpContext.Response.End();
                }
                else
                if(right == false)
                {
                    throw new UnauthorizedAccessException();
                }
            }
            else
                if(userfound && !right)
                {
                    throw new UnauthorizedAccessException();

            }
        }

        public List<string> urlTranslator(string url)
        {
            List<string> returnVal = new List<string>();
            List<string> holderVal = new List<string>();

            holderVal = Regex.Split(url, "%3f").ToList();
            returnVal = Regex.Split(holderVal[0], "%2f").ToList();
            if(returnVal.Count >5)
            {
                returnVal[4] = returnVal[4] + "/" + returnVal[5];
            }

            return returnVal;
        }
    }
}