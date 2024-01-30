using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace Canvaja
{


    public partial class MainWindow : Window
    {
        public event EventHandler<ColorChangedEventArgs> ColorChanged;

        Point currentPoint = new Point();
        int drawStyle = 0;
        bool isLineStart = false;
        bool isCurvedLineStarted = false;
        List<Line> allLines = new List<Line>();
        List<Ellipse> editedPoints = new List<Ellipse>();
        Ellipse editedPoint = new Ellipse();
        Line clickedLine = new Line();
        bool lineInEditing = false;
        bool edgeInEditing = false;
        bool startInEditing = false;

        Color selectedColor = Color.FromRgb(255, 0, 0);

        public Color SelectedColor
        {
            get
            {
                return selectedColor;
            }
            set
            {
                selectedColor = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            ColorChanged += MainWindow_ColorChanged;

        }

        private void ObszarRoboczy_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && drawStyle == 1)
            {
                Line line = new Line();

                line.Stroke = new SolidColorBrush(selectedColor);

                line.X1 = currentPoint.X;
                line.Y1 = currentPoint.Y;
                line.X2 = e.GetPosition(this).X;
                line.Y2 = e.GetPosition(this).Y;

                currentPoint = e.GetPosition(this);

                ObszarRoboczy.Children.Add(line);


            }

            if (drawStyle == 12 && e.LeftButton == MouseButtonState.Pressed)
            {
                var clickedElement = e.Source as FrameworkElement;

                if (clickedElement != null)
                {
                    if (ObszarRoboczy.Children.Contains(clickedElement))
                    {
                        ObszarRoboczy.Children.Remove(clickedElement);
                    }
                }
            }


        }

        private void ObszarRoboczy_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed && drawStyle == 3)
            {
                if (isLineStart)
                {
                    Line line = new Line();

                    line.Stroke = new SolidColorBrush(selectedColor);

                    line.X1 = currentPoint.X;
                    line.Y1 = currentPoint.Y;
                    line.X2 = e.GetPosition(this).X;
                    line.Y2 = e.GetPosition(this).Y;

                    ObszarRoboczy.Children.Add(line);
                    allLines.Add(line);

                    isLineStart = false;
                }
                else
                {
                    isLineStart = true;
                }
            }

            else if (drawStyle == 8)
            {
                if (isCurvedLineStarted)
                {
                    Line line = new Line();

                    line.Stroke = new SolidColorBrush(selectedColor);

                    line.X1 = currentPoint.X;
                    line.Y1 = currentPoint.Y;
                    line.X2 = e.GetPosition(this).X;
                    line.Y2 = e.GetPosition(this).Y;

                    ObszarRoboczy.Children.Add(line);
                    allLines.Add(line);
                }
                else
                {
                    isCurvedLineStarted = true;
                }
            }

            if (e.ButtonState == MouseButtonState.Pressed)
            {
                currentPoint = e.GetPosition(this);
            }

            if (drawStyle == 4)
            {
                CheckEditSegmentOnMouseDownEvents();
            }

            if (drawStyle == 12)
            {
                var clickedElement = e.Source as FrameworkElement;

                if (clickedElement != null)
                {
                    if (ObszarRoboczy.Children.Contains(clickedElement))
                    {
                        ObszarRoboczy.Children.Remove(clickedElement);
                    }
                }
            }



        }

        private void Przycisk_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Program do tworzenia grafiki");
        }

        private void btnDraw_Click(object sender, RoutedEventArgs e)
        {
            // Rysowanie dowolne
            drawStyle = 1;

            ClearEditSegmentData();
        }

        private void btnDrawPoints_Click(object sender, RoutedEventArgs e)
        {
            drawStyle = 2;

            ClearEditSegmentData();
        }

        private void ObszarRoboczy_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (drawStyle == 2)
            {
                Ellipse ellipse = DrawPoint(6, e.GetPosition(this).X, e.GetPosition(this).Y, Colors.Magenta);
            }

            else if (drawStyle == 5)
            {
                Polygon polygon = new Polygon();

                double mouseX = e.GetPosition(this).X, mouseY = e.GetPosition(this).Y;

                double polygonSize = 20;

                Point point0 = new Point(mouseX - polygonSize, mouseY + 2 * polygonSize);
                Point point1 = new Point(mouseX + polygonSize, mouseY + 2 * polygonSize);
                Point point2 = new Point(mouseX + 2 * polygonSize, mouseY);
                Point point3 = new Point(mouseX + polygonSize, mouseY - 2 * polygonSize);
                Point point4 = new Point(mouseX - polygonSize, mouseY - 2 * polygonSize);
                Point point5 = new Point(mouseX - 2 * polygonSize, mouseY);

                PointCollection polygonPoints = new PointCollection();
                polygonPoints.Add(point0);
                polygonPoints.Add(point1);
                polygonPoints.Add(point2);
                polygonPoints.Add(point3);
                polygonPoints.Add(point4);
                polygonPoints.Add(point5);

                polygon.Points = polygonPoints;
                Brush color = new SolidColorBrush(selectedColor);
                polygon.Stroke = color;

                ObszarRoboczy.Children.Add(polygon);
            }

            else if (drawStyle == 6)
            {
                Ellipse newEllipse = new Ellipse();
                newEllipse.Width = 40;
                newEllipse.Height = 40;
                newEllipse.Stroke = new SolidColorBrush(selectedColor);
                newEllipse.Fill = new SolidColorBrush(Colors.White);
                Canvas.SetLeft(newEllipse, e.GetPosition(this).X - newEllipse.Width / 2);
                Canvas.SetTop(newEllipse, e.GetPosition(this).Y - newEllipse.Height / 2);


                ObszarRoboczy.Children.Add(newEllipse);
            }

            else if (drawStyle == 7)
            {
                Rectangle newRectangle = new Rectangle();
                newRectangle.Width = 80;
                newRectangle.Height = 40;
                newRectangle.Stroke = new SolidColorBrush(selectedColor);
                Canvas.SetLeft(newRectangle, e.GetPosition(this).X - newRectangle.Width / 2);
                Canvas.SetTop(newRectangle, e.GetPosition(this).Y - newRectangle.Height / 2);

                ObszarRoboczy.Children.Add(newRectangle);
            }

            else if (drawStyle == 9)
            {
                DrawChristmasTree(e.GetPosition(this).X, e.GetPosition(this).Y);
            }

            else if (drawStyle == 10)
            {
                DrawArrow(e.GetPosition(this).X, e.GetPosition(this).Y);
            }

            else if (drawStyle == 11)
            {
                DrawTriangle(e.GetPosition(this).X, e.GetPosition(this).Y);
            }
        }

        private Ellipse DrawPoint(double size, double x, double y, Color color)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Width = size;
            ellipse.Height = size;

            Canvas.SetTop(ellipse, y - size / 2);
            Canvas.SetLeft(ellipse, x - size / 2);

            Brush brushColor = new SolidColorBrush(selectedColor);

            ellipse.Fill = brushColor;

            ObszarRoboczy.Children.Add(ellipse);

            return ellipse;
        }

        private void btnDrawSegment_Click(object sender, RoutedEventArgs e)
        {
            drawStyle = 3;

            ClearEditSegmentData();
        }

        #region editSegment
        private void btnEditSegment_Click(object sender, RoutedEventArgs e)
        {
            drawStyle = 4;

            ClearEditSegmentData();
        }

        private double DistanceFromPointToLine(Point p, Line line)
        {
            var A = p.X - line.X1;
            var B = p.Y - line.Y1;
            var C = line.X2 - line.X1;
            var D = line.Y2 - line.Y1;

            var dot = A * C + B * D;
            var lenSq = C * C + D * D;
            var param = -1.0;
            if (lenSq != 0)
                param = dot / lenSq;

            double xx, yy;

            if (param < 0)
            {
                xx = line.X1;
                yy = line.Y1;
            }
            else if (param > 1)
            {
                xx = line.X2;
                yy = line.Y2;
            }
            else
            {
                xx = line.X1 + param * C;
                yy = line.Y1 + param * D;
            }

            var dx = p.X - xx;
            var dy = p.Y - yy;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private double DistanceToPoint(Ellipse ellipse, Point p)
        {
            Point center = new Point(Canvas.GetLeft(ellipse) + ellipse.Width / 2, Canvas.GetTop(ellipse) + ellipse.Height / 2);
            return Math.Sqrt(Math.Pow(center.X - p.X, 2) + Math.Pow(center.Y - p.Y, 2));
        }

        private void CheckEditSegmentOnMouseDownEvents()
        {
            if (!lineInEditing && !edgeInEditing)
            {
                Line closestLine = new Line();
                double minDistance = Double.MaxValue;

                foreach (var line in allLines)
                {
                    double distance = DistanceFromPointToLine(currentPoint, line);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestLine = line;
                    }
                }

                if (minDistance <= 5)
                {
                    clickedLine = closestLine;

                    Ellipse start = DrawPoint(6, clickedLine.X1, clickedLine.Y1, Colors.Blue);
                    Ellipse end = DrawPoint(6, clickedLine.X2, clickedLine.Y2, Colors.Blue);

                    editedPoints.Add(start);
                    editedPoints.Add(end);

                    lineInEditing = true;
                }
            }
            else if (lineInEditing && !edgeInEditing)
            {
                Ellipse closestEllipse = null;
                double minDistance = Double.MaxValue;

                int index = -1;

                foreach (var ellipse in editedPoints)
                {
                    double distance = DistanceToPoint(ellipse, currentPoint);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestEllipse = ellipse;
                        index = editedPoints.IndexOf(ellipse);
                    }
                }

                if (minDistance <= 5 && closestEllipse != null)
                {
                    Point elipseCenter = new Point(Canvas.GetLeft(closestEllipse) + closestEllipse.Width / 2, Canvas.GetTop(closestEllipse) + closestEllipse.Height / 2);

                    foreach (var ellipse in editedPoints)
                    {
                        ObszarRoboczy.Children.Remove(ellipse);
                    }

                    if (index < 0)
                        return;
                    else if (index == 0)
                        startInEditing = true;
                    else
                        startInEditing = false;


                    editedPoint = DrawPoint(6, elipseCenter.X, elipseCenter.Y, Colors.Blue);

                    editedPoints = new List<Ellipse>();

                    lineInEditing = false;
                    edgeInEditing = true;
                }
            }
            else if (edgeInEditing)
            {
                if (allLines.Contains(clickedLine))
                    allLines.Remove(clickedLine);

                ObszarRoboczy.Children.Remove(clickedLine);

                if (startInEditing)
                {
                    clickedLine.X1 = currentPoint.X;
                    clickedLine.Y1 = currentPoint.Y;
                }
                else
                {
                    clickedLine.X2 = currentPoint.X;
                    clickedLine.Y2 = currentPoint.Y;
                }

                ObszarRoboczy.Children.Add(clickedLine);
                ObszarRoboczy.Children.Remove(editedPoint);
                allLines.Add(clickedLine);

                clickedLine = new Line();
                editedPoint = new Ellipse();

                edgeInEditing = false;
                startInEditing = false;
            }
        }
        private void ClearEditSegmentData()
        {
            if (editedPoint != null)
                ObszarRoboczy.Children.Remove(editedPoint);

            if (editedPoints.Count > 0)
            {
                foreach (var point in editedPoints)
                {
                    ObszarRoboczy.Children.Remove(point);
                }
            }
            editedPoints = new List<Ellipse>();
            editedPoint = new Ellipse();
            clickedLine = new Line();
            lineInEditing = false;
            edgeInEditing = false;
            startInEditing = false;
        }
        #endregion editSegment

        private void btnDrawPolygon_Click(object sender, RoutedEventArgs e)
        {
            drawStyle = 5; // Wielokąt
        }

        private void btnDrawCircle_Click(object sender, RoutedEventArgs e)
        {
            drawStyle = 6; // Koło
        }

        private void btnDrawRectangle_Click(object sender, RoutedEventArgs e)
        {
            drawStyle = 7; // Prostokąt
        }

        private void btnDrawCurve_Click(object sender, RoutedEventArgs e)
        {
            isCurvedLineStarted = false;
            drawStyle = 8; // Krzywa
        }

        private void DrawChristmasTree(double x, double y)
        {
            DrawTree(x, y, 50, 100);
            DrawBauble(x, y, 8, Brushes.Yellow);
            DrawBauble(x - 3, y + 30, 5, Brushes.Orange);
            DrawBauble(x + 5, y + 60, 5, Brushes.Blue);
            DrawBauble(x + 11, y + 75, 5, Brushes.Red);
            DrawBauble(x - 7, y + 90, 5, Brushes.Magenta);
        }
        private void DrawTree(double x, double y, double width, double height)
        {
            var tree = new Polygon
            {
                Fill = Brushes.Green,
                Stroke = Brushes.DarkGreen,
                StrokeThickness = 2,
                Points = new PointCollection
                {
                new Point(x, y),
                new Point(x - width / 2, y + height),
                new Point(x + width / 2, y + height)
                }
            };
            ObszarRoboczy.Children.Add(tree);
        }
        private void DrawBauble(double x, double y, double radius, Brush
        color)
        {
            var bauble = new Ellipse
            {
                Width = radius,
                Height = radius,
                Fill = color
            };
            Canvas.SetLeft(bauble, x - radius / 2);
            Canvas.SetTop(bauble, y - radius / 2);
            ObszarRoboczy.Children.Add(bauble);
        }

        private void btnDrawChristmasTree_Click(object sender, RoutedEventArgs e)
        {
            drawStyle = 9; // Choinka
        }

        private void DrawTriangle(double x, double y)
        {
            Polygon triangle = new Polygon();
            triangle.Stroke = new SolidColorBrush(selectedColor);
            triangle.Fill = new SolidColorBrush(Colors.White);

            PointCollection points = new PointCollection
            {
                new Point(x, y - 20),
                new Point(x - 10, y),
                new Point(x + 10, y)
            };

            triangle.Points = points;

            ObszarRoboczy.Children.Add(triangle);
        }

        private void btnDrawArrow_Click(object sender, RoutedEventArgs e)
        {
            drawStyle = 10; // Strzałka
        }

        private void btnDrawTriangle_Click(object sender, RoutedEventArgs e)
        {
            drawStyle = 11; // Trójkąt
        }

        private void DrawArrow(double x, double y)
        {
            System.Windows.Shapes.Path arrowPath = new System.Windows.Shapes.Path();
            arrowPath.Stroke = new SolidColorBrush(selectedColor);
            arrowPath.Fill = new SolidColorBrush(selectedColor);
            arrowPath.StrokeThickness = 2;

            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            Point startPoint = new Point(x - 30, y);
            pathFigure.StartPoint = startPoint;

            PointCollection points = new PointCollection
            {
                new Point(x - 30, y - 10),
                new Point(x + 20, y - 10),
                new Point(x + 20, y - 20),
                new Point(x + 50, y),
                new Point(x + 20, y + 20),
                new Point(x + 20, y + 10),
                new Point(x - 30, y + 10),
                startPoint
            };

            foreach (Point point in points)
            {
                pathFigure.Segments.Add(new LineSegment(point, true));
            }

            pathGeometry.Figures.Add(pathFigure);
            arrowPath.Data = pathGeometry;

            ObszarRoboczy.Children.Add(arrowPath);
        }




        public class ColorChangedEventArgs : EventArgs
        {
            public Color SelectedColor { get; }

            public ColorChangedEventArgs(Color selectedColor)
            {
                SelectedColor = selectedColor;
            }
        }
        private void MainWindow_ColorChanged(object sender, ColorChangedEventArgs e)
        {
            // Tutaj dostaniesz wybrany kolor z drugiego okna
            selectedColor = e.SelectedColor;

            // Możesz użyć tego koloru do zmiany wartości Rectangle lub innego elementu
            colorPick.Fill = new SolidColorBrush(selectedColor);
        }
        private void colorPick_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ColorPickWindow colorPickWindow = new ColorPickWindow();
            colorPickWindow.ColorSelected += ColorPickWindow_ColorSelected;
            colorPickWindow.Show();

            //if (colorPickWindow.ShowDialog() == true)
            //{
            //    selectedColor = colorPickWindow.tempcolor;

            //    colorPick.Fill = new SolidColorBrush(selectedColor);
            //}
            //selectedColor = colorPickWindow.tempcolor;
        }

        private void ColorPickWindow_ColorSelected(object sender, ColorChangedEventArgs e)
        {
            // Tutaj dostaniesz wybrany kolor z drugiego okna
            Color selectedColor = e.SelectedColor;

            // Możesz użyć tego koloru do zmiany wartości Rectangle
            colorPick.Fill = new SolidColorBrush(selectedColor);

            // Wywołaj zdarzenie, aby przekazać kolor do innych zainteresowanych obiektów
            ColorChanged?.Invoke(this, e);
        }


        public void SaveToPngFile(Uri path, Canvas surface)
        {
            if (path == null) return;
            Transform transform = surface.LayoutTransform;
            surface.LayoutTransform = null;

            Size size = new Size(surface.ActualWidth, surface.ActualHeight);

            surface.Measure(size);
            surface.Arrange(new Rect(size));

            RenderTargetBitmap renderTargetBitmap =
                new RenderTargetBitmap(
                    (int)size.Width,
                    (int)size.Height,
                    96d,
                    96d,
                    PixelFormats.Pbgra32
                    );
            renderTargetBitmap.Render(surface);

            using (FileStream outStream = new FileStream(path.LocalPath, FileMode.Create))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
                encoder.Save(outStream);
            }
            surface.LayoutTransform = transform;

        }

        public void SaveToJpgFile(Uri path, Canvas surface)
        {
            if (path == null) return;
            Transform transform = surface.LayoutTransform;
            surface.LayoutTransform = null;

            Size size = new Size(surface.ActualWidth, surface.ActualHeight);

            surface.Measure(size);
            surface.Arrange(new Rect(size));

            RenderTargetBitmap renderTargetBitmap =
                new RenderTargetBitmap(
                    (int)size.Width,
                    (int)size.Height,
                    96d,
                    96d,
                    PixelFormats.Pbgra32
                    );
            renderTargetBitmap.Render(surface);

            using (FileStream outStream = new FileStream(path.LocalPath, FileMode.Create))
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
                encoder.Save(outStream);
            }
            surface.LayoutTransform = transform;

        }

        private void zapisz_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Image File (*.png)|*.png|File Image (*.jpg)|*.jpg";

            if (saveFileDialog.ShowDialog() == true)
            {
                if (saveFileDialog.FilterIndex == 1)
                {
                    Uri newFileUri = new Uri(saveFileDialog.FileName);
                    SaveToPngFile(newFileUri, ObszarRoboczy);
                }
                else
                {

                    Uri newFileUri = new Uri(saveFileDialog.FileName);
                    SaveToJpgFile(newFileUri, ObszarRoboczy);
                }
            }
        }

        private void btnzdjecie_Click(object sender, RoutedEventArgs e)
        {
            Obraz obrazwindow = new Obraz();
            obrazwindow.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            drawStyle = 12;

            ClearEditSegmentData();
        }

        private void UploadPicture_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();

            openDialog.Filter = "Image Files(*.jpg;*.jpeg; *.gif;*.bmp)|*.jpg;*.jpeg;*.gif;*.bmp";

            if (openDialog.ShowDialog() == true)
            {
                string filePath = openDialog.FileName;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(filePath);
                bitmapImage.EndInit();

                Window imageWindow = new Window
                {
                    Title = "Selected Image",
                    SizeToContent = SizeToContent.WidthAndHeight,
                    Content = new Image
                    {
                        Source = bitmapImage,
                        Stretch = Stretch.Uniform
                    }
                };

                imageWindow.ShowDialog();

            }
        }

        private void sobel_Click(object sender, RoutedEventArgs e)
        {

            FilterWindow sobel = new FilterWindow();
            sobel.Show();
        }
    }





}
