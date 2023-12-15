using legallead.content.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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