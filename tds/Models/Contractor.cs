using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace tds.Models
{
    public class Contractor
    {

        public Contractor()
        {
            this.id = Guid.NewGuid().ToString();
        }
        public string id { get; set; }

      //  [Required(ErrorMessage = "Name is required")]
        public string name { get; set; }

       // [Required(ErrorMessage = "GSTIN is required")]
        public string GSTIN { get; set; }
        

        //[Required(ErrorMessage = "Registration No. is required")]
        public string regNo { get; set; }

        public long phoneNo { get; set; }

        public string address { get; set; }

    }
}