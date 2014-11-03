using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GitCode.Models
{
    [Table("Repository")]
    public class Repository
    {
        public Repository()
        {
            this.DetailRepositories = new HashSet<DetailRepository>();
            this.Features = new HashSet<Feature>();
            this.Issues = new HashSet<Issue>();
        }

        [Key]
        public int RepositoryId { get; set; }
        public bool IsPublic { get; set; }
        public bool IsPublicRead { get; set; }
        public bool IsPublicWrite { get; set; }
        public Nullable<int> TeamId { get; set; }
        public string User { get; set; }
        public string Project { get; set; }
        public string ShortDescription { get; set; }

        public virtual ICollection<DetailRepository> DetailRepositories { get; set; }
        public virtual Team Team { get; set; }
        public virtual ICollection<Feature> Features { get; set; }
        public virtual ICollection<Issue> Issues { get; set; }
    }
}