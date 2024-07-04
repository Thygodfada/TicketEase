using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketEase.Application.DTO.Project
{
    public class GetProjectsDto
    {
        public List<ProjectReponseDto> Projects { get; set; }
        public int PerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPageCount { get; set; }
        public int TotalCount { get; set; }

    }
}
