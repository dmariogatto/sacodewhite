using Xamarin.Essentials.Interfaces;

namespace SaCodeWhite
{
    public static class EssentialExtensions
    {
        public static double HeightScaled(this IDeviceDisplay deviceDisplay)
            => deviceDisplay.MainDisplayInfo.Height / deviceDisplay.MainDisplayInfo.Density;
        public static double WidthScaled(this IDeviceDisplay deviceDisplay)
            => deviceDisplay.MainDisplayInfo.Width / deviceDisplay.MainDisplayInfo.Density;
        public static bool IsSmall(this IDeviceDisplay deviceDisplay)
            => deviceDisplay.HeightScaled() <= 640 || deviceDisplay.WidthScaled() <= 320;
    }
}