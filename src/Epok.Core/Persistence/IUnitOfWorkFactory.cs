namespace Epok.Core.Persistence
{
    public interface IUnitOfWorkFactory<T> where T : IUnitOfWork
    {
        T New();
    }
}
