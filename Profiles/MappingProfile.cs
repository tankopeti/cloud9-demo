using AutoMapper;
using Cloud9_2.Models;


    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateQuoteDto, Quote>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status ?? "Folyamatban"))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy ?? "System"))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => "System"))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
;

            CreateMap<UpdateQuoteDto, Quote>()
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.ModifiedBy ?? "System"))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null)); // Csak nem null
        }
    }
