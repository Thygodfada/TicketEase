namespace TicketEase.Application.DTO.Manager
{
    public class EditManagerDto
    {
        public string CompanyName { get; set; }
        public string BusinessEmail { get; set; }
        public string BusinessPhone { get; set; }
        public string CompanyAddress { get; set; }
        public string State { get; set; }
        public string ImgUrl { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}