using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GitCode.Models
{
    [Table("DetailClassAccess")]
    public class DetailClassAccess
    {
        [Key]
        public int DetailClassAccessId { get; set; }
        public int UserId { get; set; }
        public int ClassId { get; set; }
        public string UserAccess { get; set; }

        
        public virtual User User { get; set; }
        
        public virtual Class Class { get; set; }
    }
}