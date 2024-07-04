using System.ComponentModel.DataAnnotations;
using TicketEase.Domain.Enums;

namespace TicketEase.Application.DTO
{
    public class TicketRequestDto
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [EnumDataType(typeof(Status), ErrorMessage = "Invalid Status")]
        public Status Status { get; set; }

        [EnumDataType(typeof(Priority), ErrorMessage = "Invalid Priority")]
        public Priority Priority { get; set; }

        [StringLength(255, ErrorMessage = "AssignedTo cannot be longer than 255 characters")]
        public string AssignedTo { get; set; }
    }
}
