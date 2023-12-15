using AutoMapper;
using ProInterStore_Domain.DTOModels;
using ProInterStore_Domain.EntityModels;

namespace ProInterStore_Shared.Mapper
{
    public class AutoMapperConfig
    {
        public static IMapper Configure()
        {
            var config = new MapperConfiguration(cfg =>
            {
                #region StoreItem and Atributte

                cfg.CreateMap<StoreItem, StoreItemDTO>()
                            .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes));

                cfg.CreateMap<StoreItemAttribute, StoreItemAttributeDTO>();

                cfg.CreateMap<StoreItemDTO, StoreItem>()
                    .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes));

                cfg.CreateMap<StoreItemAttributeDTO, StoreItemAttribute>();

                #endregion

                #region User and UserInfo

                cfg.CreateMap<User, UserDto>()
                .ForMember(dest => dest.ConfirmPassword, opt => opt.Ignore());

                cfg.CreateMap<UserDto, User>();


                #endregion
            });

            return config.CreateMapper();
        }
    }
}
