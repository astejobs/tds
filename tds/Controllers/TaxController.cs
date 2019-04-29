using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tds.Models;
using tds.RepositoryImpl;
using tds.RepositoryInterface;

namespace tds.Controllers
{
    public class TaxController : Controller 
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();
        GeneralInterface<Tax> generaInterface ;

        public TaxController() {
            generaInterface = new GeneralRepoImpl<Tax>();
        }



       

        [Route("tax/")]
        public ActionResult settings()
        {

            return View();
        }

        [HttpPost]
        [Route("tax/")]
        public ActionResult add(Tax tax)
        {
            generaInterface.Save(tax);
            return View();
        }
    }
}