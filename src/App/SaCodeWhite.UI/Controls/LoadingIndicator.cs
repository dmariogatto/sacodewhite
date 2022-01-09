using SaCodeWhite.ViewModels;
using Xamarin.Forms;

namespace SaCodeWhite.UI.Controls
{
    public class LoadingIndicator : Frame
    {
        public static readonly BindableProperty IsBusyProperty =
          BindableProperty.Create(
              propertyName: nameof(IsBusy),
              returnType: typeof(bool),
              declaringType: typeof(LoadingIndicator),
              defaultValue: false);

        public LoadingIndicator()
        {
            InputTransparent = true;
            HorizontalOptions = LayoutOptions.Center;
            VerticalOptions = LayoutOptions.End;

            SetDynamicResource(MarginProperty, Styles.Keys.MediumThickness);
            SetDynamicResource(PaddingProperty, Styles.Keys.XSmallThickness);
            SetDynamicResource(StyleProperty, Styles.Keys.FrameCardStyle);
            SetDynamicResource(BorderColorProperty, Styles.Keys.PrimaryColor);

            var activityIndicator = new ActivityIndicator()
            {
                WidthRequest = 24,
                HeightRequest = 24
            };
            activityIndicator.SetDynamicResource(ActivityIndicator.ColorProperty, Styles.Keys.PrimaryColor);
            activityIndicator.SetBinding(ActivityIndicator.IsRunningProperty,
                new Binding(nameof(IsBusy), source: this));

            SetBinding(IsVisibleProperty, new Binding(nameof(IsBusy), source: this));
            SetBinding(IsBusyProperty, new Binding(nameof(BaseViewModel.IsBusy)));

            Content = activityIndicator;
        }

        public bool IsBusy
        {
            get => (bool)GetValue(IsBusyProperty);
            set => SetValue(IsBusyProperty, value);
        }
    }
}