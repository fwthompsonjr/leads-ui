﻿using legallead.permissions.api.Model;
using legallead.records.search.Classes;
using legallead.records.search.tests.Mapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.records.search.tests
{
    [TestClass]
    public class CollinInteractiveTest
    {

        private const string collinjs = "{'state':'tx','county':{'name':'collin','value':26550},'details':[{'name':'Search Type','text':'Civil And Family Case Records','value':'3'}],'start':1719187200000,'end':1719273600000}";


        [TestMethod]
        public void InteractiveCanFetchCollin()
        {
            if (!System.Diagnostics.Debugger.IsAttached)
                return;

            var interactive = GetInteractive();
            var response = interactive?.Fetch();
            Assert.IsNotNull(response);
        }


        private static WebInteractive? GetInteractive()
        {
            DateTime? dt1 = new DateTime(2023, 10, 3, 0, 0, 0, DateTimeKind.Utc);
            DateTime? dt2 = dt1.Value.AddDays(0);
            var source = GetRequest();
            source.StartDate = ToUnixTime(dt1);
            source.EndDate = ToUnixTime(dt2);
            var mapped = TheMapper.MapFrom<UserSearchRequest, WebInteractive>(source);
            if (mapped == null) return null;
            if (string.IsNullOrEmpty(mapped.UniqueId))
            {
                mapped.UniqueId = Guid.NewGuid().ToString("D");
            }
            return mapped;
        }
        private static UserSearchRequest GetRequest()
        {
            return JsonConvert.DeserializeObject<UserSearchRequest>(collinjs) ?? new();
        }

        private static long ToUnixTime(DateTime? startDate, DateTimeKind dateKind = DateTimeKind.Utc)
        {
            const string dateFormat = "U"; // universal date time
            var cultureEnglishUS = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            if (startDate.HasValue && dateKind == DateTimeKind.Local)
            {
                startDate = startDate.Value.ToUniversalTime();
            }
            if (!startDate.HasValue)
            {
                startDate = DateTime.UtcNow;
            }
            var utcNow = startDate.Value;
            var dateStart = utcNow.ToString(dateFormat, cultureEnglishUS);
            var unixEpoch = DateTime.UnixEpoch;
            DateTime start = DateTime.ParseExact(dateStart, dateFormat, cultureEnglishUS);
            var milliseconds = DateTime.Now.Millisecond.ToString("000");
            var unixStart = ((long)start
                .Subtract(unixEpoch)
                .TotalMilliseconds).ToString();
            unixStart = unixStart[..^3];
            unixStart += milliseconds;
            return Convert.ToInt64(unixStart);
        }
    }
}