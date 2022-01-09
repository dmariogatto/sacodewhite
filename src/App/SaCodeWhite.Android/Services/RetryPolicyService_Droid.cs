using SaCodeWhite.Services;
using Android.Runtime;
using Polly;

namespace SaCodeWhite.Droid.Services
{
    [Preserve(AllMembers = true)]
    public class RetryPolicyService_Droid : IRetryPolicyService
    {
        public PolicyBuilder GetNativeNetRetryPolicy() =>
            Policy.Handle<Java.Net.SocketException>()
                  .Or<Java.IO.IOException>();
    }
}