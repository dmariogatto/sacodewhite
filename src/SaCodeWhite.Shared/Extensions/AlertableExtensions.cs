using SaCodeWhite.Shared.Models;

namespace SaCodeWhite.Shared
{
    public static class AlertableExtensions
    {
        public static (AlertStatusType alertStatus, double occupiedCapacity) GetAlertStatus(this IAlertable alertable, CapacityAlertConfig config = null)
        {
            if (alertable?.Capacity > 0)
            {
                config ??= new CapacityAlertConfig();

                var occCap = alertable.GetOccupiedCapacity();

                if (occCap < config.GreenLimit)
                    return (AlertStatusType.Green, occCap);

                if (occCap >= config.GreenLimit && occCap < config.AmberLimit)
                    return (AlertStatusType.Amber, occCap);

                if (occCap >= config.AmberLimit && occCap < config.RedLimit)
                    return (AlertStatusType.Red, occCap);

                return (AlertStatusType.White, occCap);
            }

            return (AlertStatusType.Unknown, 0);
        }

        private static double GetOccupiedCapacity(this IAlertable alertable)
            => alertable?.Capacity > 0
                ? alertable.PatientTotal / (double)alertable.Capacity
                : -1d;
    }
}