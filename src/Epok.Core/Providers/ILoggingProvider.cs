namespace Epok.Core.Providers
{
    public interface ILoggingProvider : ICrossCuttingProvider
    {
        void Log(string message);
    }
}
