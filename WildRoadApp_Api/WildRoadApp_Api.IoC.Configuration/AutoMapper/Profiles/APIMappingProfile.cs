using AutoMapper;
using DC = WildRoadApp_Api.API.DataContracts;
using S = WildRoadApp_Api.Services.Model;

namespace WildRoadApp_Api.IoC.Configuration.AutoMapper.Profiles
{
    public class APIMappingProfile : Profile
    {
        public APIMappingProfile()
        {
            CreateMap<DC.User, S.User>().ReverseMap();
            CreateMap<DC.Adress, S.Adress>().ReverseMap();
        }
    }
}
