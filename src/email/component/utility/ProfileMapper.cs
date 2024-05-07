using AutoMapper;
using legallead.email.interfaces;
using legallead.email.models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace legallead.email.utility
{
    internal static class ProfileMapper
    {
        private static MapperConfiguration? _configuration;
        private static IMapper? _mapper;
        private static MapperConfiguration Configuration => _configuration ??= GetConfiguration();
        public static IMapper Mapper => _mapper ??= Configuration.CreateMapper();

        private static MapperConfiguration GetConfiguration()
        {
            return new MapperConfiguration(c =>
            {
                c.CreateMap<OkObjectResult, ProfileChangedModel>()
                    .ConvertUsing(ConvertTo);
            });
        }

        private static ProfileChangedModel ConvertTo(OkObjectResult source, ProfileChangedModel dest)
        {
            dest ??= new();
            var payload = JsonConvert.SerializeObject(source.Value);
            var response = TryConvert<ProfileChangedResponse>(payload);
            if (response == null) return dest;
            dest = new(response);
            if (!ProfileChangeTypes.Contains(dest.Name, StringComparer.OrdinalIgnoreCase)) { return dest; }
            AppendProfiles(dest);
            return dest;
        }

        private static void AppendProfiles(ProfileChangedModel model)
        {
            var name = ProfileChangeTypes.Find(x => model.Name.Equals(x, StringComparison.OrdinalIgnoreCase));
            if (name == null) return;
            var jsdata = model.JsonData;
            switch (name)
            {
                case "Address":
                    var addresses = TryConvert<List<ProfileAddressChangedItem>>(jsdata) ?? new();
                    addresses.ForEach(a =>
                    {
                        if (a is IProfileChangeItem item) model.ChangeItems.Add(item);
                    });
                    return;
                case "Email":
                    var emails = TryConvert<List<ProfileEmailChangedItem>>(jsdata) ?? new();
                    emails.ForEach(a =>
                    {
                        if (a is IProfileChangeItem item) model.ChangeItems.Add(item);
                    });
                    return;
                case "Name":
                    var names = TryConvert<List<ProfileNameChangedItem>>(jsdata) ?? new();
                    names.ForEach(a =>
                    {
                        if (a is IProfileChangeItem item) model.ChangeItems.Add(item);
                    });
                    return;
                case "Phone":
                    var phones = TryConvert<List<ProfilePhoneChangedItem>>(jsdata) ?? new();
                    phones.ForEach(a =>
                    {
                        if (a is IProfileChangeItem item) model.ChangeItems.Add(item);
                    });
                    return;
            }
        }

        [ExcludeFromCodeCoverage(Justification = "Private method tested from public accessor")]
        private static T? TryConvert<T>(string value)
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

        private static readonly List<string> ProfileChangeTypes = new()
        {
            "Address",
            "Email",
            "Name",
            "Phone"
        };
    }
}
