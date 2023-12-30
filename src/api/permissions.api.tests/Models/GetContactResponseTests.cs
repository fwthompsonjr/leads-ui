using legallead.permissions.api.Model;

namespace permissions.api.tests.Models
{
    public class GetContactResponseTests
    {
        private static readonly string[] ContactResponseTypes = new[] { "Address", "Email", "Name", "Phone" };

        private static readonly Faker<GetContactResponse> faker =
            new Faker<GetContactResponse>()
                .RuleFor(x => x.IsOK, y => y.Random.Bool())
                .RuleFor(x => x.ResponseType, y => y.PickRandom(ContactResponseTypes))
                .RuleFor(x => x.Data, y => y.Random.AlphaNumeric(50))
                .RuleFor(x => x.Message, y => y.Hacker.Phrase());

        [Fact]
        public void ModelCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ModelCanGetIsOK()
        {
            var exception = Record.Exception(() =>
            {
                var test = faker.Generate();
                _ = test.IsOK;
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ModelCanSetIsOK()
        {
            var test = faker.Generate();
            var source = faker.Generate();
            test.IsOK = source.IsOK;
            Assert.Equal(source.IsOK, test.IsOK);
        }

        [Fact]
        public void ModelCanGetData()
        {
            var exception = Record.Exception(() =>
            {
                var test = faker.Generate();
                _ = test.Data;
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ModelCanSetData()
        {
            var test = faker.Generate();
            var source = faker.Generate();
            test.Data = source.Data;
            Assert.Equal(source.Data, test.Data);
        }

        [Fact]
        public void ModelCanGetMessage()
        {
            var exception = Record.Exception(() =>
            {
                var test = faker.Generate();
                _ = test.Message;
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ModelCanSetMessage()
        {
            var test = faker.Generate();
            var source = faker.Generate();
            test.Message = source.Message;
            Assert.Equal(source.Message, test.Message);
        }

        [Fact]
        public void ModelCanGetResponseType()
        {
            var exception = Record.Exception(() =>
            {
                var test = faker.Generate();
                _ = test.ResponseType;
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ModelCanSetResponseType()
        {
            var test = faker.Generate();
            var source = faker.Generate();
            test.ResponseType = source.ResponseType;
            Assert.Equal(source.ResponseType, test.ResponseType);
        }
    }
}