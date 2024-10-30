using System;
using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.DAL
{
    public class TaskAssignment
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int TaskId { get; set; }

        public int EmployeeId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public String Note { get; set; }
    }
}