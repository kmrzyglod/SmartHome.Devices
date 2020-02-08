using System;
using EspIot.Core.Helpers;

namespace EspIot.Infrastructure.Providers
{
    public class CurrentUtcDateProvider: ICurrentUtcDateProvider
    {
        public DateTime Get()
        {
            return DateTime.UtcNow;
        }
    }
}
