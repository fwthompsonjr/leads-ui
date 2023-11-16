namespace legallead.records.search.Interfaces
{
    public interface IExcelFileWriter
    {
        void SaveAs(OfficeOpenXml.ExcelPackage pck, string outputFileName);
    }
}