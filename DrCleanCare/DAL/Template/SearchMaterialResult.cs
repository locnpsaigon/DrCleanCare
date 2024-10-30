using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.DAL
{
    public class SearchMaterialResult
    {
        [Key]
        public int MaterialId { get; set; }

        public string MaterialName { get; set; }

        public string Description { get; set; }

        public string QuantityPerUnit { get; set; }

        public double UnitInStockGoVap { get; set; }

        public double UnitInStockCuChi { get; set; }

        public double UnitInStockBinhDuong { get; set; }

        public double UnitInStock { get; set; }
    }
}