﻿using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Model;
using legallead.records.search.Classes;
using legallead.records.search.Models;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace legallead.search.api.Services
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage",
        "VSTHRD110:Observe result of async calls", Justification = "This is a fire and forget call. No observer needed.")]
    internal class SearchGenerationService : BaseTimedSvc<SearchGenerationService>
    {
        private readonly IExcelGenerator generator;
        public SearchGenerationService(
            ILoggingRepository? logger,
            ISearchQueueRepository? repo,
            IBgComponentRepository? component,
            IBackgroundServiceSettings? settings,
            IExcelGenerator excel) : base(logger, repo, component, settings)
        {
            generator = excel;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD002:Avoid problematic synchronous waits",
            Justification = "<Pending>")]
        protected override void DoWork(object? state)
        {
            if (IsWorking) { return; }
            if (DataService == null || _queueDb == null) return;
            lock (_lock)
            {
                IsWorking = true;
                var queue = GetQueue().Result;
                if (!queue.Any()) { return; }
                var message = "Found ( {queue.Count} ) records to process.";
                DataService.Echo(message);
                queue.ForEach(Generate);
                IsWorking = false;
            }
        }

        private void Generate(SearchQueueDto dto)
        {
            if (DataService == null || _queueDb == null) return;
            var uniqueId = dto.Id;
            var payload = dto.Payload;
            _ = _queueDb.Start(dto).ConfigureAwait(false);
            PostStatus(uniqueId, 0, 0);
            var bo = TryConvert<UserSearchRequest>(payload);
            if (bo == null)
            {
                PostStatus(uniqueId, 0, 2);
                _ = _queueDb.Complete(uniqueId);
                return;
            }
            PostStatus(uniqueId, 0, 1);
            PostStatus(uniqueId, 1, 0);
            var interaction = WebMapper.MapFrom<UserSearchRequest, WebInteractive>(bo);
            if (interaction == null)
            {
                PostStatus(uniqueId, 1, 2);
                _ = _queueDb.Complete(uniqueId);
                return;
            }
            PostStatus(uniqueId, 1, 1);
            PostStatus(uniqueId, 2, 0);
            var response = Fetch(interaction);
            if (response == null)
            {
                PostStatus(uniqueId, 2, 2);
                _ = _queueDb.Complete(uniqueId);
                return;
            }
            PostStatus(uniqueId, 2, 1);
            PostStatus(uniqueId, 3, 0);
            var addresses = GetAddresses(response);
            if (addresses == null)
            {
                PostStatus(uniqueId, 3, 2);
                _ = _queueDb.Complete(uniqueId);
                return;
            }
            PostStatus(uniqueId, 3, 1);
            PostStatus(uniqueId, 4, 0);
            var serialized = SerializeResult(uniqueId, addresses, _queueDb);
            if (!serialized)
            {
                PostStatus(uniqueId, 4, 2);
                _ = _queueDb.Complete(uniqueId);
                return;
            }
            PostStatus(uniqueId, 4, 1);
            _ = _queueDb.Complete(uniqueId);
        }

        private WebFetchResult? Fetch(WebInteractive web)
        {
            try
            {
                return web.Fetch();
            }
            catch (Exception ex)
            {
                _ = _logger?.LogError(ex).ConfigureAwait(false);
                return null;
            }
        }

        private ExcelPackage? GetAddresses(WebFetchResult fetchResult)
        {
            if (_logger == null) return null;
            return generator.GetAddresses(fetchResult, _logger);
        }

        private bool SerializeResult(string uniqueId, ExcelPackage package, ISearchQueueRepository repo)
        {
            if (_logger == null) return false;
            return generator.SerializeResult(uniqueId, package, repo, _logger);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods",
            Justification = "<Pending>")]
        private async Task<List<SearchQueueDto>> GetQueue()
        {
            if (_queueDb == null) return new();
            var items = await _queueDb.GetQueue();
            return items;
        }

        private static T? TryConvert<T>(string? data)
        {
            try
            {
                if (string.IsNullOrEmpty(data)) return default;
                return JsonConvert.DeserializeObject<T>(data);
            }
            catch (Exception)
            {
                return default;
            }
        }

        private void PostStatus(string uniqueId, int messageId, int statusId)
        {
            if (_queueDb == null) return;
            var indexes = new[] { 0, 1, 2 };
            var statuses = new[] { "begin", "complete", "failed" };
            var messages = new[]
            {
                $"{_typeName} parameter evaluation: {0}",
                $"{_typeName} parameter conversion to search request: {0}",
                $"{_typeName} search request processing: {0}",
                $"{_typeName} excel content conversion: {0}",
                $"{_typeName} excel content serialization: {0}",
            };
            if (messageId < 0 || messageId > messages.Length - 1) { return; }
            var state = indexes.Contains(statusId) ? statuses[statusId] : "status";
            var message = string.Format(messages[messageId], state);
            _ = _queueDb.Status(uniqueId, message).ConfigureAwait(false);
        }
    }
}