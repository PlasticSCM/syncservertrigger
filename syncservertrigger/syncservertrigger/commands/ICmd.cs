namespace syncservertrigger.commands
{
    internal interface ICmd
    {
        string Help { get; }

        string CommandName { get; }

        void Execute(string[] args);
    }
}
