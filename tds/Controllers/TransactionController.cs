using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
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
        public ActionResult Get()
        {
            string id = null;
            ViewBag.contractors = contractorInterface.listAll(id);
            ViewBag.taxes = taxInterface.listAll(id).OrderBy(m => m.rate).ToList();
            return View("transaction");
        }

        [HttpPost]
        [Route("transaction/")]
        [ValidateAntiForgeryToken]
        public ActionResult Post(Transaction transaction)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                transaction.sgstAmount = (dbContext.Tax.Find(transaction.sgstId).rate / 100 * transaction.amountPaid);
                transaction.cgstAmount = (dbContext.Tax.Find(transaction.cgstId).rate / 100 * transaction.amountPaid);
                transaction.itAmount = (dbContext.Tax.Find(transaction.incometaxtId).rate / 100 * transaction.amountPaid);
              
                generalInterface.Save(transaction);
                TempData["MsgSuccess"] = "true";
            }
            else
            {
                TempData["MsgFail"] = "true";
                return RedirectToAction("Get");
            }
            return RedirectToAction("Get");
        }

    }
}