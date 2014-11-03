using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GitCode.Models
{
    [Table("Class")]
    public class Class
    {
        public Class()
        {
            this.DetailClasses = new HashSet<DetailClass>();
            this.DetailClassAccesses = new HashSet<DetailClassAccess>();
        }

        [Key]
        public int ClassId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<DetailClass> DetailClasses { get; set; }
        public virtual ICollection<DetailClassAccess> DetailClassAccesses { get; set; }
    }
}