using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PicPerfect.ViewModel
{
    public class RegisterVM
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username address is required")]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password do not match")]
        public string ConfirmPassword { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Fullname")]
        public string Fullname { get; set; }
        [DisplayName("Avatar")]
        public string Avatar { get; set; }
    }
}
