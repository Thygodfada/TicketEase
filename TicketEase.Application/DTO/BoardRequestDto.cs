using System.ComponentModel.DataAnnotations;

namespace TicketEase.Application.DTO
{
    public class BoardRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "ManagerId is required.")]
        public string ManagerId { get; set; }
    }
}
