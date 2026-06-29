using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaintShopIMS.Models;
using PaintShopIMS.Repositories;
using PaintShopIMS.Services;
using PaintShopIMS.Views.Parts;

namespace PaintShopIMS.ViewModels
{
    public partial class PartMasterViewModel : ObservableObject
    {
        private readonly PartRepository _partRepo;
        private readonly PartService _partService;

        private List<PartMaster> _allParts = new();

        [ObservableProperty] private ObservableCollection<PartMaster> _filteredParts = new();
        [ObservableProperty] private PartMaster? _selectedPart;
        
        [ObservableProperty] private string _searchText = string.Empty;
        
        public int PartsCount => FilteredParts.Count;

        public PartMasterViewModel(PartRepository partRepo, PartService partService)
        {
            _partRepo = partRepo;
            _partService = partService;
            LoadDataAsync();
        }

        public async void LoadDataAsync()
        {
            var parts = await _partRepo.GetAllAsync();
            _allParts = parts.ToList();
            FilterList();
        }

        partial void OnSearchTextChanged(string value)
        {
            FilterList();
        }

        private void FilterList()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredParts = new ObservableCollection<PartMaster>(_allParts);
            }
            else
            {
                var lower = SearchText.ToLower();
                var filtered = _allParts.Where(p => 
                    p.PartNo.ToLower().Contains(lower) || 
                    p.PartName.ToLower().Contains(lower) || 
                    (p.Description != null && p.Description.ToLower().Contains(lower))
                ).ToList();
                FilteredParts = new ObservableCollection<PartMaster>(filtered);
            }
            OnPropertyChanged(nameof(PartsCount));
        }

        [RelayCommand]
        private void AddPart()
        {
            // Create a new empty part
            var vm = new PartEditDialogViewModel(_partService, new PartMaster());
            var dialog = new PartEditDialog(vm);
            
            if (dialog.ShowDialog() == true)
            {
                LoadDataAsync();
            }
        }

        [RelayCommand]
        private void EditPart()
        {
            if (SelectedPart == null) return;

            // Clone selected part for editing
            var editClone = new PartMaster
            {
                PartId = SelectedPart.PartId,
                PartNo = SelectedPart.PartNo,
                PartName = SelectedPart.PartName,
                Description = SelectedPart.Description,
                Remark = SelectedPart.Remark
            };

            var vm = new PartEditDialogViewModel(_partService, editClone);
            var dialog = new PartEditDialog(vm);

            if (dialog.ShowDialog() == true)
            {
                LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task DeletePartAsync()
        {
            if (SelectedPart == null) return;

            var result = MessageBox.Show($"Are you sure you want to delete {SelectedPart.PartNo}?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                
            if (result == MessageBoxResult.Yes)
            {
                var response = await _partService.DeletePart(SelectedPart.PartId);
                if (response.ok)
                {
                    LoadDataAsync();
                }
                else
                {
                    MessageBox.Show(response.msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
