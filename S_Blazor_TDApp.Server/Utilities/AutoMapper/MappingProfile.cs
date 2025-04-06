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

            // Mapeo para TareasCalendario y TareasCalendarioDTO
            CreateMap<TareasCalendario, TareasCalendarioDTO>()
                // Si la entidad tiene múltiples registros completados, se selecciona el primero (o ajusta según la lógica requerida)
                .ForMember(dest => dest.ReftareasCalendarioCompletado, opt =>
                    opt.MapFrom(src => src.TareasCalendarioCompletados.FirstOrDefault()))
                .ReverseMap();

            // Mapeo para TareasCalendarioCompletado y TareasCalendarioCompletadoDTO
            CreateMap<TareasCalendarioCompletado, TareasCalendarioCompletadoDTO>()
                // Mapea la referencia de la tarea: la propiedad RefTarea de la entidad se asigna a RefTareaCalendario del DTO.
                .ForMember(dest => dest.RefTareaCalendario, opt =>
                    opt.MapFrom(src => src.RefTarea))
                .ReverseMap()
                .ForMember(dest => dest.RefTarea, opt =>
                    opt.MapFrom(src => src.RefTareaCalendario));

            CreateMap<TareasRecurrente, TareasRecurrentesDTO>()
                .ReverseMap();

            CreateMap<Usuario, UsuarioDTO>()
                .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => src.IdRolNavigation))
                .ReverseMap()
                .ForMember(dest => dest.IdRolNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.FechaActualizacion, opt => opt.MapFrom(src => DateTime.Now));

            CreateMap<RegistroProceso, RegistroProcesoDTO>()
                .ReverseMap();
        }
    }
}