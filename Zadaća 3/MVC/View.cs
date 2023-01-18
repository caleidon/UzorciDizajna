#region

using avranic_zadaca_1;

// ReSharper disable NotAccessedField.Local

#endregion

public class View : IObserver
{
    public enum TipPoruke
    {
        Greska,
        PotvrdaNaredbe,
        PorukaUnosa
    }

    private const string ANSI_ESC = "\x1b[";

    private readonly int _stvarniBrojRedakaGore;
    private readonly string?[] _listaPorukaGore;
    private readonly string?[] _listaPorukaDolje;
    private readonly bool _greskeSuDolje;

    public const string PORUKA_UNOSA = "Unesite naredbu: ";

    private Controller _controller = null!;
    private readonly Model _model;
    private int _yPozicijaKursora;

    public View(Model model, int maksimalanBrojRedaka, int omjerGornjegDijelaEkrana, bool greskeSuDolje)
    {
        _model = model;

        int stvarniBrojRedakaDolje = (int) Math.Round(maksimalanBrojRedaka * (omjerGornjegDijelaEkrana / 100.0));
        _stvarniBrojRedakaGore = maksimalanBrojRedaka - stvarniBrojRedakaDolje;

        _greskeSuDolje = greskeSuDolje;

        _listaPorukaGore = new string[_stvarniBrojRedakaGore];
        _listaPorukaDolje = new string[stvarniBrojRedakaDolje];
        _listaPorukaGore.NapuniStringArray(null);
        _listaPorukaDolje.NapuniStringArray(null);
    }

    public void Update(Model.IzvrsenaNaredba izvrsenaNaredba, bool odmahOsvjeziEkran)
    {
        switch (izvrsenaNaredba) {
            case Model.IzvrsenaNaredba.IspisiStatuseVezova:
                DodajPoruke(_model.PodaciIspisiStatuseVezova);
                break;
            case Model.IzvrsenaNaredba.IspisiZauzetostiVezova:
                DodajPoruke(_model.PodaciIspisiZauzetostiVezova);
                break;
            case Model.IzvrsenaNaredba.UcitajZahtjeveRezervacije:
                break;
            case Model.IzvrsenaNaredba.IspisiZauzetostVezovaPremaVrstiNaDatum:
                DodajPoruke(_model.PodaciIspisiZauzetostVezovaPremaVrstiNaDatum);
                break;
            case Model.IzvrsenaNaredba.IspisiDnevnik:
                DodajPoruke(_model.PodaciIspisiDnevnik);
                break;
            case Model.IzvrsenaNaredba.IspisiPotvrdu:
                DodajPoruku(_model.PodaciIspisiPotvrdu);
                break;
            case Model.IzvrsenaNaredba.IspisiGresku:
                DodajPoruku(_model.PodaciIspisiGresku, TipPoruke.Greska);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(izvrsenaNaredba), izvrsenaNaredba, null);
        }

        if (odmahOsvjeziEkran) { OsvjeziEkran(); }
    }

    public void RegisterController(Controller controller) { _controller = controller; }

    private void DodajPoruke(List<string> poruke)
    {
        for (int i = 0; i < poruke.Count - 1; i++) { DodajPoruku(poruke[i]); }
    }

    public int DodajPoruku(string poruka, TipPoruke tipPoruke = TipPoruke.PotvrdaNaredbe)
    {
        string?[] poruke = tipPoruke switch {
            TipPoruke.PotvrdaNaredbe or TipPoruke.PorukaUnosa => _greskeSuDolje ? _listaPorukaGore : _listaPorukaDolje,
            TipPoruke.Greska                                  => _greskeSuDolje ? _listaPorukaDolje : _listaPorukaGore,

            _ => throw new ArgumentOutOfRangeException(nameof(tipPoruke), tipPoruke, null)
        };

        if (tipPoruke == TipPoruke.Greska) { poruka = $"[Greška {Program.RedniBrojPogreske++}]: {poruka}"; }

        bool porukeNapunjene = poruke[^1] != null;

        if (tipPoruke == TipPoruke.PorukaUnosa) {
            if (porukeNapunjene) {
                poruke[^1] = poruka;
                return _greskeSuDolje ? poruke.Length : _listaPorukaGore.Length + 1 + poruke.Length;
            }

            for (int i = 0; i < poruke.Length; i++) {
                if (poruke[i] != null) { continue; }

                poruke[i - 1] = poruka;
                return _greskeSuDolje ? i : _listaPorukaGore.Length + 1 + i;
            }
        }

        if (porukeNapunjene) {
            for (int i = 0; i < poruke.Length - 1; i++) { poruke[i] = poruke[i + 1]; }

            poruke[^1] = poruka;

            return _greskeSuDolje ? poruke.Length : _listaPorukaGore.Length + 1 + poruke.Length;
        }

        for (int i = 0; i < poruke.Length; i++) {
            if (poruke[i] != null) { continue; }

            poruke[i] = poruka;
            return _greskeSuDolje ? i + 1 : _listaPorukaGore.Length + i + 2;
        }

        throw new Exception($"Dosao na rub, a nije vracena pozicija poruke: {poruka}");
    }

    public void OsvjeziEkran()
    {
        OcistiEkran();
        int linijaPorukeUnosa = 0;

        if (_greskeSuDolje) { linijaPorukeUnosa = DodajPoruku(PORUKA_UNOSA); }

        IspisiPoruke(true);

        IspisiSeparator();

        if (!_greskeSuDolje) { linijaPorukeUnosa = DodajPoruku(PORUKA_UNOSA); }

        IspisiPoruke(false);
        PostaviKursor(linijaPorukeUnosa, 18);
    }

    public void IspisiVirtualnoVrijeme() { DodajPoruku($"[{VirtualniSat.Instance.VirtualnoVrijeme}]"); }

    private void IspisiPoruke(bool gornjiEkran)
    {
        if ((gornjiEkran && !_greskeSuDolje) || (!gornjiEkran && _greskeSuDolje)) { PromjeniBoju(TipPoruke.Greska); }

        if ((gornjiEkran && _greskeSuDolje) || (!gornjiEkran && !_greskeSuDolje)) {
            PromjeniBoju(TipPoruke.PotvrdaNaredbe);
        }

        string?[] poruke = gornjiEkran ? _listaPorukaGore : _listaPorukaDolje;
        _yPozicijaKursora = gornjiEkran ? 1 : _stvarniBrojRedakaGore + 2;

        foreach (string? t in poruke) {
            PostaviKursor(_yPozicijaKursora++, 0);
            Console.Write(t ?? "");
        }

        PromjeniBoju(TipPoruke.PorukaUnosa);
    }

    private void IspisiSeparator()
    {
        PostaviKursor(_stvarniBrojRedakaGore + 1, 0);
        Console.Write("================================================================================");
    }

    private void PromjeniBoju(TipPoruke boja)
    {
        switch (boja) {
            case TipPoruke.Greska:
                Console.Write(ANSI_ESC + "31m");
                break;
            case TipPoruke.PotvrdaNaredbe:
                Console.Write(ANSI_ESC + "32m");
                break;
            case TipPoruke.PorukaUnosa:
                Console.Write(ANSI_ESC + "0m");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(boja), boja, null);
        }
    }

    private void PostaviKursor(int linija, int stupac) { Console.Write(ANSI_ESC + linija + ";" + stupac + "f"); }

    private void OcistiEkran()
    {
        Console.Write(ANSI_ESC + "2J");
        PostaviKursor(0, 0);
        _yPozicijaKursora = 1;
    }
}