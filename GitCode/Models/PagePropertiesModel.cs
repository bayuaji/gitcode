using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitCode.Models
{
    public static class PagePropertiesModel
    {
        public static string projectName { get; set; }
        public static string user { get; set; }
        public static string project { get; set; }
        public static bool isAdmin { get; set; }
    }
}