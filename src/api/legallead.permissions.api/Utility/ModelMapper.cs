using AutoMapper;
using legallead.jdbc.entities;
using legallead.permissions.api.Enumerations;
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

                c.CreateMap<UserProfileView, UserProfile>()
                    .ForMember(a => a.Id, opt => opt.MapFrom(b => b.Id))
                    .ForMember(a => a.UserId, opt => opt.MapFrom(b => b.UserId))
                    .ForMember(a => a.ProfileMapId, opt => opt.MapFrom(b => b.ProfileMapId))
                    .ForMember(a => a.KeyValue, opt => opt.MapFrom(b => b.KeyValue));

                c.CreateMap<ChangeContactAddressRequest, UserProfileView[]>()
                    .ConvertUsing(ConvertTo);

                c.CreateMap<ChangeContactEmailRequest, UserProfileView>()
                    .ConvertUsing(ConvertTo);

                c.CreateMap<ChangeContactNameRequest, UserProfileView>()
                    .ConvertUsing(ConvertTo);

                c.CreateMap<ChangeContactPhoneRequest, UserProfileView>()
                    .ConvertUsing(ConvertTo);

            });
        }

        private static UserProfileView[] ConvertTo(ChangeContactAddressRequest source, UserProfileView[] dest)
        {
            var hasName = Enum.TryParse<AddressTypeNames>(source.AddressType, out var index);
            char pipe = '|';
            var mappedId = hasName ? (int)index : 1;
            var prefix = AddressPrefixes[mappedId];
            var list = new List<UserProfileView>();
            if(string.IsNullOrEmpty(source.Address)) return list.ToArray();
            var pieces = source.Address.Split(pipe, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < pieces.Length; i++)
            {
                var item = new UserProfileView
                {
                    KeyName = $"{prefix} Address Line {i + 1}",
                    KeyValue = pieces[i].Trim(),   
                };
                list.Add(item);
            }
            return list.ToArray();
        }

        private static UserProfileView ConvertTo(ChangeContactEmailRequest source, UserProfileView dest)
        {
            var hasName = Enum.TryParse<EmailTypeNames>(source.EmailType, out var index);
            var mappedId = hasName ? (int)index : 1;
            return new UserProfileView
            {
                KeyName = $"Email {mappedId}",
                KeyValue = source.Email,
            };
        }

        private static UserProfileView ConvertTo(ChangeContactNameRequest source, UserProfileView dest)
        {
            var hasName = Enum.TryParse<NameTypeNames>(source.NameType, out var index);
            var mappedId = hasName ? (int)index : 1;
            return new UserProfileView
            {
                KeyName = NamePrefixes[mappedId],
                KeyValue = source.Name,
            };
        }

        private static UserProfileView ConvertTo(ChangeContactPhoneRequest source, UserProfileView dest)
        {
            var hasName = Enum.TryParse<PhoneTypeNames>(source.PhoneType, out var index);
            var mappedId = hasName ? (int)index : 1;
            return new UserProfileView
            {
                KeyName = $"Phone {mappedId}",
                KeyValue = source.Phone,
            };
        }

        private static readonly Dictionary<int, string> AddressPrefixes = new()
        {
            { 1, "Address 1 -" },
            { 2, "Address 2 -" }
        };
        private static readonly Dictionary<int, string> NamePrefixes = new()
        {
            { 1, "First Name" },
            { 2, "Last Name" },
            { 3, "Company Name" }
        };
    }
}