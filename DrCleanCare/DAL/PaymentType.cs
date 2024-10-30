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