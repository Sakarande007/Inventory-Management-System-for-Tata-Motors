using CommunityToolkit.Mvvm.ComponentModel;

namespace PaintShopIMS.Services;

public interface INavigationService
{
    ObservableObject CurrentView { get; }
    void NavigateTo<TViewModel>() where TViewModel : ObservableObject;
}
