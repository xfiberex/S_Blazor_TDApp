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

            // Mapeo entre Menu y MenuDTO
            CreateMap<Menu, MenuDTO>()
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
                // Ignorar la referencia inversa para evitar referencia circular en la serialización JSON
                .ForMember(dest => dest.RefTareaCalendario, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.RefTarea, opt =>
                    opt.MapFrom(src => src.RefTareaCalendario));

            CreateMap<TareasRecurrente, TareasRecurrentesDTO>()
                .ReverseMap();

            CreateMap<Usuario, UsuarioDTO>()
                .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => src.IdRolNavigation))
                .ForMember(dest => dest.Clave, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.IdRolNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.FechaActualizacion, opt => opt.Ignore());

            CreateMap<RegistroProceso, RegistroProcesoDTO>()
                .ReverseMap();
        }
    }
}