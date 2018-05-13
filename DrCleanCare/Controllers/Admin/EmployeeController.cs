using System;
using System.Globalization;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DrCleanCare.DAL;
using DrCleanCare.DAL.Security;
using DrCleanCare.Models;
using DrCleanCare.Helpers;

namespace DrCleanCare.Controllers.Admin
{
    public class EmployeeController : Controller
    {
        DataContext db = new DataContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add()
        {
            // create model
            AddEmployeeViewModel model = new AddEmployeeViewModel();

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Add(AddEmployeeViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // create new employee instance 
                    Employee employee = new Employee();

                    employee.FullName = model.FullName;
                    employee.DOB = new DateTime(model.DOB_Year, model.DOB_Month, model.DOB_Day);
                    employee.MaritalStatus = model.MaritalStatus;
                    employee.Address = model.Address;
                    employee.Phone = model.Phone;
                    employee.Email = model.Email;
                    employee.HireDate = DateTime.ParseExact(model.HireDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    if (model.QuitDate != null) 
                    {
                        employee.QuitDate = DateTime.ParseExact(model.QuitDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    employee.TaxCode = model.TaxCode;

                    // save new employee info
                    db.Employees.Add(employee);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                // error
                ModelState.AddModelError("", ex.Message);
            }

            return View(model);
        }

        [Authorize]
        public ActionResult Edit(int? id)
        {
            try
            {
                // get employee info
                var emp = db.Employees.Where(e => e.EmployeeId == id).FirstOrDefault();
                if (emp == null)
                {
                    throw new Exception(String.Format("Không tìm thấy thông tin nhân viên mã #{0} trong hệ thống!", id));
                }

                // initialize view model
                AddEmployeeViewModel model = new AddEmployeeViewModel();
                model.EmployeeId = emp.EmployeeId;
                model.FullName = emp.FullName;
                model.DOB_Day = emp.DOB.Day;
                model.DOB_Month = emp.DOB.Month;
                model.DOB_Year = emp.DOB.Year;
                model.MaritalStatus = emp.MaritalStatus;
                model.Address = emp.Address;
                model.Phone = emp.Phone;
                model.Email = emp.Email;
                model.HireDate = emp.HireDate.ToString("dd/MM/yyyy");
                if (emp.QuitDate != null)
                {
                    model.QuitDate = emp.QuitDate.ToString("dd/MM/yyyy");
                }
                else
                {
                    model.QuitDate = string.Empty;
                }
                model.TaxCode = emp.TaxCode;

                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorMessage", "Admin",
                    new RouteValueDictionary(
                        new { message = ex.Message }));
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult Edit(AddEmployeeViewModel model)
        {
            try
            {
                // get current employee info
                var employee = db.Employees.Where(e => e.EmployeeId == model.EmployeeId).FirstOrDefault();

                if (employee == null)
                {
                    ModelState.AddModelError("", "The employee info not existed!!!");
                    return View(model);
                }

                // update employee info
                employee.FullName = model.FullName;
                employee.DOB = new DateTime(model.DOB_Year, model.DOB_Month, model.DOB_Day);
                employee.MaritalStatus = model.MaritalStatus;
                employee.Address = model.Address;
                employee.Phone = model.Phone;
                employee.Email = model.Email;
                employee.HireDate = DateTime.ParseExact(model.HireDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                if (model.QuitDate != null)
                {
                    employee.QuitDate = DateTime.ParseExact(model.QuitDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                employee.TaxCode = model.TaxCode;

                // save employee info
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public JsonResult Delete(int id = 0)
        { 
            try
            {
                // get emp info
                Employee employee = db.Employees.Where(e => e.EmployeeId == id).FirstOrDefault();

                if (employee == null)
                    throw new Exception("Nhân viên cần xóa không tồn tại trong hệ thống!!!");

                db.Employees.Remove(employee);
                db.SaveChanges();

                return Json(new { Error = 0, Message = "Xóa thông tin nhân viên thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public JsonResult GetEmployeesList()
        {
            try
            {
                // get all employees
                var employees = db.Employees.OrderBy(e => e.FullName).ToList();

                return Json(new { Error = 0, Message = "Success", Employees = employees });
            }
            catch (Exception ex)
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
                var employees = db.Employees.OrderBy(r => r.FullName).AsEnumerable();
                if (searchText.Trim() != "")
                {
                    employees = employees.Where(r => r.FullName.ToLower().Contains(searchText.ToLower()));
                }
                var paging = new PagingHelper(employees, pageIndex, AppSettings.DEFAULT_PAGE_SIZE);
                return Json(new
                {
                    Error = 0,
                    RowCount = paging.RowCount,
                    PageIndex = paging.PageIndex,
                    PageSize = paging.PageSize,
                    PageTotal = paging.PageTotal,
                    Employees = paging.PagedData
                });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }
    }
}
