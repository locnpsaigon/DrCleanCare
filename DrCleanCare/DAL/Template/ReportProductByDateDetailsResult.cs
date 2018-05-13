using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrCleanCare.DAL
{
    public class ReportProductByDateDetailsResult
    {
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string Phone { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal AmountBT { get; set; }
        public decimal Amount { get; set; }

    }

    public class ReportProductByDateDetailsForCustomerResult
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderNo { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string Phone { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal AmountBT { get; set; }
        public decimal Amount { get; set; }
    }
}