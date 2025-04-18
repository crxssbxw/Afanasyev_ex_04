using Pizza.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace Pizza.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddEditOrder.xaml
    /// </summary>
    public partial class AddEditOrder : Window, INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private Order order;
        public Order Order
        {
            get => order;
            set
            {
                order = value;
                OnPropertyChanged(nameof(Order));
            }
        }

        private Client client;
        public Client Client
        {
            get => client;
            set
            {
                client = value;
                OnPropertyChanged(nameof(Client));
            }
        }

        private DatabaseContext context { get; set; } = new DatabaseContext();
        public List<PizzaAssortiment> PizzaAssortiments { get; set; }

        public AddEditOrder()
        {
            context.PizzaAssortiments.Load();
            context.Pizzas.Load();
            context.PizzaSizes.Load();
            context.Clients.Load();
            InitializeComponent();
            DataContext = this;

            Order = new Order()
            {
                Client = Client ?? (Client = new Client()
                {
                    SecondName = "Фамилия",
                    FirstName = "Имя",
                    MiddleName = "Отчество"
                }),
                AdressStreet = "ул. ",
                AdressApartment = "кв. ",
                PizzaCount = 1
            };
            PizzaAssortiments = context.PizzaAssortiments.ToList();
        }

        public PizzaAssortiment PizzaAssortiment 
        { 
            get => Order.PizzaAssortiment; 
            set
            {
                Order.PizzaAssortiment = value;
                OnPropertyChanged(nameof(PizzaAssortiment));
                OnPropertyChanged(nameof(Order));
                OnPropertyChanged(nameof(OrderCost));
                OnPropertyChanged(nameof(Discount));
                OnPropertyChanged(nameof(CountFinalCost));
            } 
        }

        public int PizzaCount
        {
            get => Order.PizzaCount;
            set
            {
                Order.PizzaCount = value;
                OnPropertyChanged(nameof(Order));
                OnPropertyChanged(nameof(OrderCost));
                OnPropertyChanged(nameof(Discount));
                OnPropertyChanged(nameof(CountFinalCost));
            }
        }

        public decimal OrderCost
        {
            get => Order.OrderCost;
        }

        public int Discount
        {
            get => Order.Discount;
        }

        public decimal CountFinalCost
        {
            get => Order.CountFinalCost;
        }

        private void AcceptBT_Click(object sender, RoutedEventArgs e)
        {
            if (!Validation())
                return;

            Order.Client = Client;
            Order.FinalCost = Order.CountFinalCost;

            try
            {
                context.Orders.Add(Order);
                context.SaveChanges();
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelBT_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        public bool Validation()
        {
            string message = "";

            if (string.IsNullOrEmpty(Order.Client.SecondName))
                message += "Введите фамилию\n";
            if (string.IsNullOrEmpty(Order.Client.FirstName))
                message += "Введите имя\n";
            if (string.IsNullOrEmpty(Order.Client.Phone))
                message += "Введите телефон\n";
            else if (Order.Client.Phone.Length < 11)
                message += "Номер телефона не меньше 11 цифр\n";

            if (string.IsNullOrEmpty(Order.AdressStreet))
                message += "Введите улицу\n";
            if (string.IsNullOrEmpty(Order.AdressHouse.ToString()))
                message += "Введите дом\n";
            if (string.IsNullOrEmpty(Order.AdressApartment))
                message += "Введите квартиру / офис\n";

            if (Order.PizzaAssortiment == null)
                message += "Выберите пиццу из списка\n";

            if (Order.PizzaCount <= 0 || string.IsNullOrEmpty(Order.PizzaCount.ToString()))
                message += "Введите количество пицц, не меньше 1\n";

            if (string.IsNullOrEmpty(message))
                return true;

            MessageBox.Show(message, "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
    }
}
