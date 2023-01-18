namespace avranic_zadaca_1.Sucelja;

public class RezervacijskaZauzetost : IZauzetost
{
    public Brod Brod { get; }
    private Vez Vez { get; }
    public DateTime RezerviranOd { get; }
    public DateTime RezerviranDo { get; }

    public bool ZauzetURasponu(DateTime datumOd, DateTime datumDo)
    {
        return ((IZauzetost) this).DatumiSePreklapaju(datumOd, datumDo, RezerviranOd, RezerviranDo);
    }

    public bool ZauzetURasponu(DateTime datumOd,
                               DateTime datumDo,
                               out List<Tuple<int, DateTime, DateTime>> rezerviraniRasponi)
    {
        if (ZauzetURasponu(datumOd, datumDo)) {
            rezerviraniRasponi = new List<Tuple<int, DateTime, DateTime>> { new(Vez.Id, RezerviranOd, RezerviranDo) };
            return true;
        }

        rezerviraniRasponi = null!;
        return false;
    }

    public RezervacijskaZauzetost(IParametriZauzetosti parametriZauzetosti)
    {
        var prz = (ParametriRezervacijskeZauzetosti) parametriZauzetosti;

        Vez = prz.Vez;
        Brod = prz.Brod;
        RezerviranOd = prz.RezerviranOd;
        RezerviranDo = prz.RezerviranDo;
    }
}