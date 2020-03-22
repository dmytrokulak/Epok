namespace Epok.Core.Domain.Events
{
    /// <summary>
    /// Defines type of an operation which raised an event.
    /// </summary>
    public enum Trigger
    {
        Undefined = 0,
        Added = 10,
        Changed = 30,
        Removed = 50,
    }
}
