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
{
    [Authorize(Roles ="Admin")]
    public class SchemeController : Controller
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();
        GeneralInterface<Scheme> generaInterface;

        public SchemeController()
        {
            generaInterface = new GeneralRepoImpl<Scheme>();
        }





        [Route("scheme/")]
        public ActionResult Index(int? page, string id)
        {

            EntityViewModel<Scheme> schemes = new EntityViewModel<Scheme>();
            if (id == null)
            {
                schemes.entity = null;
                TempData["actionStatus"] = "Post";
            }
            else
            {
                Scheme scheme = generaInterface.Find(id);
                schemes.entity = scheme;
                TempData["actionStatus"] = "Put";
            }

            int pageIndex = 1;
            ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<Scheme> schemeList = generaInterface.pagedList(pageIndex, id);
            schemes.entityList = schemeList;
            return View("Index", schemes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("scheme/")]
        public ActionResult Post(EntityViewModel<Scheme> scheme)
        {
            if (ModelState.IsValid)
            {
                if (generaInterface.Save(scheme.entity))
                {

                    TempData["MsgSuccess"] = "Scheme has been Saved Successfully";
                }
                else
                {

                    TempData["MsgFail"] = "Enter valid data";
                }
            }
            else
            {
                TempData["ModelState"] = ModelState;
                TempData["MsgFail"] = "Enter valid data";
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Scheme/update")]
        public ActionResult Put(EntityViewModel<Scheme> scheme)
        {
            if (ModelState.IsValid)
            {
                generaInterface.Update(scheme.entity);
                TempData["MsgSuccess"] = "Shceme has been Updated Successfully";
            }
            else
            {
                TempData["ModelState"] = ModelState;
                TempData["MsgFail"] = "Updation Failed,Enter Valid data";
            }
            return RedirectToAction("Index");
        }


       
    }
}