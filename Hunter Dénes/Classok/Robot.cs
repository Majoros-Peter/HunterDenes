namespace Hunter_Dénes.Classok;

using System.Windows.Controls;
using static Gyongy;

public class Robot
{
    private static double UTHOSSZ = 50;
    private static Gyongy ORIGO;
    private static Func<Gyongy, IEnumerable<Gyongy>, double, IList<Gyongy>>? Algoritmus;

    public double X { get; private set; } = 0;
    public double Y { get; private set; } = 0;
    public double Z { get; private set; } = 0;

    public Robot(double uthossz, Gyongy kezdopont, object algoritmusIndex)
    {
        UTHOSSZ = uthossz;
        ORIGO = kezdopont;

        Algoritmus = algoritmusIndex.ToString() switch
        {
            "Atlagos" => AtlagosAlgoritmus,
            "Greedy" => Greedy,
            _ => DFS
        };
    }

    public IList<Gyongy> AI()
    {
        IList<Gyongy> optimalis = [ORIGO];
        IEnumerable<Gyongy> szukitett = gyongyok.Skip(1).Where(G => ORIGO.szomszedok[G.Id] * 2 <= UTHOSSZ);

        Parallel.ForEach(szukitett, gyongy =>
        {
            double tavolsag = ORIGO.szomszedok[gyongy.Id];
            if (tavolsag * 2 <= UTHOSSZ)
            {
                IList<Gyongy> temp = Algoritmus(gyongy, szukitett, tavolsag);
                if (temp.Sum(G => G.Ertek) > optimalis.Sum(G => G.Ertek))
                    optimalis = temp;
            }
        });

        return optimalis;
    }
    private static IList<Gyongy> DFS(Gyongy kiindulo, IEnumerable<Gyongy> szukitett, double megtettUt)
    {
        IList<Gyongy> optimalis = [kiindulo];
        IEnumerable<Gyongy> tovabbSzukitett = szukitett.Where(G => G.Id != kiindulo.Id && megtettUt + kiindulo.szomszedok[G.Id] + ORIGO.szomszedok[G.Id] <= UTHOSSZ);

        foreach (var gyongy in tovabbSzukitett)
        {
            double tavolsag = kiindulo.szomszedok[gyongy.Id];

            IList<Gyongy> temp = DFS(gyongy, tovabbSzukitett, megtettUt + tavolsag);
            if (temp.Sum(G => G.Ertek) + kiindulo.Ertek > optimalis.Sum(G => G.Ertek))
            {
                temp.Add(kiindulo);
                optimalis = temp;
            }
        }

        return optimalis;
    }
    private static IList<Gyongy> AtlagosAlgoritmus(Gyongy kiindulo, IEnumerable<Gyongy> szukitett, double megtettUt)
    {
        IList<Gyongy> optimalis = [kiindulo];
        IEnumerable<Gyongy> tovabbSzukitett = szukitett.Where(G => G.Id != kiindulo.Id && megtettUt + kiindulo.szomszedok[G.Id] + ORIGO.szomszedok[G.Id] <= UTHOSSZ);

        if (!tovabbSzukitett.Any())
            return optimalis;

        double atlag = tovabbSzukitett.Average(G => G.Ertek / kiindulo.szomszedok[G.Id]);

        foreach (var gyongy in tovabbSzukitett)
        {
            if (gyongy.Ertek / kiindulo.szomszedok[gyongy.Id] < atlag)
                continue;

            IList<Gyongy> temp = AtlagosAlgoritmus(gyongy, tovabbSzukitett, megtettUt + kiindulo.szomszedok[gyongy.Id]);
            if (temp.Sum(G => G.Ertek) + kiindulo.Ertek > optimalis.Sum(G => G.Ertek))
            {
                temp.Add(kiindulo);
                optimalis = temp;
            }
        }

        return optimalis;
    }
    private static IList<Gyongy> Greedy(Gyongy kiindulo, IEnumerable<Gyongy> szukitett, double megtettUt)
    {
        IList<Gyongy> optimalis = [kiindulo];
        IEnumerable<Gyongy> tovabbSzukitett = szukitett.Where(G => G.Id != kiindulo.Id && megtettUt + kiindulo.szomszedok[G.Id] + ORIGO.szomszedok[G.Id] <= UTHOSSZ);

        if (!tovabbSzukitett.Any())
            return optimalis;

        foreach (var gyongy in GreedySzukit(tovabbSzukitett))
        {
            IList<Gyongy> temp = Greedy(gyongy, tovabbSzukitett, megtettUt + kiindulo.szomszedok[gyongy.Id]);
            if (temp.Sum(G => G.Ertek) + kiindulo.Ertek > optimalis.Sum(G => G.Ertek))
            {
                temp.Add(kiindulo);
                optimalis = temp;
            }
        }

        return optimalis;
    }

    private static IEnumerable<Gyongy> GreedySzukit(IEnumerable<Gyongy> gyongyok)
    {
        int max = gyongyok.Max(G => G.Ertek);

        foreach (Gyongy gyongy in gyongyok)
        {
            if (gyongy.Ertek == max)
            {
                yield return gyongy;
            }
        }
    }
}