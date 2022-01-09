using SaCodeWhite.Shared.Attributes;

namespace SaCodeWhite.Shared.Models
{
    public enum PlatformType
    {
        Unknown,
        Apple,
        Google,
    }

    public enum DashboardType
    {
        Unknown,
        [Display(nameof(Localisation.Resources.AmbulanceService), typeof(Localisation.Resources))]
        AmbulanceService,
        [Display(nameof(Localisation.Resources.EmergencyDepartment), typeof(Localisation.Resources))]
        EmergencyDepartment,
    }

    public enum AlertStatusType
    {
        Unknown,
        [Display(nameof(Localisation.Resources.Green), typeof(Localisation.Resources))]
        Green,
        [Display(nameof(Localisation.Resources.Amber), typeof(Localisation.Resources))]
        Amber,
        [Display(nameof(Localisation.Resources.Red), typeof(Localisation.Resources))]
        Red,
        [Display(nameof(Localisation.Resources.White), typeof(Localisation.Resources))]
        White,
    }
}
