using SaCodeWhite.Models;
using SaCodeWhite.UI.Attributes;
using SaCodeWhite.UI.Controls;
using SaCodeWhite.UI.Converters;
using SaCodeWhite.UI.Extensions;
using SaCodeWhite.ViewModels;
using System;
using Xamarin.Forms;

namespace SaCodeWhite.UI.Views
{
    [NavigationRoute(NavigationRoutes.Overview)]
    public class OverviewPage : BaseFramePage<OverviewViewModel>
    {
        private static readonly MinutesToFriendlyStringConverter MinutesToFriendlyString = new MinutesToFriendlyStringConverter();
        private static readonly MultiplyByConverter MultiplyBy = new MultiplyByConverter() { MinValue = 0 };

        public OverviewPage() : base()
        {
            SetupRefreshTimer(TimeSpan.FromSeconds(40), ct => ViewModel.LoadOverviewsCommand.ExecuteAsync(ct));

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

            collectionView.SetBinding(CollectionView.ItemsSourceProperty, new Binding(nameof(OverviewViewModel.Overviews)));
            collectionView.ItemTemplate = new DataTemplate(CreateOverviewItem);
            collectionView.Header = new BoxView() { HeightRequest = 5 };

            var headerLayout = new StackLayout()
            {
                Margin = (Thickness)App.Current.Resources[Styles.Keys.XSmallBottomThickness]
            };

            headerLayout.Children.Add(new AlertStatusCategories());
            headerLayout.Children.Add(new NoInternetBanner());
            headerLayout.Children[0].SetBinding(AlertStatusCategories.ItemsSourceProperty, new Binding(nameof(ViewModel.AlertStatusCategories)));

            HeaderContent = headerLayout;
            MainContent = collectionView;
        }

        private View CreateOverviewItem()
        {
            var grid = new Grid();

            var hospitalNameLbl = new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Default, typeof(Label)),
                LineBreakMode = LineBreakMode.MiddleTruncation
            };
            hospitalNameLbl.SetDynamicResource(Label.StyleProperty, Styles.Keys.LabelTitleStyle);
            hospitalNameLbl.SetBinding(Label.TextProperty, new MultiBinding()
            {
                Bindings = new[]
                {
                    new Binding(nameof(HospitalOverview.HospitalName)),
                    new Binding(nameof(HospitalOverview.HospitalCode))
                },
                StringFormat = Shared.Localisation.Resources.ItemParenthesesItem
            });
            grid.AddChild(hospitalNameLbl, 0, 0, columnSpan: 2);

            var alertStatusBadge = new AlertStatusBadge();
            alertStatusBadge.SetBinding(AlertStatusBadge.AlertStatusProperty, new Binding(nameof(HospitalOverview.AlertStatus)));
            grid.AddChild(alertStatusBadge, 1, 1);

            var detailsLbl = new Label()
            {
                Margin = (Thickness)App.Current.Resources[Styles.Keys.XSmallLeftThickness],
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                FormattedText = new FormattedString(),
                VerticalTextAlignment = TextAlignment.Center,
            };
            detailsLbl.FormattedText.Spans.Add(new Span() { FontFamily = (string)App.Current.Resources[Styles.Keys.BoldFontFamily] });
            detailsLbl.FormattedText.Spans.Add(new Span() { Text = Shared.Localisation.Resources.Space });
            detailsLbl.FormattedText.Spans.Add(new Span() { Text = Shared.Localisation.Resources.OfTotalCapacity });
            detailsLbl.FormattedText.Spans.Add(new Span() { Text = Environment.NewLine });
            detailsLbl.FormattedText.Spans.Add(new Span() { FontFamily = (string)App.Current.Resources[Styles.Keys.BoldFontFamily] });
            detailsLbl.FormattedText.Spans.Add(new Span() { Text = Shared.Localisation.Resources.Space });
            detailsLbl.FormattedText.Spans.Add(new Span() { Text = Shared.Localisation.Resources.AvgWaitTime.ToLower() });
            detailsLbl.FormattedText.Spans[0].SetBinding(Span.TextProperty,
                new Binding(
                    nameof(HospitalOverview.OccupiedCapacity),
                    converter: MultiplyBy,
                    converterParameter: 100d,
                    stringFormat: Shared.Localisation.Resources.ItemPercent));
            detailsLbl.FormattedText.Spans[4].SetBinding(Span.TextProperty, new Binding(nameof(HospitalOverview.AverageWaitMins), converter: MinutesToFriendlyString));
            grid.AddChild(detailsLbl, 1, 0);

            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.35, GridUnitType.Star) });

            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            var divider = new BoxView() { Margin = (Thickness)App.Current.Resources[Styles.Keys.XSmallTopThickness] }; ;
            divider.SetDynamicResource(BoxView.StyleProperty, Styles.Keys.BoxViewSeparatorStyle);
            grid.AddChild(divider, grid.RowDefinitions.Count, 0, columnSpan: 2);

            grid.RowDefinitions.Add(new RowDefinition() { Height = 1 + divider.Margin.Top });

            grid.Padding = (Thickness)App.Current.Resources[Styles.Keys.ItemMargin];

            grid.RowSpacing =
            grid.ColumnSpacing = (double)App.Current.Resources[Styles.Keys.XSmallSpacing];

            return grid;
        }
    }
}