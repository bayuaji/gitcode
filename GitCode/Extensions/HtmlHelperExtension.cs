using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
namespace GitCode.Extensions
{
    public static class HtmlHelperExtension
    {
        public static RouteValueDictionary OverRoute(this HtmlHelper helper, object routeValues = null, bool withQuery = true)
        {
            var old = helper.ViewContext.RouteData.Values;

            if (routeValues == null)
                return old;

            var over = new Dictionary<string, object>(old, StringComparer.OrdinalIgnoreCase);
            if (withQuery)
            {
                var qs = helper.ViewContext.HttpContext.Request.QueryString;
                foreach (string key in qs)
                    over[key] = qs[key];
            }
            var values = new RouteValueDictionary(routeValues);
            foreach (var pair in values)
                over[pair.Key] = pair.Value;

            return new RouteValueDictionary(over);
        }
    }
}