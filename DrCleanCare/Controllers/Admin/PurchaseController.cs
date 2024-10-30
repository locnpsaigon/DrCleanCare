using DrCleanCare.DAL;
using DrCleanCare.DAL.Security;
using DrCleanCare.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace DrCleanCare.Controllers.Admin
{
    public class PurchaseController : BaseController
    {
        DataContext db = new DataContext();

        //
        // GET: /Purchase/

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult Index(string dateFrom, string dateTo, string keyword)
        {
            try
            {
                DateTime d1, d2;
                // Get date range
                if (!DateTime.TryParseExact(dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out d1))
                {
                    DateTime temp = DateTime.Now;
                    d1 = new DateTime(temp.Year, temp.Month, 1);
                }
                if (!DateTime.TryParseExact(dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out d2))
                {
                    DateTime temp = DateTime.Now;
                    d2 = new DateTime(temp.Year, temp.Month, temp.Day, 23, 59, 59);
                }
                else
                {
                    d2.Add(new TimeSpan(23, 59, 59));
                }

                // save PO search filter into View Bag
                ViewBag.DateFrom = d1.ToString("dd/MM/yyyy");
                ViewBag.DateTo = d2.ToString("dd/MM/yyyy");
                ViewBag.Keyword = keyword;

                // search purchase
                var query = db.Purchases
                    .Where(p => p.PurchaseDate >= d1 && p.PurchaseDate <= d2);
                if (string.IsNullOrWhiteSpace(keyword) == false)
                {
                    query = query.Where(p => p.PurchaseName.Contains(keyword));
                }
                var purchases = query.OrderByDescending(p => p.PurchaseDate).ToList();

                // get paging configuration
                return View(purchases);
            }
            catch (ArgumentNullException ex)
            {
                return RedirectToAction("ErrorMessage", "Admin",
                    new RouteValueDictionary(
                        new { message = ex.Message }));
            }
        }

        [Authorize]
        public ActionResult Add()
        {
            AddPOViewModel model = new AddPOViewModel();
            model.PurchaseDate = DateTime.Now.ToString("dd/MM/yyyy");
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Add(AddPOViewModel model)
        {
            try
            {
                // validate user input data
                if (ModelState.IsValid == false)
                {
                    ModelState.AddModelError(string.Empty, "Thông tin chi phí không hợp lệ.");
                    return View(model);
                }

                // get amount
                var amount = (decimal)0;
                if (!decimal.TryParse(model.Amount.Replace(",", ""), out amount))
                {
                    ModelState.AddModelError("", "Chi phí '" + model.Amount + "' không hợp lệ!");
                    return View(model);
                }

                // create new purchase info
                Purchase purchaseInfo = new Purchase();
                purchaseInfo.PurchaseDate = DateTime.ParseExact(model.PurchaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                purchaseInfo.PurchaseDate += new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                purchaseInfo.PurchaseName = model.PurchaseName;
                purchaseInfo.Description = model.Description;
                purchaseInfo.Amount = amount;

                // save new purchase info
                db.Purchases.Add(purchaseInfo);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(model);
        }

        /// <summary>
        /// User access data to update purchase order information
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Edit(int id = 0)
        {
            try
            {
                // get current purchase info
                Purchase purchaseInfo = db.Purchases.Where(p => p.PurchaseId == id).FirstOrDefault();
                if (purchaseInfo == null)
                {
                    string errorMessage = string.Format("Thông tin chi phí mã #{0} không tồn tại trong hệ thống", id);
                    return RedirectToAction("ErrorMessage", "Admin", new { message = errorMessage });
                }

                // create edited purchase view model
                AddPOViewModel model = new AddPOViewModel();
                model.PurchaseId = purchaseInfo.PurchaseId;
                model.PurchaseDate = purchaseInfo.PurchaseDate.ToString("dd/MM/yyyy");
                model.PurchaseName = purchaseInfo.PurchaseName;
                model.Description = purchaseInfo.Description;
                model.Amount = purchaseInfo.Amount.ToString("#,##0");

                return View(model);
            }
            catch (ArgumentNullException ex)
            {
                return RedirectToAction("ErrorMessage", "Admin",
                    new RouteValueDictionary(
                        new { message = ex.Message }));
            }
        }


        /// <summary>
        /// User submitted data to update purchase order data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult Edit(AddPOViewModel model)
        {
            try
            {
                // validate input data
                if (ModelState.IsValid == false)
                {
                    ModelState.AddModelError(string.Empty, "Thông tin cập nhật chi phí không hợp lệ");
                    return View(model);
                }

                // get amount
                var amount = (decimal)0;
                if (!decimal.TryParse(model.Amount.Replace(",", ""), out amount))
                {
                    ModelState.AddModelError("", "Chi phí '" + model.Amount + "' không hợp lệ!");
                    return View(model);
                }

                // get current purchase info
                Purchase purchaseInfo = db.Purchases.Where(p => p.PurchaseId == model.PurchaseId).FirstOrDefault();
                if (purchaseInfo == null)
                {
                    string errorMessage = string.Format("Thông tin chi phí #{0} không tồn tại trong hệ thống", model.PurchaseId);
                    return RedirectToAction("ErrorMessage", "Admin", new { message = errorMessage });
                }

                // update purchase info
                purchaseInfo.PurchaseName = model.PurchaseName;
                purchaseInfo.PurchaseDate = DateTime.ParseExact(model.PurchaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                purchaseInfo.PurchaseDate += new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                purchaseInfo.Description = model.Description;
                purchaseInfo.Amount = amount;

                db.SaveChanges();


                Boolean isAdmin = User.Roles.Contains("Administrators");
                if (isAdmin)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Home", "Admin");
                }
            }
            catch (ArgumentNullException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View(model);
        }

        /// <summary>
        /// Delete purchase order action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public JsonResult Delete(int id = 0)
        {
            /*
             * User delete a purchase order
             */
            try
            {
                // get PO info
                Purchase purchaseInfo = db.Purchases.Where(p => p.PurchaseId == id).FirstOrDefault();
                if (purchaseInfo == null)
                {
                    string errorMessage = "Khoản chi #" + id + " không tồn tại trong hệ thống";
                    return Json(new { Error = 1, Description = errorMessage });
                }

                db.Purchases.Remove(purchaseInfo);
                db.SaveChanges();

                return Json(new { Error = 0, Description = "Delete purchase info success" });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { Error = 2, Description = ex.Message });
            }
        }



    }
}
