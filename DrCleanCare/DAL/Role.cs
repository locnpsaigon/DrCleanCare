﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.DAL
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Tên vai trò không được rỗng")]
        [DataType(DataType.Text)]
        [StringLength(50)]
        public string RoleName { get; set; }

        public string Description { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}