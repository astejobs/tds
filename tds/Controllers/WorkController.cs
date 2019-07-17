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
    public class WorkController : Controller
    {
        ApplicationDbContext dbContext;
        GeneralInterface<Work> generaInterface;

        public WorkController()
        {
            dbContext = new ApplicationDbContext();
            generaInterface = new GeneralRepoImpl<Work>();
        }





        [Route("work/")]
        public ActionResult Index(int? page, string id)
        {

            EntityViewModel<Work> works = new EntityViewModel<Work>();
            if (id == null)
            {
                works.entity = null;
                TempData["actionStatus"] = "Post";
            }
            else
            {
                Work work = generaInterface.Find(id);
                works.entity = work;
                TempData["actionStatus"] = "Put";
            }

            int pageIndex = 1;
            ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<Work> workList = dbContext.Works.Include("SchemeWorks").OrderBy(x => x.SchemeWorks.Count()).ToPagedList(pageIndex, 10);
            works.entityList = workList;
            return View("Index", works);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("work/")]
        public ActionResult Post(EntityViewModel<Work> work)
        {
            if (ModelState.IsValid)
            {
                ApplicationDbContext db = new ApplicationDbContext();
                if (db.Works.Any(x => x.Title == work.entity.Title))
                {
                    TempData["MsgFail"] = work.entity.Title + " already exists";
                }

                else if (generaInterface.Save(work.entity))
                {

                    TempData["MsgSuccess"] = "Work has been Saved Successfully";
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
        [Route("Work/update")]
        public ActionResult Put(EntityViewModel<Work> work)
        {
            if (ModelState.IsValid)
            {
                ApplicationDbContext db = new ApplicationDbContext();
                if (db.Works.Any(x => x.Title == work.entity.Title && x.Id != work.entity.Id))
                {
                    TempData["MsgFail"] =  work.entity.Title + " already exists";
                }
                else
                {
                    generaInterface.Update(work.entity);
                    TempData["MsgSuccess"] = "Work has been Updated Successfully";
                }
            }
            else
            {
                TempData["ModelState"] = ModelState;
                TempData["MsgFail"] = "Updation Failed,Enter Valid data";
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id)
        {
            Work work = dbContext.Works.Find(id);
            dbContext.Entry(work).State = System.Data.Entity.EntityState.Deleted;
            dbContext.SaveChanges();
            TempData["MsgSuccess"] = "Work Deleted Successfully";
            return RedirectToAction("index");
        }
    }
}