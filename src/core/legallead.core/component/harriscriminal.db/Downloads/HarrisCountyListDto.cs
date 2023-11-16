namespace legallead.harriscriminal.db.Downloads
{
    public class HarrisCountyListDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime FileDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime MaxFilingDate { get; set; }
        public DateTime MinFilingDate { get; set; }

        public List<HarrisCriminalDto> Data { get; set; } = new();

        public List<HarrisCriminalBo> BusinessData { get; set; } = new();
    }
}