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
{
    public class DeductorController : Controller
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();
        GeneralInterface<Deductor> deductorInterface;

        public DeductorController()
        {
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

        [Route("deductor/")]
        public ActionResult Get()
        {
            ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);

            var user = UserManager.FindById(User.Identity.GetUserId());
            Deductor deductor = deductorInterface.Find(user.Id);      
            return View("deductor",deductor);
        }

        [HttpPost]
        [Route("deductor/")]
        [ValidateAntiForgeryToken]
        public ActionResult Put(Deductor deductor)
        {
            if (ModelState.IsValid) {
                if(deductorInterface.Update(deductor))
                    TempData["MsgSuccess"] = "true";
                else
                    TempData["MsgFail"] = "true";
            }
            else
            {
                TempData["ModelState"] = ModelState;
                TempData["MsgFail"] = "true";   
            } 

            return RedirectToAction("Get");
        }
    }
}