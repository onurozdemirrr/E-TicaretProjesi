using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace ETicaret.WebUI.Models
{
    public class LoginModel
    {
        
        [Required]
        [Display(Name = "Kullanıcı Adınız")]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Şifreniz")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}
