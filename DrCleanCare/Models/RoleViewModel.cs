using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.Models
{
    public class AddRoleViewModel
    {
        [Required(ErrorMessage = "Tên vai trò không được rỗng")]
        [DataType(DataType.Text)]
        [StringLength(50)]
        public string RoleName { get; set; }

        public string Description { get; set; }
       
    }

    public class EditRoleViewModel
    {
        [Key]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Tên vai trò không được rỗng")]
        [DataType(DataType.Text)]
        [StringLength(50)]
        public string RoleName { get; set; }

        public string Description { get; set; }

    }
}