using SaCodeWhite.Shared.Models;
using SaCodeWhite.UI.Converters;
using Xamarin.Forms;

namespace SaCodeWhite.UI.Controls
{
    public class AlertStatusBadge : Frame
    {
        private readonly static AlertStatusTypeToColorConverter AlertStatusTypeToColor = new AlertStatusTypeToColorConverter();
        private readonly static ColorToContrastColorConverter ColorToContrastColor = new ColorToContrastColorConverter();
        private readonly static EnumToDescriptionConverter EnumToDescription = new EnumToDescriptionConverter() { AllCaps = true };

        public static readonly BindableProperty AlertStatusProperty =
          BindableProperty.Create(
              propertyName: nameof(AlertStatus),
              defaultBindingMode: BindingMode.OneWay,
              returnType: typeof(AlertStatusType),
              declaringType: typeof(AlertStatusBadge),
              defaultValue: AlertStatusType.Unknown);

        public static readonly BindableProperty AlertFontSizeProperty =
          BindableProperty.Create(
              propertyName: nameof(AlertFontSize),
              defaultBindingMode: BindingMode.OneWay,
              returnType: typeof(double),
              declaringType: typeof(AlertStatusBadge),
              defaultValue: Device.GetNamedSize(NamedSize.Default, typeof(Label)));

        private readonly Grid _layout;

        public AlertStatusBadge()
        {
            SetDynamicResource(StyleProperty, Styles.Keys.FrameCardStyle);

            Padding = 0;
            VerticalOptions = LayoutOptions.Center;
            IsClippedToBounds = true;

            _layout = new Grid();
            _layout.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            _layout.RowDefinitions.Add(new RowDefinition());

            var codeLbl = new Label()
            {
                Padding = 0,
                BackgroundColor = Color.DarkSlateGray,
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                Text = Shared.Localisation.Resources.Code.ToUpper(),
                TextColor = Color.White,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
            };

            var statusLbl = new Label()
            {
                Padding = (Thickness)App.Current.Resources[Styles.Keys.SmallThickness],
                FontFamily = (string)App.Current.Resources[Styles.Keys.BoldFontFamily],
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
            };
            statusLbl.SetBinding(Label.BackgroundColorProperty, new Binding(nameof(AlertStatus), converter: AlertStatusTypeToColor));
            statusLbl.SetBinding(Label.FontSizeProperty, new Binding(nameof(AlertFontSize), source: this));
            statusLbl.SetBinding(Label.TextProperty, new Binding(nameof(AlertStatus), converter: EnumToDescription));
            statusLbl.SetBinding(Label.TextColorProperty, new Binding(nameof(BackgroundColor), source: statusLbl, converter: ColorToContrastColor));

            _layout.Children.Add(codeLbl);
            _layout.Children.Add(statusLbl, 0, 1);

            Content = _layout;
        }

        public AlertStatusType AlertStatus
        {
            get => (AlertStatusType)GetValue(AlertStatusProperty);
            set => SetValue(AlertStatusProperty, value);
        }

        public double AlertFontSize
        {
            get => (double)GetValue(AlertFontSizeProperty);
            set => SetValue(AlertFontSizeProperty, value);
        }
    }
}