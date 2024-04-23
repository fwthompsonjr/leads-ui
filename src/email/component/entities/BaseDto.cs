using legallead.email.attributes;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace legallead.email.entities
{
    internal abstract class BaseDto
    {
        private readonly object locker = new();
        private List<string>? _fieldList;
        protected const StringComparison Comparison = StringComparison.CurrentCultureIgnoreCase;

        public string Id { get; set; } = string.Empty;

        public virtual List<string> FieldList
        {
            get
            {
                if (_fieldList != null) return _fieldList;
                lock (locker)
                {
                    var type = GetType();
                    if (_TableNames.TryGetValue(type, out TableDescriptor? value))
                    {
                        var attr = value.FieldList;
                        _fieldList = [.. attr.Split(',')];
                    }
                    else
                    {
                        var attributes = GetTableAttributes(type);
                        _fieldList = [.. attributes.FieldList.Split(',')];
                    }
                    return _fieldList;
                }
            }
        }

        public abstract object? this[string field] { get; set; }

        public object? this[int fieldId]
        {
            get
            {
                if (fieldId < 0 || fieldId >= FieldList.Count) return null;
                var fieldName = FieldList[fieldId];
                return this[fieldName];
            }
            set
            {
                if (fieldId < 0 || fieldId >= FieldList.Count) return;
                var fieldName = FieldList[fieldId];
                this[fieldName] = value;
            }
        }


        [ExcludeFromCodeCoverage(Justification = "Derived classes should test this property, when needed.")]
        public string TableName
        {
            get
            {
                var type = GetType();
                if (_TableNames.TryGetValue(type, out TableDescriptor? value)) return value.TableName;
                lock (locker)
                {
                    var attributes = GetTableAttributes(type);
                    return attributes.TableName;
                }
            }
        }

        [ExcludeFromCodeCoverage(Justification = "Derived classes should test field mapping.")]
        protected static T? ChangeType<T>(object? source)
        {
            if (DBNull.Value == source || source == null) return default;
            Type t = typeof(T);
            t = Nullable.GetUnderlyingType(t) ?? t;
            if (t == typeof(string))
            {
                var temp = Convert.ToString(source);
                if (temp == null) return default;
                return (T)Convert.ChangeType(temp, t);
            }
            return (T)Convert.ChangeType(source, t);
        }

        private TableDescriptor GetTableAttributes(Type type)
        {
            var attribute = type.GetCustomAttributes()
                                            .Where(f => f.GetType() == typeof(TargetTableAttribute))
                                            .Cast<TargetTableAttribute>()
                                            .FirstOrDefault();
            var attributeName = attribute == null ? type.Name : attribute.TableName;
            var names = type.GetProperties()
                .Where(p => p.CanWrite && p.CanRead)
                .Where(p => p.Name != "Item")
                .Select(s => s.Name)
                .Distinct();
            var fieldList = string.Join(",", names);
            _fieldList ??= names.ToList();
            var addition = new TableDescriptor
            {
                Name = type.Name,
                TableName = attributeName.ToUpper(),
                FieldList = fieldList,
            };
            _TableNames.Add(type, addition);
            return addition;
        }

        private static readonly Dictionary<Type, TableDescriptor> _TableNames = [];

        private sealed class TableDescriptor
        {
            public string TableName { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string FieldList { get; set; } = string.Empty;
        }
    }

}
