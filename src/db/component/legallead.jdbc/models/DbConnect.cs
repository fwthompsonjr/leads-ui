﻿using legallead.jdbc.helpers;
using Newtonsoft.Json;
using System.Text;

namespace legallead.jdbc.models
{
    internal class DbConnect
    {
        public string Target { get; set; } = "Local";
        public DbConnectDetail? Endpoints { get; set; }
        public DbConnectDetail? Passcodes { get; set; }
        public DbConnectDetail? Secrets { get; set; }
        public DbConnectDetail? Keys { get; set; }

        public string ConnectionString(string databaseName = "app")
        {
            var accepted = new[] { "Local", "Test", "Production" };
            if (!accepted.Contains(Target, StringComparer.OrdinalIgnoreCase)) return string.Empty;
            if (Endpoints == null || string.IsNullOrEmpty(Endpoints[Target])) return string.Empty;
            if (Passcodes == null || string.IsNullOrEmpty(Passcodes[Target])) return string.Empty;
            if (Secrets == null || string.IsNullOrEmpty(Secrets[Target])) return string.Empty;
            if (Keys == null || string.IsNullOrEmpty(Keys[Target])) return string.Empty;
            var connect = new StringBuilder(ConnectCommandString);
            var passcode = GetPassCode(this);
            connect.Replace("<localhost>", $"{Endpoints[Target] ?? string.Empty}");
            connect.Replace("<database>", DatabaseName(databaseName));
            connect.Replace("<username>", passcode[0]);
            connect.Replace("<password>", passcode[1]);
            return connect.ToString();
        }

        private static string DatabaseName(string databaseName = "app")
        {
            var name = DbList.Find(x => x.Name == databaseName);
            if (name != null) return name.Source;
            return DbList[0].Source;
        }

        private static string[] GetPassCode(DbConnect connect)
        {
            string saltLocal = connect.Passcodes?[connect.Target] ?? string.Empty;
            string conversion = connect.Secrets?[connect.Target] ?? string.Empty;
            string vector = connect.Keys?[connect.Target] ?? string.Empty;
            return CryptoManager.Decrypt(conversion, saltLocal, vector).Split('|');
        }
        private static List<DbName> DbList => _names ??= GetNames();
        private const string ConnectCommandString = "server=<localhost>;user=<username>;password=<password>;Database=<database>;port=3306;";
        private static readonly string DbNames = DatabaseSourceHelper.GetConfiguration();
        private static List<DbName>? _names;
        private static List<DbName> GetNames()
        {
            return JsonConvert.DeserializeObject<List<DbName>>(DbNames) ?? new();
        }

    }
}