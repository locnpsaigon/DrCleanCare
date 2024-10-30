using System;
using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.DAL
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Message { get; set; }
    }
}