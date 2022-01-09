using SaCodeWhite.Services;
using SaCodeWhite.UI.Attributes;
using SaCodeWhite.ViewModels;
using Xamarin.Forms;

namespace SaCodeWhite.UI.Views
{
    [NavigationRoute(NavigationRoutes.Settings, true)]
    public partial class SettingsPage : BaseFramePage<SettingsViewModel>
    {
        public SettingsPage() : base()
        {
            InitializeComponent();

            if (!IoC.Resolve<IEnvironmentService>().NativeDarkMode)
            {
                RadioButtonSystem.IsVisible = false;
                if (ViewModel.AppTheme == Theme.System)
                    ViewModel.AppTheme = Theme.Light;
            }
        }

        private void ThemeCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                if (ReferenceEquals(RadioButtonSystem, sender))
                    ViewModel.AppTheme = Theme.System;
                else if (ReferenceEquals(RadioButtonLight, sender))
                    ViewModel.AppTheme = Theme.Light;
                else if (ReferenceEquals(RadioButtonDark, sender))
                    ViewModel.AppTheme = Theme.Dark;
            }
        }
    }
}