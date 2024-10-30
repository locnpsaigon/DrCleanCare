using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrCleanCare.DAL
{
    public class MaterialInStock
    {
        [Key, Column(Order = 0)]
        public int MaterialId { get; set; }

        [Key, Column(Order = 1)]
        public int StockId { get; set; }

        public double Quantity { get; set; }
    }
}