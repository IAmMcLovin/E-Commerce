using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eCommerce.DAL;


namespace eCommerce.Controllers
{
    public class HomeController : Controller
    {
        private CommerceContext db = new CommerceContext();

        public ActionResult Index()
        {
            var products = db.Products.Where(s => s.Status.Equals(true)).Take(4);

            products = products.OrderByDescending(s => s.Date);

            return View(products);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact() 
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}