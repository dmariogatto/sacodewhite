using Microsoft.Extensions.Logging;
using SaCodeWhite.Api.Services;
using System;

namespace SaCodeWhite.Functions.Services
{
    public class LogService : ILogService
    {
        private readonly ILogger _logger;

        public LogService(ILogger logger)
        {
            _logger = logger;
        }

        public void Error(Exception ex, string message)
        {
            _logger.LogError(ex, message);
        }

        public void Info(string message)
        {
            _logger.LogInformation(message);
        }
    }
}
