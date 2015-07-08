


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
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.IO;


namespace WebApp.Controllers
{
    public class ProductsController1 : Controller
    {
        //private StoreContext db = new StoreContext();
        private readonly IProductService  _productService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public ProductsController1 (IProductService  productService, IUnitOfWorkAsync unitOfWork)
        {
            _productService  = productService;
            _unitOfWork = unitOfWork;
        }

        // GET: Products/Index
        public ActionResult Index()
        {

            PopulateForeignKey();
             return View();
        }

        #region operate for kend-ui grid

        private void PopulateForeignKey()
        {
            var categories = _unitOfWork.Repository<Category>();

            ViewBag.Categories = categories.Queryable();
            ViewBag.DefaultCategoryId = categories.Queryable().First().Id;     
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            
            DataSourceResult result = _productService.Queryable().ToDataSourceResult(request);
            return Json(result);
        
        
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, Product product)
        {
            if (product != null && ModelState.IsValid)
            {
                _productService.Insert(product);
                _unitOfWork.SaveChanges();
            }

            return Json(new[] { product }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request,Product product)
        {
            if (product != null && ModelState.IsValid)
            {
                if (product.Category != null)
                {
                    product.CategoryId = product.Category.Id;
                    //product.Category = null;
                }
                _productService.Update(product);
                _unitOfWork.SaveChanges();
            }


            return Json(new[] { product }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete([DataSourceRequest] DataSourceRequest request,Product product)
        {
            if (product != null)
            {
                _productService.Delete(product);
                _unitOfWork.SaveChanges();
            }

            return Json(new[] { product }.ToDataSourceResult(request, ModelState));
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
                //_unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
