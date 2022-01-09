using SaCodeWhite.Droid.Renderers;
using Android.Content;
using Android.Runtime;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Button), typeof(ButtonCustomRenderer))]
namespace SaCodeWhite.Droid.Renderers
{
    [Preserve(AllMembers = true)]
    public class ButtonCustomRenderer : ButtonRenderer
    {
        public ButtonCustomRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (Control is Android.Widget.Button btn)
            {
                // Bug in XForms 4.8, style does not work
                btn.SetAllCaps(false);
            }
        }
    }
}