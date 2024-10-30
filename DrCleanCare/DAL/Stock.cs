using System;
using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.DAL
{
    public class Stock
    {
        [Key]
        public int StockId { get; set; }

        public String StockName { get; set; }

        public String StockAddress { get; set; }

        public String Phone { get; set; }

        public string Description { get; set; }

        public DateTime CreationDate { get; set; }
    }
}