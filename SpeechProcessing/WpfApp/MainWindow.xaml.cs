using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

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
            if (e.PropertyName == nameof(mainViewModel.OriginalFile))
            {
                DrawWaveLine(mainViewModel.OriginalFile.Data[0], canvas1);
            }
            else if (e.PropertyName == nameof(mainViewModel.ConvertedFile))
            {
                DrawWaveLine(mainViewModel.ConvertedFile.Data[0], canvas2);
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