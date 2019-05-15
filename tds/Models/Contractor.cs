using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace tds.Models
{
    public class Contractor
    {

        public Contractor()
        {
            this.id = Guid.NewGuid().ToString();
            this.status = true;
        }
        public string id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string name { get; set; }

       [Required(ErrorMessage = "GSTIN is required")]
        public string GSTIN { get; set; }
        

        [Required(ErrorMessage = "Registration No. is required")]
        public string regNo { get; set; }

        [Required(ErrorMessage = "Phone No. is required")]
        public long phoneNo { get; set; }

        public string address { get; set; }

        public Boolean status { get; set; }

    }
}