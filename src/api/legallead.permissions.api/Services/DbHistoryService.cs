using AutoMapper;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Models;

namespace legallead.permissions.api.Services
{
    public class DbHistoryService(IDbHistoryRepository db) : IDbHistoryService
    {
        private readonly IDbHistoryRepository _db = db;
        private readonly static IMapper _mapper = ModelMapper.Mapper;

        public async Task<DataRequestResponse?> BeginAsync(BeginDataRequest model)
        {
            var src = _mapper.Map<DbHistoryRequest>(model);
            var response = await _db.BeginAsync(src);
            if (response == null) return null;
            return _mapper.Map<DataRequestResponse>(response);
        }

        public async Task<DataRequestResponse?> CompleteAsync(CompleteDataRequest model)
        {
            var src = _mapper.Map<DbHistoryRequest>(model);
            var response = await _db.CompleteAsync(src);
            if (response == null) return null;
            return _mapper.Map<DataRequestResponse>(response);
        }

        public async Task<IEnumerable<FindRequestResponse>?> FindAsync(FindDataRequest model)
        {
            var response = await _db.FindAsync(model.Id);
            if (response == null) return null;
            var list = new List<FindRequestResponse>();
            response.ForEach(r => { list.Add(_mapper.Map<FindRequestResponse>(r)); });
            return list;
        }

        public async Task<bool> UploadAsync(UploadDataRequest model)
        {
            var src = new DbUploadRequest { Id = model.Id };
            model.Contents.ForEach(m =>
            {
                src.Contents.Add(_mapper.Map<DbSearchHistoryResultBo>(m));
            });
            var response = await _db.UploadAsync(src);
            return response;
        }
    }
}
