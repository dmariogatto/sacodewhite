using Xamarin.Forms;

namespace SaCodeWhite.UI.Extensions
{
    public static class GridExtensions
    {
        public static void AddChild(this Grid grid, View view, int row, int column, int rowSpan = 1, int columnSpan = 1)
        {
            Grid.SetRow(view, row);
            Grid.SetRowSpan(view, rowSpan);
            Grid.SetColumn(view, column);
            Grid.SetColumnSpan(view, columnSpan);

            grid.Children.Add(view);
        }
    }
}
