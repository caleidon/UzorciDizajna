// ReSharper disable UnusedAutoPropertyAccessor.Local

#region

using System.Diagnostics.CodeAnalysis;
using avranic_zadaca_1.ChainOfResponsibility;
using avranic_zadaca_1.Observer;

#endregion

namespace avranic_zadaca_1;

public class Brod : ISubscriber, IHitniPrijevozHandler
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum VrstaBroda
    {
        TR,
        KA,
        KL,
        KR,
        RI,
        TE,
        JA,
        BR,
        RO
    }

    public float Duljina { get; }
    public float Gaz { get; }
    private float MaksimalnaBrzina { get; }
    public float Sirina { get; }

    public int Id { get; }
    private int KapacitetOsobnihVozila { get; }
    private int KapacitetPutnika { get; }
    private int KapacitetTereta { get; }
    private string Naziv { get; }
    private string OznakaBroda { get; }
    public VrstaBroda Vrsta { get; }
    public IMediator? Mediator { get; set; }
    public bool JePrivezan => BrodskaLuka.Instance.DajSveVezove().Any(v => v.PrivezaniBrod == this);

    public int BrojIzvanrednihPutnika { get; set; }

    private int MaksimalanBrojIzvanrednihPutnika =>
        Vrsta switch {
            VrstaBroda.TR => 30,
            VrstaBroda.KA => 30,
            VrstaBroda.KL => 20,
            VrstaBroda.KR => 20,
            VrstaBroda.RI => 10,
            VrstaBroda.TE => 50,
            VrstaBroda.JA => 30,
            VrstaBroda.BR => 5,
            VrstaBroda.RO => 1,
            _             => throw new ArgumentOutOfRangeException()
        };

    private bool ZeliPrevestiSluzbenike =>
        Vrsta switch {
            VrstaBroda.TR => true,
            VrstaBroda.KA => true,
            VrstaBroda.KL => true,
            VrstaBroda.KR => true,
            VrstaBroda.RI => false,
            VrstaBroda.TE => false,
            VrstaBroda.JA => true,
            VrstaBroda.BR => false,
            VrstaBroda.RO => false,
            _             => throw new ArgumentOutOfRangeException()
        };

    private bool ZeliPrevestiImigrante =>
        Vrsta switch {
            VrstaBroda.TR => true,
            VrstaBroda.KA => true,
            VrstaBroda.KL => false,
            VrstaBroda.KR => false,
            VrstaBroda.RI => true,
            VrstaBroda.TE => true,
            VrstaBroda.JA => false,
            VrstaBroda.BR => false,
            VrstaBroda.RO => true,
            _             => throw new ArgumentOutOfRangeException()
        };

    private IHitniPrijevozHandler? _sljedeciHitniPrijevozHandler;
    private readonly Model _model;

    public Brod(int id,
                string oznakaBroda,
                string naziv,
                VrstaBroda vrsta,
                float duljina,
                float sirina,
                float gaz,
                float maksimalnaBrzina,
                int kapacitetPutnika,
                int kapacitetOsobnihVozila,
                int kapacitetTereta,
                Model model)
    {
        Id = id;
        OznakaBroda = oznakaBroda;
        Naziv = naziv;
        Vrsta = vrsta;
        Duljina = duljina;
        Gaz = gaz;
        KapacitetOsobnihVozila = kapacitetOsobnihVozila;
        KapacitetPutnika = kapacitetPutnika;
        KapacitetTereta = kapacitetTereta;
        MaksimalnaBrzina = maksimalnaBrzina;
        Sirina = sirina;
        _model = model;
    }

    public void PrimiPoruku(PorukaVhfKanala poruka, IMediator kanal)
    {
        if (poruka is not OdgovorLuckeKapetanije olk) {
            _model.IspisiPotvrdu(
                $"Brod {Id} primio je zahtjev od broda {((Brod) poruka.Posiljatelj).Id} na kanalu {((Kanal) kanal).Frekvencija}.",
                true);
            return;
        }

        if (olk.BrodOdgovora != this) { return; }

        if (olk.Odobreno) {
            _model.IspisiPotvrdu($"Brod {Id} je dobio odobrenje za ulazak u luku.", true);
            return;
        }

        _model.IspisiGresku($"Brod {Id} nije dobio odobrenje za ulazak u luku.", true);
    }

    public void SetNextHitniPrijevozHandler(IHitniPrijevozHandler? handler) { _sljedeciHitniPrijevozHandler = handler; }

    public void HandleHitniPrijevoz(HitniPrijevoz hitniPrijevoz)
    {
        if (hitniPrijevoz.PreostaloSluzbenika > 0 && ZeliPrevestiSluzbenike) {
            int brojSluzbenika = Math.Min(hitniPrijevoz.PreostaloSluzbenika,
                MaksimalanBrojIzvanrednihPutnika - BrojIzvanrednihPutnika);
            hitniPrijevoz.PreostaloSluzbenika -= brojSluzbenika;
            BrojIzvanrednihPutnika += brojSluzbenika;

            _model.IspisiPotvrdu($"Brod {Id} je će prevesti {brojSluzbenika} službenika.", true);
        }

        if (hitniPrijevoz.PreostaloImigranta > 0 && ZeliPrevestiImigrante) {
            int brojImigranta = Math.Min(hitniPrijevoz.PreostaloImigranta,
                MaksimalanBrojIzvanrednihPutnika - BrojIzvanrednihPutnika);
            hitniPrijevoz.PreostaloImigranta -= brojImigranta;
            BrojIzvanrednihPutnika += brojImigranta;

            _model.IspisiPotvrdu($"Brod {Id} je će prevesti {brojImigranta} imigranta.", true);
        }

        if ((hitniPrijevoz.PreostaloImigranta > 0 || hitniPrijevoz.PreostaloSluzbenika > 0) &&
            _sljedeciHitniPrijevozHandler != null) { _sljedeciHitniPrijevozHandler.HandleHitniPrijevoz(hitniPrijevoz); }
    }

    public void OdjaviSeSaKanala()
    {
        if (JePrivezan) {
            Vez privezaniVez = BrodskaLuka.Instance.DajSveVezove().First(v => v.PrivezaniBrod == this);
            privezaniVez.OslobodiPrivezaniBrod();
        }

        Mediator?.Unsubscribe(this);
        Mediator = null;
    }
}