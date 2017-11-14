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
        private AdventureWorksViewModel ViewModel = new AdventureWorksViewModel()
        {
            Db = new ProductionContext()
        };


        // GET: Production
        public ActionResult Index()
        {
            ViewModel.ItemCat = ViewModel.Db.ProductCategories.ToList();
            ViewModel.ItemSubCat = ViewModel.Db.ProductSubcategories.ToList();
            return View(ViewModel);
        }


        public ActionResult ProductList(int? SubId, int? PageNumber)
        {
            ViewModel.ItemCat = ViewModel.Db.ProductCategories.ToList();
            ViewModel.ItemSubCat = ViewModel.Db.ProductSubcategories.ToList();
            if (SubId.HasValue == false || PageNumber.HasValue == false)
            {
                return RedirectToAction("Index");
            }

            try
            {
                int Id = (int)SubId;
                int num = (int)PageNumber;
                int count = ViewModel.Db.Products.Where(p => Id == p.ProductSubcategoryID).Count();
                ViewBag.Title = ViewModel.Db.ProductSubcategories
                                   .Where(p => Id == p.ProductSubcategoryID)
                                   .Select(p => p.Name)
                                   .FirstOrDefault();

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
                if (num <= last)
                {
                    ViewModel.ItemList = ViewModel.Db.Products
                                                  .Where(p => Id == p.ProductSubcategoryID)
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
                ViewModel.ItemCat = ViewModel.Db.ProductCategories.ToList();
                ViewModel.ItemSubCat = ViewModel.Db.ProductSubcategories.ToList();
                ViewModel.PItem = ViewModel.Db.Products.Where(p => p.ProductID == PID).FirstOrDefault();
                return View(ViewModel);

        }


        [HttpGet]
        public ActionResult AddReview(int? PID)
        {
            ViewModel.ItemCat = ViewModel.Db.ProductCategories.ToList();
            ViewModel.ItemSubCat = ViewModel.Db.ProductSubcategories.ToList();
            ViewBag.Title = "Add Review for " + ViewModel.Db.Products.Where(p => p.ProductID == PID)
                                                           .Select(p => p.Name)
                                                           .FirstOrDefault();
            return View(ViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReview(int? PID, [Bind(Include = "ProductReviewID,ProductID, ReviewerName,ReviewDate," +
                                      "EmailAddress,Rating,Comments,ModifiedDate")] ProductReview review)
        {
            ViewModel.ItemCat = ViewModel.Db.ProductCategories.ToList();
            ViewModel.ItemSubCat = ViewModel.Db.ProductSubcategories.ToList();
            ViewBag.Title = "Add Review for " + ViewModel.Db.Products.Where(p => p.ProductID == PID)
                                               .Select(p => p.Name)
                                               .FirstOrDefault();
            if(ModelState.IsValid)
            { 
                int Id = (int)PID;
                review.ProductID = Id;
                review.ReviewDate = DateTime.Now;
                review.ModifiedDate = DateTime.Now;
                ViewModel.Db.ProductReviews.Add(review);
                ViewModel.Db.SaveChanges();
                return RedirectToAction("ThankYou", new { PID = Id, Name = review.ReviewerName });
            }

            return View(ViewModel);

        }

        public ActionResult ThankYou(int PID, string Name)
        {
            ViewModel.ItemCat = ViewModel.Db.ProductCategories.ToList();
            ViewModel.ItemSubCat = ViewModel.Db.ProductSubcategories.ToList();
            ViewBag.Message = "Thank you " + Name + "for your review of " + ViewModel.Db.Products
                                                                          .Where(p => p.ProductID == PID)
                                                                          .Select(p=> p.Name).FirstOrDefault();
            return View(ViewModel);
        }
    }
}