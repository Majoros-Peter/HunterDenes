using HelixToolkit.Wpf;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Text;
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
using static Hunter_Dénes.Gyongy;
using static Hunter_Dénes.Robot;

namespace Hunter_Dénes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static float sebesség = 1.5f;

        private float X { get; set; } = 20;
        private float Y { get; set; } = 20;
        private float Z { get; set; } = 20;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Betolt("gyongyok.txt");

            foreach (var gyongy in gyongyok)
            {
                EllipsoidVisual3D gyongy3d = new();
                gyongy3d.RadiusX = 1;
                gyongy3d.RadiusY = 1;
                gyongy3d.RadiusZ = 1;
                gyongy3d.Center = new Point3D(gyongy.X, gyongy.Y, gyongy.Z);
                gyongy3d.Fill = new SolidColorBrush(Color.FromArgb(255, (byte)(255 - gyongy.Ertek * 20), 80, (byte)(gyongy.Ertek * 20)));
                ter.Children.Add(gyongy3d);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W)
                X += sebesség;
            else if (e.Key == Key.S)
                X -= sebesség;

            if (e.Key == Key.LeftShift)
                Y += sebesség;
            else if (e.Key == Key.Space)
                Y -= sebesség;

            if (e.Key == Key.A)
                Z += sebesség;
            else if (e.Key == Key.D)
                Z -= sebesség;

            kamera.Position = new(X, Y, Z);
        }
    }
}