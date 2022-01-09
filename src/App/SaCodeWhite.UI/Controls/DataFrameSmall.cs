using System.Linq;
using Xamarin.Forms;

namespace SaCodeWhite.UI.Controls
{
    public class DataFrameSmall : DataFrame
    {
        public DataFrameSmall() : base()
        {
            if (Content is Grid layout)
            {
                var lbls = layout.Children.OfType<Label>();
                lbls.First().FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label));
                lbls.Last().FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
            }
        }
    }
}