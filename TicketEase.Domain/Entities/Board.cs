using System.ComponentModel.DataAnnotations.Schema;

namespace TicketEase.Domain.Entities
{
    public class Board : BaseEntity
    {
        [ForeignKey("ManagerId")]
        public string ManagerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Project> Projects { get; set; }
    }
}
