using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GitCode.Models
{
    [Table("DetailClass")]
    public class DetailClass
    {
        [Key]
        public int DetailClassId { get; set; }
        public int TeamId { get; set; }
        public int ClassId { get; set; }

        
        public virtual Team Team { get; set; }
        
        public virtual Class Class { get; set; }
    }
}