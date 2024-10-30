using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrCleanCare.DAL
{
    public class ProductInStock
    {
        [Key, Column(Order = 0)]
        public int ProductId { get; set; }

        [Key, Column(Order = 1)]
        public int StockId { get; set; }

        public int Quantity { get; set; }
    }
}