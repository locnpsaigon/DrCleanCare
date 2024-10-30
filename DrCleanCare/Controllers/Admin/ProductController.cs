using DrCleanCare.DAL;
using DrCleanCare.DAL.Security;
using DrCleanCare.Helpers;
using DrCleanCare.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace DrCleanCare.Controllers.Admin
{
    public class ProductController : BaseController
    {
        DataContext db = new DataContext();

        #region Products
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                var categories = db.Categories.OrderBy(r => r.CategoryName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Text = r.CategoryName,
                        Value = r.CategoryId.ToString()
                    })
                    .ToList();
                ViewBag.Categories = categories;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
            }
            if (ViewBag.Categories == null) ViewBag.Categories = new List<SelectListItem>();
            return View();
        }

        [Authorize]
        public ActionResult Add()
        {
            /*
             * User access add product form
             */
            try
            {
                // create add product view model
                AddProductViewModel model = new AddProductViewModel();

                // generate category selection list
                model.Categories = db.Categories
                    .AsEnumerable()
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.CategoryName,
                        Selected = false
                    })
                    .ToList();
                model.Discontinued = false;
                model.Displayed = true;

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
        [ValidateInput(false)]
        [Authorize]
        public ActionResult Add(AddProductViewModel model)
        {
            /*
             * User submit add product form
             */
            try
            {
                // validate user input data
                if (ModelState.IsValid == false)
                {
                    ModelState.AddModelError(string.Empty, "Thông tin sản phẩm không hợp lệ");
                    return View(model);
                }

                // get posted product info 
                Product productInfo = new Product();
                productInfo.ProductName = model.ProductName;
                productInfo.ProductIntro = model.ProductIntro;
                productInfo.ProductDescription = model.ProductDescription;
                productInfo.QuantityPerUnit = model.QuantityPerUnit;
                productInfo.PriceBT = model.PriceBT;
                productInfo.Price = model.Price;
                productInfo.CategoryId = model.CategoryId;
                productInfo.Displayed = model.Displayed;

                // get upload image
                bool wasImageUploaded =
                    model.ProductImage != null &&
                    model.ProductImage.ContentLength > 0 &&
                    Helpers.Common.IsImage(model.ProductImage);

                if (wasImageUploaded)
                {
                    Image imageUploaded = Image.FromStream(model.ProductImage.InputStream);

                    // get image upload configurations
                    var uploadPath = ConfigurationManager.AppSettings["PRODUCT_IMAGE_UPLOAD_PATH"];

                    int iconWidth = 0;
                    int.TryParse(ConfigurationManager.AppSettings["PRODUCT_IMAGE_WIDTH_SMALL"], out iconWidth);

                    int imageWidth = 0;
                    int.TryParse(ConfigurationManager.AppSettings["PRODUCT_IMAGE_WIDTH_DEFAULT"], out imageWidth);

                    // scale images
                    Image icon = Helpers.Common.ScaleImage(imageUploaded, iconWidth);
                    Image image = Helpers.Common.ScaleImage(imageUploaded, imageWidth);

                    // save images
                    var iconFileName = Helpers.Common.SaveImage(icon, uploadPath, model.ProductImage.FileName);
                    var imageFileName = Helpers.Common.SaveImage(image, uploadPath, model.ProductImage.FileName);

                    // save image's url
                    productInfo.IconURL = Path.Combine(uploadPath, iconFileName);
                    productInfo.ImageURL = Path.Combine(uploadPath, imageFileName);
                }

                // save product info
                db.Products.Add(productInfo);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (ArgumentNullException ex)
            {
                ModelState.AddModelError(string.Empty, ex.ToString());
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { Error = 1, Description = ex.Message });
            }

            return View(model);
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            /*
             * User access edit product form
             */
            try
            {
                // get edited product info
                Product productInfo = db.Products.Where(p => p.ProductId == id).FirstOrDefault();
                if (productInfo == null)
                {
                    return RedirectToAction("ProductNotFound", "Error");
                }

                // create product update view model
                AddProductViewModel model = new AddProductViewModel();

                model.ProductId = productInfo.ProductId;
                model.ProductName = productInfo.ProductName;
                model.ProductIntro = productInfo.ProductIntro;
                model.QuantityPerUnit = productInfo.QuantityPerUnit;
                model.PriceBT = productInfo.PriceBT;
                model.Price = productInfo.Price;
                model.ProductDescription = productInfo.ProductDescription;
                model.IconURL = productInfo.IconURL;
                model.ImageURL = productInfo.ImageURL;
                model.CategoryId = productInfo.CategoryId;
                model.Displayed = productInfo.Displayed;

                // Try to get product categories
                model.Categories = db.Categories.ToList()
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.CategoryName,
                        Selected = (c.CategoryId == productInfo.CategoryId)
                    })
                    .ToList();

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
        [ValidateInput(false)]
        [Authorize]
        public ActionResult Edit(AddProductViewModel model)
        {
            /*
             * User submit edit product form
             */
            try
            {
                // get current product info
                Product productInfo = db.Products.Where(p => p.ProductId == model.ProductId).FirstOrDefault();
                if (productInfo == null)
                {
                    string errorMessage = "Sản phẩm #" + model.ProductId + " không tồn tại trong hệ thống";
                    return RedirectToAction("ErrorMessage", "Admin", new { message = errorMessage });
                }

                // update product info
                if (ModelState.IsValid)
                {
                    productInfo.ProductName = model.ProductName;
                    productInfo.ProductIntro = model.ProductIntro;
                    productInfo.QuantityPerUnit = model.QuantityPerUnit;
                    productInfo.PriceBT = model.PriceBT;
                    productInfo.Price = model.Price;
                    productInfo.ProductDescription = model.ProductDescription;
                    productInfo.CategoryId = model.CategoryId;
                    productInfo.Displayed = model.Displayed;

                    // get upload image
                    bool wasImageUpload =
                        model.ProductImage != null &&
                        model.ProductImage.ContentLength > 0 &&
                        Helpers.Common.IsImage(model.ProductImage);

                    if (wasImageUpload)
                    {
                        Image imageUploaded = Image.FromStream(model.ProductImage.InputStream);

                        // get image upload configurations
                        var uploadPath = ConfigurationManager.AppSettings["PRODUCT_IMAGE_UPLOAD_PATH"];

                        int iconWidth = 0;
                        int.TryParse(ConfigurationManager.AppSettings["PRODUCT_IMAGE_WIDTH_SMALL"], out iconWidth);

                        int imageWidth = 0;
                        int.TryParse(ConfigurationManager.AppSettings["PRODUCT_IMAGE_WIDTH_DEFAULT"], out imageWidth);

                        // scale images
                        Image icon = Helpers.Common.ScaleImage(imageUploaded, iconWidth);
                        Image image = Helpers.Common.ScaleImage(imageUploaded, imageWidth);

                        // save images
                        var iconFileName = Helpers.Common.SaveImage(icon, uploadPath, model.ProductImage.FileName);
                        var imageFileName = Helpers.Common.SaveImage(image, uploadPath, model.ProductImage.FileName);

                        // save image's url
                        productInfo.IconURL = Path.Combine(uploadPath, iconFileName);
                        productInfo.ImageURL = Path.Combine(uploadPath, imageFileName);
                    }

                    // save product info
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {

                    //model.Categories = db.Categories.ToList()
                    //    .Select(c => new SelectListItem
                    //    {
                    //        Value = c.CategoryId.ToString(),
                    //        Text = c.CategoryName,
                    //        Selected = (c.CategoryId == productInfo.CategoryId)

                    //    });

                    return View(model);
                }
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
             * User delete a product info
             */
            try
            {
                // get deleted product info
                Product productInfo = db.Products.Where(p => p.ProductId == id).FirstOrDefault();
                if (productInfo == null)
                {
                    string errorMessage = "Thông tin sản phẩm #" + id + " không tồn tại trong hệ thống";
                    return Json(new { Error = 1, Description = errorMessage });
                }

                // delete product
                db.Products.Remove(productInfo);
                db.SaveChanges();

                return Json(new { Error = 1, Description = "Success" });
            }
            catch (ArgumentNullException ex)
            {
                return Json(new { Error = 1, Description = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { Error = 1, Description = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public JsonResult GetProducts()
        {
            try
            {
                // get products
                var products = db.Products.OrderBy(p => p.ProductName).ToList();
                return Json(new { Error = 0, Message = "success", Products = products });
            }
            catch (ArgumentNullException ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetProductsByCategory(int categoryId)
        {
            try
            {
                // get products by category
                var products = db.Products.Where(p => p.CategoryId == categoryId)
                                    .OrderBy(p => p.ProductName)
                                    .ToList();
                return Json(new { Error = 0, Message = "success", Products = products });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public JsonResult Search(int categoryId = 0, string searchText = "", int pageIndex = 1)
        {
            try
            {
                // get date range
                var products = db.Database.SqlQuery<SearchProductsResult>(
                    "EXEC [dbo].[usp_searchProducts] @CategoryId, @SearchText",
                    new SqlParameter("CategoryId", categoryId),
                    new SqlParameter("SearchText", searchText.Trim() == "" ? DBNull.Value : (object)searchText.Trim()))
                    .ToList<SearchProductsResult>();
                var paging = new PagingHelper(products, pageIndex, AppSettings.DEFAULT_PAGE_SIZE);
                return Json(new
                {
                    Error = 0,
                    RowCount = paging.RowCount,
                    PageIndex = paging.PageIndex,
                    PageSize = paging.PageSize,
                    PageTotal = paging.PageTotal,
                    Products = paging.PagedData
                });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }

        public JsonResult SearchDisplayedProducts(int categoryId = 0, string searchText = "", int pageIndex = 1)
        {
            try
            {
                // search products
                var products = db.Products.Where(p => p.Displayed == true).AsEnumerable();
                if (categoryId > 0)
                {
                    products = products.Where(p => p.CategoryId == categoryId).AsEnumerable();
                }
                if (string.IsNullOrWhiteSpace(searchText) == false)
                {
                    products = products.Where(p => p.ProductName.Contains(searchText)).AsEnumerable();
                }
                var paging = new PagingHelper(products, pageIndex, AppSettings.DEFAULT_PAGE_SIZE);
                return Json(new
                {
                    Error = 0,
                    RowCount = paging.RowCount,
                    PageIndex = paging.PageIndex,
                    PageSize = paging.PageSize,
                    PageTotal = paging.PageTotal,
                    Products = paging.PagedData
                });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }

        #endregion

        #region Products - Stock Managements

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult AddStockInput(int productId = 0)
        {
            StockInputViewModel model = new StockInputViewModel();
            try
            {
                model.Date = DateTime.Now.ToString("dd/MM/yyyy");
                model.Quantity = "100";

                var product = db.Products.Where(r => r.ProductId == productId).FirstOrDefault();
                var categoryId = product != null ? product.CategoryId : 0;
                // initialize viewbag
                // categories
                var categories = db.Categories.OrderBy(r => r.CategoryName)
                        .AsEnumerable()
                        .Select(r => new SelectListItem
                        {
                            Text = r.CategoryName,
                            Value = r.CategoryId.ToString(),
                            Selected = r.CategoryId == categoryId
                        })
                        .ToList();
                ViewBag.Categories = categories;
                // products
                var products = db.Products.Where(r => r.CategoryId == categoryId)
                    .OrderBy(r => r.ProductName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Text = r.ProductName,
                        Value = r.ProductId.ToString(),
                        Selected = r.ProductId == productId
                    })
                    .ToList();
                ViewBag.Products = products;
                // stocks
                var stocks = db.Stocks.OrderBy(r => r.StockName).AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Text = r.StockName,
                        Value = r.StockId.ToString()
                    })
                    .ToList();
                ViewBag.Stocks = stocks;

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
            }

            if (ViewBag.Products == null) ViewBag.Products = new List<SelectListItem>();
            if (ViewBag.Categories == null) ViewBag.Categories = new List<SelectListItem>();
            if (ViewBag.Stocks == null) ViewBag.Stocks = new List<SelectListItem>();
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult AddStockInput(StockInputViewModel model)
        {
            try
            {
                var product = db.Products.Where(r => r.ProductId == model.ProductId).FirstOrDefault();
                var stock = db.Stocks.Where(r => r.StockId == model.StockId).FirstOrDefault();

                if (ModelState.IsValid)
                {
                    // validate date product, stock
                    if (product != null && stock != null)
                    {
                        var quantity = int.Parse(model.Quantity.Replace(",", ""));
                        var importDate = DateTime.ParseExact(model.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None) +
                            new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                        // update quantity
                        var productInStock = db.ProductInStocks.Where(r => r.ProductId == model.ProductId && r.StockId == model.StockId).FirstOrDefault();
                        if (productInStock != null)
                        {
                            productInStock.Quantity += quantity;
                        }
                        else
                        {
                            productInStock = new ProductInStock();
                            productInStock.ProductId = model.ProductId;
                            productInStock.StockId = model.StockId;
                            productInStock.Quantity += quantity;
                            db.ProductInStocks.Add(productInStock);
                        }

                        // add product stock input
                        var stockInput = new StockInput();
                        stockInput.Date = importDate;
                        stockInput.ProductId = model.ProductId;
                        stockInput.StockId = model.StockId;
                        stockInput.Quantity = quantity;
                        stockInput.Note = model.Note;
                        stockInput.UserName = User.Identity.Name;
                        db.StockInputs.Add(stockInput);

                        db.SaveChanges();

                        return RedirectToAction("StockInputHistory");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Sản phẩm hoặc kho không tồn tại!!!");
                    }
                }

                // initialize viewbag
                var productId = product != null ? product.ProductId : 0;
                var categoryId = product != null ? product.CategoryId : 0;
                var stockId = stock != null ? stock.StockId : 0;

                var categories = db.Categories.OrderBy(r => r.CategoryName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Text = r.CategoryName,
                        Value = r.CategoryId.ToString(),
                        Selected = r.CategoryId == categoryId
                    })
                    .ToList();
                ViewBag.Categories = categories;

                var products = db.Products.Where(r => r.CategoryId == categoryId)
                    .OrderBy(r => r.ProductName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Text = r.ProductName,
                        Value = r.ProductId.ToString(),
                        Selected = r.ProductId == productId
                    })
                    .ToList();
                ViewBag.Products = products;

                var stocks = db.Stocks.OrderBy(r => r.StockName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Text = r.StockName,
                        Value = r.StockId.ToString(),
                        Selected = r.StockId == stockId
                    })
                    .ToList();
                ViewBag.Stocks = stocks;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
            }
            if (ViewBag.Products == null) ViewBag.Products = new List<SelectListItem>();
            if (ViewBag.Categories == null) ViewBag.Categories = new List<SelectListItem>();
            if (ViewBag.Stocks == null) ViewBag.Stocks = new List<SelectListItem>();
            return View(model);
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult EditStockInput(int id)
        {
            StockInputViewModel model = new StockInputViewModel();
            try
            {
                // get stock input
                var stockInput = db.StockInputs.Where(r => r.Id == id).FirstOrDefault();
                if (stockInput != null)
                {
                    model.Id = stockInput.Id;
                    model.Date = stockInput.Date.ToString("dd/MM/yyyy");
                    model.ProductId = stockInput.ProductId;
                    model.StockId = stockInput.StockId;
                    model.Quantity = stockInput.Quantity.ToString("#,###");
                    model.Note = stockInput.Note;
                }
                else
                {
                    ModelState.AddModelError("", "Không tìm thấy thông tin nhập kho!!!");
                }

                // initalize viewbag
                int productId = stockInput != null ? stockInput.ProductId : 0;
                var product = db.Products.Where(r => r.ProductId == productId).FirstOrDefault();

                var categoryId = product != null ? product.CategoryId : 0;
                var categories = db.Categories.OrderBy(r => r.CategoryName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Text = r.CategoryName,
                        Value = r.CategoryId.ToString(),
                        Selected = r.CategoryId == categoryId
                    })
                    .ToList();
                ViewBag.Categories = categories;

                var products = db.Products.Where(r => r.CategoryId == categoryId)
                    .OrderBy(r => r.ProductName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Text = r.ProductName,
                        Value = r.ProductId.ToString(),
                        Selected = r.ProductId == productId
                    })
                    .ToList();
                ViewBag.Products = products;

                var stocks = db.Stocks.OrderBy(r => r.StockName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Text = r.StockName,
                        Value = r.StockId.ToString()
                    })
                    .ToList();
                ViewBag.Stocks = stocks;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
            }
            if (ViewBag.Categories == null) ViewBag.Categories = new List<SelectListItem>();
            if (ViewBag.Products == null) ViewBag.Products = new List<SelectListItem>();
            if (ViewBag.Stocks == null) ViewBag.Stocks = new List<SelectListItem>();
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult EditStockInput(StockInputViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // get current stock input
                    var stockInput = db.StockInputs.Where(r => r.Id == model.Id).FirstOrDefault();
                    if (stockInput != null)
                    {
                        // remove current stock input
                        db.StockInputs.Remove(stockInput);

                        // rollback stock input product quantity
                        var productInStock = db.ProductInStocks.Where(r => r.ProductId == stockInput.ProductId && r.StockId == stockInput.StockId).FirstOrDefault();
                        if (productInStock != null)
                        {
                            productInStock.Quantity -= stockInput.Quantity;
                        }
                        db.SaveChanges();

                        // add new stock input
                        var quantity = int.Parse(model.Quantity.Replace(",", ""));
                        var importDate = DateTime.ParseExact(model.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None) +
                            new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                        var stockInput2 = new StockInput();
                        stockInput2.Date = importDate;
                        stockInput2.ProductId = model.ProductId;
                        stockInput2.StockId = model.StockId;
                        stockInput2.Quantity = quantity;
                        stockInput2.Note = model.Note;
                        stockInput2.UserName = User.Identity.Name;
                        db.StockInputs.Add(stockInput2);

                        // update new stock input product quantity
                        var productInStock2 = db.ProductInStocks.Where(r => r.ProductId == model.ProductId && r.StockId == model.StockId).FirstOrDefault();
                        if (productInStock2 != null)
                        {
                            productInStock2.Quantity += quantity;
                        }
                        else
                        {
                            productInStock2 = new ProductInStock();
                            productInStock2.ProductId = model.ProductId;
                            productInStock2.StockId = model.StockId;
                            productInStock2.Quantity = quantity;
                            db.ProductInStocks.Add(productInStock2);
                        }
                        db.SaveChanges();

                        return RedirectToAction("StockInputHistory");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Không tìm thấy thông tin nhập kho sản phẩm!!!");
                    }

                    // initialize viewbag
                    var product3 = db.Products.Where(r => r.ProductId == model.ProductId).FirstOrDefault();
                    var categoryId = product3 != null ? product3.CategoryId : 0;
                    var categories = db.Categories.OrderBy(r => r.CategoryName)
                        .AsEnumerable()
                        .Select(r => new SelectListItem
                        {
                            Text = r.CategoryName,
                            Value = r.CategoryId.ToString(),
                            Selected = r.CategoryId == categoryId
                        })
                        .ToList();
                    ViewBag.Categories = categories;

                    int productId = product3 != null ? product3.ProductId : 0;
                    var products = db.Products.Where(r => r.CategoryId == categoryId)
                        .OrderBy(r => r.ProductName)
                        .AsEnumerable()
                        .Select(r => new SelectListItem
                        {
                            Text = r.ProductName,
                            Value = r.ProductId.ToString(),
                            Selected = r.ProductId == productId
                        })
                        .ToList();
                    ViewBag.Products = products;

                    var stocks = db.Stocks.OrderBy(r => r.StockName)
                        .AsEnumerable()
                        .Select(r => new SelectListItem
                        {
                            Text = r.StockName,
                            Value = r.StockId.ToString(),
                            Selected = r.StockId == model.StockId
                        })
                        .ToList();
                    ViewBag.Stocks = stocks;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
            }
            if (ViewBag.Products == null) ViewBag.Products = new List<SelectListItem>();
            if (ViewBag.Categories == null) ViewBag.Categories = new List<SelectListItem>();
            if (ViewBag.Stocks == null) ViewBag.Stocks = new List<SelectListItem>();
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public JsonResult DeleteStockInput(int id)
        {
            try
            {
                var stockInput = db.StockInputs.Where(r => r.Id == id).FirstOrDefault();
                if (stockInput != null)
                {
                    // update quantity
                    var productInStock = db.ProductInStocks.Where(r => r.ProductId == stockInput.ProductId && r.StockId == stockInput.StockId).FirstOrDefault();
                    if (productInStock != null)
                    {
                        productInStock.Quantity -= stockInput.Quantity;
                    }
                    db.StockInputs.Remove(stockInput);
                    db.SaveChanges();
                }
                return Json(new { Error = 0, Message = "Xóa nhập kho thành công!!!" });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult StockInputHistory()
        {
            try
            {
                var stocks = db.Stocks.OrderBy(r => r.StockName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r.StockId.ToString(),
                        Text = r.StockName
                    })
                    .ToList();
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
        public JsonResult GetStockInputsHistory(string dateFrom, string dateTo, int categoryId = 0, int productId = 0, int stockId = 0)
        {
            try
            {
                // get date range
                var date1 = DateTime.ParseExact(dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var date2 = DateTime.ParseExact(dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                // validate date range
                if ((date2 - date1).TotalDays > 730)
                    throw new Exception("Date range to report must be less than 2 years!!!");

                var stockInputs = db.Database.SqlQuery<SearchStockInputResult>(
                   "EXEC [dbo].[usp_searchStockInputs] @DateFrom, @DateTo, @CategoryId, @ProductId, @StockId",
                   new SqlParameter("DateFrom", date1),
                   new SqlParameter("DateTo", date2),
                   new SqlParameter("CategoryId", categoryId),
                   new SqlParameter("ProductId", productId),
                   new SqlParameter("StockId", stockId))
                   .ToList<SearchStockInputResult>();

                // return
                return Json(new { Error = 0, Message = "Success", StockInputs = stockInputs });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult ProductInStock(int productId = 0, int stockId = 0)
        {
            try
            {
                var categoryId = 0;
                var product = db.Products.Where(r => r.ProductId == productId).FirstOrDefault();
                if (product != null)
                {
                    categoryId = product.CategoryId;
                }
                var categories = db.Categories.OrderBy(r => r.CategoryName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Text = r.CategoryName,
                        Value = r.CategoryId.ToString(),
                        Selected = r.CategoryId == categoryId
                    })
                    .ToList();
                ViewBag.Categories = categories;
                var products = db.Products.Where(r => r.CategoryId == categoryId).OrderBy(r => r.ProductName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Text = r.ProductName,
                        Value = r.ProductId.ToString(),
                        Selected = r.ProductId == productId
                    })
                    .ToList();
                ViewBag.Products = products;
                var stocks = db.Stocks.OrderBy(r => r.StockName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r.StockId.ToString(),
                        Text = r.StockName,
                        Selected = r.StockId == stockId
                    })
                    .ToList();
                ViewBag.Stocks = stocks;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            if (ViewBag.Categories == null) ViewBag.Categories = new List<SelectListItem>();
            if (ViewBag.Products == null) ViewBag.Products = new List<SelectListItem>();
            if (ViewBag.Stocks == null) ViewBag.Stocks = new List<SelectListItem>();
            return View();
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public JsonResult SearchProductInStock(int categoryId = 0, int productId = 0, int stockId = 0, int pageIndex = 1)
        {
            try
            {
                // get date range
                var productInStocks = db.Database.SqlQuery<SearchProductInStockResult>(
                    "EXEC [dbo].[usp_searchProductInStocks] @CategoryId, @ProductId, @StockId",
                    new SqlParameter("CategoryId", categoryId),
                    new SqlParameter("ProductId", productId),
                    new SqlParameter("StockId", stockId))
                    .ToList<SearchProductInStockResult>();
                var paging = new PagingHelper(productInStocks, pageIndex, AppSettings.DEFAULT_PAGE_SIZE);
                return Json(new
                {
                    Error = 0,
                    RowCount = paging.RowCount,
                    PageIndex = paging.PageIndex,
                    PageSize = paging.PageSize,
                    PageTotal = paging.PageTotal,
                    ProductInStocks = paging.PagedData
                });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult AddProductInStock()
        {
            try
            {
                // assign view bag values
                var categories = db.Categories.OrderBy(t => t.CategoryName)
                    .AsEnumerable()
                    .Select(t => new SelectListItem
                    {
                        Value = t.CategoryId.ToString(),
                        Text = t.CategoryName
                    })
                    .ToList();
                ViewBag.Categories = categories;
                var stocks = db.Stocks.OrderBy(r => r.StockName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r.StockId.ToString(),
                        Text = r.StockName
                    })
                    .ToList();
                ViewBag.Stocks = stocks;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            if (ViewBag.Categories == null) ViewBag.Categories = new List<SelectListItem>();
            if (ViewBag.Products == null) ViewBag.Products = new List<SelectListItem>();
            if (ViewBag.Stocks == null) ViewBag.Stocks = new List<SelectListItem>();
            return View();
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult AddProductInStock(AddProductInStockViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var checkProductInStock = db.ProductInStocks.Where(r => r.ProductId == model.ProductId && r.StockId == model.StockId).FirstOrDefault();
                    if (checkProductInStock == null)
                    {
                        // validate material and stock existed
                        var product = db.Products.Where(r => r.ProductId == model.ProductId).FirstOrDefault();
                        var stock = db.Stocks.Where(r => r.StockId == model.StockId).FirstOrDefault();
                        if (product != null && stock != null)
                        {
                            var productInStock = new ProductInStock();
                            productInStock.ProductId = model.ProductId;
                            productInStock.StockId = model.StockId;
                            var quantity = 0;
                            if (!int.TryParse(model.Quantity.Replace(",", ""), out quantity))
                                throw new Exception("Số lượng tồn không hợp lệ!");
                            productInStock.Quantity = quantity;
                            db.ProductInStocks.Add(productInStock);
                            db.SaveChanges();
                            return RedirectToAction("ProductInstock", "Product");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Kho hoặc sản phẩm không tồn tại!!!");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Sản phẩm đã có thông tin tồn kho!!!");
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
                var categories = db.Categories.OrderBy(t => t.CategoryName)
                    .AsEnumerable()
                    .Select(t => new SelectListItem
                    {
                        Value = t.CategoryId.ToString(),
                        Text = t.CategoryName
                    })
                    .ToList();
                ViewBag.Categories = categories;
                var stocks = db.Stocks.OrderBy(r => r.StockName)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r.StockId.ToString(),
                        Text = r.StockName
                    })
                    .ToList();
                ViewBag.Stocks = stocks;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            if (ViewBag.Categories == null) ViewBag.Categories = new List<SelectListItem>();
            if (ViewBag.Products == null) ViewBag.Products = new List<SelectListItem>();
            if (ViewBag.Stocks == null) ViewBag.Stocks = new List<SelectListItem>();
            return View(model);
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult EditProductInStock(int productId = 0, int stockId = 0)
        {
            var model = new EditProductInStockViewModel();
            try
            {
                // get material in stock
                var productInStock = db.ProductInStocks.Where(r => r.ProductId == productId && r.StockId == stockId).FirstOrDefault();
                if (productInStock != null)
                {
                    model.ProductId = productInStock.ProductId;
                    model.Quantity = productInStock.Quantity.ToString();
                    var product = db.Products.Where(r => r.ProductId == model.ProductId).FirstOrDefault();
                    if (product != null)
                    {
                        model.ProductName = product.ProductName;
                        var category = db.Categories.Where(r => r.CategoryId == product.CategoryId).FirstOrDefault();
                        if (category != null)
                        {
                            model.CategoryId = category.CategoryId;
                            model.CategoryName = category.CategoryName;
                        }
                        var stock = db.Stocks.Where(r => r.StockId == productInStock.StockId).FirstOrDefault();
                        if (stock != null)
                        {
                            model.StockId = stock.StockId;
                            model.StockName = stock.StockName;
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Không tìm thấy sản phẩm!!!");
                    }
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
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult EditProductInStock(EditProductInStockViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var productInStock = db.ProductInStocks.Where(r => r.ProductId == model.ProductId && r.StockId == model.StockId).FirstOrDefault();
                    if (productInStock != null)
                    {
                        int quantity = 0;
                        if (!int.TryParse(model.Quantity.Replace(",", ""), out quantity))
                            throw new Exception("Số lượng tồn không hợp lệ!");
                        productInStock.Quantity = quantity;
                        db.SaveChanges();
                        return RedirectToAction("ProductInStock", "Product");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Không tìm thấy thông tin tồn kho sản phẩm!!!");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public JsonResult DeleteProductInStock(int productId = 0, int stockId = 0)
        {
            try
            {
                var productInStock = db.ProductInStocks.Where(r => r.ProductId == productId && r.StockId == stockId).FirstOrDefault();
                if (productInStock != null)
                {
                    db.ProductInStocks.Remove(productInStock);
                    db.SaveChanges();
                }
                return Json(new { Error = 0, Message = "Xóa tồn kho sản phẩm thành công!!!" });
            }
            catch (Exception ex)
            {
                return Json(new { Error = 1, Message = ex.Message });
            }
        }

        #endregion
    }
}
