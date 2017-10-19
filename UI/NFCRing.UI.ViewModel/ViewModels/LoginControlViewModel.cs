﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using NFCRing.UI.ViewModel.Services;

namespace NFCRing.UI.ViewModel
{
    public class LoginControlViewModel : ContentViewModel
    {
        private readonly IRepositoryService _repositoryService;
        private readonly IDialogService _dialogService;
        private ObservableCollection<RingItemViewModel> _items;
        private RingItemViewModel _selectedItem;
        private bool _isBusy;

        public ObservableCollection<RingItemViewModel> Items
        {
            get => _items ?? (_items = new ObservableCollection<RingItemViewModel>());
            set => Set(ref _items, value);
        }

        public RingItemViewModel SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref _isBusy, value);
        }

        /// <summary>
        /// Add new ring item command.
        /// </summary>
        public RelayCommand AddCommand { get; }

        /// <summary>
        /// Remove ring item command.
        /// </summary>
        public RelayCommand<RingItemViewModel> RemoveCommand { get; }

        /// <summary>
        /// Ctor.
        /// </summary>
        public LoginControlViewModel(IRepositoryService repositoryService, IDialogService dialogService)
        {
            _repositoryService = repositoryService;
            _dialogService = dialogService;

            Title = "NFC Ring Login Control";

            AddCommand = new RelayCommand(Add);
            RemoveCommand = new RelayCommand<RingItemViewModel>(Remove);
            PropertyChanged += OnPropertyChanged;
        }

        public async Task InitializeAsync()
        {
            var items = await _repositoryService.GetRingsAsync();

#if DEBUG
            for (int i = 1; i < 5; i++)
            {
                Items.Add(new RingItemViewModel {Name = $"Ring {i}", Token = Guid.NewGuid().ToString()});

            }
#endif
        }

        private void Add()
        {
            ServiceLocator.Current.GetInstance<MainViewModel>().SetContent(ServiceLocator.Current.GetInstance<WizardViewModel>());
        }

        private void Remove(RingItemViewModel item)
        {
            if (item == null)
                return;

            if (!_dialogService.ShowQuestionDialog($"Remove {item.Name}"))
                return;

            Items.Remove(item);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsBusy))
                ServiceLocator.Current.GetInstance<MainViewModel>().IsBusy = IsBusy;
        }
    }
}
