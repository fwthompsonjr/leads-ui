using Bogus;
using legallead.content.entities;
using legallead.content.implementations;
using legallead.content.interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.content.tests.implementations
{
    public class PageBuilderTests
    {
        private const string preloginpage = "HOME.PRE.LOGIN";

        private readonly Faker<WebContentDto> contentFaker =
            new Faker<WebContentDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.InternalId, y => y.Random.Int(10, 20000))
            .RuleFor(x => x.VersionId, y => y.Random.Int(1, 1000))
            .RuleFor(x => x.IsActive, y => y.Random.Bool())
            .RuleFor(x => x.IsChild, y => y.Random.Bool())
            .RuleFor(x => x.ContentName, y =>
            {
                var suffix = y.Random.AlphaNumeric(15).ToUpper();
                return $"{preloginpage}.{suffix}";
            })
            .RuleFor(x => x.CreateDate, y => y.Date.Recent(300));

        private readonly Faker<WebContentLineDto> lineFaker =
            new Faker<WebContentLineDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ContentId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.LineNbr, y => y.Random.Int(10, 20000))
            .RuleFor(x => x.InternalId, y => y.Random.Int(1, 1000))
            .RuleFor(x => x.Content, y => y.Hacker.Phrase());

        [Theory]
        [InlineData(preloginpage)]
        public void PagesContainsExpectedName(string name)
        {
            var provider = TestContextProvider.GetTestFramework();
            var sut = provider.GetRequiredService<IPageBuilder>();
            Assert.Contains(name, sut.Pages);
        }

        [Fact]
        public async Task PagesCanNotGetContentWithInvalidPageName()
        {
            var fakeName = lineFaker.Generate().Content ?? string.Empty;
            var repo = new Mock<IWebPageRepository>();

            var sut = new PageBuilder(repo.Object);
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                _ = await sut.GetContent(fakeName);
            });
        }

        [Fact]
        public async Task PagesCanGetContent()
        {
            var builder = new ContentCollection(contentFaker, lineFaker);
            var repo = new Mock<IWebPageRepository>();
            var contentMock = new Mock<IWebContentRepository>();
            var linesMock = new Mock<IWebContentLineRepository>();
            var orderedData = new List<List<WebContentLineDto>>();
            foreach (var line in builder.Content)
            {
                var subset = builder.Lines.Where(x => x.ContentId == line.Id).ToList();
                orderedData.Add(subset);
            }
            repo.SetupGet(g => g.ContentRepository).Returns(contentMock.Object);
            repo.SetupGet(g => g.LineRepository).Returns(linesMock.Object);
            contentMock.Setup(m => m.GetAllActive()).ReturnsAsync(builder.Content);
            linesMock.SetupSequence(m => m.GetAll(It.IsAny<WebContentDto>()))
                .ReturnsAsync(orderedData[0])
                .ReturnsAsync(orderedData[1])
                .ReturnsAsync(orderedData[2])
                .ReturnsAsync(orderedData[3])
                .ReturnsAsync(orderedData[4])
                .ReturnsAsync(orderedData[5])
                .ReturnsAsync(orderedData[6])
                .ReturnsAsync(orderedData[7])
                .ReturnsAsync(orderedData[8])
                .ReturnsAsync(orderedData[9]);

            var sut = new PageBuilder(repo.Object);
            var response = await sut.GetContent(preloginpage);
            Assert.NotNull(response);
            Assert.NotEmpty(response);
        }

        [Fact]
        public async Task PagesCanGetContentWithKeys()
        {
            var builder = new ContentCollection(contentFaker, lineFaker);
            var repo = new Mock<IWebPageRepository>();
            var contentMock = new Mock<IWebContentRepository>();
            var linesMock = new Mock<IWebContentLineRepository>();
            var orderedData = new List<List<WebContentLineDto>>();
            foreach (var line in builder.Content)
            {
                var subset = builder.Lines.Where(x => x.ContentId == line.Id).ToList();
                orderedData.Add(subset);
            }
            repo.SetupGet(g => g.ContentRepository).Returns(contentMock.Object);
            repo.SetupGet(g => g.LineRepository).Returns(linesMock.Object);
            contentMock.Setup(m => m.GetAllActive()).ReturnsAsync(builder.Content);
            linesMock.SetupSequence(m => m.GetAll(It.IsAny<WebContentDto>()))
                .ReturnsAsync(orderedData[0])
                .ReturnsAsync(orderedData[1])
                .ReturnsAsync(orderedData[2])
                .ReturnsAsync(orderedData[3])
                .ReturnsAsync(orderedData[4])
                .ReturnsAsync(orderedData[5])
                .ReturnsAsync(orderedData[6])
                .ReturnsAsync(orderedData[7])
                .ReturnsAsync(orderedData[8])
                .ReturnsAsync(orderedData[9]);

            var sut = new PageBuilder(repo.Object);
            var keys = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>( "", "fund"),
                new KeyValuePair<string, string>( "{{abc}}", "real item"),
                new KeyValuePair<string, string>( "{{abc", "not a real item"),
                new KeyValuePair<string, string>( "abc}}", "not a real item"),
            };
            var response = await sut.GetContent(preloginpage, keys);
            Assert.NotNull(response);
            Assert.NotEmpty(response);
        }

        private sealed class ContentCollection
        {
            public ContentCollection(Faker<WebContentDto> content, Faker<WebContentLineDto> lines)
            {
                var web = content.Generate(10);
                var data = new List<WebContentLineDto>();
                web.ForEach(w =>
                {
                    var items = lines.Generate(5);
                    items.ForEach(ii => ii.ContentId = w.Id);
                    w.IsChild = web.IndexOf(w) >= 7;
                    data.AddRange(items);
                });
                Content = web;
                Lines = data;
            }

            public List<WebContentDto> Content { get; private set; }
            public List<WebContentLineDto> Lines { get; private set; }
        }
    }
}