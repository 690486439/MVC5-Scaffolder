


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
