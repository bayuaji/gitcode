using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GitCode.Models
{
    [Table("Issue")]
    public class Issue
    {
        public Issue()
        {
            this.Comments = new HashSet<Comment>();
        }
        [Key]
        public int IssueId { get; set; }
        public int RepositoryId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime When { get; set; }
        public Boolean IsClear { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual Repository Repository { get; set; }
        public virtual User User { get; set; }
    }
}