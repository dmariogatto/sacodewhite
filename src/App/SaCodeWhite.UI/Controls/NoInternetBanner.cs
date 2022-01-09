using SaCodeWhite.UI.Converters;
using SaCodeWhite.ViewModels;
using Xamarin.Forms;

namespace SaCodeWhite.UI.Controls
{
    public class NoInternetBanner : ContentView
    {
        public static readonly BindableProperty TextColorProperty =
          BindableProperty.Create(
              propertyName: nameof(TextColor),
              defaultBindingMode: BindingMode.OneWay,
              returnType: typeof(Color),
              declaringType: typeof(NoInternetBanner),
              defaultValue: Color.White);

        public NoInternetBanner()
        {
            SetDynamicResource(BackgroundColorProperty, Styles.Keys.PrimaryColor);

            var layout = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = (double)App.Current.Resources[Styles.Keys.XSmallSpacing],
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };

            var iconLbl = new Label
            {
                Text = Fonts.MaterialIconKeys.WifiOff,
                FontFamily = (string)App.Current.Resources[Styles.Keys.MaterialIconsFontFamily],
                VerticalTextAlignment = TextAlignment.Center
            };

            var textLbl = new Label
            {
                Text = Shared.Localisation.Resources.NoInternet,
                VerticalTextAlignment = TextAlignment.Center
            };

            iconLbl.SetBinding(Label.TextColorProperty, new Binding(nameof(TextColor), source: this));
            textLbl.SetBinding(Label.TextColorProperty, new Binding(nameof(TextColor), source: this));

            SetBinding(IsVisibleProperty, new Binding(nameof(IViewModel.HasInternet), converter: new InverseBoolConverter()));

            layout.Children.Add(iconLbl);
            layout.Children.Add(textLbl);

            Content = layout;
        }

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }
    }
}