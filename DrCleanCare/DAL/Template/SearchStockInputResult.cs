using System;
using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.DAL
{
    public class SearchStockInputResult
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int CategoryId { get; set; }

        public int StockId { get; set; }

        public string StockName { get; set; }

        public int Quantity { get; set; }

        public string Note { get; set; }

        public string UserName { get; set; }
    }
}