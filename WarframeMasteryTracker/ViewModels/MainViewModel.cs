using System.Collections.ObjectModel;
using System.ComponentModel;
using WarframeMasteryTracker.Data;
using WarframeMasteryTracker.Models;

namespace WarframeMasteryTracker.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly ItemsRepository _repository;

    public ObservableCollection<ItemViewModel> Items { get; }
        = new ObservableCollection<ItemViewModel>();

    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText == value) return;
            _searchText = value;
            OnPropertyChanged();
            ReloadItems();
        }
    }
    // IS MASTERED

    // CATEGORY
    private string? _selectedCategory;
    public string? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (_selectedCategory == value) return;
            _selectedCategory = value;
            OnPropertyChanged();
            LoadTypes();
            ReloadItems();
        }
    }
    public ObservableCollection<string> AvailableCategories { get; }
        = new ObservableCollection<string>();

    // TYPE
    private string? _selectedType;
    public string? SelectedType
    {
        get => _selectedType;
        set
        {
            if (_selectedType == value) return;
            _selectedType = value;
            OnPropertyChanged();
            ReloadItems();
        }
    }

    public ObservableCollection<string> AvailableTypes { get; }
        = new ObservableCollection<string>();


    //-------//
    public MainViewModel()
    {
        _repository = new ItemsRepository();

        LoadTypes();
        LoadCategories();
        ReloadItems();
    }
    //-------//

    private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not ItemViewModel vm)
            return;

        if (e.PropertyName == nameof(ItemViewModel.IsMastered))
        {
            _repository.SetMastered(vm.Id, vm.IsMastered);
        }
    }

    private void ReloadItems()
    {
        Items.Clear();

        var items = _repository.GetItems(
            searchText: SearchText,
            typeFilter: SelectedType,
            categoryFilter: SelectedCategory
        );

        foreach (var item in items)
        {
            var vm = new ItemViewModel(item);
            vm.PropertyChanged += Item_PropertyChanged;
            Items.Add(vm);
        }
    }

    private void LoadTypes()
    {
        AvailableTypes.Clear();
        AvailableTypes.Add("All");

        var types = _repository.GetAvailableTypes(
            categoryFilter: SelectedCategory
        );

        foreach (var type in types)
            AvailableTypes.Add(type);

        SelectedType = AvailableTypes.FirstOrDefault();
    }

    private void LoadCategories()
    {
        AvailableCategories.Clear();
        AvailableCategories.Add("All");

        var types = _repository.GetAvailableCategories();

        foreach (var type in types)
            AvailableCategories.Add(type);

        SelectedCategory = AvailableCategories.FirstOrDefault();

    }
}
