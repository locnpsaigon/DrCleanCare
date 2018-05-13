using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrCleanCare.DAL
{
    public class ReportOrdersInDebtResult
    {
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public decimal AmountBT { get; set; }
        public decimal VAT { get; set; }
        public decimal Amount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal DebtAmount { get; set; }
    }
}