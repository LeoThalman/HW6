using System;
using System.Collections.Generic;
using System.Linq;
using HW6.Models;
using HW6.ViewModel;
using HW6.DAL;
using System.Web;
using System.Web.Mvc;

namespace HW6.Controllers
{

    
    public class ProductionController : Controller
    {
        private ProductionContext db = new ProductionContext();
        private AdventureWorksViewModel ViewModel = new AdventureWorksViewModel();

        // GET: Production
        public ActionResult Index()
        {
            ViewModel.ItemCat = db.ProductCategories.ToList();
            ViewModel.ItemSubCat = db.ProductSubcategories.ToList();
            ViewBag.Title = "Home";
            return View(ViewModel);
        }
        public ActionResult ProductList(int? SubId)
        {
            if(SubId.HasValue == false)
            {
                RedirectToAction("index");
            }
            Console.WriteLine(SubId);
            try
            {
                int Id = (int)SubId;
                ViewModel.ItemCat = db.ProductCategories.ToList();
                ViewModel.ItemSubCat = db.ProductSubcategories.ToList();
                ViewModel.ItemList = db.Products.Where(p => Id == p.ProductSubcategoryID )
                                              .ToList();
                return View(ViewModel);
            }
            catch(Exception E)
            {
                Console.WriteLine(E);
            }

            RedirectToAction("index");
            return View();
        }
    }
}