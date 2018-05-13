using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.DAL
{
    public class PaymentType
    {
        [Key]
        public int PaymentTypeId { get; set; }
        public string PaymentTypeName { get; set; }
        public string Description { get; set; }
    }
}