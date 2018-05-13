using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DrCleanCare.Models;
using DrCleanCare.DAL;
using DrCleanCare.DAL.Security;

namespace DrCleanCare.Controllers.Admin
{
    public class PaymentController : Controller
    {
        DataContext db = new DataContext();

        /// <summary>
        /// User add payment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Add(int id)
        {
            try
            {
                // get so info
                var soInfo = db.Orders.Where(p => p.OrderId == id).FirstOrDefault();
                if (soInfo == null)
                {
                    return RedirectToAction("ErrorMessage", "Admin", new { message = "Không tìm thấy đơn hàng!" });
                }

                // create view model
                AddPaymentViewModel model = new AddPaymentViewModel();
                model.OrderID = soInfo.OrderId;
                model.OrderDate = soInfo.OrderDate.ToString("dd/MM/yyyy");
                model.OrderNo = soInfo.OrderNo;
                model.CustomerName = soInfo.CustomerName;
                model.CustomerAddress = soInfo.CustomerAddress;
                model.Email = soInfo.Email;

                // generate payment types select list
                model.PaymentTypeOptions = new SelectList(
                    db.PaymentTypes.OrderBy(p => p.PaymentTypeName).ToList(),
                    "PaymentTypeId",
                    "PaymentTypeName");

                // get order details to calculate order amount
                var orderDetails = db.OrderDetails.Where(p => p.OrderId == soInfo.OrderId).ToList();
                model.AmountBT = 0;
                model.VAT = 0;
                model.GrandTotal = 0;
                foreach (var item in orderDetails)
                {
                    model.AmountBT += item.Quantity * item.UnitPriceBT;
                    model.VAT += item.Quantity * (item.UnitPrice - item.UnitPriceBT);
                    model.GrandTotal += item.Quantity * item.UnitPrice;
                }
                
                // get payment history
                var payments = db.Payments.Where(p => p.OrderID == soInfo.OrderId);
                var paymentTypes = db.PaymentTypes;
                var paymentHistory = payments.AsEnumerable().Join(paymentTypes.AsEnumerable(),
                    p => p.PaymentTypeId,
                    d => d.PaymentTypeId,
                    (p, d) => new PaymentHistoryResult
                    {
                        PaymentId = p.PaymentId,
                        PaymentDate = p.PaymentDate,
                        PaymentTypeId = p.PaymentTypeId,
                        PaymentTypeName = d.PaymentTypeName,
                        PaymentAmount = p.PaymentAmount,
                        Description = p.Description
                    })
                    .OrderByDescending(h => h.PaymentDate)
                    .ToList();
                model.PaymentHistory = paymentHistory;
                
                model.PaidAmount = 0;
                foreach (var item in model.PaymentHistory)
                {
                    model.PaidAmount += item.PaymentAmount;
                }

                // calculate debt amount
                model.DebtAmount = model.GrandTotal - model.PaidAmount;

                // set default payment amount equals to debt amount
                model.PaymentAmount = model.DebtAmount; 

                // get payments history
                model.PaymentHistory = paymentHistory;

                return View(model);
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
        public ActionResult Add(AddPaymentViewModel model)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    // generate payment type options
                    model.PaymentTypeOptions = new SelectList(
                        db.PaymentTypes.OrderBy(p => p.PaymentTypeName).ToList(),
                        "PaymentTypeId",
                        "PaymentTypeName");

                    ModelState.AddModelError(string.Empty, "Thông tin thanh toán không hợp lệ!");
                    return View(model);
                }

                Payment paymentInfo = new Payment();
                paymentInfo.OrderID = model.OrderID;

                paymentInfo.PaymentDate = DateTime.ParseExact(model.PaymentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                paymentInfo.PaymentDate += new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                paymentInfo.PaymentTypeId = model.PaymentType;
                paymentInfo.PaymentAmount = model.PaymentAmount;
                paymentInfo.Description = model.Description;

                db.Payments.Add(paymentInfo);
                db.SaveChanges();

                return RedirectToAction("Index", "Order");
            }
            catch (ArgumentNullException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(model);
        }

        [Authorize]
        public ActionResult Unpaid()
        {
            try
            {
                // get orders which are currently in debt
                var model = db.Database.SqlQuery<ReportOrdersInDebtResult>(
                    "EXEC [dbo].[usp_getOrdersInDebt]");

                return View(model);
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

        /// <summary>
        /// Delete a specified payment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public JsonResult Delete(int id)
        {
            try
            {
                // get payment info
                var paymentInfo = db.Payments.Where(p => p.PaymentId == id).FirstOrDefault();

                if (paymentInfo != null)
                {
                    db.Payments.Remove(paymentInfo);
                    db.SaveChanges();

                    return Json(new { Error = 0, Message = "Xóa thanh toán thành công!" });
                }

                return Json(new { Error = 1, Message = "Xóa thanh toán thất bại!" });
            }
            catch (ArgumentNullException ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }
    }
}
