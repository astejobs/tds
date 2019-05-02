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
       public IEnumerable<string> contractorIds { get; set; }
       public IEnumerable<string> GSTIN;

       
       public DateTime from { get; set; }
       public DateTime to { get; set; }
    }
}