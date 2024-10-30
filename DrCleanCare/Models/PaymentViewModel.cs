using DrCleanCare.DAL;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

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

        public string AmountBT { get; set; }

        public string VAT { get; set; }

        public string PaidAmount { get; set; }

        public string DebtAmount { get; set; }

        public string GrandTotal { get; set; }

        [Required(ErrorMessage = "Nhập ngày thanh toán")]
        public string PaymentDate { get; set; }

        [Required(ErrorMessage = "Chọn hình thức thanh toán")]
        public int PaymentType { get; set; }

        public SelectList PaymentTypeOptions { get; set; }

        [Required(ErrorMessage = "Nhập số tiền thanh toán")]
        [RegularExpression(@"^\d+(,\d{3}){0,}(.\d+){0,1}$", ErrorMessage = "Số tiền thanh toán không hợp lệ!")]
        public string PaymentAmount { get; set; }

        public string Description { get; set; }

        public List<PaymentHistoryResult> PaymentHistory { get; set; }

        public AddPaymentViewModel()
        {
            PaymentHistory = new List<PaymentHistoryResult>();
        }
    }
}