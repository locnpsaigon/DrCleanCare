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

    public class RoleController : BaseController
    {

        DataContext db = new DataContext();

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult Index(int? page)
        {
            return View();
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult Add()
        {
            /*
             * User access add role page
             */

            // create view model
            var model = new AddRoleViewModel();

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult Add(AddRoleViewModel model)
        {
            /*
             * User submitted add role data
             */
            try
            {
                // validate user input data
                if (ModelState.IsValid == false)
                {
                    ModelState.AddModelError("", "Thông tin vai trò cần tạo không hợp lệ");
                    return View(model);
                }

                // check role name existed
                var result = db.Roles
                    .Where(r => r.RoleName.Equals(model.RoleName.Trim(), StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

                bool wasRoleNameExisted = result != null;

                // create new role info
                Role roleInfo = new Role();
                roleInfo.RoleName = model.RoleName;
                roleInfo.Description = model.Description;

                // save new role info
                db.Roles.Add(roleInfo);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (ArgumentNullException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
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
        public ActionResult Edit(int id = 0)
        {
            /*
             * User access edited role page
             */
            try
            {
                // get role info
                Role roleInfo = db.Roles.Where(r => r.RoleId == id).FirstOrDefault();
                if (roleInfo == null)
                    throw new Exception("Vai trò #" + id + " không tồn tại trong hệ thống");

                var model = new EditRoleViewModel();
                model.RoleId = roleInfo.RoleId;
                model.RoleName = roleInfo.RoleName;
                model.Description = roleInfo.Description;

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
        public ActionResult Edit(EditRoleViewModel model)
        {
            /*
             * User submitted edited role info
             */
            try
            {
                // validate user input data
                if (ModelState.IsValid == false)
                {
                    string errorMessage = "Thông tin cập nhật vai trò không hợp lệ";
                    ModelState.AddModelError("", errorMessage);

                    return View(model);
                }

                // get current role info
                Role roleInfo = db.Roles.Where(r => r.RoleId == model.RoleId).FirstOrDefault();

                // update role info
                roleInfo.RoleName = model.RoleName;
                roleInfo.Description = model.Description;

                // save updated role info
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
            /*
             * User delete a role
             */
            try
            {
                // get role info
                Role roleInfo = db.Roles.Where(r => r.RoleId == id).FirstOrDefault();
                if (roleInfo == null)
                {
                    string errorMessage = "Vai trò #" + id + " không tồn tại trong hệ thống";
                    return Json(new { Error = 1, Description = errorMessage });
                }

                // remove all users from current role
                for (int i = roleInfo.Users.Count - 1; i >= 0; i--)
                {
                    var roleUser = roleInfo.Users.ElementAt(i);

                    // remove user from this role
                    roleInfo.Users.Remove(roleUser);
                }

                // remove role
                db.Roles.Remove(roleInfo);
                db.SaveChanges();

                return Json(new { Error = 0, Message = "Xóa vai trò thành công" });
            }
            catch (Exception ex)
            {
                string errorMessage = "Xóa vai trò thất bại! <p />Mô tả lỗi:" + ex.Message;
                return Json(new { Error = 1, Message = errorMessage });
            }
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public JsonResult Search(string searchText = "", int pageIndex = 1)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var roles = db.Roles.OrderBy(r => r.RoleName).AsEnumerable();
                if (searchText.Trim() != "")
                {
                    roles = roles.Where(r => r.RoleName.ToLower().Contains(searchText.ToLower()));
                }
                var paging = new PagingHelper(roles, pageIndex, AppSettings.DEFAULT_PAGE_SIZE);
                return Json(new
                {
                    Error = 0,
                    RowCount = paging.RowCount,
                    PageIndex = paging.PageIndex,
                    PageSize = paging.PageSize,
                    PageTotal = paging.PageTotal,
                    Roles = paging.PagedData
                });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }
    }
}
