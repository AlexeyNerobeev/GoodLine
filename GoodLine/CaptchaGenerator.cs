using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodLine
{
    public class CaptchaGenerator
    {
        private static Random random = new Random();

        public static Bitmap GenerateCaptcha(out string captchaText)
        {
            // Генерация случайного текста
            int length = random.Next(4, 8);
            captchaText = GenerateRandomText(length);

            // Настройка изображения
            Bitmap bitmap = new Bitmap(200, 100);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // Задание фона
                graphics.Clear(Color.White);
                DrawNoise(graphics, bitmap.Width, bitmap.Height);
                DrawLines(graphics, bitmap.Width, bitmap.Height);

                // Отрисовка текста капчи в случайных позициях
                for (int i = 0; i < captchaText.Length; i++)
                {
                    DrawRandomCharacter(graphics, captchaText[i].ToString(), i);
                }
            }

            return bitmap;
        }

        private static string GenerateRandomText(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] text = new char[length];
            for (int i = 0; i < length; i++)
            {
                text[i] = chars[random.Next(chars.Length)];
            }
            return new string(text);
        }

        private static void DrawNoise(Graphics graphics, int width, int height)
        {
            // Отрисовка точек на фоне
            for (int i = 0; i < 1000; i++)
            {
                int x = random.Next(0, width);
                int y = random.Next(0, height);
                graphics.FillEllipse(Brushes.LightGray, x, y, 2, 2);
            }
        }

        private static void DrawLines(Graphics graphics, int width, int height)
        {
            for (int i = 0; i < 5; i++)
            {
                graphics.DrawLine(Pens.LightGray, random.Next(0, width), random.Next(0, height), random.Next(0, width), random.Next(0, height));
            }
        }

        private static void DrawRandomCharacter(Graphics graphics, string character, int index)
        {
            int fontSize = random.Next(20, 40);
            Font font = new Font("Arial", fontSize, FontStyle.Bold);
            Brush brush = new SolidBrush(Color.FromArgb(random.Next(256), random.Next(256), random.Next(256)));

            float x = 10 + (index * 25) + random.Next(-5, 5);
            float y = 20 + random.Next(-10, 10);
            graphics.TranslateTransform(x, y);
            graphics.RotateTransform(random.Next(-10, 10));
            graphics.DrawString(character, font, brush, 0, 0);
            graphics.ResetTransform();
        }
    }
}
