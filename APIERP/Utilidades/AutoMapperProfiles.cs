using AutoMapper;
using APIERP.DTOs;
using APIERP.Entidades;

namespace APIERP.Utilidades
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CategoryDTOAdd, Category>()
                .ForMember(x => x.ImageUrl, opciones => opciones.Ignore());
            CreateMap<Category, CategoryDTO>();

            CreateMap<StoreDTOAdd, Store>();
            CreateMap<Store, StoreDTO>();

            CreateMap<ProductDTOAdd, Product>();
            CreateMap<Product, ProductDTO>();

            CreateMap<OrderDTOAdd, Order>();
            CreateMap<Order, OrderDTO>();

            CreateMap<OrderDetailDTOAdd, OrderDetail>();
            CreateMap<OrderDetailDTOMultiAdd, OrderDetail>();
            CreateMap<OrderDetail, OrderDetailDTO>();
            

            CreateMap<RaincheckDTOAdd, Raincheck>();
            CreateMap<Raincheck, RaincheckDTO>();

            CreateMap<CartItemDTOAdd, CartItem>();
            CreateMap<CartItem, CartItemWithPriceDTO>();
            CreateMap<CartItem, CartItemDTO>();
        }
    }
}
