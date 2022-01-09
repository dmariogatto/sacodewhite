using SaCodeWhite.Models;
using SaCodeWhite.Shared.Models;
using SaCodeWhite.UI.Controls;
using SaCodeWhite.UI.Converters;
using SaCodeWhite.UI.Extensions;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Input;
using Xamarin.Forms;

namespace SaCodeWhite.UI.Views
{
    public class DashboardItem : Grid
    {
        private static readonly AlertStatusTypeToColorConverter AlertStatusTypeToColor = new AlertStatusTypeToColorConverter();
        private static readonly MultiplyByConverter MultiplyBy = new MultiplyByConverter() { MinValue = 0 };

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(
                propertyName: nameof(Command),
                returnType: typeof(ICommand),
                declaringType: typeof(DashboardItem),
                defaultValue: null,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: null);
        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(
                propertyName: nameof(CommandParameter),
                returnType: typeof(object),
                declaringType: typeof(DashboardItem),
                defaultValue: null,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: null);

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => (object)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public DashboardItem(DashboardType dashboard)
        {
            var tappedGesture = new TapGestureRecognizer();
            tappedGesture.SetBinding(TapGestureRecognizer.CommandProperty, new Binding(nameof(Command), source: this));
            tappedGesture.SetBinding(TapGestureRecognizer.CommandParameterProperty, new Binding(nameof(CommandParameter), source: this));

            GestureRecognizers.Add(tappedGesture);

            var hospitalNameLbl = new Label()
            {
                LineBreakMode = LineBreakMode.TailTruncation
            };
            hospitalNameLbl.SetDynamicResource(Label.StyleProperty, Styles.Keys.LabelTitleStyle);
            hospitalNameLbl.SetBinding(Label.TextProperty, new Binding(nameof(HospitalDashboard.HospitalCode)));
            this.AddChild(hospitalNameLbl, 0, 0, columnSpan: 2);

            var totalCapacityLbl = new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                FormattedText = new FormattedString()
            };
            totalCapacityLbl.FormattedText.Spans.Add(new Span() { FontFamily = (string)App.Current.Resources[Styles.Keys.BoldFontFamily] });
            totalCapacityLbl.FormattedText.Spans.Add(new Span() { Text = Shared.Localisation.Resources.Space });
            totalCapacityLbl.FormattedText.Spans.Add(new Span() { Text = Shared.Localisation.Resources.OfTotalCapacity });
            totalCapacityLbl.FormattedText.Spans[0].SetBinding(Span.TextProperty,
                new Binding(
                    nameof(HospitalDashboard.OccupiedCapacity),
                    converter: MultiplyBy,
                    converterParameter: 100d,
                    stringFormat: Shared.Localisation.Resources.ItemPercent));
            this.AddChild(totalCapacityLbl, 1, 0, columnSpan: 2);

            var alertStatusBadge = new AlertStatusBadge();
            alertStatusBadge.SetBinding(AlertStatusBadge.AlertStatusProperty, new Binding(nameof(HospitalDashboard.AlertStatus)));
            this.AddChild(alertStatusBadge, 0, 2, rowSpan: 2);

            var expArrivals = new DataFrame()
            {
                DataTextColor = (Color)App.Current.Resources[Styles.Keys.ExpectedArrivalsColor],
                TitleText = Shared.Localisation.Resources.ExpArrivals
            };
            expArrivals.SetBinding(DataFrame.DataTextProperty, new Binding(nameof(HospitalDashboard.ExpectedArrivals)));
            this.AddChild(expArrivals, 2, 0);

            var wtbs = new DataFrame()
            {
                DataTextColor = (Color)App.Current.Resources[Styles.Keys.WtbsColor],
                TitleText = Shared.Localisation.Resources.Waiting
            };
            wtbs.SetBinding(DataFrame.DataTextProperty, new Binding(nameof(HospitalDashboard.WaitingToBeSeen)));
            this.AddChild(wtbs, 2, 1);

            var treating = new DataFrame()
            {
                DataTextColor = (Color)App.Current.Resources[Styles.Keys.CommencedTreatmentColor],
                TitleText = Shared.Localisation.Resources.Treating
            };
            treating.SetBinding(DataFrame.DataTextProperty, new Binding(nameof(HospitalDashboard.CommencedTreatment)));
            this.AddChild(treating, 2, 2);

            var capacity = new DataFrame()
            {
                TitleText = Shared.Localisation.Resources.Capacity
            };
            capacity.SetBinding(DataFrame.DataTextProperty, new Binding(nameof(HospitalDashboard.Capacity)));
            this.AddChild(capacity, 3, 0);

            var total = CreateTotalDataFrame();
            this.AddChild(total, 3, 1, columnSpan: 2);

