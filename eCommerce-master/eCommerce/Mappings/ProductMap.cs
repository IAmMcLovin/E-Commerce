using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CsvHelper.Configuration;
using eCommerce.Models;

namespace eCommerce.Mappings
{
    public class ProductMap : CsvClassMap<Product>
    {
        public override void CreateMap()
        {
            Map(m => m.ProductSKU).Name("SKU");
            Map(m => m.CategoryID).Name("CategoryID");
            Map(m => m.Name).Name("Name");
            Map(m => m.Description).Name("Description");
            Map(m => m.UnitPrice).Name("Price");
            Map(m => m.Quantity).Name("Quantity");
        }
    }
}