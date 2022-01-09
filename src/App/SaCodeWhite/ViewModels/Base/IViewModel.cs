using System;
using System.ComponentModel;

namespace SaCodeWhite.ViewModels
{
    public interface IViewModel : INotifyPropertyChanged
    {
        string Title { get; }
        bool IsBusy { get; }
        bool HasInternet { get; }

        void OnCreate();
        void OnAppearing();
        void OnDisappearing();
        void OnDestory();
    }
}