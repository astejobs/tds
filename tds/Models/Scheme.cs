using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace tds.Models
{
    public class Scheme
    {
        public Scheme()
        {
          this.Id = Guid.NewGuid().ToString();
            this.Works = new List<string>();

        }
        public string Id { get; set; }
        [Required]
        public string AccountNo { get; set; }
        [NotMapped]
        public List<string> Works { get; set; }
        public ICollection<SchemeWork> SchemeWorks { get; set; }
    }
}