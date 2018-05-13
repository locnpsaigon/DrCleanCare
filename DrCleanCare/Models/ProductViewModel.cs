using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using DrCleanCare.DAL;

namespace DrCleanCare.Models
{
    /// <summary>
    /// ProductDetailsViewModel
    /// </summary>
    public class ProductDetailsViewModel
    {
        public Product Product { get; set; }

        public Category Category { get; set; }
    }

    /// <summary>
    /// AddProductViewModel
    /// </summary>
    public class AddProductViewModel
    {
        public AddProductViewModel()
        {
            this.Categories = new List<SelectListItem>();
        }

        public int ProductId { get; set; }

        [Required(ErrorMessage="Vui lòng chọn nhóm sản phẩm")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage="Tên sản phẩm không được rỗng")]
        public string ProductName { get; set; }

        [Required(ErrorMessage="Giới thiệu sản phẩm không được rỗng")]
        [MaxLength(500)]
        public string ProductIntro { get; set; }

        [AllowHtml]
        [Required(ErrorMessage="Mô tả sản phẩm không được rỗng")]
        public string ProductDescription { get; set; }

        [Required(ErrorMessage = "Đơn vị tính")]
        [MaxLength(100)]
        public string QuantityPerUnit { get; set; }

        [Required(ErrorMessage="Giá trước thứ")]
        [DataType(DataType.Currency, ErrorMessage="Giá trước thuế phải là số")]
        public decimal PriceBT { get; set; }

        [Required(ErrorMessage = "Giá sau thuế")]
        [DataType(DataType.Currency, ErrorMessage = "Giá sau thuế phải là số")]
        public decimal Price { get; set; }

        public string IconURL { get; set; }

        public string ImageURL { get; set; }

        public HttpPostedFileBase ProductImage { get; set; }

        public bool Discontinued { get; set; }

        public bool Displayed { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        
    }

    /// <summary>
    /// StockInputViewModel
    /// </summary>
    public class StockInputViewModel
    {
        [Key]
        public int Id { get; set; }

        public string Date { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn sản phẩm nhập kho")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn kho nhập")]
        public int StockId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng nhập kho")]
        public string Quantity { get; set; }

        public string Note { get; set; }

        public string UserName { get; set; }
    }

    public class AddProductInStockViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn sản phẩm")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn kho")]
        public int StockId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        public string Quantity { get; set; }
    }

    public class EditProductInStockViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn sản phẩm")]
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn kho")]

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public int StockId { get; set; }

        public string StockName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        public string Quantity { get; set; }
    }
}