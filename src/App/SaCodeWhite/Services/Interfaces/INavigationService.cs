using SaCodeWhite.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaCodeWhite.Services
{
    public interface INavigationService
    {
        void Init();

        IViewModel TopViewModel { get; }

        Task NavigateToAsync<T>(IDictionary<string, string> parameters = null, bool animated = true) where T : IViewModel;
        Task PopAsync(bool animated = true);
        Task PopToRootAsync(bool animated = true);
    }
}