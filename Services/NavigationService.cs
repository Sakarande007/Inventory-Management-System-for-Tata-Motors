using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PaintShopIMS.Services;

public class NavigationService : ObservableObject, INavigationService
{
    private readonly Func<Type, ObservableObject> _viewModelFactory;
    private ObservableObject _currentView = null!;

    public ObservableObject CurrentView
    {
        get => _currentView;
        private set => SetProperty(ref _currentView, value);
    }

    public NavigationService(Func<Type, ObservableObject> viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
    }

    public void NavigateTo<TViewModel>() where TViewModel : ObservableObject
    {
        CurrentView = _viewModelFactory(typeof(TViewModel));
    }
}
