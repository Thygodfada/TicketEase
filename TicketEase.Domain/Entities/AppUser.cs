using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketEase.Domain.Entities
{
    public class AppUser : IdentityUser
    {

        [ForeignKey("ManagerId")]
        public string ManagerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }        
        public string Address { get; set; }
        public string State { get; set; }
        public string Gender { get; set; }
        public string CloudinaryPublicId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = new DateTime();
        public string ImageUrl { get; set; }
        public string VerificationToken { get; set; }
        public string PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public Manager Manager { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
