


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Repository.Pattern.UnitOfWork;
using Repository.Pattern.Infrastructure;
using WebApp.Models;
using WebApp.Services;
using WebApp.Repositories;
using WebApp.Extensions;
using PagedList;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace WebApp.Controllers
{
    public class CategoriesController : Controller
    {
        //private StoreContext db = new StoreContext();
        private readonly ICategoryService  _categoryService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public CategoriesController (ICategoryService  categoryService, IUnitOfWorkAsync unitOfWork)
        {
            _categoryService  = categoryService;
            _unitOfWork = unitOfWork;
        }

        // GET: Categories/Index
        public ActionResult Index( )
        {

            return View();
          
        }
        #region operate for kend-ui grid
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            DataSourceResult result = _categoryService.Queryable().ToDataSourceResult(request, c => new
            {
                Id = c.Id,
                Name = c.Name
               
            });
            //DataSourceResult result = students.ToDataSourceResult(request);
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, Category category)
        {
            if (category != null && ModelState.IsValid)
            {
                _categoryService.Insert(category);
                _unitOfWork.SaveChanges();
            }

            return Json(new[] { category }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, Category category)
        {
            if (category != null && ModelState.IsValid)
            {
                _categoryService.Update(category);
                _unitOfWork.SaveChanges();
            }

            return Json(new[] { category }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete([DataSourceRequest] DataSourceRequest request, Category category)
        {
            if (category != null)
            {
                _categoryService.Delete(category);
                _unitOfWork.SaveChanges();
            }

            return Json(new[] { category }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        #endregion
        // Get :Categories/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult PageList(int offset = 0, int limit = 10, string search = "", string sort = "", string order = "")
        {
            int totalCount = 0;
            int pagenum = offset / limit +1;
                        var category  = _categoryService.Query(new CategoryQuery().WithAnySearch(search)).OrderBy(n=>n.OrderBy(sort,order)).SelectPage(pagenum, limit, out totalCount);
                        var rows = category .Select(  n => new {  Id = n.Id , Name = n.Name }).ToList();
            var pagelist = new { total = totalCount, rows = rows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

       
        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = _categoryService.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }
        

        // GET: Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Products,Id,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
             				_categoryService.Insert(category);
                           _unitOfWork.SaveChanges();
                DisplaySuccessMessage("Has append a Category record");
                return RedirectToAction("Index");
            }

            DisplayErrorMessage();
            return View(category);
        }

        // GET: Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = _categoryService.Find(id);

 


            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Products,Id,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                category.ObjectState = ObjectState.Modified;
                				_categoryService.Update(category);
                                
                _unitOfWork.SaveChanges();
                DisplaySuccessMessage("Has update a Category record");
                return RedirectToAction("Index");
            }
            DisplayErrorMessage();
            return View(category);
        }

        // GET: Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = _categoryService.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category =  _categoryService.Find(id);
             _categoryService.Delete(category);
            _unitOfWork.SaveChanges();
            DisplaySuccessMessage("Has delete a Category record");
            return RedirectToAction("Index");
        }


       

 

        private void DisplaySuccessMessage(string msgText)
        {
            TempData["SuccessMessage"] = msgText;
        }

        private void DisplayErrorMessage()
        {
            TempData["ErrorMessage"] = "Save changes was unsuccessful.";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
               
            }
            base.Dispose(disposing);
        }
    }
}
