using AutoMapper;
using S_Blazor_TDApp.Server.Entities;
using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Server.Utilities.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeo entre Rol y RolDTO
            CreateMap<Rol, RolDTO>()
                .ReverseMap();

            CreateMap<TareaDia, TareaDiasDTO>()
                .ReverseMap();

            CreateMap<TareasCalendario, TareasCalendarioDTO>()
                .ReverseMap();

            CreateMap<TareasRecurrente, TareasRecurrentesDTO>()
                .ReverseMap();

            CreateMap<Usuario, UsuarioDTO>()
                // Mapea la entidad Rol en IdRolNavigation hacia la propiedad Rol del DTO
                .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => src.IdRolNavigation))
                .ReverseMap()
                // Al mapear de DTO a entidad, ignoramos la propiedad de navegación
                // y asignamos FechaActualizacion a DateTime.Now
                .ForMember(dest => dest.IdRolNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.FechaActualizacion, opt => opt.MapFrom(src => DateTime.Now));

            CreateMap<RegistroProceso, RegistroProcesoDTO>()
                .ReverseMap();
        }
    }
}