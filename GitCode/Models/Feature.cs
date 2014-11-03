using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GitCode.Models
{
    [Table("Feature")]
    public class Feature
    {
        [Key]
        public int FeatureId { get; set; }
        public string Detail { get; set; }
        public int RepositoryRepositoryId { get; set; }

        public virtual Repository Repository { get; set; }
    }
}