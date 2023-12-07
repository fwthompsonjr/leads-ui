using AutoMapper;
using legallead.jdbc.entities;
using legallead.permissions.api.Model;

namespace legallead.permissions.api
{
    public static class ModelMapper
    {
        private static MapperConfiguration? _configuration;
        private static IMapper? _mapper;
        private static MapperConfiguration Configuration => _configuration ??= GetConfiguration();
        public static IMapper Mapper => _mapper ??= Configuration.CreateMapper();

        private static MapperConfiguration GetConfiguration()
        {
            return new MapperConfiguration(c =>
            {
                c.CreateMap<PermissionGroup, PermissionGroupModel>()
                    .ForMember(a => a.Name, opt => opt.MapFrom(b => b.Name))
                    .ForMember(a => a.GroupId, opt => opt.MapFrom(b => b.GroupId))
                    .ForMember(a => a.OrderId, opt => opt.MapFrom(b => b.OrderId))
                    .ForMember(a => a.PerRequest, opt => opt.MapFrom(b => b.PerRequest))
                    .ForMember(a => a.PerMonth, opt => opt.MapFrom(b => b.PerMonth))
                    .ForMember(a => a.PerYear, opt => opt.MapFrom(b => b.PerYear))
                    .ForMember(a => a.IsActive, opt => opt.MapFrom(b => b.IsActive));

                c.CreateMap<UserProfileView, UserProfileModel>()
                    .ForMember(a => a.Id, opt => opt.MapFrom(b => b.Id))
                    .ForMember(a => a.KeyName, opt => opt.MapFrom(b => b.KeyName))
                    .ForMember(a => a.KeyValue, opt => opt.MapFrom(b => b.KeyValue));

                c.CreateMap<UserPermissionView, UserProfileModel>()
                    .ForMember(a => a.Id, opt => opt.MapFrom(b => b.Id))
                    .ForMember(a => a.KeyName, opt => opt.MapFrom(b => b.KeyName))
                    .ForMember(a => a.KeyValue, opt => opt.MapFrom(b => b.KeyValue));
            });
        }
    }
}