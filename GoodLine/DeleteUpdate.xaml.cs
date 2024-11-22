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
    /// Логика взаимодействия для DeleteUpdate.xaml
    /// </summary>
    public partial class DeleteUpdate : Window
    {
        GoodLineEntities db;
        public DeleteUpdate(Materials sup)
        {
            InitializeComponent();
            db = new GoodLineEntities();
            tbId.Text = sup.MaterialID.ToString();
            tbP.Text = sup.Name;
            tbA.Text = sup.Type;
            tbE.Text = sup.Price.ToString();
            tbSupId.Text = sup.SupplierID.ToString();
            tbCount.Text = sup.Count.ToString();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
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
            Materials sup = new Materials();
            sup.MaterialID = Convert.ToInt32(tbId.Text);
            sup.Name = tbP.Text;
            sup.Type = tbA.Text;
            sup.Price = Convert.ToInt32(tbE.Text);
            sup.SupplierID = Convert.ToInt32(tbSupId.Text);
            sup.Count = Convert.ToInt32(tbCount.Text);
            db.Materials.Add(sup);
            db.SaveChanges();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
