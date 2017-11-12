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
        private AdventureWorksViewModel Menu = new AdventureWorksViewModel();
        // GET: Production
        public ActionResult Index()
        {

            Menu.ItemCat = db.ProductCategories.ToList();
            Menu.ItemSubCat = db.ProductSubcategories.ToList();
            ViewBag.Title = "Home";
            return View(Menu);
        }
        public ActionResult ProductList()
        {
            return View();
        }
    }
}