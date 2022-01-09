using SaCodeWhite.UI.Converters;
using SaCodeWhite.UI.Effects;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace SaCodeWhite.UI.Controls
{
    public class StackedBarItem : BindableObject
    {
        public static readonly BindableProperty LabelProperty =
          BindableProperty.Create(
              propertyName: nameof(Label),
              defaultBindingMode: BindingMode.OneWay,
              returnType: typeof(string),
              declaringType: typeof(StackedBarItem),
              defaultValue: string.Empty);

        public static readonly BindableProperty PercentProperty =
          BindableProperty.Create(
              propertyName: nameof(Percent),
              defaultBindingMode: BindingMode.OneWay,
              returnType: typeof(double),
              declaringType: typeof(StackedBarItem),
              defaultValue: 0d);

        public static readonly BindableProperty ColorProperty =
          BindableProperty.Create(
              propertyName: nameof(Color),
              defaultBindingMode: BindingMode.OneWay,
              returnType: typeof(Color),
              declaringType: typeof(StackedBarItem),
              defaultValue: Color.Default);

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public double Percent
        {
            get => (double)GetValue(PercentProperty);
            set => SetValue(PercentProperty, value);
        }

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }
    }

    public class StackedBar : Grid
    {
        private readonly static PercentToGridWidthConverter PercentToGridWidth = new PercentToGridWidthConverter();
        private readonly static ColorToContrastColorConverter ColorToContrastColor = new ColorToContrastColorConverter();

        public static readonly BindableProperty ItemsSourceProperty =
          BindableProperty.Create(
              propertyName: nameof(ItemsSource),
              defaultBindingMode: BindingMode.OneWay,
              returnType: typeof(IList<StackedBarItem>),
              declaringType: typeof(StackedBar),
              defaultValue: default,
              propertyChanged: OnItemsSourceChanged);

        public StackedBar()
        {
            ColumnSpacing = RowSpacing = 0;
            ColumnDefinitions.Add(new ColumnDefinition());
        }

        public IList<StackedBarItem> ItemsSource
        {
            get => (IList<StackedBarItem>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        private void Rebind()
        {
            if (ItemsSource == null || ItemsSource.Count == 0)
            {
                for (var i = 0; i < ColumnDefinitions.Count - 1; i++)
                    ColumnDefinitions.RemoveAt(i);

                Children.Clear();
                return;
            }

            if (Children.Count > ItemsSource.Count)
            {
                var startIdx = ItemsSource.Count - 1;
                var length = Children.Count;
                for (var i = startIdx; i < length; i++)
                {
                    ColumnDefinitions.RemoveAt(i);
                    Children.RemoveAt(i);
                }
            }
            else
            {
                var startIdx = Children.Count;
                var length = ItemsSource.Count;
                for (var i = startIdx; i < length; i++)
                {
                    var colDef = new ColumnDefinition() { Width = 0 };
                    var child = new BoxView();
                    var lbl = new Label()
                    {
                        FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        LineBreakMode = LineBreakMode.NoWrap
                    };

                    var bindingContextPath = $"{nameof(ItemsSource)}[{i}]";
                    colDef.SetBinding(BindingContextProperty, new Binding(bindingContextPath, source: this));
                    child.SetBinding(BindingContextProperty, new Binding(bindingContextPath, source: this));
                    lbl.SetBinding(BindingContextProperty, new Binding(bindingContextPath, source: this));

                    colDef.SetBinding(ColumnDefinition.WidthProperty, new MultiBinding()
                    {
                        Bindings = new[]
                        {
                            new Binding(nameof(StackedBarItem.Percent)),
                            new Binding(nameof(Width), source: this)
                        },
                        Converter = PercentToGridWidth
                    });
                    child.SetBinding(BoxView.BackgroundColorProperty, new Binding(nameof(StackedBarItem.Color)));
                    lbl.SetBinding(Label.TextProperty, new Binding(nameof(StackedBarItem.Label)));
                    lbl.SetBinding(Label.TextColorProperty, new Binding(nameof(StackedBarItem.Color), converter: ColorToContrastColor));

                    var hideLabelTrigger = new DataTrigger(typeof(View))
                    {
                        Binding = new Binding(nameof(StackedBarItem.Label)),
                        Value = string.Empty
                    };
                    hideLabelTrigger.Setters.Add(new Setter() { Property = View.IsVisibleProperty, Value = false });
                    lbl.Triggers.Add(hideLabelTrigger);

                    lbl.Effects.Add(new HideLabelIfTooWideEffect());

                    ColumnDefinitions.Insert(ColumnDefinitions.Count - 1, colDef);
                    Children.Add(child, i, 0);
                    Children.Add(lbl, i, 0);
                }
            }
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            => Rebind();

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (ItemsSource != null && ItemsSource.Count > 0)
            {
                foreach (var item in ItemsSource)
                    SetInheritedBindingContext(item, BindingContext);
            }
        }

        private static void OnItemsSourceChanged(BindableObject sender, object oldValue, object newValue)
        {
            var view = (StackedBar)sender;

            if (oldValue is INotifyCollectionChanged oldCollection)
                oldCollection.CollectionChanged -= view.CollectionChanged;

            if (newValue is INotifyCollectionChanged newCollection)
                newCollection.CollectionChanged += view.CollectionChanged;

            view.Rebind();
        }

        private class PercentToGridWidthConverter : IMultiValueConverter
        {
            private readonly static DoubleToGridLengthConverter DoubleToGridLength = new DoubleToGridLengthConverter();

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                var length = -1d;

                if (values.Length == 2 && values.All(i => i is not null))
                {
                    var percent = (double)values[0];
                    var width = (double)values[1];

                    length = percent * width;
                }

                return length <= 0
                    ? new GridLength(0, GridUnitType.Absolute)
                    : DoubleToGridLength.Convert(length, targetType, parameter, culture);
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}