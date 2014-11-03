using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitCode.Models
{
    public class TagModel
    {
        public string name { get; set; }
        public string SHA { get; set; }
        public DateTimeOffset when { get; set; }
        public string message { get; set; }
    }
}