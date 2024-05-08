using legallead.email.models;
using Microsoft.AspNetCore.Mvc;

namespace legallead.email.tests
{
    internal static class PermissionMockInfrastructure
    {
        private static readonly List<string> changeTypes =
        [
            "Discount",
            "PermissionLevel"
        ];
        private static readonly List<string> LevelTypes = [.. "Platinum,Gold,Silver,Guest".Split(',')];
        private static readonly List<string> Counties =
            [.. "Harris,Denton,Tarrant,Dallas,Travis,Houston".Split(',')];
        private static readonly List<string> StateCodes =
            [.. "TX,AZ,CO,NM,GA,MO".Split(',')];

        private static readonly Faker<PermissionDiscountChoice> DiscountChoiceFaker =
            new Faker<PermissionDiscountChoice>()
                .RuleFor(x => x.IsSelected, y =>
                {
                    return y.Random.Double() > 0.3f;
                })
            .RuleFor(a => a.CountyName, y => y.PickRandom(Counties))
            .RuleFor(a => a.StateName, y => y.PickRandom(StateCodes));

        internal static readonly Faker<PermissionDiscountRequest> DiscountRequestFaker
            = new Faker<PermissionDiscountRequest>()
            .RuleFor(x => x.Choices, y =>
            {
                var n = y.Random.Int(5, 25);
                return DiscountChoiceFaker.Generate(n);
            });

        internal static readonly Faker<PermissionLevelRequest> PermissionLevelFaker
            = new Faker<PermissionLevelRequest>()
            .RuleFor(x => x.Level, y => y.PickRandom(LevelTypes));

        internal static readonly Faker<PermissionLevelResponseBo> BoFaker =
            new Faker<PermissionLevelResponseBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ExternalId, y => y.Hacker.Phrase())
            .RuleFor(x => x.InvoiceUri, y => y.Hacker.Phrase())
            .RuleFor(x => x.LevelName, y => y.Hacker.Phrase())
            .RuleFor(x => x.SessionId, y => y.Hacker.Phrase())
            .RuleFor(x => x.IsPaymentSuccess, y => y.Random.Bool())
            .RuleFor(x => x.CompletionDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        internal static readonly Faker<PermissionChangeResponse> ApiResponseFaker
            = new Faker<PermissionChangeResponse>()
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.Name, y => y.PickRandom(changeTypes))
            .RuleFor(x => x.Request, y => y.Hacker.Phrase())
            .RuleFor(x => x.Dto, y => BoFaker.Generate())
            .FinishWith((a, b) =>
            {
                object obj = b.Name == "Discount" ?
                    DiscountRequestFaker.Generate() :
                    PermissionLevelFaker.Generate();
                var js = JsonConvert.SerializeObject(obj);
                b.Request = js;
            });
        internal static string GetChangeType()
        {
            var faker = new Faker();
            return faker.PickRandom(changeTypes);

        }

        internal static IActionResult GetResult(int statusCode, string changeType)
        {
            if (!changeTypes.Contains(changeType, StringComparer.OrdinalIgnoreCase))
            {
                return new BadRequestResult();
            }

            var response = ApiResponseFaker.Generate();
            while (!response.Name.Equals(changeType, StringComparison.OrdinalIgnoreCase))
            {
                response = ApiResponseFaker.Generate();
            }
            return statusCode == 200 ?
                new OkObjectResult(response) :
                new ObjectResult(response) { StatusCode = statusCode };
        }
    }
}
