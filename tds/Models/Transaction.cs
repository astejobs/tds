using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tds.Models
{
    public class Transaction
    {
        public string id { get; set; }
        
        public string deductorId { get; set; }

        public virtual Deductor deductor { get; set; }

        public string contractorId { get; set; }

        public virtual Contractor contractor { get; set; }

        public Double amountPaid { get; set; }

        public string cgstId { get; set; }
        public virtual Tax cgst { get; set; }

        public string sgstId { get; set; }
        public virtual Tax sgst { get; set; }

        public string incometaxtId { get; set; }
        public virtual Tax incomeTax { get; set; }

        public string labourCessId { get; set; }
        public virtual Tax labourCess { get; set; }

        public Double cgstAmount { get; set; }
        public Double sgstAmount { get; set; }
        public Double itAmount { get; set; }



    }
}