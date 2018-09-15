using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreCityInfo
{
    public static class ApplicationLogging
    {
        private static ILoggerFactory _factory = null;

        public static void ConfigureLogger(ILoggerFactory factory)
        {
            _factory = factory;
        }

        public static ILogger<T> CreateLogger<T>() => _factory.CreateLogger<T>();
    }
}
