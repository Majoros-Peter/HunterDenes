﻿namespace Hunter_Dénes;

    using HelixToolkit.Wpf;
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
    using static Robot;

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

        BetoltGyongyok();
    }

    private void BetoltGyongyok()
    {
        Betolt("gyongyok.txt");

        foreach(Gyongy gyongy in gyongyok)
        {
            EllipsoidVisual3D gyongy3d = new()
            {
                RadiusX = .05 * (gyongy.Ertek+1) + 1,
                RadiusY = .05 * (gyongy.Ertek+1) + 1,
                RadiusZ = .05 * (gyongy.Ertek+1) + 1,
                Center = new Point3D(-gyongy.X, gyongy.Y, -gyongy.Z),
                Fill = new SolidColorBrush(Color.FromArgb(255, (byte)(255 - gyongy.Ertek * 20), 80, (byte)(gyongy.Ertek * 20)))
            };

            gyongy3d.SetName(gyongy.Id.ToString());
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

    private void Inditas_Click(object sender, RoutedEventArgs e)
    {
        if(stopper.IsChecked is true)
        {
            Stopwatch sw = Stopwatch.StartNew();

            lbGyongyok.ItemsSource = new Robot(Convert.ToDouble(txtUthossz.Text)).AI(gyongyok[0]);

            sw.Stop();
            MessageBox.Show($"{sw.ElapsedMilliseconds} milliseconds");
        }
        else
        {
            lbGyongyok.ItemsSource = new Robot(Convert.ToDouble(txtUthossz.Text)).AI(gyongyok[0]);
        }

        foreach (Gyongy gyongy in lbGyongyok.Items)
            (ter.Children.First(G => G.GetName() == gyongy.Id.ToString()) as EllipsoidVisual3D).Fill = new SolidColorBrush(Colors.Green);
    }
}