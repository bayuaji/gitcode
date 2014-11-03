using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GitCode.Models
{
    [Table("GitCodeBase")]
    public class GitCodeBase
    {
        [Key]
        public int GitCodeBaseId { get; set; }
        public string AboutContent { get; set; }
        public string GitPath { get; set; }
        public string RepositoryPath { get; set; }
        public string CachePath { get; set; }
        [AllowHtml]
        public string CommitContent { get; set; }
        [AllowHtml]
        public string ClassContent { get; set; }
        [AllowHtml]
        public string TeamContent { get; set; }
        [AllowHtml]
        public string HomeContent { get; set; }
    }
}