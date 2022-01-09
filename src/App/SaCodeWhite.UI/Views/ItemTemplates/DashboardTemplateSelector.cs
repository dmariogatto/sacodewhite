using SaCodeWhite.Models;
using SaCodeWhite.Shared.Models;
using SaCodeWhite.ViewModels;
using System;
using Xamarin.Forms;

namespace SaCodeWhite.UI.Views
{
    public class DashboardTemplateSelector : DataTemplateSelector
    {
        private static readonly DataTemplate AmbulanceServiceTemplate = new DataTemplate(() => CreateItem(DashboardType.AmbulanceService));
        private static readonly DataTemplate EmergencyDepartmentTemplate = new DataTemplate(() => CreateItem(DashboardType.EmergencyDepartment));

        private static View CreateItem(DashboardType dashboard)
        {
            var item = new DashboardItem(dashboard);
            item.SetBinding(DashboardItem.CommandProperty, new Binding(nameof(IDashboardViewModel.HospitalTappedCommand), source: new RelativeBindingSource(RelativeBindingSourceMode.FindAncestorBindingContext, typeof(IDashboardViewModel))));
            item.SetBinding(DashboardItem.CommandParameterProperty, new Binding("."));

            return item;
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is HospitalDashboard dashboard)
            {
                return dashboard.Type switch
                {
                    DashboardType.AmbulanceService => AmbulanceServiceTemplate,
                    DashboardType.EmergencyDepartment => EmergencyDepartmentTemplate,
                    _ => throw new NotSupportedException(),
                };
            }

            throw new NotSupportedException();
        }
    }
}
