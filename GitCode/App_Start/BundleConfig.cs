using System.Web;
using System.Web.Optimization;

namespace GitCode
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.autosize.js",
                        "~/Scripts/jsBootstrap/bootstrap.typeahead.js",
                        "~/Scripts/jsBootstrap/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            //Bootstrap
            bundles.Add(new StyleBundle("~/Content/bootstrapstyle").Include("~/Content/Bootstrap/bootstrap.css"));

            //Bootstrap Tab
            bundles.Add(new ScriptBundle("~/bundles/dropdownscript").Include(
                        "~/Scripts/jsBootstrap/bootstrap.min.js"));
            bundles.Add(new StyleBundle("~/Content/dropdownstyle").Include("~/Content/Bootstrap/bootstrap.min.css"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/Bootstrap/bootstrap.css",
                "~/Content/Content.css",
                "~/Content/markitup/skins/markitup/style.css",
                "~/Content/markitup/sets/wiki/style.css",
                "~/Content/jHtmlArea.css",
                "~/Content/site.css"));
            bundles.Add(new StyleBundle("~/Content/contentcss").Include("~/Content/Content.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));
        }
    }
}