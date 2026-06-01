using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Windows.Input;
namespace Inventory;
public class InventoryViewModel
{
    private readonly string connString = @"Server=tcp:server-proiect-bengos-ii.database.windows.net,1433;Initial Catalog=BengosDB;User ID=admin-proiect;Password=Bengos67;Encrypt=True;TrustServerCertificate=False;";
    public ObservableCollection<Product> Products { get; } = new();
    private string _productName = "";
    public string ProductName
    {
        get => _productName;
        set => _productName = value ?? "";
    }
    private string _category = "";
    public string Category
    {
        get => _category;
        set => _category = value ?? "";
    }
    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set => _quantity = value;
    }
    private string _unit = "Kilograms";
    public string Unit
    {
        get => _unit;
        set => _unit = value ?? "Kilograms";
    }
    private int _minStock = 10;
    public int MinStock
    {
        get => _minStock;
        set => _minStock = value;
    }
    private Product? _selectedProduct;
    public Product? SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            _selectedProduct = value;
            if (value != null)
            {
                ProductName = value.ProductName;
                Category = value.Category;
                Quantity = value.Quantity;
                Unit = value.Unit;
                MinStock = value.MinStock;
            }
        }
    }
    private string _filterCategory = "All";
    public string FilterCategory
    {
        get => _filterCategory;
        set
        {
            _filterCategory = value ?? "All";
            LoadProducts();
        }
    }
    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ClearFilterCommand { get; }
    public InventoryViewModel()
    {
        AddCommand = new RelayCommand(AddProduct);
        EditCommand = new RelayCommand(EditProduct, () => SelectedProduct != null);
        DeleteCommand = new RelayCommand(DeleteProduct, () => SelectedProduct != null);
        ClearFilterCommand = new RelayCommand(() => { FilterCategory = "All"; });
        LoadProducts();
    }
    public void LoadProducts()
    {
        Products.Clear();
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            string query = "SELECT Id, Name, Category, Quantity, Unit, MinStock FROM Produses";
            if (FilterCategory != "All")
            {
                query += " WHERE Category = @cat";
            }
            query += " ORDER BY Name";
            using var cmd = new SqlCommand(query, conn);
            if (FilterCategory != "All")
                cmd.Parameters.AddWithValue("@cat", FilterCategory);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Products.Add(new Product
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    ProductName = reader["Name"]?.ToString() ?? "",
                    Category = reader["Category"]?.ToString() ?? "",
                    Quantity = Convert.ToInt32(reader["Quantity"]),
                    Unit = reader["Unit"]?.ToString() ?? "",
                    MinStock = Convert.ToInt32(reader["MinStock"])
                });
            }
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show("Database Load Error: " + ex.Message);
        }
    }
    private void AddProduct()
    {
        if (string.IsNullOrWhiteSpace(ProductName)) return;
        if (ProductName.Any(char.IsDigit))
        {
            System.Windows.MessageBox.Show("Product name must contain only letters.", "Validation Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand(
                "INSERT INTO Produses (Name, Category, Quantity, Unit, MinStock) VALUES (@name, @cat, @qty, @unit, @min)",
                conn);
            cmd.Parameters.AddWithValue("@name", ProductName);
            cmd.Parameters.AddWithValue("@cat", Category);
            cmd.Parameters.AddWithValue("@qty", Quantity);
            cmd.Parameters.AddWithValue("@unit", Unit);
            cmd.Parameters.AddWithValue("@min", MinStock);
            cmd.ExecuteNonQuery();
            ClearForm();
            LoadProducts();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show("Error adding product: " + ex.Message);
        }
    }
    private void EditProduct()
    {
        if (SelectedProduct == null || string.IsNullOrWhiteSpace(ProductName)) return;
        if (ProductName.Any(char.IsDigit))
        {
            System.Windows.MessageBox.Show("Product name must contain only letters.", "Validation Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand(
                "UPDATE Produses SET Name=@name, Category=@cat, Quantity=@qty, Unit=@unit, MinStock=@min WHERE Id=@id",
                conn);
            cmd.Parameters.AddWithValue("@id", SelectedProduct.Id);
            cmd.Parameters.AddWithValue("@name", ProductName);
            cmd.Parameters.AddWithValue("@cat", Category);
            cmd.Parameters.AddWithValue("@qty", Quantity);
            cmd.Parameters.AddWithValue("@unit", Unit);
            cmd.Parameters.AddWithValue("@min", MinStock);
            cmd.ExecuteNonQuery();
            ClearForm();
            LoadProducts();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show("Error updating product: " + ex.Message);
        }
    }
    private void DeleteProduct()
    {
        if (SelectedProduct == null) return;
        if (System.Windows.MessageBox.Show($"Delete '{SelectedProduct.ProductName}'?", "Confirm",
                System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question) !=
            System.Windows.MessageBoxResult.Yes)
            return;
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand("DELETE FROM Produses WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", SelectedProduct.Id);
            cmd.ExecuteNonQuery();
            ClearForm();
            LoadProducts();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show("Error deleting product: " + ex.Message);
        }
    }
    private void ClearForm()
    {
        ProductName = "";
        Category = "";
        Quantity = 0;
        Unit = "Kilograms";
        MinStock = 10;
        SelectedProduct = null;
    }
}