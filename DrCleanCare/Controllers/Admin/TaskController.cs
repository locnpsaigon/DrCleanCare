using DrCleanCare.DAL;
using DrCleanCare.DAL.Security;
using DrCleanCare.Helpers;
using DrCleanCare.Models;
using OfficeOpenXml;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace DrCleanCare.Controllers.Admin
{
    public class TaskController : Controller
    {

        private DataContext db = new DataContext();

        //
        // GET: /Task/

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult Add(AddTaskViewModel model)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    // create new task
                    Task task = new Task();
                    task.TaskId = model.TaskId;
                    task.TaskName = model.TaskName;
                    task.Description = model.Description;

                    // save task
                    db.Tasks.Add(task);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                    throw new Exception("Invalid input data!!!");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View(model);
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult Edit(int? id)
        {
            try
            {
                // get task info
                Task task = db.Tasks.Where(t => t.TaskId == id).FirstOrDefault();

                if (task == null)
                {
                    ModelState.AddModelError("", "Task not existed!!!");
                    return View();
                }

                // create task model
                AddTaskViewModel model = new AddTaskViewModel();
                model.TaskId = task.TaskId;
                model.TaskName = task.TaskName;
                model.Description = task.Description;

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
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult Edit(AddTaskViewModel model)
        {
            try
            {
                // validate model state
                if (ModelState.IsValid == false)
                    throw new Exception("Invalid task info model");

                // get current task info
                Task task = db.Tasks.Where(t => t.TaskId == model.TaskId).FirstOrDefault();
                if (task == null)
                    throw new Exception("Task not found!!!");

                // update task info
                task.TaskName = model.TaskName;
                task.Description = model.Description;

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
                // get task info
                Task task = db.Tasks.Where(t => t.TaskId == id).FirstOrDefault();

                if (task == null)
                    throw new Exception("Đầu việc cần xóa không tồn tại hệ thống!!!!");

                db.Tasks.Remove(task);
                db.SaveChanges();

                return Json(new { Error = 0, Message = "Xóa đầu việc thành công!!!" });

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
                var tasks = db.Tasks.OrderBy(t => t.TaskName).AsEnumerable();

                // filter tasks
                searchText = searchText.Trim();
                if (searchText != string.Empty)
                {
                    tasks = tasks.Where(t => t.TaskName.ToLower().Contains(searchText.ToLower())).ToList();
                }

                // paging data
                var pager = new PagingHelper(tasks, pageIndex, AppSettings.DEFAULT_PAGE_SIZE);

                return Json(new
                {
                    Error = 0,
                    RowCount = pager.RowCount,
                    PageIndex = pager.PageIndex,
                    PageSize = pager.PageSize,
                    PageTotal = pager.PageTotal,
                    Tasks = pager.PagedData
                });

            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public JsonResult GetTasksList()
        {
            try
            {
                // get tasks list
                var tasks = db.Tasks.OrderBy(t => t.TaskName).ToList();
                return Json(new { Error = 0, Message = "Success", Tasks = tasks });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult AssignTask(int employeeId = 0)
        {
            try
            {
                // create model 
                AssignTaskViewModel model = new AssignTaskViewModel();
                model.EmployeeId = employeeId;
                model.StartDate = DateTime.Now.ToString("dd/MM/yyyy");
                model.StartHour = 7;
                model.StartMinute = 30;
                model.EndDate = DateTime.Now.ToString("dd/MM/yyyy");
                model.EndHour = 17;
                model.EndMinute = 0;
                model.Note = string.Empty;

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
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult AssignTask(AssignTaskViewModel model)
        {
            DateTime startTime, endTime;

            try
            {
                // validate input data
                if (!ModelState.IsValid)
                    throw new Exception("Invalid task assignment input data!!!");

                if (DateTime.TryParseExact(model.StartDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime) == false)
                    throw new Exception("Fail to convert string to start date!!!");
                startTime += new TimeSpan(model.StartHour, model.StartMinute, 0);

                if (DateTime.TryParseExact(model.EndDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out endTime) == false)
                    throw new Exception("Fail to convert string to end date");
                endTime += new TimeSpan(model.EndHour, model.EndMinute, 0);

                // check employee/task existed
                var employee = db.Employees.Where(e => e.EmployeeId == model.EmployeeId).FirstOrDefault();

                if (employee == null)
                    throw new Exception("Employee is unidentified!!!");

                var task = db.Tasks.Where(t => t.TaskId == model.TaskId).FirstOrDefault();

                if (task == null)
                    throw new Exception("Task is unidentified!!!");

                // create new task assignment
                TaskAssignment ta = new TaskAssignment();
                ta.Date = DateTime.Now;
                ta.TaskId = task.TaskId;
                ta.EmployeeId = employee.EmployeeId;
                ta.StartTime = startTime;
                ta.EndTime = endTime;
                ta.Note = model.Note;

                // save task assignment
                db.TaskAssignments.Add(ta);
                db.SaveChanges();

                return RedirectToAction("AssignTask", "Task", new { employeeId = employee.EmployeeId });
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorMessage", "Admin",
                    new RouteValueDictionary(
                        new { message = ex.Message }));
            }
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult EditAssignment(int id)
        {
            try
            {

                // get task assignment info
                var assignment = db.TaskAssignments.Where(ta => ta.Id == id).FirstOrDefault();
                if (assignment == null)
                    throw new Exception("Unidentified task assignment!!!");

                // create view model
                AssignTaskViewModel model = new AssignTaskViewModel();
                model.Id = assignment.Id;
                model.EmployeeId = assignment.EmployeeId;
                model.TaskId = assignment.TaskId;
                model.StartDate = assignment.StartTime.ToString("dd/MM/yyyy");
                model.StartHour = assignment.StartTime.Hour;
                model.StartMinute = assignment.StartTime.Minute;
                model.EndDate = assignment.EndTime.ToString("dd/MM/yyyy");
                model.EndHour = assignment.EndTime.Hour;
                model.EndMinute = assignment.EndTime.Minute;
                model.Note = assignment.Note;

                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorMessage", "Admin",
                    new RouteValueDictionary(
                        new { message = ex.Message }));
            }
        }

        [Authorize]
        [HttpPost]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult EditAssignment(AssignTaskViewModel model)
        {
            DateTime startTime, endTime;

            try
            {
                // validate model
                if (ModelState.IsValid == false)
                    throw new Exception("Fail to update task assignment. Invalid input data!!!");

                if (DateTime.TryParseExact(model.StartDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime) == false)
                    throw new Exception("Fail to convert string to start date!!!");
                startTime += new TimeSpan(model.StartHour, model.StartMinute, 0);

                if (DateTime.TryParseExact(model.EndDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out endTime) == false)
                    throw new Exception("Fail to convert string to end date");
                endTime += new TimeSpan(model.EndHour, model.EndMinute, 0);

                // employee existed?
                var employee = db.Employees.Where(e => e.EmployeeId == model.EmployeeId).FirstOrDefault();
                if (employee == null)
                    throw new Exception("Employee is unidentified!!!");

                // task existed?
                var task = db.Tasks.Where(t => t.TaskId == model.TaskId).FirstOrDefault();
                if (task == null)
                    throw new Exception("Task is unidentified!!!");

                // get task assignment
                var assignment = db.TaskAssignments.Where(ta => ta.Id == model.Id).FirstOrDefault();
                if (assignment == null)
                    throw new Exception("Unidentified task assignment");

                // update task assignment;
                assignment.Date = DateTime.Now;
                assignment.TaskId = task.TaskId;
                assignment.EmployeeId = employee.EmployeeId;
                assignment.StartTime = startTime;
                assignment.EndTime = endTime;
                assignment.Note = model.Note;

                db.SaveChanges();

                return RedirectToAction("AssignTask", "Task", new { employeeId = employee.EmployeeId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View(model);
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult Report()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public JsonResult DeleteAssignment(int id)
        {
            try
            {
                // find assignment
                var assignment = db.TaskAssignments.Where(ta => ta.Id == id).FirstOrDefault();

                if (assignment == null)
                    return Json(new { Error = 1, Description = "Unidentified task assignment!!!" });

                db.TaskAssignments.Remove(assignment);
                db.SaveChanges();

                return Json(new { Error = 0, Description = "Delete task assignment success." });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 2, Description = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public JsonResult GetTopEmployeeTasks(int employeeId, int top)
        {
            try
            {
                // validate requested data
                Employee employee = db.Employees.Where(e => e.EmployeeId == employeeId).FirstOrDefault();

                if (employee == null)
                    throw new Exception("Unidentified employee!!!");

                var userTasks = (from t1 in db.TaskAssignments
                                 join t2 in db.Tasks on t1.TaskId equals t2.TaskId
                                 where t1.EmployeeId == employee.EmployeeId
                                 orderby t1.EndTime descending
                                 select new
                                 {
                                     t1.Id,
                                     t1.Date,
                                     t1.TaskId,
                                     t2.TaskName,
                                     t1.EmployeeId,
                                     t1.StartTime,
                                     t1.EndTime,
                                     t1.Note
                                 }).Take(top);

                return Json(new { Error = 0, Description = "Success", UserTasks = userTasks });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Description = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public JsonResult SearchEmployeeTasks(string dateFrom, string dateTo, int employeeId, int taskId = 0)
        {
            try
            {
                // get employee info
                Employee employee = db.Employees.Where(e => e.EmployeeId == employeeId).FirstOrDefault();
                if (employee == null)
                    throw new Exception("Fail to get report. Employee not found");

                // get date range
                DateTime d1, d2;
                bool parse1 = DateTime.TryParseExact(dateFrom + " 00:00:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out d1);
                bool parse2 = DateTime.TryParseExact(dateTo + " 00:00:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out d2);
                if (parse1 == false || parse2 == false)
                    throw new Exception("Fail to get report. Wrong date range");

                // query employee tasks by date range
                var userTasks = (from t1 in db.TaskAssignments
                                 join t2 in db.Tasks on t1.TaskId equals t2.TaskId
                                 where t1.EmployeeId == employee.EmployeeId && t1.StartTime >= d1 && t1.EndTime <= d2
                                 orderby t1.StartTime descending
                                 select new
                                 {
                                     t1.Id,
                                     t1.Date,
                                     t1.TaskId,
                                     t2.TaskName,
                                     t1.EmployeeId,
                                     t1.StartTime,
                                     t1.EndTime,
                                     t1.Note
                                 });

                // filter employee tasks by task id
                if (taskId > 0)
                {
                    userTasks = userTasks.Where(t => t.TaskId == taskId);
                }

                return Json(new { Error = 0, Description = "Success", UserTasks = userTasks.ToList() });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Description = ex.Message });
            }
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public JsonResult ExportEmployeeTasksToExcel(string dateFrom, string dateTo, int employeeId, int taskId = 0)
        {
            try
            {
                // get employee info
                Employee employee = db.Employees.Where(e => e.EmployeeId == employeeId).FirstOrDefault();
                if (employee == null)
                    throw new Exception("Fail to get report. Employee not found");

                // get date range
                DateTime d1, d2;
                bool parse1 = DateTime.TryParseExact(dateFrom + " 00:00:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out d1);
                bool parse2 = DateTime.TryParseExact(dateTo + " 00:00:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out d2);
                if (parse1 == false || parse2 == false)
                    throw new Exception("Fail to get report. Wrong date range");

                // query employee tasks by date range
                var userTasks = (from t1 in db.TaskAssignments
                                 join t2 in db.Tasks on t1.TaskId equals t2.TaskId
                                 where t1.EmployeeId == employee.EmployeeId && t1.StartTime >= d1 && t1.EndTime <= d2
                                 orderby t1.StartTime descending
                                 select new
                                 {
                                     t1.Id,
                                     t1.Date,
                                     t1.TaskId,
                                     t2.TaskName,
                                     t1.EmployeeId,
                                     t1.StartTime,
                                     t1.EndTime,
                                     t1.Note
                                 });

                // filter employee tasks by task id
                if (taskId > 0)
                {
                    userTasks = userTasks.Where(t => t.TaskId == taskId);
                }

                var template = new FileInfo(Server.MapPath("~/App_Data/Export_Employee_Tasks_Template.xlsx"));
                using (var package = new ExcelPackage(template))
                {
                    ExcelWorkbook workBook = package.Workbook;
                    if (workBook != null)
                    {
                        if (workBook.Worksheets.Count > 0)
                        {
                            ExcelWorksheet currentWorksheet = workBook.Worksheets.First();

                            var rowIndex = 1; // first row is headers
                            foreach (var item in userTasks.ToList())
                            {
                                rowIndex++;
                                currentWorksheet.Cells[rowIndex, 1].Value = rowIndex - 1;
                                currentWorksheet.Cells[rowIndex, 2].Value = item.TaskName;
                                currentWorksheet.Cells[rowIndex, 3].Value = item.Note;
                                currentWorksheet.Cells[rowIndex, 4].Value = item.StartTime;
                                currentWorksheet.Cells[rowIndex, 5].Value = item.EndTime;
                            }

                            Response.Clear();
                            package.SaveAs(Response.OutputStream);
                            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            var excel_filename = "Export_Employee_" + employee.FullName + "_Tasks_" + d1.ToString("yyyyMMdd") + "_" + d2.ToString("yyyyMMdd") + ".xlsx";
                            Response.AddHeader("content-disposition", "attachment;  filename=" + excel_filename);
                            Response.End();
                        }
                    }
                }

                return Json(new { Error = 0, Description = "Success", UserTasks = userTasks.ToList() });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Description = ex.Message });
            }
        }
    }
}
