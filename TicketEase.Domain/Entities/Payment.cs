using TicketEase.Domain.Enums;

namespace TicketEase.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public string TransactionReference { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public Status Status {  get; set; }
    }
}
