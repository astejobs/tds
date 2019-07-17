using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tds.Models
{
    public class Work
    {
        public Work()
        {
            this.Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public ICollection<SchemeWork> SchemeWorks { get; set; }
    }
}