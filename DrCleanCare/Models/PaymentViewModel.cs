using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using DrCleanCare.DAL;

namespace DrCleanCare.Models
{
    public class AddPaymentViewModel
    {
        public int OrderID { get; set; }

        public string OrderNo { get; set; }

        public string OrderDate { get; set; }

        public string CustomerName { get; set; }

        public string CustomerAddress { get; set; }

        public string Email { get; set; }

        public decimal AmountBT { get; set; }
        
        public decimal VAT { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal DebtAmount { get; set; }
        
        public decimal GrandTotal { get; set; }

        [Required(ErrorMessage="Nhập ngày thanh toán")]
        public string PaymentDate { get; set; }

        [Required(ErrorMessage="Chọn hình thức thanh toán")]
        public int PaymentType { get; set; }
        
        public SelectList PaymentTypeOptions { get; set; }

        [Required(ErrorMessage="Nhập số tiền thanh toán")]
        public decimal PaymentAmount { get; set; }
        
        public string Description { get; set; }

        public List<PaymentHistoryResult> PaymentHistory { get; set; }
    }
}