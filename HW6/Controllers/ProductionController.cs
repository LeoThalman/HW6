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
        /// <summary>
        /// instantiate view model and add Db.
        /// </summary>
        private AdventureWorksViewModel ViewModel = new AdventureWorksViewModel()
        {
            Db = new ProductionContext()
        };


        /// <summary>
        /// Load Home page and set title and message, as well as product categories
        /// and sub categories
        /// </summary>
        /// <returns>Returns home view with ViewModel information</returns>
        public ActionResult Index()
        {
            ViewBag.Title = "Adventure Works";
            ViewBag.Message = "Welcome to the Adventure Works website. You can use the navigation bar above you view different items.";
            ViewModel.ItemCat = ViewModel.Db.ProductCategories.ToList();
            ViewModel.ItemSubCat = ViewModel.Db.ProductSubcategories.ToList();
            return View(ViewModel);
        }

        /// <summary>
        /// Load informtaion for product subcategory based on SubId and page number.
        /// If one of the values is null redirect to index, or if page number is too high
        /// redirect to index.
        /// </summary>
        /// <param name="SubId">ID of Product subcategory, to know which items to load</param>
        /// <param name="PageNumber">Page number to know where in list to load items from</param>
        /// <returns></returns>
        public ActionResult ProductList(int? SubId, int? PageNumber)
        {
            ViewBag.Title = "Product Page";
            ViewModel.ItemCat = ViewModel.Db.ProductCategories.ToList();
            ViewModel.ItemSubCat = ViewModel.Db.ProductSubcategories.ToList();
            //check that both values are valid, if not go back to index
            if (SubId.HasValue == false || PageNumber.HasValue == false)
            {
                return RedirectToAction("Index");
            }

            //instantiate count, page number, and page title.
            int Id = (int)SubId;
            int num = (int)PageNumber;
            int count = ViewModel.Db.Products.Where(p => Id == p.ProductSubcategoryID).Count();
            ViewBag.Title = ViewModel.Db.ProductSubcategories
                                .Where(p => Id == p.ProductSubcategoryID)
                                .Select(p => p.Name)
                                .FirstOrDefault();

            //divide count by 6 to get total number of pages, then find
            //last, previous and next
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
            //if current is higher than last go back to index, otherwise find the list of products
            if (num <= last)
            {
                //Item list finds all item of certain subcategory and orders by product ID
                //then skips a certain number based on page number and takes the next 6
                //it is converted to an enumerable so it can be used to find the photos.
                ViewModel.ItemList = ViewModel.Db.Products
                                                .Where(p => Id == p.ProductSubcategoryID)
                                                .OrderBy(p => p.ProductID)
                                                .Skip(skip)
                                                .Take(6)
                                                .AsEnumerable();
                //get item costs and enter them into a dictionary by item ID
                ViewModel.ItemCost = new Dictionary<int, decimal>();
                foreach (var i in ViewModel.ItemList)
                {
                    ViewModel.ItemCost.Add(i.ProductID, decimal.Round(i.ListPrice, 2));                    
                }
                //first join item list with the product productphotos table to get item ID and photoID
                IEnumerable<ProductPhoto> Photos = ViewModel.Db.ProductPhotoes.AsEnumerable();
                var PhotoIDs = ViewModel.Db.ProductProductPhotoes.Join(ViewModel.ItemList,
                                                            item => item.ProductID,
                                                            id => id.ProductID, (id, item) =>
                                                            new { ProductPhotoID = id.ProductPhotoID, ProductID = id.ProductID }).AsEnumerable();
                //take the previous list of photoIDs and join it with productPhotos table
                //to find the byte array for thumbnail photos and create a new table with that
                //and product ID and thumbnail byte array
                var tempPhotos = Photos.Join(PhotoIDs,
                                            id => id.ProductPhotoID,
                                            photo => photo.ProductPhotoID,
                                            (photo, id) => new {
                                                ProductPhotoID = photo.ProductPhotoID,
                                                ThumbNailPhoto = photo.ThumbNailPhoto,
                                                ProductID = id.ProductID
                                            })
                                            .OrderBy(p => p.ProductID).ToList();
                //create a dictionary of thumbnails with productID as the key
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

        /// <summary>
        /// Load information of product based on ID passed in from
        /// if product ID is invalid loads error page.
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        public ActionResult ProductPage(int? PID)
        {
            try
            {
                int Id = (int)PID;
                ViewModel.ItemCat = ViewModel.Db.ProductCategories.ToList();
                ViewModel.ItemSubCat = ViewModel.Db.ProductSubcategories.ToList();
                ViewModel.PItem = ViewModel.Db.Products.Where(p => p.ProductID == PID).First();
                if (ViewModel.PItem == null)
                {

                }
                ViewModel.ItemCost = new Dictionary<int, decimal>
                {
                    { ViewModel.PItem.ProductID, decimal.Round(ViewModel.PItem.ListPrice, 2) }
                };

                //grab the product description based on the productModelID
                var PMID = ViewModel.PItem.ProductModelID;
                ProductModelProductDescriptionCulture DesID = ViewModel.Db.ProductModelProductDescriptionCultures.Where(d => d.ProductModelID == PMID).FirstOrDefault();
                ViewModel.Desc = ViewModel.Db.ProductDescriptions.Where(d => d.ProductDescriptionID == DesID.ProductDescriptionID).FirstOrDefault();

                //grab the large photo of the item based on product photo ID, same process as
                //find the thumbnails for the productlist method
                ProductProductPhoto PhotoID = ViewModel.Db.ProductProductPhotoes
                                                        .Where(p => p.ProductID == Id)
                                                        .FirstOrDefault();
                ProductPhoto tPhoto = ViewModel.Db.ProductPhotoes
                                                .Where(p => p.ProductPhotoID == PhotoID.ProductPhotoID)
                                                .FirstOrDefault();
                ViewModel.ItemReviews = ViewModel.Db.ProductReviews.Where(r => r.ProductID == Id);
                ViewModel.ItemLargePhoto = (tPhoto.LargePhoto);
                //return view of the product page
                return View(ViewModel);
            }
            catch (Exception e)
            {
            }
            //redirects to error page if productID was not valid.
            return RedirectToAction("ErrorPage", new { message = "Not a valid Product, please try another Product" });

        }

        /// <summary>
        /// Loads a page with an error that is loaded when a product SubId or Product ID
        /// that is invalid is passed to method.
        /// </summary>
        /// <param name="message">Error Message</param>
        /// <returns> View with a error message</returns>
        public ActionResult ErrorPage(string message)
        {
            ViewBag.Message = message;
            ViewModel.ItemCat = ViewModel.Db.ProductCategories.ToList();
            ViewModel.ItemSubCat = ViewModel.Db.ProductSubcategories.ToList();
            return View(ViewModel);
        }

        /// <summary>
        /// Loads page to add a review for an item, if an invalid ID
        /// is passed loads an error page
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddReview(int? PID)
        {
            try
            {
                ViewModel.ItemCat = ViewModel.Db.ProductCategories.ToList();
                ViewModel.ItemSubCat = ViewModel.Db.ProductSubcategories.ToList();
                ViewBag.Title = "Add Review for " + ViewModel.Db.Products.Where(p => p.ProductID == PID)
                                                               .Select(p => p.Name)
                                                               .FirstOrDefault();
                return View(ViewModel);
            }
            catch (Exception e)
            {
            }
            return RedirectToAction("ErrorPage", new { message = "Not a valid Product, please try another Product" });
        }

        /// <summary>
        /// adds review to review table and redirects user to to a thank you apge
        /// </summary>
        /// <param name="PID">ID of product being reviewed</param>
        /// <param name="review">information of the product to add to review table</param>
        /// <returns>The tankyou page if information was entered correctly, otherwise the  review page</returns>
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

        /// <summary>
        /// Thank you page loaded after an item has been reviewed
        /// </summary>
        /// <param name="PID">ID of product that was reviewed</param>
        /// <param name="Name">Name of the reviewer</param>
        /// <returns>The thank you page with a message to the reviewer</returns>
        public ActionResult ThankYou(int PID, string Name)
        {
            ViewModel.ItemCat = ViewModel.Db.ProductCategories.ToList();
            ViewModel.ItemSubCat = ViewModel.Db.ProductSubcategories.ToList();
            ViewBag.Message = "Thank you " + Name + " for your review of " + ViewModel.Db.Products
                                                                          .Where(p => p.ProductID == PID)
                                                                          .Select(p=> p.Name).FirstOrDefault();
            return View(ViewModel);
        }
    }
}