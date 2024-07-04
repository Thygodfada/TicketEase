using System.ComponentModel.DataAnnotations.Schema;
using TicketEase.Domain.Enums;

namespace TicketEase.Domain.Entities
{
    public class Ticket : BaseEntity
    {
        public string TicketReference { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Status Status { get; set; }
        public Priority Priority { get; set; }
        public DateTime ResolvedAt { get; set; }
        public string AssignedTo { get; set; }
        [ForeignKey("AppUserId")]
        public string AppUserId { get; set; }
        [ForeignKey("ProjectId")]
        public string ProjectId { get; set; }
        public ICollection<Comment> Comment { get; set; }
    }
}
