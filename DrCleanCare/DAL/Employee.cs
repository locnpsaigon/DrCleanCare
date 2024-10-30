using System;
using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.DAL
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        public string FullName { get; set; }

        public DateTime DOB { get; set; }

        public int Gender { get; set; }

        public int MaritalStatus { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public DateTime HireDate { get; set; }

        public DateTime QuitDate { get; set; }

        public string TaxCode { get; set; }

    }
}