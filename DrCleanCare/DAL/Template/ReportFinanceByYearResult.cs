using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrCleanCare.DAL
{
    public class ReportFinanceByYearResult
    {
        public int Month { get; set; }

        public decimal Purchase { get; set; }

        public decimal Revenue { get; set; }
        
        public decimal Payment { get; set; }
        
        public decimal Debt { get; set; }
    }
}