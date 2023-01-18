// ReSharper disable UnusedAutoPropertyAccessor.Global

#region

using avranic_zadaca_1.Observer;

#endregion

namespace avranic_zadaca_1;

public class BrodskaLuka : Composite, ISubscriber, IMolIteratorCreator, IOriginator
{
    private static BrodskaLuka? _instance;

    public static BrodskaLuka Instance => _instance ??= new BrodskaLuka();

    public int DubinaLuke { get; set; }
    public int UkupniBrojOstalihVezova { get; set; }
    public int UkupniBrojPoslovnihVezova { get; set; }
    public int UkupniBrojPutnickihVezova { get; set; }

    public List<Brod> SviBrodovi { get; set; } = new();
    public List<MolVez> SviMolVezovi { get; set; } = new();
    public List<Kanal> SviKanali { get; set; } = new();
    public List<Raspored> SviRasporedi { get; set; } = new();
    public List<ZahtjevRezervacije> SviZahtjeviRezervacije { get; set; } = new();
    public List<IMemento> SpremljenaStanjaZauzetosti { get; set; } = new();
    public string GpsSirina { get; set; } = null!;
    public string GpsVisina { get; set; } = null!;
    public string Naziv { get; set; } = null!;
    public DateTime InicijalnoVirtualnoVrijeme { get; set; }
    public List<ZapisZahtjevaBroda> Dnevnik { get; } = new();

    public Model Model { private get; set; } = null!;

    private BrodskaLuka() { }

    public void PrimiPoruku(PorukaVhfKanala poruka, IMediator kanal)
    {
        var brodZahtjeva = (Brod) poruka.Posiljatelj;

        if (poruka is ZahtjevZaOdjavu) {
            ProbajOdjavitiBrod(brodZahtjeva, (Kanal) kanal);
            return;
        }

        if (brodZahtjeva.JePrivezan) {
            Vez vez = DajSveVezove().First(v => v.PrivezaniBrod == brodZahtjeva);

            Model.IspisiGresku($"Brod {brodZahtjeva.Id} već je privezan na vez {vez.Id} stoga se ne može vezati.",
                true);
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

    public void ProbajPrivezatiBrodZaSlobodanVez(Brod brod, int trajanjeUSatima, IMediator kanal)
    {
        if (!Model.ProbajPrivezatiZaSlobodniVez(brod, trajanjeUSatima)) {
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

    public void ProbajPrivezatiBrodZaRezerviraniVez(Brod brod, IMediator kanal)
    {
        if (!Model.ProbajPrivezatiZaRezerviraniVez(brod)) {
            Dnevnik.Add(new ZapisZahtjevaBroda(brod, false, VirtualniSat.Instance.VirtualnoVrijeme,
                VirtualniSat.Instance.VirtualnoVrijeme, ZapisZahtjevaBroda.TipZahtjeva.RezerviraniVez));
            return;
        }

        Vez vez = DajSveVezove().First(v => v.PrivezaniBrod == brod);

        if (vez.BrodPrivezanDo == null) {
            Model.IspisiGresku($"Brod {brod.Id} nije imao vrijeme do kada je privezan, a trebao je!", true);
            return;
        }

        kanal.PosaljiPoruku(new OdgovorLuckeKapetanije(this, brod, true));

        Dnevnik.Add(new ZapisZahtjevaBroda(brod, true, VirtualniSat.Instance.VirtualnoVrijeme,
            (DateTime) vez.BrodPrivezanDo, ZapisZahtjevaBroda.TipZahtjeva.RezerviraniVez));
    }

    private void ProbajOdjavitiBrod(Brod brod, Kanal kanal)
    {
        if (brod.JePrivezan) {
            Vez vez = DajSveVezove().First(v => v.PrivezaniBrod == brod);
            vez.OslobodiPrivezaniBrod();
        }

        brod.OdjaviSeSaKanala();

        Dnevnik.Add(new ZapisZahtjevaBroda(brod, true, VirtualniSat.Instance.VirtualnoVrijeme,
            VirtualniSat.Instance.VirtualnoVrijeme, ZapisZahtjevaBroda.TipZahtjeva.Odjava));

        Model.IspisiPotvrdu($"Brod {brod.Id} je odjavljen sa kanala {kanal.Frekvencija}.", true);
    }

    public IIterator<Mol> CreateMolIterator() { return new MolIterator(Djeca.Cast<Mol>().ToList()); }

    public List<Mol> DajSveMolove()
    {
        IIterator<Mol> molIterator = CreateMolIterator();
        var molovi = new List<Mol>();

        while (!molIterator.IsDone()) { molovi.Add(molIterator.GetNext()); }

        return molovi;
    }

    public List<Vez> DajSveVezove()
    {
        IIterator<Mol> molIterator = CreateMolIterator();

        var vezovi = new List<Vez>();

        while (!molIterator.IsDone()) {
            Mol mol = molIterator.GetNext();
            List<Vez> vezoviMola = mol.DajSveVezove();
            vezovi.AddRange(vezoviMola);
        }

        return vezovi;
    }

    public IMemento Save(string naziv) { return new StanjeZauzetostiMemento(naziv, DajSveVezove()); }

    public void Restore(IMemento memento)
    {
        if (memento is not StanjeZauzetostiMemento stanjeZauzetostiMemento) {
            Model.IspisiGresku("Memento nije tipa StanjeZauzetostiMemento!", true);
            return;
        }

        VirtualniSat.Instance.VirtualnoVrijeme = stanjeZauzetostiMemento.DajVrijemeStvaranja();

        foreach (StanjeZauzetostiPodaciPoVezu podatak in stanjeZauzetostiMemento.DajPodatkeZauzetostiVezova()) {
            Vez vez = DajSveVezove().First(v => v.Id == podatak.VezId);
            vez.SveZauzetosti = podatak.Zauzetosti;
            vez.BrodPrivezanDo = podatak.BrodPrivezanDo;
            vez.PrivezaniBrod = podatak.PrivezaniBrod;
        }
    }
}