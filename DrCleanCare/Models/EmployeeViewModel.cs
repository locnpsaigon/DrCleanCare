using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.Models
{
    public class AddEmployeeViewModel
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Employee's name can not blank!!!")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "You must choose a day")]
        public int DOB_Day { get; set; }

        [Required(ErrorMessage = "You must choose a month")]
        public int DOB_Month { get; set; }

        [Required(ErrorMessage = "You must choose a year")]
        public int DOB_Year { get; set; }

        public int Gender { get; set; }

        public int MaritalStatus { get; set; }

        [MaxLength(150, ErrorMessage = "Address only contains less than 150 characters")]
        public string Address { get; set; }

        public string Phone { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string HireDate { get; set; }

        public string QuitDate { get; set; }

        [MaxLength(20, ErrorMessage = "Taxcode must be less than 20 characters")]
        public string TaxCode { get; set; }

    }
}