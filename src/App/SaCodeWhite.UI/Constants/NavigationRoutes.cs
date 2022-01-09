using System;

namespace SaCodeWhite.UI
{
    public static class NavigationRoutes
    {
        public const string AbsoluteFormat = "//{0}";

        public const string Overview = "overview";
        public const string AmbulanceService = "ambulanceservice";
        public const string EmergencyDepartment = "emergencydepartment";
        public const string Settings = "settings";

        public const string AmbulanceServiceHospital = "ambulanceservicehospital";
        public const string EmergencyDepartmentHospital = "emergencydepartmenthospital";

        public static string ToAbsolute(string uri) => string.Format(AbsoluteFormat, uri);
        public static string ToAbsolute(string uri, params string[] queryProperties) => ToAbsolute(AppendQueryProperties(uri, queryProperties));
        public static string AppendQueryProperties(string uri, params string[] queryProperties) => $"{uri}?{string.Join("&", queryProperties)}";
        public static string ToQueryProperty(string prop, object value) => $"{prop}={value}";
    }
}