using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tds.Models;
using tds.RepositoryImpl;
using tds.RepositoryInterface;

namespace tds.Controllers
{[Authorize(Roles ="Admin")]
    public class TransactionController : Controller
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();
        GeneralInterface<Transaction> generalInterface;
        GeneralInterface<Contractor> contractorInterface;
        GeneralInterface<Tax> taxInterface;

        public TransactionController()
        {
            generalInterface = new GeneralRepoImpl<Transaction>();
            contractorInterface = new GeneralRepoImpl<Contractor>();
            taxInterface = new GeneralRepoImpl<Tax>();
        }

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        [Route("transaction/")]
        public ActionResult Get(string id)
        {
            Transaction t = new Transaction();
            if (id == null)
            {
                t= null;
                TempData["actionStatus"] = "Post";
            }
            else
            {
                t=generalInterface.Find(id);                
                TempData["actionStatus"] = "Put";
            }
            
           
            
            ViewBag.contractors = contractorInterface.listAll(id);
            ViewBag.taxes = taxInterface.listAll(id).OrderBy(m => m.rate).ToList();
            return View("transaction",t);
        }

        [HttpPost]
        [Route("transaction/")]
        [ValidateAntiForgeryToken]
        public ActionResult Post(Transaction transaction)
        {
           

            if (ModelState.IsValid)
            {
                setAmounts(transaction);
                if (generalInterface.Save(transaction))
                {
                    TempData["MsgSuccess"] = "Transaction has been Saved Successfully";
                }
                else {
                    TempData["MsgFail"] = "Enter Valid Data";
                }
                }
            else
            {
                TempData["MsgFail"] = "Enter Valid Data";
                return RedirectToAction("Get");
            }
            return RedirectToAction("Get");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("transaction/update")]
        public ActionResult Put(Transaction transaction)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                setAmounts(transaction);
                generalInterface.Update(transaction);
                TempData["MsgSuccess"] = "Transaction has been Updated Successfully";
            }
            else
            {
                TempData["MsgFail"] = "Updation Failed,Enter Valid data";
            }
            return RedirectToAction("Get");
        }

        [HttpGet]
        [Route("transaction/search")]
        public ActionResult Search(int? page, string fromDate, string toDate,int? id)
        {
            String fromDatee = fromDate;
            String toDatee = toDate;
            SearchViewModel transCriteria = (SearchViewModel)Session["transCriteria"];

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1; 
            
            ViewBag.types = Models.Constants.type;
            ViewBag.Contractors = contractorInterface.listAll(id.ToString());
            System.Diagnostics.Debug.WriteLine(fromDate + "----*******");
            if (!(String.IsNullOrEmpty(fromDatee) || String.IsNullOrEmpty(toDatee)))
            {
                DateTime dt = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                DateTime dt2 = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string g1 = Convert.ToDateTime(dt).ToString("yyyy-MM-dd HH:mm:ss.fff");
                string g2 = Convert.ToDateTime(dt2).ToString("yyyy-MM-dd HH:mm:ss.fff");

                System.Diagnostics.Debug.WriteLine(g1 + "jjjj***jjjjj");
                
                DateTime d1 = DateTime.ParseExact(g1, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                DateTime d2 = DateTime.ParseExact(g2, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                ViewBag.transList = generalInterface.Search(transCriteria, d1, d2, pageIndex);
               
            }
            ViewBag.startDate = fromDate;
             ViewBag.endDate = toDate;
            
            return View("Search", transCriteria);
        }


        [HttpPost]
        [Route("transaction/search")]
        public ActionResult Search(int? page,SearchViewModel transCriteria,string fromDate,string toDate,string btnName,string id)
        {

            Session["transCriteria"]= transCriteria;
            System.Diagnostics.Debug.WriteLine(Request["fromDate"] + "jjjjjjjjj");
            System.Diagnostics.Debug.WriteLine(Request["toDate"] + "jjjj***jjjjj");

            DateTime dt = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime dt2 = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
             string g1 = Convert.ToDateTime(dt).ToString("yyyy-MM-dd HH:mm:ss.fff");
            string g2 = Convert.ToDateTime(dt2).ToString("yyyy-MM-dd HH:mm:ss.fff");

            System.Diagnostics.Debug.WriteLine(g1+ "jjjj***jjjjj");

            DateTime d1 = DateTime.ParseExact(g1, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            DateTime d2 = DateTime.ParseExact(g2, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            System.Diagnostics.Debug.WriteLine(d1 + "jjjjstarjjjjj");

            IPagedList<Transaction> transList=null;

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            if (transCriteria.type == "Individual")
            {
                transList = generalInterface.Search(transCriteria, d1, d2,pageIndex);
              //  transCriteria.transList= generalInterface.Search(transCriteria, d1, d2, pageIndex);
                 ViewBag.transList = transList;

            }
            else
            {
              transList= generalInterface.SearchGeneral(transCriteria, d1, d2,pageIndex);
                //  transCriteria.transList = generalInterface.SearchGeneral(transCriteria, d1, d2, pageIndex);
              ViewBag.transList= transList;


            }
           
            ViewBag.Contractors = contractorInterface.listAll(id);
            ViewBag.types = Models.Constants.type;
            ViewBag.startDate = fromDate;
            ViewBag.endDate = toDate;

            return View("Search",transCriteria);
        }
    
        public void setAmounts(Transaction transaction)
        {

            var user = UserManager.FindById(User.Identity.GetUserId());

            transaction.sgstAmount = (dbContext.Tax.Find(transaction.sgstId).rate / 100 * transaction.amountPaid);
            transaction.cgstAmount = (dbContext.Tax.Find(transaction.cgstId).rate / 100 * transaction.amountPaid);
            transaction.itAmount = (dbContext.Tax.Find(transaction.incometaxtId).rate / 100 * transaction.amountPaid);
            transaction.labourCessAmount = (dbContext.Tax.Find(transaction.labourCessId).rate / 100 * transaction.amountPaid);
            transaction.netAmount = transaction.amountPaid - transaction.sgstAmount - transaction.cgstAmount - transaction.labourCessAmount - transaction.itAmount;
            transaction.deductorId = user.Id;
        }
    }
}