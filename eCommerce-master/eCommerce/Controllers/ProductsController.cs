using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eCommerce.Models;
using eCommerce.DAL;
using System.IO;
using PagedList;
using CsvHelper;
using eCommerce.Mappings;

namespace eCommerce.Controllers
{
    public class ProductsController : Controller
    {
        private CommerceContext db = new CommerceContext();

        // GET: Product
        public ActionResult ViewAll(int? categoryID, string sortOrder, string searchString)
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryId", "Name");

            var products = db.Products.Where(s => s.Status.Equals(true));

            if (!String.IsNullOrEmpty(searchString))
            {
                products = db.Products.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper())
                    || s.Category.Name.ToUpper().Contains(searchString.ToUpper())
                    || s.Description.ToUpper().Contains(searchString.ToUpper()));
            }

            if (categoryID.HasValue)
            {
                products = db.Products.Where(s => s.CategoryID == categoryID);
            }

            switch (sortOrder)
            {
                case "name_asc":
                    products = products.OrderBy(s => s.Name);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(s => s.UnitPrice);
                    break;
                case "price_asc":
                    products = products.OrderBy(s => s.UnitPrice);
                    break;
                default: // Most Recent
                    products = products.OrderByDescending(s => s.Date);
                    break;
            }

            return View(products);
        }

        public ActionResult ViewProduct()
        {
            return View();
        }

        // GET: Products
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            //ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1; 
            }
            else
            { 
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var products = db.Products.Include(p => p.Category);

            if (!String.IsNullOrEmpty(searchString))
            {
                products = db.Products.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper())
                    || s.Category.Name.ToUpper().Contains(searchString.ToUpper())
                    || s.Description.ToUpper().Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(s => s.Name);
                    break;
                //case "Date":
                //    products = products.OrderBy(s => s.EnrollmentDate);
                //    break;
                //case "date_desc":
                //    products = products.OrderByDescending(s => s.EnrollmentDate);
                //    break;
                default: // Name ascending
                    products = products.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1); 
            
            return View(products.ToPagedList(pageNumber, pageSize));
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryId", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase ImagePath, [Bind(Include = "ProductID,CategoryID,Name,Description,UnitPrice,Quantity,ImagePath,Status")] Product product)
        {
             var validImageTypes = new string[]
            {
                "image/gif",
                "image/jpeg",
                "image/pjpeg",
                "image/png"
            };

            if (ImagePath == null || ImagePath.ContentLength == 0)
            {
                ModelState.AddModelError("ImageUpload", "This field is required");
            }
            else if (!validImageTypes.Contains(ImagePath.ContentType))
            {
                ModelState.AddModelError("ImageUpload", "Please choose either a GIF, JPG or PNG image.");
            }

            if (ModelState.IsValid)
            {
                if (ImagePath != null && ImagePath.ContentLength > 0)
                {
                    var uploadDir = "~/Upload";
                    var imagePath = Path.Combine(Server.MapPath(uploadDir), ImagePath.FileName);
                    var imageUrl = Path.Combine(uploadDir, ImagePath.FileName);
                    ImagePath.SaveAs(imagePath);
                    product.ImagePath = ImagePath.FileName;
                }

                product.Date = DateTime.Now;
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryId", "Name", product.CategoryID);
            return View(product);
        }

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

                foreach (var product in productList)
                {
                    product.Date = DateTime.Now;
                    db.Products.Add(product);
                }

                db.SaveChanges();
            }
            return View();
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryId", "Name", product.CategoryID);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HttpPostedFileBase ImagePath, [Bind(Include = "ProductID,CategoryID,Name,Description,UnitPrice,Quantity,ImagePath,Status")] Product product)
        {
            var validImageTypes = new string[]
            {
                "image/gif",
                "image/jpeg",
                "image/pjpeg",
                "image/png"
            };

            if (ImagePath != null && ImagePath.ContentLength > 0 && !validImageTypes.Contains(ImagePath.ContentType))
            {
                ModelState.AddModelError("ImageUpload", "Please choose either a GIF, JPG or PNG image.");
            }

            if (ModelState.IsValid)
            {
                if (ImagePath != null && ImagePath.ContentLength > 0)
                {
                    var uploadDir = "~/Upload";
                    var imagePath = Path.Combine(Server.MapPath(uploadDir), ImagePath.FileName);
                    var imageUrl = Path.Combine(uploadDir, ImagePath.FileName);
                    ImagePath.SaveAs(imagePath);
                    product.ImagePath = ImagePath.FileName;
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    db.Products.Attach(product);
                    db.Entry(product).State = EntityState.Modified;
                    db.Entry(product).Property("ImagePath").IsModified = false;
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryId", "Name", product.CategoryID);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
