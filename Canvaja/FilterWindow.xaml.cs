using Emgu.CV.CvEnum;
using Emgu.CV;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Interop;
using Emgu.CV.Structure;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Media3D;

namespace Canvaja
{
    /// <summary>
    /// Logika interakcji dla klasy FilterWindow.xaml
    /// </summary>
    public partial class FilterWindow : Window
    {
        public FilterWindow()
        {
            InitializeComponent();

        }
        Uri uri;
        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "png Files(*.png)|*.png|jpg files(*.jpg)|*.jpg";
            if (ofd.ShowDialog() == true)
            {
                string path = ofd.FileName;
                DisplayImage(path);
            }

        }
        private void DisplayImage(string path)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path);
                bitmap.EndInit();
                Image.Source = bitmap;
                uri = new Uri(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}");
            }
        }
        private void Transform_Click(object sender, RoutedEventArgs e)
        {
            Mat image = new Mat();
            if (uri != null)
            {
                image = CvInvoke.Imread(uri.LocalPath, ImreadModes.Grayscale);
            }
            if (image.IsEmpty)
            {
                Console.WriteLine("Failed to load image.");
                return;
            }


            Mat sobelImage = new Mat();
            Emgu.CV.CvInvoke.Sobel(image, sobelImage, DepthType.Cv8U, 1, 1);


            Bitmap bitmapa = sobelImage.ToBitmap();

            ImageSource imageSource = ToImageSource(bitmapa);

            Transformed.Source = imageSource;
        }
        static ImageSource ToImageSource(Bitmap bitmap)
        {
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return bitmapSource;
        }

    }
}