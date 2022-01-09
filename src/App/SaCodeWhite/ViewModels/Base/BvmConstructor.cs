using SaCodeWhite.Services;
using Xamarin.Essentials.Interfaces;

namespace SaCodeWhite.ViewModels
{
    public class BvmConstructor : IBvmConstructor
    {
        public ICodeWhiteService CodeWhiteService { get; }
        public INavigationService NavigationService { get; }
        public IAppPreferences AppPrefs { get; }
        public IDialogService DialogService { get; }
        public ILogger Logger { get; }

        public IBrowser Browser { get; }
        public IConnectivity Connectivity { get; }

        public BvmConstructor(
            ICodeWhiteService codeWhiteService,
            INavigationService navigationService,
            IAppPreferences appPrefs,
            IDialogService dialogService,
            ILogger logger,
            IBrowser browser,
            IConnectivity connectivity) : base()
        {
            CodeWhiteService = codeWhiteService;
            NavigationService = navigationService;
            AppPrefs = appPrefs;
            DialogService = dialogService;
            Logger = logger;

            Browser = browser;
            Connectivity = connectivity;
        }
    }
}