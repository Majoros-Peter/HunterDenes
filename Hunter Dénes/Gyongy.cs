using System.IO;

namespace Hunter_Dénes;
public struct Gyongy
{
    public static int fileHossz;
    public static Gyongy[] gyongyok;
    public Dictionary<int, double> szomszedok;

    public int Id { get; init; } = 0;
    public byte X { get; init; } = 0;
    public byte Y { get; init; } = 0;
    public byte Z { get; init; } = 0;
    public byte Ertek { get; init; } = 0;

    public Gyongy()
    {
        szomszedok = new(fileHossz - 1);
    }
    public Gyongy(byte[] adatok, int id)
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
        using(StreamReader sr = new(path))
        {
            string? sor;
            sr.ReadLine();

            fileHossz = File.ReadLines(path).Count();
            gyongyok = new Gyongy[fileHossz];
            gyongyok[0] = new();

            int id = 1;
            while((sor = sr.ReadLine()) is not null)
            {
                Gyongy gyongy = new(Array.ConvertAll(sor.TrimEnd(';').Split(';'), Convert.ToByte), id);

                for(int i = 0; i < id; i++)
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
    public static void Betolt(Random rand)
    {
        fileHossz = rand.Next(50, 221);

        gyongyok = new Gyongy[fileHossz];
        gyongyok[0] = new();

        for(int id = 1; id < fileHossz; id++)
        {
            Gyongy gyongy = new([(byte)(rand.Next(40) * 2), (byte)(rand.Next(40) * 2), (byte)(rand.Next(30) * 2), (byte)(rand.Next(9) + 1)], id);

            for(int i = 0; i < id; i++)
            {
                double tavolsag = Távolság(gyongyok[i], gyongy);
                gyongyok[i].szomszedok.Add(gyongy.Id, tavolsag);
                gyongy.szomszedok.Add(gyongyok[i].Id, tavolsag);
            }

            gyongyok[id] = gyongy;
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

    public override string ToString() => $"({X};{Y};{Z}) {Ertek} Mihazánk fitying";
}