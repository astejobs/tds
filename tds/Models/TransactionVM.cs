using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace tds.Models
{
    public class TransactionVM
    {

      

        public string ContractorId { get; set; }

        public string ContractorName { get; set; }
        public Double AmountPaid { get; set; }

        public string GSTIN { get; set; }

        public Double CgstAmount { get; set; }
        public Double SgstAmount { get; set; }
        public Double ItAmount { get; set; }
        public Double LabourCessAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CreateDate { get; set; }
        public Double Deposit { get; set; }
        public Double NetAmount { get; set; }

    }
}