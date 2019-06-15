using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace tds.Models
{
    public class Scheme
    {
        public Scheme()
        {
          this.Id = Guid.NewGuid().ToString();

        }
        public string Id { get; set; }
        [Required]
        public string AccountNo { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}