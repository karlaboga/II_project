using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace inventoryWPF
{
    public class Product : INotifyPropertyChanged
    {
        private int _id;
        private string _productName = string.Empty;
        private string _category = string.Empty;
        private int _quantity;
        private string _unit = string.Empty;
        private int _minStock;

        public int ID { get => _id; set { _id = value; OnPropertyChanged(); } }
        public string ProductName { get => _productName; set { _productName = value; OnPropertyChanged(); } }
        public string Category { get => _category; set { _category = value; OnPropertyChanged(); } }
        public int Quantity { get => _quantity; set { _quantity = value; OnPropertyChanged(); } }
        public string Unit { get => _unit; set { _unit = value; OnPropertyChanged(); } }
        public int Min_Stock { get => _minStock; set { _minStock = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}