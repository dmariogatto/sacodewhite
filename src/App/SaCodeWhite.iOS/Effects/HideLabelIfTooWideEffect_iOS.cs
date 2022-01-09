using SaCodeWhite.iOS.Effects;
using SaCodeWhite.UI.Effects;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.ComponentModel;

[assembly: ExportEffect(typeof(HideLabelIfTooWideEffect_iOS), nameof(HideLabelIfTooWideEffect))]
namespace SaCodeWhite.iOS.Effects
{
    [Preserve(AllMembers = true)]
    public class HideLabelIfTooWideEffect_iOS : PlatformEffect
    {
        private Label _element;
        private UILabel _control;

        public HideLabelIfTooWideEffect_iOS()
        {
        }

        protected override void OnAttached()
        {
            if (Element is Label lbl && Control is UILabel uiLbl)
            {
                _element = lbl;
                _control = uiLbl;

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
                var width = _control.IntrinsicContentSize.Width;
                _element.IsVisible = width <= _element.Width;
            }
        }
    }
}