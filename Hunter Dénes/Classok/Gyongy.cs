using System.IO;
using System.Windows.Media.Media3D;

namespace Hunter_Dénes.Classok;
public struct Gyongy
{
    private static int fileHossz;
    public static Gyongy[] gyongyok;
    public Dictionary<int, double> szomszedok;

    public int Id { get; init; } = 0;
    public int X { get; init; } = 0;
    public int Y { get; init; } = 0;
    public int Z { get; init; } = 0;
    public int Ertek { get; init; } = 0;

    public Gyongy()
    {
        szomszedok = new(fileHossz - 1);
    }
    public Gyongy(int[] adatok, int id)
    {
        szomszedok = new(fileHossz - 1);
        Id = id;

        X = adatok[0];
        Y = adatok[1];
        Z = adatok[2];
        Ertek = adatok[3];
    }

    public static void Betolt(string path)
    {
        using (StreamReader sr = new(path))
        {
            string? sor;
            sr.ReadLine();

            fileHossz = File.ReadLines(path).Count();
            gyongyok = new Gyongy[fileHossz];
            gyongyok[0] = new();

            int id = 1;
            while ((sor = sr.ReadLine()) is not null)
            {
                Gyongy gyongy = new(Array.ConvertAll(sor.TrimEnd(';').Split(';'), Convert.ToInt32), id);

                for (int i = 0; i < id; i++)
                {
                    double tavolsag = Távolság(gyongyok[i], gyongy);
                    gyongyok[i].szomszedok.Add(gyongy.Id, tavolsag);
                    gyongy.szomszedok.Add(gyongyok[i].Id, tavolsag);
                }

                gyongyok[id] = gyongy;
                id++;
            }

            sr.Close();
        }
    }

    public static void Betolt(Func<int> x, Func<int> y, Func<int> z, Func<int> ertek, int gyongyokSzama)
    {
        int kimaradt = 0;
        gyongyok = new Gyongy[fileHossz = gyongyokSzama];
        gyongyok[0] = new();

        for (int id = 1; id < fileHossz; id++)
        {
            Gyongy gyongy = new([x(), y(), z(), ertek()], id);

            for (int i = 0; i < id; i++)
            {
                if (gyongy.X == gyongyok[i].X && gyongy.Y == gyongyok[i].Y && gyongy.Z == gyongyok[i].Z)
                {
                    kimaradt++;
                    break;
                }

                gyongyok[id-kimaradt] = gyongy;
            }
        }

        if(kimaradt != 0)
        {
            fileHossz -= kimaradt;
            Array.Resize(ref gyongyok, fileHossz);
        }

        for (int id = 1; id < fileHossz; id++)
        { 
            for (int i = 0; i < id; i++)
            {
                double tavolsag = Távolság(gyongyok[i], gyongyok[id]);
                gyongyok[i].szomszedok.Add(id, tavolsag);
                gyongyok[id].szomszedok.Add(gyongyok[i].Id, tavolsag);
            }
        }
    }
    public static double Távolság(Gyongy a, Gyongy b)
    {
        double temp = a.X - b.X;
        double x = temp * temp;

        temp = a.Y - b.Y;
        double y = temp * temp;

        temp = a.Z - b.Z;
        double z = temp * temp;

        return Math.Sqrt(x + y + z);
    }
    public static Point3D Pont(Gyongy gyongy, int xEltolas = 0, int yEltolas = 0, int zEltolas = 0)
    {
        return new(gyongy.X * -2 + xEltolas,
                   gyongy.Y * 2 + yEltolas,
                   gyongy.Z * -2 + zEltolas);
    }

    public override string ToString() => $"({X};{Y};{Z}) {Ertek} Zed";
}