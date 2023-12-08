using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using Npgsql;
using System.Data;

namespace legallead.jdbc.helpers
{
    public class DataInitializer : IDataInitializer
    {
        private bool IsDbInitialized = false;
        private readonly string _connectionString;

        public DataInitializer()
        {
            _connectionString = RemoteData.GetPostGreString();
        }

        public virtual IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public async Task Init()
        {
            if (IsDbInitialized) return;
            await InitTables();
            await InitViews();
            await InitStoredProcedures();
            await InitApplications();
            await InitProfile();
            await InitPermissions();
            await InitPermissionGroups();
            await InitReasonCodes();
            IsDbInitialized = true;
        }

        private async Task InitTables()
        {
            // create tables if they don't exist
            using var connection = CreateConnection();
            var sql = "CREATE TABLE IF NOT EXISTS APPLICATIONS ( "
                + Environment.NewLine
                + "\tId CHAR(36) PRIMARY KEY, "
                + Environment.NewLine
                + "\tName VARCHAR(150) "
                + Environment.NewLine
                + ");"
                + Environment.NewLine
                + ""
                + Environment.NewLine
                + "CREATE TABLE IF NOT EXISTS USERS ("
                + Environment.NewLine
                + "\tId CHAR(36) PRIMARY KEY,"
                + Environment.NewLine
                + "\tUserName VARCHAR(50),"
                + Environment.NewLine
                + "\tEmail VARCHAR(255),"
                + Environment.NewLine
                + "\tPasswordHash VARCHAR(150),"
                + Environment.NewLine
                + "\tPasswordSalt VARCHAR(150)"
                + Environment.NewLine
                + ");"
                + Environment.NewLine
                + ""
                + Environment.NewLine
                + "CREATE TABLE IF NOT EXISTS PROFILEMAP ("
                + Environment.NewLine
                + "\tId CHAR(36) PRIMARY KEY,"
                + Environment.NewLine
                + "\tOrderId INT,"
                + Environment.NewLine
                + "\tKeyName VARCHAR(100)"
                + Environment.NewLine
                + ");"
                + Environment.NewLine
                + ""
                + Environment.NewLine
                + "CREATE TABLE IF NOT EXISTS PERMISSIONMAP ("
                + Environment.NewLine
                + "\tId CHAR(36) PRIMARY KEY,"
                + Environment.NewLine
                + "\tOrderId INT,"
                + Environment.NewLine
                + "\tKeyName VARCHAR(100)"
                + Environment.NewLine
                + ");"
                + Environment.NewLine
                + ""
                + Environment.NewLine
                + "CREATE TABLE IF NOT EXISTS USERPROFILE ("
                + Environment.NewLine
                + "\tId CHAR(36) PRIMARY KEY,"
                + Environment.NewLine
                + "\tUserId CHAR(36) references USERS(Id) ,"
                + Environment.NewLine
                + "\tProfileMapId CHAR(36) references PROFILEMAP(Id) ,"
                + Environment.NewLine
                + "\tKeyValue VARCHAR(256)"
                + Environment.NewLine
                + ");"
                + Environment.NewLine
                + ""
                + Environment.NewLine
                + "CREATE TABLE IF NOT EXISTS USERPERMISSION ("
                + Environment.NewLine
                + "\tId CHAR(36) PRIMARY KEY,"
                + Environment.NewLine
                + "\tUserId CHAR(36) references USERS(Id),"
                + Environment.NewLine
                + "\tPermissionMapId CHAR(36) references PERMISSIONMAP(Id) ,"
                + Environment.NewLine
                + "\tKeyValue VARCHAR(256)"
                + Environment.NewLine
                + ");"
                + Environment.NewLine
                + "DROP TABLE IF EXISTS USERTOKENS;"
                + Environment.NewLine
                + "CREATE TABLE IF NOT EXISTS USERTOKENS ("
                + Environment.NewLine
                + "\tId CHAR(36) PRIMARY KEY,"
                + Environment.NewLine
                + "\tUserId CHAR(36) references USERS(Id),"
                + Environment.NewLine
                + "\tRefreshToken VARCHAR(256) NOT NULL default(''),"
                + Environment.NewLine
                + "\tIsActive BOOLEAN NOT NULL default( TRUE ),"
                + Environment.NewLine
                + "\tCreateDate timestamp with time zone NOT NULL default ( now() )"
                + Environment.NewLine
                + ");"
                + Environment.NewLine
                + "DROP TABLE IF EXISTS PERMISSIONGROUP;"
                + Environment.NewLine
                + "CREATE TABLE IF NOT EXISTS PERMISSIONGROUP ( "
                + Environment.NewLine
                + "\t Id CHAR(36) PRIMARY KEY, "
                + Environment.NewLine
                + "\t Name VARCHAR(50), "
                + Environment.NewLine
                + "\t GroupId INT, "
                + Environment.NewLine
                + "\t OrderId INT, "
                + Environment.NewLine
                + "\t PerRequest INT, "
                + "\t PerMonth INT, "
                + Environment.NewLine
                + "\t PerYear INT, "
                + Environment.NewLine
                + "\t IsActive BOOLEAN NOT NULL default( TRUE ), "
                + Environment.NewLine
                + "\t IsVisible BOOLEAN NOT NULL default( TRUE ), "
                + Environment.NewLine
                + "\t CreateDate timestamp with time zone NOT NULL default ( now() ) "
                + Environment.NewLine
                + " ); "
                + Environment.NewLine
                + "CREATE TABLE IF NOT EXISTS USERPERMISSIONHISTORY"
                + Environment.NewLine
                + "("
                + Environment.NewLine
                + "\t Id CHAR(36) PRIMARY KEY NOT NULL "
                + Environment.NewLine
                + "\t DEFAULT ( md5(random()::text || clock_timestamp()::text)::uuid::CHAR(36) ), "
                + Environment.NewLine
                + "\t UserPermissionId CHAR(36) references USERPERMISSION(Id), "
                + Environment.NewLine
                + "\t GroupId INT NULL, "
                + Environment.NewLine
                + "\t UserId CHAR(36) references USERS(Id) , "
                + Environment.NewLine
                + "\t PermissionMapId CHAR(36) references PERMISSIONMAP(Id) , "
                + Environment.NewLine
                + "\t KeyName VARCHAR(100) , "
                + Environment.NewLine
                + "\t KeyValue VARCHAR(256) , "
                + Environment.NewLine
                + "\t CreateDate timestamp with time zone NOT NULL default ( now() ) "
                + Environment.NewLine
                + "); "
                + Environment.NewLine
                + "CREATE TABLE IF NOT EXISTS REASONCODES "
                + Environment.NewLine
                + "("
                + Environment.NewLine
                + "\t ReasonCode char(4) primary key,"
                + Environment.NewLine
                + "\t Reason varchar(125)"
                + Environment.NewLine
                + "); ";
            var stmts = sql.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var stmt in stmts)
            {
                if (!string.IsNullOrEmpty(stmt))
                {
                    var command = $"{stmt};";
                    await connection.ExecuteAsync(command);
                }
            }
        }

