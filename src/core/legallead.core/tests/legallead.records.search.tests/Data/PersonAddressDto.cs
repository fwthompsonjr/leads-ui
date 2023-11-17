using System.Globalization;

namespace legallead.records.search.Tests.Data
{
    public class PersonAddressDto
    {

        #region FieldList Helpers

        private const string FieldNames = @"Name,FirstName,LastName,Zip," +
            @"Address1,Address2,Address3," +
            @"Case Number,Date Filed,Court," +
            @"Case Type,case style,plantiff";
        private string? _fieldNames;
        private List<string>? _fieldList;

        protected string LoweredFieldNames
        {
            get
            {
                return _fieldNames ??= FieldNames.ToLower(CultureInfo.CurrentCulture);
            }
        }

        public List<string> FieldList
        {
            get
            {
                return _fieldList ??= LoweredFieldNames.Split(',').ToList();
            }
        }

        #endregion

        #region Properties

        public string Name { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Address3 { get; set; } = string.Empty;
        public string CaseNumber { get; set; } = string.Empty;
        public string DateFiled { get; set; } = string.Empty;
        public string Court { get; set; } = string.Empty;
        public string CaseType { get; set; } = string.Empty;
        public string CaseStyle { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Plantiff { get; set; } = string.Empty;
        public bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(Zip))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(Address1))
                {
                    return false;
                }

                return true;
            }
        }

        #endregion

    }
}
