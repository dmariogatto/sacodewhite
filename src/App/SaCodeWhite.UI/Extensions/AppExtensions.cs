using System;

namespace SaCodeWhite.UI.Extensions
{
    public static class AppExtensions
    {
        public static T GetResource<T>(this App app, string resourceKey)
            => (T)app.Resources[resourceKey];

        public static T GetResource<T>(this App app, string resourceKey, T defaultValue)
            => app.Resources.ContainsKey(resourceKey)
               ? (T)app.Resources[resourceKey]
               : defaultValue;
    }
}
