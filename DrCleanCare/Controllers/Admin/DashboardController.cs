using DrCleanCare.DAL;
using DrCleanCare.DAL.Security;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace DrCleanCare.Controllers.Admin
{
    public class DashboardController : Controller
    {

        DataContext db = new DataContext();

        //
        // GET: /Dashboard/
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult FinanceByDate()
        {
            return View();
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult FinanceByYear()
        {
            return View();
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult ProductByDate()
        {
            return View();
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult ProductByDateDetails(string dateFrom, string dateTo, string productId)
        {
            var reportData = new List<ReportProductByDateDetailsResult>();
            try
            {
                DateTime d1, d2;
                d1 = DateTime.ParseExact(dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                d2 = DateTime.ParseExact(dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                ViewBag.DateFrom = d1;
                ViewBag.DateTo = d2;
                ViewBag.ProductId = productId;

                reportData = db.Database.SqlQuery<ReportProductByDateDetailsResult>(
                   "EXEC [dbo].[usp_getProductReportByDateDetails] @DateFrom, @DateTo, @ProductId",
                   new SqlParameter("DateFrom", d1),
                   new SqlParameter("DateTo", d2),
                   new SqlParameter("ProductId", productId))
                   .ToList<ReportProductByDateDetailsResult>();

                return View(reportData);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
            }

            return View(reportData);
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult ProductByDateDetailsToExcel(string dateFrom, string dateTo, string productId)
        {
            var reportData = new List<ReportProductByDateDetailsResult>();
            try
            {
                DateTime d1, d2;
                d1 = DateTime.ParseExact(dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                d2 = DateTime.ParseExact(dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                ViewBag.DateFrom = d1;
                ViewBag.DateTo = d2;

                reportData = db.Database.SqlQuery<ReportProductByDateDetailsResult>(
                   "EXEC [dbo].[usp_getProductReportByDateDetails] @DateFrom, @DateTo, @ProductId",
                   new SqlParameter("DateFrom", d1),
                   new SqlParameter("DateTo", d2),
                   new SqlParameter("ProductId", productId))
                   .ToList<ReportProductByDateDetailsResult>();

                var template = new FileInfo(Server.MapPath("~/App_Data/Export_Product_By_Date_Details_Template.xlsx"));
                using (var package = new ExcelPackage(template))
                {
                    ExcelWorkbook workBook = package.Workbook;
                    if (workBook != null)
                    {
                        if (workBook.Worksheets.Count > 0)
                        {
                            ExcelWorksheet currentWorksheet = workBook.Worksheets.First();

                            var rowIndex = 2; // first row is headers
                            foreach (var item in reportData)
                            {
                                currentWorksheet.Cells[rowIndex, 1].Value = rowIndex - 1;
                                currentWorksheet.Cells[rowIndex, 2].Value = item.CustomerName;
                                currentWorksheet.Cells[rowIndex, 3].Value = item.CustomerAddress;
                                currentWorksheet.Cells[rowIndex, 4].Value = item.Phone;
                                currentWorksheet.Cells[rowIndex, 5].Value = item.ProductName;
                                currentWorksheet.Cells[rowIndex, 6].Value = item.Quantity;
                                currentWorksheet.Cells[rowIndex, 7].Value = item.AmountBT;
                                currentWorksheet.Cells[rowIndex, 8].Value = item.Amount;
                                rowIndex++;
                            }

                            Response.Clear();
                            package.SaveAs(Response.OutputStream);
                            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            var excel_filename = "Export_Product_By_Date_Details_" + d1.ToString("yyyyMMdd") + "_" + d2.ToString("yyyyMMdd") + ".xlsx";
                            Response.AddHeader("content-disposition", "attachment;  filename=" + excel_filename);
                            Response.End();
                        }
                    }
                }

                return View();
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
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult ProductByDateDetailsForCustomer(string phone, string dateFrom, string dateTo, string productId)
        {
            var reportData = new List<ReportProductByDateDetailsForCustomerResult>();
            try
            {
                DateTime d1, d2;
                d1 = DateTime.ParseExact(dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                d2 = DateTime.ParseExact(dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                ViewBag.DateFrom = d1;
                ViewBag.DateTo = d2;
                ViewBag.ProductId = productId;

                reportData = db.Database.SqlQuery<ReportProductByDateDetailsForCustomerResult>(
                   "EXEC [dbo].[usp_getProductReportByDateDetailsForCustomer] @CustomerPhone, @DateFrom, @DateTo, @ProductId",
                   new SqlParameter("CustomerPhone", phone),
                   new SqlParameter("DateFrom", d1),
                   new SqlParameter("DateTo", d2),
                   new SqlParameter("ProductId", productId))
                   .ToList<ReportProductByDateDetailsForCustomerResult>();

                return View(reportData);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
            }

            return View(reportData);
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult ProductByDateDetailsForCustomerToExcel(string phone, string dateFrom, string dateTo, string productId)
        {
            var reportData = new List<ReportProductByDateDetailsForCustomerResult>();
            try
            {
                DateTime d1, d2;
                d1 = DateTime.ParseExact(dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                d2 = DateTime.ParseExact(dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                ViewBag.DateFrom = d1;
                ViewBag.DateTo = d2;

                reportData = db.Database.SqlQuery<ReportProductByDateDetailsForCustomerResult>(
                   "EXEC [dbo].[usp_getProductReportByDateDetailsForCustomer]  @CustomerPhone, @DateFrom, @DateTo, @ProductId",
                   new SqlParameter("CustomerPhone", phone),
                   new SqlParameter("DateFrom", d1),
                   new SqlParameter("DateTo", d2),
                   new SqlParameter("ProductId", productId))
                   .ToList<ReportProductByDateDetailsForCustomerResult>();

                var template = new FileInfo(Server.MapPath("~/App_Data/Export_Product_By_Date_Details_For_Customer_Template.xlsx"));
                using (var package = new ExcelPackage(template))
                {
                    ExcelWorkbook workBook = package.Workbook;
                    if (workBook != null)
                    {
                        if (workBook.Worksheets.Count > 0)
                        {
                            ExcelWorksheet currentWorksheet = workBook.Worksheets.First();

                            var rowIndex = 2; // first row is headers
                            foreach (var item in reportData)
                            {
                                currentWorksheet.Cells[rowIndex, 1].Value = rowIndex - 1;
                                currentWorksheet.Cells[rowIndex, 2].Value = item.OrderDate;
                                currentWorksheet.Cells[rowIndex, 3].Value = item.OrderNo;
                                currentWorksheet.Cells[rowIndex, 4].Value = item.CustomerName;
                                currentWorksheet.Cells[rowIndex, 5].Value = item.CustomerAddress;
                                currentWorksheet.Cells[rowIndex, 6].Value = item.Phone;
                                currentWorksheet.Cells[rowIndex, 7].Value = item.ProductName;
                                currentWorksheet.Cells[rowIndex, 8].Value = item.Quantity;
                                currentWorksheet.Cells[rowIndex, 9].Value = item.AmountBT;
                                currentWorksheet.Cells[rowIndex, 10].Value = item.Amount;
                                rowIndex++;
                            }

                            Response.Clear();
                            package.SaveAs(Response.OutputStream);
                            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            var excel_filename = "Export_Product_By_Date_Details_For_Customer_" + d1.ToString("yyyyMMdd") + "_" + d2.ToString("yyyyMMdd") + ".xlsx";
                            Response.AddHeader("content-disposition", "attachment;  filename=" + excel_filename);
                            Response.End();
                        }
                    }
                }

                return View();
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
    }
}
