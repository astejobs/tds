using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace tds.Models
{
    public class Transaction
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();

        public Transaction()
        {
           id = Guid.NewGuid().ToString();

            createDate = DateTime.Today;
        }


        public string id { get; set; }
        
        public string deductorId { get; set; }

        public virtual Deductor deductor { get; set; }

        [Required(ErrorMessage = "Contractor is required")]
        public string contractorId { get; set; }

        public virtual Contractor contractor { get; set; }

        [Required(ErrorMessage = "Amount is required")]
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
        public Double labourCessAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? createDate { get; set; }

       
        public Double deposit { get; set; }
        public Double netAmount { get; set; }

        public override string ToString()
        {
            return "Contractor "+contractor.name+" cgst  sgst  it lc dep"+cgstAmount+" "+sgstAmount+" "+itAmount+" "+labourCessAmount+" "+deposit;
        }
    }
}