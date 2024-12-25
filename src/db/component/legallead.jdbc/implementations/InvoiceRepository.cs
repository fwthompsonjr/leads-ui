using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace legallead.jdbc.implementations
{
    public class InvoiceRepository(DataContext context) :
        BaseRepository<DbCountyUsageRequestDto>(context), IInvoiceRepository
    {

        public async Task<List<LeadCustomerBo>?> CreateAccountAsync(LeadCustomerBo query)
        {
            try
            {
                var prc = ProcNames.CREATE_ACCOUNT;
                var q = new[] { query };
                var json = JsonConvert.SerializeObject(q);
                var parms = new DynamicParameters();
                parms.Add(ProcParameterNames.Js, json);
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<LeadCustomerDto>(connection, prc, parms);
                if (response == null) return default;
                var list = new List<LeadCustomerBo>();
                response.ToList().ForEach(a =>
                    list.Add(MapFrom(a)));
                return list;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<List<DbInvoiceViewBo>?> FindAllAsync()
        {
            try
            {
                var prc = ProcNames.FIND_ALL;
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<DbInvoiceViewDto>(connection, prc);
                if (response == null) return default;
                var list = new List<DbInvoiceViewBo>();
                response.ToList().ForEach(a =>
                    list.Add(MapFrom(a)));
                ApplySort(list);
                return list;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<List<LeadCustomerBo>?> FindAccountAsync(LeadCustomerBo query)
        {
            try
            {
                var prc = ProcNames.FIND_ACCOUNT;
                var q = new[] { query };
                var json = JsonConvert.SerializeObject(q);
                var parms = new DynamicParameters();
                parms.Add(ProcParameterNames.Js, json);
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<LeadCustomerDto>(connection, prc, parms);
                if (response == null) return default;
                var list = new List<LeadCustomerBo>();
                response.ToList().ForEach(a =>
                    list.Add(MapFrom(a)));
                return list;
            }
            catch (Exception)
            {
                return default;
            }

        }

        public async Task<GenerateInvoiceBo?> GenerateInvoicesAsync()
        {
            try
            {
                var prc = ProcNames.GENERATE_INVOICES;
                using var connection = _context.CreateConnection();
                var response = await _command.QuerySingleOrDefaultAsync<GenerateInvoiceDto>(connection, prc);
                if (response == null) return default;
                return GenericMap<GenerateInvoiceDto, GenerateInvoiceBo>(response);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<List<DbInvoiceViewBo>?> QueryAsync(DbInvoiceViewBo query)
        {
            try
            {
                var prc = ProcNames.QUERY_ALL;
                var q = new[] { query };
                var json = JsonConvert.SerializeObject(q);
                var parms = new DynamicParameters();
                parms.Add(ProcParameterNames.Js, json);
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<DbInvoiceViewDto>(connection, prc, parms);
                if (response == null) return default;
                var list = new List<DbInvoiceViewBo>();
                response.ToList().ForEach(a =>
                    list.Add(MapFrom(a)));
                ApplySort(list);
                return list;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<KeyValuePair<bool, string>> UpdateAsync(DbInvoiceViewBo query)
        {
            try
            {
                if (string.IsNullOrEmpty(query.Id)) return new(false, "Invoice Id is required");
                var prc = ProcNames.UPDATE_INVOICE;
                var q = new[] { query };
                var json = JsonConvert.SerializeObject(q);
                var parms = new DynamicParameters();
                parms.Add(ProcParameterNames.Js, json);
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc, parms);
                return new(true, string.Empty);
            }
            catch (Exception ex)
            {
                return new(false, ex.Message);
            }
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public accessor")]
        private static T GenericMap<S, T>(S source) where T : class, new()
        {
            try
            {
                var src = JsonConvert.SerializeObject(source);
                return JsonConvert.DeserializeObject<T>(src) ?? new();
            }
            catch (Exception)
            {
                return new();
            }
        }


        private static DbInvoiceViewBo MapFrom(DbInvoiceViewDto source)
        {
            return new()
            {
                Id = source.Id,
                UserName = source.UserName,
                Email = source.Email,
                LeadUserId = source.LeadUserId,
                RequestId = source.RequestId,
                InvoiceNbr = source.InvoiceNbr,
                InvoiceUri = source.InvoiceUri,
                RecordCount = source.RecordCount,
                LineNbr = source.LineNbr,
                Description = source.Description,
                ItemCount = source.ItemCount,
                ItemPrice = source.ItemPrice,
                ItemTotal = source.ItemTotal,
                InvoiceTotal = source.InvoiceTotal,
                CompleteDate = source.CompleteDate,
                CreateDate = source.CreateDate,
            };
        }


        private static LeadCustomerBo MapFrom(LeadCustomerDto source)
        {
            return new()
            {
                Id = source.Id,
                LeadUserId = source.LeadUserId,
                CustomerId = source.CustomerId,
                Email = source.Email,
                IsTest = source.IsTest,
                CreateDate = source.CreateDate,
            };
        }
        private static void ApplySort(List<DbInvoiceViewBo> list)
        {
            list.Sort((a, b) =>
            {
                var aa = (a.LeadUserId ?? string.Empty).CompareTo(b.LeadUserId ?? string.Empty);
                if (aa != 0) return aa;
                aa = a.CreateDate.GetValueOrDefault().CompareTo(b.CreateDate.GetValueOrDefault());
                if (aa != 0) return aa;
                aa = (a.RequestId ?? string.Empty).CompareTo(b.RequestId ?? string.Empty);
                if (aa != 0) return aa;
                return a.LineNbr.GetValueOrDefault().CompareTo(b.LineNbr.GetValueOrDefault());

            });
        }
        private static class ProcNames
        {
            public const string CREATE_ACCOUNT = "CALL USP_LEADUSER_CREATE_CUSTOMER_ACCOUNT ( ? );";
            public const string GENERATE_INVOICES = "CALL USP_LEADUSER_GENERATE_INVOICE ( );";
            public const string FIND_ACCOUNT = "CALL USP_LEADUSER_FIND_CUSTOMER_ACCOUNT ( ? );";
            public const string FIND_ALL = "CALL USP_LEADUSER_LIST_INVOICES ( );";
            public const string QUERY_ALL = "CALL USP_LEADUSER_QUERY_INVOICES ( ? );";
            public const string UPDATE_INVOICE = "CALL USP_LEADUSER_UPDATE_INVOICE ( ? );";
        }

        private static class ProcParameterNames
        {
            public const string Js = "js_parameter";
        }
    }
}