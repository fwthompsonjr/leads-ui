using legallead.jdbc.interfaces;
using legallead.permissions.api.Models;
using Stripe;

namespace legallead.permissions.api.Services
{
    public class LeadInvoiceService(
        IInvoiceRepository repo,
        PaymentStripeOption payment) : ILeadInvoiceService
    {
        private readonly IInvoiceRepository db = repo;
        private readonly PaymentStripeOption stripeoption = payment;

        public async Task<UpdateInvoiceResponse?> CreateRemoteInvoiceAsync(GetInvoiceRequest request)
        {
            const string expected = "Invoice";
            if (!request.RequestType.Equals(expected, StringComparison.OrdinalIgnoreCase)) return null;
            if (string.IsNullOrWhiteSpace(request.InvoiceId)) return null;
            var details = await GetByInvoiceIdAsync(request.InvoiceId);
            if (details == null || details.Headers.Count == 0 || details.Lines.Count == 0) return null;
            var header = details.Headers[0];
            var requestId = header.Id;
            if (string.IsNullOrEmpty(requestId)) return null;
            if (completiontypes.Contains(header.InvoiceNbr, StringComparer.OrdinalIgnoreCase))
            {
                return new()
                {
                    IsUpdated = true,
                    Message = "Invoice previously created",
                    InvoiceId = request.InvoiceId,
                    InvoiceStatus = header.InvoiceNbr ?? string.Empty,
                    InvoiceUri = header.InvoiceUri ?? string.Empty,
                    UpdateType = expected
                };
            }
            var lines = details.Lines.FindAll(x => x.Id == requestId);
            if (lines.Count == 0) return null;
            var model = new CreateInvoiceAccountModel
            {
                LeadId = header.LeadUserId,
                EmailAcct = header.Email
            };
            var customer = await GetOrCreateAccountAsync(model);
            if (customer == null || customer.Count == 0) return null;
            var customerId = customer[0].CustomerId;

            if (string.IsNullOrEmpty(customerId)) return null;
            var invoice = CreateInvoice(customerId, header, lines);
            if (invoice == null) return null;
            if (invoice.Id == lessThanMinIndex)
            {
                var completion = await PostInvoiceDetailsAsync(requestId, invoice);
                if (completion == null) return null;
                var finalDto = new UpdateInvoiceRequest
                {
                    UpdateType = "Complete",
                    InvoiceId = requestId,
                    InvoiceStatus = "PAID",
                    InvoiceUri = invoice.InvoicePdf
                };
                return await UpdateInvoiceAsync(finalDto);
            }
            return await PostInvoiceDetailsAsync(requestId, invoice);
        }

        private async Task<UpdateInvoiceResponse?> PostInvoiceDetailsAsync(string? requestId, Invoice? invoice)
        {
            if (invoice == null) return null;
            if (string.IsNullOrEmpty(requestId)) return null;
            var updateDto = new UpdateInvoiceRequest
            {
                UpdateType = "Status",
                InvoiceId = requestId,
                InvoiceStatus = "SENT",
                InvoiceUri = invoice.InvoicePdf
            };

            var sent = await UpdateInvoiceAsync(updateDto);
            if (sent == null || !sent.IsUpdated) return null;
            updateDto.UpdateType = "Uri";
            var addressed = await UpdateInvoiceAsync(updateDto);
            if (addressed == null || !addressed.IsUpdated) return null;
            return addressed;
        }

        public async Task<bool> CloseInvoiceAsync(string invoiceId)
        {
            var data = await GetByInvoiceIdAsync(invoiceId);
            if (data == null || data.Headers.Count == 0) return false;
            var header = data.Headers.FirstOrDefault();
            if (header == null) return false;
            if (string.IsNullOrEmpty(header.Id)) return false;
            if (string.IsNullOrEmpty(header.InvoiceNbr)) return false;
            if (string.IsNullOrEmpty(header.InvoiceUri)) return false;
            if (header.InvoiceNbr == "PAID") return true;

            var payload = new UpdateInvoiceRequest
            {
                InvoiceId = header.Id,
                InvoiceStatus = header.InvoiceNbr,
                InvoiceUri = header.InvoiceUri,
                UpdateType = "Complete",
            };
            var updated = await UpdateInvoiceAsync(payload);
            return updated.IsUpdated;
        }

        public async Task<GetInvoiceResponse?> GetByCustomerIdAsync(string id)
        {
            var payload = new DbInvoiceViewBo { LeadUserId = id };
            return await MapResponseAsync(payload);
        }

        public async Task<GetInvoiceResponse?> GetByInvoiceIdAsync(string id)
        {
            var payload = new DbInvoiceViewBo { Id = id };
            return await MapResponseAsync(payload);
        }

        public async Task<List<LeadCustomerBo>?> GetOrCreateAccountAsync(CreateInvoiceAccountModel query)
        {
            if (string.IsNullOrEmpty(query.LeadId))
                throw new ArgumentOutOfRangeException(nameof(query));

            if (string.IsNullOrEmpty(query.EmailAcct))
                throw new ArgumentOutOfRangeException(nameof(query));
            query.IsTesting = IsTestPayment;

            var found = await db.FindAccountAsync(new LeadCustomerBo
            {
                Id = query.LeadId,
                Email = query.EmailAcct,
                IsTest = query.IsTesting
            });
            if (found != null && found.Count > 0 && found.Exists(x => !string.IsNullOrEmpty(x.CustomerId))) return found;
            var account = CreatePaymentAccount(query);
            if (string.IsNullOrEmpty(account)) return null;
            var payload = new LeadCustomerBo
            {
                Email = query.EmailAcct,
                IsTest = query.IsTesting,
                CustomerId = account,
                LeadUserId = query.LeadId,
            };
            var added = await db.CreateAccountAsync(payload);
            return added;
        }

        public async Task<UpdateInvoiceResponse> UpdateInvoiceAsync(UpdateInvoiceRequest request)
        {
            var response = new UpdateInvoiceResponse
            {
                InvoiceId = request.InvoiceId,
                InvoiceStatus = request.InvoiceStatus,
                InvoiceUri = request.InvoiceUri,
                UpdateType = request.UpdateType,
            };
            if (string.IsNullOrEmpty(request.InvoiceId))
            {
                response.Message = "Invoice Id is not provided";
                return response;
            }
            var payload = new DbInvoiceViewBo
            {
                Id = request.InvoiceId,
            };
            var index = updatetypes.FindIndex(x => x.Equals(request.UpdateType, StringComparison.OrdinalIgnoreCase));
            if (index == -1)
            {
                response.Message = $"Invoice Update {request.UpdateType} is not valid request type";
                return response;
            }
            if (index == 0) payload.InvoiceNbr = request.InvoiceStatus;
            if (index == 1) payload.InvoiceUri = request.InvoiceUri;
            if (index == 2)
            {
                payload.InvoiceNbr = "PAID";
                payload.InvoiceUri = request.InvoiceUri;
                payload.CompleteDate = DateTime.UtcNow;
            }
            var data = await db.UpdateAsync(payload);
            response.IsUpdated = data.Key;
            response.Message = data.Value;
            return response;
        }

        protected virtual string CreatePaymentAccount(CreateInvoiceAccountModel model)
        {
            if (string.IsNullOrWhiteSpace(model.EmailAcct)) return string.Empty;
            try
            {
                var service = new Stripe.CustomerService();
                var options = GenerateCreateOption(model.EmailAcct);
                var account = service.Create(options);
                return account?.Id ?? string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }


        protected static CustomerCreateOptions GenerateCreateOption(string email)
        {
            return new CustomerCreateOptions
            {
                Description = "Legal Lead Customer",
                Email = email
            };
        }


        private async Task<GetInvoiceResponse?> MapResponseAsync(DbInvoiceViewBo payload)
        {
            var response = new GetInvoiceResponse();
            _ = await db.GenerateInvoicesAsync();
            var data = await db.QueryAsync(payload);
            if (data == null || data.Count == 0) return response;
            var headings = MapFrom(data);
            var completed = headings.FindAll(x =>
                !completiontypes.Contains(x.InvoiceNbr, StringComparer.OrdinalIgnoreCase) &&
                x.InvoiceTotal < 0.50m);
            response.Headers.AddRange(headings);
            response.Lines.AddRange(MapDetail(data));
            if (completed.Count == 0)
            {
                return response;
            }
            var successes = 0;
            foreach (var item in completed)
            {
                if (string.IsNullOrEmpty(item.Id)) continue;
                var bo = new UpdateInvoiceRequest
                {
                    InvoiceId = item.Id,
                    InvoiceStatus = "PAID",
                    InvoiceUri = "http://www.completed.com",
                    UpdateType = "Complete"
                };
                var added = await UpdateInvoiceAsync(bo);
                if (added != null && added.IsUpdated) successes++;
            }
            if (successes == 0) return response;

            return await MapResponseAsync(payload);
        }

        private static List<InvoiceHeaderModel> MapFrom(List<DbInvoiceViewBo> response)
        {
            var list = new List<InvoiceHeaderModel>();
            response.ForEach(r =>
            {
                list.Add(new()
                {
                    CompleteDate = r.CompleteDate,
                    CreateDate = r.CreateDate,
                    Email = r.Email,
                    Id = r.Id,
                    InvoiceNbr = r.InvoiceNbr,
                    InvoiceTotal = r.InvoiceTotal,
                    InvoiceUri = r.InvoiceUri,
                    LeadUserId = r.LeadUserId,
                    RecordCount = r.RecordCount,
                    RequestId = r.RequestId,
                    UserName = r.UserName,
                });
            });
            return list.GroupBy(x => x.Id).Select(x => x.First()).ToList();
        }

        private static List<InvoiceDetailModel> MapDetail(List<DbInvoiceViewBo> response)
        {
            var list = new List<InvoiceDetailModel>();
            response.ForEach(r =>
            {
                list.Add(new()
                {
                    Id = r.Id,
                    Description = r.Description,
                    ItemCount = r.ItemCount,
                    ItemPrice = r.ItemPrice,
                    ItemTotal = r.ItemTotal,
                    LineNbr = r.LineNbr
                });
            });
            return list;
        }

        protected virtual Invoice? CreateInvoice(string customerId, InvoiceHeaderModel model, List<InvoiceDetailModel> lines)
        {
            try
            {
                // minimum amount check
                if (model.InvoiceTotal < 0.50m)
                {
                    var min = new Invoice
                    {
                        Id = lessThanMinIndex,
                        InvoicePdf = "http://api.legallead.co"
                    };
                    return min;
                }
                // Create an Invoice
                var invoiceOptions = new InvoiceCreateOptions
                {
                    AutoAdvance = true,
                    AutomaticallyFinalizesAt = DateTime.UtcNow.AddMinutes(2),
                    Customer = customerId,
                    CollectionMethod = "send_invoice",
                    DaysUntilDue = 30,
                    PaymentSettings = new(),
                    Currency = "usd",
                    AutomaticTax = new() { Enabled = false },
                    Description = $"Record Search {model.CreateDate:d}",
                    Metadata = new()
                    {
                        { "RecordIndex", model.Id },
                    },
                };
                invoiceOptions.PaymentSettings.PaymentMethodTypes = ["card", "customer_balance"];
                var service = new InvoiceService();
                var invoice = service.Create(invoiceOptions);
                var lineService = new InvoiceItemService();
                lines.ForEach(line =>
                {
                    var lineOptions = new InvoiceItemCreateOptions
                    {
                        Customer = customerId,
                        Invoice = invoice.Id,
                        Description = line.Description,
                        Quantity = Convert.ToInt64(line.ItemCount.GetValueOrDefault()),
                        Currency = "usd",
                        UnitAmount = Convert.ToInt64(line.ItemPrice.GetValueOrDefault() * 100),
                    };
                    if (line.LineNbr.GetValueOrDefault() != 1)
                    {
                        lineOptions.Quantity = 1;
                        lineOptions.UnitAmount = Convert.ToInt64(line.ItemTotal.GetValueOrDefault() * 100);
                    }
                    lineService.Create(lineOptions);
                });
                
                service.FinalizeInvoice(invoice.Id);
                return invoice;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static readonly List<string> updatetypes = [.. "Status,Uri,Complete".Split(',')];
        private static readonly List<string> completiontypes = [.. "SENT,PAID".Split(',')];
        private const string lessThanMinIndex = "less_than_min_billable_amount";
        private bool IsTestPayment
        {
            get
            {
                if (isTesting.HasValue) return isTesting.Value;
                var option = stripeoption.Key;
                if (string.IsNullOrEmpty(option)) option = "test";
                isTesting = option.Contains("test", StringComparison.OrdinalIgnoreCase);
                return isTesting.Value;
            }
        }


        private bool? isTesting;
    }
}
