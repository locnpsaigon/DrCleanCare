using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.DAL
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductIntro { get; set; }

        public string ProductDescription { get; set; }

        public string QuantityPerUnit { get; set; }

        public decimal PriceBT { get; set; }

        public decimal Price { get; set; }

        public string IconURL { get; set; }

        public string ImageURL { get; set; }
        
        public int CategoryId { get; set; }

        public bool Discontinued { get; set; }

        public bool Displayed { get; set; }
    }
}