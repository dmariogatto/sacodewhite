using SaCodeWhite.Services;
using SaCodeWhite.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using TabbedPage_Droid = Xamarin.Forms.PlatformConfiguration.AndroidSpecific.TabbedPage;
using ToolbarPlacement_Droid = Xamarin.Forms.PlatformConfiguration.AndroidSpecific.ToolbarPlacement;

namespace SaCodeWhite.UI.Services
{
    public class TabbedNavigationService : BaseNavigationService, INavigationService
    {
        private readonly Type[] _tabViewModels = new[]
        {
            typeof(OverviewViewModel),
            typeof(AmbulanceServiceViewModel),
            typeof(EmergencyDepartmentViewModel),
            typeof(SettingsViewModel)
        };

        private NavigationPage MainPage => (NavigationPage)Application.Current.MainPage;
        private TabbedPage TabbedPage => MainPage?.RootPage as TabbedPage;

        public TabbedNavigationService(ILogger logger) : base(logger)
        {
        }

        public IViewModel TopViewModel => MainPage.CurrentPage is TabbedPage tp
            ? (tp.CurrentPage as NavigationPage)?.RootPage?.BindingContext as IViewModel
            : MainPage.CurrentPage.BindingContext as IViewModel;

        public void Init()
        {
            if (Application.Current.MainPage == null)
            {
                var tabbedPage = new TabbedPage();
                tabbedPage.SetDynamicResource(TabbedPage.StyleProperty, Styles.Keys.BaseTabbedPageStyle);

                TabbedPage_Droid.SetToolbarPlacement(tabbedPage, ToolbarPlacement_Droid.Bottom);
                TabbedPage_Droid.SetIsSwipePagingEnabled(tabbedPage, false);
                NavigationPage.SetHasNavigationBar(tabbedPage, false);

                tabbedPage.Children.Add(new NavigationPage()
                {
                    IconImageSource = ImageSource.FromFile(Application.Current.Resources[Styles.Keys.PulseImg]?.ToString()),
                    Title = Shared.Localisation.Resources.Pulse
                });
                tabbedPage.Children.Add(new NavigationPage()
                {
                    IconImageSource = ImageSource.FromFile(Application.Current.Resources[Styles.Keys.AmbulanceImg]?.ToString()),
                    Title = Shared.Localisation.Resources.Ambulance
                });
                tabbedPage.Children.Add(new NavigationPage()
                {
                    IconImageSource = ImageSource.FromFile(Application.Current.Resources[Styles.Keys.HospitalImg]?.ToString()),
                    Title = Shared.Localisation.Resources.ED
                });
                tabbedPage.Children.Add(new NavigationPage()
                {
                    IconImageSource = ImageSource.FromFile(Application.Current.Resources[Styles.Keys.CogImg]?.ToString()),
                    Title = Shared.Localisation.Resources.Settings
                });

                tabbedPage.CurrentPage = tabbedPage.Children.First();

                if (Device.RuntimePlatform == Device.Android)
                {
                    LoadTab(tabbedPage, (NavigationPage)tabbedPage.CurrentPage);
                    // lazy load tabs for the slight start-up gain
                    tabbedPage.CurrentPageChanged += CurrentPageChanged;
                }
                else
                {
                    foreach (var c in tabbedPage.Children.OfType<NavigationPage>())
                        LoadTab(tabbedPage, c);
                }

                var mainNavPage = new NavigationPage(tabbedPage);
                Application.Current.MainPage = mainNavPage;
            }
        }

        public async Task NavigateToAsync<T>(IDictionary<string, string> parameters = null, bool animated = true) where T : IViewModel
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var vmType = typeof(T);

                var navigatedPage = default(Page);
                var navFunc = default(Func<Task>);

                if (_tabViewModels.Contains(vmType))
                {
                    var navTab = (NavigationPage)TabbedPage.Children[_tabViewModels.IndexOf(vmType)];
                    navigatedPage = navTab.RootPage ?? LoadTab(TabbedPage, navTab);
                    navFunc = () =>
                    {
                        TabbedPage.CurrentPage = navTab;
                        return Task.CompletedTask;
                    };
                }
                else
                {
                    navigatedPage = CreatePage<T>();
                    navFunc = () => MainPage.PushAsync(navigatedPage, animated);
                }

                if (navigatedPage != null && parameters?.Any() == true)
                {
                    var pageType = navigatedPage.GetType();
                    var qProps = pageType.GetCustomAttributes(false).OfType<QueryPropertyAttribute>();
                    foreach (var qp in qProps)
                    {
                        if (parameters.TryGetValue(qp.QueryId, out var val) &&
                            pageType.GetProperty(qp.Name) is PropertyInfo pi &&
                            pi.CanWrite)
                        {
                            pi.SetValue(navigatedPage, val);
                        }
                    }
                }

                await navFunc.Invoke();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task PopAsync(bool animated = true)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                await MainPage.PopAsync(animated);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task PopToRootAsync(bool animated = true)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                await MainPage.PopToRootAsync(animated);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void CurrentPageChanged(object sender, EventArgs e)
        {
            if (sender is TabbedPage tabbedPage &&
                tabbedPage.CurrentPage is NavigationPage navPage)
            {
                if (navPage.RootPage == null)
                    LoadTab(tabbedPage, navPage);

                if (tabbedPage.Children.OfType<NavigationPage>().All(np => np.RootPage != null))
                    tabbedPage.CurrentPageChanged -= CurrentPageChanged;
            }
        }

        private Page LoadTab(TabbedPage tabbedPage, NavigationPage navPage)
        {
            if (navPage.RootPage == null && tabbedPage.Children.Count == _tabViewModels.Length)
            {
                var index = tabbedPage.Children.IndexOf(navPage);
                if (index >= 0)
                {
                    var vmType = _tabViewModels[index];
                    var page = CreatePage(vmType);

                    navPage.PushAsync(page, false);
                    return page;
                }
            }

            return null;
        }
    }
}