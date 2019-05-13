using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tds.Models
{
    public class GeneralTransaction
    {
        public GeneralTransaction()
        {
            this.transactions = new List<Transaction>();
            this.itAmount = new List<double>();
            this.cgstAmount = new List<double>();
            this.labourCessAmount = new List<double>();
            this.sgstAmount = new List<double>();
            this.amountPaid = new List<double>();
        }

       
        public List<Transaction> transactions { get; set; }
        public List<Double> cgstAmount { get; set; }
        public List<Double> sgstAmount { get; set; }
        public List<Double> itAmount { get; set; }
        public List<Double> labourCessAmount { get; set; }
        public List<Double> amountPaid { get; set; }

    }
}