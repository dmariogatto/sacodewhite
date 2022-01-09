using System;

namespace SaCodeWhite
{
    public static class AppCenterEvents
    {
        public static class PageView
        {
            public const string OverviewView = "overview_view";
            public const string AmbulanceServiceView = "ambulance_service_view";
            public const string EmergencyDepartmentView = "emergency_department_view";
            public const string HospitalView = "hospital_view";
            public const string SettingsView = "settings_view";
        }

        public static class Setting
        {
            public const string AppTheme = "app_theme";
        }

        public static class Action
        {
            public const string ReviewRequested = "review_requested";
            public const string AppAction = "app_action";
        }
    }
}