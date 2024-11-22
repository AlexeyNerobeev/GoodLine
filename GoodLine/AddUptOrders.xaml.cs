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

namespace GoodLine
{
    /// <summary>
    /// Логика взаимодействия для AddUptOrders.xaml
    /// </summary>
    public partial class AddUptOrders : Window
    {
        GoodLineEntities db;
        public AddUptOrders(Orders order)
        {
            InitializeComponent();
            db = new GoodLineEntities();
            tbId.Text = order.OrderID.ToString();
            tbP.Text = order.ClientID.ToString();
            tbA.Text = order.Status;
            tbE.Text = order.Sum.ToString();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            int sUpId = Convert.ToInt32(tbId.Text);
            var selecUptId = db.Orders.Where(w => w.OrderID == sUpId).FirstOrDefault();
            selecUptId.OrderID = Convert.ToInt32(tbId.Text);
            selecUptId.ClientID = Convert.ToInt32(tbP.Text);
            selecUptId.Status = tbA.Text;
            selecUptId.Sum = Convert.ToInt32(tbE.Text);
            db.SaveChanges();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Orders sup = new Orders();
            sup.OrderID = Convert.ToInt32(tbId.Text);
            sup.ClientID = Convert.ToInt32(tbP.Text);
            sup.Status = tbA.Text;
            sup.Sum = Convert.ToInt32(tbE.Text);
            db.Orders.Add(sup);
            db.SaveChanges();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