        private async Task InitViews()
        {
            // create views if they don't exist
            using var connection = CreateConnection();
            var sql = "CREATE OR REPLACE VIEW VWUSERPROFILE "
                + Environment.NewLine
                + "AS "
                + Environment.NewLine
                + "\t SELECT u.id, "
                + Environment.NewLine
                + "\t u.userid,"
                + Environment.NewLine
                + "\t u.profilemapid, "
                + Environment.NewLine
                + "\t u.keyvalue, "
                + Environment.NewLine
                + "\t p.keyname, "
                + Environment.NewLine
                + "\t p.orderid "
                + Environment.NewLine
                + "\t FROM USERPROFILE u "
                + Environment.NewLine
                + "\t JOIN PROFILEMAP p "
                + Environment.NewLine
                + "\t ON u.profilemapid = p.id; "
                + Environment.NewLine
                + "CREATE OR REPLACE VIEW VWUSERPERMISSION "
                + Environment.NewLine
                + "AS "
                + Environment.NewLine
                + "\t SELECT u.id, "
                + Environment.NewLine
                + "\t u.userid,"
                + Environment.NewLine
                + "\t u.permissionmapid, "
                + Environment.NewLine
                + "\t u.keyvalue, "
                + Environment.NewLine
                + "\t p.keyname, "
                + Environment.NewLine
                + "\t p.orderid "
                + Environment.NewLine
                + "\t FROM userpermission u "
                + Environment.NewLine
                + "\t JOIN permissionmap p "
                + Environment.NewLine
                + "\t ON u.permissionmapid = p.id; "
                + Environment.NewLine;
            var stmts = sql.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var stmt in stmts)
            {
                if (!string.IsNullOrEmpty(stmt))
                {
                    var command = $"{stmt};";
                    await connection.ExecuteAsync(command);
                }
            }
        }

        private async Task InitApplications()
        {
            var applicationNames = "legallead.permissions.api".Split(',');
            var command = "INSERT INTO APPLICATIONS " + Environment.NewLine +
            "( Id, Name ) " + Environment.NewLine +
            "SELECT b.Id, b.Name " + Environment.NewLine +
            "FROM APPLICATIONS as a " + Environment.NewLine +
            "RIGHT JOIN " + Environment.NewLine +
            "( " + Environment.NewLine +
            "SELECT  " + Environment.NewLine +
            "\t'{0}' Id, " + Environment.NewLine +
            "\t'{1}' Name " + Environment.NewLine +
            ") as b " + Environment.NewLine +
            "ON a.Name = b.Name " + Environment.NewLine +
            "WHERE a.Name IS NULL;";

            using var connection = CreateConnection();
            foreach (var application in applicationNames)
            {
                var indx = Guid.NewGuid().ToString("D").ToLower();
                var stmt = string.Format(command, indx, application);
                await connection.ExecuteAsync(stmt);
            }
        }

        private async Task InitProfile()
        {
            var keynames = new List<string> {
            "First Name",
            "Last Name",
            "Company Name",
            "Phone 1",
            "Phone 2",
            "Phone 3",
            "Email 1",
            "Email 2",
            "Email 3",
            "Address 1 - Address Line 1",
            "Address 1 - Address Line 2",
            "Address 1 - Address Line 3",
            "Address 1 - Address Line 4",
            "Address 2 - Address Line 1",
            "Address 2 - Address Line 2",
            "Address 2 - Address Line 3",
            "Address 2 - Address Line 4",
        };
            var command = "INSERT INTO PROFILEMAP " + Environment.NewLine +
            "( Id, OrderId, KeyName ) " + Environment.NewLine +
            "SELECT b.Id, b.OrderId, b.KeyName " + Environment.NewLine +
            "FROM PROFILEMAP as a " + Environment.NewLine +
            "RIGHT JOIN " + Environment.NewLine +
            "( " + Environment.NewLine +
            "SELECT  " + Environment.NewLine +
            "\t'{0}' Id, " + Environment.NewLine +
            "\t {1}  OrderId, " + Environment.NewLine +
            "\t'{2}' KeyName " + Environment.NewLine +
            ") as b " + Environment.NewLine +
            "ON a.KeyName = b.KeyName " + Environment.NewLine +
            "WHERE a.KeyName IS NULL;";

            using var connection = CreateConnection();
            foreach (var key in keynames)
            {
                var indx = Guid.NewGuid().ToString("D").ToLower();
                var stmt = string.Format(command, indx, keynames.IndexOf(key), key);
                await connection.ExecuteAsync(stmt);
            }
        }

        private async Task InitPermissions()
        {
            var keynames = new List<string> {
                "Account.IsPrimary",
                "Account.Permission.Level",
                "Account.Linked.Accounts",
                "Setting.MaxRecords.Per.Year",
                "Setting.MaxRecords.Per.Month",
                "Setting.MaxRecords.Per.Request",
                "Setting.Pricing.Name",
                "Setting.Pricing.Per.Year",
                "Setting.Pricing.Per.Month",
                "Setting.Pricing.Per.Request",
                "Setting.State.Subscriptions",
                "Setting.State.County.Subscriptions",
                "Setting.State.Subscriptions.Active",
                "Setting.State.County.Subscriptions.Active",
                "User.State.Discount",
                "User.State.County.Discount",
            };
            var command = "INSERT INTO PERMISSIONMAP " + Environment.NewLine +
            "( Id, OrderId, KeyName ) " + Environment.NewLine +
            "SELECT b.Id, b.OrderId, b.KeyName " + Environment.NewLine +
            "FROM PERMISSIONMAP as a " + Environment.NewLine +
            "RIGHT JOIN " + Environment.NewLine +
            "( " + Environment.NewLine +
            "SELECT  " + Environment.NewLine +
            "\t'{0}' Id, " + Environment.NewLine +
            "\t {1}  OrderId, " + Environment.NewLine +
            "\t'{2}' KeyName " + Environment.NewLine +
            ") as b " + Environment.NewLine +
            "ON a.KeyName = b.KeyName " + Environment.NewLine +
            "WHERE a.KeyName IS NULL;";

            using var connection = CreateConnection();
            foreach (var key in keynames)
            {
                var indx = Guid.NewGuid().ToString("D").ToLower();
                var stmt = string.Format(command, indx, keynames.IndexOf(key), key);
                await connection.ExecuteAsync(stmt);
            }
        }

        private async Task InitPermissionGroups()
        {
            try
            {
                using var connection = CreateConnection();
                foreach (var grp in permissionGroups)
                {
                    var finder = new PermissionGroup
                    {
                        GroupId = grp.GroupId,
                        OrderId = grp.OrderId,
                        IsActive = null,
                        IsVisible = null,
                        CreateDate = null
                    };
                    var selectCommand = grp.SelectSQL(finder);
                    var parms = grp.SelectParameters(finder);
                    var existing = await connection.QuerySingleOrDefaultAsync(selectCommand, parms);
                    if (existing == null)
                    {
                        grp.Id = Guid.NewGuid().ToString("D");
                        grp.CreateDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                        var command = grp.InsertSQL();
                        parms = grp.InsertParameters();
                        await connection.ExecuteAsync(command, parms);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async Task InitStoredProcedures()
        {
            // create procs if they don't exist
            var nl = Environment.NewLine;
            using var connection = CreateConnection();
            var sql = new List<string>() {
                "create or replace procedure usp_append_permission_history( " + nl +
                    "userindex char(36), " + nl +
                    "changecode char(4) " + nl +
                    ") " + nl +
                    "language plpgsql " + nl +
                    "as $$ " + nl +
                    "begin " + nl +
                    " " + nl +
                    "-- append records  " + nl +
                    "insert into USERPERMISSIONHISTORY " + nl +
                    "( " + nl +
                    "UserPermissionId, UserId, PermissionMapId, KeyName, KeyValue " + nl +
                    ") " + nl +
                    "SELECT " + nl +
                    "v.Id, " + nl +
                    "v.UserId,  " + nl +
                    "v.PermissionMapId, " + nl +
                    "v.KeyName,  " + nl +
                    "v.KeyValue " + nl +
                    "FROM VWUSERPERMISSION v " + nl +
                    "WHERE v.UserId = userindex; " + nl +
                    " " + nl +
                    "UPDATE USERPERMISSIONHISTORY h " + nl +
                    "SET GroupId = CASE WHEN GroupId IS NULL THEN 0 ELSE GroupId - 1 END " + nl +
                    "WHERE h.UserId = userindex; " + nl +
                    " " + nl +
                    "-- add change history line record(s) for user " + nl +
                    "INSERT INTO USERPERMISSIONCHANGE " + nl +
                    "( " + nl +
                    "UserId, GroupId, CreateDate " + nl +
                    ")" + nl +
                    "SELECT h.UserId, h.GroupId, h.CreateDate " + nl +
                    "FROM " + nl +
                    "( " + nl +
                    "SELECT UserId, GroupId, Max( createdate ) createdate  " + nl +
                    "FROM USERPERMISSIONHISTORY " + nl +
                    "WHERE UserId = userindex " + nl +
                    "GROUP BY UserId, GroupId " + nl +
                    ") h " + nl +
                    "LEFT JOIN USERPERMISSIONCHANGE c " + nl +
                    "ON  \t  \t h.UserId = c.UserId " + nl +
                    "AND  \t h.CreateDate = c.CreateDate " + nl +
                    "WHERE \t c.Id is null; " + nl +
                    " " + nl +
                    "-- synchronize group indexes " + nl +
                    "UPDATE  \t USERPERMISSIONCHANGE " + nl +
                    "SET  \t GroupId = subquery.GroupId " + nl +
                    "FROM ( " + nl +
                    " \t  \t SELECT UserId, GroupId, Max( CreateDate ) CreateDate  " + nl +
                    " \t  \t FROM USERPERMISSIONHISTORY " + nl +
                    " \t  \t WHERE UserId = userindex " + nl +
                    " \t  \t GROUP BY UserId, GroupId " + nl +
                    " \t ) AS subquery " + nl +
                    "WHERE 1 = 1 " + nl +
                    "AND USERPERMISSIONCHANGE.UserId = subquery.UserId " + nl +
                    "AND USERPERMISSIONCHANGE.CreateDate = subquery.CreateDate " + nl +
                    "AND USERPERMISSIONCHANGE.GroupId != subquery.GroupId; " + nl +
                    " " + nl +
                    "-- set change reason code " + nl +
                    "UPDATE  \t USERPERMISSIONCHANGE " + nl +
                    "SET  \t ReasonCode = changecode " + nl +
                    "WHERE  " + nl +
                    "EXISTS (SELECT 1 FROM REASONCODES WHERE ReasonCode = changecode) " + nl +
                    "AND  \t UserId = userindex " + nl +
                    "AND  \t GroupId = 0 " + nl +
                    "AND \t ReasonCode IS NULL; " + nl +
                    " " + nl +
                    "commit; " + nl +
                    " " + nl +
                    "end;$$"
            };
            foreach (var stmt in sql)
            {
                if (!string.IsNullOrEmpty(stmt))
                {
                    await connection.ExecuteAsync(stmt);
                }
            }
        }

        private async Task InitReasonCodes()
        {
            var command = "INSERT INTO REASONCODES " + Environment.NewLine +
            "( ReasonCode, Reason ) " + Environment.NewLine +
            "SELECT b.Reason, b.Reason " + Environment.NewLine +
            "FROM REASONCODES as a " + Environment.NewLine +
            "RIGHT JOIN " + Environment.NewLine +
            "( " + Environment.NewLine +
            "SELECT  " + Environment.NewLine +
            "\t'{0}' ReasonCode, " + Environment.NewLine +
            "\t {1}  Reason " + Environment.NewLine +
            ") as b " + Environment.NewLine +
            "ON a.ReasonCode = b.ReasonCode " + Environment.NewLine +
            "WHERE a.ReasonCode IS NULL;";

            using var connection = CreateConnection();
            foreach (var code in reasonCodes)
            {
                var stmt = string.Format(command, code.Code, code.Description);
                await connection.ExecuteAsync(stmt);
            }
        }
        private static readonly List<PermissionGroup> permissionGroups = new()
        {
            new() {  Name = "None", GroupId = 100, OrderId = 10, PerRequest = 0, PerMonth = 0, PerYear = 0 },
            new() {  Name = "Guest", GroupId = 110, OrderId = 20, PerRequest = 5, PerMonth = 15, PerYear = 50 },
            new() {  Name = "Silver", GroupId = 120, OrderId = 30, PerRequest = 20, PerMonth = 200, PerYear = 1500 },
            new() {  Name = "Gold", GroupId = 130, OrderId = 40, PerRequest = 100, PerMonth = 1500, PerYear = 10000 },
            new() {  Name = "Platinum", GroupId = 140, OrderId = 50, PerRequest = 1000, PerMonth = 10000, PerYear = 100000 },
            new() {  Name = "Admin", GroupId = 175, OrderId = 100, PerRequest = -1, PerMonth = -1, PerYear = -1, IsVisible = false },
            new() {  Name = "None.Pricing", GroupId = 1000, OrderId = 10, PerRequest = 0, PerMonth = 0, PerYear = 0 },
            new() {  Name = "Guest.Pricing", GroupId = 1010, OrderId = 20, PerRequest = 0, PerMonth = 0, PerYear = 0 },
            new() {  Name = "Silver.Pricing", GroupId = 1020, OrderId = 30, PerRequest = 5, PerMonth = 12, PerYear = 100 },
            new() {  Name = "Gold.Pricing", GroupId = 1030, OrderId = 40, PerRequest = 4, PerMonth = 20, PerYear = 225 },
            new() {  Name = "Platinum.Pricing", GroupId = 1040, OrderId = 50, PerRequest = 3, PerMonth = 30, PerYear = 300 },
            new() {  Name = "Admin.Pricing", GroupId = 1075, OrderId = 100, PerRequest = -1, PerMonth = -1, PerYear = -1, IsVisible = false },
            new() {  Name = "State.Discount.Pricing", GroupId = 2100, OrderId = 10, PerRequest = 15, PerMonth = 2, PerYear = 24 },
            new() {  Name = "County.Discount.Pricing", GroupId = 2200, OrderId = 20, PerRequest = 10, PerMonth = 1, PerYear = 12, },
        };

        private static readonly List<ReasonCodes> reasonCodes = new()
        {
			// user permission changes
			new () { Code = "UC00" , Description = "User account created by system." },
            new () { Code = "UC02" , Description = "User account registration completed." },
            new () { Code = "UC10" , Description = "User Permission level changed."  },
            new () { Code = "UC20" , Description = "User State Subscription changed."  },
            new () { Code = "UC22" , Description = "User County Subscription changed."  },
            new () { Code = "UC30" , Description = "User account temporarily locked, excessive login failures."  },
            new () { Code = "UC32" , Description = "User account locked for non-payment."  },
            new () { Code = "UC34" , Description = "User account locked by administrator."  },
			// user profile changes
			new () { Code = "UP00" , Description = "User account created by system." },
            new () { Code = "UP02" , Description = "User account registration completed." },
            new () { Code = "UP10" , Description = "User contact name changed by user." },
            new () { Code = "UP20" , Description = "User phone changed by user." },
            new () { Code = "UP30" , Description = "User email changed by user." },
            new () { Code = "UP40" , Description = "User address changed by user." },
        };

        private sealed class ReasonCodes
        {
            public string Code { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }
    }
}