#region

using System.Globalization;
using System.Text.RegularExpressions;
using avranic_zadaca_1.Sucelja;

#endregion

namespace avranic_zadaca_1;

public static class Program
{
    public static int RedniBrojPogreske { get; set; } = 1;

    public static int Main(string[] args)
    {
        var regex = new Regex(
            @"(?=.*-l (?<luka>[^\s]+))(?=.*-v (?<vez>[^\s]+))(?=.*-b (?<brod>[^\s]+))(?=.*-m (?<mol>[^\s]+))(?=.*-mv (?<molVez>[^\s]+))(?=.*-k (?<kanal>[^\s]+))(?=.*-r (?<raspored>[^\s]+))?");
        Match matcher = regex.Match(string.Join(" ", args));

        if (!matcher.Success || matcher.Groups.Count > 8) {
            Alati.IspisiGresku("Neispravni ulazni argumenti!");
            return 1;
        }

        string datotekaLuka = matcher.Groups["luka"].Value;
        string datotekaMolova = matcher.Groups["mol"].Value;
        string datotekaVeza = matcher.Groups["vez"].Value;
        string datotekaMolVezova = matcher.Groups["molVez"].Value;
        string datotekaKanala = matcher.Groups["kanal"].Value;
        string datotekaBroda = matcher.Groups["brod"].Value;
        string datotekaRasporeda = matcher.Groups["raspored"].Value;

        var brodskaLukaCsvCitac = new BrodskaLukaCsvCitac();
        brodskaLukaCsvCitac.Citaj(datotekaLuka);

        BrodskaLuka.Instance.SviBrodovi = new BrodCsvCitac().Citaj(datotekaBroda);
        BrodskaLuka.Instance.SviMolovi = new MolCsvCitac().Citaj(datotekaMolova);
        BrodskaLuka.Instance.SviVezovi = new VezCsvCitac().Citaj(datotekaVeza);
        BrodskaLuka.Instance.SviMolVezovi = new MolVezCsvCitac().Citaj(datotekaMolVezova);
        BrodskaLuka.Instance.SviKanali = new KanalCsvCitac().Citaj(datotekaKanala);

        if (datotekaRasporeda != "") {
            BrodskaLuka.Instance.SviRasporedi = new RasporedCsvCitac().Citaj(datotekaRasporeda);
        }

        Alati.ProvjeriPripadaLiSvakiSvakiVezMolu();

        var servis = new Servis();
        IServis servisProxy = new ServisProxy(servis);
        BrodskaLuka.Instance.Servis = servis;

        while (true) {
            Console.WriteLine("\nUnesite naredbu: ");
            string unos = Console.ReadLine()!.Trim();

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

            switch (unos) {
                case { } when regexI.IsMatch(unos):
                    IspisiStatuseVezova(servisProxy);
                    break;
                case { } when regexVr.IsMatch(unos):
                    PromjeniVirtualnoVrijeme(servisProxy, unos);
                    break;
                case { } when regexV.IsMatch(unos):
                    IspisiZauzetostiVezova(servisProxy, regexV, unos);
                    break;
                case { } when regexUr.IsMatch(unos):
                    UcitajZahtjeveRezervacije(servisProxy, regexUr, unos);
                    Console.WriteLine(unos);
                    break;
                case { } when regexZd.IsMatch(unos):
                    PosaljiPorukuZahjevaPrivezaZaRezerviraniVez(servisProxy, unos);
                    break;
                case { } when regexZp.IsMatch(unos):
                    PosaljiPorukuZahjevaPrivezaZaSlobodniVez(servisProxy, unos);
                    break;
                case { } when regexZa.IsMatch(unos):
                    IspisiZauzetostVezovaPremaVrstiNaDatum(servisProxy, unos);
                    break;
                case { } when regexF.IsMatch(unos):
                    KomunicirajSaKanalom(servisProxy, unos);
                    break;
                case { } when regexT.IsMatch(unos):
                    PromjeniPostavkeTablicnogIspisa(servisProxy, unos);
                    break;
                case { } when regexD.IsMatch(unos):
                    IspisiDnevnik(servisProxy);
                    break;
                case { } when regexHp.IsMatch(unos):
                    OrganizirajHitniPrijevoz(servisProxy, unos);
                    break;
                default:
                    Alati.IspisiGresku("Neispravna naredba!");
                    break;
            }
        }

        return 0;
    }

