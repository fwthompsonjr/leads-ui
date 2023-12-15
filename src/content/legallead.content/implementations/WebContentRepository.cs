using legallead.content.entities;
using legallead.content.extensions;
using legallead.content.helpers;
using legallead.content.interfaces;

namespace legallead.content.implementations
{
    public class WebContentRepository : BaseContentDbRepository<WebContentDto>, IWebContentRepository
    {
        public WebContentRepository(ContentDbContext context) : base(context)
        {
        }

        public async Task CreateRevision(WebContentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Id)) return;
            var statement = $"CALL USP_CREATE_CONTENT_REVISION( '{dto.Id}' );";
            using var connection = _context.CreateConnection();
            await _command.ExecuteAsync(connection, statement);
        }

        public async Task<IEnumerable<WebContentDto>> GetAllActive()
        {
            using var connection = _context.CreateConnection();
            var parm = new WebContentDto { IsActive = true };
            var sql = _sut.SelectSQL(parm);
            var parms = _sut.SelectParameters(parm);
            return await _command.QueryAsync<WebContentDto>(connection, sql, parms);
        }

        public async Task<IEnumerable<WebContentDto>> GetByInternalId(int internalId)
        {
            using var connection = _context.CreateConnection();
            var parm = new WebContentDto { InternalId = internalId };
            var sql = _sut.SelectSQL(parm);
            var parms = _sut.SelectParameters(parm);
            return await _command.QueryAsync<WebContentDto>(connection, sql, parms);
        }

        public async Task<IEnumerable<WebContentDto>> GetByName(string name)
        {
            using var connection = _context.CreateConnection();
            var parm = new WebContentDto { ContentName = name };
            var sql = _sut.SelectSQL(parm);
            var parms = _sut.SelectParameters(parm);
            return await _command.QueryAsync<WebContentDto>(connection, sql, parms);
        }

        public async Task Insert(CreateContentRequest dto)
        {
            _ = dto.GetValidationResult(out var isValid);
            if (string.IsNullOrWhiteSpace(dto.Name) || !isValid)
                throw new ArgumentOutOfRangeException(nameof(dto));

            var statement = $"CALL USP_INSERT_CONTENT( '{dto.Name}' );";
            using var connection = _context.CreateConnection();
            await _command.ExecuteAsync(connection, statement);
        }

        public async Task SetActiveRevision(WebContentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Id) || !dto.VersionId.HasValue)
                throw new ArgumentOutOfRangeException(nameof(dto));
            var statement = $"CALL USP_SET_ACTIVE_REVISION( '{dto.Id}', {dto.VersionId.GetValueOrDefault()} );";
            using var connection = _context.CreateConnection();
            await _command.ExecuteAsync(connection, statement);
        }
    }
}