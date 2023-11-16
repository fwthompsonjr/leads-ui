namespace legallead.json.interfaces
{
    public interface IDataEntity
    {
        string? Id { get; set; }
        string? Name { get; set; }
        bool IsDeleted { get; set; }
    }
}