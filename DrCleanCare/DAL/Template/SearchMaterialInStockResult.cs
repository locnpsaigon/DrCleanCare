using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.DAL
{
    public class SearchMaterialInStockResult
    {
        [Key]
        public int MaterialId { get; set; }

        public string MaterialName { get; set; }

        public int StockId { get; set; }

        public string StockName { get; set; }

        public double UnitInStock { get; set; }
    }
}