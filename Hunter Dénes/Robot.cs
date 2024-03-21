namespace Hunter_Dénes;

using static Gyongy;

public class Robot
{
    private static double UTHOSSZ = 140;
    private static Gyongy ORIGO;

    public double X { get; private set; } = 0;
    public double Y { get; private set; } = 0;
    public double Z { get; private set; } = 0;

    public Robot() { }

    //public List<Gyongy> AI(Gyongy kezdopont, HashSet<Gyongy> _visited)
    //{
    //    ORIGO = kezdopont;
    //    HashSet<Gyongy> visited = [kezdopont];
    //    List<Gyongy> optimalis = [kezdopont];

    //    foreach(Gyongy gyongy in gyongyok.Skip(1).Where(G => !visited.Contains(G) && szomszedok[(ORIGO, G)] * 2 <= UTHOSSZ))
    //    {
    //        double tavolsag = szomszedok[(ORIGO, gyongy)];
    //        if(tavolsag * 2 <= UTHOSSZ)
    //        {
    //            List<Gyongy> temp = DFS(gyongy, visited, tavolsag);
    //            if(temp.Sum(G => G.Ertek) > optimalis.Sum(G => G.Ertek))
    //                optimalis = temp;
    //        }
    //    }

    //    return optimalis;
    //}

    //private List<Gyongy> DFS(Gyongy kiindulo, HashSet<Gyongy> _visited, double megtettUt)
    //{
    //    HashSet<Gyongy> visited = new(_visited) { kiindulo };
    //    List<Gyongy> optimalis = [kiindulo];

    //    foreach(var gyongy in gyongyok)
    //    {
    //        if(!visited.Contains(gyongy))
    //        {
    //            double tavolsag = szomszedok[DictKey(kiindulo, gyongy)];
    //            if(megtettUt + tavolsag + szomszedok[(ORIGO, gyongy)] <= UTHOSSZ)
    //            {
    //                List<Gyongy> temp = DFS(gyongy, visited, megtettUt + tavolsag);
    //                if(temp.Sum(G => G.Ertek) > optimalis.Sum(G => G.Ertek))
    //                    optimalis = temp;
    //            }
    //        }
    //    }

    //    return optimalis;
    //}

    public List<Gyongy> AI(Gyongy kezdopont, HashSet<Gyongy> _visited)
    {
        ORIGO = kezdopont;
        HashSet<Gyongy> visited = [kezdopont];
        List<Gyongy> optimalis = [kezdopont];

        foreach(Gyongy gyongy in ForCiklus(G => !visited.Contains(G) && ORIGO.szomszedok[G.Id] * 2 <= UTHOSSZ))
        {
            double tavolsag = ORIGO.szomszedok[gyongy.Id];
            if(tavolsag * 2 <= UTHOSSZ)
            {
                List<Gyongy> temp = DFS(gyongy, visited, tavolsag);
                if(temp.Sum(G => G.Ertek) > optimalis.Sum(G => G.Ertek))
                    optimalis = temp;
            }
        }

        return optimalis;
    }

    private List<Gyongy> DFS(Gyongy kiindulo, HashSet<Gyongy> _visited, double megtettUt)
    {
        HashSet<Gyongy> visited = new(_visited) { kiindulo };
        List<Gyongy> optimalis = [kiindulo];

        foreach(var gyongy in ForCiklus(G => !visited.Contains(G)))
        {
            double tavolsag = kiindulo.szomszedok[gyongy.Id];
            if(megtettUt + tavolsag + ORIGO.szomszedok[gyongy.Id] <= UTHOSSZ)
            {
                List<Gyongy> temp = DFS(gyongy, visited, megtettUt + tavolsag);
                temp.Insert(0, kiindulo);
                if(temp.Sum(G => G.Ertek) > optimalis.Sum(G => G.Ertek))
                    optimalis = temp;
            }
        }

        return optimalis;
    }

    private void Greedy(Gyongy kezdopont, HashSet<Gyongy> _visited)
    {

    }

    private void Djikstra(Gyongy kezdopont, HashSet<Gyongy> _visited)
    {

    }

    private IEnumerable<Gyongy> ForCiklus(Func<Gyongy, bool> predicate)
    {
        foreach (Gyongy gyongy in gyongyok)
        {
            if(predicate(gyongy))
                yield return gyongy;
        }
    }
}