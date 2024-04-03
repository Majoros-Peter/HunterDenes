namespace Hunter_Dénes.Classok;

using static Gyongy;

public class Robot
{
    private static double UTHOSSZ;
    private static Gyongy ORIGO;

    public Robot(double uthossz, Gyongy kezdopont)
    {
        UTHOSSZ = uthossz;
        ORIGO = kezdopont;
    }
   
    public IList<Gyongy> AI()
    {
        IEnumerable<Gyongy> szukitett = gyongyok.Skip(1)
                                                .Where(G => ORIGO.szomszedok[G.Id] * 2 <= UTHOSSZ);
        int[] ertekek = szukitett.DistinctBy(G => G.Ertek)
                                  .Select(G => G.Ertek)
                                  .Where(G => G != 0)
                                  .ToArray();
        IList<Gyongy> optimalis = [ORIGO];

        foreach (int ertek in ertekek)
        {
            IList<Gyongy> gyongyok = LegjobbAlgo(ORIGO, szukitett.Where(G => G.Ertek >= ertek));
            if(gyongyok.Sum(G => G.Ertek) > optimalis.Sum(G => G.Ertek))
                optimalis = gyongyok;
        }

        return optimalis;
    }

    private static IList<Gyongy> LegjobbAlgo(Gyongy kiindulo, IEnumerable<Gyongy> szukitett, double megtettUt=0)
    {
        Gyongy gyongy = szukitett.MinBy(G => G.Id != kiindulo.Id ? kiindulo.szomszedok[G.Id] : 0);

        if(megtettUt + kiindulo.szomszedok[gyongy.Id] + ORIGO.szomszedok[gyongy.Id] > UTHOSSZ)
            return [kiindulo];

        IEnumerable<Gyongy> tovabbSzukitett = szukitett.Where(G => G.Id != gyongy.Id && megtettUt + kiindulo.szomszedok[G.Id] + ORIGO.szomszedok[G.Id] <= UTHOSSZ);

        if(!tovabbSzukitett.Any())
            return [kiindulo];

        return [..LegjobbAlgo(gyongy, tovabbSzukitett, megtettUt+ kiindulo.szomszedok[gyongy.Id]), kiindulo];
    }

    #region __Amik túl lassúk (és rosszak) voltak__
    //private static IList<Gyongy> DFS(Gyongy kiindulo, IEnumerable<Gyongy> szukitett, double megtettUt, int melyseg = int.MinValue)
    //{
    //    IList<Gyongy> optimalis = [kiindulo];
    //    IEnumerable<Gyongy> tovabbSzukitett = szukitett.Where(G => G.Id != kiindulo.Id && megtettUt + kiindulo.szomszedok[G.Id] + ORIGO.szomszedok[G.Id] <= UTHOSSZ);

    //    foreach(var gyongy in tovabbSzukitett)
    //    {
    //        double tavolsag = kiindulo.szomszedok[gyongy.Id];

    //        IList<Gyongy> temp = DFS(gyongy, tovabbSzukitett, megtettUt + tavolsag);
    //        if(temp.Sum(G => G.Ertek) + kiindulo.Ertek > optimalis.Sum(G => G.Ertek))
    //        {
    //            temp.Add(kiindulo);
    //            optimalis = temp;
    //        }
    //    }

    //    return optimalis;
    //}
    //private static IList<Gyongy> AtlagosAlgoritmus(Gyongy kiindulo, IEnumerable<Gyongy> szukitett, double megtettUt, int melyseg = int.MinValue)
    //{
    //    IList<Gyongy> optimalis = [kiindulo];
    //    IEnumerable<Gyongy> tovabbSzukitett = szukitett.Where(G => G.Id != kiindulo.Id && megtettUt + kiindulo.szomszedok[G.Id] + ORIGO.szomszedok[G.Id] <= UTHOSSZ);

    //    if(!tovabbSzukitett.Any())
    //        return optimalis;

    //    double atlag = tovabbSzukitett.Average(G => G.Ertek / kiindulo.szomszedok[G.Id]);

    //    foreach(var gyongy in tovabbSzukitett)
    //    {
    //        if(gyongy.Ertek / kiindulo.szomszedok[gyongy.Id] < atlag)
    //            continue;

    //        IList<Gyongy> temp = AtlagosAlgoritmus(gyongy, tovabbSzukitett, megtettUt + kiindulo.szomszedok[gyongy.Id]);
    //        if(temp.Sum(G => G.Ertek) + kiindulo.Ertek > optimalis.Sum(G => G.Ertek))
    //        {
    //            temp.Add(kiindulo);
    //            optimalis = temp;
    //        }
    //    }

    //    return optimalis;
    //}
    //private static IList<Gyongy> Greedy(Gyongy kiindulo, IEnumerable<Gyongy> szukitett, double megtettUt, int melyseg = int.MinValue)
    //{
    //    IList<Gyongy> optimalis = [kiindulo];
    //    IEnumerable<Gyongy> tovabbSzukitett = szukitett.Where(G => G.Id != kiindulo.Id && megtettUt + kiindulo.szomszedok[G.Id] + ORIGO.szomszedok[G.Id] <= UTHOSSZ);

