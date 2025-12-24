using System.ComponentModel;
using System.Runtime.CompilerServices;
using WarframeMasteryTracker.Models;

namespace WarframeMasteryTracker.ViewModels;

public class ItemViewModel : ViewModelBase
{
    private readonly Item _item;

    public ItemViewModel(Item item)
    {
        _item = item;
        _isMastered = item.IsMastered;
    }

    public int Id => _item.Id;
    public string Name => _item.Name;
    public string Category => _item.Category;
    public string Type => _item.Type;
    public int? MasteryReq => _item.MasteryReq;

    private bool _isMastered;
    public bool IsMastered
    {
        get => _isMastered;
        set
        {
            if (_isMastered == value) return;
            _isMastered = value;
            OnPropertyChanged();

            _item.IsMastered = value;
        }
    }

    public Item Model => _item;
}
