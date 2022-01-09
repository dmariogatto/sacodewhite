using SaCodeWhite.Services;
using System.Threading.Tasks;

namespace SaCodeWhite.UI.Services
{
    public class DialogService : IDialogService
    {
        public void Alert(string message, string title, string ok)
        {
            App.Current.MainPage.DisplayAlert(title, message, ok);
        }

        public async Task<bool> ConfirmAsync(string message, string title, string ok, string cancel)
        {
            var result = await App.Current.MainPage.DisplayPromptAsync(title, message, ok, cancel);
            return result == ok;
        }

        public Task<string> ActionSheetAsync(string title, string cancel, string destructive, params string[] buttons)
        {
            return App.Current.MainPage.DisplayActionSheet(title, cancel, destructive, buttons);
        }
    }
}
