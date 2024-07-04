using System.ComponentModel.DataAnnotations;

namespace TicketEase.Application.DTO
{
    public class UpdateUserDto
    {
       
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string Address { get; set; }

        public string State { get; set; }

        public string Gender { get; set; }
        public string PhoneNumber { get; set; }

        public string CloudinaryPublicId { get; set; }

        public bool IsActive { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
