﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.Models
{

    /// <summary>
    /// OrderTypes
    /// </summary>
    public enum OrderTypes
    {
        VAT = 0,
        NoneVAT = 1
    }

    /// <summary>
    /// SOAddViewModel
    /// </summary>
    public class AddSOViewModel
    {
        /// <summary>
        /// Constructors
        /// </summary>
        public AddSOViewModel()
        {
            this.ShippingCost = "0";
            this.Discount = "0";
            this.OrderDetails = new List<AddSOLineViewModel>();
        }


        public int OrderID { get; set; }


        public int? EmployeeId { get; set; }


        [Required(ErrorMessage = "Nhập số hóa đơn")]
        [MaxLength(15)]
        public string OrderNo { get; set; }


        public int OrderType { get; set; }


        [Required(ErrorMessage = "Nhập ngày bán")]
        public string OrderDate { get; set; }


        [Required(ErrorMessage = "Nhập tên khách hàng")]
        [MaxLength(100)]
        public string CustomerName { get; set; }


        [Required(ErrorMessage = "Nhập địa chỉ khách hàng")]
        [MaxLength(100)]
        public string CustomerAddress { get; set; }


        [Required(ErrorMessage = "Nhập số điện thoại")]
        [MaxLength(15)]
        public string Phone { get; set; }


        public string Email { get; set; }


        public string TaxCode { get; set; }


        [Required(ErrorMessage = "Vui lòng chọn hình thức thanh toán")]
        public int? PaymentTypeId { get; set; }


        public string ShipName { get; set; }


        [Required(ErrorMessage = "Vui lòng nhập số điện thoại khách hàng")]
        public string ShippedDate { get; set; }


        public string ShipAddress { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập chi phí vận chuyển")]
        public string ShippingCost { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập chiết khấu")]
        public string Discount { get; set; }

        public string DeliveryName { get; set; }


        public string Notes { get; set; }

        public IList<AddSOLineViewModel> OrderDetails { get; set; }
    }

    /// <summary>
    /// SOAddViewModel
    /// </summary>
    public class SODetailsViewModel
    {

        public SODetailsViewModel()
        {
            OrderDetails = new List<AddSOLineViewModel>();
        }

        public int OrderID { get; set; }

        public int? EmployeeId { get; set; }

        public string FullName { get; set; }

        [Required(ErrorMessage = "Nhập số hóa đơn")]
        [MaxLength(15)]
        public string OrderNo { get; set; }

        public int OrderTypeId { get; set; }

        public string OrderTypeName { get; set; }

        [Required(ErrorMessage = "Nhập ngày bán")]
        public string OrderDate { get; set; }

        [Required(ErrorMessage = "Nhập tên khách hàng")]
        [MaxLength(100)]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Nhập địa chỉ khách hàng")]
        [MaxLength(100)]
        public string CustomerAddress { get; set; }

        [Required(ErrorMessage = "Nhập số điện thoại")]
        [MaxLength(15)]
        public string Phone { get; set; }

        public string Email { get; set; }

        public string TaxCode { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hình thức thanh toán")]
        public int? PaymentTypeId { get; set; }

        public string PaymentTypeName { get; set; }

        public string ShipName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại khách hàng")]
        public string ShippedDate { get; set; }

        public string ShipAddress { get; set; }

        public string ShippingCost { get; set; }

        public string Discount { get; set; }

        public string DeliveryName { get; set; }


        public string Notes { get; set; }

        public IList<AddSOLineViewModel> OrderDetails { get; set; }
    }


    /// <summary>
    /// SOLineAddViewModel
    /// </summary>
    public class AddSOLineViewModel
    {

        /// <summary>
        /// Constructors
        /// </summary>
        public AddSOLineViewModel()
        {
            Quantity = "1";
            UnitPriceBT = "0";
            UnitPrice = "0";
        }

        public int ProductId { get; set; }


        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn kho xuất")]
        public int StockId { get; set; }

        public string StockName { get; set; }

        public string Quantity { get; set; }


        [RegularExpression(@"^\d+(,\d{3}){0,}(.\d+){0,1}$", ErrorMessage = "Giá trước thuế không hợp lệ!")]
        public string UnitPriceBT { get; set; }


        [RegularExpression(@"^\d+(,\d{3}){0,}(.\d+){0,1}$", ErrorMessage = "Giá sau thuế không hợp lệ!")]
        public string UnitPrice { get; set; }

    }
}