using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.Models
{

    /// <summary>
    /// TaskViewModel
    /// </summary>
    public class AddTaskViewModel
    {
        [Key]
        public int TaskId { get; set; }

        [Required(ErrorMessage="Tên đầu việc không được rỗng!")]
        public string TaskName { get; set; }

        public string Description { get; set; }
    }

    /// <summary>
    /// TaskAssignmentViewModel
    /// </summary>
    public class AssignTaskViewModel
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn đầu việc")]
        public int TaskId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày bắt đầu")]
        public string StartDate { get; set; }

        [Required(ErrorMessage = "Vui chọn giờ")]
        public int StartHour { get; set; }

        [Required(ErrorMessage = "Vui chọn phút")]
        public int StartMinute { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày kết thúc")]
        public string EndDate { get; set; }

        [Required(ErrorMessage = "Vui chọn giờ")]
        public int EndHour { get; set; }

        [Required(ErrorMessage = "Vui chọn phút")]
        public int EndMinute { get; set; }

        public string Note { get; set; }
    }
}