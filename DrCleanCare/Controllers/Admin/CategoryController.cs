using DrCleanCare.DAL;
using DrCleanCare.DAL.Security;
using DrCleanCare.Helpers;
using DrCleanCare.Models;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace DrCleanCare.Controllers.Admin
{
    public class CategoryController : BaseController
    {

        DataContext db = new DataContext();

        //
        // GET: /Category/
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Add()
        {
            /*
             * User access add category form
             */

            AddCategoryViewModel model = new AddCategoryViewModel();
            model.OrderNumber = 100;
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Add(AddCategoryViewModel model)
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
                Category categoryInfo = new Category();

                categoryInfo.CategoryName = model.CategoryName;
                categoryInfo.Description = model.Description;
                categoryInfo.OrderNumber = model.OrderNumber;
                categoryInfo.Displayed = model.Displayed;

                // Save upload image
                bool wasImageUploaded =
                    model.Icon != null &&
                    model.Icon.ContentLength > 0 &&
                    Helpers.Common.IsImage(model.Icon);

                if (wasImageUploaded)
                {
                    Image imageUpload = Image.FromStream(model.Icon.InputStream);

                    // get image upload configuration info 
                    var uploadPath = ConfigurationManager.AppSettings["PRODUCT_IMAGE_UPLOAD_PATH"];

                    int iconWidth = 128;
                    int.TryParse(ConfigurationManager.AppSettings["PRODUCT_IMAGE_WIDTH_SMALL"], out iconWidth);

                    // scale image
                    Image iconImage = Helpers.Common.ScaleImage(imageUpload, iconWidth);

                    // save upload image
                    var iconFileName = Helpers.Common.SaveImage(iconImage, uploadPath, model.Icon.FileName);
                    categoryInfo.IconURL = Path.Combine(uploadPath, iconFileName);
                }

                // save new category info
                db.Categories.Add(categoryInfo);
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
             * User access edited category form
             */
            try
            {
                // get category info
                Category categoryInfo = db.Categories.Where(c => c.CategoryId == id).FirstOrDefault();
                if (categoryInfo == null)
                {
                    string errorMessage = "Nhóm sản phẩm #" + id + " không tồn tại trong hệ thống";
                    return RedirectToAction("Index", "AdminError", new { message = errorMessage });
                }

                // Create view model
                AddCategoryViewModel model = new AddCategoryViewModel();
                model.CategoryId = categoryInfo.CategoryId;
                model.CategoryName = categoryInfo.CategoryName;
                model.Description = categoryInfo.Description;
                model.IconURL = categoryInfo.IconURL;
                model.OrderNumber = categoryInfo.OrderNumber ?? 0;
                model.Displayed = categoryInfo.Displayed;

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
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult Edit(AddCategoryViewModel model)
        {
            /*
             * User posted edited category data
             */
            try
            {
                // validate input data
                if (ModelState.IsValid == false)
                {
                    ModelState.AddModelError("", "Thông tin nhóm sản phẩm không hợp lệ");
                    return View(model);
                }

                // get current category info
                Category categoryInfo = db.Categories.Where(c => c.CategoryId == model.CategoryId).FirstOrDefault();
                if (categoryInfo == null)
                {
                    string errorMessage = "Nhóm sản phẩm #" + model.CategoryId + " không tồn tại trong hệ thống";
                    return RedirectToAction("Index", "AdminError", new { message = errorMessage });
                }

                // update category info
                categoryInfo.CategoryName = model.CategoryName;
                categoryInfo.Description = model.Description;
                categoryInfo.OrderNumber = model.OrderNumber;
                categoryInfo.Displayed = model.Displayed;

                // save category image
                bool wasImageUploaded =
                    model.Icon != null &&
                    model.Icon.ContentLength > 0 &&
                    Helpers.Common.IsImage(model.Icon);
                if (wasImageUploaded)
                {
                    // get uploaded image
                    Image imageUploaded = Image.FromStream(model.Icon.InputStream);

                    // get image upload configuration info 
                    var uploadPath = ConfigurationManager.AppSettings["PRODUCT_IMAGE_UPLOAD_PATH"];

                    int iconWidth = 128;
                    int.TryParse(ConfigurationManager.AppSettings["PRODUCT_IMAGE_WIDTH_SMALL"], out iconWidth);

                    // scale image
                    Image iconImage = Helpers.Common.ScaleImage(imageUploaded, iconWidth);

                    try
                    {
                        // save upload image
                        var iconFileName = Helpers.Common.SaveImage(iconImage, uploadPath, model.Icon.FileName);
                        categoryInfo.IconURL = Path.Combine(uploadPath, iconFileName);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                        return View(model);
                    }
                }

                // save updated category info
                db.SaveChanges();

                return RedirectToAction("Index");
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
            Category categoryInfo = db.Categories.Where(c => c.CategoryId == id).FirstOrDefault();
            if (categoryInfo == null)
            {
                string errorMessage = "Nhóm sản phẩm #" + id + " không tồn tại trong hệ thống";
                return Json(new { Error = 1, Message = errorMessage });
            }

            // delete category
            try
            {
                db.Categories.Remove(categoryInfo);
                db.SaveChanges();
                return Json(new { Error = 0, Message = "Xóa nhóm sản phẩm thành công" });
            }
            catch (ArgumentNullException ex)
            {
                return Json(new { Error = 2, Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { Error = 2, Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public JsonResult Search(string searchText, int pageIndex = 1)
        {
            try
            {
                var categories = db.Categories.OrderBy(r => r.OrderNumber).AsEnumerable();
                if (searchText.Trim() != "")
                {
                    categories = from t1 in categories
                                 where t1.CategoryName.Contains(searchText.Trim())
                                 select t1;
                }
                var pageHelper = new PagingHelper(categories, pageIndex, AppSettings.DEFAULT_PAGE_SIZE);
                return Json(new
                {
                    Error = 0,
                    RowCount = pageHelper.RowCount,
                    PageIndex = pageHelper.PageIndex,
                    PageSize = pageHelper.PageSize,
                    PageTotal = pageHelper.PageTotal,
                    Categories = pageHelper.PagedData
                });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetCategories()
        {
            try
            {
                // get categories
                var categories = db.Categories.OrderBy(c => c.CategoryName).ToList();

                return Json(new { Error = 0, Message = "Success", Categories = categories });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }
    }
}
