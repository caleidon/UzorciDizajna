#region

using System.Globalization;
using System.Text.RegularExpressions;
using avranic_zadaca_1;
using avranic_zadaca_1.Sucelja;

#endregion

public class Controller
{
    private readonly IModel _model;
    private readonly View _view;

    public Controller(IModel model, View view)
    {
        _model = model;
        _view = view;
    }

    public void ZapocniRadPrograma()
    {
        _view.OsvjeziEkran();

        while (true) {
            string unos = Console.ReadLine()!.Trim();
            _view.DodajPoruku($"{View.PORUKA_UNOSA}{unos}", View.TipPoruke.PorukaUnosa);

            if (unos == "Q") { break; }

            var regexI = new Regex(@"^I$");

            var regexVr = new Regex(
                @"^VR (0[1-9]|1\d|2[0-8]|29(?=-\d\d-(?!1[01345789]00|2[1235679]00)\d\d(?:[02468][048]|[13579][26]))|30(?!-02)|31(?=\.0[13578]|\.1[02]))\.(0[1-9]|1[0-2])\.([12]\d{3})\. ([01]\d|2[0-3]):([0-5]\d):([0-5]\d)$");

            var regexV = new Regex(
                @"V\s([A-Z]{2})\s([ZS])\s(\d{2}\.\d{2}\.\d{4}\.\s\d{1,2}:\d{2}(:\d{2})?)\s(\d{2}\.\d{2}\.\d{4}\.\s\d{1,2}:\d{2}(:\d{2})?)");
            var regexUr = new Regex(@"^UR\s(?<naziv_datoteke>.+)$");
            var regexZd = new Regex(@"^ZD\s\d+$");
            var regexZp = new Regex(@"ZP\s(\d+)\s(\d+)");
            var regexT = new Regex(@"^(T)([ ](Z|P|RB))*$");

            var regexZa = new Regex(
                @"^ZA\s(0[1-9]|1\d|2[0-8]|29(?=-\d\d-(?!1[01345789]00|2[1235679]00)\d\d(?:[02468][048]|[13579][26]))|30(?!-02)|31(?=\.0[13578]|\.1[02]))\.(0[1-9]|1[0-2])\.([12]\d{3})\. ([01]\d|2[0-3]):([0-5]\d)$");

            var regexF = new Regex(@"^F\s(\d+)\s(\d+)(\sQ)?$");
            var regexD = new Regex(@"^D$");
            var regexHp = new Regex(@"^HP\s(?<sluzbenici>\d+)\s(?<imigranti>\d+)$");
            var regexSps = new Regex(@"^SPS\s""(?<naziv>.+)""$");
            var regexVps = new Regex(@"^VPS\s""(?<naziv>.+)""$");

            switch (unos) {
                case { } when regexI.IsMatch(unos):
                    IspisiStatuseVezova(_model);
                    break;
                case { } when regexVr.IsMatch(unos):
                    PromjeniVirtualnoVrijeme(_model, unos);
                    break;
                case { } when regexV.IsMatch(unos):
                    IspisiZauzetostiVezova(_model, regexV, unos);
                    break;
                case { } when regexUr.IsMatch(unos):
                    UcitajZahtjeveRezervacije(_model, regexUr, unos);
                    break;
                case { } when regexZd.IsMatch(unos):
                    PosaljiPorukuZahjevaPrivezaZaRezerviraniVez(_model, unos);
                    break;
                case { } when regexZp.IsMatch(unos):
                    PosaljiPorukuZahjevaPrivezaZaSlobodniVez(_model, unos);
                    break;
                case { } when regexZa.IsMatch(unos):
                    IspisiZauzetostVezovaPremaVrstiNaDatum(_model, unos);
                    break;
                case { } when regexF.IsMatch(unos):
                    KomunicirajSaKanalom(_model, unos);
                    break;
                case { } when regexT.IsMatch(unos):
                    PromjeniPostavkeTablicnogIspisa(_model, unos);
                    break;
                case { } when regexD.IsMatch(unos):
                    IspisiDnevnik(_model);
                    break;
                case { } when regexHp.IsMatch(unos):
                    OrganizirajHitniPrijevoz(_model, unos);
                    break;
                case { } when regexSps.IsMatch(unos):
                    SpremiPostojeceStanjeSvihVezova(_model, unos);
                    break;
                case { } when regexVps.IsMatch(unos):
                    VratiPostojeceStanjeSvihVezova(_model, unos);
                    break;
                default:
                    _model.IspisiGresku("Neispravna naredba!", true);
                    break;
            }
        }
    }

    private void IspisiZauzetostVezovaPremaVrstiNaDatum(IModel model, string unos)
    {
        string[] unosSplit = unos.Split(" ");

        DateTime vrijemeOd = DateTime.ParseExact(unosSplit[1] + " " + unosSplit[2] + ":00", "dd.MM.yyyy. HH:mm:ss",
            CultureInfo.CreateSpecificCulture("hr-HR"));
        DateTime vrijemeDo = vrijemeOd.AddMinutes(1);

        model.IspisiZauzetostVezovaPremaVrstiNaDatum(vrijemeOd, vrijemeDo);
    }

