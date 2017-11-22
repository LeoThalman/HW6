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
            ViewBag.Title = "Adventure Works";
            ViewBag.Message = "Welcome to the Adventure Works website. You can use the navigation bar above you view different items.";
            ViewModel.ItemCat = ViewModel.Db.ProductCategories.ToList();
            ViewModel.ItemSubCat = ViewModel.Db.ProductSubcategories.ToList();
            return View(ViewModel);
        }


        public ActionResult ProductList(int? SubId, int? PageNumber)
        {
            ViewBag.Title = "Product Page";
            ViewModel.ItemCat = ViewModel.Db.ProductCategories.ToList();
            ViewModel.ItemSubCat = ViewModel.Db.ProductSubcategories.ToList();
            if (SubId.HasValue == false || PageNumber.HasValue == false)
            {
                return RedirectToAction("Index");
            }

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
                                                .AsEnumerable();
                ViewModel.ItemCost = new Dictionary<int, decimal>();
                foreach (var i in ViewModel.ItemList)
                {
                    ViewModel.ItemCost.Add(i.ProductID, decimal.Round(i.ListPrice, 2));                    
                }
                IEnumerable<ProductPhoto> Photos = ViewModel.Db.ProductPhotoes.AsEnumerable();
                var PhotoIDs = ViewModel.Db.ProductProductPhotoes.Join(ViewModel.ItemList,
                                                            item => item.ProductID,
                                                            id => id.ProductID, (id, item) =>
                                                            new { ProductPhotoID = id.ProductPhotoID, ProductID = id.ProductID }).AsEnumerable();
                var tempPhotos = Photos.Join(PhotoIDs,
                                            id => id.ProductPhotoID,
                                            photo => photo.ProductPhotoID,
                                            (photo, id) => new {
                                                ProductPhotoID = photo.ProductPhotoID,
                                                ThumbNailPhoto = photo.ThumbNailPhoto,
                                                ThumbNailPhotoFileName = photo.ThumbnailPhotoFileName,
                                                LargePhoto = photo.LargePhoto,
                                                LargePhotoFileName = photo.LargePhotoFileName,
                                                ProductID = id.ProductID
                                            })
                                            .OrderBy(p => p.ProductID).ToList();
                ViewModel.ItemThumbNail = new Dictionary<int, byte[]>();
                foreach (var tPhoto in tempPhotos)
                {
                    ViewModel.ItemThumbNail.Add(tPhoto.ProductID, tPhoto.ThumbNailPhoto);
                }

                ViewModel.ItemList = ViewModel.ItemList.ToList();
                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult ProductPage(int? PID)
        {
                
            int Id = (int)PID;
            ViewModel.ItemCat = ViewModel.Db.ProductCategories.ToList();
            ViewModel.ItemSubCat = ViewModel.Db.ProductSubcategories.ToList();
            ViewModel.PItem = ViewModel.Db.Products.Where(p => p.ProductID == PID).FirstOrDefault();
            ViewModel.ItemCost = new Dictionary<int, decimal>
            {
                { ViewModel.PItem.ProductID, decimal.Round(ViewModel.PItem.ListPrice, 2) }
            };

            var PMID = ViewModel.PItem.ProductModelID;
            ProductModelProductDescriptionCulture DesID = ViewModel.Db.ProductModelProductDescriptionCultures.Where(d => d.ProductModelID == PMID).FirstOrDefault();
            ViewModel.Desc = ViewModel.Db.ProductDescriptions.Where(d=> d.ProductDescriptionID == DesID.ProductDescriptionID).FirstOrDefault();

            ProductProductPhoto PhotoID = ViewModel.Db.ProductProductPhotoes
                                                   .Where(p => p.ProductID == Id)
                                                   .FirstOrDefault();
            ProductPhoto tPhoto = ViewModel.Db.ProductPhotoes
                                           .Where(p => p.ProductPhotoID == PhotoID.ProductPhotoID)
                                           .FirstOrDefault();
            ViewModel.ItemReviews = ViewModel.Db.ProductReviews.Where(r => r.ProductID == Id);
            ViewModel.ItemLargePhoto = (tPhoto.LargePhoto);
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