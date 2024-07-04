using TicketEase.Domain.Entities;
using TicketEase.Domain.Enums;

namespace TicketEase.Application.DTO
{
    public class TicketResponseDto
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TicketReference { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Status Status { get; set; }
        public Priority Priority { get; set; }
        public string AssignedTo { get; set; }
        public string AppUserId { get; set; }
        public string ProjectId { get; set; }
        public ICollection<Comment> Comment { get; set; }
    }
}
