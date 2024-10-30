using System;
using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.DAL
{
    public class StockInput
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int ProductId { get; set; }

        public int StockId { get; set; }

        public int Quantity { get; set; }

        public string Note { get; set; }

        public string UserName { get; set; }
    }
}