using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HW6.Models;

namespace HW6.ViewModel
{
    public class AdventureWorksViewModel
    {
        public IEnumerable<ProductCategory> ItemCat { get; set; }
        public IEnumerable<ProductSubcategory> ItemSubCat { get; set; }
        public IEnumerable<Product> ItemList { get; set; }
        public Product PItem { get; set; }
    }
}