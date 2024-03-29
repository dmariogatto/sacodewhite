﻿using System;
using System.ComponentModel;

namespace SaCodeWhite.Services
{
    public interface IAppPreferences : INotifyPropertyChanged
    {
        Theme AppTheme { get; set; }

        DateTime LastDateOpened { get; set; }
        int DayCount { get; set; }
        bool ReviewRequested { get; set; }
    }
}