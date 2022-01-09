using SaCodeWhite.Services;
using Xamarin.Essentials.Interfaces;

namespace SaCodeWhite.ViewModels
{
    public interface IBvmConstructor
    {
        ICodeWhiteService CodeWhiteService { get; }
        INavigationService NavigationService { get; }
        IAppPreferences AppPrefs { get; }
        IDialogService DialogService { get; }
        ILogger Logger { get; }

        IBrowser Browser { get; }
        IConnectivity Connectivity { get; }
    }
}