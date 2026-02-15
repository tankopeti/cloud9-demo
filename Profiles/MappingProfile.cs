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
                .ForMember(dest => dest.QuoteItems, opt => opt.MapFrom(src => src.QuoteItems));

            CreateMap<CreateQuoteItemDto, QuoteItem>()
                .ForMember(dest => dest.DiscountTypeId, opt => opt.MapFrom(src => src.DiscountTypeId))
                .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.DiscountAmount))
                .ForMember(dest => dest.PartnerPrice, opt => opt.MapFrom(src => src.PartnerPrice))
                .ForMember(dest => dest.VolumePrice, opt => opt.MapFrom(src => src.VolumePrice));

            CreateMap<UpdateQuoteDto, Quote>()
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.ModifiedBy ?? "System"))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.QuoteItems, opt => opt.MapFrom(src => src.QuoteItems));

            CreateMap<UpdateQuoteItemDto, QuoteItem>()
                .ForMember(dest => dest.DiscountTypeId, opt => opt.MapFrom(src => src.DiscountTypeId))
                .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.DiscountAmount))
                .ForMember(dest => dest.PartnerPrice, opt => opt.MapFrom(src => src.PartnerPrice))
                .ForMember(dest => dest.VolumePrice, opt => opt.MapFrom(src => src.VolumePrice));
        }
    }
