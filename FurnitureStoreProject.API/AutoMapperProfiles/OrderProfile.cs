﻿using AutoMapper;



namespace FurnitureStore.API.AutoMapperProfiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile() 
        {
            CreateMap<Entities.Order, DTOs.OrderDTOs.OrderDto>();
            CreateMap<DTOs.OrderDTOs.OrderDto, Entities.Order>();
            //CreateMap<Entities.Order, DTOs.OrderDTOs.OrderToCreateDto>();
            //CreateMap<DTOs.OrderDTOs.OrderToCreateDto, Entities.Order>();
            CreateMap<Entities.Order, DTOs.OrderDTOs.OrderDto >()
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails ?? new List<Entities.OrderDetail>()));

            CreateMap<DTOs.OrderDetailDTOs.OrderDetailDto, Entities.OrderDetail>();

            CreateMap<DTOs.OrderDTOs.OrderToCreateDto, Entities.Order>()
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails));

            CreateMap<DTOs.OrderDTOs.OrderToUpdateDto, Entities.Order>()
                    .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails));

        }
    }
}
