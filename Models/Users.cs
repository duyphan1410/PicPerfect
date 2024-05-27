using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PicPerfect.Models
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        [DisplayName("Username")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Username { get; set; }
        [Required]
        [DisplayName("Mật khẩu")]
        public string? Password { get; set; }
        [NotMapped]
        [Required]
        [System.ComponentModel.DataAnnotations.Compare("Password")]
        public string? ConfirmPassword { get; set; }
        [DisplayName("Email")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }
        public string? Fullname { get; set; }
      
        public string? Avatar { get; set; }

    }

}
