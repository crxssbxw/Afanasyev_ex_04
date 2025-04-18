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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Pizza.Database;
using Pizza.Windows;
using System.Data.Entity;

namespace Pizza
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private DatabaseContext context { get; set; } = new DatabaseContext();
        public MainWindow()
        {
            context.PizzaAssortiments.Load();
            context.Pizzas.Load();
            context.PizzaSizes.Load();
            context.Clients.Load();
            InitializeComponent();

            Orders = context.Orders.ToList();
            DataContext = this;
        }

        private List<Order> orders;
        public List<Order> Orders
        {
            get => orders;
            set
            {
                orders = value;
                OnPropertyChanged(nameof(Orders));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private void NewOrderBT_Click(object sender, RoutedEventArgs e)
        {
            AddEditOrder addEditOrder = new AddEditOrder();
            if (addEditOrder.ShowDialog() == true)
            {
                MessageBox.Show("Заказ создан", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                Refresh();
            }
        }

        public void Refresh()
        {
            context = new DatabaseContext();
            Orders = context.Orders.ToList();
        }
    }
}
