using Polly;

namespace SaCodeWhite.Services
{
    public interface IRetryPolicyService
    {
        /// <summary>
        /// Get retry policy to handle native web request exceptions
        /// </summary>
        PolicyBuilder GetNativeNetRetryPolicy();
    }
}