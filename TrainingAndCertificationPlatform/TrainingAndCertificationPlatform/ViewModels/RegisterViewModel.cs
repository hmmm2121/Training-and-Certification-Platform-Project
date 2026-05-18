using System.ComponentModel.DataAnnotations;

namespace TrainingAndCertificationPlatform.ViewModels
{
    public class RegisterViewModel
    {
            [Required]
            [StringLength(100)]
            [Display(Name = "Full Name")]
            public string FullName { get; set; } = "";

            [Required]
            [EmailAddress]
            public string Email { get; set; } = "";

            [Required]
            [DataType(DataType.Password)]
            [StringLength(256, MinimumLength = 6,
                ErrorMessage = "Password must be at least 6 characters.")]
            public string Password { get; set; } = "";
        }
    }


