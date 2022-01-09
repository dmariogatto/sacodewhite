using SaCodeWhite.Models;
using SaCodeWhite.Shared.Models;
using SaCodeWhite.UI.Attributes;
using SaCodeWhite.UI.Controls;
using SaCodeWhite.UI.Converters;
using SaCodeWhite.UI.Effects;
using SaCodeWhite.UI.Extensions;
using SaCodeWhite.UI.Fonts;
using SaCodeWhite.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace SaCodeWhite.UI.Views
{
    [NavigationRoute(NavigationRoutes.AmbulanceServiceHospital)]
    [QueryProperty(nameof(HospitalCode), NavigationKeys.HospitalCodeQueryProperty)]
    public class AmbulanceServiceHospitalPage : HospitalPage<AmbulanceServiceHospitalViewModel>
    {
        public AmbulanceServiceHospitalPage() : base() { }
    }

    [NavigationRoute(NavigationRoutes.EmergencyDepartmentHospital)]
    [QueryProperty(nameof(HospitalCode), NavigationKeys.HospitalCodeQueryProperty)]
    public class EmergencyDepartmentHospitalPage : HospitalPage<EmergencyDepartmentHospitalViewModel>
    {
        public EmergencyDepartmentHospitalPage() : base() { }
    }

    public class HospitalPage<T> : BaseFramePage<T> where T : class, IHospitalDashboardViewModel
    {
        private static readonly AlertStatusTypeToColorConverter AlertStatusTypeToColor = new AlertStatusTypeToColorConverter();
        private static readonly MinutesToFriendlyStringConverter MinutesToFriendlyString = new MinutesToFriendlyStringConverter();
        private static readonly MultiplyByConverter MultiplyBy = new MultiplyByConverter() { MinValue = 0 };

        public HospitalPage() : base()
        {
            SetupRefreshTimer(TimeSpan.FromSeconds(40), ct => ViewModel.LoadHospitalCommand.ExecuteAsync((HospitalCode, ct)));

            var stackLayout = new StackLayout()
            {
                Padding = (Thickness)App.Current.Resources[Styles.Keys.SmallThickness],
                Spacing = (double)App.Current.Resources[Styles.Keys.SmallSpacing]
            };
            stackLayout.SetBinding(StackLayout.BindingContextProperty, new Binding(nameof(IHospitalDashboardViewModel.Dashboard)));

            var scrollView = new ScrollView() { Content = stackLayout };
            var hideTrigger = new DataTrigger(typeof(View))
            {
                Binding = new Binding(nameof(ViewModel.LastUpdatedUtc)),
                Value = DateTime.MinValue
            };
            hideTrigger.Setters.Add(new Setter() { Property = View.IsVisibleProperty, Value = false });
            scrollView.Triggers.Add(hideTrigger);
            SafeAreaInsetEffect.SetInsets(scrollView, SafeAreaInsets.Bottom);

            var dashboardItem = new DashboardItem(ViewModel.Type) { Padding = 0 };
            stackLayout.Children.Add(dashboardItem);

            switch (ViewModel.Type)
            {
                case DashboardType.AmbulanceService:
                    var clearanceLayout = new Grid()
                    {
                        RowSpacing = (double)App.Current.Resources[Styles.Keys.XSmallSpacing],
                        ColumnSpacing = (double)App.Current.Resources[Styles.Keys.XSmallSpacing],
                    };

                    clearanceLayout.ColumnDefinitions.Add(new ColumnDefinition());
                    clearanceLayout.ColumnDefinitions.Add(new ColumnDefinition());
                    clearanceLayout.ColumnDefinitions.Add(new ColumnDefinition());

                    var last3HrsLbl = new Label()
                    {
                        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                        Text = Shared.Localisation.Resources.AmbulanceClearanceLast3Hrs
                    };
                    stackLayout.Children.Add(last3HrsLbl);

                    var cleared = new DataFrameSmall()
                    {
                        TitleText = Shared.Localisation.Resources.Cleared
                    };
                    cleared.SetBinding(DataFrame.DataTextProperty, new Binding(nameof(HospitalDashboard.AmbulancesClearedLast3Hrs)));
                    clearanceLayout.AddChild(cleared, 0, 0);

                    var avgClearTime = new DataFrameSmall()
                    {
                        TitleText = Shared.Localisation.Resources.AvgTime
                    };
                    avgClearTime.SetBinding(DataFrame.DataTextProperty, new Binding(nameof(HospitalDashboard.AmbulancesAvgClearedMinsLast3Hrs), converter: MinutesToFriendlyString));
                    clearanceLayout.AddChild(avgClearTime, 0, 1);

                    var waitingOver30Mins = new DataFrameSmall()
                    {
                        TitleText = Shared.Localisation.Resources.GreaterThan30Mins
                    };
                    waitingOver30Mins.SetBinding(DataFrame.DataTextProperty, new Binding(nameof(HospitalDashboard.AmbulancesWaitingOver30MinsLast3Hrs)));
                    clearanceLayout.AddChild(waitingOver30Mins, 0, 2);

                    stackLayout.Children.Add(clearanceLayout);
                    stackLayout.Children.Add(new BoxView());
                    stackLayout.Children.Last().SetDynamicResource(BoxView.StyleProperty, Styles.Keys.BoxViewSeparatorStyle);

                    stackLayout.Children.Add(new Label()
                    {
                        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                        Text = Shared.Localisation.Resources.InpatientBedStatus
                    });
                    stackLayout.Children.Add(CreateInpatientLayout());

                    stackLayout.Children.Add(CreateAccessBlockLayout());
                    stackLayout.Children.Add(new BoxView());
                    stackLayout.Children.Last().SetDynamicResource(BoxView.StyleProperty, Styles.Keys.BoxViewSeparatorStyle);

                    stackLayout.Children.Add(new Label()
                    {
                        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                        Text = Shared.Localisation.Resources.SpecialtyBeds
                    });
                    stackLayout.Children.Add(CreateSpecialtyBedsLayout());
                    break;
                case DashboardType.EmergencyDepartment:
                    var edGrid = new Grid() { ColumnSpacing = (double)App.Current.Resources[Styles.Keys.XSmallSpacing] };
                    edGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    edGrid.ColumnDefinitions.Add(new ColumnDefinition());

                    var avgWaitLbl = new Label()
                    {
                        FormattedText = new FormattedString(),
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                    };
                    avgWaitLbl.FormattedText.Spans.Add(new Span() { FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)), FontFamily = (string)App.Current.Resources[Styles.Keys.BoldFontFamily] });
                    avgWaitLbl.FormattedText.Spans.Add(new Span() { Text = Environment.NewLine });
                    avgWaitLbl.FormattedText.Spans.Add(new Span() { FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), Text = Shared.Localisation.Resources.AvgWaitTime.ToLower() });
                    avgWaitLbl.FormattedText.Spans[0].SetBinding(Span.TextProperty, new Binding(nameof(HospitalDashboard.AverageWaitMins), converter: MinutesToFriendlyString));
                    var avgFrame = new Frame() { Content = avgWaitLbl };
                    avgFrame.SetDynamicResource(Frame.StyleProperty, Styles.Keys.FrameCardStyle);
                    edGrid.AddChild(avgFrame, 0, 0);

                    var edStatusLbl = new Label()
                    {
                        FontFamily = (string)App.Current.Resources[Styles.Keys.BoldFontFamily],
                        FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                    };
                    edStatusLbl.SetBinding(Label.TextProperty,
                        new Binding(
                            nameof(HospitalDashboard.AlertStatus),
                            converter: new ConverterTyped<AlertStatusType, string>(value =>
                            {
                                switch (value)
                                {
                                    case AlertStatusType.Green:
                                        return Shared.Localisation.Resources.Normal;
                                    case AlertStatusType.Amber:
                                        return Shared.Localisation.Resources.Busy;
                                    case AlertStatusType.Red:
                                    case AlertStatusType.White:
                                        return Shared.Localisation.Resources.VeryBusy;
                                    default:
                                        return string.Empty;
                                }
                            })));
                    edStatusLbl.SetBinding(Label.TextColorProperty,
                        new Binding(
                            nameof(HospitalDashboard.AlertStatus),
                            converter: new ConverterTyped<AlertStatusType, Color>(value =>
                            {
                                switch (value)
                                {
                                    case AlertStatusType.Green:
                                        return (Color)App.Current.Resources[Styles.Keys.GreenStatusColor];
                                    case AlertStatusType.Amber:
                                        return (Color)App.Current.Resources[Styles.Keys.BusyStatusColor];
                                    case AlertStatusType.Red:
                                    case AlertStatusType.White:
                                        return (Color)App.Current.Resources[Styles.Keys.VeryBusyStatusColor];
                                    default:
                                        return (Color)App.Current.Resources[Styles.Keys.PrimaryColor];
                                }
                            })));
                    var statusFrame = new Frame() { Content = edStatusLbl };
                    statusFrame.SetDynamicResource(Frame.StyleProperty, Styles.Keys.FrameCardStyle);
                    edGrid.AddChild(statusFrame, 0, 1);

                    stackLayout.Children.Add(edGrid);
                    stackLayout.Children.Add(new BoxView());
                    stackLayout.Children.Last().SetDynamicResource(BoxView.StyleProperty, Styles.Keys.BoxViewSeparatorStyle);

                    stackLayout.Children.Add(new Label()
                    {
                        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                        Text = string.Format(
                            Shared.Localisation.Resources.ItemParenthesesItem,
                            Shared.Localisation.Resources.WaitingTimes,
                            Shared.Localisation.Resources.Hours.ToLower())
                    });
                    stackLayout.Children.Add(CreateWaitingTimesLayout());
                    stackLayout.Children.Add(CreateAccessBlockLayout());
                    break;
                default:
                    break;
            }

            MainContent = scrollView;
        }

        public string HospitalCode { get; set; }

        private View CreateInpatientLayout()
        {
            var frame = new Frame();
            frame.SetDynamicResource(Frame.StyleProperty, Styles.Keys.FrameCardStyle);

            var legend = new Legend()
            {
                Margin = (Thickness)App.Current.Resources[Styles.Keys.SmallBottomThickness],
                ItemsSource = new[]
                {
                    new LegendItem() { Label = Shared.Localisation.Resources.WaitingForBed, Color = (Color)App.Current.Resources[Styles.Keys.WaitingColor1] },
                    new LegendItem() { Label = Shared.Localisation.Resources.Occupancy, Color = (Color)App.Current.Resources[Styles.Keys.WaitingColor2] },
                    new LegendItem() { Label = Shared.Localisation.Resources.Capacity, Color = (Color)App.Current.Resources[Styles.Keys.WaitingColor3] },
                }
            };

            StackedBar createBar(Layout<View> parentLayout, Color barColor, string propertyName)
            {
                var bindingPath = $"{nameof(HospitalDashboard.InpatientBeds)}.";

                var stackBar = new StackedBar()
                {
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Start,
                    HeightRequest = 26,
                    ItemsSource = new List<StackedBarItem>()
                    {
                        new StackedBarItem () { Percent = 1, Color = barColor },
                    }
                };
                stackBar.ItemsSource[0].SetBinding(StackedBarItem.LabelProperty, new Binding($"{bindingPath}{propertyName}"));
                stackBar.SetBinding(StackedBar.WidthRequestProperty, new MultiBinding()
                {
                    Bindings = new[]
                    {
                        new Binding($"{bindingPath}{propertyName}"),
                        new Binding($"{bindingPath}{nameof(InpatientBeds.Max)}"),
                        new Binding(nameof(Width), source: parentLayout)
                    },
                    Converter = new TotalToWidthRequestConverter()
                });

                var hideTrigger = new DataTrigger(typeof(View))
                {
                    Binding = new Binding($"{bindingPath}{propertyName}"),
                    Value = 0
                };
                hideTrigger.Setters.Add(new Setter() { Property = View.IsVisibleProperty, Value = false });
                stackBar.Triggers.Add(hideTrigger);

                return stackBar;
            }

            var layout = new StackLayout() { Spacing = 0 };
            layout.Children.Add(legend);
            layout.Children.Add(createBar(layout, (Color)App.Current.Resources[Styles.Keys.WaitingColor1], nameof(InpatientBeds.WaitingForBed)));
            layout.Children.Add(createBar(layout, (Color)App.Current.Resources[Styles.Keys.WaitingColor2], nameof(InpatientBeds.Occupancy)));
            layout.Children.Add(createBar(layout, (Color)App.Current.Resources[Styles.Keys.WaitingColor3], nameof(InpatientBeds.Capacity)));

            var hideTrigger = new DataTrigger(typeof(View))
            {
                Binding = new Binding(nameof(HospitalDashboard.InpatientBeds)),
                Value = null
            };
            hideTrigger.Setters.Add(new Setter() { Property = View.IsVisibleProperty, Value = false });
            layout.Triggers.Add(hideTrigger);

            frame.Content = layout;
            return frame;
        }

        private View CreateSpecialtyBedsLayout()
        {
            var frame = new Frame();
            frame.SetDynamicResource(Frame.StyleProperty, Styles.Keys.FrameCardStyle);

            var repeaterLayout = new StackLayout()
            {
                Spacing = (double)App.Current.Resources[Styles.Keys.XSmallSpacing]
            };
            repeaterLayout.SetBinding(BindableLayout.ItemsSourceProperty, new Binding(nameof(HospitalDashboard.SpecialtyBedOccupancies)));
            BindableLayout.SetItemTemplate(repeaterLayout, new DataTemplate(() =>
            {
                var lbl = new Label()
                {
                    Margin = (Thickness)App.Current.Resources[Styles.Keys.XSmallLeftThickness],
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    VerticalTextAlignment = TextAlignment.Center,
                };
                lbl.SetBinding(Label.TextProperty, new Binding(nameof(SpecialtyBedOccupancy.DisplayName)));

                Color getAlertStatusColor(AlertStatusType type)
                    => (Color)AlertStatusTypeToColor.Convert(type, typeof(Color), null, CultureInfo.InvariantCulture);
                var alertStack = new StackedBar()
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    ItemsSource = new List<StackedBarItem>()
                    {
                        new StackedBarItem () { Color = getAlertStatusColor(AlertStatusType.Green) },
                        new StackedBarItem () { Color = getAlertStatusColor(AlertStatusType.Amber) },
                        new StackedBarItem () { Color = getAlertStatusColor(AlertStatusType.Red) },
                        new StackedBarItem () { Color = getAlertStatusColor(AlertStatusType.White) },
                    }
                };
                alertStack.ItemsSource[0].SetBinding(StackedBarItem.PercentProperty, new Binding(nameof(SpecialtyBedOccupancy.CapacityGreenPercent)));
                alertStack.ItemsSource[1].SetBinding(StackedBarItem.PercentProperty, new Binding(nameof(SpecialtyBedOccupancy.CapacityAmberPercent)));
                alertStack.ItemsSource[2].SetBinding(StackedBarItem.PercentProperty, new Binding(nameof(SpecialtyBedOccupancy.CapacityRedPercent)));
                alertStack.ItemsSource[3].SetBinding(StackedBarItem.PercentProperty, new Binding(nameof(SpecialtyBedOccupancy.CapacityWhitePercent)));

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
                    HeightRequest = 12,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Start,
                };

                outerFrame.SetBinding(Frame.CornerRadiusProperty, new Binding(
                    nameof(Height),
                    source: outerFrame,
                    converter: MultiplyBy,
                    converterParameter: 0.5d));

                outerFrame.SetBinding(Frame.WidthRequestProperty, new MultiBinding()
                {
                    Bindings = new[]
                        {
                            new Binding(nameof(SpecialtyBedOccupancy.Capacity)),
                            new Binding(nameof(HospitalDashboard.SpecialtyBedMax), source: new RelativeBindingSource(RelativeBindingSourceMode.FindAncestorBindingContext, typeof(HospitalDashboard))),
                            new Binding(nameof(Width), source: repeaterLayout)
                        },
                    Converter = new TotalToWidthRequestConverter()
                });

                var outOfLbl = new Label()
                {
                    Margin = (Thickness)App.Current.Resources[Styles.Keys.XSmallRightThickness],
                    FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                    HorizontalTextAlignment = TextAlignment.End,
                    VerticalTextAlignment = TextAlignment.Center,
                };
                outOfLbl.SetBinding(Label.TextProperty, new MultiBinding()
                {
                    Bindings = new[]
                    {
                        new Binding(nameof(SpecialtyBedOccupancy.Occupancy)),
                        new Binding(nameof(SpecialtyBedOccupancy.Capacity)),
                    },
                    StringFormat = Shared.Localisation.Resources.ItemOfItem
                });

                var itemLayout = new Grid();
                itemLayout.ColumnDefinitions.Add(new ColumnDefinition());
                itemLayout.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                itemLayout.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                itemLayout.RowDefinitions.Add(new RowDefinition() { Height = outerFrame.HeightRequest });

                itemLayout.AddChild(lbl, 0, 0);
                itemLayout.AddChild(outOfLbl, 0, 1);
                itemLayout.AddChild(outerFrame, 1, 0, columnSpan: 2);

                var hideTrigger = new DataTrigger(typeof(View))
                {
                    Binding = new Binding(nameof(SpecialtyBedOccupancy.Capacity)),
                    Value = 0
                };
                hideTrigger.Setters.Add(new Setter() { Property = View.IsVisibleProperty, Value = false });
                itemLayout.Triggers.Add(hideTrigger);

                return itemLayout;
            }));

            frame.Content = repeaterLayout;
            return frame;
        }

        private View CreateWaitingTimesLayout()
        {
            var frame = new Frame();
            frame.SetDynamicResource(Frame.StyleProperty, Styles.Keys.FrameCardStyle);

            var legend = new Legend()
            {
                Margin = (Thickness)App.Current.Resources[Styles.Keys.SmallBottomThickness],
                ItemsSource = new[]
                {
                    new LegendItem() { Label = "0-2", Color = (Color)App.Current.Resources[Styles.Keys.WaitingColor1] },
                    new LegendItem() { Label = "2-4", Color = (Color)App.Current.Resources[Styles.Keys.WaitingColor2] },
                    new LegendItem() { Label = "4-8", Color = (Color)App.Current.Resources[Styles.Keys.WaitingColor3] },
                    new LegendItem() { Label = "8-12", Color = (Color)App.Current.Resources[Styles.Keys.WaitingColor4] },
                    new LegendItem() { Label = "12-24", Color = (Color)App.Current.Resources[Styles.Keys.WaitingColor5] },
                    new LegendItem() { Label = "24+", Color = (Color)App.Current.Resources[Styles.Keys.WaitingColor6] },
                }
            };

            var repeaterLayout = new StackLayout()
            {
                Margin = (Thickness)App.Current.Resources[Styles.Keys.SmallTopThickness],
                Spacing = (double)App.Current.Resources[Styles.Keys.XSmallSpacing]
            };
            repeaterLayout.SetBinding(BindableLayout.ItemsSourceProperty, new Binding(nameof(HospitalDashboard.WaitingTimes)));
            BindableLayout.SetItemTemplate(repeaterLayout, new DataTemplate(() =>
            {
                var stackBar = new StackedBar()
                {
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Start,
                    HeightRequest = 26,
                    ItemsSource = new List<StackedBarItem>()
                    {
                        new StackedBarItem () { Color = (Color)App.Current.Resources[Styles.Keys.WaitingColor1] },
                        new StackedBarItem () { Color = (Color)App.Current.Resources[Styles.Keys.WaitingColor2] },
                        new StackedBarItem () { Color = (Color)App.Current.Resources[Styles.Keys.WaitingColor3] },
                        new StackedBarItem () { Color = (Color)App.Current.Resources[Styles.Keys.WaitingColor4] },
                        new StackedBarItem () { Color = (Color)App.Current.Resources[Styles.Keys.WaitingColor5] },
                        new StackedBarItem () { Color = (Color)App.Current.Resources[Styles.Keys.WaitingColor6] },
                    }
                };
                stackBar.ItemsSource[0].SetBinding(StackedBarItem.LabelProperty, new Binding(nameof(WaitingTime.LessThan2Hours)));
                stackBar.ItemsSource[1].SetBinding(StackedBarItem.LabelProperty, new Binding(nameof(WaitingTime.Between2And4Hours)));
                stackBar.ItemsSource[2].SetBinding(StackedBarItem.LabelProperty, new Binding(nameof(WaitingTime.Between4And8Hours)));
                stackBar.ItemsSource[3].SetBinding(StackedBarItem.LabelProperty, new Binding(nameof(WaitingTime.Between8And12Hours)));
                stackBar.ItemsSource[4].SetBinding(StackedBarItem.LabelProperty, new Binding(nameof(WaitingTime.Between12And24Hours)));
                stackBar.ItemsSource[5].SetBinding(StackedBarItem.LabelProperty, new Binding(nameof(WaitingTime.Over24Hours)));

                stackBar.ItemsSource[0].SetBinding(StackedBarItem.PercentProperty, new Binding(nameof(WaitingTime.LessThan2HoursPercent)));
                stackBar.ItemsSource[1].SetBinding(StackedBarItem.PercentProperty, new Binding(nameof(WaitingTime.Between2And4HoursPercent)));
                stackBar.ItemsSource[2].SetBinding(StackedBarItem.PercentProperty, new Binding(nameof(WaitingTime.Between4And8HoursPercent)));
                stackBar.ItemsSource[3].SetBinding(StackedBarItem.PercentProperty, new Binding(nameof(WaitingTime.Between8And12HoursPercent)));
                stackBar.ItemsSource[4].SetBinding(StackedBarItem.PercentProperty, new Binding(nameof(WaitingTime.Between12And24HoursPercent)));
                stackBar.ItemsSource[5].SetBinding(StackedBarItem.PercentProperty, new Binding(nameof(WaitingTime.Over24HoursPercent)));

                stackBar.SetBinding(StackedBar.WidthRequestProperty, new MultiBinding()
                {
                    Bindings = new[]
                        {
                            new Binding(nameof(WaitingTime.Total)),
                            new Binding(nameof(HospitalDashboard.WaitingMax), source: new RelativeBindingSource(RelativeBindingSourceMode.FindAncestorBindingContext, typeof(HospitalDashboard))),
                            new Binding(nameof(Width), source: repeaterLayout)
                        },
                    Converter = new TotalToWidthRequestConverter()
                });

                var lbl = new Label();
                lbl.SetBinding(Label.TextProperty, new Binding(nameof(WaitingTime.DisplayName)));
                var totalLbl = new Label()
                {
                    FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                    VerticalTextAlignment = TextAlignment.Center,
                };
                totalLbl.SetBinding(Label.TextProperty, new Binding(nameof(WaitingTime.Total), stringFormat: Shared.Localisation.Resources.ItemTotal));

                var itemGrid = new Grid();
                itemGrid.RowSpacing = itemGrid.ColumnSpacing = 0;
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition());
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                itemGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                itemGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                itemGrid.AddChild(lbl, 0, 0);
                itemGrid.AddChild(totalLbl, 0, 1);
                itemGrid.AddChild(stackBar, 1, 0, columnSpan: 2);

                var hideTrigger = new DataTrigger(typeof(View))
                {
                    Binding = new Binding(nameof(WaitingTime.Total)),
                    Value = 0
                };
                hideTrigger.Setters.Add(new Setter() { Property = View.IsVisibleProperty, Value = false });
                itemGrid.Triggers.Add(hideTrigger);

                return itemGrid;
            }));

            var layout = new StackLayout() { Spacing = 0 };
            layout.Children.Add(legend);
            layout.Children.Add(repeaterLayout);

            frame.Content = layout;
            return frame;
        }

        private View CreateAccessBlockLayout()
        {
            var layout = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = (double)App.Current.Resources[Styles.Keys.SmallSpacing],
                HorizontalOptions = LayoutOptions.Center,
            };
            layout.Children.Add(new Image()
            {
                HeightRequest = 24,
                WidthRequest = 24,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                Source = new FontImageSource()
                {
                    FontFamily = (string)App.Current.Resources[Styles.Keys.MaterialIconsFontFamily],
                    Color = (Color)App.Current.Resources[Styles.Keys.PrimaryColor],
                    Glyph = MaterialIconKeys.Bed,
                    Size = 30
                }
            });

            var lbl = new Label()
            {
                FontFamily = (string)App.Current.Resources[Styles.Keys.BoldFontFamily],
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                VerticalTextAlignment = TextAlignment.Center,
            };
            lbl.SetBinding(Label.TextProperty, new Binding(nameof(HospitalDashboard.AccessBlock), converter: MultiplyBy, converterParameter: 100d, stringFormat: Shared.Localisation.Resources.ItemPercent));
            lbl.SetBinding(Label.TextColorProperty,
                new Binding(
                    nameof(HospitalDashboard.AccessBlock),
                    converter: new ConverterTyped<double, Color>(value => value switch
                    {
                        >= 0.40 => (Color)App.Current.Resources[Styles.Keys.VeryBusyStatusColor],
                        >= 0.20 => (Color)App.Current.Resources[Styles.Keys.BusyStatusColor],
                        _ => (Color)App.Current.Resources[Styles.Keys.PrimaryTextColor],
                    })));

            var descriptionLbl = new Label()
            {
                FormattedText = new FormattedString(),
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                VerticalTextAlignment = TextAlignment.Center,
            };
            descriptionLbl.FormattedText.Spans.Add(new Span() { Text = Shared.Localisation.Resources.AccessBlock });
            descriptionLbl.FormattedText.Spans.Add(new Span() { Text = Environment.NewLine });
            descriptionLbl.FormattedText.Spans.Add(new Span() { Text = Shared.Localisation.Resources.InpatientsWaitingForBed.ToLower(), FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)), });

            layout.Children.Add(lbl);
            layout.Children.Add(descriptionLbl);

            return layout;
        }

        private class TotalToWidthRequestConverter : IMultiValueConverter
        {
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                var widthRequest = -1d;

                if (values.Length == 3 && values.All(i => i is not null))
                {
                    var width = (double)values[2];
                    if (width >= 0)
                    {
                        var total = (int)values[0];
                        var maxTotal = (int)values[1];
                        widthRequest = total / (double)maxTotal * width;
                    }
                }

                return widthRequest;
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}