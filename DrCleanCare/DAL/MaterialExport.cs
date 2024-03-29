﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.DAL
{
    public class MaterialExport
    {
        [Key]
        public int MaterialExportId { get; set; }

        public DateTime MaterialExportDate { get; set; }

        public int MaterialId { get; set; }

        public int? StockId { get; set; }

        public double Quantity { get; set; }

        public string Notes { get; set; }
    }
}