namespace legallead.desktop.interfaces
{
    internal interface IUserMailboxMapper
    {
        string Substitute(IMailPersistence? persistence, string source);
    }
}
