using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DrCleanCare.DAL
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }  
        public string FullName { get; set; }
        public string Address { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingContact { get; set; }
        public string Phone { get; set; } 
        public string Email { get; set; } 
        public string TaxCode { get; set; }
        public string Note { get; set; }
        public DateTime CreationDate { get; set; }  
    }
}