    //    if(!tovabbSzukitett.Any())
    //        return optimalis;

    //    foreach(var gyongy in GreedySzukit(tovabbSzukitett))
    //    {
    //        IList<Gyongy> temp = Greedy(gyongy, tovabbSzukitett, megtettUt + kiindulo.szomszedok[gyongy.Id]);
    //        if(temp.Sum(G => G.Ertek) + kiindulo.Ertek > optimalis.Sum(G => G.Ertek))
    //        {
    //            temp.Add(kiindulo);
    //            optimalis = temp;
    //        }
    //    }

    //    return optimalis;
    //}
    //private static IList<Gyongy> GyorsabbGreedy(Gyongy kiindulo, IEnumerable<Gyongy> szukitett, double megtettUt, int melyseg = int.MinValue)
    //{
    //    IEnumerable<Gyongy> tovabbSzukitett = szukitett.Where(G => G.Id != kiindulo.Id && megtettUt + kiindulo.szomszedok[G.Id] + ORIGO.szomszedok[G.Id] <= UTHOSSZ);

    //    if(!tovabbSzukitett.Any())
    //        return [kiindulo];

    //    Gyongy gyongy = tovabbSzukitett.Where(G => G.Ertek == tovabbSzukitett.Max(G => G.Ertek))
    //                                   .OrderBy(G => kiindulo.szomszedok[G.Id])
    //                                   .First();

    //    return [.. GyorsabbGreedy(gyongy, tovabbSzukitett, megtettUt + kiindulo.szomszedok[gyongy.Id], melyseg + 1), kiindulo];
    //}
    //private static IList<Gyongy> GyorsAtlagHybrid(Gyongy kiindulo, IEnumerable<Gyongy> szukitett, double megtettUt, int melyseg = int.MinValue)
    //{
    //    melyseg = melyseg == int.MinValue ? 1 : melyseg;

    //    IList<Gyongy> optimalis = [kiindulo];
    //    IEnumerable<Gyongy> tovabbSzukitett = szukitett.Where(G => G.Id != kiindulo.Id && megtettUt + kiindulo.szomszedok[G.Id] + ORIGO.szomszedok[G.Id] <= UTHOSSZ);

    //    if(!tovabbSzukitett.Any())
    //        return optimalis;

    //    double atlag = tovabbSzukitett.Average(G => G.Ertek / kiindulo.szomszedok[G.Id]);

    //    if(melyseg % 3 == 0)
    //    {
    //        foreach(var gyongy in tovabbSzukitett)
    //        {
    //            if(gyongy.Ertek / kiindulo.szomszedok[gyongy.Id] < atlag)
    //                continue;

    //            IList<Gyongy> temp = GyorsAtlagHybrid(gyongy, tovabbSzukitett, megtettUt + kiindulo.szomszedok[gyongy.Id], melyseg + 1);

    //            if(temp.Sum(G => G.Ertek) + kiindulo.Ertek > optimalis.Sum(G => G.Ertek))
    //            {
    //                temp.Add(kiindulo);
    //                optimalis = temp;
    //            }
    //        }
    //    } else
    //    {
    //        Gyongy gyongy = tovabbSzukitett.Where(G => G.Ertek == tovabbSzukitett.Max(G => G.Ertek))
    //                                   .OrderBy(G => kiindulo.szomszedok[G.Id])
    //                                   .First();

    //        optimalis = [.. GyorsabbGreedy(gyongy, tovabbSzukitett, megtettUt + kiindulo.szomszedok[gyongy.Id], melyseg + 1), kiindulo];
    //    }

    //    return optimalis;

    //    #region __Módosított Greedy__
    //    IList<Gyongy> Greedy(Gyongy kiindulo, IEnumerable<Gyongy> szukitett, double megtettUt)
    //    {
    //        IEnumerable<Gyongy> tovabbSzukitett = szukitett.Where(G => G.Id != kiindulo.Id && megtettUt + kiindulo.szomszedok[G.Id] + ORIGO.szomszedok[G.Id] <= UTHOSSZ);

    //        if(!tovabbSzukitett.Any())
    //            return [kiindulo];

    //        Gyongy gyongy = tovabbSzukitett.Where(G => G.Ertek == tovabbSzukitett.Max(G => G.Ertek))
    //                                       .OrderBy(G => kiindulo.szomszedok[G.Id])
    //                                       .First();

    //        IList<Gyongy> temp = Greedy(gyongy, tovabbSzukitett, megtettUt + kiindulo.szomszedok[gyongy.Id]);

    //        return [.. temp, kiindulo];
    //    }
    //    #endregion
    //}


    //private static IEnumerable<Gyongy> GreedySzukit(IEnumerable<Gyongy> gyongyok)
    //{
    //    int max = gyongyok.Max(G => G.Ertek);

    //    foreach(Gyongy gyongy in gyongyok)
    //    {
    //        if(gyongy.Ertek == max)
    //        {
    //            yield return gyongy;
    //        }
    //    }
    //}
    #endregion
}