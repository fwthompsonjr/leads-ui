using legallead.logging.helpers;
using legallead.logging.implementations;

namespace legallead.logging.tests.testobj
{
    internal class TempDtoRepository : BaseLoggingDbRepository<TempDto>
    {
        public TempDtoRepository(LoggingDbContext context) : base(context)
        {
        }
    }
}