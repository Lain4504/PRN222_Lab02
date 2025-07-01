using System.ComponentModel.DataAnnotations;

namespace HuynhNgocTien_SE18B01_A02.ViewModels
{
    public class SystemAccountViewModel
    {
        public short AccountId { get; set; }

        [Required(ErrorMessage = "Account name is required")]
        [Display(Name = "Account Name")]
        public string AccountName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email")]
        public string AccountEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        [Display(Name = "Role")]
        public int AccountRole { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string? AccountPassword { get; set; }
    }
}
