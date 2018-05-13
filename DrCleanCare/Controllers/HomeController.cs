using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using DrCleanCare.DAL;
using DrCleanCare.Models;

namespace DrCleanCare.Controllers
{
    public class HomeController : Controller
    {
        DataContext db = new DataContext();

        public ActionResult Index()
        {
            //return RedirectToAction("Maintenance");
            try
            {
                var categories = db.Categories
                    .Where(r => r.Displayed == true)
                    .OrderBy(r => r.OrderNumber)
                    .ThenBy(r => r.CategoryName)
                    .ToList();
                
                return View(categories);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorMessage",
                    new RouteValueDictionary(
                        new { message = ex.Message }));
            }
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Events()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Maintenance()
        {
            return View();
        }

        public ActionResult ErrorMessage(string message)
        {
            ViewBag.ErrorMessage = message;
            return View();
        }

        public ActionResult Products(int categoryId = 0)
        {
            //return RedirectToAction("Maintenance");
            try
            {
                var categories = db.Categories.Where(r => r.Displayed == true).OrderBy(r => r.CategoryName).AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Text = r.CategoryName,
                        Value = r.CategoryId.ToString(),
                        Selected = r.CategoryId == categoryId
                    })
                    .ToList();
                ViewBag.Categories = categories;
            }
            catch (ArgumentNullException ex)
            {
                return RedirectToAction("ErrorMessage",
                    new RouteValueDictionary(
                        new { message = ex.Message }));
            }
            return View();
        }

        public ActionResult ProductDetails(int id = 0)
        {
            //return RedirectToAction("Maintenance");
            try
            {
                ProductDetailsViewModel model = new ProductDetailsViewModel();
                model.Product = db.Products
                    .Where(p => p.ProductId == id)
                    .FirstOrDefault();
                if (model.Product == null)
                {
                    string errorMessage = "Sản phẩm #" + id + " không có thông tin";

                    return RedirectToAction("ErrorMessage",
                    new RouteValueDictionary(
                        new { message = errorMessage }));

                }

                model.Category = db.Categories.Where(c => c.CategoryId == model.Product.CategoryId).FirstOrDefault();
                if (model.Category == null)
                {

                    string errorMessage = "Sản phẩm #" + id + " không có thông tin";

                    return RedirectToAction("ErrorMessage",
                    new RouteValueDictionary(
                        new { message = errorMessage }));
                }

                return View(model);
            }
            catch (ArgumentNullException ex)
            {
                return RedirectToAction("ErrorMessage",
                    new RouteValueDictionary(
                        new { message = ex.Message }));
            } 
        }
    }
}
