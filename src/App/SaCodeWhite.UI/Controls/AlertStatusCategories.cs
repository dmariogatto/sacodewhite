using SaCodeWhite.Models;
using SaCodeWhite.UI.Converters;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SaCodeWhite.UI.Controls
{
    public class AlertStatusCategories : StackLayout
    {
        private static readonly AlertStatusTypeToColorConverter AlertStatusTypeToColor = new AlertStatusTypeToColorConverter();
        private static readonly MultiplyByConverter MultiplyBy = new MultiplyByConverter() { MinValue = 0 };

        public static readonly BindableProperty ItemsSourceProperty =
          BindableProperty.Create(
              propertyName: nameof(ItemsSource),
              defaultBindingMode: BindingMode.OneWay,
              returnType: typeof(IList<AlertStatusCategory>),
              declaringType: typeof(AlertStatusCategories),
              defaultValue: default);

        public AlertStatusCategories()
        {
            Margin = (Thickness)App.Current.Resources[Styles.Keys.MediumLeftThickness];
            Orientation = StackOrientation.Horizontal;
            Spacing = (double)App.Current.Resources[Styles.Keys.SmallSpacing];

            BindableLayout.SetItemTemplate(this, new DataTemplate(() =>
            {
                var frame = new Frame();

                frame.SetBinding(Frame.BackgroundColorProperty, new Binding(nameof(AlertStatusCategory.AlertStatus), converter: AlertStatusTypeToColor));
                frame.SetBinding(Frame.CornerRadiusProperty, new Binding(nameof(Height), source: frame, converter: MultiplyBy, converterParameter: 0.5d));
                frame.SetBinding(Frame.WidthRequestProperty, new Binding(nameof(Height), source: frame));

                var lbl = new Label()
                {
                    Margin = (Thickness)App.Current.Resources[Styles.Keys.XSmallThickness],
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    FontFamily = (string)App.Current.Resources[Styles.Keys.BoldFontFamily],
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label))
                };

                lbl.SetBinding(Label.TextProperty, new Binding(nameof(AlertStatusCategory.Count)));
                lbl.SetBinding(Label.TextColorProperty, new Binding(nameof(BackgroundColor), source: frame, converter: new ColorToContrastColorConverter()));

                frame.Content = lbl;

                return frame;
            }));

            SetBinding(BindableLayout.ItemsSourceProperty, new Binding(nameof(ItemsSource), source: this));
        }

        public IList<AlertStatusCategory> ItemsSource
        {
            get => (IList<AlertStatusCategory>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
    }
}