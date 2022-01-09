using System;

namespace SaCodeWhite.Shared.Models
{
    public class CapacityAlertConfig
    {
        public CapacityAlertConfig() : this(0.80, 0.95, 1.25) { }

        public CapacityAlertConfig(double greenLimit, double amberLimit, double redLimit)
        {
            GreenLimit = greenLimit;
            AmberLimit = amberLimit;
            RedLimit = redLimit;
        }

        public double GreenLimit { get; }
        public double AmberLimit { get; }
        public double RedLimit { get; }
    }
}
