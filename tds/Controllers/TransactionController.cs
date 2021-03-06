﻿using Microsoft.AspNet.Identity;
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
using System.Data.Entity;

namespace tds.Controllers
{
[Authorize(Roles ="Admin")]
    public class TransactionController : Controller
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();
        static List<Transaction> _transactions = null;
        GeneralInterface<Transaction> generalInterface;
        GeneralInterface<Contractor> contractorInterface;
        GeneralInterface<Deductor> deductorInterface;
        GeneralInterface<Tax> taxInterface;
        static private Transaction _transPrint;

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
            ViewBag.Schemes = dbContext.Schemes;
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
                ViewBag.SchemeId = t.SchemeWork.SchemeId;
                ViewBag.WorkId = t.SchemeWorkId;
               t.contractor =(Contractor) contractorInterface.Find(t.contractorId);
                t.cgst =(Tax) taxInterface.Find(t.cgstId);
                t.sgst = (Tax)taxInterface.Find(t.sgstId);
                t.deductor = (Deductor)deductorInterface.Find(user.Id);
                t.labourCess = (Tax)taxInterface.Find(t.labourCessId);
                TempData["actionStatus"] = "Put";
            }

            if (TempData.ContainsKey("ModelState"))
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);


          
            ViewBag.contractors = contractorInterface.listActiveContractors();
            ViewBag.cgstTaxes = taxInterface.listTaxes(Constants.type_of_tax[0]).OrderBy(m => m.rate).ToList();
            ViewBag.sgstTaxes= taxInterface.listTaxes(Constants.type_of_tax[1]).OrderBy(m => m.rate).ToList();
            ViewBag.itTaxes= taxInterface.listTaxes(Constants.type_of_tax[2]).OrderBy(m => m.rate).ToList();
            ViewBag.labourTaxes= taxInterface.listTaxes(Constants.type_of_tax[3]).OrderBy(m => m.rate).ToList();
            return View("transaction", t);
        }

        [HttpPost]
    
        [Route("transaction/")]
        [ValidateAntiForgeryToken]
        public ActionResult Post([Bind(Exclude = "createDate")] Transaction transaction,string createDate)
        {
            DateTime dt = DateTime.ParseExact(createDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            string g1 = Convert.ToDateTime(dt).ToString("yyyy-MM-dd HH:mm:ss.fff");
          transaction.createDate = DateTime.ParseExact(g1, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

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
        public ActionResult Put(Transaction transac,string createDate)
        {
            DateTime dt = DateTime.ParseExact(createDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            string g1 = Convert.ToDateTime(dt).ToString("yyyy-MM-dd HH:mm:ss.fff");
            transac.createDate = DateTime.ParseExact(g1, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

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
            ViewBag.Contractors = contractorInterface.listActiveContractors();
           // ViewBag.taxes = taxInterface.listAll(transac.id).OrderBy(m => m.rate).ToList();
            ViewBag.cgstTaxes = taxInterface.listTaxes(Constants.type_of_tax[0]).OrderBy(m => m.rate).ToList();
            ViewBag.sgstTaxes = taxInterface.listTaxes(Constants.type_of_tax[1]).OrderBy(m => m.rate).ToList();
            ViewBag.itTaxes = taxInterface.listTaxes(Constants.type_of_tax[2]).OrderBy(m => m.rate).ToList();
            ViewBag.labourTaxes = taxInterface.listTaxes(Constants.type_of_tax[3]).OrderBy(m => m.rate).ToList();
            TempData["actionStatus"] = "Put";
            return RedirectToAction("Get",new {id=transac.id});
        }

      
        [HttpGet]
        [Route("transaction/search/")]
        public ActionResult search(int? page, string type,string contractorId,string GSTIN, string fromDate, string toDate, int? id,string isActive,string schemeId)
        {
            string fromDatee = fromDate;
            string toDatee = toDate;
           
            ViewBag.Contractors = contractorInterface.listActiveContractors();
            ViewBag.types = Models.Constants.type;
            ViewBag.type = type;
            ViewBag.contractorId = contractorId;
            ViewBag.GSTN = GSTIN;
            ViewBag.SchemeId = new SelectList(dbContext.Schemes, "Id", "AccountNo");
            IPagedList<Transaction> transList = null;
            

            if ((String.IsNullOrEmpty(fromDatee) || String.IsNullOrEmpty(toDatee)))
            {

                ViewBag.DateMsg = "Please Select StartDate And EndDate";
                
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
                transCriteria.ContractorId = contractorId;
                transCriteria.GSTIN = GSTIN;
                transCriteria.Type = type;
                
             if (type == Constants.type[1])
                    {
                    ViewBag.scheme = schemeId;
                    ViewBag.typo = Constants.type[1];
                    if(!(string.IsNullOrEmpty(schemeId)))
                    {
                        ViewBag.transList = generalInterface.SearchIndividual(m => (m.contractorId == contractorId || m.contractor.GSTIN == GSTIN) && m.SchemeWork.SchemeId == schemeId && DbFunctions.TruncateTime(m.createDate) >= DbFunctions.TruncateTime(d1) && DbFunctions.TruncateTime(m.createDate) <= DbFunctions.TruncateTime(d2), pageIndex);
                    }
                    else
                    {
                        ViewBag.transList = generalInterface.SearchIndividual(m => (m.contractorId == contractorId || m.contractor.GSTIN == GSTIN) && DbFunctions.TruncateTime(m.createDate) >= DbFunctions.TruncateTime(d1) && DbFunctions.TruncateTime(m.createDate) <= DbFunctions.TruncateTime(d2), pageIndex);
                    }
                 
                    }
                    else
                    {
                        if (isActive == "Active")
                        {
                            ViewBag.transList = generalInterface.SearchGeneralJSon(m => DbFunctions.TruncateTime(m.createDate) >= DbFunctions.TruncateTime(d1) && DbFunctions.TruncateTime(m.createDate) <= DbFunctions.TruncateTime(d2) && m.contractor.status == true, pageIndex);
                        }
                        else if (isActive == "InActive")
                        {
                            ViewBag.transList = generalInterface.SearchGeneralJSon(m => DbFunctions.TruncateTime(m.createDate) >= DbFunctions.TruncateTime(d1) && DbFunctions.TruncateTime(m.createDate) <= DbFunctions.TruncateTime(d2) && m.contractor.status == false, pageIndex);
                        }
                        else
                        {
                            ViewBag.transList = generalInterface.SearchGeneralJSon(m => DbFunctions.TruncateTime(m.createDate) >= DbFunctions.TruncateTime(d1) && DbFunctions.TruncateTime(m.createDate) <= DbFunctions.TruncateTime(d2), pageIndex);
                        }
                    ViewBag.typo = Constants.type[0];
                    ViewBag.activeType = isActive;
                }
                    
             
                
                return View("Search");
            }
          

        }
      

        [HttpGet]
        [Route("transactions/")]
          public ActionResult DashBoard(int? page,string id)
          {
            ViewBag.check = "true";
            ViewBag.Contractors = contractorInterface.listActiveContractors();
            ViewBag.types = Models.Constants.type;
            ViewBag.type = Constants.type[1];
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            ViewBag.transList = generalInterface.listAll(id).ToPagedList(pageIndex,10);
            
            return View("Search");
        }

        public void setAmounts(Transaction transaction)
        {

            var user = UserManager.FindById(User.Identity.GetUserId());

            transaction.sgstAmount = (dbContext.Tax.Find(transaction.sgstId).rate / 100 * transaction.amountPaid);
            transaction.cgstAmount = (dbContext.Tax.Find(transaction.cgstId).rate / 100 * transaction.amountPaid);
            transaction.itAmount = (dbContext.Tax.Find(transaction.incomeTaxId).rate / 100 * transaction.amountPaid);
            transaction.labourCessAmount = (dbContext.Tax.Find(transaction.labourCessId).rate / 100 * transaction.amountPaid);
            transaction.netAmount = transaction.amountPaid - (transaction.sgstAmount + transaction.cgstAmount + transaction.labourCessAmount + transaction.itAmount+Convert.ToDouble(transaction.deposit));
            transaction.deductorId = user.Id;
        }

        [AllowAnonymous]
        public ActionResult Invoice()
        {
            //List<Transaction> transList = new JavaScriptSerializer().Deserialize<List<Transaction>>(trans);

            return View(_transactions);
        }
        public ActionResult ExportToPdf(string id, string Gstin, string fromDate, string toDate,string scheme)
        {
               ////new changes/////
            SearchViewModel VM = new SearchViewModel();
            VM.ContractorId = id;
            VM.GSTIN = Gstin;
            DateTime dt = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime dt2 = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            string g1 = Convert.ToDateTime(dt).ToString("yyyy-MM-dd HH:mm:ss.fff");
            string g2 = Convert.ToDateTime(dt2).ToString("yyyy-MM-dd HH:mm:ss.fff");
            DateTime d1 = DateTime.ParseExact(g1, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            DateTime d2 = DateTime.ParseExact(g2, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            if (!(string.IsNullOrEmpty(scheme)))
            {
                _transactions = generalInterface.SearchForPdf(m => (m.contractorId == id || m.contractor.GSTIN == Gstin) && m.SchemeWork.SchemeId == scheme&& DbFunctions.TruncateTime(m.createDate) >= DbFunctions.TruncateTime(d1) && DbFunctions.TruncateTime(m.createDate) <= DbFunctions.TruncateTime(d2)).ToList();
            }
            else
            {
                _transactions = generalInterface.SearchForPdf(m => (m.contractorId == id || m.contractor.GSTIN == Gstin) && DbFunctions.TruncateTime(m.createDate) >= DbFunctions.TruncateTime(d1) && DbFunctions.TruncateTime(m.createDate) <= DbFunctions.TruncateTime(d2)).ToList();
            }


            Transaction total = new Transaction
            {
                
                amountPaid = _transactions.Sum(item => item.amountPaid),
                sgstAmount = _transactions.Sum(item => item.sgstAmount),
                cgstAmount = _transactions.Sum(item => item.cgstAmount),
                itAmount = _transactions.Sum(item => item.itAmount),
                deposit = _transactions.Sum(item => item.deposit),
                netAmount = _transactions.Sum(item => item.netAmount),
                labourCessAmount = _transactions.Sum(item => item.labourCessAmount),

            };
            _transactions.Add(total);
            //var trans = new JavaScriptSerializer().Serialize(transList);
            var report = new ActionAsPdf("Invoice");
            return report;
        }
    
        public ActionResult ExportToExcel(string fromDate, string toDate,string activeType)
        {
            DateTime dt = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime dt2 = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            string g1 = Convert.ToDateTime(dt).ToString("yyyy-MM-dd HH:mm:ss.fff");
            string g2 = Convert.ToDateTime(dt2).ToString("yyyy-MM-dd HH:mm:ss.fff");
            DateTime d1 = DateTime.ParseExact(g1, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

            DateTime d2 = DateTime.ParseExact(g2, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

            //////new changes//////
            ///
            List<Transaction> excel = new List<Transaction>();
            if (activeType == "Active")
            {
                 excel = generalInterface.SearchGeneralExcel(m => DbFunctions.TruncateTime(m.createDate) >= DbFunctions.TruncateTime(d1) && DbFunctions.TruncateTime(m.createDate) <= DbFunctions.TruncateTime(d2) && m.contractor.status == true).ToList();
            }
            else if (activeType == "InActive")
            {
                excel = generalInterface.SearchGeneralExcel(m => DbFunctions.TruncateTime(m.createDate) >= DbFunctions.TruncateTime(d1) && DbFunctions.TruncateTime(m.createDate) <= DbFunctions.TruncateTime(d2) && m.contractor.status == false).ToList();
            }
            else 
            {
               excel = generalInterface.SearchGeneralExcel(m => DbFunctions.TruncateTime(m.createDate) >= DbFunctions.TruncateTime(d1) && DbFunctions.TruncateTime(m.createDate) <= DbFunctions.TruncateTime(d2)).ToList();
            }



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
        [AllowAnonymous]
        public ActionResult PrintInvoice()
        {
            return View(_transPrint);
        }
        public ActionResult PrintTransaction(string id)
        {
            _transPrint = new Transaction();
            _transPrint = generalInterface.SearchForPdf(x => x.id == id).FirstOrDefault();
            var report = new ActionAsPdf("PrintInvoice");
            return report;
            //return View("PrintInvoice", _transPrint);
        }

        public JsonResult GetContractors(string isActive)
        {
            dynamic contractors;
            if (isActive=="Active")
            {
                 contractors = generalInterface.ContractorsList(x => x.status == true);
            }
          else  if (isActive == "InActive")
            {
                 contractors = generalInterface.ContractorsList(x => x.status == false);
            }
            else
            {
                contractors = generalInterface.ContractorsList(x => true);
            }
            return Json(contractors, JsonRequestBehavior.AllowGet);

        }

        public ActionResult SearchByScheme(int? page, string fromDate, string toDate, string schemeId)
        {
            ViewBag.SchemeId = new SelectList(dbContext.Schemes, "Id", "AccountNo");
            if ((String.IsNullOrEmpty(fromDate) || String.IsNullOrEmpty(toDate)))
            {

                ViewBag.DateMsg = "Please Select StartDate And EndDate";

                return View();
            }
            else
            {
                DateTime dt = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                DateTime dt2 = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string g1 = Convert.ToDateTime(dt).ToString("yyyy-MM-dd HH:mm:ss.fff");
                string g2 = Convert.ToDateTime(dt2).ToString("yyyy-MM-dd HH:mm:ss.fff");
                DateTime d1 = DateTime.ParseExact(g1, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                DateTime d2 = DateTime.ParseExact(g2, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);


                
                ViewBag.startDate = fromDate;
                ViewBag.endDate = toDate;
                ViewBag.scheme = schemeId;

                int pageIndex = 1;


                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

                ViewBag.transList = generalInterface.SearchIndividual(m => m.SchemeWork.SchemeId == schemeId && DbFunctions.TruncateTime(m.createDate) >= DbFunctions.TruncateTime(d1) && DbFunctions.TruncateTime(m.createDate) <= DbFunctions.TruncateTime(d2), pageIndex);
                if(ViewBag.transList!=null)
                {
                    ViewBag.status = "excel";
                }
                return View();
            }
        }


        public ActionResult ExportToExcelByScheme(string fromDate, string toDate, string schemeId)
        {
            DateTime dt = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime dt2 = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            string g1 = Convert.ToDateTime(dt).ToString("yyyy-MM-dd HH:mm:ss.fff");
            string g2 = Convert.ToDateTime(dt2).ToString("yyyy-MM-dd HH:mm:ss.fff");
            DateTime d1 = DateTime.ParseExact(g1, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

            DateTime d2 = DateTime.ParseExact(g2, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

            //////new changes//////
            ///
            List<Transaction> excel = new List<Transaction>();
             excel = generalInterface.SearchForPdf(m => DbFunctions.TruncateTime(m.createDate) >= DbFunctions.TruncateTime(d1) && DbFunctions.TruncateTime(m.createDate) <= DbFunctions.TruncateTime(d2) && m.SchemeWork.SchemeId == schemeId).ToList();
           
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
        public JsonResult GetTransaction(string id)
        {
            dynamic transaction = dbContext.Transaction.Where(x => x.id == id).Select(y => new
            {
                id=y.id,
                contractor = y.contractor.name,
                tvNo=y.TreasuryVoucherNo,
                work=y.SchemeWork.Work.Title,
                gstin = y.contractor.GSTIN,
                amountPaid = (y.amountPaid),
                sgstAmount = (y.sgstAmount),
                cgstAmount = (y.cgstAmount),
                itAmount = (y.itAmount),
                labourCessAmount = (y.labourCessAmount),
                deposit = (y.deposit),
                netAmount = (y.netAmount),
                scheme = y.SchemeWork.Scheme.AccountNo,
                createDate = (y.createDate).ToString()


            }).FirstOrDefault();
            return Json(transaction, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetWorks(string schemeId)
        {
          dynamic works=  dbContext.SchemeWorks.Where(x => x.SchemeId == schemeId).Select(y => new
            {
                id = y.Id,
                title = y.Work.Title
            });
            return Json(works, JsonRequestBehavior.AllowGet);
        }
    }
}