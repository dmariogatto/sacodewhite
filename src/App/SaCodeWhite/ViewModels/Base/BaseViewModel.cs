using MvvmHelpers.Commands;
using SaCodeWhite.Services;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;

namespace SaCodeWhite.ViewModels
{
    public abstract class BaseViewModel : MvvmHelpers.BaseViewModel, IViewModel
    {
        protected readonly ICodeWhiteService CodeWhiteService;
        protected readonly INavigationService NavigationService;
        protected readonly IAppPreferences AppPrefs;
        protected readonly IDialogService DialogService;
        protected readonly ILogger Logger;

        protected readonly IConnectivity Connectivity;

        public BaseViewModel(
            IBvmConstructor bvmConstructor) : base()
        {
            CodeWhiteService = bvmConstructor.CodeWhiteService;
            NavigationService = bvmConstructor.NavigationService;
            AppPrefs = bvmConstructor.AppPrefs;
            DialogService = bvmConstructor.DialogService;
            Logger = bvmConstructor.Logger;

            Connectivity = bvmConstructor.Connectivity;

            OpenUrlCommand = new AsyncCommand<string>(url => OpenUriCommand.ExecuteAsync(new Uri(url)));
            OpenUriCommand = new AsyncCommand<Uri>(uri => bvmConstructor.Browser.OpenAsync(uri));
        }

        public void TrackEvent(string eventName, IDictionary<string, string> properties = null) =>
            Logger.Event(eventName, properties);
        
        public AsyncCommand<string> LaunchCommand { get; private set; }
        public AsyncCommand<string> OpenUrlCommand { get; private set; }
        public AsyncCommand<Uri> OpenUriCommand { get; private set; }

        public virtual void OnCreate()
        {
        }

        public virtual void OnAppearing()
        {
            HasInternet = Connectivity.NetworkAccess == NetworkAccess.Internet;
            Connectivity.ConnectivityChanged += ConnectivityChanged;
        }

        public virtual void OnDisappearing()
        {
            Connectivity.ConnectivityChanged -= ConnectivityChanged;
        }

        public virtual void OnDestory()
        {
        }

        protected virtual void HasInternetChanged(bool oldValue, bool newValue)
        {
        }

#if DEBUG
        private bool _isDevelopment = true;
#else
        private bool _isDevelopment = false;
#endif
        public bool IsDevelopment
        {
            get => _isDevelopment;
            set => OnPropertyChanged(nameof(IsDevelopment));
        }

        private bool _hasError;
        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        private bool _hasInternet = true;
        public bool HasInternet
        {
            get => _hasInternet;
            set => SetProperty(ref _hasInternet, value);
        }

        private void ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            var hadInternet = HasInternet;
            HasInternet = Connectivity.NetworkAccess == NetworkAccess.Internet;

            if (hadInternet != HasInternet)
                HasInternetChanged(hadInternet, HasInternet);
        }
    }
}