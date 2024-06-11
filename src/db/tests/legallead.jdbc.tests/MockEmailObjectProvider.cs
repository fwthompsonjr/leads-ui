using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests
{
    internal static class MockEmailObjectProvider
    {
        public static readonly Faker<EmailBodyDto> BodyDtoFaker =
            new Faker<EmailBodyDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Body, y => y.Hacker.Phrase());

        public static readonly Faker<EmailCountDto> CountDtoFaker =
            new Faker<EmailCountDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Items, y => y.Random.Int(1, 500));

        public static readonly Faker<EmailListDto> ListDtoFaker =
            new Faker<EmailListDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.FromAddress, y => y.Person.Email)
            .RuleFor(x => x.ToAddress, y => y.Person.Email)
            .RuleFor(x => x.Subject, y => y.Hacker.Phrase())
            .RuleFor(x => x.StatusId, y => y.Random.Int(1, 500))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        public static readonly Faker<EmailBodyBo> BodyBoFaker =
            new Faker<EmailBodyBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Body, y => y.Hacker.Phrase());

        public static readonly Faker<EmailCountBo> CountBoFaker =
            new Faker<EmailCountBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Items, y => y.Random.Int(1, 500));

        public static readonly Faker<EmailListBo> ListBoFaker =
            new Faker<EmailListBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.FromAddress, y => y.Person.Email)
            .RuleFor(x => x.ToAddress, y => y.Person.Email)
            .RuleFor(x => x.Subject, y => y.Hacker.Phrase())
            .RuleFor(x => x.StatusId, y => y.Random.Int(1, 500))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());
    }
}
