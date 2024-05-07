using legallead.email.interfaces;
using legallead.email.models;
using Microsoft.AspNetCore.Mvc;

namespace legallead.email.tests
{
    internal static class ProfileMockInfrastructure
    {
        private static readonly string[] addressTypes = ["Business", "Home", "Mailing"];
        private static readonly string[] emailTypes = ["Business", "Home", "Personal", "Other"];
        private static readonly string[] nameTypes = ["First Name", "Last Name", "FullName"];

        internal static readonly Faker<ProfileAddressChangedItem> AddressChangeFaker =
            new Faker<ProfileAddressChangedItem>()
            .RuleFor(x => x.FieldName, y => y.PickRandom(addressTypes))
            .RuleFor(x => x.Description, y =>
            {
                var address = y.Person.Address;
                return $"{address.Street} {address.City}, {address.State} {address.ZipCode}";
            });

        internal static readonly Faker<ProfileEmailChangedItem> EmailChangeFaker =
            new Faker<ProfileEmailChangedItem>()
            .RuleFor(x => x.FieldName, y => y.PickRandom(emailTypes))
            .RuleFor(x => x.Description, y => y.Person.Email);

        internal static readonly Faker<ProfileNameChangedItem> NameChangeFaker =
            new Faker<ProfileNameChangedItem>()
            .RuleFor(x => x.FieldName, y => y.PickRandom(nameTypes))
            .RuleFor(x => x.Description, y => y.Person.FullName)
            .FinishWith((a, b) =>
            {
                if (b.FieldName == "First Name") b.Description = a.Person.FirstName;
                if (b.FieldName == "Last Name") b.Description = a.Person.LastName;
            });

        internal static readonly Faker<ProfilePhoneChangedItem> PhoneChangeFaker =
            new Faker<ProfilePhoneChangedItem>()
            .RuleFor(x => x.FieldName, y => y.PickRandom(emailTypes))
            .RuleFor(x => x.Description, y => y.Person.Phone);
        internal static string GetChangeType()
        {
            var faker = new Faker();
            List<string> names = ["Address", "Email", "Name", "Phone"];
            return faker.PickRandom(names);

        }
        internal static IActionResult GetResult(int statusCode, string changeType)
        {
            var faker = new Faker();
            var count = faker.Random.Int(1, 8);
            var list = JsonConvert.SerializeObject(GetList(count, changeType));
            var response = new ProfileChangedResponse
            {
                Email = faker.Person.Email,
                Name = changeType,
                Message = faker.Hacker.Phrase(),
                JsonData = list
            };

            return statusCode == 200 ?
                new OkObjectResult(response) :
                new ObjectResult(response) { StatusCode = statusCode };
        }

        private static List<IProfileChangeItem> GetList(int count, string name)
        {
            if (name == "Address")
                return ConvertFrom(AddressChangeFaker.Generate(count));
            if (name == "Email")
                return ConvertFrom(EmailChangeFaker.Generate(count));
            if (name == "Name")
                return ConvertFrom(NameChangeFaker.Generate(count));
            if (name == "Phone")
                return ConvertFrom(PhoneChangeFaker.Generate(count));
            return [];
        }

        private static List<IProfileChangeItem> ConvertFrom<T>(List<T> items)
        {
            var list = new List<IProfileChangeItem>();
            items.ForEach(i =>
            {
                if (i is IProfileChangeItem item) list.Add(item);
            });
            return list;
        }
    }
}
