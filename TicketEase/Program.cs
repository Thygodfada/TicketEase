using CloudinaryDotNet;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TicketEase.Common.Utilities;
using TicketEase.Configurations;
using TicketEase.Domain.Entities;
using TicketEase.Mapper;
using TicketEase.Persistence.Context;
using TicketEase.Persistence.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var configuration = builder.Configuration;
//var services = builder.Services;
//var env = builder.Environment;

// Add services to the container.
builder.Services.AddDependencies(configuration);
builder.Services.AddAuthentication();
builder.Services.AuthenticationConfiguration(configuration);

// Identity  configuration
builder.Services.IdentityConfiguration();
builder.Services.AddLoggingConfiguration(configuration);

builder.Services.AddSingleton(provider =>
{
    var cloudinarySettings = provider.GetRequiredService<IOptions<CloudinarySetting>>().Value;

    Account cloudinaryAccount = new(
        cloudinarySettings.CloudName,
        cloudinarySettings.APIKey,
        cloudinarySettings.APISecret);
    return new Cloudinary(cloudinaryAccount);
});

builder.Services.AddMailService(configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.Services.AddAutoMapper(typeof(MapperProfile));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ticket Ease v1"));
}
using (var scope = app.Services.CreateScope())
{
    var serviceprovider = scope.ServiceProvider;
    Seeder.SeedRolesAndSuperAdmin(serviceprovider);
}

app.UseCors(p => p.AllowAnyOrigin()
    .AllowAnyHeader().AllowAnyMethod());

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();