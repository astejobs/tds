using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace tds.Models
{
    public class SearchViewModel
    {

        public SearchViewModel()
        {
        }
        public string Type { get; set; }
       public string ContractorId { get; set; }
       public string GSTIN { get; set; }
        public string IsActive { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime FromDate { get; set; }
        public int PageIndex { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime ToDate { get; set; }
    }
}