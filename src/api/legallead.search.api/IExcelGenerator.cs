﻿using legallead.jdbc.interfaces;
using legallead.records.search.Models;
using OfficeOpenXml;

namespace legallead.search.api
{
    internal interface IExcelGenerator
    {
        ExcelPackage? GetAddresses(WebFetchResult fetchResult, ILoggingRepository logging);
        bool SerializeResult(string uniqueId, ExcelPackage package, ISearchQueueRepository repo, ILoggingRepository logging);
    }
}
