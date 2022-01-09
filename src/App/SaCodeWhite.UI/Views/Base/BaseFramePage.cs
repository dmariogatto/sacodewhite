using SaCodeWhite.UI.Controls;
using SaCodeWhite.ViewModels;
using System;
using System.Threading;
using Xamarin.Forms;

namespace SaCodeWhite.UI.Views
{
    [ContentProperty(nameof(MainContent))]
    public class BaseFramePage<T> : BasePage<T> where T : class, IViewModel
    {
        public static readonly BindableProperty HeaderContentProperty =
            BindableProperty.Create(
                propertyName: nameof(HeaderContent),
                defaultBindingMode: BindingMode.OneWay,
                returnType: typeof(View),
                declaringType: typeof(BaseFramePage<T>),
                defaultValue: default,
                propertyChanged: OnHeaderContentChanged);

        public static readonly BindableProperty MainContentProperty =
            BindableProperty.Create(
                propertyName: nameof(MainContent),
                defaultBindingMode: BindingMode.OneWay,
                returnType: typeof(View),
                declaringType: typeof(BaseFramePage<T>),
                defaultValue: default);

        private readonly Grid _grid = new Grid();

        private CancellationTokenSource _refreshCts;
        private TimeSpan _refreshInterval;
        private Action<CancellationToken> _refreshAction;

        public BaseFramePage() : base()
        {
            BackgroundColor = (Color)App.Current.Resources[Styles.Keys.PrimaryColor];

            var tabletPadding = Device.Idiom is TargetIdiom.Tablet ? 28 : 0;
            var frame = new Frame()
            {
                CornerRadius = 20,
                Margin = new Thickness(0, 0, 0, -20),
                Padding = new Thickness(tabletPadding, tabletPadding, tabletPadding, 20)
            };
            frame.SetDynamicResource(Frame.BackgroundColorProperty, Styles.Keys.PageBackgroundColor);
            frame.SetBinding(Frame.ContentProperty, new Binding(nameof(MainContent), source: this));

            _grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            _grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });

            _grid.Children.Add(frame, 0, 1);
            _grid.Children.Add(new LoadingIndicator()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
            }, 0, 1);

            Content = _grid;
        }

        public View HeaderContent
        {
            get => (View)GetValue(HeaderContentProperty);
            set => SetValue(HeaderContentProperty, value);
        }

        public View MainContent
        {
            get => (View)GetValue(MainContentProperty);
            set => SetValue(MainContentProperty, value);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            StartRefreshTimer();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _refreshCts?.Cancel();
        }

        protected void SetupRefreshTimer(TimeSpan interval, Action<CancellationToken> refreshAction)
        {
            _refreshInterval = interval;
            _refreshAction = refreshAction;
        }

        protected void TearDownRefreshTimer()
        {
            _refreshCts?.Cancel();

            _refreshInterval = TimeSpan.Zero;
            _refreshAction = null;
        }

        private void StartRefreshTimer()
        {
            if (_refreshAction is null || _refreshInterval <= TimeSpan.Zero)
                return;

            _refreshCts = new CancellationTokenSource();

            // safe copy
            var cts = _refreshCts;

            _refreshAction(cts.Token);

            Device.StartTimer(_refreshInterval, () =>
            {
                if (cts.IsCancellationRequested)
                {
                    cts.Cancel();

                    if (cts == _refreshCts)
                        _refreshCts = null;

                    cts.Dispose();
                    return false;
                }

                if (ViewModel.IsBusy)
                    return true;

                _refreshAction?.Invoke(cts.Token);
                return _refreshAction is not null;
            });
        }

        private static void OnHeaderContentChanged(BindableObject sender, object oldValue, object newValue)
        {
            var page = (BaseFramePage<T>)sender;

            if (oldValue is View oldView)
                page._grid.Children.Remove(oldView);
            if (newValue is View newView)
                page._grid.Children.Insert(0, newView);
        }
    }
}