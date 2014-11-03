using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GitCode.Models
{
    [Table("Comment")]
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public int IssueId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public string When { get; set; }

        public virtual Issue Issue { get; set; }
        public virtual User User { get; set; }
    }
}