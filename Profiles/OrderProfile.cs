// using AutoMapper;
// using Cloud9_2.Models;
// using Cloud9_2.Data;
// using Cloud9_2.Services;
// using Cloud9_2.Controllers;

// namespace Cloud9_2.Models
// {
//     public class OrderProfile : Profile
//     {
//         public OrderProfile()
//         {
//             CreateMap<Order, OrderDTO>()
//                 .ForMember(dest => dest.Partner, opt => opt.MapFrom(src => src.Partner))
//                 .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency))
//                 .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
//                 .ForMember(dest => dest.ShippingMethod, opt => opt.MapFrom(src => src.ShippingMethod))
//                 .ForMember(dest => dest.Contact, opt => opt.MapFrom(src => src.Contact))
//                 .ForMember(dest => dest.PaymentTerm, opt => opt.MapFrom(src => src.PaymentTerm))
//                 .ReverseMap();

//             CreateMap<OrderItem, OrderItemDTO>()
//                 .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
//                 .ReverseMap();
//         }
//     }
// }