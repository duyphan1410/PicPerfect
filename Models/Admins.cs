using System.ComponentModel.DataAnnotations;

namespace PicPerfect.Models
{
    public class Admins
    {
        [Key]
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
