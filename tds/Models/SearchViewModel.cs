using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tds.Models
{
    public class SearchViewModel
    {

        public SearchViewModel()
        {
        }
        public string type { get; set; }
       public string contractorId { get; set; }
       public string GSTIN { get; set; }

       
       public DateTime from { get; set; }
       public DateTime to { get; set; }
    }
}