using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketEase.Domain.Entities;

namespace TicketEase.Application.DTO.Project
{
    public class ProjectReponseDto
    {
        public string Id { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
