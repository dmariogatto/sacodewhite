using System;

namespace SaCodeWhite.Api.Services
{
    public interface ILogService
    {
        void Error(Exception ex, string message = null);
        void Info(string message);
    }
}
