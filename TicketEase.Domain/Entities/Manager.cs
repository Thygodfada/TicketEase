using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketEase.Domain.Entities
{
	public class Manager
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public  string Id { get; set; }
		public string CompanyName { get; set; }
		public string BusinessEmail { get; set; }
		public string BusinessPhone { get; set; }
		public string CompanyAddress { get; set; }
		public string CompanyDescription { get; set; }
		public string State { get; set; }
		public DateTime CreatedDate { get; set; } = DateTime.Now;
		public DateTime UpdatedDate { get; set; } = DateTime.Now;
		public ICollection<AppUser> Users { get; set; }        
		public string ImgUrl { get; set; }
		public ICollection<Board> Boards { get; set; }
	}
}