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
    /// Логика взаимодействия для ImageWindow.xaml
    /// </summary>
    public partial class ImageWindow : Window
    {
        public ImageWindow(string imagePath)
        {
            InitializeComponent();
            LoadImage(imagePath);
        }

        private void LoadImage(string imagePath)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
            bitmap.EndInit();

            imageControl.Source = bitmap;
        }
    }
}
