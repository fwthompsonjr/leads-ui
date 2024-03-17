using AutoMapper;
using legallead.jdbc.entities;
using legallead.json.db.entity;
using legallead.permissions.api.Enumerations;
using legallead.permissions.api.Model;
using legallead.permissions.api.Models;
using Newtonsoft.Json;

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

                c.CreateMap<ChangeContactAddressRequest, UserProfileView[]>()
                    .ConvertUsing(ConvertTo);

                c.CreateMap<ChangeContactEmailRequest, UserProfileView>()
                    .ConvertUsing(ConvertTo);

                c.CreateMap<ChangeContactNameRequest, UserProfileView>()
                    .ConvertUsing(ConvertTo);

                c.CreateMap<ChangeContactPhoneRequest, UserProfileView>()
                    .ConvertUsing(ConvertTo);

                c.CreateMap<UserProfileView, UserProfile>()
                    .ConvertUsing(ConvertTo);

                c.CreateMap<UserProfileView[], ChangeContactAddressRequest[]>()
                    .ConvertUsing(ConvertTo);

                c.CreateMap<UserProfileView[], ChangeContactEmailRequest[]>()
                    .ConvertUsing(ConvertTo);

                c.CreateMap<UserProfileView[], ChangeContactPhoneRequest[]>()
                    .ConvertUsing(ConvertTo);

                c.CreateMap<UserProfileView[], ChangeContactNameRequest[]>()
                    .ConvertUsing(ConvertTo);

                c.CreateMap<UserProfileView[], GetContactResponse[]>()
                    .ConvertUsing(ConvertTo);

                c.CreateMap<ChangeContactAddressRequest[], UserProfileView[]>()
                    .ConvertUsing(ConvertTo);

                c.CreateMap<ChangeContactEmailRequest[], UserProfileView[]>()
                    .ConvertUsing(ConvertTo);

                c.CreateMap<ChangeContactPhoneRequest[], UserProfileView[]>()
                    .ConvertUsing(ConvertTo);

                c.CreateMap<ChangeContactNameRequest[], UserProfileView[]>()
                    .ConvertUsing(ConvertTo);

                c.CreateMap<ChangeDiscountRequest, List<KeyValuePair<bool, UsState>>>()
                    .ConvertUsing(ConvertTo);

                c.CreateMap<ChangeDiscountRequest, List<KeyValuePair<bool, UsStateCounty>>>()
                    .ConvertUsing(ConvertTo);

                c.CreateMap<DiscountChoice, DiscountChangeRequest>()
                    .ForMember(a => a.StateName, opt => opt.MapFrom(b => b.StateName))
                    .ForMember(a => a.CountyName, opt => opt.MapFrom(b => b.CountyName))
                    .ForMember(a => a.IsSelected, opt => opt.MapFrom(b => b.IsSelected));

                c.CreateMap<DiscountChangeRequest, DiscountChoice>()
                    .ForMember(a => a.StateName, opt => opt.MapFrom(b => b.StateName))
                    .ForMember(a => a.CountyName, opt => opt.MapFrom(b => b.CountyName))
                    .ForMember(a => a.IsSelected, opt => opt.MapFrom(b => b.IsSelected));

                c.CreateMap<ChangeDiscountRequest, DiscountChangeParent>()
                    .ConvertUsing(ConvertTo);

                c.CreateMap<DiscountChangeParent, ChangeDiscountRequest>()
                    .ConvertUsing(ConvertTo);
            });
        }

        private static UserProfileView[] ConvertTo(ChangeContactAddressRequest[] source, UserProfileView[] dest)
        {
            var list = new List<UserProfileView>();
            foreach (var item in source)
            {
                var addition = ConvertTo(item, Array.Empty<UserProfileView>());
                list.AddRange(addition);
            }
            return list.ToArray();
        }

        private static UserProfileView[] ConvertTo(ChangeContactEmailRequest[] source, UserProfileView[] dest)
        {
            var list = new List<UserProfileView>();
            foreach (var item in source)
            {
                var addition = ConvertTo(item, new UserProfileView());
                list.Add(addition);
            }
            return list.ToArray();
        }

        private static UserProfileView[] ConvertTo(ChangeContactPhoneRequest[] source, UserProfileView[] dest)
        {
            var list = new List<UserProfileView>();
            foreach (var item in source)
            {
                var addition = ConvertTo(item, new UserProfileView());
                list.Add(addition);
            }
            return list.ToArray();
        }

        private static UserProfileView[] ConvertTo(ChangeContactNameRequest[] source, UserProfileView[] dest)
        {
            var list = new List<UserProfileView>();
            foreach (var item in source)
            {
                var addition = ConvertTo(item, new UserProfileView());
                list.Add(addition);
            }
            return list.ToArray();
        }

        private static UserProfileView[] ConvertTo(ChangeContactAddressRequest source, UserProfileView[] dest)
        {
            var hasName = Enum.TryParse<AddressTypeNames>(source.AddressType, out var index);
            char pipe = '|';
            var mappedId = hasName ? (int)index : 1;
            var prefix = AddressPrefixes[mappedId];
            var list = new List<UserProfileView>();
            if (string.IsNullOrEmpty(source.Address)) return list.ToArray();
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
            const char space = ' ';
            var named = source.NameType ?? string.Empty;
            if (named.Contains(space)) named = named.Split(space)[0];
            var hasName = Enum.TryParse<NameTypeNames>(named, out var index);
            var mappedId = hasName ? (int)index : 1;
            var mappedName = NamePrefixes[mappedId];
            return new UserProfileView
            {
                KeyName = $"{mappedName} Name",
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

        private static UserProfile ConvertTo(UserProfileView source, UserProfile dest)
        {
            return new UserProfile
            {
                Id = source.Id,
                UserId = source.UserId ?? string.Empty,
                ProfileMapId = source.ProfileMapId ?? string.Empty,
                KeyValue = source.KeyValue ?? string.Empty,
            };
        }

        private static GetContactResponse[] ConvertTo(UserProfileView[] source, GetContactResponse[] dest)
        {
            var response = new List<GetContactResponse>();
            var addresses = ConvertTo(ConvertTo(source, Array.Empty<ChangeContactAddressRequest>()), new GetContactResponse());
            var emails = ConvertTo(ConvertTo(source, Array.Empty<ChangeContactEmailRequest>()), new GetContactResponse());
            var phones = ConvertTo(ConvertTo(source, Array.Empty<ChangeContactPhoneRequest>()), new GetContactResponse());
            var names = ConvertTo(ConvertTo(source, Array.Empty<ChangeContactNameRequest>()), new GetContactResponse());
            response.Add(names);
            response.Add(addresses);
            response.Add(emails);
            response.Add(phones);
            return response.ToArray();
        }

        private static ChangeContactAddressRequest[] ConvertTo(UserProfileView[] source, ChangeContactAddressRequest[] dest)
        {
            const string mailingKey = "Address 1";
            const string billingKey = "Address 2";
            const string joiner = " ";
            var response = new List<ChangeContactAddressRequest>();
            var mailing = source.Where(w => (w.KeyName ?? string.Empty).StartsWith(mailingKey)).Select(x => x.KeyValue);
            var billing = source.Where(w => (w.KeyName ?? string.Empty).StartsWith(billingKey)).Select(x => x.KeyValue);
            response.Add(new ChangeContactAddressRequest { Address = string.Join(joiner, mailing), AddressType = "Mailing" });
            response.Add(new ChangeContactAddressRequest { Address = string.Join(joiner, billing), AddressType = "Billing" });
            return response.ToArray();
        }

        private static ChangeContactEmailRequest[] ConvertTo(UserProfileView[] source, ChangeContactEmailRequest[] dest)
        {
            const string selector = "Email";
            const string emaila = $"{selector} 1";
            const string emailb = $"{selector} 2";
            const string emailc = $"{selector} 3";
            var list = source.Where(w => (w.KeyName ?? string.Empty).StartsWith(selector)).ToList();
            var response = new List<ChangeContactEmailRequest>();
            var first = list.Find(w => (w.KeyName ?? string.Empty).Equals(emaila)) ?? new();
            var second = list.Find(w => (w.KeyName ?? string.Empty).Equals(emailb)) ?? new();
            var third = list.Find(w => (w.KeyName ?? string.Empty).Equals(emailc)) ?? new();
            response.Add(new ChangeContactEmailRequest { Email = first.KeyValue, EmailType = "Personal" });
            response.Add(new ChangeContactEmailRequest { Email = second.KeyValue, EmailType = "Business" });
            response.Add(new ChangeContactEmailRequest { Email = third.KeyValue, EmailType = "Other" });
            return response.ToArray();
        }

        private static ChangeContactPhoneRequest[] ConvertTo(UserProfileView[] source, ChangeContactPhoneRequest[] dest)
        {
            const string selector = "Phone";
            const string phonea = $"{selector} 1";
            const string phoneb = $"{selector} 2";
            const string phonec = $"{selector} 3";
            var list = source.Where(w => (w.KeyName ?? string.Empty).StartsWith(selector)).ToList();
            var response = new List<ChangeContactPhoneRequest>();
            var first = list.Find(w => (w.KeyName ?? string.Empty).Equals(phonea)) ?? new();
            var second = list.Find(w => (w.KeyName ?? string.Empty).Equals(phoneb)) ?? new();
            var third = list.Find(w => (w.KeyName ?? string.Empty).Equals(phonec)) ?? new();
            response.Add(new ChangeContactPhoneRequest { Phone = first.KeyValue, PhoneType = "Personal" });
            response.Add(new ChangeContactPhoneRequest { Phone = second.KeyValue, PhoneType = "Business" });
            response.Add(new ChangeContactPhoneRequest { Phone = third.KeyValue, PhoneType = "Other" });
            return response.ToArray();
        }

        private static ChangeContactNameRequest[] ConvertTo(UserProfileView[] source, ChangeContactNameRequest[] dest)
        {
            const string selector = "Name";
            const string namea = $"First {selector}";
            const string nameb = $"Last {selector}";
            const string namec = $"Company {selector}";
            var list = source.Where(w => (w.KeyName ?? string.Empty).EndsWith(selector)).ToList();
            var response = new List<ChangeContactNameRequest>();
            var first = list.Find(w => (w.KeyName ?? string.Empty).Equals(namea)) ?? new();
            var second = list.Find(w => (w.KeyName ?? string.Empty).Equals(nameb)) ?? new();
            var third = list.Find(w => (w.KeyName ?? string.Empty).Equals(namec)) ?? new();
            response.Add(new ChangeContactNameRequest { Name = first.KeyValue, NameType = "First" });
            response.Add(new ChangeContactNameRequest { Name = second.KeyValue, NameType = "Last" });
            response.Add(new ChangeContactNameRequest { Name = third.KeyValue, NameType = "Company" });
            return response.ToArray();
        }

        private static GetContactResponse ConvertTo(ChangeContactNameRequest[] source, GetContactResponse dest)
        {
            dest.IsOK = true;
            dest.ResponseType = "Name";
            dest.Data = JsonConvert.SerializeObject(source);
            dest.Message = "Mapping completed";
            return dest;
        }

        private static GetContactResponse ConvertTo(ChangeContactAddressRequest[] source, GetContactResponse dest)
        {
            dest.IsOK = true;
            dest.ResponseType = "Address";
            dest.Data = JsonConvert.SerializeObject(source);
            dest.Message = "Mapping completed";
            return dest;
        }

        private static GetContactResponse ConvertTo(ChangeContactEmailRequest[] source, GetContactResponse dest)
        {
            dest.IsOK = true;
            dest.ResponseType = "Email";
            dest.Data = JsonConvert.SerializeObject(source);
            dest.Message = "Mapping completed";
            return dest;
        }

        private static GetContactResponse ConvertTo(ChangeContactPhoneRequest[] source, GetContactResponse dest)
        {
            dest.IsOK = true;
            dest.ResponseType = "Phone";
            dest.Data = JsonConvert.SerializeObject(source);
            dest.Message = "Mapping completed";
            return dest;
        }

        private static List<KeyValuePair<bool, UsState>> ConvertTo(ChangeDiscountRequest source, List<KeyValuePair<bool, UsState>> dest)
        {
            dest ??= new();
            var choices = source.Choices.Where(s =>
                string.IsNullOrEmpty(s.CountyName) &&
                !string.IsNullOrEmpty(s.StateName));
            foreach (var request in choices)
            {
                var item = request.ToState();
                if (item == null || !item.IsActive) continue;
                dest.Add(new KeyValuePair<bool, UsState>(request.IsSelected, item));
            }
            return dest;
        }

        private static List<KeyValuePair<bool, UsStateCounty>> ConvertTo(ChangeDiscountRequest source, List<KeyValuePair<bool, UsStateCounty>> dest)
        {
            dest ??= new();
            var choices = source.Choices.Where(s =>
                !string.IsNullOrEmpty(s.CountyName) &&
                !string.IsNullOrEmpty(s.StateName));
            foreach (var request in choices)
            {
                var item = request.ToCounty();
                if (item == null || !item.IsActive) continue;
                dest.Add(new KeyValuePair<bool, UsStateCounty>(request.IsSelected, item));
            }
            return dest;
        }

        private static DiscountChangeParent ConvertTo(ChangeDiscountRequest source, DiscountChangeParent dest)
        {
            dest ??= new();
            var choices = new List<DiscountChangeRequest>();
            foreach (var request in source.Choices)
            {
                var item = Mapper.Map<DiscountChoice, DiscountChangeRequest>(request);
                if (item == null) continue;
                choices.Add(item);
            }
            dest.Choices = choices;
            return dest;
        }

        private static ChangeDiscountRequest ConvertTo(DiscountChangeParent source, ChangeDiscountRequest dest)
        {
            dest ??= new();
            var choices = new List<DiscountChoice>();
            foreach (var request in source.Choices)
            {
                var item = Mapper.Map<DiscountChangeRequest, DiscountChoice>(request);
                if (item == null) continue;
                choices.Add(item);
            }
            dest.Choices = choices;
            return dest;
        }
        private static readonly Dictionary<int, string> AddressPrefixes = new()
        {
            { 1, "Address 1 -" },
            { 2, "Address 2 -" }
        };

        private static readonly Dictionary<int, string> NamePrefixes = new()
        {
            { 0, "First" },
            { 1, "Last" },
            { 2, "Company" },
            { 3, "Time" }
        };
    }
}