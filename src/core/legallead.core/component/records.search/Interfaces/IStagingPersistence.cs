using legallead.records.search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.records.search.Interfaces
{
    public interface IStagingPersistence
    {
        KeyValuePair<bool, string> Add(string searchid, string key, string value);
        KeyValuePair<bool, string> Add(string searchid, string key, byte[] value);
        Task<KeyValuePair<bool, string>> AddAsync(string searchid, string key, string value);
        Task<KeyValuePair<bool, string>> AddAsync(string searchid, string key, byte[] value);

        KeyValuePair<bool, string> Update(string searchid, string key, string value);
        KeyValuePair<bool, string> Update(string searchid, string key, byte[] value);
        Task<KeyValuePair<bool, string>> UpdateAsync(string searchid, string key, string value);
        Task<KeyValuePair<bool, string>> UpdateAsync(string searchid, string key, byte[] value);

        KeyValuePair<bool, StagedContentModel> Fetch(string searchid, string key);
        Task<KeyValuePair<bool, StagedContentModel>> FetchAsync(string searchid, string key);
    }
}
