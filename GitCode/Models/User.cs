using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GitCode.Models
{

    [Table("User")]
    public class User
    {
        public User()
        {
            this.DetailTeams = new HashSet<DetailTeam>();
            this.DetailClassAccesses = new HashSet<DetailClassAccess>();
            this.DetailRepositories = new HashSet<DetailRepository>();
            this.Issues = new HashSet<Issue>();
            this.Comments = new HashSet<Comment>();
        }

        [Key]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public virtual ICollection<DetailTeam> DetailTeams { get; set; }
        public virtual ICollection<DetailClassAccess> DetailClassAccesses { get; set; }
        public virtual ICollection<DetailRepository> DetailRepositories { get; set; }
        public virtual ICollection<Issue> Issues { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}