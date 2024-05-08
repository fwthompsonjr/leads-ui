using AutoMapper;
using legallead.email.models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace legallead.email.utility
{
    internal static class PermissionMapper
    {
        private static MapperConfiguration? _configuration;
        private static IMapper? _mapper;
        private static MapperConfiguration Configuration => _configuration ??= GetConfiguration();
        public static IMapper Mapper => _mapper ??= Configuration.CreateMapper();

        private static MapperConfiguration GetConfiguration()
        {
            return new MapperConfiguration(c =>
            {
                c.CreateMap<OkObjectResult, PermissionChangeResponse>()
                    .ConvertUsing(ConvertTo);
                c.CreateMap<PermissionChangeResponse, PermissionChangeValidation>()
                    .ConvertUsing(ConvertTo);
            });
        }

        
        private static PermissionChangeValidation ConvertTo(PermissionChangeResponse source, PermissionChangeValidation dest)
        {
            dest ??= new();
            var name = IsMappable(source);
            if (string.IsNullOrEmpty(name)) return dest;
            if (string.IsNullOrEmpty(source.Email)) return dest;
            if (source.DiscountRequest == null && name == "Discount") return dest;
            if (source.LevelRequest == null && name == "PermissionLevel") return dest;
            dest.IsValid = true;
            return dest;
        }

        [ExcludeFromCodeCoverage(Justification = "Private method tested from public accessor")]
        private static PermissionChangeResponse ConvertTo(OkObjectResult source, PermissionChangeResponse dest)
        {
            dest ??= new();
            var payload = JsonConvert.SerializeObject(source.Value);
            var response = TryConvert<PermissionChangeResponse>(payload);
            if (response == null) return dest;
            if (!PermissionChangeTypes.Contains(response.Name, StringComparer.OrdinalIgnoreCase)) { return response; }
            AppendPermissions(response);
            return response;
        }
        
        private static void AppendPermissions(PermissionChangeResponse model)
        {
            var name = IsMappable(model);
            if(string.IsNullOrEmpty(name)) return;
            var jsdata = model.Request;
            switch (name)
            {
                case "Discount":
                    var discounts = TryConvert<PermissionDiscountRequest>(jsdata) ?? new();
                    var list = discounts.Choices.ToList();
                    list.Sort((a, b) =>
                    {
                        // sort by selection
                        var aa = a.IsSelected ? 1 : -1;
                        var bb = b.IsSelected ? 1 : -1;
                        var cc = aa.CompareTo(bb);
                        if (cc != 0) return cc;
                        // then by state name
                        var dd = a.StateName.CompareTo(b.StateName);
                        if (dd != 0) return dd;
                        // then by county
                        return a.CountyName.CompareTo(b.CountyName);
                    });
                    discounts.Choices = list;
                    model.DiscountRequest = discounts;
                    model.LevelRequest = null;
                    return;
                case "PermissionLevel":
                    var levels = TryConvert<PermissionLevelRequest>(jsdata) ?? new();
                    model.LevelRequest = levels;
                    model.DiscountRequest = null;
                    return;
            }
        }

        private static string? IsMappable(PermissionChangeResponse model)
        {
            var name = PermissionChangeTypes.Find(x => model.Name.Equals(x, StringComparison.OrdinalIgnoreCase));
            return name;
        }

        [ExcludeFromCodeCoverage(Justification = "Private method tested from public accessor")]
        internal static T? TryConvert<T>(string value)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch
            {
                return default;
            }
        }

        private static readonly List<string> PermissionChangeTypes =
        [
            "Discount",
            "PermissionLevel"
        ];
    }
}