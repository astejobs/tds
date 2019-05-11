using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Rotativa;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using tds.Models;
using tds.RepositoryImpl;
using tds.RepositoryInterface;
using Constants = tds.Models.Constants;

namespace tds.Controllers
{[Authorize(Roles ="Admin")]
    public class TransactionController : Controller
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();
        static TransactionVM _transactions = null;
        GeneralInterface<Transaction> generalInterface;
        GeneralInterface<Contractor> contractorInterface;
        GeneralInterface<Deductor> deductorInterface;
        GeneralInterface<Tax> taxInterface;

        public TransactionController()
        {
            generalInterface = new GeneralRepoImpl<Transaction>();
            contractorInterface = new GeneralRepoImpl<Contractor>();
            taxInterface = new GeneralRepoImpl<Tax>();
            deductorInterface = new GeneralRepoImpl<Deductor>();
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

        [HttpGet]
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
                var user = UserManager.FindById(User.Identity.GetUserId());

                t = generalInterface.Find(id);
                t.contractor =(Contractor) contractorInterface.Find(t.contractorId);
                t.cgst =(Tax) taxInterface.Find(t.cgstId);
                t.sgst = (Tax)taxInterface.Find(t.sgstId);
                t.deductor = (Deductor)deductorInterface.Find(user.Id);
                t.labourCess = (Tax)taxInterface.Find(t.labourCessId);
                TempData["actionStatus"] = "Put";
            }

