using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
using ExcelDataReader;
using Microsoft.Win32;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace GoodLine
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GoodLineEntities db; 
        public MainWindow()
        {
            InitializeComponent();
            db = new GoodLineEntities();
            dgMaterials.ItemsSource = db.Materials.ToList();
            dgOrders.ItemsSource = db.Orders.ToList();

            var ordersClients = from orders in db.Orders
                                join clients in db.Clients on orders.OrderID equals clients.ClientID into Group
                                from clients in Group.DefaultIfEmpty()
                                select new
                                {
                                    OrderID = orders.OrderID,
                                    Status = orders.Status,
                                    Sum = orders.Sum,
                                    ClientID = clients.ClientID,
                                    Name = clients.Name,
                                    Surname = clients.Surname,
                                    Email = clients.Email,
                                    Phone = clients.Phone,
                                    Adress = clients.Adress
                                };
            dgOrdersClients.ItemsSource = ordersClients.ToList();
        }



        public List<Suppliers> Sup { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Materials sup = new Materials();
            sup.MaterialID = Convert.ToInt32(tbId.Text);
            sup.Name = tbP.Text;
            sup.Type = tbA.Text;
            sup.Price = Convert.ToInt32(tbE.Text);
            sup.SupplierID = Convert.ToInt32(tbSupId.Text);
            sup.Count = Convert.ToInt32(tbCount.Text);
            db.Materials.Add(sup);
            db.SaveChanges();
            dgMaterials.ItemsSource = db.Materials.ToList();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int sDId = Convert.ToInt32(tbId.Text);
            var selectDId = db.Materials.Where(w => w.MaterialID == sDId).FirstOrDefault();
            db.Materials.Remove(selectDId);
            db.SaveChanges();
            dgMaterials.ItemsSource = db.Materials.ToList();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            int sUpId = Convert.ToInt32(tbId.Text);
            var selecUptId = db.Materials.Where(w => w.MaterialID == sUpId).FirstOrDefault();
            selecUptId.MaterialID = Convert.ToInt32(tbId.Text);
            selecUptId.Name = tbP.Text;
            selecUptId.Type = tbA.Text;
            selecUptId.Price = Convert.ToInt32(tbE.Text);
            selecUptId.SupplierID = Convert.ToInt32(tbSupId.Text);
            selecUptId.Count = Convert.ToInt32(tbCount.Text);
            db.SaveChanges(); 
            dgMaterials.ItemsSource = db.Materials.ToList();
        }

        private void DGMouse_Click(object sender, MouseButtonEventArgs e)
        {
            if(dgMaterials.SelectedItem != null)
            {
                Materials materials = (Materials)dgMaterials.SelectedItem;
                if(materials.MaterialID != 0)
                {
                    DeleteUpdate deleteUpdate = new DeleteUpdate(materials);
                    deleteUpdate.Show();
                    this.Close();
                }
            }
        }

        private void cbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void tbFilterValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            string selectedField = ((ComboBoxItem)cbFilter.SelectedItem)?.Content.ToString() ?? "Название"; 
            string filterValue = tbFilterValue.Text.ToLower();

            var filteredMaterials = db.Materials.AsQueryable();

            if (!string.IsNullOrEmpty(filterValue))
            {
                switch (selectedField)
                {
                    case "Название":
                        filteredMaterials = filteredMaterials.Where(s => s.Name.ToLower().Contains(filterValue));
                        break;
                    case "Тип":
                        filteredMaterials = filteredMaterials.Where(s => s.Type.ToLower().Contains(filterValue));
                        break;
                    case "Цена":
                        filteredMaterials = filteredMaterials.Where(s => s.Price.ToString().ToLower().Contains(filterValue));
                        break;
                    case "Id Поставщика":
                        filteredMaterials = filteredMaterials.Where(s => s.SupplierID.ToString().ToLower().Contains(filterValue));
                        break;
                    case "Количество":
                        filteredMaterials = filteredMaterials.Where(s => s.Count.ToString().ToLower().Contains(filterValue));
                        break;
                }
            }

            dgMaterials.ItemsSource = filteredMaterials.ToList();
        }

        private void PDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                PrintDialog pD = new PrintDialog();
                if (pD.ShowDialog() == true)
                {
                    pD.PrintVisual(dgMaterials, "Проект");
                }
            }
            finally
            {
                this.IsEnabled = true;
            }
        }

        private void DK_dgOrders(object sender, MouseButtonEventArgs e)
        {
            if (dgOrders.SelectedItem != null)
            {
                Orders orders = (Orders)dgOrders.SelectedItem;
                if (orders.OrderID != 0)
                {
                    AddUptOrders addUptOrders = new AddUptOrders(orders);
                    addUptOrders.Show();
                    this.Close();
                }
            }
        }

        private void DelOrder_Click(object sender, RoutedEventArgs e)
        {
            int sDId = Convert.ToInt32(tbDelOrder.Text);
            var selectDId = db.Orders.Where(w => w.OrderID == sDId).FirstOrDefault();
            db.Orders.Remove(selectDId);
            db.SaveChanges();
            dgOrders.ItemsSource = db.Orders.ToList();
        }

        private void cbFilterOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OrdersFilter();
        }

        private void tbFilterOrders_TextChanged(object sender, TextChangedEventArgs e)
        {
            OrdersFilter();
        }

        private void OrdersFilter()
        {
            string selectedField = ((ComboBoxItem)cbFilterOrders.SelectedItem)?.Content.ToString() ?? "Статус";
            string filterValue = tbFilterOrders.Text.ToLower();

            var filteredMaterials = db.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(filterValue))
            {
                switch (selectedField)
                {
                    case "Статус":
                        filteredMaterials = filteredMaterials.Where(s => s.Status.ToLower().Contains(filterValue));
                        break;
                    case "Id Клиента":
                        filteredMaterials = filteredMaterials.Where(s => s.ClientID.ToString().ToLower().Contains(filterValue));
                        break;
                    case "Сумма":
                        filteredMaterials = filteredMaterials.Where(s => s.Sum.ToString().ToLower().Contains(filterValue));
                        break;
                }
            }

            dgOrders.ItemsSource = filteredMaterials.ToList();
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                PrintDialog pD = new PrintDialog();
                if (pD.ShowDialog() == true)
                {
                    pD.PrintVisual(dgOrdersClients, "Проект");
                }
            }
            finally
            {
                this.IsEnabled = true;
            }
        }
        private void FilterOrdersClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OrdersClientsFilter();
        }

        private void tbFilterOrdersClients_TextChanged(object sender, TextChangedEventArgs e)
        {
            OrdersClientsFilter();
        }

        private void OrdersClientsFilter()
        {
            string selectedField = ((ComboBoxItem)cbFilterOrdersClients.SelectedItem)?.Content.ToString() ?? "Id Заказа";
            string filterValue = tbFilterOrdersClients.Text.ToLower();

            var filteredMaterials = from orders in db.Orders
                                    join clients in db.Clients on orders.OrderID equals clients.ClientID into Group
                                    from clients in Group.DefaultIfEmpty()
                                    select new
                                    {
                                        OrderID = orders.OrderID,
                                        Status = orders.Status,
                                        Sum = orders.Sum,
                                        ClientID = clients.ClientID,
                                        Name = clients.Name,
                                        Surname = clients.Surname,
                                        Email = clients.Email,
                                        Phone = clients.Phone,
                                        Adress = clients.Adress
                                    };

            if (!string.IsNullOrEmpty(filterValue))
            {
                switch (selectedField)
                {
                    case "Id Заказа":
                        filteredMaterials = filteredMaterials.Where(s => s.OrderID.ToString().ToLower().Contains(filterValue));
                        break;
                    case "Статус":
                        filteredMaterials = filteredMaterials.Where(s => s.Status.ToLower().Contains(filterValue));
                        break;
                    case "Сумма":
                        filteredMaterials = filteredMaterials.Where(s => s.Sum.ToString().ToLower().Contains(filterValue));
                        break;
                    case "Id Клиента":
                        filteredMaterials = filteredMaterials.Where(s => s.ClientID.ToString().ToLower().Contains(filterValue));
                        break;
                    case "Имя":
                        filteredMaterials = filteredMaterials.Where(s => s.Name.ToLower().Contains(filterValue));
                        break;
                    case "Фамилия":
                        filteredMaterials = filteredMaterials.Where(s => s.Surname.ToLower().Contains(filterValue));
                        break;
                    case "Почта":
                        filteredMaterials = filteredMaterials.Where(s => s.Email.ToLower().Contains(filterValue));
                        break;
                    case "Телефон":
                        filteredMaterials = filteredMaterials.Where(s => s.Phone.ToLower().Contains(filterValue));
                        break;
                    case "Адрес":
                        filteredMaterials = filteredMaterials.Where(s => s.Adress.ToLower().Contains(filterValue));
                        break;
                }
            }

            dgOrdersClients.ItemsSource = filteredMaterials.ToList();
        }
    }
}
