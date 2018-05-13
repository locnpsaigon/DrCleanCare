using System;
using System.Globalization;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using DrCleanCare.DAL;
using DrCleanCare.DAL.Security;
using DrCleanCare.Helpers;

namespace DrCleanCare.Controllers.Admin
{
    public class FeedbackController : Controller
    {
        DataContext db = new DataContext();

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public JsonResult Add(string name, string phone, string email, string message, string captcha)
        {
            try
            {
                // decode base64
                //name = Common.DecodeFrom64(name);
                //phone = Common.DecodeFrom64(phone);
                //email = Common.DecodeFrom64(email);
                //message = Common.DecodeFrom64(message);
                //captcha = Common.DecodeFrom64(captcha);

                // validate captcha
                var currentCaptcha = Session["Captcha"].ToString();
                if (string.Compare(currentCaptcha, captcha, false) != 0)
                {
                    return Json(new { Error = 1, Message = "Mã bảo vệ không đúng!" });
                }

                Feedback fb = new Feedback();
                fb.Date = DateTime.Now;
                fb.Name = name;
                fb.Phone = phone;
                fb.Email = email;
                fb.Message = message;

                db.Feedbacks.Add(fb);
                db.SaveChanges();

                return Json(new { Error = 0, Message = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }

        [Authorize]
        public JsonResult GetFeedbacks(string dateFrom, string dateTo)
        {
            try
            {
                // parse date range
                var d1 = DateTime.ParseExact(dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var d2 = DateTime.ParseExact(dateTo + " 23:59:59", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                var feedbacks = db.Feedbacks
                    .Where(fb => fb.Date >= d1 && fb.Date <= d2)
                    .OrderByDescending(fb => fb.Date)
                    .ToList();

                return Json(new {
                    Error = 0,
                    Message = "Success",
                    Feedbacks = feedbacks });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public JsonResult Delete(string feedbackIds)
        {
            try
            {
                var ids = feedbackIds.Split(',');
                foreach(var id in ids)
                {
                    var idToRemove = 0;
                    int.TryParse(id, out idToRemove);
                    var itemToRemove = db.Feedbacks.Where(fb => fb.Id == idToRemove).FirstOrDefault();
                    if (itemToRemove != null) db.Feedbacks.Remove(itemToRemove);
                }
                db.SaveChanges();

                return Json(new { Error = 0, Message = "Xóa thông tin phản hồi thành công!" });
            }
            catch(Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }
    }
}