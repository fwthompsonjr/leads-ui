using legallead.content.entities;
using legallead.content.extensions;
using legallead.content.helpers;
using legallead.content.interfaces;

namespace legallead.content.implementations
{
    public class WebContentLineRepository : BaseContentDbRepository<WebContentLineDto>, IWebContentLineRepository
    {
        public WebContentLineRepository(ContentDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<WebContentLineDto>> GetAll(WebContentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Id))
                throw new ArgumentOutOfRangeException(nameof(dto));

            using var connection = _context.CreateConnection();
            var parm = new WebContentLineDto { ContentId = dto.Id };
            var sql = _sut.SelectSQL(parm);
            var parms = _sut.SelectParameters(parm);
            return Sort(await _command.QueryAsync<WebContentLineDto>(connection, sql, parms));
        }

        public async Task<IEnumerable<WebContentLineDto>> GetByInternalId(int internalId)
        {
            using var connection = _context.CreateConnection();
            var parm = new WebContentLineDto { InternalId = internalId };
            var sql = _sut.SelectSQL(parm);
            var parms = _sut.SelectParameters(parm);
            return Sort(await _command.QueryAsync<WebContentLineDto>(connection, sql, parms));
        }

        private static IEnumerable<WebContentLineDto> Sort(IEnumerable<WebContentLineDto> items)
        {
            var list = items.ToList();
            list.Sort((a, b) =>
            {
                var aa = a.InternalId.GetValueOrDefault().CompareTo(b.InternalId.GetValueOrDefault());
                if (aa != 0) return aa;
                return a.LineNbr.GetValueOrDefault().CompareTo(b.LineNbr.GetValueOrDefault());
            });
            return list;
        }
    }
}