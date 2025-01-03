﻿using System;
using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.DAL
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        public int OrderID { get; set; }
        public DateTime PaymentDate { get; set; }
        public int PaymentTypeId { get; set; }
        public decimal PaymentAmount { get; set; }
        public string Description { get; set; }
    }
}