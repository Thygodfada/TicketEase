using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace TicketEase.Application.DTO
{
    public class AppUserCreateDto
    {
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Required]
        public string ManagerId { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }
    }
}
