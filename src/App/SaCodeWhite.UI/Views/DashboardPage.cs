using SaCodeWhite.UI.Attributes;
using SaCodeWhite.UI.Controls;
using SaCodeWhite.UI.Converters;
using SaCodeWhite.ViewModels;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace SaCodeWhite.UI.Views
{
    [NavigationRoute(NavigationRoutes.AmbulanceService)]
    public class AmbulanceServicePage : DashboardPage<AmbulanceServiceViewModel>
    {
        public AmbulanceServicePage() : base() { }
    }

    [NavigationRoute(NavigationRoutes.EmergencyDepartment)]
    public class EmergencyDepartmentPage : DashboardPage<EmergencyDepartmentViewModel>
    {
        public EmergencyDepartmentPage() : base() { }
    }

    public class DashboardPage<T> : BaseFramePage<T> where T : class, IDashboardViewModel
    {
        public DashboardPage() : base()
        {
            SetupRefreshTimer(TimeSpan.FromSeconds(40), ct => ViewModel.LoadDashboardsCommand.ExecuteAsync(ct));

            var collectionView = new CollectionView();

            switch (Device.Idiom)
            {
                case TargetIdiom.Desktop:
                case TargetIdiom.Tablet:
                    collectionView.ItemsLayout = new GridItemsLayout(2, ItemsLayoutOrientation.Vertical);
                    break;
                default:
                    break;
            }

            collectionView.SetBinding(CollectionView.ItemsSourceProperty, new Binding(nameof(IDashboardViewModel.Dashboards)));
            collectionView.ItemTemplate = new DashboardTemplateSelector();
            collectionView.Header = new BoxView() { HeightRequest = 5 };

            HeaderContent = CreateHeader();
            MainContent = collectionView;
        }

        private View CreateHeader()
        {
            var headerLayout = new StackLayout()
            {
                Margin = (Thickness)App.Current.Resources[Styles.Keys.XSmallBottomThickness]
            };

            var alertCategories = new AlertStatusCategories();
            alertCategories.SetBinding(AlertStatusCategories.ItemsSourceProperty, new Binding(nameof(ViewModel.AlertStatusCategories)));

            var updateLbl = new Label()
            {
                Margin = (Thickness)App.Current.Resources[Styles.Keys.MediumRightThickness],
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                FontFamily = (string)App.Current.Resources[Styles.Keys.ItalicFontFamily],
                TextColor = Color.White
            };
            var dtf = CultureInfo.CurrentCulture.DateTimeFormat;
            var dateTimeFmt = $"{{0:{dtf.ShortDatePattern.Replace("yyyy", "yy")} {dtf.ShortTimePattern}}}";
            updateLbl.SetBinding(Label.TextProperty, new Binding(nameof(ViewModel.LastUpdatedUtc), converter: new DateToLocalConverter(), stringFormat: dateTimeFmt));

            var hideUpdatedTrigger = new DataTrigger(typeof(View))
            {
                Binding = new Binding(nameof(ViewModel.LastUpdatedUtc)),
                Value = DateTime.MinValue
            };
            hideUpdatedTrigger.Setters.Add(new Setter() { Property = View.IsVisibleProperty, Value = false });

            updateLbl.Triggers.Add(hideUpdatedTrigger);

            var topLayout = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
            };

            topLayout.Children.Add(alertCategories);
            topLayout.Children.Add(updateLbl);

            headerLayout.Children.Add(topLayout);
            headerLayout.Children.Add(new NoInternetBanner());

            return headerLayout;
        }
    }
}
