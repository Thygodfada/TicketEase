using TicketEase.Domain.Entities;

namespace TicketEase.Configurations
{
    public static class MailServiceExtension
    {
        public static void AddMailService(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<EmailSettings>(config.GetSection("EmailSettings"));
        }
    }
}
