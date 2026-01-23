using Microsoft.UI.Xaml.Controls;

namespace Clankboard.Services.Navigation;

public interface INavigationService
{
    Frame? Frame { get; set; }
    bool CanGoBack { get; }
    void NavigateTo<T>() where T : Page;
    void GoBack();
}
