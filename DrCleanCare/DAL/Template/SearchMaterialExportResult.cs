using System;
using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.DAL
{
    public class SearchMaterialExportResult
    {
        [Key]
        public int MaterialExportId { get; set; }

        public DateTime MaterialExportDate { get; set; }

        public int MaterialId { get; set; }

        public string MaterialName { get; set; }

        public int StockId { get; set; }

        public string StockName { get; set; }

        public double Quantity { get; set; }

        public string Notes { get; set; }
    }
}