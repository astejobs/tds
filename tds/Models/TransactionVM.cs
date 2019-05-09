using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tds.Models
{
    public class TransactionVM
    {

        public List<Transaction> Transactions{ get; set; }
        public Double TotalAmountPaid { get; set; }
        public Double TotalcgstAmount { get; set; }
        public Double TotalsgstAmount { get; set; }
        public Double TotalitAmount { get; set; }
        public Double TotallabourCessAmount { get; set; }
        public Double Totaldeposit { get; set; }
        public Double TotalnetAmount { get; set; }

    }
}