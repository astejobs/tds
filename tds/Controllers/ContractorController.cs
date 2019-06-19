using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tds.Models;
using tds.RepositoryImpl;
using tds.RepositoryInterface;

namespace tds.Controllers
{[Authorize]
    public class ContractorController : Controller
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();
        GeneralInterface<Contractor> generalInterface;

        public ContractorController() {

            generalInterface = new GeneralRepoImpl<Contractor>();

        }
        public ActionResult Index()
        {
            return View();
        }

        [Route("contractor/")]
        public ActionResult Get(int? page,string id) 
        {
            EntityViewModel<Contractor> contractor = new EntityViewModel<Contractor>();
           
            if (id == null)
            {
               contractor.entity =null;
                TempData["actionStatus"] = "Post";
            }
            else
            {
                Contractor cont = generalInterface.Find(id);
                contractor.entity = cont;
                TempData["actionStatus"] = "Put";              
            }
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<Contractor> conList = generalInterface.pagedList(pageIndex, id);     
            contractor.entityList = conList;
            
            ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);
            return View("contractor",contractor);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("contractor/")]        
        public ActionResult Post(EntityViewModel<Contractor> contractor)
        {
            

            if (ModelState.IsValid)
            {
                if (!generalInterface.checkAlreadyExists(m => (m.GSTIN == contractor.entity.GSTIN || m.regNo == contractor.entity.regNo)))
                {
                    if (generalInterface.Save(contractor.entity))
                    {
                        TempData["MsgSuccess"] = "Contractor has been Saved Successfully";
                    }
                    else
                    {
                        TempData["MsgFail"] = "Enter valid data";
                    }
                }
                else {
                    TempData["MsgFail"] = "GSTIN/Registration No. Already Exists";
                }
            }
            else
            {
                TempData["ModelState"] = ModelState;
                TempData["MsgFail"] = "Enter valid data";
            }
            return RedirectToAction("Get");
                 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("contractor/update")]
        public ActionResult Put(EntityViewModel<Contractor> contractor)
        {
            if (ModelState.IsValid)
            {
                if (!generalInterface.checkAlreadyExists(m => (m.GSTIN == contractor.entity.GSTIN || m.regNo == contractor.entity.regNo) && m.id != contractor.entity.id))
                {
                    generalInterface.Update(contractor.entity);
                    TempData["MsgSuccess"] = "Contractor has been Updated Successfully";
                }
                else {
                    TempData["MsgFail"] = "GSTIN/Registration No. Already Exists";
                }
            }
            else
            {
                TempData["ModelState"] = ModelState;
                TempData["MsgFail"] = "Updation Failed,Enter Valid data";
            }
            return RedirectToAction("Get", new { id = contractor.entity.id});
        }

        
    }
}