namespace Epok.Core.Providers
{
    public interface ISettingsProvider : ICrossCuttingProvider
    {
        string GetValue(string key);
        T GetValue<T>(string key);
    }
}
