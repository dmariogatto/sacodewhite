using SaCodeWhite.ViewModels;
using System;
using Xamarin.Forms;

namespace SaCodeWhite.UI.Views
{
    public class BasePage<T> : ContentPage where T : class, IViewModel
    {
        public T ViewModel => BindingContext as T;

        public BasePage() : base()
        {
            var vm = IoC.ResolveViewModel<T>();
            BindingContext = vm ?? throw new ArgumentNullException(typeof(T).Name);

            SetBinding(TitleProperty, new Binding(nameof(IViewModel.Title)));
            SetDynamicResource(BackgroundColorProperty, Styles.Keys.PageBackgroundColor);

            ViewModel.OnCreate();
        }

        ~BasePage()
        {
            ViewModel.OnDestory();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ViewModel.OnDisappearing();
        }
    }
}