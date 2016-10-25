using System.ComponentModel;
using System.Linq;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        private MainViewModel mainViewModel;

        public MainWindow()
        {
            InitializeComponent();
            mainViewModel = new MainViewModel();
            mainViewModel.PropertyChanged += MainViewModel_PropertyChanged;
            DataContext = mainViewModel;
        }

        private void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(mainViewModel.File))
            {
                DrawWaveLine(mainViewModel.File.Data[0], canvas);
            }
        }

        private void DrawWaveLine(int[] array, Canvas canvas)
        {
            if (array.Length < 2) return;
            int max = array.Max(x => Math.Abs(x));
            double height = canvas.ActualHeight;
            double step = canvas.ActualWidth / (array.Length - 1);
            Path path = new Path()
            {
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1,
            };
            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(0, ConvertY(array[0], max, height));
            Point prev, current;
            prev = pathFigure.StartPoint;
            for (int i = 1; i < array.Length - 2; i++)
            {
                current = new Point(step * i, ConvertY(array[i], max, height));
                //pathFigure.Segments.Add(new QuadraticBezierSegment()
                //{
                //    Point1 = prev,
                //    Point2 = new Point((current.X + prev.X) / 2, (current.Y + prev.Y) / 2)
                //});
                pathFigure.Segments.Add(new LineSegment()
                {
                    Point = current
                });
                prev = current;
            }
            pathFigure.Segments.Add(new LineSegment()
            {
                Point = new Point(canvas.ActualWidth, ConvertY(array[array.Length - 1], max, height))
            });
            pathGeometry.Figures.Add(pathFigure);
            path.Data = pathGeometry;
            canvas.Children.Clear();
            canvas.Children.Add(path);
        }

        private double ConvertY(int y, int maxY, double height) => (maxY - y) * height / (2 * maxY);
    }
}