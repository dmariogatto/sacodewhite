using SaCodeWhite.Shared.Models;
using SaCodeWhite.Shared.Models.AmbulanceService;
using SaCodeWhite.Shared.Models.EmergencyDepartment;
using System;

namespace SaCodeWhite.ViewModels
{
    public abstract class BaseDashboardViewModel<T> : BaseAlertStatusCategoriesViewModel where T : IStatus
    {
        public BaseDashboardViewModel(
            IBvmConstructor bvmConstructor) : base(bvmConstructor)
        {
            if (typeof(T) == typeof(AmbulanceServiceDashboard))
                Type = DashboardType.AmbulanceService;
            else if (typeof(T) == typeof(EmergencyDepartmentDashboard))
                Type = DashboardType.EmergencyDepartment;
            else
                throw new NotSupportedException("Unknown dashboard type not supported");
        }

        private DashboardType _type;
        public DashboardType Type
        {
            get => _type;
            private set => SetProperty(ref _type, value);
        }

        private DateTime _lastUpdatedUtc = DateTime.MinValue;
        public DateTime LastUpdatedUtc
        {
            get => _lastUpdatedUtc;
            protected set => SetProperty(ref _lastUpdatedUtc, value);
        }
    }
}