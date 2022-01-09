using MvvmHelpers;
using System;
using Xamarin.Essentials.Interfaces;

namespace SaCodeWhite.Services
{
    public class AppPreferences : ObservableObject, IAppPreferences
    {
        private readonly IPreferences _preferences;

        public AppPreferences(IPreferences preferences)
        {
            _preferences = preferences;
        }

        public Theme AppTheme
        {
            get => (Theme)_preferences.Get(nameof(AppTheme), (int)Theme.System);
            set
            {
                if (AppTheme != value)
                {
                    _preferences.Set(nameof(AppTheme), (int)value);
                    OnPropertyChanged();
                }
            }
        }

        public DateTime LastDateOpened
        {
            get => _preferences.Get(nameof(LastDateOpened), DateTime.Now.Date.AddDays(-1));
            set
            {
                if (LastDateOpened != value.Date)
                {
                    _preferences.Set(nameof(LastDateOpened), value.Date);
                    OnPropertyChanged();
                }
            }
        }

        public int DayCount
        {
            get => _preferences.Get(nameof(DayCount), 0);
            set
            {
                if (DayCount != value)
                {
                    _preferences.Set(nameof(DayCount), value);
                    OnPropertyChanged();
                }
            }
        }

        public bool ReviewRequested
        {
            get => _preferences.Get(nameof(ReviewRequested), false);
            set
            {
                if (ReviewRequested != value)
                {
                    _preferences.Set(nameof(ReviewRequested), value);
                    OnPropertyChanged();
                }
            }
        }
    }
}