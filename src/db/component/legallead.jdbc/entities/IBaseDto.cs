
namespace legallead.jdbc.entities
{
    public interface IBaseDto
    {
        object? this[int fieldId] { get; set; }
        object? this[string field] { get; set; }

        List<string> FieldList { get; }
        string Id { get; set; }
        List<string> InsertFieldList { get; }
        string TableName { get; }
        List<string> UpdateFieldList { get; }
    }
}