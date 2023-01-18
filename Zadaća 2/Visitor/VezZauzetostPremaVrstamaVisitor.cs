namespace avranic_zadaca_1;

public class VezZauzetostPremaVrstamaVisitor : IVezVisitor
{
    private readonly DateTime _zauzetOd;
    private readonly DateTime _zauzetDo;
    private readonly Dictionary<Vez.VrstaVeza, int> _zauzetostPremaVrstiVeza = new();

    public VezZauzetostPremaVrstamaVisitor(DateTime zauzetOd, DateTime zauzetDo)
    {
        _zauzetOd = zauzetOd;
        _zauzetDo = zauzetDo;

        _zauzetostPremaVrstiVeza.Add(Vez.VrstaVeza.OS, 0);
        _zauzetostPremaVrstiVeza.Add(Vez.VrstaVeza.PO, 0);
        _zauzetostPremaVrstiVeza.Add(Vez.VrstaVeza.PU, 0);
    }

    public void VisitVez(Vez vez)
    {
        if (!vez.RezerviranUDatumskomRasponu(_zauzetOd, _zauzetDo)) { return; }

        _zauzetostPremaVrstiVeza[vez.Vrsta] += 1;
    }

    public void IspisiZauzetostiPoVrstama()
    {
        var zaglavlje = new List<string> { "Vrsta", "Broj zauzetih" };

        var podaci = new List<List<object>>();

        foreach (KeyValuePair<Vez.VrstaVeza, int> zauzetost in _zauzetostPremaVrstiVeza) {
            var redak = new List<object> { zauzetost.Key.ToString(), zauzetost.Value };
            podaci.Add(redak);
        }

        IspisivacTablice.Instance.IspisiTablicu(zaglavlje, podaci);
    }
}