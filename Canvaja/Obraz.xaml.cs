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

namespace Canvaja
{
    /// <summary>
    /// Logika interakcji dla klasy Obraz.xaml
    /// </summary>
    public partial class Obraz : Window
    {
        public Obraz()
        {
            InitializeComponent();
        }


        private void ApplyLinearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage originalImage = new BitmapImage(new Uri(@"../1663268170627.jpg", UriKind.Relative));

            WriteableBitmap writeableBitmap = new WriteableBitmap(originalImage);

            double[,] filterMatrix = {
                { 1, 1, 1 },
                { 1, 1, 1 },
                { 1, 1, 1 }
            };

            ApplyLinearFilter(writeableBitmap, filterMatrix);

            ProcessedImage.Source = writeableBitmap;
        }

        private void ApplyLinearFilter(WriteableBitmap image, double[,] filter)
        {
            int width = image.PixelWidth;
            int height = image.PixelHeight;

            int stride = width * 4; // Każdy piksel zajmuje 4 bajty (ARGB)

            byte[] pixels = new byte[height * stride];
            image.CopyPixels(pixels, stride, 0);

            byte[] resultPixels = new byte[height * stride];

            int filterSize = filter.GetLength(0);
            int filterOffset = filterSize / 2;

            for (int y = filterOffset; y < height - filterOffset; y++)
            {
                for (int x = filterOffset; x < width - filterOffset; x++)
                {
                    double red = 0, green = 0, blue = 0;

                    for (int i = 0; i < filterSize; i++)
                    {
                        for (int j = 0; j < filterSize; j++)
                        {
                            int pixelIndex = ((y + i - filterOffset) * stride) + ((x + j - filterOffset) * 4);

                            red += pixels[pixelIndex + 2] * filter[i, j];
                            green += pixels[pixelIndex + 1] * filter[i, j];
                            blue += pixels[pixelIndex] * filter[i, j];
                        }
                    }

                    int resultIndex = (y * stride) + (x * 4);
                    resultPixels[resultIndex + 2] = Clamp((int)red);
                    resultPixels[resultIndex + 1] = Clamp((int)green);
                    resultPixels[resultIndex] = Clamp((int)blue);
                    resultPixels[resultIndex + 3] = 255; // Alpha

                }
            }

            image.WritePixels(new Int32Rect(0, 0, width, height), resultPixels, stride, 0);
        }

        private byte Clamp(int value)
        {
            return (byte)(value < 0 ? 0 : (value > 255 ? 255 : value));
        }
    }
}
