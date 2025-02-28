using AutoMapper;
using S_Blazor_TDApp.Server.Entities;
using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Server.Utilities.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Rol, RolDTO>()
                .ReverseMap();

            CreateMap<TareaDia, TareaDiasDTO>()
                .ReverseMap();

            CreateMap<TareasCalendario, TareasCalendarioDTO>()
                .ReverseMap();

            CreateMap<TareasRecurrente, TareasRecurrentesDTO>()
                .ReverseMap();

            CreateMap<Usuario, UsuarioDTO>()
                .ReverseMap()
                 
                // Se asigna este miembro para poder pasar la fecha al editar el usuario y reflejar su fecha de actualización 
                .ForMember(dest => dest.FechaActualizacion, opt => opt.MapFrom(src => DateTime.Now));
        }
    }
}