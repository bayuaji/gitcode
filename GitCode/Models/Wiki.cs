using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GitCode.Models
{
    [Table("Wiki")]
    public class Wiki
    {
        public Wiki()
        {
            this.WikiDetails = new HashSet<WikiDetail>();
        }

        [Key]
        public int WikiId { get; set; }
        public string Title { get; set; }
        [AllowHtml]
        public string Content { get; set; }

        public virtual Repository Repository { get; set; }
        public virtual ICollection<WikiDetail> WikiDetails { get; set; }
    }
}