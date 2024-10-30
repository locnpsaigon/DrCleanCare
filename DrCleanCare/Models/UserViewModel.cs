using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.Models
{

    /// <summary>
    /// LoginViewModel
    /// </summary>
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập không được rỗng")]
        [RegularExpression(@"^[a-z0-9_-]{2,15}$",
            ErrorMessage = "Tên đăng nhập dài 2 đến 15 ký tự gồm ký tự thường, số, gạch ngang và gạch dưới")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được rỗng")]
        public string Password { get; set; }

        public Boolean RememberMe { get; set; }
    }

    /// <summary>
    /// AddUserViewModel
    /// </summary>
    public class AddUserViewModel
    {

        public int UserId { get; set; }

        public AddUserViewModel()
        {
            SelectedRoleModels = new List<UserRoleSelectedViewModel>();
        }

        [Required(ErrorMessage = "Tên đăng nhập không được rỗng")]
        [RegularExpression(@"^[a-z0-9_-]{2,15}$",
            ErrorMessage = "Tên tài khoản chỉ gồm chữ và số, gạch ngang và gạch dưới")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được rỗng")]
        [RegularExpression(@"^.*(?=.{5,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[_!@#$%^&+=]).*$",
            ErrorMessage = "Mật khẩu dài 5 đến 18 ký tự. Bao gồm ít nhất 1 ký tự hoa, 1 ký tự thường và 1 ký tự đặc biệt (_!@#$%^&+=)")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Xác nhận mật khẩu và mật khẩu không trùng khớp")]
        [RegularExpression(@"^.*(?=.{5,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[_!@#$%^&+=]).*$",
            ErrorMessage = "Mật khẩu dài 5 đến 18 ký tự. Bao gồm ít nhất 1 ký tự hoa, 1 ký tự thường và 1 ký tự đặc biệt (_!@#$%^&+=)")]
        public string PasswordConfirm { get; set; }

        [Required(ErrorMessage = "Họ không được rỗng")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Tên không được rỗng")]
        public string LastName { get; set; }

        public string Email { get; set; }

        public Boolean IsActive { get; set; }

        public List<UserRoleSelectedViewModel> SelectedRoleModels { get; set; }
    }

    /// <summary>
    /// EditUserViewModel
    /// </summary>
    public class EditUserViewModel
    {
        /// <summary>
        /// Constructors
        /// </summary>
        public EditUserViewModel()
        {
            this.SelectedRoleModels = new List<UserRoleSelectedViewModel>();
        }

        public int UserId { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được rỗng")]
        public string Username { get; set; }

        [RegularExpression(@"^.*(?=.{5,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[_!@#$%^&+=]).*$",
            ErrorMessage = "Mật khẩu dài 5 đến 18 ký tự. Bao gồm ít nhất 1 ký tự hoa, 1 ký tự thường và 1 ký tự đặc biệt (_!@#$%^&+=)")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Xác nhận mật khẩu và mật khẩu không trùng khớp")]
        [RegularExpression(@"^.*(?=.{5,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[_!@#$%^&+=]).*$",
            ErrorMessage = "Mật khẩu dài 5 đến 18 ký tự. Bao gồm ít nhất 1 ký tự hoa, 1 ký tự thường và 1 ký tự đặc biệt (_!@#$%^&+=)")]
        public string PasswordConfirm { get; set; }

        [Required(ErrorMessage = "Họ không được rỗng")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Tên không được rỗng")]
        public string LastName { get; set; }

        public string Email { get; set; }

        public Boolean IsActive { get; set; }

        public List<UserRoleSelectedViewModel> SelectedRoleModels { get; set; }
    }

    /// <summary>
    /// ChangeUserPassViewModel
    /// </summary>
    public class ChangeUserPassViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được rỗng")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được rỗng")]
        [RegularExpression(@"^.*(?=.{5,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[_!@#$%^&+=]).*$",
            ErrorMessage = "Mật khẩu dài 5 đến 18 ký tự. Bao gồm ít nhất 1 ký tự hoa, 1 ký tự thường và 1 ký tự đặc biệt (_!@#$%^&+=)")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Mật khẩu và mật khẩu xác nhận chưa trùng khớp")]
        [RegularExpression(@"^.*(?=.{5,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[_!@#$%^&+=]).*$",
            ErrorMessage = "Mật khẩu dài 5 đến 18 ký tự. Bao gồm ít nhất 1 ký tự hoa, 1 ký tự thường và 1 ký tự đặc biệt (_!@#$%^&+=)")]
        public string PasswordConfirm { get; set; }
    }

    public class UserRoleSelectedViewModel
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public bool IsSelected { get; set; }
    }
}