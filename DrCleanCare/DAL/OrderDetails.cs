using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.DAL
{
    public class OrderDetails
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int StockId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitPriceBT { get; set; }
        public int Quantity { get; set; }
        public float Discount { get; set; }
    }
}