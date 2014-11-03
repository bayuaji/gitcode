using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GitCode.Models
{
    [Table("DetailTeam")]
    public class DetailTeam
    {
        [Key]
        public int DetailTeamId { get; set; }
        public int UserId { get; set; }
        public int TeamId { get; set; }
        public bool IsOwner { get; set; }
        public string Role { get; set; }

        public virtual User User { get; set; }
        public virtual Team Team { get; set; }
    }
}