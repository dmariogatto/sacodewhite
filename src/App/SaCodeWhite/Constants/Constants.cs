using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;

namespace SaCodeWhite
{
    public static class Constants
    {
        static Constants()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resName = assembly.GetManifestResourceNames()
                ?.FirstOrDefault(r => r.EndsWith("settings.json", StringComparison.OrdinalIgnoreCase));

            using var file = assembly.GetManifestResourceStream(resName);
            using var sr = new StreamReader(file);
            using var jtr = new JsonTextReader(sr);

            var j = JsonSerializer.Create().Deserialize<JObject>(jtr);

            ApiUrlBase = j.Value<string>(nameof(ApiUrlBase));
            ApiKeyHospitals = j.Value<string>(nameof(ApiKeyHospitals));
            ApiKeyTriageCategories = j.Value<string>(nameof(ApiKeyTriageCategories));
            ApiKeyAmbulanceDashboards = j.Value<string>(nameof(ApiKeyAmbulanceDashboards));
            ApiKeyEmergencyDepartmentDashboards = j.Value<string>(nameof(ApiKeyEmergencyDepartmentDashboards));
            ApiKeyRegisterDevice = j.Value<string>(nameof(ApiKeyRegisterDevice));
            ApiKeyDeleteDevice = j.Value<string>(nameof(ApiKeyDeleteDevice));
        }

        public const string Email = "outtaapps@gmail.com";
        public const string AndroidId = "com.dgatto.SaCodeWhite";
        public const string AppleId = "XXXXXXXXXX";

        public const string AmbulanceDashboardUrl = "https://www.sahealth.sa.gov.au/wps/themes/html/Portal/js/OBI_DATA/MHSDashboard_rwd.html";
        public const string EmergencyDepartmentDashboardUrl = "https://www.sahealth.sa.gov.au/wps/themes/html/Portal/js/OBI_DATA/EDDashboard_rwd.html";

        public const string AuthHeader = "x-functions-key";

        public static readonly TimeZoneInfo AdelaideTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Australia/Adelaide");
        public static DateTime AdelaideNow => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, AdelaideTimeZone);

        public static readonly string ApiUrlBase;

        public static readonly string ApiKeyHospitals;
        public static readonly string ApiKeyTriageCategories;
        public static readonly string ApiKeyAmbulanceDashboards;
        public static readonly string ApiKeyEmergencyDepartmentDashboards;

        public static readonly string ApiKeyRegisterDevice;
        public static readonly string ApiKeyDeleteDevice;

        public static string AppId => ValueForPlatform(AndroidId, AppleId);

        private static Lazy<DevicePlatform> Platform => new Lazy<DevicePlatform>(() => IoC.Resolve<IDeviceInfo>().Platform);
        private static string ValueForPlatform(string android, string ios)
        {
            if (Platform.Value == DevicePlatform.Android)
                return android;
            if (Platform.Value == DevicePlatform.iOS)
                return ios;

            return string.Empty;
        }
    }
}