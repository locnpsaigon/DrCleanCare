using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.Models
{
    public class AddFeedbackViewModel
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [Required(ErrorMessage="Vui lòng nhập họ tên")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string Phone { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        public string Message { get; set; }
    }
}