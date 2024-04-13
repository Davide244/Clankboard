using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clankboard.Classes;

namespace Clankboard.Controls
{
    public class KeybindChangeBtn : Microsoft.UI.Xaml.Controls.HyperlinkButton
    {
        public KeybindsManager.KeybindTypes KeybindType { get; set; }

        public KeybindChangeBtn()
        {
            this.DefaultStyleKey = typeof(KeybindChangeBtn);
            this.Click += KeybindChangeBtn_Click;
        }

        private void KeybindChangeBtn_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => KeybindsManager.KeybindButton_Click(sender, e);
    }
}
