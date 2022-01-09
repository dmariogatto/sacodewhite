using SaCodeWhite.Shared.Models;
using System;

namespace SaCodeWhite.ViewModels
{
    public interface IBaseDashboardViewModel : IViewModel
    {
        public DashboardType Type { get; }
        public DateTime LastUpdatedUtc { get; }
    }
}