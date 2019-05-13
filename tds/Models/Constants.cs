using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tds.Models
{
    public class Constants
    {
        public static List<string> type = new List<string>(new string[] { "General", "Individual"});
        public static List<string> type_of_tax = new List<string>(new string[] { "CGST", "SGST","IT","LabourCess" });
    }
}