            if (TempData.ContainsKey("ModelState"))
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);
            
           
            
            ViewBag.contractors = contractorInterface.listAll(id);
            ViewBag.cgstTaxes = taxInterface.listTaxes(Constants.type_of_tax[0]).OrderBy(m => m.rate).ToList();
            ViewBag.sgstTaxes= taxInterface.listTaxes(Constants.type_of_tax[1]).OrderBy(m => m.rate).ToList();
            ViewBag.itTaxes= taxInterface.listTaxes(Constants.type_of_tax[2]).OrderBy(m => m.rate).ToList();
            ViewBag.labourTaxes= taxInterface.listTaxes(Constants.type_of_tax[3]).OrderBy(m => m.rate).ToList();
            return View("transaction", t);
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
                TempData["ModelState"] = ModelState;
                TempData["MsgFail"] = "Enter Valid Data";
                return RedirectToAction("Get");
            }
            return RedirectToAction("Get");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("transaction/update")]
        public ActionResult Put(Transaction transac)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());

             transac.contractor =(Contractor) contractorInterface.Find(transac.contractorId);
             transac.cgst = taxInterface.Find(transac.cgstId);
             transac.sgst = taxInterface.Find(transac.sgstId);
             transac.deductor = deductorInterface.Find(user.Id);
            
              transac.labourCess = taxInterface.Find(transac.labourCessId);
            if (ModelState["deposit"].Errors.Count == 0 && ModelState["amountPaid"].Errors.Count == 0 && ModelState["contractorId"].Errors.Count == 0)
            {      
                setAmounts(transac);
                if (generalInterface.Update(transac))
                {
                    TempData["MsgSuccess"] = "Transaction has been Updated Successfully";
                }
                else
                {
                    TempData["MsgFail"] = "Updation Failed,Enter Valid data";
                    TempData["ModelState"] = ModelState;
                }
            }
            else
            {
                TempData["MsgFail"] = "Updation Failed,Enter Valid data";
                TempData["ModelState"] = ModelState;
            }
            ViewBag.Contractors = contractorInterface.listAll(transac.id);
           // ViewBag.taxes = taxInterface.listAll(transac.id).OrderBy(m => m.rate).ToList();
            ViewBag.cgstTaxes = taxInterface.listTaxes(Constants.type_of_tax[0]).OrderBy(m => m.rate).ToList();
            ViewBag.sgstTaxes = taxInterface.listTaxes(Constants.type_of_tax[1]).OrderBy(m => m.rate).ToList();
            ViewBag.itTaxes = taxInterface.listTaxes(Constants.type_of_tax[2]).OrderBy(m => m.rate).ToList();
            ViewBag.labourTaxes = taxInterface.listTaxes(Constants.type_of_tax[3]).OrderBy(m => m.rate).ToList();
            TempData["actionStatus"] = "Put";
            return View("transaction");
        }

       /* [HttpGet]
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
                if(transCriteria.type== "Individual") 
                ViewBag.transList = generalInterface.Search(transCriteria, d1, d2, pageIndex);
               else
                    ViewBag.transList = generalInterface.SearchGeneral(transCriteria, d1, d2, pageIndex);
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

            DateTime dt = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime dt2 = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
             string g1 = Convert.ToDateTime(dt).ToString("yyyy-MM-dd HH:mm:ss.fff");
            string g2 = Convert.ToDateTime(dt2).ToString("yyyy-MM-dd HH:mm:ss.fff");

       
            DateTime d1 = DateTime.ParseExact(g1, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            DateTime d2 = DateTime.ParseExact(g2, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
           

            IPagedList<Transaction> transList=null;

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            if (transCriteria.type == "Individual")
            {
                ////////new changes////////////
                transList = generalInterface.Search(transCriteria, d1, d2,pageIndex);
                 ViewBag.transList = transList;
                if (transList != null)
                {
                    ViewBag.id = transList.FirstOrDefault().contractorId;
                    ViewBag.fromDate = d1;
                    ViewBag.toDate = d2;
                    ViewBag.type = "Individual";

                }
            }
            else
            {
                ////////new changes////////////
                transList = generalInterface.SearchGeneral(transCriteria, d1, d2,pageIndex);
               ViewBag.transList= transList;
                ViewBag.fromDate = d1;
                ViewBag.toDate = d2;
                ViewBag.type = "General";


            }
           
            ViewBag.Contractors = contractorInterface.listAll(id);
            ViewBag.types = Models.Constants.type;
            ViewBag.startDate = fromDate;
            ViewBag.endDate = toDate;

            return View("Search");
        }*/
    
        [HttpGet]
        [Route("transaction/search/")]
        public ActionResult search(int? page, string type,string contractorId,string GSTIN, string fromDate, string toDate, int? id)
        {
            string fromDatee = fromDate;
            string toDatee = toDate;
           
            ViewBag.Contractors = contractorInterface.listAll(id.ToString());
            ViewBag.types = Models.Constants.type;
            ViewBag.type = type;
            ViewBag.contractorId = contractorId;
            ViewBag.GSTN = GSTIN;
            IPagedList<Transaction> transList = null;
            

            if ((String.IsNullOrEmpty(fromDatee) || String.IsNullOrEmpty(toDatee)))
            {
                
                ViewData["DateMsg"] = "Please Select StartDate And EndDate";
                
                return View("Search");
            }
            else
            {
                DateTime dt = DateTime.ParseExact(fromDatee, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                DateTime dt2 = DateTime.ParseExact(toDatee, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string g1 = Convert.ToDateTime(dt).ToString("yyyy-MM-dd HH:mm:ss.fff");
                string g2 = Convert.ToDateTime(dt2).ToString("yyyy-MM-dd HH:mm:ss.fff");
                DateTime d1 = DateTime.ParseExact(g1, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                DateTime d2 = DateTime.ParseExact(g2, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
             
                ViewBag.startDate = fromDate;
                ViewBag.endDate = toDate;
                ViewBag.id = contractorId;
                int pageIndex = 1;


                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

                SearchViewModel transCriteria=new SearchViewModel();
                transCriteria.contractorId = contractorId;
                transCriteria.GSTIN = GSTIN;
                transCriteria.type = type;
                
                if (type == Constants.type[1])
                {
                   ViewBag.transList= generalInterface.Search(transCriteria, d1, d2, pageIndex);
                    ViewBag.typo = Constants.type[1];
                }
                else
                {
                    ViewBag.transList = generalInterface.SearchGeneral(transCriteria, d1, d2, pageIndex);
                    ViewBag.typo =Constants.type[0];
                }
                
                return View("Search");
            }
          

        }

        [HttpGet]
        [Route("transactions/")]
          public ActionResult DashBoard(int? page,string id)
          {
            ViewBag.check = "true";
            ViewBag.Contractors = contractorInterface.listAll(id);
            ViewBag.types = Models.Constants.type;
            ViewBag.type = Constants.type[1];
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            SearchViewModel transCriteria = new SearchViewModel();
            ViewBag.transList = generalInterface.listAll(id).ToPagedList(pageIndex,10);
            
            return View("Search");
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

        [AllowAnonymous]
        public ActionResult Invoice()
        {
            //List<Transaction> transList = new JavaScriptSerializer().Deserialize<List<Transaction>>(trans);

            return View(_transactions);
        }
        public ActionResult ExportToPdf(string id, string fromDate, string toDate)
        {
               ////new changes/////
            SearchViewModel VM = new SearchViewModel();
            VM.contractorId = id;
            DateTime dt = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime dt2 = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            string g1 = Convert.ToDateTime(dt).ToString("yyyy-MM-dd HH:mm:ss.fff");
            string g2 = Convert.ToDateTime(dt2).ToString("yyyy-MM-dd HH:mm:ss.fff");
            DateTime d1 = DateTime.ParseExact(g1, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            DateTime d2 = DateTime.ParseExact(g2, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            List<Transaction> trans = generalInterface.SearchForPdf(VM, d1, d2).ToList();
            _transactions = new TransactionVM();
            //foreach(var item in trans)
            //{
            //    _transactions.TotalAmountPaid += item.amountPaid;
            //    _transactions.TotalcgstAmount += item.cgstAmount;
            //    _transactions.Totaldeposit += item.deposit;
            //    _transactions.TotalitAmount += item.itAmount;
            //    _transactions.TotallabourCessAmount += item.labourCessAmount;
            //    _transactions.TotalnetAmount += item.netAmount;
            //    _transactions.TotalsgstAmount += item.sgstAmount;
            //}
            _transactions.Transactions = trans;

            Transaction total = new Transaction
            {
                
                amountPaid = trans.Sum(item => item.amountPaid),
                sgstAmount = trans.Sum(item => item.sgstAmount),
                cgstAmount = trans.Sum(item => item.cgstAmount),
                itAmount = trans.Sum(item => item.itAmount),
                deposit = trans.Sum(item => item.deposit),
                netAmount = trans.Sum(item => item.netAmount),
                labourCessAmount = trans.Sum(item => item.labourCessAmount),

            };
            trans.Add(total);
            //var trans = new JavaScriptSerializer().Serialize(transList);
            var report = new ActionAsPdf("Invoice");
            return report;
        }
    
        public ActionResult ExportToExcel(string fromDate, string toDate)
        {
            DateTime dt = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime dt2 = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            string g1 = Convert.ToDateTime(dt).ToString("yyyy-MM-dd HH:mm:ss.fff");
            string g2 = Convert.ToDateTime(dt2).ToString("yyyy-MM-dd HH:mm:ss.fff");
            DateTime d1 = DateTime.ParseExact(g1, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

            DateTime d2 = DateTime.ParseExact(g2, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

            //////new changes//////
            List<Transaction> excel = generalInterface.SearchGeneralExcel(d1 ,d2).ToList();

           
           

            Transaction total = new Transaction
            {
                contractor = new Contractor { name = "Total" },
                amountPaid = excel.Sum(item => item.amountPaid),
                sgstAmount = excel.Sum(item => item.sgstAmount),
                cgstAmount = excel.Sum(item => item.cgstAmount),
                itAmount = excel.Sum(item => item.itAmount),
                deposit = excel.Sum(item => item.deposit),
                netAmount = excel.Sum(item => item.netAmount),
                labourCessAmount = excel.Sum(item => item.labourCessAmount),

            };
            excel.Add(total);

            return View(excel);
        }





    }
}