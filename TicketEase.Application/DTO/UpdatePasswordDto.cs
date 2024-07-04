using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketEase.Application.DTO
{
    public class UpdatePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }
    }
}
