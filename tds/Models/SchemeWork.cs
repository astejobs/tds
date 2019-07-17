using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace tds.Models
{
    public class SchemeWork
    {
        public SchemeWork()
        {
            this.Id = Guid.NewGuid().ToString();
            
        }
        public string Id { get; set; }
       
        public string SchemeId { get; set; }
   
        public string WorkId { get; set; }
        public virtual Scheme Scheme { get; set; }
        public virtual Work Work { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}