using System.ComponentModel.DataAnnotations;

namespace TicketEase.Application.DTO
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
