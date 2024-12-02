namespace legallead.jdbc.interfaces
{
    internal interface IHarrisCriminalReader
    {
        Task<bool> Navigate();
        Task<bool> Fetch(string datasetName);
        bool Translate();
        bool Upload();
    }
}
