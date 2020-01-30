using System;
using EspIot.Core.Configuration;

namespace Infrastructure.Config
{
    public class ConfigurationManager : IConfigurationManager
    {
        //TODO load configuration from EEPROM/sd card instead of return hardcoded config
        public IConfiguration Load()
        {
            throw new NotImplementedException();
        }

        //TODO implement this when EEPROM/sdcard configuration storage will be implemented 
        public void Save(IConfiguration configuration)
        {
            throw new NotImplementedException();
        }
    }
}
