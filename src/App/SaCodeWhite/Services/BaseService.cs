using System;
using System.Runtime.CompilerServices;

namespace SaCodeWhite.Services
{
    public class BaseService
    {
        protected readonly ILogger Logger;

        public BaseService(ILogger logger)
        {
            Logger = logger;
        }
    }
}