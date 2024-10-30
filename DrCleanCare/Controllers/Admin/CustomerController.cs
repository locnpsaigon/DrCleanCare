using DrCleanCare.DAL;
using DrCleanCare.DAL.Security;
using DrCleanCare.Helpers;
using DrCleanCare.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace DrCleanCare.Controllers.Admin
{
    public class CustomerController : BaseController
    {
        DataContext db = new DataContext();

        // GET: Customer
        public ActionResult Index()
        {
            return View();
        }


        [Authorize]
        public ActionResult Add()
        {
            /*
             * User access add customer form
             */
            var model = new AddCustomerViewModel();
            //var model = new AddCustomerViewModel
            //{
            //    FullName = "Nguyễn Lộc",
            //    Address = "113 Trương Định, Quận 3",
            //    Email = "abc@gmail.com",
            //    Phone = "090927373",
            //    Note = "Nhà có chó dữ",
            //    ShippingAddress = "12 Tân Định",
            //    ShippingContact = "A Cón",
            //    TaxCode = "HCM45678654"
            //};

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Add(AddCustomerViewModel model)
        {
            /*
             * User submitted add category form
             */
            try
            {
                // validate user input data
                if (ModelState.IsValid == false)
                {
                    ModelState.AddModelError(string.Empty, "Thông tin Nhóm sản phẩm không hợp lệ!");
                    return View(model);
                }

                // Create new category info 
                var customer = new Customer
                {
                    FullName = model.FullName,
                    Address = model.Address,
                    CreationDate = DateTime.Now,
                    Email = model.Email,
                    Note = model.Note,
                    Phone = model.Phone,
                    ShippingAddress = model.ShippingAddress,
                    ShippingContact = model.ShippingContact,
                    TaxCode = model.TaxCode
                };

                // save new category info
                db.Customers.Add(customer);
                db.SaveChanges();

                return RedirectToAction("Index");

            }
            catch (ArgumentException ex)
            {
                return RedirectToAction("ErrorMessage", "Admin",
                    new RouteValueDictionary(
                        new { message = ex.Message }));

            }
            catch (InvalidOperationException ex)
            {
                return RedirectToAction("ErrorMessage", "Admin",
                    new RouteValueDictionary(
                        new { message = ex.Message }));
            }
        }

        [Authorize]
        public ActionResult Edit(int? id)
        {
            /*
             * User access edited customer form
             */
            try
            {
                // get category info
                var customer = db.Customers.FirstOrDefault(x => x.CustomerID == id);
                if (customer != null)
                {
                    // Create view model
                    var model = new AddCustomerViewModel()
                    {
                        CustomerId = customer.CustomerID,
                        FullName = customer.FullName,
                        Address = customer.Address,
                        Email = customer.Email,
                        Note = customer.Note,
                        Phone = customer.Phone,
                        ShippingAddress = customer.ShippingAddress,
                        ShippingContact = customer.ShippingContact,
                        TaxCode = customer.TaxCode
                    };
                    return View(model);
                }
                else
                {
                    string errorMessage = "Khác hàng có mã #" + id + " không tồn tại";
                    return RedirectToAction("Index", "AdminError", new { message = errorMessage });
                }
            }
            catch (ArgumentNullException ex)
            {
                return RedirectToAction("ErrorMessage", "Admin",
                    new RouteValueDictionary(
                        new { message = ex.Message }));
            }
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult Edit(AddCustomerViewModel model)
        {
            /*
             * User posted edited category data
             */
            try
            {
                // validate input data
                if (ModelState.IsValid == false)
                {
                    ModelState.AddModelError("", "Thông tin khách hàng không hợp lệ");
                    return View(model);
                }

                var customer = db.Customers.FirstOrDefault(x => x.CustomerID == model.CustomerId);
                if (customer != null)
                {
                    customer.FullName = model.FullName; customer.Address = model.Address;
                    customer.ShippingAddress = model.ShippingAddress;
                    customer.ShippingContact = model.ShippingContact;
                    customer.TaxCode = model.TaxCode;
                    customer.Phone = model.Phone;
                    customer.Email = model.Email;
                    customer.Note = model.Note;
                    customer.Address = model.Address;

                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", $"Khách hàng có mã {model.CustomerId} không tồn tại");
                    return View(model);
                }
            }
            catch (ArgumentNullException ex)
            {
                return RedirectToAction("ErrorMessage", "Admin",
                    new RouteValueDictionary(
                        new { message = ex.Message }));

            }
            catch (InvalidOperationException ex)
            {
                return RedirectToAction("ErrorMessage", "Admin",
                    new RouteValueDictionary(
                        new { message = ex.Message }));
            }
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public JsonResult Delete(int id = 0)
        {
            // get category info
            var customer = db.Customers.FirstOrDefault(x => x.CustomerID == id);
            if (customer != null)
            {
                db.Customers.Remove(customer);
                db.SaveChanges();
            }
            return Json(new { Error = 0, Message = "Thông tin khách hàng thành công" });
        }

        [HttpPost]
        [Authorize]
        public JsonResult Search(string searchText, int pageIndex = 1)
        {
            try
            {
                var customers = db.Customers
                    .AsEnumerable()
                    .Where(x =>
                        x.FullName.ConvertToUnSign().Contains(searchText.ConvertToUnSign()) ||
                        string.IsNullOrWhiteSpace(searchText))
                    .ToList();

                var pageHelper = new PagingHelper(customers, pageIndex, AppSettings.DEFAULT_PAGE_SIZE);
                return Json(new
                {
                    Error = 0,
                    RowCount = pageHelper.RowCount,
                    PageIndex = pageHelper.PageIndex,
                    PageSize = pageHelper.PageSize,
                    PageTotal = pageHelper.PageTotal,
                    Customers = pageHelper.PagedData
                });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }
    }
}