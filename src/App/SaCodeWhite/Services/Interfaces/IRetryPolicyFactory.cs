using Polly;

namespace SaCodeWhite.Services
{
    public interface IRetryPolicyFactory
    {
        PolicyBuilder GetNetRetryPolicy();
    }
}