using legallead.content.attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace legallead.content.entities
{
    [DbTable(TableName = "ContentLine")]
    public class WebContentLineDto : CommonBaseDto
    {

        public string? ContentId { get; set; }
        public int? VersionId { get; set; }
        public int? LineNbr { get; set; }
        public string? Content { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("ContentId", Comparison)) return ContentId;
                if (fieldName.Equals("VersionId", Comparison)) return VersionId;
                if (fieldName.Equals("LineNbr", Comparison)) return LineNbr;
                if (fieldName.Equals("Content", Comparison)) return Content;
                return null;
            }
            set
            {
                if (field == null) return;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return;
                if (fieldName.Equals("Id", Comparison))
                {
                    Id = ChangeType<string>(value) ?? string.Empty;
                    return;
                }
                if (fieldName.Equals("ContentId", Comparison))
                {
                    ContentId = ChangeType<string>(value) ?? string.Empty;
                    return;
                }
                if (fieldName.Equals("VersionId", Comparison))
                {
                    VersionId = ChangeType<int?>(value);
                    return;
                }
                if (fieldName.Equals("LineNbr", Comparison))
                {
                    LineNbr = ChangeType<int?>(value);
                    return;
                }
                if (fieldName.Equals("Content", Comparison))
                {
                    Content = ChangeType<string>(value) ?? string.Empty;
                }
            }
        }
    }
}
