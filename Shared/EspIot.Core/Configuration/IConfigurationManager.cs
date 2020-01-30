namespace EspIot.Core.Configuration
{
    public interface IConfigurationManager
    {
        IConfiguration Load();
        void Save(IConfiguration configuration);
    }
}
