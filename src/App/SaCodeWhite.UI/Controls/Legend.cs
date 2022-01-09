using System.Collections.Generic;
using Xamarin.Forms;

namespace SaCodeWhite.UI.Controls
{
    public class LegendItem : BindableObject
    {
        public static readonly BindableProperty LabelProperty =
          BindableProperty.Create(
              propertyName: nameof(Label),
              defaultBindingMode: BindingMode.OneWay,
              returnType: typeof(string),
              declaringType: typeof(LegendItem),
              defaultValue: string.Empty);

        public static readonly BindableProperty ColorProperty =
          BindableProperty.Create(
              propertyName: nameof(Color),
              defaultBindingMode: BindingMode.OneWay,
              returnType: typeof(Color),
              declaringType: typeof(LegendItem),
              defaultValue: Color.Default);

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }
    }

    public class Legend : FlexLayout
    {
        public static readonly BindableProperty ItemsSourceProperty =
          BindableProperty.Create(
              propertyName: nameof(ItemsSource),
              defaultBindingMode: BindingMode.OneWay,
              returnType: typeof(IList<LegendItem>),
              declaringType: typeof(Legend),
              defaultValue: default);

        public Legend()
        {
            JustifyContent = FlexJustify.Center;
            Wrap = FlexWrap.Wrap;

            BindableLayout.SetItemTemplate(this, new DataTemplate(() =>
            {
                var layout = new Grid()
                {
                    Padding = (Thickness)App.Current.Resources[Styles.Keys.SmallThickness],
                    ColumnSpacing = (double)App.Current.Resources[Styles.Keys.XSmallSpacing],
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Start,
                };

                layout.ColumnDefinitions.Add(new ColumnDefinition() { Width = 16 });
                layout.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

                layout.Children.Add(new BoxView()
                {
                    CornerRadius = 8,
                    HeightRequest = 16,
                    WidthRequest = 16,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                }, 0, 0);
                layout.Children[0].SetBinding(BoxView.BackgroundColorProperty, new Binding(nameof(LegendItem.Color)));

                layout.Children.Add(new Label()
                {
                    FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                    VerticalTextAlignment = TextAlignment.Center
                }, 1, 0);
                layout.Children[1].SetBinding(Label.TextProperty, new Binding(nameof(LegendItem.Label)));

                return layout;
            }));

            SetBinding(BindableLayout.ItemsSourceProperty, new Binding(nameof(ItemsSource), source: this));
        }

        public IList<LegendItem> ItemsSource
        {
            get => (IList<LegendItem>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
    }
}