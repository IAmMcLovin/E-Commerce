using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eCommerce.Models
{
    public class Product
    {
        public int ProductID { get; set; }

        [Display(Name = "Category")]
        public int CategoryID { get; set; }
        [Display(Name = "Category")]
        public virtual Category Category { get; set; }

        [Display(Name = "ProductSKU")]
        public string ProductSKU { get; set; }

        [Display(Name = "Product")]
        public string Name { get; set; }

        public string Description { get; set; }
        [DataType(DataType.Currency)]
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }

        [Display(Name = "Date Added")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [DataType(DataType.ImageUrl)]
        public string ImagePath { get; set; }

        public bool Status { get; set; }

    }
}