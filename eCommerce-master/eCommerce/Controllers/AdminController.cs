using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using eCommerce.Models;
using eCommerce.DAL;
using CsvHelper;
using eCommerce.Mappings;


namespace eCommerce.Controllers
{
    public class AdminController : Controller
    {
        CommerceContext db = new CommerceContext();

        // GET: Upload
        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }

        // POST: Upload
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase productFile)
        {
            string path = null;

            if (productFile.ContentLength > 0)
            {
                var filename = Path.GetFileName(productFile.FileName);
                path = AppDomain.CurrentDomain.BaseDirectory + "Upload\\" + filename;
                productFile.SaveAs(path);

                var csv = new CsvReader(new StreamReader(path));
                csv.Configuration.RegisterClassMap<ProductMap>();

                var productList = csv.GetRecords<Product>();

                foreach(var product in productList)
                {
                    product.Date = DateTime.Now;
                    db.Products.Add(product);
                }

                db.SaveChanges();
            }        
            return View();
        }
    }
}