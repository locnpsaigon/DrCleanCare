using DrCleanCare.DAL;
using DrCleanCare.DAL.Security;
using DrCleanCare.Helpers;
using DrCleanCare.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DrCleanCare.Controllers.Admin
{
    public class StockController : BaseController
    {
        DataContext db = new DataContext();

        //
        // GET: /Category/
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult AddStock()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult AddStock(AddStockViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // check stock name existed
                    var temp = db.Stocks.Where(r => string.Compare(r.StockName, model.StockName, true) == 0).FirstOrDefault();
                    if (temp != null)
                    {
                        ModelState.AddModelError("", "Tên kho đã tồn tại trong hệ thống! Vui lòng chọn ");
                        return View(model);
                    }

                    // add new stock
                    var stock = new Stock();
                    stock.StockName = model.StockName;
                    stock.StockAddress = model.StockAddress;
                    stock.Phone = model.Phone;
                    stock.Description = model.Description;
                    stock.CreationDate = DateTime.Now;
                    db.Stocks.Add(stock);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult EditStock(int id)
        {
            try
            {
                // get stock info
                var stock = db.Stocks.Where(r => r.StockId == id).FirstOrDefault();
                if (stock == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy thông tin kho");
                    return View();
                }

                // create view model
                var model = new EditStockViewModel();
                model.StockId = stock.StockId;
                model.StockName = stock.StockName;
                model.StockAddress = stock.StockAddress;
                model.Phone = stock.Phone;
                model.Description = stock.Description;

                return View(model);
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
        public ActionResult EditStock(EditStockViewModel model)
        {
            if (ModelState.IsValid)
            {
                // check stock name existed
                var stock = db.Stocks.Where(
                    r => string.Compare(r.StockName, model.StockName, true) == 0 &&
                    r.StockId != model.StockId)
                    .FirstOrDefault();
                if (stock == null)
                {
                    var stockEdit = db.Stocks.Where(r => r.StockId == model.StockId).FirstOrDefault();
                    if (stockEdit != null)
                    {
                        // edit stock info
                        stockEdit.StockName = model.StockName;
                        stockEdit.StockAddress = model.StockAddress;
                        stockEdit.Phone = model.Phone;
                        stockEdit.Description = model.Description;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        // edited stock not found
                        ModelState.AddModelError("", "Không tìm thấy thông tin kho");
                    }
                }
                else
                {
                    // duplicated stock name
                    ModelState.AddModelError("", "Tên kho đã tồn tại trong hệ thống, vui lòng chọn tên khác!");
                }
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public JsonResult DeleteStock(int id)
        {
            try
            {
                // check stock is in used
                var check1 = db.StockInputs.Where(r => r.StockId == id).FirstOrDefault();
                var check2 = db.MaterialImports.Where(r => r.StockId == id).FirstOrDefault();
                var check3 = db.MaterialExports.Where(r => r.StockId == id).FirstOrDefault();
                if (check1 == null && check2 == null && check3 == null)
                {
                    var stock = db.Stocks.Where(r => r.StockId == id).FirstOrDefault();
                    if (stock != null)
                    {
                        db.Stocks.Remove(stock);
                        db.SaveChanges();
                    }
                    return Json(new { Error = 0, Message = "Xóa thông tin kho thành công!!!" });
                }
                else
                {
                    return Json(new { Error = 1, Message = "Kho đang sử dụng, xóa thông tin kho thất bại!!!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public JsonResult SearchStock(string searchText = "", int pageIndex = 1)
        {
            try
            {
                // validate input params
                searchText = searchText.Trim();
                var stockList = searchText == string.Empty ?
                    db.Stocks.OrderBy(r => r.StockName).ToList() :
                    db.Stocks.Where(r => r.StockName.Contains(searchText)).ToList();
                var pagingHelper = new PagingHelper(stockList, pageIndex, AppSettings.DEFAULT_PAGE_SIZE);
                return Json(new
                {
                    Error = 0,
                    RowCount = pagingHelper.RowCount,
                    PageIndex = pagingHelper.PageIndex,
                    PageSize = pagingHelper.PageSize,
                    PageTotal = pagingHelper.PageTotal,
                    Stocks = pagingHelper.PagedData
                });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public JsonResult GetStockList()
        {
            try
            {
                // get stock list
                var stockList = db.Stocks.OrderBy(r => r.StockName).ToList();
                return Json(new { Error = 0, Message = "Success", StockList = stockList });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }
    }
}
