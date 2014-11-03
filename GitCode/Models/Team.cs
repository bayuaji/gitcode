using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GitCode.Models
{
    [Table("Team")]
    public class Team
    {
        public Team()
        {
            this.DetailTeams = new HashSet<DetailTeam>();
            this.DetailClasses = new HashSet<DetailClass>();
            this.Repositories = new HashSet<Repository>();
        }

        [Key]
        public int TeamId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<DetailTeam> DetailTeams { get; set; }
        public virtual ICollection<DetailClass> DetailClasses { get; set; }
        public virtual ICollection<Repository> Repositories { get; set; }

    }
}