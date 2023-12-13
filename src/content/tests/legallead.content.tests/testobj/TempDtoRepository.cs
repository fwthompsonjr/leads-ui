using legallead.content.helpers;
using legallead.content.implementations;

namespace legallead.content.tests.testobj
{
    internal class TempDtoRepository : BaseContentDbRepository<TempDto>
    {
        public TempDtoRepository(ContentDbContext context) : base(context)
        {
        }
    }
}