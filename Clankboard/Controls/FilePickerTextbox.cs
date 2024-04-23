using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Win32.Foundation;

namespace Clankboard.Controls
{
    public enum FilePickerType
    {
        OpenFile,
        SaveFile,
        PickFolder
    }
    public class FilePickerTextbox : TextBox
    {
        // Property for the file picker icon. (DEFAULT IS &#xEC50;)
        public string FilePickerIcon { get; set; } = "\uEC50";
        //public static readonly DependencyProperty FilePickerIconProperty = DependencyProperty.Register("FilePickerIcon", typeof(string), typeof(FilePickerTextbox), new PropertyMetadata("\uEC50"));

        // Property for the file picker type. (DEFAULT IS OpenFile)
        public FilePickerType FilePickerType { get; set; } = FilePickerType.OpenFile;
        //public static readonly DependencyProperty FilePickerTypeProperty = DependencyProperty.Register("FilePickerType", typeof(FilePickerType), typeof(FilePickerTextbox), new PropertyMetadata(FilePickerType.OpenFile));

        // Private refrence to the file picker button
        private Button filePickerButton;

        public FilePickerTextbox()
        {
            this.DefaultStyleKey = typeof(KeybindChangeBtn);
            // get the file picker button from the template
            this.Loaded += (s, e) =>
            {
                filePickerButton = (Button)GetTemplateChild("FilePickerButton");
                filePickerButton.Click += FilePickerButton_Click;
            };
        }

        // Event handler for the file picker button
        private async void FilePickerButton_Click(object sender, RoutedEventArgs e)
        {
            string OutputPath = "";

            var window = (Application.Current as App)?.m_window as MainWindow;
            nint hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

            // Create a new file picker with the correct type
            switch (FilePickerType)
            {
                case FilePickerType.OpenFile:
                    Windows.Storage.Pickers.FileOpenPicker openPicker = new Windows.Storage.Pickers.FileOpenPicker();
                    WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hwnd);
                    openPicker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                    openPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                    openPicker.FileTypeFilter.Add("*");
                    Windows.Storage.StorageFile file = await openPicker.PickSingleFileAsync();
                    if (file != null)
                        OutputPath = file.Path;
                    break;
                case FilePickerType.SaveFile:
                    Windows.Storage.Pickers.FileSavePicker savePicker = new Windows.Storage.Pickers.FileSavePicker();
                    WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);
                    savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                    savePicker.FileTypeChoices.Add("Clank Board", new List<string>() { ".clankboard" });
                    savePicker.SuggestedFileName = "New Document";
                    Windows.Storage.StorageFile saveFile = await savePicker.PickSaveFileAsync();
                    if (saveFile != null)
                        OutputPath = saveFile.Path;
                    break;
                case FilePickerType.PickFolder:
                    Windows.Storage.Pickers.FolderPicker folderPicker = new Windows.Storage.Pickers.FolderPicker();
                    WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);
                    folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                    folderPicker.FileTypeFilter.Add("*");
                    Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();
                    if (folder != null)
                        OutputPath = folder.Path;
                    break;
            }

            // Set the textbox text to the output path
            this.Text = OutputPath;
        }

        public bool isPathValid()
        {
            // Take the text from the textbox and:
            // Check if the path is valid according to the file picker type. (Folder picker = Folder exists? | File Picker = File Exists? | File Saver = Folder Exists & No illegal chars?)

            string FilePath = System.IO.Path.GetDirectoryName(this.Text);
            string FileName = System.IO.Path.GetFileName(FilePath);

            switch (FilePickerType)
            {
                case FilePickerType.OpenFile:
                    return System.IO.File.Exists(this.Text);
                case FilePickerType.SaveFile:
                    return System.IO.Directory.Exists(FilePath) && !System.IO.Path.GetInvalidPathChars().Any(c => FilePath.Contains(c)) && !System.IO.Path.GetInvalidFileNameChars().Any(c => FileName.Contains(c));
                case FilePickerType.PickFolder:
                    return System.IO.Directory.Exists(this.Text);
                default:
                    return false;
            }
        }
    }
}
