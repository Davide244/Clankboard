using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Clankboard.Services.Navigation;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private Frame? _frame;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Frame? Frame
    {
        get => _frame;
        set => _frame = value;
    }

    public bool CanGoBack => _frame?.CanGoBack ?? false;

    public void NavigateTo<T>() where T : Page
    {
        if (_frame == null)
        {
            throw new InvalidOperationException("Frame is not set. Initialize the Frame property before navigating.");
        }

        var page = _serviceProvider.GetRequiredService<T>();
        _frame.Content = page;
    }

    public void GoBack()
    {
        if (_frame?.CanGoBack == true)
        {
            _frame.GoBack();
        }
    }
}
