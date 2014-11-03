using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GitCode.Models
{
    [Table("WikiDetail")]
    public class WikiDetail
    {
        [Key]
        public int WikiDetailId { get; set; }
        public int WikiId { get; set; }
        public string ImagePath { get; set; }

        public virtual Wiki Wiki { get; set; }
    }
}