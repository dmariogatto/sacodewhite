using MvvmHelpers;
using SaCodeWhite.Models;
using SaCodeWhite.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SaCodeWhite.ViewModels
{
    public abstract class BaseAlertStatusCategoriesViewModel : BaseViewModel
    {
        private readonly Dictionary<AlertStatusType, AlertStatusCategory> _alertStatusCategories = new Dictionary<AlertStatusType, AlertStatusCategory>()
        {
            { AlertStatusType.Green, new AlertStatusCategory(AlertStatusType.Green) },
            { AlertStatusType.Amber, new AlertStatusCategory(AlertStatusType.Amber) },
            { AlertStatusType.Red, new AlertStatusCategory(AlertStatusType.Red) },
            { AlertStatusType.White, new AlertStatusCategory(AlertStatusType.White) },
        };

        public BaseAlertStatusCategoriesViewModel(
            IBvmConstructor bvmConstructor) : base(bvmConstructor)
        {
            AlertStatusCategories = new ObservableRangeCollection<AlertStatusCategory>(_alertStatusCategories.Values.Reverse());
        }

        public ObservableRangeCollection<AlertStatusCategory> AlertStatusCategories { get; private set; }

        protected void UpdateAlertStatusCategoryCount(AlertStatusType status, int count)
            => _alertStatusCategories[status].Count = count;
    }
}