namespace Codice.SyncServerTrigger.Commands
{
    internal interface ICmd
    {
        string Help { get; }

        string CommandName { get; }

        void Execute(string[] args);
    }
}
