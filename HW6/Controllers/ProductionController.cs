using System;
using System.Collections.Generic;
using System.Linq;
using HW6.Models;
using HW6.DAL;
using System.Web;
using System.Web.Mvc;

namespace HW6.Controllers
{

    
    public class ProductionController : Controller
    {
        private ProductionContext db = new ProductionContext();
        // GET: Production
        public ActionResult Index()
        {
            ViewBag.Title = "Home";
            return View(db.ProductCategories.ToList());
        }
    }
}