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