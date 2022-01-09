using SaCodeWhite.iOS.Renderers;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Page), typeof(CustomPageRenderer))]
namespace SaCodeWhite.iOS.Renderers
{
    [Preserve(AllMembers = true)]
    public class CustomPageRenderer : PageRenderer
    {
        public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
        {
            base.TraitCollectionDidChange(previousTraitCollection);

            TraitCollection?.UpdateTheme(previousTraitCollection);
        }
    }
}