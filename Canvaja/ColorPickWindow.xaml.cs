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
using static Canvaja.MainWindow;

namespace Canvaja
{
    /// <summary>
    /// Logika interakcji dla klasy ColorPickWindow.xaml
    /// </summary>
    public partial class ColorPickWindow : Window
    {
        public Color tempcolor = new Color(); 
        public ColorPickWindow()
        {
            InitializeComponent();
        }
        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            if (byte.TryParse(RedTextBox.Text, out byte red) &&
                byte.TryParse(GreenTextBox.Text, out byte green) &&
                byte.TryParse(BlueTextBox.Text, out byte blue))
            {
                tempcolor = Color.FromRgb(red, green, blue);
                OnColorSelected(tempcolor);
                ConvertRGBtoHSV(red, green, blue);
            }
            else
            {
                MessageBox.Show("Please enter valid integer values for Red, Green, and Blue components.");
            }
        }

        private void ConvertRGBtoHSV(int red, int green, int blue)
        {
            // Normalize RGB values to the range [0, 1]
            double normalizedRed = red / 255.0;
            double normalizedGreen = green / 255.0;
            double normalizedBlue = blue / 255.0;

            double Cmax = Math.Max(normalizedRed, Math.Max(normalizedGreen, normalizedBlue));
            double Cmin = Math.Min(normalizedRed, Math.Min(normalizedGreen, normalizedBlue));

            double hue = 0;
            double saturation = (Cmax == 0) ? 0 : 1 - (Cmin / Cmax);
            double value = Cmax;
            double delta = Cmax - Cmin;


            if (delta != 0)
            {
                if (Cmax == normalizedRed)
                    hue = ((normalizedGreen - normalizedBlue) / delta) % 6;
                else if (Cmax == normalizedGreen)
                    hue = 2 + (normalizedBlue - normalizedRed) / delta;
                else
                    hue = 4 + (normalizedRed - normalizedGreen) / delta;

                hue *= 60;

                if (hue < 0)
                    hue += 360;
            }

            HueTextBox.Text = Math.Round(hue, 2).ToString();
            SaturationTextBox.Text = Math.Round(saturation * 100, 2).ToString();
            ValueTextBox.Text = Math.Round(value * 100, 2).ToString();


            
        }
        public event EventHandler<ColorChangedEventArgs> ColorSelected;

        private void OnColorSelected(Color selectedColor)
        {
            ColorSelected?.Invoke(this, new ColorChangedEventArgs(selectedColor));
        }

    }
}
