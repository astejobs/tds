using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace tds.Models
{
    public class Deductor
    {

        public Deductor()
        {
            
        }
        public string id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string legalName { get; set; }

       
        public string GSTIN { get; set; }

        public string tradeName { get; set; }
        public string departmentName { get; set; }
    }
}