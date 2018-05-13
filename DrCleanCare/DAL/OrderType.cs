using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.DAL
{
    public class OrderType
    {
        [Key]
        public int OrderTypeId { get; set; }
        public string OrderTypeName { get; set; }
        public string Description { get; set; }
    }
}