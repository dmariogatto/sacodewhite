using System;
using Xamarin.Forms;

namespace SaCodeWhite.UI.Services
{
    public interface IRendererService
    {
        object GetRenderer(VisualElement element);
        bool HasRenderer(VisualElement element);

        void OnRendererSet(VisualElement element, Action<VisualElement, object> callback);
    }
}