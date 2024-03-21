namespace Hunter_Dénes;

using static Gyongy;

public class Robot
{
    private static double UTHOSSZ = 100;
    private static Gyongy ORIGO;

    public double X { get; private set; } = 0;
    public double Y { get; private set; } = 0;
    public double Z { get; private set; } = 0;

    public Robot() { }

    public List<Gyongy> AI(Gyongy kezdopont)
    {
        ORIGO = kezdopont;
        HashSet<Gyongy> visited = [kezdopont];
        List<Gyongy> optimalis = [kezdopont];
        IEnumerable<Gyongy> szukitett = gyongyok.Skip(1).Where(G => ORIGO.szomszedok[G.Id] * 2 <= UTHOSSZ);

        Parallel.ForEach(szukitett, gyongy => {
            double tavolsag = ORIGO.szomszedok[gyongy.Id];
            if(tavolsag * 2 <= UTHOSSZ)
            {
                List<Gyongy> temp = DFS(gyongy, visited, szukitett, tavolsag);
                if(temp.Sum(G => G.Ertek) > optimalis.Sum(G => G.Ertek))
                    optimalis = temp;
            }
        });

        return optimalis;
    }

    private List<Gyongy> DFS(Gyongy kiindulo, HashSet<Gyongy> _visited, IEnumerable<Gyongy> szukitett, double megtettUt)
    {
        HashSet<Gyongy> visited = new(_visited) { kiindulo };
        List<Gyongy> optimalis = [kiindulo];
        IEnumerable<Gyongy> tovabbSzukitett = szukitett.Where(G => !visited.Contains(G) && megtettUt+kiindulo.szomszedok[G.Id]+ORIGO.szomszedok[G.Id] <= UTHOSSZ);

        foreach(var gyongy in tovabbSzukitett)
        {
            double tavolsag = kiindulo.szomszedok[gyongy.Id];

            List<Gyongy> temp = DFS(gyongy, visited, tovabbSzukitett, megtettUt + tavolsag);
            if(temp.Sum(G => G.Ertek) + kiindulo.Ertek > optimalis.Sum(G => G.Ertek))
            {
                temp.Add(kiindulo);
                optimalis = temp;
            }
        }

        return optimalis;
    }


    public List<Gyongy> Greedy(Gyongy kezdopont)
    {
        return [];
    }

    private List<Gyongy> Djikstra(Gyongy kezdopont)
    {
        return [];
    }
}