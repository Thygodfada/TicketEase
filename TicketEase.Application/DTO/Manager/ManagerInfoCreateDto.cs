using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TicketEase.Application.DTO.Manager
{
    public class ManagerInfoCreateDto
    {
        [Required(ErrorMessage = "Business email is required.")]
        [EmailAddress(ErrorMessage = "Invalid business email address.")]
        public string BusinessEmail { get; set; }

        [Required(ErrorMessage = "Company name is required.")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Company description is required.")]
        public string CompanyDescription { get; set; }

        [JsonIgnore]
        public string AdminEmail { get; set; }

        [JsonIgnore]
        public string Password { get; set; }
    }
}
