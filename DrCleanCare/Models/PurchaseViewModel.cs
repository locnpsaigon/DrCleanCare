using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.Models
{
    public class AddPOViewModel
    {
        public int PurchaseId { get; set; }

        [Required(ErrorMessage="Vui lòng chọn ngày chi")]
        public string PurchaseDate { get; set; }

        [Required(ErrorMessage="Tên chi phí không được rỗng")]
        [MaxLength(150)]
        public string PurchaseName { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required(ErrorMessage="Vui lòng nhập chi phí")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
    }
}