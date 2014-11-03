using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitCode.Models
{
    public class ContributorModel
    {
        public string contributorName { get; set; }
        public string contributorEmail { get; set; }
        public int commitContrib { get; set; }
        public int featureContrib { get; set; }
    }
}