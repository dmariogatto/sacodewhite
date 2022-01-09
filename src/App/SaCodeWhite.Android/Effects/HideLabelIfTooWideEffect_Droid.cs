using SaCodeWhite.Droid.Effects;
using SaCodeWhite.UI.Effects;
using Android.Runtime;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Widget;
using System.ComponentModel;

[assembly: ExportEffect(typeof(HideLabelIfTooWideEffect_Droid), nameof(HideLabelIfTooWideEffect))]
namespace SaCodeWhite.Droid.Effects
{
    [Preserve(AllMembers = true)]
    public class HideLabelIfTooWideEffect_Droid : PlatformEffect
    {
        private Label _element;
        private TextView _control;

        public HideLabelIfTooWideEffect_Droid()
        {
        }

        protected override void OnAttached()
        {
            if (Element is Label lbl && Control is TextView tv)
            {
                _element = lbl;
                _control = tv;

                UpdateVisibility();

                _element.PropertyChanged += OnPropertyChanged;
            }
        }

        protected override void OnDetached()
        {
            if (_element is not null)
                _element.PropertyChanged -= OnPropertyChanged;

            _element = null;
            _control = null;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Label.Width):
                case nameof(Label.Text):
                case nameof(Label.FontAttributes):
                case nameof(Label.FontFamily):
                case nameof(Label.FontSize):
                    UpdateVisibility();
                    break;
            }
        }

        private void UpdateVisibility()
        {
            if (_control is not null && _element is not null && !string.IsNullOrEmpty(_element.Text) && _element.Width >= 0)
            {
                var paint = _control.Paint;
                var width = paint.MeasureText(_element.Text);
                _element.IsVisible = width <= _element.Width;
            }
        }
    }
}