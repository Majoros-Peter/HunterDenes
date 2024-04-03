using Hunter_Dénes.Classok;

namespace Hunter_Dénes;

using HelixToolkit.Wpf;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.RegularExpressions;
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
    #region __Változók__
    private static Color[] szinek;
    private static string[] zenek = { "1812", "Ballad Of The Mer", "Bosun Bill", "Grogg Mayles", "Maiden Voyage", "Row Row Row Your Boat", "Who Shall not be Returning", "Yo Ho - A Pirate's Life" };
    private static int musicCounter = 0;

    public double Hosszusag { get; set; } = 5;
    public double Szelesseg { get; set; } = 5;
    public double Magassag { get; set; } = 5;

    Point3D hajoHelye = new(0, 0, 0);
    private MediaPlayer mediaPlayer = new();

    public int Osszesen { get; private set; } = 0;
    public int Osszeszedve { get; private set; } = 0;
    public int Osszeg { get; private set; } = 0;
    public double Szazalek { get; private set; } = 0;
    #endregion

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        PlayMusic();
        InitSzinek();
    }

    #region __Backend__
    private void InitSzinek()
    {
        szinek =
        [
            Color.FromRgb(31, 100, 238),
            Color.FromRgb(17, 73, 186),
            Color.FromRgb(41, 20, 199),
            Color.FromRgb(87, 18, 224),
            Color.FromRgb(109, 34, 201),
            Color.FromRgb(119, 40, 189),
            Color.FromRgb(174, 52, 201),
            Color.FromRgb(217, 52, 159),
            Color.FromRgb(255, 249, 61),
            Color.FromRgb(237, 164, 55)
        ];
    }
    private void PlayMusic()
    {
        try
        {
            mediaPlayer.Open(new Uri(Directory.GetCurrentDirectory() + $"/Zenek/{zenek[musicCounter]}.mp3"));
            mediaPlayer.MediaEnded += delegate {
                musicCounter++;
                PlayMusic();
            };
            mediaPlayer.Play();
            lblZene.Content = "Zene: " + zenek[musicCounter];
        } catch(Exception ex)
        {
            MessageBox.Show("Hiba történt a zene lejátszása közben: " + ex.Message);
        }
    }
    private void btnMusicBack_Click(object sender, RoutedEventArgs e)
    {
        musicCounter--;
        if(musicCounter == -1)
        {
            musicCounter = zenek.Length - 1;
        }
        PlayMusic();

    }
    private void btnMusicNext_Click(object sender, RoutedEventArgs e)
    {
        musicCounter++;
        if(musicCounter >= zenek.Length)
        {
            musicCounter = 0;
        }
        PlayMusic();
    }

    private void BetoltGyongyok(string path)
    {
        Hosszusag = 0;
        Szelesseg = 0;
        Magassag = 0;
        ter.Children.Clear();

        Betolt(path);

        int oszto = gyongyok.Max(G => G.Ertek) / szinek.Length + 1;

        foreach (Gyongy gyongy in gyongyok)
        {
            Hosszusag = Math.Max(gyongy.X, Hosszusag);
            Szelesseg = Math.Max(gyongy.Y, Szelesseg);
            Magassag = Math.Max(gyongy.Z, Magassag);

            EllipsoidVisual3D gyongy3d = new()
            {
                RadiusX = gyongy.Ertek == 0 ? 0 : Math.Log10(gyongy.Ertek + 1) * 1.2,
                RadiusY = gyongy.Ertek == 0 ? 0 : Math.Log10(gyongy.Ertek + 1) * 1.2,
                RadiusZ = gyongy.Ertek == 0 ? 0 : Math.Log10(gyongy.Ertek + 1) * 1.2,
                Center = Pont(gyongy),
                Fill = new SolidColorBrush(szinek[gyongy.Ertek / oszto])
            };

            gyongy3d.SetName(gyongy.Id.ToString());
            ter.Children.Add(gyongy3d);
        }
    }
    private void BetoltGyongyok()
    {
        ter.Children.Clear();

        Random rand = new();
        Func<int> x = () => rand.Next((int)Hosszusag+1);
        Func<int> y = () => rand.Next((int)Szelesseg+1);
        Func<int> z = () => rand.Next((int)Magassag+1);
        int gyongyokSzama = rand.Next((int)slGyongyokSzama.LowerValue, (int)slGyongyokSzama.HigherValue)+1;
        Func<int> ertek = () => rand.Next((int)slGyongyErtekek.Value)+1;

        Betolt(x, y, z, ertek, gyongyokSzama);

        foreach (Gyongy gyongy in gyongyok)
        {
            EllipsoidVisual3D gyongy3d = new()
            {
                RadiusX = gyongy.Ertek == 0 ? 0 : Math.Log10(gyongy.Ertek + 1) * 1.2,
                RadiusY = gyongy.Ertek == 0 ? 0 : Math.Log10(gyongy.Ertek + 1) * 1.2,
                RadiusZ = gyongy.Ertek == 0 ? 0 : Math.Log10(gyongy.Ertek + 1) * 1.2,
                Center = Pont(gyongy),
                Fill = new SolidColorBrush(szinek[gyongy.Ertek / (gyongyok.Max(G => G.Ertek) / szinek.Length+1)])
            };

            gyongy3d.SetName(gyongy.Id.ToString());
            ter.Children.Add(gyongy3d);
        }
    }
    private void RobotAI()
    {
        Robot robot = new(Convert.ToDouble(txtSebesseg.Text) * Convert.ToDouble(txtIdo.Text), gyongyok[0]);
        IList<Gyongy> koordinatak;

        if (stopper.IsChecked is false)
        {
            koordinatak = robot.AI();
            lbGyongyok.ItemsSource = koordinatak.Count != 0 ? koordinatak : new List<string>() { "Nincsenek összegyűjthető gyöngyök" };
            return;
        }

        Stopwatch sw = Stopwatch.StartNew();

        koordinatak = robot.AI();
        lbGyongyok.ItemsSource = koordinatak.Count != 0 ? koordinatak : new List<string>() { "Nincsenek összegyűjthető gyöngyök" };

        sw.Stop();
        MessageBox.Show($"{sw.ElapsedMilliseconds} milliseconds");

    }

    private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = new Regex(@"[^-0-9.]+").IsMatch(e.Text);
    #endregion

    #region __UI Elemek__
    private void Osszekotes()
    {
        ter.Children.Clear();
        Gyongy kezdet, veg;

        for(int i = 1; i < lbGyongyok.Items.Count; i++)
        {
            kezdet = (lbGyongyok.ItemsSource as List<Gyongy>)[i - 1];
            veg = (lbGyongyok.ItemsSource as List<Gyongy>)[i];

            LinesVisual3D vonal3D = new()
            {
                Thickness = 5,
                Points = [Pont(kezdet), Pont(veg)],
                Color = Colors.DarkGreen,
            };

            ter.Children.Add(vonal3D);
        }

        Gyongy elso = (lbGyongyok.ItemsSource as List<Gyongy>)[0];
        LinesVisual3D elsoVonal = new()
        {
            Thickness = 5,
            Points = [new Point3D(0, 0, 0), Pont(elso)],
            Color = Colors.DarkGreen,
        };

        Gyongy utolso = (lbGyongyok.ItemsSource as List<Gyongy>)[^1];
        LinesVisual3D utolsoVonal = new()
        {
            Thickness = 5,
            Points = [new Point3D(0, 0, 0), Pont(utolso)],
            Color = Colors.DarkGreen,
        };

        ter.Children.Add(elsoVonal);
        ter.Children.Add(utolsoVonal);
        foreach(Gyongy gyongy in gyongyok)
        {
            Hosszusag = gyongy.X < Hosszusag ? Hosszusag : gyongy.X;
            Szelesseg = gyongy.Y < Szelesseg ? Szelesseg : gyongy.Y;
            Magassag = gyongy.Z < Magassag ? Magassag : gyongy.Z;

            EllipsoidVisual3D gyongy3d = new()
            {
                RadiusX = gyongy.Ertek == 0 ? 0 : Math.Log10(gyongy.Ertek + 1) * 1.2,
                RadiusY = gyongy.Ertek == 0 ? 0 : Math.Log10(gyongy.Ertek + 1) * 1.2,
                RadiusZ = gyongy.Ertek == 0 ? 0 : Math.Log10(gyongy.Ertek + 1) * 1.2,
                Center = new Point3D(-gyongy.X * 2, gyongy.Y * 2, -gyongy.Z * 2),
                Fill = new SolidColorBrush(szinek[gyongy.Ertek % szinek.Length])
            };

            gyongy3d.SetName(gyongy.Id.ToString());
            ter.Children.Add(gyongy3d);
        }
        LerakTengeralattjaro();
        KeszitAkvarium(2 * Hosszusag + 4, 2 * Szelesseg + 4, 2 * Magassag + 4);
        ter.Children.Add(new SunLight());

    }
    private void KeszitAkvarium(double x, double y, double z)
    {
        SolidColorBrush viz = new(Colors.LightBlue)
        {
            Opacity = .6
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
    private void LerakTengeralattjaro()
    {
        var existingViewport = ter.Viewport;
        string objFilePath = "Jarmu/TengeralattjaroSzines.obj";
        string mtlFilePath = "Jarmu/TengeralattjaroSzines.mtl";
        ObjImporter.ImportObjWithColors(objFilePath, mtlFilePath, existingViewport, 0.01);
    }
    #endregion

    #region __Click/DoubleClick__
    private void BtnBeolvas_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = "Szöveges dokumentumok (*.txt)|*.txt"
        };

        if (openFileDialog.ShowDialog() is true)
        {
            Osszeszedve = 0;
            Osszeg = 0;
            lbGyongyok.ItemsSource = new List<Gyongy>();

            BetoltGyongyok(openFileDialog.FileName);

            LerakTengeralattjaro();
            KeszitAkvarium(2 * Hosszusag + 4, 2 * Szelesseg + 4, 2 * Magassag + 4);
            ter.Children.Add(new SunLight());
        }
    }
    private void BtnLerakas_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(TbX.Text) && !string.IsNullOrWhiteSpace(TbY.Text) && !string.IsNullOrWhiteSpace(TbZ.Text))
        {
            camera.Position = new Point3D(double.Parse(TbX.Text), double.Parse(TbY.Text), double.Parse(TbZ.Text));
            camera.LookDirection = new Vector3D(-0.9, -0.9, -0.9);
            camera.UpDirection = new Vector3D(0, 0, 1);
        }
        else
        {
            MessageBox.Show("Adjon meg koordinátákat!");
        }
    }
    private void Inditas_Click(object sender, RoutedEventArgs e)
    {
        if (ter.Children.Count < 3)
        {
            MessageBox.Show("Töltsön be pályát!");
            return;
        }

        Cursor = Cursors.Wait;
        RobotAI();
        Cursor = Cursors.Arrow;

        if(lbGyongyok.Items.Count > 1)
        {
            Osszekotes();
            EredmenyKiiras();
        }
    }
    private void EredmenyKiiras() {
        int sum = 0;
        foreach (Gyongy gyongy in lbGyongyok.Items)
            sum += gyongy.Ertek;
        
        Osszeszedve = lbGyongyok.Items.Count;
        Osszeg = sum;
    }

    private void lbGyongyok_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        camera.Position = Pont((Gyongy)lbGyongyok.SelectedItem, 5, 5, 5);
        camera.LookDirection = new Vector3D(-0.9, -0.9, -0.9);
        camera.UpDirection = new Vector3D(0, 0, 1);
    }
    private void BtnVeletlenPalya_Click(object sender, RoutedEventArgs e)
    {
        Osszesen = 0;
        Osszeg = 0;
        lbGyongyok.ItemsSource = new List<Gyongy>();

        Random rand = new();

        Hosszusag = rand.Next((int)slHosszusag.LowerValue -1, (int)slHosszusag.HigherValue);
        Szelesseg = rand.Next((int)slHosszusag.LowerValue -1, (int)slHosszusag.HigherValue);
        Magassag = rand.Next((int)slHosszusag.LowerValue -1, (int)slHosszusag.HigherValue);

        BetoltGyongyok();

        LerakTengeralattjaro();
        KeszitAkvarium(2 * Hosszusag + 4, 2 * Szelesseg + 4, 2 * Magassag + 4);
        ter.Children.Add(new SunLight());
    }
    #endregion

    #region __Animáció__
    private void BtnAnimacio_Click(object sender, RoutedEventArgs e)
    {
        if (lbGyongyok.Items.Count == 1)
            return;

        List<Point3D> coordinateList = new List<Point3D>()
        {
        };

        foreach (Gyongy gyongy in lbGyongyok.ItemsSource)
        {
            coordinateList.Add(new Point3D(-gyongy.X * 2, gyongy.Y * 2, -gyongy.Z * 2 - 0.7));
        };


        Animacio(coordinateList);
    }

    private void Animacio(List<Point3D> kordik)
    {
        MoveToNextCoordinate(0, kordik);
    }
    void MoveToCoordinate(ModelVisual3D jarmu, Point3D coordinate, Action callback)
    {
        TranslateTransform3D translateTransform = jarmu.Transform as TranslateTransform3D;

        double tavolsag = CalculateDistance(hajoHelye, coordinate);

        double idoMpBen = tavolsag / Convert.ToInt32(txtSebesseg.Text) * 1.0;
        // Create translation animations for X, Y, and Z coordinates
        DoubleAnimation animationX = new DoubleAnimation();
        animationX.From = hajoHelye.X; // Start from the current X position
        animationX.To = coordinate.X; // Destination X coordinate
        animationX.Duration = TimeSpan.FromSeconds(idoMpBen); // Duration of the animation

        DoubleAnimation animationY = new DoubleAnimation();
        animationY.From = hajoHelye.Y; // Start from the current Y position
        animationY.To = coordinate.Y; // Destination Y coordinate
        animationY.Duration = TimeSpan.FromSeconds(idoMpBen); // Duration of the animation

        DoubleAnimation animationZ = new DoubleAnimation();
        animationZ.From = hajoHelye.Z; // Start from the current Z position
        animationZ.To = coordinate.Z; // Destination Z coordinate
        animationZ.Duration = TimeSpan.FromSeconds(idoMpBen); // Duration of the animation

        // Create a new TranslateTransform3D to hold the current translation
        TranslateTransform3D translation = new TranslateTransform3D();
        jarmu.Transform = translation;

        // Handle the Completed event of each animation to trigger the next movement
        animationZ.Completed += (sender, e) =>
        {
            hajoHelye = coordinate; // Update the current position
            callback(); // Trigger the callback function
        };

        // Start the animations
        translation.BeginAnimation(TranslateTransform3D.OffsetXProperty, animationX);
        translation.BeginAnimation(TranslateTransform3D.OffsetYProperty, animationY);
        translation.BeginAnimation(TranslateTransform3D.OffsetZProperty, animationZ);
    }

    void MoveToNextCoordinate(int index, List<Point3D> kordik)
    {
        ModelVisual3D tengeralattjaro = ter.Children.ToList().Find(G => G.GetName() == "tengeralattjaro") as ModelVisual3D;

        if (index < kordik.Count)
        {
            MoveToCoordinate(tengeralattjaro, kordik[index], () =>
            {
                MoveToNextCoordinate(index + 1, kordik); // Move to the next coordinate in the list
            });
        }
    }

    public static double CalculateDistance(Point3D point1, Point3D point2)
    {
        Vector3D vector = Point3D.Subtract(point2, point1);
        double distance = vector.Length;
        return distance;
    }
    #endregion
}