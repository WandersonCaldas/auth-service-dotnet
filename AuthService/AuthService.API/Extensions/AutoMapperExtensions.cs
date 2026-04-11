using AutoMapper;

namespace AuthService.API.Extensions
{
    public static class AutoMapperExtensions
    {
        public static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
        {
            var config = new MapperConfiguration(cfg =>
            {
                //cfg.CreateMap<Profile, ProfileResponseDto>().ReverseMap();                
            });

            IMapper Mapper = config.CreateMapper();
            services.AddSingleton(Mapper);

            return services;
        }
    }
}
