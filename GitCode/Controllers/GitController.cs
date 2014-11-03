using GitCode.Data;
using GitCode.Filters;
using GitCode.Git;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace GitCode.Controllers
{
    public class GitController : Controller
    {
        private TransferContent transferContent = new TransferContent();
        [SmartGit]
        public ActionResult Smart(string username, string project, string service, string verb)
        {
            switch (verb)
            {
                case "info/refs":
                    return InfoRefs(username, project, service);
                case "git-upload-pack":
                    return ExecutePack(username, project, "git-upload-pack");
                case "git-receive-pack":
                    return ExecutePack(username, project, "git-receive-pack");
                default:
                    return RedirectToAction("Tree", "Repository", new { Name = project });
            }
        }

        protected ActionResult InfoRefs(string username,string project, string service)
        {
            Response.Charset = "";
            Response.ContentType = String.Format(CultureInfo.InvariantCulture, "application/x-{0}-advertisement", service);
            SetNoCache();
            Response.Write(FormatMessage(String.Format(CultureInfo.InvariantCulture, "# service={0}\n", service)));
            Response.Write(FlushMessage());

            try
            {
                using (var git = new GitService(GitService.GetDirectoryInfo(project,username,transferContent.getRepoDirectory()).FullName))
                {
                    var svc = service.Substring(4);
                    git.InfoRefs(svc, username, GetInputStream(), Response.OutputStream);
                }
                return new EmptyResult();
            }
            catch (RepositoryNotFoundException e)
            {
                throw new HttpException((int)HttpStatusCode.NotFound, string.Empty, e);
            }
            catch (Exception e)
            {
                throw new HttpException((int)HttpStatusCode.InternalServerError, string.Empty, e);
            }
        }

        protected ActionResult ExecutePack(string username, string project, string service)
        {
            try
            {
                Response.Charset = "";
                Response.ContentType = String.Format(CultureInfo.InvariantCulture, "application/x-{0}-result", service);
                SetNoCache();

                try
                {
                    using (var git = new GitService(GitService.GetDirectoryInfo(project, username, transferContent.getRepoDirectory()).FullName))
                    {
                        var svc = service.Substring(4);
                        git.ExecutePack(svc, username, GetInputStream(), Response.OutputStream);
                    }
                    return new EmptyResult();
                }
                catch (RepositoryNotFoundException e)
                {
                    throw new HttpException((int)HttpStatusCode.NotFound, string.Empty, e);
                }
                catch (Exception e)
                {
                    throw new HttpException((int)HttpStatusCode.InternalServerError, string.Empty, e);
                }
            }
            catch (Exception e)
            {
                throw new HttpException((int)HttpStatusCode.InternalServerError, string.Empty, e);
            }
        }

        private void SetNoCache()
        {
            Response.AddHeader("Expires", "Fri, 01 Jan 1980 00:00:00 GMT");
            Response.AddHeader("Pragma", "no-cache");
            Response.AddHeader("Cache-Control", "no-cache, max-age=0, must-revalidate");
        }

        private Stream GetInputStream()
        {
            if (Request.Headers["Content-Encoding"] == "gzip")
            {
                return new GZipStream(Request.InputStream, CompressionMode.Decompress);
            }
            return Request.InputStream;
        }

        private static string FormatMessage(string input)
        {
            return (input.Length + 4).ToString("X4", CultureInfo.InvariantCulture) + input;
        }

        private static string FlushMessage()
        {
            return "0000";
        }
    }
}
