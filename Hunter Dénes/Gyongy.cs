using System.IO;

namespace Hunter_Dénes;
public class Gyongy
{
    public static readonly List<Gyongy> gyongyok = [];
    //public static Dictionary<(Gyongy, Gyongy), double> szomszedok = [];
    public Dictionary<int, double> szomszedok = [];

    public int Id { get; init; }
    public int X { get; init; } = 0;
    public int Y { get; init; } = 0;
    public int Z { get; init; } = 0;
    public int Ertek { get; init; } = 0;

    private Gyongy() { Id = 0; }
    public Gyongy(int[] adatok, int id)
    {
        Id = id;

        X = adatok[0];
        Y = adatok[1];
        Z = adatok[2];
        Ertek = adatok[3];
    }

    public static void Betolt(string path)
    {
        gyongyok.Add(new());

        using (StreamReader sr = new(path))
        {
            sr.ReadLine();

            int id = 1;
            while (sr.Peek() > 0)
            {
                Gyongy gyongy = new(Array.ConvertAll(sr.ReadLine().TrimEnd(';').Split(';'), Convert.ToInt32), id++);

                //gyongyok.ForEach(G => szomszedok.Add(DictKey(G, gyongy), Távolság(G, gyongy)));
                gyongyok.ForEach(G => {
                    double tavolsag = Távolság(G, gyongy);
                    G.szomszedok.Add(gyongy.Id, tavolsag);
                    gyongy.szomszedok.Add(G.Id, tavolsag);
                });

                gyongyok.Add(gyongy);
            }

            sr.Close();
        }
    }
    public static double Távolság(Gyongy a, Gyongy b)
    {
        double x = Math.Pow(a.X - b.X, 2);
        double y = Math.Pow(a.Y - b.Y, 2);
        double z = Math.Pow(a.Z - b.Z, 2);

        return Math.Sqrt(x + y + z);
    }
    public static (Gyongy, Gyongy) DictKey(Gyongy a, Gyongy b) => a.Id<b.Id ? (a, b) : (b, a);

    public override string ToString() => $"({X};{Y};{Z}) {Ertek} Mihazánk fitying";
}