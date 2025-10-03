using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Clankboard.Views.Pages.Soundboard
{
    public partial class SoundboardPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<SoundboardListItem> _soundboardItems = new();

        [ObservableProperty] private bool _noItemsDisplayVisible = true;


        public SoundboardPageViewModel()
        {
            SoundboardItems.CollectionChanged += SoundboardItems_CollectionChanged;
        }

        private void SoundboardItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NoItemsDisplayVisible = SoundboardItems.Count == 0;
        }
    }
}