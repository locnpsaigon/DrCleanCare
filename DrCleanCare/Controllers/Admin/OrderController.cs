using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Data.SqlClient;
using DrCleanCare.Models;
using DrCleanCare.DAL;
using DrCleanCare.DAL.Security;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;

namespace DrCleanCare.Controllers.Admin
{
    public class OrderController : BaseController
    {

        DataContext db = new DataContext();

        //
        // GET: /Order/

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult Index(string dateFrom, string dateTo, string keyword)
        {
            try
            {
                DateTime d1, d2;

                // try to parse date range
                if (!DateTime.TryParseExact(dateFrom + " 00:00:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out d1))
                {
                    d1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                }

                if (!DateTime.TryParseExact(dateTo + " 23:59:59", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out d2))
                {
                    d2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                }

                // setup view bag data
                ViewBag.DateFrom = d1.ToString("dd/MM/yyyy");
                ViewBag.DateTo = d2.ToString("dd/MM/yyyy");
                ViewBag.Keyword = keyword;

                var searchResult = db.Database.SqlQuery<SearchSaleOrdersResult>(
                    "EXEC [dbo].[usp_searchSaleOrders] @DateFrom, @DateTo, @Keyword",
                    new SqlParameter("DateFrom", d1),
                    new SqlParameter("DateTo", d2),
                    new SqlParameter("Keyword", keyword ?? string.Empty));

                return View(searchResult.ToList());
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
        public ActionResult IndexSimple(string dateFrom, string dateTo, string keyword)
        {
            try
            {
                DateTime d1, d2;
                // Get date range
                if (!DateTime.TryParseExact(dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out d1))
                {
                    DateTime temp = DateTime.Now;
                    d1 = new DateTime(temp.Year, temp.Month, 1);
                }
                if (!DateTime.TryParseExact(dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out d2))
                {
                    DateTime temp = DateTime.Now;
                    d2 = new DateTime(temp.Year, temp.Month, temp.Day, 23, 59, 59);
                }
                else
                {
                    d2.Add(new TimeSpan(23, 59, 59));
                }

                // setup view bag data
                ViewBag.DateFrom = d1.ToString("dd/MM/yyyy");
                ViewBag.DateTo = d2.ToString("dd/MM/yyyy");
                ViewBag.Keyword = keyword;

                var searchResult = db.Database.SqlQuery<SearchSaleOrdersResult>(
                    "EXEC [dbo].[usp_searchSaleOrders] @DateFrom, @DateTo, @Keyword",
                    new SqlParameter("DateFrom", d1),
                    new SqlParameter("DateTo", d2),
                    new SqlParameter("Keyword", keyword ?? string.Empty));

                return View(searchResult.ToList());
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
        public ActionResult ExportToExcel(string dateFrom, string dateTo, string keyword)
        {
            try
            {
                DateTime d1, d2;

                // try to parse date range
                if (!DateTime.TryParseExact(dateFrom + " 00:00:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out d1))
                {
                    d1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                }

                if (!DateTime.TryParseExact(dateTo + " 23:59:59", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out d2))
                {
                    d2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                }

                var searchResult = db.Database.SqlQuery<SearchSaleOrdersResult>(
                    "EXEC [dbo].[usp_searchSaleOrders] @DateFrom, @DateTo, @Keyword",
                    new SqlParameter("DateFrom", d1),
                    new SqlParameter("DateTo", d2),
                    new SqlParameter("Keyword", keyword ?? string.Empty))
                    .ToList();

                var template = new FileInfo(Server.MapPath("~/App_Data/Export_Orders_Template.xlsx"));
                using (var package = new ExcelPackage(template))
                {
                    ExcelWorkbook workBook = package.Workbook;
                    if (workBook != null)
                    {
                        if (workBook.Worksheets.Count > 0)
                        {
                            ExcelWorksheet currentWorksheet = workBook.Worksheets.First();

                            var rowIndex = 1; // first row is headers
                            foreach (var item in searchResult)
                            {
                                rowIndex++;
                                currentWorksheet.Cells[rowIndex, 1].Value = rowIndex - 1;
                                currentWorksheet.Cells[rowIndex, 2].Value = item.OrderDate;
                                currentWorksheet.Cells[rowIndex, 3].Value = item.OrderNo;
                                currentWorksheet.Cells[rowIndex, 4].Value = item.CustomerName;
                                currentWorksheet.Cells[rowIndex, 5].Value = item.CustomerAddress;
                                currentWorksheet.Cells[rowIndex, 6].Value = item.Phone;
                                currentWorksheet.Cells[rowIndex, 7].Value = item.PaymentTypeName;
                                currentWorksheet.Cells[rowIndex, 8].Value = item.AmountBT;
                                currentWorksheet.Cells[rowIndex, 9].Value = item.VAT;
                                currentWorksheet.Cells[rowIndex, 10].Value = item.GrandTotal;
                                currentWorksheet.Cells[rowIndex, 11].Value = item.ShippingCost;
                                currentWorksheet.Cells[rowIndex, 12].Value = item.PaidAmount;
                            }

                            Response.Clear();
                            package.SaveAs(Response.OutputStream);
                            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            var excel_filename = "Export_Orders_" + d1.ToString("yyyyMMdd") + "_" + d2.ToString("yyyyMMdd") + ".xlsx";
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
        public ActionResult Add()
        {
            var model = new AddSOViewModel();

            // fake data
            //model.OrderDate = DateTime.Now.ToString("dd/MM/yyyy");
            //model.OrderNo = "HD000000013";
            //model.OrderType = 2;
            //model.Phone = "0909841682";
            //model.CustomerName = "Nguyễn Phước Lộc";
            //model.CustomerAddress = "113G/14/44 Lạc Long Quân, F.3, Q.11";
            //model.Email = "locnp.saigon@gmail.com";
            //model.TaxCode = "MST0000000012";
            //model.ShippedDate = DateTime.Now.ToString("dd/MM/yyyy");
            //model.ShipName = "A Lộc";
            //model.ShipAddress = "113G/14/44 Lạc Long Quân, F.3, Q.11";

            // generate order types select list items
            var orderTypes = db.OrderTypes.OrderBy(ot => ot.OrderTypeName)
                .AsEnumerable()
                .Select(t => new SelectListItem
                {
                    Text = t.OrderTypeName,
                    Value = t.OrderTypeId.ToString()
                })
                .ToList();

            ViewBag.OrderTypes = orderTypes;

            // generate employees select list items
            var employees = db.Employees.OrderBy(t => t.FullName)
                .AsEnumerable()
                .Select(t => new SelectListItem
                {
                    Text = t.FullName,
                    Value = t.EmployeeId.ToString()
                })
                .ToList();

            ViewBag.Employees = employees;

            // generate payment type select list items
            var paymentTypes = db.PaymentTypes.OrderBy(t => t.PaymentTypeName)
                .AsEnumerable()
                .Select(t => new SelectListItem
                {
                    Value = t.PaymentTypeId.ToString(),
                    Text = t.PaymentTypeName
                })
                .ToList();

            ViewBag.PaymentTypes = paymentTypes;

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Add(AddSOViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var order = db.Orders.Where(r => string.Compare(r.OrderNo, model.OrderNo, true) == 0).FirstOrDefault();
                    if (order == null)
                    {
                        // create so header
                        Order soHeader = new Order();
                        soHeader.OrderNo = model.OrderNo;
                        soHeader.OrderDate = DateTime.ParseExact(model.OrderDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        soHeader.OrderDate += new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                        soHeader.OrderType = model.OrderType;
                        soHeader.CustomerName = String.IsNullOrWhiteSpace(model.CustomerName) ? null : model.CustomerName.Trim();
                        soHeader.CustomerAddress = String.IsNullOrWhiteSpace(model.CustomerAddress) ? null : model.CustomerAddress.Trim();
                        soHeader.TaxCode = String.IsNullOrWhiteSpace(model.TaxCode) ? null : model.TaxCode.Trim();
                        soHeader.Phone = String.IsNullOrWhiteSpace(model.Phone) ? null : model.Phone.Trim();
                        soHeader.Email = String.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim();
                        soHeader.ShipName = String.IsNullOrWhiteSpace(model.ShipName) ? null : model.ShipName.Trim();
                        soHeader.ShipAddress = String.IsNullOrWhiteSpace(model.ShipAddress) ? null : model.ShipAddress.Trim();
                        soHeader.ShippedDate = DateTime.ParseExact(model.ShippedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        soHeader.Owner = User.Identity.Name;
                        soHeader.CreationDate = DateTime.Now;
                        soHeader.EmployeeId = model.EmployeeId;
                        soHeader.PaymentTypeId = model.PaymentTypeId;
                        soHeader.DeliveryName = String.IsNullOrWhiteSpace(model.DeliveryName) ? null : model.DeliveryName.Trim();
                        soHeader.Notes = String.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
                        if (decimal.TryParse(model.ShippingCost.Replace(",", ""), out decimal shippingCost))
                        {
                            soHeader.ShippingCost = shippingCost;
                        }
                        if (decimal.TryParse(model.Discount.Replace(",", ""), out decimal discount))
                        {
                            soHeader.Discount = discount;
                        }

                        // create so lines
                        var soLines = new List<OrderDetails>();
                        foreach (var details in model.OrderDetails)
                        {
                            var line = new OrderDetails();
                            var quantity = (decimal)0;
                            var unitPriceBT = (decimal)0;
                            var unitPrice = (decimal)0;
                            // parse SO lines
                            decimal.TryParse(details.Quantity, out quantity);
                            decimal.TryParse(details.UnitPriceBT, out unitPriceBT);
                            decimal.TryParse(details.UnitPrice, out unitPrice);

                            line.OrderId = 0;
                            line.ProductId = details.ProductId;
                            line.ProductName = details.ProductName;
                            line.StockId = details.StockId;
                            line.Quantity = (int)quantity;
                            line.UnitPriceBT = unitPriceBT;
                            line.UnitPrice = unitPrice;
                            soLines.Add(line);
                        }

                        // validate product quantity
                        var isValidQuantity = true;
                        foreach (var line in soLines)
                        {
                            var productInStock = db.ProductInStocks.Where(r => r.ProductId == line.ProductId && r.StockId == line.StockId).FirstOrDefault();
                            if (productInStock == null || productInStock.Quantity < line.Quantity)
                            {
                                isValidQuantity = false;
                                ModelState.AddModelError("", "Không đủ tồn kho để xuất hàng cho sản phẩm " + line.ProductName + "!!!");
                                break;
                            }
                        }

                        // save so info
                        if (isValidQuantity)
                        {
                            db.Orders.Add(soHeader);
                            db.SaveChanges();
                            foreach (var line in soLines)
                            {
                                // update line order id
                                line.OrderId = soHeader.OrderId;
                                // update product in stock quantity
                                var productInStock = db.ProductInStocks.Where(r => r.ProductId == line.ProductId && r.StockId == line.StockId).FirstOrDefault();
                                if (productInStock != null)
                                {
                                    productInStock.Quantity -= line.Quantity;
                                }
                                db.OrderDetails.Add(line);
                                db.SaveChanges();
                            }
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Số đơn hàng đã được sử dụng. Vui lòng nhập số đơn hàng mới!!!");
                    }
                }
                // initialize viewbag
                var orderTypes = db.OrderTypes.OrderBy(ot => ot.OrderTypeName).AsEnumerable()
                    .Select(t => new SelectListItem
                    {
                        Text = t.OrderTypeName,
                        Value = t.OrderTypeId.ToString()
                    })
                    .ToList();
                ViewBag.OrderTypes = orderTypes;

                var employees = db.Employees.OrderBy(t => t.FullName).AsEnumerable()
                    .Select(t => new SelectListItem
                    {
                        Text = t.FullName,
                        Value = t.EmployeeId.ToString()
                    })
                    .ToList();
                ViewBag.Employees = employees;

                var paymentTypes = db.PaymentTypes.OrderBy(t => t.PaymentTypeName).AsEnumerable()
                    .Select(t => new SelectListItem
                    {
                        Value = t.PaymentTypeId.ToString(),
                        Text = t.PaymentTypeName
                    })
                    .ToList();
                ViewBag.PaymentTypes = paymentTypes;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
            }

            if (ViewBag.OrderTypes == null) ViewBag.OrderTypes = new List<SelectListItem>();
            if (ViewBag.Employees == null) ViewBag.Employees = new List<SelectListItem>();
            if (ViewBag.PaymentTypes == null) ViewBag.PaymentTypes = new List<SelectListItem>();
            return View(model);
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult Edit(int id)
        {
            var model = new AddSOViewModel();
            try
            {
                // get header
                var header = db.Orders.Where(r => r.OrderId == id).FirstOrDefault();
                if (header != null)
                {
                    model.OrderID = header.OrderId;
                    model.OrderDate = header.OrderDate.ToString("dd/MM/yyyy");
                    model.OrderNo = header.OrderNo;
                    model.OrderType = header.OrderType;
                    model.CustomerName = header.CustomerName;
                    model.CustomerAddress = header.CustomerAddress;
                    model.TaxCode = header.TaxCode;
                    model.Phone = header.Phone;
                    model.Email = header.Email;
                    model.ShipName = header.ShipName;
                    model.ShipAddress = header.ShipAddress;
                    model.ShippedDate = header.ShippedDate.ToString("dd/MM/yyyy");
                    model.EmployeeId = header.EmployeeId;
                    model.PaymentTypeId = header.PaymentTypeId;
                    model.DeliveryName = header.DeliveryName;
                    model.Notes = header.Notes;
                    model.ShippingCost = header.ShippingCost.ToString("#,##0");
                    model.Discount = header.Discount.ToString("#,##0");

                    // get lines
                    var lines = (from t1 in db.OrderDetails
                                 join t2 in db.Stocks on t1.StockId equals t2.StockId into j1
                                 from t3 in j1.DefaultIfEmpty()
                                 where t1.OrderId == id
                                 select new
                                 {
                                     Id = t1.OrderId,
                                     OrderId = t1.OrderId,
                                     ProductId = t1.ProductId,
                                     ProductName = t1.ProductName,
                                     StockId = t1.StockId,
                                     StockName = t3.StockName,
                                     UnitPrice = t1.UnitPrice,
                                     UnitPriceBT = t1.UnitPriceBT,
                                     Quantity = t1.Quantity,
                                     Discount = t1.Discount
                                 }).ToList();
                    foreach (var item in lines)
                    {
                        var line = new AddSOLineViewModel();
                        line.ProductId = item.ProductId;
                        line.ProductName = item.ProductName;
                        line.StockId = item.StockId;
                        line.StockName = item.StockName;
                        line.Quantity = item.Quantity.ToString();
                        line.UnitPriceBT = item.UnitPriceBT.ToString();
                        line.UnitPrice = item.UnitPrice.ToString();
                        model.OrderDetails.Add(line);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Không tìm thấy thông tin đơn hàng!!!");
                }

                // initialize viewbag

                // Order Types
                int orderTypeId = 0;
                if (header != null)
                {
                    orderTypeId = header.OrderType;
                }
                var orderTypes = db.OrderTypes.OrderBy(r => r.OrderTypeName).AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Text = r.OrderTypeName,
                        Value = r.OrderTypeId.ToString(),
                        Selected = r.OrderTypeId == orderTypeId
                    }).ToList();
                ViewBag.OrderTypes = orderTypes;

                // Employees
                int employeeId = 0;
                if (header != null && header.EmployeeId != null)
                {
                    employeeId = (int)header.EmployeeId;
                }
                var employees = db.Employees.OrderBy(r => r.FullName).AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Text = r.FullName,
                        Value = r.EmployeeId.ToString(),
                        Selected = r.EmployeeId == employeeId
                    })
                    .ToList();
                ViewBag.Employees = employees;

                // Payment Types
                int paymentTypeId = 0;
                if (header != null && header.PaymentTypeId != null)
                {
                    paymentTypeId = (int)header.PaymentTypeId;
                }
                var paymentTypes = db.PaymentTypes.OrderBy(r => r.PaymentTypeName).AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Text = r.PaymentTypeName,
                        Value = r.PaymentTypeId.ToString(),
                        Selected = r.PaymentTypeId == paymentTypeId
                    })
                    .ToList();
                ViewBag.PaymentTypes = paymentTypes;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
            }
            if (ViewBag.OrderTypes == null) ViewBag.OrderTypes = new List<SelectListItem>();
            if (ViewBag.Employees == null) ViewBag.Employees = new List<SelectListItem>();
            if (ViewBag.PaymentTypes == null) ViewBag.PaymentTypes = new List<SelectListItem>();
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult Edit(AddSOViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // validate order number
                    var header = db.Orders.Where(r => string.Compare(r.OrderNo, model.OrderNo, true) == 0 && r.OrderId != model.OrderID).FirstOrDefault();
                    if (header == null)
                    {
                        // get current header
                        header = db.Orders.Where(r => r.OrderId == model.OrderID).FirstOrDefault();
                        if (header != null)
                        {
                            var isValidQuantiy = true;

                            // get current lines
                            var curentLines = db.OrderDetails.Where(r => r.OrderId == header.OrderId).ToList();

                            // create new lines
                            var newLines = new List<OrderDetails>();
                            foreach (var item in model.OrderDetails)
                            {
                                var details = new OrderDetails();
                                var quantity = (decimal)0;
                                var unitPriceBT = (decimal)0;
                                var unitPrice = (decimal)0;
                                // parse SO lines
                                decimal.TryParse(item.Quantity, out quantity);
                                decimal.TryParse(item.UnitPriceBT, out unitPriceBT);
                                decimal.TryParse(item.UnitPrice, out unitPrice);

                                details.OrderId = header.OrderId;
                                details.ProductId = item.ProductId;
                                details.ProductName = item.ProductName;
                                details.StockId = item.StockId;
                                details.Quantity = (int)quantity;
                                details.UnitPriceBT = unitPriceBT;
                                details.UnitPrice = unitPrice;
                                newLines.Add(details);
                            }

                            // validate product quantity
                            foreach (var item in newLines)
                            {
                                var unitInStock = 0;
                                var productInStock = db.ProductInStocks.Where(r => r.ProductId == item.ProductId && r.StockId == item.StockId).FirstOrDefault();
                                if (productInStock != null)
                                {
                                    unitInStock += productInStock.Quantity;
                                }
                                foreach (var itemCurrent in curentLines)
                                {
                                    if (itemCurrent.ProductId == item.ProductId && itemCurrent.StockId == item.StockId)
                                    {
                                        unitInStock += itemCurrent.Quantity;
                                        break;
                                    }
                                }
                                if (item.Quantity > unitInStock)
                                {
                                    isValidQuantiy = false;
                                    var stockInfo = db.Stocks.Where(r => r.StockId == item.StockId).FirstOrDefault();
                                    if (stockInfo != null)
                                    {
                                        ModelState.AddModelError("", "Sản phẩm [" + item.ProductName + "] không đủ số lượng tồn kho [" + stockInfo.StockName + "]!!!");
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("", "Sản phẩm [" + item.ProductName + "] không đủ số lượng tồn kho!!!");
                                    }

                                    break;
                                }
                            }

                            // update so info
                            if (isValidQuantiy)
                            {
                                // update header info
                                header.OrderNo = model.OrderNo;
                                header.OrderDate = DateTime.ParseExact(model.OrderDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                header.OrderDate += new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                                header.OrderType = model.OrderType;
                                header.CustomerName = String.IsNullOrWhiteSpace(model.CustomerName) ? null : model.CustomerName.Trim();
                                header.CustomerAddress = String.IsNullOrWhiteSpace(model.CustomerAddress) ? null : model.CustomerAddress.Trim();
                                header.TaxCode = String.IsNullOrWhiteSpace(model.TaxCode) ? null : model.TaxCode.Trim();
                                header.Phone = String.IsNullOrWhiteSpace(model.Phone) ? null : model.Phone.Trim();
                                header.Email = String.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim();
                                header.ShipName = String.IsNullOrWhiteSpace(model.ShipName) ? null : model.ShipName.Trim();
                                header.ShipAddress = String.IsNullOrWhiteSpace(model.ShipAddress) ? null : model.ShipAddress.Trim();
                                header.ShippedDate = DateTime.ParseExact(model.ShippedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                header.Owner = User.Identity.Name;
                                header.CreationDate = DateTime.Now;
                                header.EmployeeId = model.EmployeeId;
                                header.PaymentTypeId = model.PaymentTypeId;
                                header.DeliveryName = String.IsNullOrWhiteSpace(model.DeliveryName) ? null : model.DeliveryName.Trim();
                                header.Notes = String.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
                                if (decimal.TryParse(model.ShippingCost.Replace(",", ""), out decimal shippingCost))
                                {
                                    header.ShippingCost = shippingCost;
                                }
                                if (decimal.TryParse(model.Discount.Replace(",", ""), out decimal discount))
                                {
                                    header.Discount = discount;
                                }

                                // remove current lines
                                foreach (var item in curentLines)
                                {
                                    db.OrderDetails.Remove(item);
                                }

                                // add new lines
                                foreach (var item in newLines)
                                {
                                    db.OrderDetails.Add(item);
                                }

                                // update product unit in stock
                                foreach (var item in curentLines)
                                {
                                    var productInStock = db.ProductInStocks.Where(r => r.ProductId == item.ProductId && r.StockId == item.StockId).FirstOrDefault();
                                    if (productInStock != null)
                                    {
                                        productInStock.Quantity += item.Quantity;
                                    }
                                    db.SaveChanges();
                                }
                                foreach (var item in newLines)
                                {
                                    var productInStock = db.ProductInStocks.Where(r => r.ProductId == item.ProductId && r.StockId == item.StockId).FirstOrDefault();
                                    if (productInStock != null)
                                    {
                                        productInStock.Quantity -= item.Quantity;
                                    }
                                }

                                db.SaveChanges();
                                return RedirectToAction("Index");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Không tìm thấy thông tin đơn hàng!!!");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Số đơn hàng " + model.OrderNo + " đã được sử dụng. Vui lòng nhập số đơn hàng mới!!!");
                    }

                    // initialize viewbag
                    // Order Types
                    int orderTypeId = 0;
                    if (header != null)
                    {
                        orderTypeId = header.OrderType;
                    }
                    var orderTypes = db.OrderTypes.OrderBy(r => r.OrderTypeName).AsEnumerable()
                        .Select(r => new SelectListItem
                        {
                            Text = r.OrderTypeName,
                            Value = r.OrderTypeId.ToString(),
                            Selected = r.OrderTypeId == orderTypeId
                        }).ToList();
                    ViewBag.OrderTypes = orderTypes;

                    // Employees
                    int employeeId = 0;
                    if (header != null && header.EmployeeId != null)
                    {
                        employeeId = (int)header.EmployeeId;
                    }
                    var employees = db.Employees.OrderBy(r => r.FullName).AsEnumerable()
                        .Select(r => new SelectListItem
                        {
                            Text = r.FullName,
                            Value = r.EmployeeId.ToString(),
                            Selected = r.EmployeeId == employeeId
                        })
                        .ToList();
                    ViewBag.Employees = employees;

                    // Payment Types
                    int paymentTypeId = 0;
                    if (header != null && header.PaymentTypeId != null)
                    {
                        paymentTypeId = (int)header.PaymentTypeId;
                    }
                    var paymentTypes = db.PaymentTypes.OrderBy(r => r.PaymentTypeName).AsEnumerable()
                        .Select(r => new SelectListItem
                        {
                            Text = r.PaymentTypeName,
                            Value = r.PaymentTypeId.ToString(),
                            Selected = r.PaymentTypeId == paymentTypeId
                        })
                        .ToList();
                    ViewBag.PaymentTypes = paymentTypes;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            if (ViewBag.OrderTypes == null) ViewBag.OrderTypes = new List<SelectListItem>();
            if (ViewBag.Employees == null) ViewBag.Employees = new List<SelectListItem>();
            if (ViewBag.PaymentTypes == null) ViewBag.PaymentTypes = new List<SelectListItem>();
            return View(model);
        }

        [Authorize]
        public ActionResult Details(int id)
        {
            try
            {
                // get so info (header & lines)
                var soHeader = (from t1 in db.Orders
                                join t2 in db.OrderTypes on t1.OrderType equals t2.OrderTypeId into j1
                                from t3 in j1.DefaultIfEmpty()
                                join t4 in db.PaymentTypes on t1.PaymentTypeId equals t4.PaymentTypeId into j2
                                from t5 in j2.DefaultIfEmpty()
                                join t6 in db.Employees on t1.EmployeeId equals t6.EmployeeId into j3
                                from t7 in j3.DefaultIfEmpty()
                                where t1.OrderId == id
                                select new
                                {
                                    OrderId = t1.OrderId,
                                    OrderDate = t1.OrderDate,
                                    OrderNo = t1.OrderNo,
                                    OrderType = t1.OrderType,
                                    OrderTypeName = t3.OrderTypeName,
                                    CustomerName = t1.CustomerName,
                                    CustomerAddress = t1.CustomerAddress,
                                    TaxCode = t1.TaxCode,
                                    Phone = t1.Phone,
                                    Email = t1.Email,
                                    DeliveryName = t1.DeliveryName,
                                    ShipName = t1.ShipName,
                                    ShipAddress = t1.ShipAddress,
                                    ShippedDate = t1.ShippedDate,
                                    ShippingCost = t1.ShippingCost,
                                    Discount = t1.Discount,
                                    Owner = t1.Owner,
                                    CreationDate = t1.CreationDate,
                                    EmployeeId = t1.EmployeeId,
                                    FullName = t7.FullName,
                                    PaymentTypeId = t1.PaymentTypeId,
                                    PaymentTypeName = t5.PaymentTypeName,
                                    Notes = t1.Notes
                                }).FirstOrDefault();

                if (soHeader != null)
                {
                    SODetailsViewModel model = new SODetailsViewModel();
                    // create header view model
                    model.CustomerAddress = soHeader.CustomerAddress;
                    model.CustomerName = soHeader.CustomerName;
                    model.Email = soHeader.Email;
                    model.OrderDate = soHeader.OrderDate.ToString("dd/MM/yyyy");
                    model.OrderID = soHeader.OrderId;
                    model.OrderNo = soHeader.OrderNo;
                    model.OrderTypeId = soHeader.OrderType;
                    model.OrderTypeName = soHeader.OrderTypeName;
                    model.Phone = soHeader.Phone;
                    model.ShipAddress = soHeader.ShipAddress;
                    model.ShipName = soHeader.ShipName;
                    model.ShippedDate = soHeader.ShippedDate.ToString("dd/MM/yyyy");
                    model.ShippingCost = soHeader.ShippingCost.ToString("#,##0");
                    model.Discount = soHeader.Discount.ToString("#,##0");
                    model.TaxCode = soHeader.TaxCode;
                    model.PaymentTypeId = soHeader.PaymentTypeId;
                    model.PaymentTypeName = soHeader.PaymentTypeName;
                    model.EmployeeId = soHeader.EmployeeId;
                    model.FullName = soHeader.FullName;
                    model.DeliveryName = soHeader.DeliveryName;
                    model.Notes = soHeader.Notes;

                    // create lines view model
                    var soLines = (from t1 in db.OrderDetails
                                   join t2 in db.Stocks on t1.StockId equals t2.StockId into j1
                                   from t3 in j1.DefaultIfEmpty()
                                   where t1.OrderId == id
                                   select new
                                   {
                                       Id = t1.Id,
                                       OrderId = t1.OrderId,
                                       ProductId = t1.ProductId,
                                       ProductName = t1.ProductName,
                                       StockId = t1.StockId,
                                       StockName = t3.StockName,
                                       UnitPrice = t1.UnitPrice,
                                       UnitPriceBT = t1.UnitPriceBT,
                                       Quantity = t1.Quantity,
                                       Discount = t1.Discount
                                   }).ToList();

                    foreach (var item in soLines)
                    {
                        AddSOLineViewModel line = new AddSOLineViewModel();
                        line.ProductId = item.ProductId;
                        line.ProductName = item.ProductName;
                        line.StockId = item.StockId;
                        line.StockName = item.StockName;
                        line.Quantity = item.Quantity.ToString("#,##0");
                        line.UnitPrice = item.UnitPrice.ToString("#,##0");
                        line.UnitPriceBT = item.UnitPriceBT.ToString("#,##0");
                        model.OrderDetails.Add(line);
                    }

                    return View(model);
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Admin",
                    new RouteValueDictionary(
                        new { message = string.Format("Đơn hàng #{0} không tồn tại trong hệ thống!", id) }));
                }
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
        public JsonResult Delete(int id = 0)
        {

            try
            {
                // get so info
                var header = db.Orders.Where(r => r.OrderId == id).FirstOrDefault();
                var lines = db.OrderDetails.Where(r => r.OrderId == id).ToList();
                var payments = db.Payments.Where(r => r.OrderID == id).ToList();

                // rollback product quantity
                foreach (var item in lines)
                {
                    var productInStock = db.ProductInStocks.Where(r => r.ProductId == item.ProductId && r.StockId == item.StockId).FirstOrDefault();
                    if (productInStock != null)
                    {
                        productInStock.Quantity += item.Quantity;
                    }
                    db.SaveChanges();
                }

                // remove so info
                db.Orders.Remove(header);
                foreach (var item in lines)
                {
                    db.OrderDetails.Remove(item);
                }
                foreach (var item in payments)
                {
                    db.Payments.Remove(item);
                }
                db.SaveChanges();

                return Json(new { Error = 0, Message = "Xóa thông tin đơn hàng thành công!!!" });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { Error = 2, Message = ex.Message });
            }
        }
    }
}
