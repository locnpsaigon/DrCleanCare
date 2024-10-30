using System;
using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.Models
{
    public class AddStockViewModel
    {
        [Required(ErrorMessage = "Tên kho không được rỗng")]
        public String StockName { get; set; }

        [Required(ErrorMessage = "Địa chỉ kho không được rỗng")]
        public String StockAddress { get; set; }

        public String Phone { get; set; }

        public string Description { get; set; }
    }

    public class EditStockViewModel
    {
        [Key]
        public int StockId { get; set; }

        [Required(ErrorMessage = "Tên kho không được rỗng")]
        public String StockName { get; set; }

        [Required(ErrorMessage = "Địa chỉ kho không được rỗng")]
        public String StockAddress { get; set; }

        public String Phone { get; set; }

        public string Description { get; set; }
    }
}