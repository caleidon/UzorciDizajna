#region

using avranic_zadaca_1;
using avranic_zadaca_1.Sucelja;

#endregion

public class StanjeZauzetostiMemento : IMemento
{
    private readonly string _naziv;
    private readonly DateTime _virtualnoVrijeme;
    private readonly List<StanjeZauzetostiPodaciPoVezu> _podaciZauzetostiVezova = new();

    public StanjeZauzetostiMemento(string naziv, List<Vez> vezovi)
    {
        _naziv = naziv;
        _virtualnoVrijeme = VirtualniSat.Instance.VirtualnoVrijeme;

        foreach (Vez vez in vezovi) {
            _podaciZauzetostiVezova.Add(new StanjeZauzetostiPodaciPoVezu(vez.Id, vez.SveZauzetosti.ToList(),
                vez.PrivezaniBrod, vez.BrodPrivezanDo));
        }
    }

    public string DajNaziv() { return _naziv; }
    public List<StanjeZauzetostiPodaciPoVezu> DajPodatkeZauzetostiVezova() { return _podaciZauzetostiVezova; }

    public DateTime DajVrijemeStvaranja() { return _virtualnoVrijeme; }
}

public class StanjeZauzetostiPodaciPoVezu
{
    public readonly int VezId;
    public readonly List<IZauzetost> Zauzetosti;
    public readonly Brod? PrivezaniBrod;
    public DateTime? BrodPrivezanDo;

    public StanjeZauzetostiPodaciPoVezu(int vezId,
                                        List<IZauzetost> zauzetosti,
                                        Brod? privezaniBrod,
                                        DateTime? brodPrivezanDo)
    {
        VezId = vezId;
        Zauzetosti = zauzetosti;
        PrivezaniBrod = privezaniBrod;
        BrodPrivezanDo = brodPrivezanDo;
    }
}