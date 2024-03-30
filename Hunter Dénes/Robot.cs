namespace Hunter_Dénes;

using System.Windows.Controls;
using static Gyongy;

public class Robot
{
    private static double UTHOSSZ = 50;
    private static Gyongy ORIGO;
    private static Func<Gyongy, HashSet<Gyongy>, IEnumerable<Gyongy>, double, IList<Gyongy>>? Algoritmus;

    public double X { get; private set; } = 0;
    public double Y { get; private set; } = 0;
    public double Z { get; private set; } = 0;

    public Robot(double uthossz, Gyongy kezdopont, string algoritmusIndex)
    {
        UTHOSSZ = uthossz;
        ORIGO = kezdopont;

        Algoritmus = algoritmusIndex switch
        {
            "Chad" => AtlagosAlgoritmus,
            "Griddy" => Greedy,
            _ => DFS
        };
    }

    public IList<Gyongy> AI()
    {
        HashSet<Gyongy> visited = [ORIGO];
        IList<Gyongy> optimalis = [ORIGO];
        IEnumerable<Gyongy> szukitett = gyongyok.Skip(1).Where(G => ORIGO.szomszedok[G.Id] * 2 <= UTHOSSZ);

        Parallel.ForEach(szukitett, gyongy => {
            double tavolsag = ORIGO.szomszedok[gyongy.Id];
            if(tavolsag * 2 <= UTHOSSZ)
            {
                IList<Gyongy> temp = Algoritmus(gyongy, visited, szukitett, tavolsag);
                if(temp.Sum(G => G.Ertek) > optimalis.Sum(G => G.Ertek))
                    optimalis = temp;
            }
        });

        return optimalis;
    }
    private static IList<Gyongy> DFS(Gyongy kiindulo, HashSet<Gyongy> _visited, IEnumerable<Gyongy> szukitett, double megtettUt)
    {
        HashSet<Gyongy> visited = new(_visited) { kiindulo };
        IList<Gyongy> optimalis = [kiindulo];
        IEnumerable<Gyongy> tovabbSzukitett = szukitett.Where(G => !visited.Contains(G) && megtettUt + kiindulo.szomszedok[G.Id] + ORIGO.szomszedok[G.Id] <= UTHOSSZ);

        foreach(var gyongy in tovabbSzukitett)
        {
            double tavolsag = kiindulo.szomszedok[gyongy.Id];

            IList<Gyongy> temp = DFS(gyongy, visited, tovabbSzukitett, megtettUt + tavolsag);
            if(temp.Sum(G => G.Ertek) + kiindulo.Ertek > optimalis.Sum(G => G.Ertek))
            {
                temp.Add(kiindulo);
                optimalis = temp;
            }
        }

        return optimalis;
    }
    private static IList<Gyongy> AtlagosAlgoritmus(Gyongy kiindulo, HashSet<Gyongy> _visited, IEnumerable<Gyongy> szukitett, double megtettUt)
    {
        HashSet<Gyongy> visited = new(_visited) { kiindulo };
        IList<Gyongy> optimalis = [kiindulo];
        IEnumerable<Gyongy> tovabbSzukitett = szukitett.Where(G => !visited.Contains(G) && megtettUt + kiindulo.szomszedok[G.Id] + ORIGO.szomszedok[G.Id] <= UTHOSSZ);

        if(!tovabbSzukitett.Any())
            return optimalis;

        double atlag = tovabbSzukitett.Average(G => G.Ertek / kiindulo.szomszedok[G.Id]);

        foreach(var gyongy in tovabbSzukitett)
        {
            if(gyongy.Ertek / kiindulo.szomszedok[gyongy.Id] < atlag)
                continue;

            IList<Gyongy> temp = AtlagosAlgoritmus(gyongy, visited, tovabbSzukitett, megtettUt + kiindulo.szomszedok[gyongy.Id]);
            if(temp.Sum(G => G.Ertek) + kiindulo.Ertek > optimalis.Sum(G => G.Ertek))
            {
                temp.Add(kiindulo);
                optimalis = temp;
            }
        }

        return optimalis;
    }
    private static IList<Gyongy> Greedy(Gyongy kiindulo, HashSet<Gyongy> _visited, IEnumerable<Gyongy> szukitett, double megtettUt)
    {
        HashSet<Gyongy> visited = new(_visited) { kiindulo };
        IList<Gyongy> optimalis = [kiindulo];
        IEnumerable<Gyongy> tovabbSzukitett = szukitett.Where(G => !visited.Contains(G) && megtettUt + kiindulo.szomszedok[G.Id] + ORIGO.szomszedok[G.Id] <= UTHOSSZ);

        foreach(var gyongy in LegnagyobbNGyongy(kiindulo, tovabbSzukitett, 25))
        {
            double tavolsag = kiindulo.szomszedok[gyongy.Id];

            IList<Gyongy> temp = Greedy(gyongy, visited, tovabbSzukitett, megtettUt + tavolsag);
            if(temp.Sum(G => G.Ertek) + kiindulo.Ertek > optimalis.Sum(G => G.Ertek))
            {
                temp.Add(kiindulo);
                optimalis = temp;
            }
        }

        return optimalis;
    }

    private static IEnumerable<Gyongy> LegnagyobbNGyongy(Gyongy kiindulo, IEnumerable<Gyongy> gyongyok, int n) => gyongyok.OrderBy(G => kiindulo.szomszedok[G.Id]).Take(Math.Min(n, gyongyok.Count()));
}