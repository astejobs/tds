using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace tds.Models
{
    public class Tax
    {


        public Tax()
        {
            this.id = Guid.NewGuid().ToString();
           
        }
        
        public string id { get; set; }
        [Required(ErrorMessage = "Rate is required")]
        public double rate { get; set;}
       [Required(ErrorMessage = "Type is required")]
        public string type { get; set; }

    }

   


}