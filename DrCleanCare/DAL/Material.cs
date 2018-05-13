using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.DAL
{
    public class Material
    {
        [Key]
        public int MaterialId { get; set; }

        public string MaterialName { get; set; }

        public string Description { get; set; }

        public string QuantityPerUnit { get; set; }

        public DateTime CreationDate { get; set; }
    }
}