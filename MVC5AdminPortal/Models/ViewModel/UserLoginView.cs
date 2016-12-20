using System.ComponentModel.DataAnnotations;

namespace MVC5AdminPortal.Models.ViewModel
{
    public class UserLoginView
    {
        [Key]
        public int SYSUserID { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Login ID")]
        public string LoginName { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}