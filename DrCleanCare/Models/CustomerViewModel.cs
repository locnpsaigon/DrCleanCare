using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DrCleanCare.Models
{
    public class AddCustomerViewModel
    {
        public int CustomerId { get; set; }
        [Required(ErrorMessage = "Tên khách hàng không được rỗng")]
        public string FullName { get; set; }
        public string Address { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingContact { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string TaxCode { get; set; }
        public string Note { get; set; }
    }
}