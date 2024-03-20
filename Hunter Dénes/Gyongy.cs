using System.IO;

namespace Hunter_Dénes;
public class Gyongy
{
    public static readonly List<Gyongy> gyongyok = [];

    public int X { get; init; } = 0;
    public int Y { get; init; } = 0;
    public int Z { get; init; } = 0;
    public int Ertek { get; init; } = 0;

    private Gyongy() { }
    public Gyongy(int[] adatok)
    {
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

            while (sr.Peek() > 0)
                gyongyok.Add(new(Array.ConvertAll(sr.ReadLine().TrimEnd(';').Split(';'), Convert.ToInt32)));

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

    public override string ToString() => $"({X};{Y};{Z}) {Ertek} Mihazánk fitying";
}

public class Robot
{
    public double X { get; private set; } = 0;
    public double Y { get; private set; } = 0;
    public double Z { get; private set; } = 0;

    public void AI(Gyongy kezdopont, HashSet<Gyongy> _visited)
    {
        HashSet<Gyongy> visited = new(_visited) { kezdopont };

        Parallel.ForEach(Gyongy.gyongyok.Except(visited), gyongy => {
            if (!visited.Contains(gyongy))
            {
                DFS(gyongy, visited);
                Console.WriteLine($"{gyongy}");
            }
        });
    }

    private void DFS(Gyongy kezdopont, HashSet<Gyongy> _visited, byte melyseg = 0)
    {
        HashSet<Gyongy> visited = new(_visited) { kezdopont };

        foreach (var gyongy in Gyongy.gyongyok.Except(visited))
        {
            if (!visited.Contains(gyongy))
                DFS(gyongy, visited, (byte)(melyseg + 1));
        }
    }
}