namespace Hunter_Dénes.Classok;

using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using static Gyongy;

public class Robot
{
    private static double UTHOSSZ = 50;
    private static Gyongy ORIGO;
    private static Func<Gyongy, IEnumerable<Gyongy>, double, int, IList<Gyongy>>? Algoritmus;

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
            "Gyorsitott" => GyorsabbGreedy,
            "Hybrid" => GyorsAtlagHybrid,
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
                IList<Gyongy> temp = Algoritmus(gyongy, szukitett, tavolsag, int.MinValue);
                if (temp.Sum(G => G.Ertek) > optimalis.Sum(G => G.Ertek))
                    optimalis = temp;
            }
        });

        return optimalis;
    }
    private static IList<Gyongy> DFS(Gyongy kiindulo, IEnumerable<Gyongy> szukitett, double megtettUt, int melyseg = int.MinValue)
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
    private static IList<Gyongy> AtlagosAlgoritmus(Gyongy kiindulo, IEnumerable<Gyongy> szukitett, double megtettUt, int melyseg = int.MinValue)
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
    private static IList<Gyongy> Greedy(Gyongy kiindulo, IEnumerable<Gyongy> szukitett, double megtettUt, int melyseg = int.MinValue)
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
    private static IList<Gyongy> GyorsabbGreedy(Gyongy kiindulo, IEnumerable<Gyongy> szukitett, double megtettUt, int melyseg = int.MinValue)
    {
        IEnumerable<Gyongy> tovabbSzukitett = szukitett.Where(G => G.Id != kiindulo.Id && megtettUt + kiindulo.szomszedok[G.Id] + ORIGO.szomszedok[G.Id] <= UTHOSSZ);

        if(!tovabbSzukitett.Any())
            return [kiindulo];

        Gyongy gyongy = tovabbSzukitett.Where(G => G.Ertek == tovabbSzukitett.Max(G => G.Ertek))
                                       .OrderBy(G => kiindulo.szomszedok[G.Id])
                                       .First();

        return [..GyorsabbGreedy(gyongy, tovabbSzukitett, megtettUt + kiindulo.szomszedok[gyongy.Id], melyseg+1), kiindulo];
    }
    private static IList<Gyongy> GyorsAtlagHybrid(Gyongy kiindulo, IEnumerable<Gyongy> szukitett, double megtettUt, int melyseg = int.MinValue)
    {
        melyseg = melyseg==int.MinValue ? 1 : melyseg;

        IList<Gyongy> optimalis = [kiindulo];
        IEnumerable<Gyongy> tovabbSzukitett = szukitett.Where(G => G.Id != kiindulo.Id && megtettUt + kiindulo.szomszedok[G.Id] + ORIGO.szomszedok[G.Id] <= UTHOSSZ);

        if(!tovabbSzukitett.Any())
            return optimalis;

        double atlag = tovabbSzukitett.Average(G => G.Ertek / kiindulo.szomszedok[G.Id]);

        if(melyseg % 3 == 0)
        {
            foreach(var gyongy in tovabbSzukitett)
            {
                if(gyongy.Ertek / kiindulo.szomszedok[gyongy.Id] < atlag)
                    continue;

                IList<Gyongy> temp = GyorsAtlagHybrid(gyongy, tovabbSzukitett, megtettUt + kiindulo.szomszedok[gyongy.Id], melyseg+1);

                if(temp.Sum(G => G.Ertek) + kiindulo.Ertek > optimalis.Sum(G => G.Ertek))
                {
                    temp.Add(kiindulo);
                    optimalis = temp;
                }
            }
        } else
        {
            Gyongy gyongy = tovabbSzukitett.Where(G => G.Ertek == tovabbSzukitett.Max(G => G.Ertek))
                                       .OrderBy(G => kiindulo.szomszedok[G.Id])
                                       .First();

            optimalis = [.. GyorsabbGreedy(gyongy, tovabbSzukitett, megtettUt + kiindulo.szomszedok[gyongy.Id], melyseg+1), kiindulo];
        }

        return optimalis;

        #region __Módosított Greedy__
        IList<Gyongy> Greedy(Gyongy kiindulo, IEnumerable<Gyongy> szukitett, double megtettUt)
        {
            IEnumerable<Gyongy> tovabbSzukitett = szukitett.Where(G => G.Id != kiindulo.Id && megtettUt + kiindulo.szomszedok[G.Id] + ORIGO.szomszedok[G.Id] <= UTHOSSZ);

            if(!tovabbSzukitett.Any())
                return [kiindulo];

            Gyongy gyongy = tovabbSzukitett.Where(G => G.Ertek == tovabbSzukitett.Max(G => G.Ertek))
                                           .OrderBy(G => kiindulo.szomszedok[G.Id])
                                           .First();

            IList<Gyongy> temp = Greedy(gyongy, tovabbSzukitett, megtettUt + kiindulo.szomszedok[gyongy.Id]);

            return [..temp, kiindulo];
        }
        #endregion
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