    private static void IspisiZauzetostVezovaPremaVrstiNaDatum(IServis servis, string unos)
    {
        string[] unosSplit = unos.Split(" ");

        DateTime vrijemeOd = DateTime.ParseExact(unosSplit[1] + " " + unosSplit[2] + ":00", "dd.MM.yyyy. HH:mm:ss",
            CultureInfo.CreateSpecificCulture("hr-HR"));
        DateTime vrijemeDo = vrijemeOd.AddMinutes(1);

        servis.IspisiZauzetostVezovaPremaVrstiNaDatum(vrijemeOd, vrijemeDo);
    }

    private static void IspisiStatuseVezova(IServis servis) { servis.IspisiStatuseVezova(); }

    private static void IspisiZauzetostiVezova(IServis servis, Regex regexVeza, string unos)
    {
        Match match = regexVeza.Match(unos);

        var vrstaVezova = (Vez.VrstaVeza) Enum.Parse(typeof(Vez.VrstaVeza), match.Groups[1].Value);
        bool zauzeti = match.Groups[2].Value == "Z";
        DateTime datumOd = DateTime.Parse(match.Groups[3].Value, CultureInfo.CreateSpecificCulture("hr-HR"));
        DateTime datumDo = DateTime.Parse(match.Groups[5].Value, CultureInfo.CreateSpecificCulture("hr-HR"));

        servis.IspisiZauzetostiVezova(vrstaVezova, zauzeti, datumOd, datumDo);
    }

    private static void PosaljiPorukuZahjevaPrivezaZaRezerviraniVez(IServis servis, string unos)
    {
        int idBroda = int.Parse(unos.Split(" ")[1]);

        if (!Alati.ProbajPronaciBrod(idBroda, true, out Brod brod)) { return; }

        servis.PosaljiPorukuZahjevaPrivezaZaRezerviraniVez(brod);
    }

    private static void PosaljiPorukuZahjevaPrivezaZaSlobodniVez(IServis servis, string unos)
    {
        int idBroda = int.Parse(unos.Split(" ")[1]);
        int trajanje = int.Parse(unos.Split(" ")[2]);

        if (!Alati.ProbajPronaciBrod(idBroda, true, out Brod brod)) { return; }

        servis.PosaljiPorukuZahjevaPrivezaZaSlobodniVez(brod, trajanje);
    }

    private static void UcitajZahtjeveRezervacije(IServis servis, Regex regexUr, string unos)
    {
        string nazivDatoteke = regexUr.Match(unos).Groups["naziv_datoteke"].Value;

        servis.UcitajZahtjeveRezervacije(nazivDatoteke);
    }

    private static void PromjeniVirtualnoVrijeme(IServis servis, string unos)
    {
        string[] unosSplit = unos.Split(" ");

        DateTime novoVrijeme = DateTime.ParseExact(unosSplit[1] + " " + unosSplit[2], "dd.MM.yyyy. HH:mm:ss",
            CultureInfo.CreateSpecificCulture("hr-HR"));

        servis.PromjeniVirtualnoVrijeme(novoVrijeme);
        Alati.IspisiPotvrdu($"Virtualno vrijeme je postavljeno na {novoVrijeme}");
    }

    private static void PromjeniPostavkeTablicnogIspisa(IServis servis, string unos)
    {
        bool ispisZaglavlja = unos.Contains('Z');
        bool ispisPodnozja = unos.Contains('P');
        bool ispisRednihBrojeva = unos.Contains("RB");

        servis.PromjeniPostavkeTablicnogIspisa(ispisZaglavlja, ispisPodnozja, ispisRednihBrojeva);
    }

    private static void KomunicirajSaKanalom(IServis servisProxy, string unos)
    {
        string[] podaci = unos.Split(" ");
        int idBroda = int.Parse(podaci[1]);
        int frekvencija = int.Parse(podaci[2]);
        bool odjava = unos.Contains('Q');

        if (!Alati.ProbajPronaciBrod(idBroda, false, out Brod brod)) { return; }

        servisProxy.SpojiSeNaKanal(brod, frekvencija, odjava);
    }

    private static void IspisiDnevnik(IServis servisProxy) { servisProxy.IspisiDnevnik(); }

    private static void OrganizirajHitniPrijevoz(IServis servisProxy, string unos)
    {
        string[] podaci = unos.Split(" ");
        int brojSluzbenika = int.Parse(podaci[1]);
        int brojImigranta = int.Parse(podaci[2]);

        servisProxy.OrganizirajHitniPrijevoz(brojSluzbenika, brojImigranta);
    }
}