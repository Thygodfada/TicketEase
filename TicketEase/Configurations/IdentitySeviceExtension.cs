using Microsoft.AspNetCore.Identity;
using TicketEase.Domain.Entities;
using TicketEase.Persistence.Context;

namespace TicketEase.Configurations
{
    public static class IdentityServiceExtension
    {
        public static void IdentityConfiguration(this IServiceCollection services)
        {
            var builder = services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
                options.Password.RequireDigit = false; //return this to true
                options.Password.RequireNonAlphanumeric = false; //return this to true
				options.Password.RequireUppercase = false; //return this to true
				options.Password.RequireLowercase = true;
                options.Password.RequiredLength = 6;
            });
           // builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
            builder.AddEntityFrameworkStores<TicketEaseDbContext>().AddDefaultTokenProviders();


            //services.AddIdentity<Manager, IdentityRole>()
            //	.AddEntityFrameworkStores<TicketEaseDbContext>()
            //	.AddDefaultTokenProviders();

        }
	}
}