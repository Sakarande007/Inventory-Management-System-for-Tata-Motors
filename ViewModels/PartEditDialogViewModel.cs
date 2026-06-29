using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaintShopIMS.Models;
using PaintShopIMS.Services;

namespace PaintShopIMS.ViewModels
{
    public partial class PartEditDialogViewModel : ObservableObject
    {
        private readonly PartService _partService;
        private readonly PartMaster _part;

        public bool IsNew => _part.PartId == 0;
        public string Title => IsNew ? "Add Part" : "Edit Part";
        
        public bool IsPartNoReadOnly => !IsNew; // Can't edit existing PartNo

        [ObservableProperty] private string _partNo = string.Empty;
        [ObservableProperty] private string _partName = string.Empty;
        [ObservableProperty] private string _description = string.Empty;
        [ObservableProperty] private string _remark = string.Empty;

        // Simple validation tracking
        [ObservableProperty] private Dictionary<string, string> _validationErrors = new();

        public PartEditDialogViewModel(PartService partService, PartMaster part)
        {
            _partService = partService;
            _part = part;

            PartNo = part.PartNo ?? string.Empty;
            PartName = part.PartName ?? string.Empty;
            Description = part.Description ?? string.Empty;
            Remark = part.Remark ?? string.Empty;
        }

        [RelayCommand]
        private async Task SaveAsync(Window window)
        {
            ValidationErrors.Clear();

            if (string.IsNullOrWhiteSpace(PartNo))
                ValidationErrors["PartNo"] = "Part No is required";
                
            if (string.IsNullOrWhiteSpace(PartName))
                ValidationErrors["PartName"] = "Part Name is required";

            if (ValidationErrors.Count > 0)
            {
                OnPropertyChanged(nameof(ValidationErrors));
                return;
            }

            _part.PartNo = PartNo;
            _part.PartName = PartName;
            _part.Description = Description;
            _part.Remark = Remark;

            var (ok, msg) = IsNew 
                ? await _partService.AddPart(_part) 
                : await _partService.UpdatePart(_part);

            if (ok)
            {
                window.DialogResult = true;
                window.Close();
            }
            else
            {
                MessageBox.Show(msg, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Cancel(Window window)
        {
            window.DialogResult = false;
            window.Close();
        }
    }
}
