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
using System.Windows.Media.Animation;
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

        foreach (Gyongy gyongy in gyongyok)
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

        foreach (Gyongy gyongy in gyongyok)
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

        ModelVisual3D modelVisual = new ModelVisual3D
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

        if (openFileDialog.ShowDialog() is true)
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
        if (TbX.Text != "" && TbY.Text != "" && TbZ.Text != "")
        {
            HajoX = Convert.ToDouble(TbX.Text);
            HajoY = Convert.ToDouble(TbY.Text);
            HajoZ = Convert.ToDouble(TbZ.Text);

            LerakTengeralattjaro(HajoX, HajoY, HajoZ);
            KeszitAkvarium(2 - (Hosszusag + 4), 2 * Szelesseg + 4, -(2 * Magassag + 4));

            ter.Children.Add(new SunLight());
        }
        else
        {
            MessageBox.Show("Adjon meg koordinátákat!");
        }


    }

    private void RobotAI()
    {
        Robot robot = new(Convert.ToDouble(txtUthossz.Text));

        if (stopper.IsChecked is false)
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
        if (ter.Children.Count() < 3)
        {
            MessageBox.Show("Töltsön be pályát!");
        }
        else
        {
            Cursor = Cursors.Wait;

            RobotAI();

            Cursor = Cursors.Arrow;

            foreach (Gyongy gyongy in lbGyongyok.Items)
                (ter.Children.First(G => G.GetName() == gyongy.Id.ToString()) as EllipsoidVisual3D).Fill = new SolidColorBrush(Colors.Green);

            Osszekotes();
        }
    }

    private void Osszekotes()
    {
        for (int i = 1; i < lbGyongyok.Items.Count; i++)
        {

            double[] kezdet = lbGyongyok.Items[i - 1].ToString().Split(' ')[0].Trim('(').Trim(')').Split(';').Select(G => double.Parse(G)).ToArray();
            double[] veg = lbGyongyok.Items[i].ToString().Split(' ')[0].Trim('(').Trim(')').Split(';').Select(G => double.Parse(G)).ToArray();
            LinesVisual3D vonal3D = new()
            {
                Thickness = 5,
                Points = [new Point3D(kezdet[0] * -2, kezdet[1] * 2, kezdet[2] * -2), new Point3D(veg[0] * -2, veg[1] * 2, veg[2] * -2)],
                Color = Colors.DarkGreen,
            };
            ter.Children.Add(vonal3D);

        }

        double[] elso = lbGyongyok.Items[0].ToString().Split(' ')[0].Trim('(').Trim(')').Split(';').Select(G => double.Parse(G)).ToArray();
        LinesVisual3D elsoVonal = new()
        {
            Thickness = 5,
            Points = [new Point3D(0, 0, 0), new Point3D(elso[0] * -2, elso[1] * 2, elso[2] * -2)],
            Color = Colors.DarkGreen,
        };
        double[] utolso = lbGyongyok.Items[^1].ToString().Split(' ')[0].Trim('(').Trim(')').Split(';').Select(G => double.Parse(G)).ToArray();
        LinesVisual3D utolsoVonal = new()
        {
            Thickness = 5,
            Points = [new Point3D(0, 0, 0), new Point3D(utolso[0] * -2, utolso[1] * 2, utolso[2] * -2)],
            Color = Colors.DarkGreen,
        };
        ter.Children.Add(elsoVonal);
        ter.Children.Add(utolsoVonal);
    }

    private void lbGyongyok_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        int[] koordinatak = lbGyongyok.SelectedItem.ToString().Split(' ')[0].Trim('(').Trim(')').Split(';').Select(G => int.Parse(G)).ToArray();
        camera.Position = new Point3D(-koordinatak[0] * 2 + 5, koordinatak[1] * 2 + 5, -koordinatak[2] * 2 + 5);
        camera.LookDirection = new Vector3D(-0.9, -0.9, -0.9);
        camera.UpDirection = new Vector3D(0, 0, 1);
    }

    private void TengeralattjaroMozgat(float X1, float Y1, float Z1, float X2, float Y2, float Z2)
    {
        var importer = new ModelImporter();
        var model = importer.Load("Tengeralatjaro.obj");

        // Modell létrehozása
        ModelVisual3D modelVisual = new ModelVisual3D
        {
            Content = model
        };

        Point3D position = new Point3D(X1, Y1, Z1);

        // Transformációk létrehozása
        TranslateTransform3D translation = new TranslateTransform3D(position.X, position.Y, position.Z);
        ScaleTransform3D scale = new ScaleTransform3D(18, 18, 18);
        RotateTransform3D rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90));

        // Transformációk hozzáadása a modellhez
        Transform3DGroup transformGroup = new Transform3DGroup();
        transformGroup.Children.Add(translation);
        transformGroup.Children.Add(scale);
        transformGroup.Children.Add(rotate);
        modelVisual.Transform = transformGroup;

        // Modell hozzáadása a viewport-hoz
        ter.Children.Add(modelVisual);

        // Animáció létrehozása
        Point3D newPosition = new Point3D(X2, Y2, Z2); // Példaérték, cseréld le a tényleges értékre
        DoubleAnimation animationX = new DoubleAnimation
        {
            From = position.X,
            To = newPosition.X,
            Duration = TimeSpan.FromSeconds(3),
        };

        DoubleAnimation animationY = new DoubleAnimation
        {
            From = position.Y,
            To = newPosition.Y,
            Duration = TimeSpan.FromSeconds(3),
        };

        DoubleAnimation animationZ = new DoubleAnimation
        {
            From = position.Z,
            To = newPosition.Z,
            Duration = TimeSpan.FromSeconds(3),
        };

        // Animációk hozzáadása a TranslateTransform3D objektumhoz
        translation.BeginAnimation(TranslateTransform3D.OffsetXProperty, animationX);
        translation.BeginAnimation(TranslateTransform3D.OffsetYProperty, animationY);
        translation.BeginAnimation(TranslateTransform3D.OffsetZProperty, animationZ);
    }

    private void animation_Click(object sender, RoutedEventArgs e)
    {

        for (int i = 1; i < gyongyok.Length; i++)
        {
            TengeralattjaroMozgat(0, 0, 0, 0f, 0f, -2f);
        }



    }

}

