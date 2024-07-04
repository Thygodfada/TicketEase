using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using TicketEase.Domain.Entities;

namespace TicketEase.Configurations
{
    public static class CloudinaryServiceExtension
    {
        public static void AddCloudinaryService(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<CloudinarySetting>(config.GetSection("CloudinarySettings"));


            services.AddSingleton(provider =>
            {
                var cloudinarySettings = provider.GetRequiredService<IOptions<CloudinarySetting>>().Value;

                Account cloudinaryAccount = new(
                    cloudinarySettings.CloudName,
                    cloudinarySettings.APIKey,
                cloudinarySettings.APISecret);

                return new Cloudinary(cloudinaryAccount);
            });
        }
    }
}