            ColumnDefinitions.Add(new ColumnDefinition());
            ColumnDefinitions.Add(new ColumnDefinition());
            ColumnDefinitions.Add(new ColumnDefinition());

            RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            var divider = new BoxView() { Margin = (Thickness)App.Current.Resources[Styles.Keys.XSmallTopThickness] };
            divider.SetDynamicResource(BoxView.StyleProperty, Styles.Keys.BoxViewSeparatorStyle);
            this.AddChild(divider, RowDefinitions.Count, 0, columnSpan: 3);

            RowDefinitions.Add(new RowDefinition() { Height = 1 + divider.Margin.Top });

            Padding = (Thickness)App.Current.Resources[Styles.Keys.ItemMargin];

            RowSpacing =
            ColumnSpacing = (double)App.Current.Resources[Styles.Keys.XSmallSpacing];
        }

        private View CreateTotalDataFrame()
        {
            var frame = new Frame();
            frame.SetDynamicResource(Frame.StyleProperty, Styles.Keys.FrameCardStyle);

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.ColumnSpacing = (double)App.Current.Resources[Styles.Keys.SmallSpacing];

            var totalLbl = new Label() { Text = Shared.Localisation.Resources.Total };
            grid.AddChild(totalLbl, 0, 0, columnSpan: 2);

            var resusLbl = new Label()
            {
                FontFamily = (string)App.Current.Resources[Styles.Keys.ItalicFontFamily],
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                HorizontalTextAlignment = TextAlignment.End,
                VerticalTextAlignment = TextAlignment.Center
            };
            resusLbl.SetBinding(Label.TextProperty, new Binding(nameof(HospitalDashboard.Resuscitation), stringFormat: Shared.Localisation.Resources.ItemResus));
            var hideResusTrigger = new DataTrigger(typeof(View))
            {
                Binding = new Binding(nameof(HospitalDashboard.Resuscitation)),
                Value = 0
            };
            hideResusTrigger.Setters.Add(new Setter() { Property = View.IsVisibleProperty, Value = false });
            resusLbl.Triggers.Add(hideResusTrigger);
            grid.AddChild(resusLbl, 0, 0, columnSpan: 2);

            Color getAlertStatusColor(AlertStatusType type)
                => (Color)AlertStatusTypeToColor.Convert(type, typeof(Color), null, CultureInfo.InvariantCulture);
            var alertStack = new StackedBar()
            {
                VerticalOptions = LayoutOptions.Center,
                ItemsSource = new List<StackedBarItem>()
                {
                    new StackedBarItem () { Color = getAlertStatusColor(AlertStatusType.Green) },
                    new StackedBarItem () { Color = getAlertStatusColor(AlertStatusType.Amber) },
                    new StackedBarItem () { Color = getAlertStatusColor(AlertStatusType.Red) },
                    new StackedBarItem () { Color = getAlertStatusColor(AlertStatusType.White) },
                }
            };
            alertStack.ItemsSource[0].SetBinding(StackedBarItem.PercentProperty, new Binding(nameof(HospitalDashboard.CapacityGreenPercent)));
            alertStack.ItemsSource[1].SetBinding(StackedBarItem.PercentProperty, new Binding(nameof(HospitalDashboard.CapacityAmberPercent)));
            alertStack.ItemsSource[2].SetBinding(StackedBarItem.PercentProperty, new Binding(nameof(HospitalDashboard.CapacityRedPercent)));
            alertStack.ItemsSource[3].SetBinding(StackedBarItem.PercentProperty, new Binding(nameof(HospitalDashboard.CapacityWhitePercent)));

            var innerFrame = new Frame()
            {
                BackgroundColor = Color.LightGray,
                Content = alertStack,
                Margin = new Thickness(1)
            };
            innerFrame.SetBinding(Frame.CornerRadiusProperty, new Binding(
                nameof(Height),
                source: innerFrame,
                converter: MultiplyBy,
                converterParameter: 0.5d));

            var outerFrame = new Frame()
            {
                BackgroundColor = Color.SlateGray,
                Content = innerFrame,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = 26,
            };

            outerFrame.SetBinding(Frame.CornerRadiusProperty, new Binding(
                nameof(Height),
                source: outerFrame,
                converter: MultiplyBy,
                converterParameter: 0.5d));

            grid.AddChild(outerFrame, 1, 0);

            var totalDataLbl = new Label() { VerticalTextAlignment = TextAlignment.Center };
            totalDataLbl.SetDynamicResource(Label.StyleProperty, Styles.Keys.LabelDataStyle);
            totalDataLbl.SetBinding(Label.TextProperty, new Binding(nameof(HospitalDashboard.Occupancy)));
            grid.AddChild(totalDataLbl, 1, 1);

            frame.Content = grid;
            return frame;
        }
    }
}
