using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketEase.Application.DTO
{
    public class AppUserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string ImageUrl { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }
    }
}
