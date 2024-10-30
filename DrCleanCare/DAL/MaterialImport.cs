using System;
using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.DAL
{
    public class MaterialImport
    {
        [Key]
        public int MaterialImportId { get; set; }

        public DateTime MaterialImportDate { get; set; }

        public int MaterialId { get; set; }

        public int? StockId { get; set; }

        public double Quantity { get; set; }

        public decimal Price { get; set; }

        public string Notes { get; set; }
    }
}