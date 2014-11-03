using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GitCode
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            #region remoteURL
            routes.MapRoute(
                name: "RemoteURL",
                url: "{username}/{project}.git/{*verb}",
                defaults: new { controller = "Git", action = "Smart" }
                );
            /*
            routes.MapRoute(
                name: "Git",
                url: "{username}/{project}/{*verb}",
                defaults: new { controller = "Git", action = "Smart" }
            );*/
            #endregion     

            #region User
            /*routes.MapRoute(
                name: "User",
                url: "User/{action}/{name}",
                defaults: new { controller = "User" }
                );*/
            routes.MapRoute(
                name: "User",
                url: "User/{action}",
                defaults: new { controller = "User" }
                );
            #endregion

            #region Repository
            routes.MapRoute(
                name: "Repository",
                url: "repository/{action}/{*path}",
                defaults: new { controller = "Repository", path=""}
                );
            #endregion

            #region Team
            routes.MapRoute(
                name: "Team",
                url: "team/{action}",
                defaults: new { controller = "team" }
                );
            #endregion

            #region Class
            routes.MapRoute(
                name: "Class",
                url: "class/{action}",
                defaults: new { controller = "class" }
                );
            #endregion 

            #region Wiki

            routes.MapRoute(
                name: "Wiki",
                url: "wiki/{action}/{*path}",
                defaults: new { controller = "Wiki", path = "" }
            );

            #endregion

            #region Wiki

            routes.MapRoute(
                name: "GitCode",
                url: "gitcode/{action}/{*path}",
                defaults: new { controller = "GitCode", path = "" }
            );

            #endregion

            #region Feature

            routes.MapRoute(
                name: "Feature",
                url: "feature/{action}/{*path}",
                defaults: new { controller = "Feature", path = "" }
            );

            #endregion

            #region Issue

            routes.MapRoute(
                name: "Issue",
                url: "issue/{action}/{*path}",
                defaults: new { controller = "Issue", path = "" }
            );

            #endregion

            #region DetailTeam

            routes.MapRoute(
                name: "DetailTeam",
                url: "detailteam/{action}/{*path}",
                defaults: new { controller = "DetailTeam", path = "" }
            );

            #endregion

            #region Account;
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Repository", action = "Index", id = UrlParameter.Optional }
            );
            #endregion;
        }
    }
}