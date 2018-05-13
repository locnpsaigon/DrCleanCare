using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Globalization;
using DrCleanCare.DAL;
using DrCleanCare.DAL.Security;
using DrCleanCare.Models;
using DrCleanCare.Helpers;

namespace DrCleanCare.Controllers.Admin
{
    public class MaterialController : BaseController
    {
        DataContext db = new DataContext();

        #region Materials
        [Authorize]
        [Authorize]
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
        public ActionResult Add(AddMaterialViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // check material existed
                    var isExisted = db.Materials
                        .Where(t => string.Compare(t.MaterialName, model.MaterialName, true) == 0)
                        .FirstOrDefault() != null;
                    if (isExisted)
                    {
                        throw new Exception("Nguyên liệu tên [" + model.MaterialName + "] đã tồn tại trong hệ thống!");
                    }
                    else
                    {
                        // insert new material
                        Material material = new Material();
                        material.MaterialName = model.MaterialName;
                        material.Description = model.Description;
                        material.CreationDate = DateTime.Now;
                        material.QuantityPerUnit = model.QuantityPerUnit;

                        db.Materials.Add(material);
                        db.SaveChanges();

                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View(model);
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult Edit(int id)
        {
            var model = new EditMaterialViewModel();
            try
            {
                var material = db.Materials.Where(t => t.MaterialId == id).FirstOrDefault();
                if (material == null)
                    throw new Exception("Tài nguyên không tồn tại trong hệ thống");

                model.MaterialId = material.MaterialId;
                model.MaterialName = material.MaterialName;
                model.Description = material.Description;
                model.QuantityPerUnit = material.QuantityPerUnit;

                return View(model);
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
        public ActionResult Edit(EditMaterialViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var material = db.Materials.Where(t => t.MaterialId == model.MaterialId).FirstOrDefault();
                    if (material == null)
                        throw new Exception("Nguyên liệu không tồn tại trong hệ thống");

                    // check material name existed
                    var isExisted = db.Materials
                        .Where(t => t.MaterialId != model.MaterialId && string.Compare(t.MaterialName, model.MaterialName, true) == 0)
                        .FirstOrDefault() != null;

                    if (isExisted)
                        throw new Exception("Tên nguyên liệu [" + model.MaterialName + "] đã tồn tại trong hệ thống!");

                    material.MaterialName = model.MaterialName;
                    material.Description = model.Description;
                    material.QuantityPerUnit = model.QuantityPerUnit;

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
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
        public JsonResult Delete(int id)
        {
            try
            {
                var material = db.Materials.Where(t => t.MaterialId == id).FirstOrDefault();
                if (material == null)
                    throw new Exception("Nguyên liệu không tồn tại trong hệ thống");

                db.Materials.Remove(material);
                db.SaveChanges();

                return Json(new { Error = 0, Message = "Xóa nguyên liệu thành công!" });
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
                // get date range
                var materials = db.Database.SqlQuery<SearchMaterialResult>(
                    "EXEC [dbo].[usp_searchMaterials] @SearchText",
                    new SqlParameter("SearchText", searchText.Trim() == "" ? DBNull.Value : (object)searchText.Trim()))
                    .ToList<SearchMaterialResult>();
                var paging = new PagingHelper(materials, pageIndex, AppSettings.DEFAULT_PAGE_SIZE);
                return Json(new
                {
                    Error = 0,
                    RowCount = paging.RowCount,
                    PageIndex = paging.PageIndex,
                    PageSize = paging.PageSize,
                    PageTotal = paging.PageTotal,
                    Materials = paging.PagedData
                });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }

        #endregion

        #region Materials - Stock Managements

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult AddImport()
        {
            try
            {
                // assign view bag values
                var materials = db.Materials.OrderBy(t => t.MaterialName)
                    .AsEnumerable()
                    .Select(t => new SelectListItem
                    {
                        Value = t.MaterialId.ToString(),
                        Text = t.MaterialName
                    })
                    .ToList();
                var stocks = db.Stocks.OrderBy(r => r.StockName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r.StockId.ToString(),
                        Text = r.StockName
                    })
                    .ToList();
                ViewBag.Materials = materials;
                ViewBag.Stocks = stocks;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            if (ViewBag.Materials == null) ViewBag.Materials = new List<SelectListItem>();
            if (ViewBag.Stocks == null) ViewBag.Stocks = new List<SelectListItem>();
            return View();
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult AddImport(AddMaterialImportViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // get quantity 
                    double quantity = 0;
                    if (!double.TryParse(model.Quantity.Replace(",", ""), out quantity))
                        throw new Exception("Số lượng nhập không hợp lệ!");

                    // get price
                    decimal price = 0;
                    if (!decimal.TryParse(model.Price.Replace(",", ""), out price))
                        throw new Exception("Giá nhập không hợp lệ!");

                    // get material info
                    var material = db.Materials.Where(t => t.MaterialId == model.MaterialId).FirstOrDefault();
                    if (material == null)
                        throw new Exception("Không tìm thấy thông tin nguyên liệu");

                    // update quantity 
                    var unitInStock = db.MaterialInStocks.Where(r => r.MaterialId == model.MaterialId && r.StockId == model.StockId).FirstOrDefault();
                    if (unitInStock != null)
                    {
                        unitInStock.Quantity += quantity;
                    }
                    else
                    {
                        var materialInStock = new MaterialInStock();
                        materialInStock.MaterialId = model.MaterialId;
                        materialInStock.StockId = model.StockId;
                        materialInStock.Quantity = quantity;
                        db.MaterialInStocks.Add(materialInStock);
                    }

                    // insert import
                    var import = new MaterialImport();
                    import.MaterialImportDate = DateTime.ParseExact(model.MaterialImportDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None) + 
                        new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    import.MaterialId = model.MaterialId;
                    import.StockId = model.StockId;
                    import.Quantity = quantity;
                    import.Price = price;
                    import.Notes = model.Notes;
                    db.MaterialImports.Add(import);

                    db.SaveChanges();
                    return RedirectToAction("ListImports");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            try
            {
                // assign view bag values
                var materials = db.Materials.OrderBy(t => t.MaterialName)
                    .AsEnumerable()
                    .Select(t => new SelectListItem
                    {
                        Value = t.MaterialId.ToString(),
                        Text = t.MaterialName
                    })
                    .ToList();
                var stocks = db.Stocks.OrderBy(r => r.StockName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r.StockId.ToString(),
                        Text = r.StockName
                    })
                    .ToList();
                ViewBag.Materials = materials;
                ViewBag.Stocks = stocks;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View(model);
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult EditImport(int id)
        {
            var model = new EditMaterialImportViewModel();
            try
            {
                // get import info
                var import = db.MaterialImports.Where(t => t.MaterialImportId == id).FirstOrDefault();
                if (import == null)
                    throw new Exception("Không tìm thấy thông tin nhập nguyên liệu!");

                model.MaterialImportId = import.MaterialImportId;
                model.MaterialImportDate = import.MaterialImportDate.ToString("dd/MM/yyyy");
                model.MaterialId = import.MaterialId;
                model.StockId = import.StockId ?? 0;
                model.Quantity = import.Quantity.ToString("#,###");
                model.Price = import.Price.ToString("#,###");
                model.Notes = import.Notes;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            try
            {
                // assign view bag values
                var materials = db.Materials.OrderBy(t => t.MaterialName)
                    .AsEnumerable()
                    .Select(t => new SelectListItem
                    {
                        Value = t.MaterialId.ToString(),
                        Text = t.MaterialName
                    })
                    .ToList();
                var stocks = db.Stocks.OrderBy(r => r.StockName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r.StockId.ToString(),
                        Text = r.StockName
                    })
                    .ToList();
                ViewBag.Materials = materials;
                ViewBag.Stocks = stocks;
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
        public ActionResult EditImport(EditMaterialImportViewModel model)
        {
            try
            {
                // get import info
                var import = db.MaterialImports.Where(t => t.MaterialImportId == model.MaterialImportId).FirstOrDefault();
                if (import == null)
                    throw new Exception("Không tìm thấy thông tin nhập nguyên liệu!");

                // get material info
                var material = db.Materials.Where(t => t.MaterialId == import.MaterialId).FirstOrDefault();
                if (material == null)
                    throw new Exception("Không tìm thấy thông tin nguyên liệu!");

                // get quantity 
                double quantity = 0;
                if (!double.TryParse(model.Quantity.Replace(",", ""), out quantity))
                    throw new Exception("Số lượng nhập không hợp lệ!");

                // get price
                decimal price = 0;
                if (!decimal.TryParse(model.Price.Replace(",", ""), out price))
                    throw new Exception("Giá nhập không hợp lệ!");

                // update material in stock
                var unitInstock = db.MaterialInStocks.Where(r => r.MaterialId == import.MaterialId && r.StockId == import.StockId).FirstOrDefault();
                if (unitInstock != null)
                {
                    unitInstock.Quantity = unitInstock.Quantity - import.Quantity + quantity;
                }
                else
                {
                    var materialInStock = new MaterialInStock();
                    materialInStock.MaterialId = import.MaterialId;
                    materialInStock.StockId = import.StockId ?? 0;
                    materialInStock.Quantity = 0;
                    db.MaterialInStocks.Add(materialInStock);
                }

                // update import info
                import.MaterialImportDate = DateTime.ParseExact(model.MaterialImportDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None) + 
                    new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                import.MaterialId = model.MaterialId;
                import.StockId = model.StockId;
                import.Quantity = quantity;
                import.Price = (decimal)price;
                import.Notes = model.Notes;

                db.SaveChanges();

                return RedirectToAction("ListImports");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            try
            {
                // assign view bag values
                var materials = db.Materials.OrderBy(t => t.MaterialName)
                    .AsEnumerable()
                    .Select(t => new SelectListItem
                    {
                        Value = t.MaterialId.ToString(),
                        Text = t.MaterialName
                    })
                    .ToList();
                var stocks = db.Stocks.OrderBy(r => r.StockName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r.StockId.ToString(),
                        Text = r.StockName
                    })
                    .ToList();
                ViewBag.Materials = materials;
                ViewBag.Stocks = stocks;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View(model);
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult ListImports()
        {
            try
            {
                // initialize viewbag values 
                var materials = db.Materials.OrderBy(t => t.MaterialName)
                    .AsEnumerable()
                    .Select(t => new SelectListItem
                    {
                        Value = t.MaterialId.ToString(),
                        Text = t.MaterialName
                    })
                    .ToList();
                var stocks = db.Stocks.OrderBy(r => r.StockName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r.StockId.ToString(),
                        Text = r.StockName
                    })
                    .ToList();
                ViewBag.Materials = materials;
                ViewBag.Stocks = stocks;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public JsonResult DeleteImport(int id)
        {
            try
            {
                var import = db.MaterialImports.Where(t => t.MaterialImportId == id).FirstOrDefault();
                if (import != null)
                {
                    // update quantity
                    var unitInstock = db.MaterialInStocks.Where(r => r.MaterialId == import.MaterialId && r.StockId == import.StockId).FirstOrDefault();
                    if (unitInstock != null)
                    {
                        unitInstock.Quantity -= import.Quantity;
                    }
                    db.MaterialImports.Remove(import);
                    db.SaveChanges();
                }
                return Json(new { Error = 0, Message = "Xóa nhập kho nguyên liệu thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public JsonResult SearchImports(string dateFrom, string dateTo, int materialId = 0, int stockId = 0)
        {
            try
            {
                // get date range
                var d1 = DateTime.ParseExact(dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var d2 = DateTime.ParseExact(dateTo + " 23:59:59", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                var imports = db.Database.SqlQuery<SearchMaterialImportResult>(
                    "EXEC [dbo].[usp_searchMaterialImports] @DateFrom, @DateTo, @MaterialId, @StockId",
                    new SqlParameter("DateFrom", d1),
                    new SqlParameter("DateTo", d2),
                    new SqlParameter("MaterialId", materialId),
                    new SqlParameter("StockId", stockId))
                    .ToList<SearchMaterialImportResult>();
                return Json(new { Error = 0, Imports = imports });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult AddExport()
        {
            try
            {
                // assign view bag values
                var materials = db.Materials.OrderBy(t => t.MaterialName)
                    .AsEnumerable()
                    .Select(t => new SelectListItem
                    {
                        Value = t.MaterialId.ToString(),
                        Text = t.MaterialName
                    })
                    .ToList();
                var stocks = db.Stocks.OrderBy(r => r.StockName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r.StockId.ToString(),
                        Text = r.StockName
                    })
                    .ToList();
                ViewBag.Materials = materials;
                ViewBag.Stocks = stocks;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View();
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult AddExport(AddMaterialExportViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // get quantity 
                    double quantity = 0;
                    if (!double.TryParse(model.Quantity.Replace(",", ""), out quantity))
                        throw new Exception("Số lượng nhập không hợp lệ!");

                    // get material info
                    var material = db.Materials.Where(t => t.MaterialId == model.MaterialId).FirstOrDefault();
                    if (material == null)
                        throw new Exception("Không tìm thấy thông tin nguyên liệu");

                    // update unit in stock quantity 
                    var unitInStock = db.MaterialInStocks.Where(r => r.MaterialId == model.MaterialId && r.StockId == model.StockId).FirstOrDefault();
                    if (unitInStock != null)
                    {
                        unitInStock.Quantity -= quantity;
                    }

                    // insert import
                    var export = new MaterialExport();
                    export.MaterialExportDate = DateTime.ParseExact(model.MaterialExportDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None) + 
                        new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second); ;
                    export.MaterialId = model.MaterialId;
                    export.StockId = model.StockId;
                    export.Quantity = quantity;
                    export.Notes = model.Notes;

                    db.MaterialExports.Add(export);
                    db.SaveChanges();

                    return RedirectToAction("ListExports");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            try
            {
                // assign view bag values
                var materials = db.Materials.OrderBy(t => t.MaterialName)
                    .AsEnumerable()
                    .Select(t => new SelectListItem
                    {
                        Value = t.MaterialId.ToString(),
                        Text = t.MaterialName
                    })
                    .ToList();
                var stocks = db.Stocks.OrderBy(r => r.StockName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r.StockId.ToString(),
                        Text = r.StockName
                    })
                    .ToList();
                ViewBag.Materials = materials;
                ViewBag.Stocks = stocks;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View(model);
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult EditExport(int id)
        {
            var model = new EditMaterialExportViewModel();
            try
            {
                // get import info
                var export = db.MaterialExports.Where(t => t.MaterialExportId == id).FirstOrDefault();
                if (export == null)
                    throw new Exception("Không tìm thấy thông tin nhập nguyên liệu!");

                model.MaterialExportId = export.MaterialExportId;
                model.MaterialExportDate = export.MaterialExportDate.ToString("dd/MM/yyyy");
                model.MaterialId = export.MaterialId;
                model.StockId = export.StockId ?? 0;
                model.Quantity = export.Quantity.ToString("#,###");
                model.Notes = export.Notes;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            try
            {
                var materials = db.Materials.OrderBy(t => t.MaterialName)
                    .AsEnumerable()
                    .Select(t => new SelectListItem
                    {
                        Value = t.MaterialId.ToString(),
                        Text = t.MaterialName
                    })
                    .ToList();
                var stocks = db.Stocks.OrderBy(r => r.StockName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r.StockId.ToString(),
                        Text = r.StockName
                    })
                    .ToList();
                ViewBag.Materials = materials;
                ViewBag.Stocks = stocks;
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
        public ActionResult EditExport(EditMaterialExportViewModel model)
        {
            try
            {
                // get import info
                var export = db.MaterialExports.Where(t => t.MaterialExportId == model.MaterialExportId).FirstOrDefault();
                if (export == null)
                    throw new Exception("Không tìm thấy thông tin xuất nguyên liệu!");

                // get material info
                var material = db.Materials.Where(t => t.MaterialId == export.MaterialId).FirstOrDefault();
                if (material == null)
                    throw new Exception("Không tìm thấy thông tin nguyên liệu!");

                // get quantity 
                double quantity = 0;
                if (!double.TryParse(model.Quantity.Replace(",", ""), out quantity))
                    throw new Exception("Số lượng nhập không hợp lệ!");

                // update material unit in stock
                var unitInStock = db.MaterialInStocks.Where(r => r.MaterialId == model.MaterialId && r.StockId == model.StockId).FirstOrDefault();
                if (unitInStock != null)
                {
                    unitInStock.Quantity = unitInStock.Quantity + export.Quantity - quantity;
                }
                else
                {
                    var materialInStock = new MaterialInStock();
                    materialInStock.MaterialId = model.MaterialId;
                    materialInStock.StockId = model.StockId;
                    materialInStock.Quantity = 0;
                    db.MaterialInStocks.Add(materialInStock);
                }
                // update import info
                export.MaterialExportDate = DateTime.ParseExact(model.MaterialExportDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None) +
                    new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                export.MaterialId = model.MaterialId;
                export.Quantity = quantity;
                export.Notes = model.Notes;

                db.SaveChanges();
                return RedirectToAction("ListExports");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            try
            {
                // initialize view bag values
                var materials = db.Materials.OrderBy(t => t.MaterialName)
                    .AsEnumerable()
                    .Select(t => new SelectListItem
                    {
                        Value = t.MaterialId.ToString(),
                        Text = t.MaterialName
                    })
                    .ToList();
                var stocks = db.Stocks.OrderBy(r => r.StockName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r.StockId.ToString(),
                        Text = r.StockName
                    })
                    .ToList();
                ViewBag.Materials = materials;
                ViewBag.Stocks = stocks;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(model);
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult ListExports()
        {
            try
            {
                // assign view bag values
                var materials = db.Materials.OrderBy(t => t.MaterialName)
                    .AsEnumerable()
                    .Select(t => new SelectListItem
                    {
                        Value = t.MaterialId.ToString(),
                        Text = t.MaterialName
                    })
                    .ToList();
                var stocks = db.Stocks.OrderBy(r => r.StockName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r.StockId.ToString(),
                        Text = r.StockName
                    })
                    .ToList();
                ViewBag.Materials = materials;
                ViewBag.Stocks = stocks;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public JsonResult DeleteExport(int id)
        {
            try
            {
                // get import info
                var export = db.MaterialExports.Where(t => t.MaterialExportId == id).FirstOrDefault();
                if (export != null)
                {
                    // get material info
                    var unitInStock = db.MaterialInStocks.Where(r => r.MaterialId == export.MaterialId && r.StockId == export.StockId).FirstOrDefault();
                    if (unitInStock != null)
                    {
                        unitInStock.Quantity += export.Quantity;
                    }
                    db.MaterialExports.Remove(export);
                    db.SaveChanges();
                }
                return Json(new { Error = 0, Message = "Xóa xuất kho nguyên liệu thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public JsonResult SearchExports(string dateFrom, string dateTo, int materialId = 0, int stockId = 0)
        {
            try
            {
                // get date range
                var d1 = DateTime.ParseExact(dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var d2 = DateTime.ParseExact(dateTo + " 23:59:59", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                var exports = db.Database.SqlQuery<SearchMaterialExportResult>(
                    "EXEC [dbo].[usp_searchMaterialExports] @DateFrom, @DateTo, @MaterialId, @StockId",
                    new SqlParameter("DateFrom", d1),
                    new SqlParameter("DateTo", d2),
                    new SqlParameter("MaterialId", materialId),
                    new SqlParameter("StockId", stockId))
                    .ToList<SearchMaterialExportResult>();
                return Json(new { Error = 0, Exports = exports });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult MaterialInStock(int materialId = 0, int stockId = 0)
        {
            try
            {
                // assign view bag values
                var materials = db.Materials.OrderBy(t => t.MaterialName)
                    .AsEnumerable()
                    .Select(t => new SelectListItem
                    {
                        Value = t.MaterialId.ToString(),
                        Text = t.MaterialName,
                        Selected = t.MaterialId == materialId
                    })
                    .ToList();
                var stocks = db.Stocks.OrderBy(r => r.StockName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r.StockId.ToString(),
                        Text = r.StockName,
                        Selected = r.StockId == stockId
                    })
                    .ToList();
                ViewBag.Materials = materials;
                ViewBag.Stocks = stocks;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View();
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public JsonResult SearchMaterialInStock(int materialId = 0, int stockId = 0, int pageIndex = 1)
        {
            try
            {
                // get date range
                var materialInStocks = db.Database.SqlQuery<SearchMaterialInStockResult>(
                    "EXEC [dbo].[usp_searchMaterialInStocks] @MaterialId, @StockId",
                    new SqlParameter("MaterialId", materialId),
                    new SqlParameter("StockId", stockId))
                    .ToList<SearchMaterialInStockResult>();
                var paging = new PagingHelper(materialInStocks, pageIndex, AppSettings.DEFAULT_PAGE_SIZE);
                return Json(new
                {
                    Error = 0,
                    RowCount = paging.RowCount,
                    PageIndex = paging.PageIndex,
                    PageSize = paging.PageSize,
                    PageTotal = paging.PageTotal,
                    MaterialInStocks = paging.PagedData
                });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult AddMaterialInStock()
        {
            try
            {
                // assign view bag values
                var materials = db.Materials.OrderBy(t => t.MaterialName)
                    .AsEnumerable()
                    .Select(t => new SelectListItem
                    {
                        Value = t.MaterialId.ToString(),
                        Text = t.MaterialName
                    })
                    .ToList();
                var stocks = db.Stocks.OrderBy(r => r.StockName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r.StockId.ToString(),
                        Text = r.StockName
                    })
                    .ToList();
                ViewBag.Materials = materials;
                ViewBag.Stocks = stocks;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View();
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult AddMaterialInStock(AddMaterialInStockViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var checkMaterialInStock = db.MaterialInStocks.Where(r => r.MaterialId == model.MaterialId && r.StockId == model.StockId).FirstOrDefault();
                    if (checkMaterialInStock == null)
                    {
                        // validate material and stock existed
                        var material = db.Materials.Where(r => r.MaterialId == model.MaterialId).FirstOrDefault();
                        var stock = db.Stocks.Where(r => r.StockId == model.StockId).FirstOrDefault();
                        if (material != null && stock != null)
                        {
                            var materialInStock = new MaterialInStock();
                            materialInStock.MaterialId = model.MaterialId;
                            materialInStock.StockId = model.StockId;
                            var quantity = 0;
                            if (!int.TryParse(model.Quantity.Replace(",", ""), out quantity))
                                throw new Exception("Số lượng tồn không hợp lệ!");
                            materialInStock.Quantity = quantity;
                            db.MaterialInStocks.Add(materialInStock);
                            db.SaveChanges();
                            return RedirectToAction("MaterialInstock", "Material");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Kho hoặc nguyên liệu không tồn tại!!!");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Nguyên liệu đã có thông tin tồn kho!!!");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
            }

            try
            {
                // assign view bag values
                var materials = db.Materials.OrderBy(t => t.MaterialName)
                    .AsEnumerable()
                    .Select(t => new SelectListItem
                    {
                        Value = t.MaterialId.ToString(),
                        Text = t.MaterialName
                    })
                    .ToList();
                var stocks = db.Stocks.OrderBy(r => r.StockName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r.StockId.ToString(),
                        Text = r.StockName
                    })
                    .ToList();
                ViewBag.Materials = materials;
                ViewBag.Stocks = stocks;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View();
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult EditMaterialInStock(int materialId = 0, int stockId = 0)
        {
            var model = new EditMaterialInStockViewModel();
            try
            {
                // get material in stock
                var materialInStock = db.MaterialInStocks.Where(r => r.MaterialId == materialId && r.StockId == stockId).FirstOrDefault();
                if (materialInStock != null)
                {
                    model.MaterialId = materialInStock.MaterialId;
                    var material = db.Materials.Where(r => r.MaterialId == materialInStock.MaterialId).FirstOrDefault();
                    if (material != null)
                    {
                        model.MaterialName = material.MaterialName;
                    }
                    model.StockId = materialInStock.StockId;
                    var stock = db.Stocks.Where(r => r.StockId == materialInStock.StockId).FirstOrDefault();
                    if (stock != null)
                    {
                        model.StockName = stock.StockName;
                    }
                    model.Quantity = materialInStock.Quantity.ToString();
                }
                else
                {
                    ModelState.AddModelError("", "Không tìm thấy thông tin tồn kho!!!");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
            }

            // assign view bag values
            try
            {
                var materials = db.Materials.OrderBy(t => t.MaterialName)
                    .AsEnumerable()
                    .Select(t => new SelectListItem
                    {
                        Value = t.MaterialId.ToString(),
                        Text = t.MaterialName,
                        Selected = t.MaterialId == materialId
                    })
                    .ToList();
                var stocks = db.Stocks.OrderBy(r => r.StockName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r.StockId.ToString(),
                        Text = r.StockName,
                        Selected = r.StockId == stockId
                    })
                    .ToList();
                ViewBag.Materials = materials;
                ViewBag.Stocks = stocks;
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
        public ActionResult EditMaterialInStock(EditMaterialInStockViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var materialInStock = db.MaterialInStocks.Where(r => r.MaterialId == model.MaterialId && r.StockId == model.StockId).FirstOrDefault();
                    if (materialInStock != null)
                    {
                        // get quantity
                        var quantity = 0;
                        if (!int.TryParse(model.Quantity.Replace(",", ""), out quantity))
                            throw new Exception("Số lượng tồn không hợp lệ!");
                        materialInStock.Quantity = quantity;
                        db.SaveChanges();
                        return RedirectToAction("MaterialInstock", "Material");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Không tìm thấy thông tin tồn kho!!!");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
            }

            // assign view bag values
            try
            {
                var materials = db.Materials.OrderBy(t => t.MaterialName)
                    .AsEnumerable()
                    .Select(t => new SelectListItem
                    {
                        Value = t.MaterialId.ToString(),
                        Text = t.MaterialName,
                        Selected = t.MaterialId == model.MaterialId
                    })
                    .ToList();
                var stocks = db.Stocks.OrderBy(r => r.StockName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r.StockId.ToString(),
                        Text = r.StockName,
                        Selected = r.StockId == model.StockId
                    })
                    .ToList();
                ViewBag.Materials = materials;
                ViewBag.Stocks = stocks;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View();
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public JsonResult DeleteMaterialInStock(int materialId = 0, int stockId = 0)
        {
            try
            {
                var materialInstock = db.MaterialInStocks.Where(r => r.MaterialId == materialId && r.StockId == stockId).FirstOrDefault();
                if (materialInstock != null)
                {
                    db.MaterialInStocks.Remove(materialInstock);
                    db.SaveChanges();
                }
                return Json(new { Error = 0, Message = "Xóa tồn kho nguyên liệu thành công!!!" });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }
        #endregion
    }
}