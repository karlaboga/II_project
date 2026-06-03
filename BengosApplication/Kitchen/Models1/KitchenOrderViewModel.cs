using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Kitchen.Models1
{
    public class KitchenOrderViewModel
    {
        public string OrderDisplay { get; set; } = "";
        public int OrderNumber { get; set; }
        public int TableNumber { get; set; }
        public DateTime Timestamp { get; set; }
        public int TotalPrepTime { get; set; }
        public List<KitchenItem> Items { get; set; } = new();
    }
}

