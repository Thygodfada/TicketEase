using System.ComponentModel.DataAnnotations.Schema;

namespace TicketEase.Domain.Entities
{
    public class BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set;}
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

    }
}
