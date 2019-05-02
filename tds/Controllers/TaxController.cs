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
    public class TaxController : Controller 
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();
        GeneralInterface<Tax> generaInterface ;

        public TaxController() {
            generaInterface = new GeneralRepoImpl<Tax>();
        }



       

        [Route("tax/")]
        public ActionResult Get(int? page,string id)
        {
           
            EntityViewModel<Tax> tax = new EntityViewModel<Tax>();
            if (id == null)
            {
               tax.entity =null;
                TempData["actionStatus"] = "Post";
            }
            else
            {
                Tax taxw = generaInterface.Find(id);
                tax.entity = taxw;
                TempData["actionStatus"] = "Put";
            }

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<Tax> taxList= generaInterface.pagedList(pageIndex, id);
            tax.entityList = taxList;
            return View("tax",tax);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("tax/")]
        public ActionResult Post(EntityViewModel<Tax> tax)
        {
            if (ModelState.IsValid)
            {
                if (generaInterface.Save(tax.entity))
                {

                    TempData["MsgSuccess"] = "Tax has been Saved Successfully";
                }
                else { TempData["MsgFail"] = "Enter valid data"; }
            }else{
                TempData["MsgFail"] = "Enter valid data";
            }
            return RedirectToAction("Get");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("tax/update")]
        public ActionResult Put(EntityViewModel<Tax> tax)
        {
            if (ModelState.IsValid)
            {
                generaInterface.Update(tax.entity);
                TempData["MsgSuccess"] = "Tax has been Updated Successfully";
            }
            else
            {
                TempData["MsgFail"] = "Updation Failed,Enter Valid data";
            }
            return RedirectToAction("Get");
        }


        [HttpGet]
        [Route("tax/delete/")]
        public ActionResult Delete(string id)
        {
            

            if (id == null)
            {
                HttpNotFound();
                TempData["MsgFail"] = "Deletion Failed";
            }
            else
            {
                if (generaInterface.Delete(id)){
                    TempData["MsgSuccess"] = "Record Deleted Successfully";
                }
                else {
                    TempData["MsgSuccess"] = "Deletion Failed";
                }
            }
            return RedirectToAction("Get");
        }
    }
}