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
    //class selItem
    //{
    //    public int Id { get; set; }
    //    public string Value { get; set; }
    //}
    [Authorize(Roles = "Admin")]
    public class SchemeController : Controller
    {
        ApplicationDbContext dbContext;
        GeneralInterface<Scheme> generaInterface;

        public SchemeController()
        {
            dbContext = new ApplicationDbContext();
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
            //List<selItem> sel = new List<selItem>();
            //for(int i=0;i<50;i++)
            //{
            //    selItem sel1 = new selItem
            //    {
            //        Id = i,
            //        Value = "Apple is a fruit Apple is a fruit. I love apple . aplle. I love apple . aplle" + i
            //    };
            //sel.Add(sel1);
            //}

            ViewBag.Works = dbContext.Works;
           
            int pageIndex = 1;
            ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<Scheme> schemeList = dbContext.Schemes.Include("SchemeWorks").OrderBy(x => x.SchemeWorks.Count()).ToPagedList(pageIndex, 10);
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
                ApplicationDbContext db = new ApplicationDbContext();
                if (db.Schemes.Any(x => x.AccountNo == scheme.entity.AccountNo))
                {
                    TempData["MsgFail"] = "Scheme with Account No " + scheme.entity.AccountNo + " already exists";
                }

                else if (generaInterface.Save(scheme.entity))
                {

                    TempData["MsgSuccess"] = "Scheme has been Saved Successfully";
                    List<SchemeWork> schemeWorks = new List<SchemeWork>();
                    foreach(string workId in scheme.entity.Works)
                    {
                        SchemeWork schemeWork = new SchemeWork
                        {
                            WorkId = workId,
                            SchemeId = scheme.entity.Id

                        };
                        schemeWorks.Add(schemeWork);
                    }
                    dbContext.SchemeWorks.AddRange(schemeWorks);
                    dbContext.SaveChanges();
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
                ApplicationDbContext db = new ApplicationDbContext();
                if (db.Schemes.Any(x => x.AccountNo == scheme.entity.AccountNo && x.Id != scheme.entity.Id))
                {
                    TempData["MsgFail"] = "Scheme with Account No " + scheme.entity.AccountNo + " already exists";
                }
                else
                {
                    generaInterface.Update(scheme.entity);
                    TempData["MsgSuccess"] = "Shceme has been Updated Successfully";
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
            Scheme scheme = dbContext.Schemes.Find(id);
            dbContext.Entry(scheme).State = System.Data.Entity.EntityState.Deleted;
            dbContext.SaveChanges();
            TempData["MsgSuccess"] = "Scheme Deleted Successfully";
            return RedirectToAction("index");
        }
      public JsonResult  GetWorksByScheme(string schemeId)
        {
            
          dynamic works=  dbContext.Works.Where(x => x.SchemeWorks.Any(y => y.SchemeId == schemeId) == false).Select(z=>new {

                id=z.Id,
                title=z.Title


            }).ToList();
            return Json(works, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AddSchemeWork(SchemeWorkVM schemeWorkVM)
        {
            List<SchemeWork> schemeWorks = new List<SchemeWork>();
            foreach (string workId in schemeWorkVM.Works)
            {
                SchemeWork schemeWork = new SchemeWork
                {
                    WorkId = workId,
                    SchemeId = schemeWorkVM.SchemeId

                };
                schemeWorks.Add(schemeWork);
            }
            dbContext.SchemeWorks.AddRange(schemeWorks);
            dbContext.SaveChanges();
            TempData["MsgSuccess"] = "Works has been Added Successfully to Scheme";
            return RedirectToAction("index");
        }
    }
}