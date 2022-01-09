using System;
using System.Threading.Tasks;

namespace SaCodeWhite.Services
{
    public interface INotificationService
    {
        DateTime RegisteredUtc { get; }

        Task<bool> RefreshDeviceRegistrationAsync();

        string[] GetTags();
        bool HasTag(string tag);
        void AddTag(string tag);
        void RemoveTag(string tag);
    }
}