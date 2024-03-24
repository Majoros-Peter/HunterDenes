namespace Hunter_Dénes;

using HelixToolkit.Wpf;
using Microsoft.Win32;
using System.Diagnostics;
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
using static Gyongy;

public partial class MainWindow : Window
{
    #region __Hajo__
    private static Color[] szinek;

    public double HajoX { get; set; } = 0;
    public double HajoY { get; set; } = 0;
    public double HajoZ { get; set; } = 0;

    public double Hosszusag { get; set; } = 5;
    public double Szelesseg { get; set; } = 5;
    public double Magassag { get; set; } = 5;
    #endregion

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        InitSzinek();
    }

    private void InitSzinek()
    {
        szinek = new Color[]
        {
            Color.FromRgb(255, 249, 61),
            Color.FromRgb(17, 73, 186),
            Color.FromRgb(41, 20, 199),
            Color.FromRgb(87, 18, 224),
            Color.FromRgb(109, 34, 201),
            Color.FromRgb(119, 40, 189),
            Color.FromRgb(174, 52, 201),
            Color.FromRgb(217, 52, 159),
            Color.FromRgb(220, 227, 0)
        };
    }

    private void BetoltGyongyok(string path)
    {
        ter.Children.Clear();

        Betolt(path);

        foreach(Gyongy gyongy in gyongyok)
        {
            Hosszusag = gyongy.X < Hosszusag ? Hosszusag : gyongy.X;
            Szelesseg = gyongy.Y < Szelesseg ? Szelesseg : gyongy.Y;
            Magassag = gyongy.Z < Magassag ? Magassag : gyongy.Z;

            EllipsoidVisual3D gyongy3d = new()
            {
                RadiusX = .05 * (gyongy.Ertek + 1) + 1,
                RadiusY = .05 * (gyongy.Ertek + 1) + 1,
                RadiusZ = .05 * (gyongy.Ertek + 1) + 1,
                Center = new Point3D(-gyongy.X * 2, gyongy.Y * 2, -gyongy.Z * 2),
                Fill = new SolidColorBrush(szinek[gyongy.Ertek % szinek.Length])
            };

            gyongy3d.SetName(gyongy.Id.ToString());
            ter.Children.Add(gyongy3d);
        }
    }

    private void BetoltGyongyok(Random rand)
    {
        ter.Children.Clear();

        Betolt(rand);

        foreach(Gyongy gyongy in gyongyok)
        {
            Hosszusag = gyongy.X < Hosszusag ? Hosszusag : gyongy.X;
            Szelesseg = gyongy.Y < Szelesseg ? Szelesseg : gyongy.Y;
            Magassag = gyongy.Z < Magassag ? Magassag : gyongy.Z;

            EllipsoidVisual3D gyongy3d = new()
            {
                RadiusX = .05 * (gyongy.Ertek + 1) + 1,
                RadiusY = .05 * (gyongy.Ertek + 1) + 1,
                RadiusZ = .05 * (gyongy.Ertek + 1) + 1,
                Center = new Point3D(-gyongy.X * 2, gyongy.Y * 2, -gyongy.Z * 2),
                Fill = new SolidColorBrush(szinek[gyongy.Ertek % szinek.Length])
            };

            gyongy3d.SetName(gyongy.Id.ToString());
            ter.Children.Add(gyongy3d);
        }
    }

    private void KeszitAkvarium(double x, double y, double z)
    {
        SolidColorBrush viz = new(Colors.LightBlue)
        {
            Opacity = 0.4
        };
        BoxVisual3D teglaTest = new()
        {
            Length = x,
            Width = y,
            Height = z,
            Center = new Point3D(-Math.Ceiling(x / 2.0 - 2), Math.Ceiling(y / 2.0 - 2), -Math.Ceiling(z / 2.0 - 2)),
            Fill = viz
        };
        ter.Children.Add(teglaTest);

        RajzolVonal([x, y, z], [x, y, 0]);
        RajzolVonal([x, y, z], [x, 0, z]);
        RajzolVonal([x, y, z], [0, y, z]);
        RajzolVonal([0, 0, 0], [0, 0, z]);
        RajzolVonal([0, 0, 0], [0, y, 0]);
        RajzolVonal([0, 0, 0], [x, 0, 0]);

        RajzolVonal([x, 0, 0], [x, y, 0]);
        RajzolVonal([x, 0, 0], [x, 0, z]);
        RajzolVonal([0, y, 0], [0, y, z]);
        RajzolVonal([0, y, 0], [x, y, 0]);
        RajzolVonal([0, 0, z], [0, y, z]);
        RajzolVonal([0, 0, z], [x, 0, z]);
    }

    private void RajzolVonal(double[] kezdet, double[] veg)
    {
        LinesVisual3D vonal3D = new()
        {
            Thickness = 1.2,
            Points = [new Point3D(-(kezdet[0] - 2), kezdet[1] - 2, -(kezdet[2] - 2)), new Point3D(-(veg[0] - 2), veg[1] - 2, -(veg[2] - 2))]
        };
        ter.Children.Add(vonal3D);
    }

    private void LerakTengeralattjaro(double x, double y, double z)
    {
        var importer = new ModelImporter();
        var model = importer.Load("Tengeralatjaro.obj");

        var modelVisual = new ModelVisual3D
        {
            Content = model
        };

        // Hol legyen a tengeralattjáró
        Point3D position = new(x / 10.0, y / 10.0 + 0.2, z / 10.0);
        TranslateTransform3D translation = new(position.X, position.Y, position.Z);

        ScaleTransform3D scale = new(18, 18, 18);
        RotateTransform3D rotate = new(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90));
        Transform3DGroup transformGroup = new();
        transformGroup.Children.Add(rotate);
        transformGroup.Children.Add(translation);
        transformGroup.Children.Add(scale);

        modelVisual.Transform = transformGroup;
        ter.Children.Add(modelVisual);
    }

    private void BtnBeolvas_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = "Szöveges dokumentumok (*.txt)|*.txt"
        };

        if(openFileDialog.ShowDialog() is true)
        {
            BetoltGyongyok(openFileDialog.FileName);

            LerakTengeralattjaro(HajoX, HajoY, HajoZ);
            KeszitAkvarium(2 * Hosszusag + 4, 2 * Szelesseg + 4, 2 * Magassag + 4);
            ter.Children.Add(new SunLight());
        }
    }

    private void BtnVeletlenPalya_Click(object sender, RoutedEventArgs e)
    {
        BetoltGyongyok(new Random());

        LerakTengeralattjaro(HajoX, HajoY, HajoZ);
        KeszitAkvarium(2 * Hosszusag + 4, 2 * Szelesseg + 4, 2 * Magassag + 4);
        ter.Children.Add(new SunLight());
    }

    private void BtnLerakas_Click(object sender, RoutedEventArgs e)
    {
        HajoX = Convert.ToDouble(TbX.Text);
        HajoY = Convert.ToDouble(TbY.Text);
        HajoZ = Convert.ToDouble(TbZ.Text);

        LerakTengeralattjaro(HajoX, HajoY, HajoZ);
        KeszitAkvarium(2 - (Hosszusag + 4), 2 * Szelesseg + 4, -(2 * Magassag + 4));

        ter.Children.Add(new SunLight());
    }

    private void RobotAI()
    {
        Robot robot = new(Convert.ToDouble(txtUthossz.Text));

        if(stopper.IsChecked is false)
        {
            lbGyongyok.ItemsSource = robot.AI(gyongyok[0]);
            return;
        }

        Stopwatch sw = Stopwatch.StartNew();

        lbGyongyok.ItemsSource = robot.AI(gyongyok[0]);

        sw.Stop();
        MessageBox.Show($"{sw.ElapsedMilliseconds} milliseconds");

    }

    private void Inditas_Click(object sender, RoutedEventArgs e)
    {
        Cursor = Cursors.Wait;

        RobotAI();

        Cursor = Cursors.Arrow;

        foreach(Gyongy gyongy in lbGyongyok.Items)
            (ter.Children.First(G => G.GetName() == gyongy.Id.ToString()) as EllipsoidVisual3D).Fill = new SolidColorBrush(Colors.Green);
    }
}