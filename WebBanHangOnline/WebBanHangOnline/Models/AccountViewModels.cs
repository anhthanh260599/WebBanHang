using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBanHangOnline.Models
{
   
    public class CreateAccountrViewModel
    {
        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Phone { get; set; }
        public List<string> Roles { get; set; }
        public string Address { get; set; }
        public string Avatar { get; set; }
        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage ="Vui lòng không để trống")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng không để trống")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng không để trống")]
        [DataType(DataType.Password)]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class EditAccountViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Vui lòng không để trống")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Vui lòng không để trống")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Vui lòng không để trống")]
        public string Phone { get; set; }
        public List<string> Roles { get; set; }
        public string Address { get; set; }
        public string Avatar { get; set; }
        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "Vui lòng không để trống")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginConfirmationViewModel
    {
        [Required(ErrorMessage = "Vui lòng không để trống")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required(ErrorMessage = "Vui lòng không để trống")]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required(ErrorMessage = "Vui lòng không để trống")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng không để trống")]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        //[Required]
        //[Display(Name = "Email")]
        //[EmailAddress]
        //public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng không để trống")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Nhớ mật khẩu?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        //[Required]
        //public string UserName { get; set; }
        [Required(ErrorMessage = "Vui lòng không để trống")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Vui lòng không để trống")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng không để trống")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
        [Required(ErrorMessage = "Vui lòng không để trống")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu không trùng khớp.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng không để trống")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng không để trống")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng không để trống")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
