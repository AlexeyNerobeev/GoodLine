using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;
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
    /// Логика взаимодействия для Autorization.xaml
    /// </summary>
    public partial class Autorization : Window
    {
        GoodLineEntities db;
        public Autorization()
        {
            InitializeComponent();
            db = new GoodLineEntities();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            string inputLogin = tbL.Text;
            var select = db.Users.Where(w => w.Login == inputLogin).FirstOrDefault();
            //if (select != null)
            //{
            //    string enteredHashedPassword = HashPassword(tbP.Text, "fixedSalt");
            //    if (select.Password == enteredHashedPassword)
            //    {
            //        MainWindow mainWindow = new MainWindow();
            //        mainWindow.Show();
            //        this.Close();
            //    }
            //    else
            //    {
            //        MessageBox.Show("Неверный пароль!");
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Такого пользователя не существует!");
            //}

            if (select != null)
            {
                string enteredHashedPassword = HashPassword(tbP.Text, "fixedSalt");
                if (select.Password == enteredHashedPassword)
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                    failedAttempts = 0; // сбросить попытки
                }
                else
                {
                    failedAttempts++;
                    if (failedAttempts >= 3)
                    {
                        ShowCaptcha();
                    }
                    else
                    {
                        MessageBox.Show("Неверный пароль!");
                    }
                }
            }
            else
            {
                MessageBox.Show("Такого пользователя не существует!");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //private void Hash_Click(object sender, RoutedEventArgs e)
        //{
        //    int userid = Convert.ToInt32(tbHash.Text);
        //    var user = db.Users.Where(w => w.UserID == userid).FirstOrDefault();
        //    if (user != null)
        //    {
        //        string password = user.Password;
        //        const string salt = "fixedSalt";
        //        string hashedPassword = HashPassword(password, salt);
        //        user.Password = hashedPassword;
        //        db.SaveChanges();
        //        MessageBox.Show("Пароль успешно хэширован и сохранён.");
        //    }
        //    else
        //    {
        //        MessageBox.Show("Пользователь не найден.");
        //    }
        //}


        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = password + salt;
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2")); 
                }
                return builder.ToString();
            }
        }

        private int failedAttempts = 0;
        private string captchaText = string.Empty;
        private Bitmap captchaImage;

        private void ShowCaptcha()
        {
            captchaImage = CaptchaGenerator.GenerateCaptcha(out captchaText);
            var captchaWindow = new Window
            {
                Title = "Enter Captcha",
                Height = 150,
                Width = 200,
                Content = new System.Windows.Controls.Image { Source = BitmapToImageSource(captchaImage) }
            };

            TextBox captchaTextBox = new TextBox();
            Button submitButton = new Button { Content = "Submit" };
            submitButton.Click += (s, e) =>
            {
                if (captchaTextBox.Text == captchaText)
                {
                    captchaWindow.Close();
                    failedAttempts = 0; // сбросить попытки
                }
                else
                {
                    MessageBox.Show("Неверная капча! Попробуйте снова.");
                }
            };

            StackPanel panel = new StackPanel();
            panel.Children.Add(new System.Windows.Controls.Image { Source = BitmapToImageSource(captchaImage) });
            panel.Children.Add(captchaTextBox);
            panel.Children.Add(submitButton);
            captchaWindow.Content = panel;
            captchaWindow.ShowDialog();
        }

        private ImageSource BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                PngBitmapDecoder decoder = new PngBitmapDecoder(memory, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                return decoder.Frames[0];
            }
        }

    }
}
