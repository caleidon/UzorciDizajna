// ReSharper disable UnusedAutoPropertyAccessor.Global

#region

using avranic_zadaca_1.Observer;

#endregion

namespace avranic_zadaca_1;

public class BrodskaLuka : ISubscriber
{
    private static BrodskaLuka? _instance;

    public static BrodskaLuka Instance => _instance ??= new BrodskaLuka();

    public int DubinaLuke { get; set; }
    public int UkupniBrojOstalihVezova { get; set; }
    public int UkupniBrojPoslovnihVezova { get; set; }
    public int UkupniBrojPutnickihVezova { get; set; }

    public List<Brod> SviBrodovi { get; set; } = new();
    public List<Mol> SviMolovi { get; set; } = new();
    public List<MolVez> SviMolVezovi { get; set; } = new();
    public List<Kanal> SviKanali { get; set; } = new();
    public List<Vez> SviVezovi { get; set; } = new();
    public List<Raspored> SviRasporedi { get; set; } = new();
    public List<ZahtjevRezervacije> SviZahtjeviRezervacije { get; set; } = new();
    public string GpsSirina { get; set; } = null!;
    public string GpsVisina { get; set; } = null!;
    public string Naziv { get; set; } = null!;
    public DateTime InicijalnoVirtualnoVrijeme { get; set; }
    public List<ZapisZahtjevaBroda> Dnevnik { get; } = new();

    public Servis Servis { get; set; } = null!;

    private BrodskaLuka() { }

    public void PrimiPoruku(PorukaVhfKanala poruka, IMediator kanal)
    {
        var brodZahtjeva = (Brod) poruka.Posiljatelj;

        if (poruka is ZahtjevZaOdjavu) {
            ProbajOdjavitiBrod(brodZahtjeva, (Kanal) kanal);
            return;
        }

        if (brodZahtjeva.JePrivezan) {
            Vez vez = SviVezovi.First(v => v.PrivezaniBrod == brodZahtjeva);

            Alati.IspisiGresku($"Brod {brodZahtjeva.Id} već je privezan na vez {vez.Id} stoga se ne može vezati.");
            kanal.PosaljiPoruku(new OdgovorLuckeKapetanije(this, brodZahtjeva, false));
            return;
        }

        switch (poruka) {
            case ZahtjevZaPrivezNaRezerviraniVez:
                ProbajPrivezatiBrodZaRezerviraniVez(brodZahtjeva, kanal);
                return;
            case ZahtjevZaPrivezNaSlobodanVez zsv:
                ProbajPrivezatiBrodZaSlobodanVez(brodZahtjeva, zsv.TrajanjeUSatima, kanal);
                return;
        }

        kanal.PosaljiPoruku(new OdgovorLuckeKapetanije(this, brodZahtjeva, false));

        Dnevnik.Add(new ZapisZahtjevaBroda(brodZahtjeva, false, VirtualniSat.Instance.VirtualnoVrijeme,
            VirtualniSat.Instance.VirtualnoVrijeme,
            poruka is ZahtjevZaPrivezNaRezerviraniVez ?
                ZapisZahtjevaBroda.TipZahtjeva.RezerviraniVez :
                ZapisZahtjevaBroda.TipZahtjeva.SlobodanVez));
    }

    private void ProbajPrivezatiBrodZaRezerviraniVez(Brod brod, IMediator kanal)
    {
        if (!Servis.ProbajPrivezatiZaRezerviraniVez(brod)) {
            Dnevnik.Add(new ZapisZahtjevaBroda(brod, false, VirtualniSat.Instance.VirtualnoVrijeme,
                VirtualniSat.Instance.VirtualnoVrijeme, ZapisZahtjevaBroda.TipZahtjeva.RezerviraniVez));
            return;
        }

        Vez vez = SviVezovi.First(v => v.PrivezaniBrod == brod);

        if (vez.BrodPrivezanDo == null) {
            throw new Exception($"Brod {brod.Id} nije imao vrijeme do kada je privezan, a trebao je!");
        }

        kanal.PosaljiPoruku(new OdgovorLuckeKapetanije(this, brod, true));

        Dnevnik.Add(new ZapisZahtjevaBroda(brod, true, VirtualniSat.Instance.VirtualnoVrijeme,
            (DateTime) vez.BrodPrivezanDo, ZapisZahtjevaBroda.TipZahtjeva.RezerviraniVez));
    }

    private void ProbajPrivezatiBrodZaSlobodanVez(Brod brod, int trajanjeUSatima, IMediator kanal)
    {
        if (!Servis.ProbajPrivezatiZaSlobodniVez(brod, trajanjeUSatima)) {
            Dnevnik.Add(new ZapisZahtjevaBroda(brod, false, VirtualniSat.Instance.VirtualnoVrijeme,
                VirtualniSat.Instance.VirtualnoVrijeme.AddHours(trajanjeUSatima),
                ZapisZahtjevaBroda.TipZahtjeva.SlobodanVez));
            return;
        }

        kanal.PosaljiPoruku(new OdgovorLuckeKapetanije(this, brod, true));

        Dnevnik.Add(new ZapisZahtjevaBroda(brod, true, VirtualniSat.Instance.VirtualnoVrijeme,
            VirtualniSat.Instance.VirtualnoVrijeme.AddHours(trajanjeUSatima),
            ZapisZahtjevaBroda.TipZahtjeva.SlobodanVez));
    }

    private void ProbajOdjavitiBrod(Brod brod, Kanal kanal)
    {
        if (brod.JePrivezan) {
            Vez vez = SviVezovi.First(v => v.PrivezaniBrod == brod);
            vez.OslobodiPrivezaniBrod();
        }

        brod.OdjaviSeSaKanala();

        Dnevnik.Add(new ZapisZahtjevaBroda(brod, true, VirtualniSat.Instance.VirtualnoVrijeme,
            VirtualniSat.Instance.VirtualnoVrijeme, ZapisZahtjevaBroda.TipZahtjeva.Odjava));

        Alati.IspisiPotvrdu($"Brod {brod.Id} je odjavljen sa kanala {kanal.Frekvencija}.");
    }
}