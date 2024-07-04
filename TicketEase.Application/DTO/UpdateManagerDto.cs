using Microsoft.AspNetCore.Http;

namespace TicketEase.Application.DTO
{
    public class UpdateManagerDto
    {
        public string CompanyName { get; set; }
        public string BusinessEmail { get; set; }
        public string BusinessPhone { get; set; }
        public string CompanyAddress { get; set; }
        public string State { get; set; }
        public IFormFile File { get; set; }
       // public DateTime UpdatedDate { get; set; }
    }
}
