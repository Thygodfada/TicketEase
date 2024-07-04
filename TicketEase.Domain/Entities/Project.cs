using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketEase.Domain.Enums;

namespace TicketEase.Domain.Entities
{
    public class Project : BaseEntity
    {
        [StringLength(50)]
        public string Title { get; set; }

        [StringLength(125)]
        public string Description { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        
        [ForeignKey("BoardId")]
        public string BoardId { get; set; }
        //public virtual Board Board { get; set; }
    }
}
