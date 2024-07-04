using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketEase.Domain.Entities;

namespace TicketEase.Application.DTO.Project
{
    public class ProjectRequestDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        [MaxLength(500)]
        public string Description { get; set; }
    }
}
