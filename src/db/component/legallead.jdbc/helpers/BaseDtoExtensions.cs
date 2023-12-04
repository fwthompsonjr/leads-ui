using legallead.jdbc.entities;
using System.Text;

namespace legallead.jdbc.helpers
{
    internal static class BaseDtoExtensions
    {
        public static Dapper.DynamicParameters SelectParameters(this BaseDto obj, BaseDto? predicate = null)
        {
            var parms = new Dapper.DynamicParameters();
            if (predicate != null)
            {
                List<string> search = obj.FieldList.FindAll(f =>
                {
                    var objvalue = predicate[f];
                    if (objvalue == null) return false;
                    var dfvalue = GetDefaultValue(objvalue.GetType());
                    return !objvalue.Equals(dfvalue);
                });
                search.ForEach(f =>
                {
                    parms.Add(f, predicate[f]);
                });
            }
            return parms;
        }

        public static Dapper.DynamicParameters UpdateParameters(this BaseDto obj)
        {
            var parms = new Dapper.DynamicParameters();
            var fields = new List<string>(obj.UpdateFieldList);
            if (!fields.Contains("Id")) { fields.Add("Id"); }
            fields.ForEach(f => parms.Add(f, obj[f]));
            return parms;
        }

        public static Dapper.DynamicParameters InsertParameters(this BaseDto obj)
        {
            var parms = new Dapper.DynamicParameters();
            var fields = obj.InsertFieldList;
            fields.ForEach(f => parms.Add(f, obj[f]));
            return parms;
        }

        public static string SelectSQL<T>(this T obj, T? predicate = null, string delimiter = "@", bool mySqlFormat = false)
            where T : BaseDto
        {
            var builder = new StringBuilder($"SELECT * {Environment.NewLine} FROM [{obj.TableName}] ");
            if (predicate != null)
            {
                List<string> search = obj.FieldList.FindAll(f =>
                {
                    var objvalue = predicate[f];
                    if (objvalue == null) return false;
                    var dfvalue = GetDefaultValue(objvalue.GetType());
                    return !objvalue.Equals(dfvalue);
                });

                if (search.Any())
                    builder.AppendLine($"{Environment.NewLine} WHERE ");

                search.ForEach(f =>
                {
                    // when not the first item
                    var comma = f == search[0] ? string.Empty : " AND ";
                    builder.AppendLine($"{comma} [{f}] = {delimiter}{f}");
                });
            }
            builder = FormatSQL(builder, mySqlFormat);
            builder.Append(';');
            return builder.ToString();
        }

        public static string InsertSQL<T>(this T obj, string delimiter = "@", bool mySqlFormat = false)
            where T : BaseDto
        {
            var builder = new StringBuilder($"INSERT INTO [{obj.TableName}] {Environment.NewLine} ( {Environment.NewLine}");
            var fields = new StringBuilder();
            var values = new StringBuilder();
            // exclude any fields without a value
            var list = obj.InsertFieldList;
            list.ForEach(f =>
            {
                // when not the first item
                var comma = f == list[0] ? "    " : "   ,";
                fields.AppendLine($"{comma}[{f}]");
                values.AppendLine($"{comma}{delimiter}{f}");
            });
            builder.Append(fields);
            builder.AppendLine(" ) VALUES ( ");
            builder.Append(values);
            builder.AppendLine(" );");

            builder = FormatSQL(builder, mySqlFormat);
            return builder.ToString();
        }

        public static string UpdateByIdSQL<T>(this T obj, T? predicate = null, bool mySqlFormat = false) where T : BaseDto
        {
            var builder = new StringBuilder($"UPDATE [{obj.TableName}] {Environment.NewLine} SET ");
            var fields = new StringBuilder();
            var list = obj.UpdateFieldList.FindAll(f => !f.Equals("Id"));

            list.ForEach(f =>
            {
                // when not the first item
                var comma = f == list[0] ? string.Empty : "   ,";
                fields.AppendLine($"{comma}[{f}] = @{f}");
            });
            builder.Append(fields);
            if (predicate == null)
                builder.AppendLine($" WHERE [Id] = @Id");

            if (predicate != null)
            {
                var search = obj.UpdateFieldList
                    .FindAll(f => predicate[f] != null)
                    .FindAll(f => !f.Equals("Id"));
                search.ForEach(f =>
                {
                    // when not the first item
                    var comma = (f == search[0]) ? " WHERE " : " AND ";
                    builder.AppendLine($"{comma}[tbl].[{f}] = @{f}");
                });
            }
            builder = FormatSQL(builder, mySqlFormat);
            builder.Append(';');
            return builder.ToString();
        }

        public static string DeleteSQL(this BaseDto obj, bool mySqlFormat = false)
        {
            var builder = new StringBuilder($"DELETE [tbl] {Environment.NewLine}");
            builder.AppendLine($" FROM [{obj.TableName}] [tbl] ");
            builder.AppendLine($" WHERE [Id] = @Id;");
            builder = FormatSQL(builder, mySqlFormat);
            return builder.ToString();
        }

        public static Dapper.DynamicParameters DeleteParameters(this BaseDto obj)
        {
            var parms = new Dapper.DynamicParameters();
            var fields = new List<string>("Id".Split(','));
            fields.ForEach(f => parms.Add(f, obj[f]));
            return parms;
        }

        private static object? GetDefaultValue(this Type t)
        {
            if (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
                return Activator.CreateInstance(t);
            else
                return null;
        }

        private static StringBuilder FormatSQL(StringBuilder builder, bool isSQLServer)
        {
            if (!isSQLServer)
            {
                builder.Replace('['.ToString(), string.Empty);
                builder.Replace(']'.ToString(), string.Empty);
            }
            return builder;
        }
    }
}