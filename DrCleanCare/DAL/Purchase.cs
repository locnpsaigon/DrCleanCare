using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.DAL
{
    public class Purchase
    {
        [Key]
        public int PurchaseId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string PurchaseName { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }
}