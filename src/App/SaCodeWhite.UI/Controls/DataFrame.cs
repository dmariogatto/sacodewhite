using Xamarin.Forms;

namespace SaCodeWhite.UI.Controls
{
    public class DataFrame : Frame
    {
        public static readonly BindableProperty TitleTextProperty =
          BindableProperty.Create(
              propertyName: nameof(TitleText),
              defaultBindingMode: BindingMode.OneWay,
              returnType: typeof(string),
              declaringType: typeof(DataFrame),
              defaultValue: string.Empty);

        public static readonly BindableProperty DataTextProperty =
          BindableProperty.Create(
              propertyName: nameof(DataText),
              defaultBindingMode: BindingMode.OneWay,
              returnType: typeof(string),
              declaringType: typeof(DataFrame),
              defaultValue: string.Empty);

        public static readonly BindableProperty DataTextColorProperty =
          BindableProperty.Create(
              propertyName: nameof(DataTextColor),
              defaultBindingMode: BindingMode.OneWay,
              returnType: typeof(Color),
              declaringType: typeof(DataFrame),
              defaultValue: Color.Default);

        private readonly Grid _layout;

        public DataFrame()
        {
            SetDynamicResource(StyleProperty, Styles.Keys.FrameCardStyle);
            SetDynamicResource(DataTextColorProperty, Styles.Keys.PrimaryTextColor);

            _layout = new Grid();
            _layout.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            _layout.RowDefinitions.Add(new RowDefinition());

            var titleLbl = new Label();
            titleLbl.SetBinding(Label.TextProperty, new Binding(nameof(TitleText), source: this));

            var dataLbl = new Label();
            dataLbl.SetDynamicResource(Label.StyleProperty, Styles.Keys.LabelDataStyle);
            dataLbl.SetBinding(Label.TextProperty, new Binding(nameof(DataText), source: this));
            dataLbl.SetBinding(Label.TextColorProperty, new Binding(nameof(DataTextColor), source: this));

            _layout.Children.Add(titleLbl);
            _layout.Children.Add(dataLbl, 0, 1);

            Content = _layout;
        }

        public string TitleText
        {
            get => (string)GetValue(TitleTextProperty);
            set => SetValue(TitleTextProperty, value);
        }

        public string DataText
        {
            get => (string)GetValue(DataTextProperty);
            set => SetValue(DataTextProperty, value);
        }

        public Color DataTextColor
        {
            get => (Color)GetValue(DataTextColorProperty);
            set => SetValue(DataTextColorProperty, value);
        }
    }
}