using System;

namespace DrCleanCare.DAL
{
    public class PaymentHistoryResult
    {
        public int PaymentId { get; set; }

        public int OrderID { get; set; }

        public DateTime PaymentDate { get; set; }

        public int PaymentTypeId { get; set; }

        public string PaymentTypeName { get; set; }

        public decimal PaymentAmount { get; set; }

        public string Description { get; set; }
    }
}