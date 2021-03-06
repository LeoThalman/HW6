﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HW6.Models;
using HW6.DAL;

namespace HW6.ViewModel
{
    public class AdventureWorksViewModel
    {
        public IEnumerable<ProductReview> ItemReviews { get; set; }
        public ProductReview Review { get; set; }
        public ProductionContext Db { get; set; }
        public ProductDescription Desc { get; set; }
        public Dictionary<int, decimal> ItemCost { get; set; }
        public Dictionary<int, byte[]> ItemThumbNail { get; set; }
        public byte[] ItemLargePhoto {get; set; }
        public IEnumerable<ProductCategory> ItemCat { get; set; }
        public IEnumerable<ProductSubcategory> ItemSubCat { get; set; }
        public IEnumerable<Product> ItemList { get; set; }
        public Product PItem { get; set; }

    }
}