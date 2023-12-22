using legallead.logging.entities;
using legallead.logging.extensions;
using legallead.logging.interfaces;

namespace legallead.logging.implementations
{
    internal class LogContentRepository : BaseLoggingDbRepository<LogContentDetailDto>, ILogContentRepository
    {
        public LogContentRepository(ILoggingDbContext context) : base(context)
        {
        }

        public async Task InsertChild(LogContentDetailDto dto)
        {
            await Create(dto);
        }

        public async Task Insert(LogInsertModel dto)
        {
            var parmlist = LoggingService.GetInsertParameters(dto);
            var statement = $"CALL USP_INSERT_LOG_CONTENT( {parmlist} );";
            using var connection = GetContext.CreateConnection();
            var insertId = await GetCommand.QuerySingleOrDefaultAsync<InsertIndexDto>(connection, statement);
            if (string.IsNullOrEmpty(dto.Detail) || insertId == null) return;
            var logContentId = insertId.Id;
            var lines = dto.Detail.SplitByLength(500);
            lines.Keys.ToList().ForEach(async line =>
            {
                var text = lines[line].Truncate(500);
                var detail = new LogContentDetailDto
                {
                    LogContentId = logContentId,
                    LineId = line,
                    Line = text
                };
                await InsertChild(detail);
            });
        }

        public async Task<IEnumerable<VwLogDto>> Query(LogQueryModel query)
        {
            var parmlist = LoggingService.GetQueryParameters(query);
            var statement = $"CALL USP_SEARCH_LOG( {parmlist} );";
            using var connection = GetContext.CreateConnection();
            var results = await GetCommand.QueryAsync<VwLogDto>(connection, statement);
            return results;
        }
    }
}