using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Inventory;

public class Product : INotifyPropertyChanged
{
    private int _id;
    private string _productName = "";
    private string _category = "";
    private double _quantity; // 1. MODIFICAT AICI: din int în double
    private string _unit = "";
    private int _minStock;

    public int Id
    {
        get => _id;
        set { _id = value; OnPropertyChanged(); }
    }

    public string ProductName
    {
        get => _productName;
        set { _productName = value; OnPropertyChanged(); }
    }

    public string Category
    {
        get => _category;
        set { _category = value; OnPropertyChanged(); }
    }

    public double Quantity // 2. MODIFICAT AICI: din int în double
    {
        get => _quantity;
        set { _quantity = value; OnPropertyChanged(); }
    }

    public string Unit
    {
        get => _unit;
        set { _unit = value; OnPropertyChanged(); }
    }

    public int MinStock
    {
        get => _minStock;
        set { _minStock = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // Aici va funcționa automat (va compara corect double-ul din Quantity cu int-ul din MinStock)
    public bool IsLowStock => Quantity < MinStock;
}