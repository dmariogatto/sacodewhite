using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SaCodeWhite.Functions.Models
{
    public class NotificationRequest
    {
        public const string ParameterFormat = "$({0})";
        public const string AppleTemplate = "{\"aps\":{\"alert\":{\"title\":\"$(Title)\",\"body\":\"$(Body)\"},\"thread-id\":\"$(AppleThreadId)\"}}";
        public const string AndroidTemplate = "{\"notification\":{\"title\":\"$(Title)\",\"body\":\"$(Body)\"},\"data\":$(AndroidData)}";

        public string Title { get; set; }
        public string Body { get; set; }

        public string[] Tags { get; set; } = Array.Empty<string>();

        public string AppleThreadId { get; set; }
        public Dictionary<string, string> AndroidData { get; set; } = new Dictionary<string, string>();

        public string ToApplePayload()
        {
            return AppleTemplate
                .Replace(string.Format(ParameterFormat, nameof(Title)), Title, StringComparison.InvariantCulture)
                .Replace(string.Format(ParameterFormat, nameof(Body)), Body, StringComparison.InvariantCulture)
                .Replace(string.Format(ParameterFormat, nameof(AppleThreadId)), AppleThreadId, StringComparison.InvariantCulture);
        }

        public string ToAndroidPayload()
        {
            return AndroidTemplate
                .Replace(string.Format(ParameterFormat, nameof(Title)), Title, StringComparison.InvariantCulture)
                .Replace(string.Format(ParameterFormat, nameof(Body)), Body, StringComparison.InvariantCulture)
                .Replace(string.Format(ParameterFormat, nameof(AndroidData)), JsonConvert.SerializeObject(AndroidData), StringComparison.InvariantCulture);
        }
    }
}
