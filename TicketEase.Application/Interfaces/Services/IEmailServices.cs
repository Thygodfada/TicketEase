using TicketEase.Domain.Entities;

namespace TicketEase.Application.Interfaces.Services
{
    public interface IEmailServices
    {
        //Task SendEmailAsync(MailRequest mailRequest);
        Task SendHtmlEmailAsync(MailRequest request);
    }
}
