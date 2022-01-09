using System.Threading.Tasks;

namespace SaCodeWhite.Services
{
    public interface IDialogService
    {
        void Alert(string message, string title, string ok);

        Task<bool> ConfirmAsync(string message, string title, string ok, string cancel);
        Task<string> ActionSheetAsync(string title, string cancel, string destructive, params string[] buttons);
    }
}
