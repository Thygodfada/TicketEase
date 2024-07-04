using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicketEase.Application.Interfaces.Repositories;
using TicketEase.Application.Interfaces.Services;
using TicketEase.Application.ServicesImplementation;
using TicketEase.Domain.Entities;
using TicketEase.Persistence.Context;
using TicketEase.Persistence.Repositories;

namespace TicketEase.Persistence.Extensions
{
	public static class DIServiceExtension
	{
		public static void AddDependencies(this IServiceCollection services, IConfiguration config)
		{
			//Bind CloudinarySettings from configuration
			var cloudinarySettings = new CloudinarySetting();
			config.GetSection("CloudinarySettings").Bind(cloudinarySettings);
			services.AddSingleton(cloudinarySettings);
			var emailSettings = new EmailSettings();
			config.GetSection("EmailSettings").Bind(emailSettings);
			services.AddSingleton(emailSettings);
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped(typeof(ICloudinaryServices<>), typeof(CloudinaryServices<>));
			services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
			services.AddScoped<IUnitOfWork, UnitOfWork>();
			services.AddScoped<IProjectServices, ProjectServices>();
			services.AddScoped<IBoardServices, BoardServices>();
			services.AddScoped<ITicketService, TicketService>();
			services.AddScoped<ITicketRepository, TicketRepository>();
			services.AddScoped<ICommentRepository, CommentRepository>();
			services.AddScoped<IPaymentRepository, PaymentRepository>();
			services.AddScoped<IManagerServices, ManagerServices>();
			services.AddScoped<IUserServices, UserServices>();
			services.AddScoped<IAuthenticationService, AuthenticationService>();
			services.AddScoped<IEmailServices, EmailServices>();			
			services.AddDbContext<TicketEaseDbContext>(options =>
			options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
		}
	}

}
