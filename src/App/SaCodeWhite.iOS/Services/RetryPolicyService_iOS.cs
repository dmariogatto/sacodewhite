using SaCodeWhite.Services;
using Foundation;
using Polly;

namespace SaCodeWhite.iOS.Services
{
    [Preserve(AllMembers = true)]
    public class RetryPolicyService_iOS : IRetryPolicyService
    {
        public PolicyBuilder GetNativeNetRetryPolicy() =>
            Policy.Handle<System.IO.IOException>()
                  .Or<System.AggregateException>();
    }
}