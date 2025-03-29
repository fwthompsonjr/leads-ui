using page.load.utility.Extensions;

namespace page.load.utility.Entities
{
    public class CaseItemDtoMapper
    {
        public string? MappedContent { get; set; } = string.Empty;
        public CaseItemDto? Dto { get; set; }
        public void Map()
        {
            if (!IsMapped()) return;
            var src = MappedContent?.ToInstance<TheAddress>();
            if (Dto == null || src == null || !src.IsValid) return;
            Dto.CaseNumber = src.CaseNumber;
            Dto.CaseStyle = src.CaseHeader;
            Dto.Plaintiff = src.Plaintiff;
            if (!string.IsNullOrWhiteSpace(src.Address)) Dto.Address = src.Address;
        }
        public bool IsMapped()
        {
            if (string.IsNullOrWhiteSpace(MappedContent)) return false;
            var src = MappedContent.ToInstance<TheAddress>();
            if (src == null || !src.IsValid) return false;
            return true;
        }
    }
}
