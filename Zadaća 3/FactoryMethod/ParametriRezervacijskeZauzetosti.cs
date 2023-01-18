namespace avranic_zadaca_1.Sucelja;

public class ParametriRezervacijskeZauzetosti : IParametriZauzetosti
{
    public Brod Brod { get; }
    public Vez Vez { get; }
    public DateTime RezerviranDo { get; }
    public DateTime RezerviranOd { get; }

    public ParametriRezervacijskeZauzetosti(Brod brod, Vez vez, DateTime rezerviranOd, DateTime rezerviranDo)
    {
        Brod = brod;
        Vez = vez;
        RezerviranDo = rezerviranDo;
        RezerviranOd = rezerviranOd;
    }
}