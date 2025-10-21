using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Clankboard.Views.Pages.Soundboard
{
    public partial class SoundboardPageViewModel : ObservableObject
    {
        // Services


        [ObservableProperty]
        private ObservableCollection<SoundboardListItem> _soundboardItems = new();

        [ObservableProperty] private bool _noItemsDisplayVisible = true;

        // Commands
        [ObservableProperty] private RelayCommand _addLocalSoundFileCommand;
        [ObservableProperty] private RelayCommand _addInternetSoundFileCommand;
        [ObservableProperty] private RelayCommand _addTTSSoundFileCommand;
        [ObservableProperty] private RelayCommand _mergeSoundboardFileCommand;
        [ObservableProperty] private RelayCommand _clearSoundboardCommand;


        public SoundboardPageViewModel()
        {
            SoundboardItems.CollectionChanged += SoundboardItems_CollectionChanged;

            //AddLocalSoundFileCommand = new RelayCommand(() => AddLocalSoundFile(""));
        }

        private void SoundboardItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NoItemsDisplayVisible = SoundboardItems.Count == 0;
        }

        private void AddLocalSoundFile(string filePath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Invoked when the download audio dialog is complete. Runs the downloader when user input is sufficient.
        /// </summary>
        /// <param name="url">The URL of the sound file.</param>
        private void AddInternetSoundFile(string url)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Invoked when the TTS dialog is complete. Invokes generation of TTS audio file.
        /// </summary>
        /// <param name="ttsText">The text of the TTS</param>
        /// <param name="voice">What voice is used</param>
        private void AddTTSSoundFile(string ttsText, string voice)
        {
            throw new NotImplementedException();
        }

        // Adds all audio from the specified clankboard file to the current soundboard.
        private void MergeSoundBoardFile(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}