using AutoMapper;
using TicketEase.Application.DTO;
using TicketEase.Application.DTO.Manager;
using TicketEase.Application.DTO.Project;
using TicketEase.Common.Utilities;
using TicketEase.Domain.Entities;

namespace TicketEase.Mapper
{
	public class MapperProfile : Profile
	{
		public MapperProfile()
		{
			CreateMap<UpdateProjectRequestDto, Project>();
			CreateMap<AppUser, UpdateUserDto>().ReverseMap();
			CreateMap<Project, ProjectReponseDto>().ReverseMap();
			CreateMap<Manager, EditManagerDto>().ReverseMap();
			CreateMap<AppUser, ManagerInfoCreateDto>().ReverseMap();
			CreateMap<BoardRequestDto, Board>();
			CreateMap<Board, BoardResponseDto>().ReverseMap();
			CreateMap<AppUser, UpdatePhotoDTO>();
			CreateMap<Ticket, TicketRequestDto>().ReverseMap();
			CreateMap<UpdateTicketRequestDto, Ticket>();
			CreateMap<AppUser, AppUserDto>();
			CreateMap<PageResult<IEnumerable<AppUser>>, PageResult<IEnumerable<AppUserDto>>>();
			CreateMap<UpdatePhotoDTO, AppUser>();

			CreateMap<ManagerInfoCreateDto, Manager>()
				.ForMember(man => man.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
				.ForMember(man => man.BusinessEmail, opt => opt.MapFrom(src => src.BusinessEmail));

			//CreateMap<Manager, ManagerResponseDto>()
			//	.ForMember(man => man.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
			//	.ForMember(man => man.BusinessEmail, opt => opt.MapFrom(src => src.BusinessEmail));

			CreateMap<Manager, ManagerResponseDto>();

			CreateMap<ProjectRequestDto, Project>()
			 .ForMember(dest => dest.Id, opt => opt.Ignore())
			 .ForMember(dest => dest.BoardId, opt => opt.Ignore())
			 .ForMember(dest => dest.Id, opt => opt.Ignore())
			 .ForMember(dest => dest.BoardId, opt => opt.Ignore());

			CreateMap<Ticket, TicketResponseDto>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.TicketReference, opt => opt.MapFrom(src => src.TicketReference))
			.ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment));

			
		}
	}
}
