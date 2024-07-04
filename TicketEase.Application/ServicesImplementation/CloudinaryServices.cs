using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using TicketEase.Application.Interfaces.Repositories;
using TicketEase.Application.Interfaces.Services;

namespace TicketEase.Application.ServicesImplementation
{
    public class CloudinaryServices<TEntity> : ICloudinaryServices<TEntity> where TEntity : class
    {
        private readonly IGenericRepository<TEntity> _repository;

        public CloudinaryServices(IGenericRepository<TEntity> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<string> UploadImage(string entityId, IFormFile file)
        {
            var entity = _repository.GetById(entityId);

            if (entity == null)
            {
                return $"{typeof(TEntity).Name} not found";
            }

            var cloudinary = new Cloudinary(new Account(
                "dlpryp6af",
                "969623236923961",
                "QL5lf-M_syJrxGJdJzbu2oRMAZA"
            ));

            var upload = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream())
            };

            var uploadResult = await cloudinary.UploadAsync(upload);

            
            // Save the updated entity to the database
            _repository.Update(entity);

            try
            {
                _repository.SaveChanges();
                // Return the updated property value
                return uploadResult.SecureUrl.AbsoluteUri;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return "Database update error occurred";
            }
        }
    }
}
