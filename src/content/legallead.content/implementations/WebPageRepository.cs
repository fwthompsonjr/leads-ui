using legallead.content.interfaces;

namespace legallead.content.implementations
{
    public class WebPageRepository : IWebPageRepository
    {
        private readonly IWebContentLineRepository lineRepository;
        private readonly IWebContentRepository contentRepository;

        public WebPageRepository(IWebContentLineRepository lineRepo, IWebContentRepository contentRepo)
        {
            lineRepository = lineRepo;
            contentRepository = contentRepo;
        }

        public IWebContentLineRepository LineRepository => lineRepository;
        public IWebContentRepository ContentRepository => contentRepository;
    }
}