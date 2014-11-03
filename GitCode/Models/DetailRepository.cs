using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GitCode.Models
{
    [Table("DetailRepository")]
    public class DetailRepository
    {
        [Key]
        public int DetailRepositoryId { get; set; }
        public bool IsOwner { get; set; }
        public int UserId { get; set; }
        public int RepositoryId { get; set; }

        public virtual User User { get; set; }
        public virtual Repository Repository { get; set; }
    }
}