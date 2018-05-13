using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Newtonsoft.Json;
using System.Web.Security;
using DrCleanCare.DAL;
using DrCleanCare.DAL.Security;
using DrCleanCare.Models;
using DrCleanCare.Helpers;

namespace DrCleanCare.Controllers.Admin
{
    public class UserController : BaseController
    {

        DataContext db = new DataContext();

        [AllowAnonymous]
        public ActionResult Login()
        {
            /*
             * User access login page
             */
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model)
        {
            /*
             * User submitted login info
             */
            try
            {
                // validate login info
                if (ModelState.IsValid == false)
                {
                    ModelState.AddModelError("", "Thông tin đăng nhập không hợp lệ");
                    return View(model);
                }

                // get current user info
                User userInfo = db.Users
                    .Where(u => u.Username.Equals(model.UserName.Trim(), StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();
                if (userInfo == null)
                {
                    ModelState.AddModelError("", "Đăng nhập thất bại!<p />Tên tài khoản/mật khẩu không đúng");
                    return View(model);
                }

                // verify user name and password
                bool loginSuccess = SaltedHash.Verify(userInfo.Salt, userInfo.Password, model.Password);
                
                if (loginSuccess)
                {
                    // save authentication info
                    DrCleanCarePrincipalSerializeModel principal = new DrCleanCarePrincipalSerializeModel();
                    principal.UserId = userInfo.UserId;
                    principal.FirstName = userInfo.FirstName;
                    principal.LastName = userInfo.LastName;
                    principal.Roles = userInfo.Roles.Select(r => r.RoleName).ToArray();

                    string principalJson = JsonConvert.SerializeObject(principal);
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                        1,
                        userInfo.Username,
                        DateTime.Now,
                        DateTime.Now.AddHours(168), // 7 days
                        model.RememberMe,
                        principalJson);

                    string encTicket = FormsAuthentication.Encrypt(ticket);
                    HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                    Response.Cookies.Add(faCookie);

                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Đăng nhập thất bại!<p />Tên tài khoản/mật khẩu không đúng");
                    return View(model);
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
            catch (NotImplementedException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
            
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            /*
             * User sign out
             */
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }

        //
        // GET: /User/
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult Index()
        {
            return View();
        }


        [Authorize]
        [DrCleanCareAuthorize(Roles="Administrators")]
        public ActionResult Add()
        {
            /*
             * Administrator access add user page
             */
            try
            {
                // create add user view model
                AddUserViewModel model = new AddUserViewModel();

                // generate user role selected model
                var roleList = db.Roles.OrderBy(r => r.RoleName).ToList();
                foreach (Role roleItem in roleList)
                {
                    // create a new user role selected view model
                    UserRoleSelectedViewModel userRoleSelectedModel = new UserRoleSelectedViewModel();
                    userRoleSelectedModel.RoleId = roleItem.RoleId;
                    userRoleSelectedModel.RoleName = roleItem.RoleName;
                    userRoleSelectedModel.IsSelected = false;

                    model.SelectedRoleModels.Add(userRoleSelectedModel);
                }

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
        public ActionResult Add(AddUserViewModel model)
        {
            /*
             * User submitted user creation data
             */
            try
            {
                // validate input data
                if (ModelState.IsValid == false)
                {
                    ModelState.AddModelError(string.Empty, "Thông tin tạo tài khoản không hợp lệ");
                    return View(model);
                }

                // hash password
                SaltedHash sh = new SaltedHash(model.Password);

                // create new user info
                User userInfo = new User();

                userInfo.Username = model.Username;
                userInfo.Salt = sh.Salt;
                userInfo.Password = sh.Hash;
                userInfo.FirstName = model.FirstName;
                userInfo.LastName = model.LastName;
                userInfo.Email = model.Email;
                userInfo.CreateDate = DateTime.Now;
                userInfo.IsActive = model.IsActive;

                // add user's roles
                userInfo.Roles = new List<Role>();
                foreach (var userRoleSelectedModel in model.SelectedRoleModels)
                {
                    if (userRoleSelectedModel.IsSelected == true)
                    {
                        // get selected role
                        var selectedRole = db.Roles
                            .Where(r => r.RoleId == userRoleSelectedModel.RoleId)
                            .FirstOrDefault();

                        if (selectedRole != null)
                        {
                            userInfo.Roles.Add(selectedRole);
                        }
                    }
                }

                // save new user info
                db.Users.Add(userInfo);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (ArgumentNullException ex)
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
             * User access edited user info form
             */
            try
            {
                // get user info
                User userInfo = db.Users.Where(u => u.UserId == id).FirstOrDefault();
                if (userInfo == null)
                {
                    string errorMessage = "Tài khoản #" + id + " không tồn tại trong hệ thống";
                    return RedirectToAction("ErrorMessage", "Admin", new { message = errorMessage });
                }

                // create user edited model
                EditUserViewModel model = new EditUserViewModel();

                model.UserId = userInfo.UserId;
                model.Username = userInfo.Username;
                model.FirstName = userInfo.FirstName;
                model.LastName = userInfo.LastName;
                model.Password = string.Empty;
                model.PasswordConfirm = string.Empty;
                model.Email = userInfo.Email;
                model.IsActive = userInfo.IsActive;

                // generate user role selected models
                var roleList = db.Roles.OrderBy(r => r.RoleName).ToList();
                foreach (var roleItem in roleList)
                {
                    // create selected role model
                    UserRoleSelectedViewModel selectedRoleModel = new UserRoleSelectedViewModel();

                    selectedRoleModel.RoleId = roleItem.RoleId;
                    selectedRoleModel.RoleName = roleItem.RoleName;
                    selectedRoleModel.IsSelected = userInfo.Roles.Contains(roleItem);

                    model.SelectedRoleModels.Add(selectedRoleModel);
                }

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
        public ActionResult Edit(EditUserViewModel model)
        {
            /*
             * User submitted edited user data
             */
            try
            {
                // first, validate submitted data
                if (ModelState.IsValid == false)
                {
                    ModelState.AddModelError(string.Empty, "Thông tin tài khoản cập nhật không hợp lệ");
                    return View(model);
                }

                // get current user's info
                User userInfo = db.Users.Where(u => u.UserId == model.UserId).FirstOrDefault();
                if (userInfo == null)
                {
                    string errorMessage = "Cập nhật tài khoản thất bại! <p />Tài khoản #" + model.UserId + " không tồn tại trong hệ thống";
                    return RedirectToAction("ErrorMessage", "Admin", new { message = errorMessage });
                }

                // update user info
                userInfo.FirstName = model.FirstName;
                userInfo.LastName = model.LastName;
                userInfo.Email = model.Email;
                userInfo.IsActive = model.IsActive;

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    // update password
                    SaltedHash sh = new SaltedHash(model.Password);

                    userInfo.Salt = sh.Salt;
                    userInfo.Password = sh.Hash;
                }

                // clear current user's roles
                userInfo.Roles.Clear();

                // update new user's roles
                foreach (var selectedRoleModel in model.SelectedRoleModels)
                {
                    if (selectedRoleModel.IsSelected)
                    {
                        var selectedRole = db.Roles.Where(r => r.RoleId == selectedRoleModel.RoleId).FirstOrDefault();
                        if (selectedRole != null)
                        {
                            userInfo.Roles.Add(selectedRole);
                        }
                    }
                }

                // save changes
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (ArgumentNullException ex)
            {
                return RedirectToAction("ErrorMessage", "Admin",
                    new RouteValueDictionary(
                        new { message = ex.Message }));
            }
            catch (NotSupportedException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [Authorize]
        public ActionResult ChangePass()
        {
            /*
             * User change password
             */
            try
            {
                // get current logon user
                User logonUser = db.Users.Where(u => u.UserId == User.UserId).FirstOrDefault();
                if (logonUser == null)
                {
                    return RedirectToAction("Logout", "User");
                }

                // create user change password view model
                ChangeUserPassViewModel model = new ChangeUserPassViewModel();
                model.UserId = logonUser.UserId;
                model.UserName = logonUser.Username;

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
        public ActionResult ChangePass(ChangeUserPassViewModel model)
        {
            /*
             * User submitted data to change password
             */ 
            try
            {
                // is model valid?
                if (ModelState.IsValid == false)
                {
                    ModelState.AddModelError(string.Empty, "Thông tin đổi mật khẩu không hợp lệ");
                    return View(model);
                }

                // get current user info
                User userInfo = db.Users.Where(u => u.UserId == model.UserId).FirstOrDefault();
                if (userInfo == null)
                {
                    return RedirectToAction("Logout", "User");
                }

                // update user's password
                SaltedHash sh = new SaltedHash(model.Password);
                userInfo.Salt = sh.Salt;
                userInfo.Password = sh.Hash;

                db.SaveChanges();

                return RedirectToAction("Index", "Admin");
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
        public JsonResult Delete(int id = 0)
        {
            /*
             * Delete a user
             */
            try
            {
                // get user info
                User userInfo = db.Users.Where(u => u.UserId == id).FirstOrDefault();
                if (userInfo == null)
                {
                    string errorMessage = "Tài khoản #" + id + " không tồn tại trong hệ thống";
                    return Json(new { Error = 1, Message = errorMessage });
                }

                // remove user
                db.Users.Remove(userInfo);
                db.SaveChanges();

                return Json(new { Error = 0, Message = "Success" });
            }
            catch (ArgumentNullException ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
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
                var users  = db.Users.OrderBy(r => r.Username).AsEnumerable();
                if (searchText.Trim() != "")
                {
                    users = users.Where(r => r.Username.ToLower().Contains(searchText.ToLower()));
                }
                var paging = new PagingHelper(users, pageIndex, AppSettings.DEFAULT_PAGE_SIZE);
                return Json(new
                {
                    Error = 0,
                    RowCount = paging.RowCount,
                    PageIndex = paging.PageIndex,
                    PageSize = paging.PageSize,
                    PageTotal = paging.PageTotal,
                    Users = paging.PagedData
                });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }
    }
}