    private void IspisiStatuseVezova(IModel model) { model.IspisiStatuseVezova(); }

    private void IspisiZauzetostiVezova(IModel model, Regex regexVeza, string unos)
    {
        Match match = regexVeza.Match(unos);

        var vrstaVezova = (Vez.VrstaVeza) Enum.Parse(typeof(Vez.VrstaVeza), match.Groups[1].Value);
        bool zauzeti = match.Groups[2].Value == "Z";
        DateTime datumOd = DateTime.Parse(match.Groups[3].Value, CultureInfo.CreateSpecificCulture("hr-HR"));
        DateTime datumDo = DateTime.Parse(match.Groups[5].Value, CultureInfo.CreateSpecificCulture("hr-HR"));

        model.IspisiZauzetostiVezova(vrstaVezova, zauzeti, datumOd, datumDo);
    }

    private void PosaljiPorukuZahjevaPrivezaZaRezerviraniVez(IModel model, string unos)
    {
        int idBroda = int.Parse(unos.Split(" ")[1]);

        if (!ProbajPronaciBrod(idBroda, true, out Brod brod)) { return; }

        model.PosaljiPorukuZahjevaPrivezaZaRezerviraniVez(brod);
    }

    private void PosaljiPorukuZahjevaPrivezaZaSlobodniVez(IModel model, string unos)
    {
        int idBroda = int.Parse(unos.Split(" ")[1]);
        int trajanje = int.Parse(unos.Split(" ")[2]);

        if (!ProbajPronaciBrod(idBroda, true, out Brod brod)) { return; }

        model.PosaljiPorukuZahjevaPrivezaZaSlobodniVez(brod, trajanje);
    }

    private void UcitajZahtjeveRezervacije(IModel model, Regex regexUr, string unos)
    {
        string nazivDatoteke = regexUr.Match(unos).Groups["naziv_datoteke"].Value;

        model.UcitajZahtjeveRezervacije(nazivDatoteke);
    }

    private void PromjeniVirtualnoVrijeme(IModel model, string unos)
    {
        string[] unosSplit = unos.Split(" ");

        DateTime novoVrijeme = DateTime.ParseExact(unosSplit[1] + " " + unosSplit[2], "dd.MM.yyyy. HH:mm:ss",
            CultureInfo.CreateSpecificCulture("hr-HR"));

        model.PromjeniVirtualnoVrijeme(novoVrijeme, true);
    }

    private void PromjeniPostavkeTablicnogIspisa(IModel model, string unos)
    {
        bool ispisZaglavlja = unos.Contains('Z');
        bool ispisPodnozja = unos.Contains('P');
        bool ispisRednihBrojeva = unos.Contains("RB");

        model.PromjeniPostavkeTablicnogIspisa(ispisZaglavlja, ispisPodnozja, ispisRednihBrojeva);
    }

    private void KomunicirajSaKanalom(IModel modelProxy, string unos)
    {
        string[] podaci = unos.Split(" ");
        int idBroda = int.Parse(podaci[1]);
        int frekvencija = int.Parse(podaci[2]);
        bool odjava = unos.Contains('Q');

        if (!ProbajPronaciBrod(idBroda, false, out Brod brod)) { return; }

        modelProxy.SpojiSeNaKanal(brod, frekvencija, odjava);
    }

    private void IspisiDnevnik(IModel modelProxy) { modelProxy.IspisiDnevnik(); }

    private void OrganizirajHitniPrijevoz(IModel modelProxy, string unos)
    {
        string[] podaci = unos.Split(" ");
        int brojSluzbenika = int.Parse(podaci[1]);
        int brojImigranta = int.Parse(podaci[2]);

        modelProxy.OrganizirajHitniPrijevoz(brojSluzbenika, brojImigranta);
    }

    private void SpremiPostojeceStanjeSvihVezova(IModel modelProxy, string unos)
    {
        string nazivDatoteke = unos.Split(" ")[1];
        modelProxy.SpremiPostojeceStanjeSvihVezova(nazivDatoteke);
    }

    private void VratiPostojeceStanjeSvihVezova(IModel modelProxy, string unos)
    {
        string nazivDatoteke = unos.Split(" ")[1];
        modelProxy.VratiPostojeceStanjeSvihVezova(nazivDatoteke);
    }

    private bool ProbajPronaciBrod(int idBroda, bool neprivezan, out Brod pronadeniBrod)
    {
        Brod? brod = BrodskaLuka.Instance.SviBrodovi.FirstOrDefault(b => b.Id == idBroda);

        if (brod != null) {
            pronadeniBrod = brod;

            if (!neprivezan) { return true; }

            if (!brod.JePrivezan) { return true; }

            _model.IspisiGresku($"Brod {idBroda} je pronađen ali je već privezan na vez.", true);
            return false;
        }

        pronadeniBrod = null!;
        _model.IspisiGresku($"Brod {idBroda} nije pronađen.", true);
        return false;
    }
}