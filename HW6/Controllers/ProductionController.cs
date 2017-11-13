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
        public ActionResult ProductList(int? SubId, int? PageNumber)
        {
            if(SubId.HasValue == false || PageNumber.HasValue == false)
            {
                return RedirectToAction("Index");
            }

            try
            {
                int Id = (int)SubId;
                int num = (int)PageNumber;
                int count = db.Products.Where(p => Id == p.ProductSubcategoryID).Count();
                decimal temp = count;
                temp = temp / 6;
                temp = Math.Ceiling(temp);
                int last = (int)temp;
                int skip = num - 1;
                ViewBag.Prev = PageNumber -1;
                ViewBag.Next = PageNumber + 1;
                ViewBag.Last = last;
                ViewBag.SubId = Id;
                skip = skip * 6;
                ViewModel.ItemCat = db.ProductCategories.ToList();
                ViewModel.ItemSubCat = db.ProductSubcategories.ToList();
                if (num <= last)
                {
                    ViewModel.ItemList = db.Products.Where(p => Id == p.ProductSubcategoryID)
                                                  .OrderBy(p => p.ProductID)
                                                  .Skip(skip)
                                                  .Take(6)
                                                  .ToList();
                    return View(ViewModel);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch(Exception E)
            {
                Console.WriteLine(E);
            }

            return RedirectToAction("Index");
        }

        public ActionResult ProductPage(int? PID)
        {
                ViewModel.ItemCat = db.ProductCategories.ToList();
                ViewModel.ItemSubCat = db.ProductSubcategories.ToList();
                ViewModel.PItem = db.Products.Where(p => p.ProductID == PID).FirstOrDefault();
                return View(ViewModel);

        }
    }
}