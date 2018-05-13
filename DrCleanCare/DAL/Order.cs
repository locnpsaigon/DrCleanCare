using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.DAL
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderNo { get; set; }
        public int OrderType { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string TaxCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string ShipName { get; set; }
        public string ShipAddress { get; set; }
        public DateTime ShippedDate { get; set; }
        public string Owner { get; set; }
        public DateTime CreationDate { get; set; }
        public int? PaymentTypeId { get; set; }
        public int? EmployeeId { get; set; }
        public string DeliveryName { get; set; }
        public string Notes { get; set; }
    }
}