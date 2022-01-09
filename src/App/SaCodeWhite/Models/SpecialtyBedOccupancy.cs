using System;

namespace SaCodeWhite.Models
{
    public class SpecialtyBedOccupancy : AlertModel
    {
        public SpecialtyBedOccupancy(string code, string displayName)
        {
            Code = code;
            DisplayName = displayName;
        }

        private string _code;
        public string Code
        {
            get => _code;
            set => SetProperty(ref _code, value);
        }

        private string _displayName;
        public string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value);
        }

        public override void UpdateProperties()
        {
            base.UpdateProperties();
        }
    }
}