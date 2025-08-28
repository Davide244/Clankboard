using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Clankboard.Views
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty] 
        private string _windowTitle;
        [ObservableProperty]
        private bool _isAlwaysOnTop;
    }
}
