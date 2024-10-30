using DrCleanCare.DAL;
using DrCleanCare.DAL.Security;
using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace DrCleanCare.Controllers.Admin
{
    public class ReportController : Controller
    {
        DataContext db = new DataContext();

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public JsonResult GetFinanceReportByDate(string dateFrom, string dateTo)
        {
            try
            {
                // get date range
                DateTime d1, d2;
                Boolean success1 = DateTime.TryParseExact(dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out d1);
                if (success1 == false)
                {
                    d1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                }
                Boolean success2 = DateTime.TryParseExact(dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out d2);
                if (success2 == false)
                {
                    d2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                }
                d2 = d2.Add(new TimeSpan(23, 59, 59));

                // get finance report
                var reportData = db.Database.SqlQuery<ReportFinanceByDateResult>(
                    "EXEC [dbo].[usp_getFinanceReportByDate] @DateFrom, @DateTo",
                    new SqlParameter("DateFrom", d1),
                    new SqlParameter("DateTo", d2))
                    .ToList<ReportFinanceByDateResult>();

                return Json(reportData);
            }
            catch (FormatException ex)
            {
                return Json(new { Error = 0, Message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return Json(new { Error = 0, Message = ex.Message });
            }
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public JsonResult GetFinanceReportByYear(int year)
        {
            try
            {
                // get finance report
                var financeReport = db.Database.SqlQuery<ReportFinanceByYearResult>(
                    "EXEC [dbo].[ups_getFinanceReportByYear]   @year",
                    new SqlParameter("Year", year));

                return Json(financeReport.ToList());
            }
            catch (FormatException ex)
            {
                return Json(new { Error = 0, Message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return Json(new { Error = 0, Message = ex.Message });
            }
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public JsonResult GetProductReportByDate(string dateFrom, string dateTo)
        {
            try
            {
                // get date range
                DateTime d1, d2;
                Boolean success1 = DateTime.TryParseExact(dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out d1);
                if (success1 == false)
                {
                    d1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                }
                Boolean success2 = DateTime.TryParseExact(dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out d2);
                if (success2 == false)
                {
                    d2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                }
                d2 = d2.Add(new TimeSpan(23, 59, 59));

                // get product report
                var reportData = db.Database.SqlQuery<ReportProductByDateResult>(
                    "EXEC [dbo].[usp_getProductReportByDate] @DateFrom, @DateTo",
                    new SqlParameter("DateFrom", d1),
                    new SqlParameter("DateTo", d2))
                    .ToList<ReportProductByDateResult>();

                return Json(reportData);
            }
            catch (FormatException ex)
            {
                return Json(new { Error = 0, Message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return Json(new { Error = 0, Message = ex.Message });
            }
        }
    }
}
