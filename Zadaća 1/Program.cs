#region

using System.Globalization;
using System.Text.RegularExpressions;
using avranic_zadaca_1.Sucelja;

#endregion

namespace avranic_zadaca_1;

public static class Program
{
    private static int RedniBrojPogreske { get; set; } = 1;

    public static int Main(string[] args)
    {
        var regex = new Regex(
            @"(?=.*-l (?<luka>[^\s]+))(?=.*-v (?<vez>[^\s]+))(?=.*-b (?<brod>[^\s]+))(?=.*-r (?<raspored>[^\s]+))?");
        Match matcher = regex.Match(string.Join(" ", args));

        if (!matcher.Success || matcher.Groups.Count > 5) {
            IspisiGresku("Neispravni ulazni argumenti!");
            return 1;
        }

        string datotekaLuka = matcher.Groups["luka"].Value;
        string datotekaVeza = matcher.Groups["vez"].Value;
        string datotekaBroda = matcher.Groups["brod"].Value;
        string datotekaRasporeda = matcher.Groups["raspored"].Value;

        var brodskaLukaCsvCitac = new BrodskaLukaCsvCitac();
        brodskaLukaCsvCitac.Citaj(datotekaLuka);

        BrodskaLuka.Instance.SviBrodovi = new BrodCsvCitac().Citaj(datotekaBroda);
        BrodskaLuka.Instance.SviVezovi = new VezCsvCitac().Citaj(datotekaVeza);

        if (datotekaRasporeda != "") {
            BrodskaLuka.Instance.SviRasporedi = new RasporedCsvCitac().Citaj(datotekaRasporeda);
        }

        IServis servisProxy = new ServisProxy(new Servis());

        while (true) {
            Console.WriteLine("\nUnesite naredbu: ");
            string unos = Console.ReadLine()!.ToUpper();

            if (unos == "Q") { break; }

            var regexI = new Regex(@"^I$");

            var regexVr = new Regex(
                @"^VR (0[1-9]|1\d|2[0-8]|29(?=-\d\d-(?!1[01345789]00|2[1235679]00)\d\d(?:[02468][048]|[13579][26]))|30(?!-02)|31(?=\.0[13578]|\.1[02]))\.(0[1-9]|1[0-2])\.([12]\d{3})\. ([01]\d|2[0-3]):([0-5]\d):([0-5]\d)$");

            var regexV = new Regex(
                @"V\s([A-Z]{2})\s([ZS])\s(\d{2}\.\d{2}\.\d{4}\.\s\d{1,2}:\d{2}(:\d{2})?)\s(\d{2}\.\d{2}\.\d{4}\.\s\d{1,2}:\d{2}(:\d{2})?)");
            var regexUr = new Regex(@"^UR\s(?<naziv_datoteke>.+)$");
            var regexZd = new Regex(@"^ZD\s\d+$");
            var regexZp = new Regex(@"ZP\s(\d+)\s(\d+)");

            // switch statement on regex matches

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
                    break;
                case { } when regexZd.IsMatch(unos):
                    PriveziZaRezerviraniVez(servisProxy, unos);
                    break;
                case { } when regexZp.IsMatch(unos):
                    PriveziZaSlobodniVez(servisProxy, unos);
                    break;
                default:
                    IspisiGresku("Neispravna naredba!");
                    break;
            }
        }

        return 0;
    }

    private static void IspisiStatuseVezova(IServis servis) { servis.IspisiStatuseVezova(); }

    private static void IspisiZauzetostiVezova(IServis servis, Regex regexVeza, string unos)
    {
        Match match = regexVeza.Match(unos);

        var vrstaVezova = (Vez.VrstaVeza) Enum.Parse(typeof(Vez.VrstaVeza), match.Groups[1].Value);
        bool zauzeti = match.Groups[2].Value == "Z";
        DateTime datumOd = DateTime.Parse(match.Groups[3].Value);
        DateTime datumDo = DateTime.Parse(match.Groups[5].Value);

        servis.IspisiZauzetostiVezova(vrstaVezova, zauzeti, datumOd, datumDo);
    }

    private static void PriveziZaRezerviraniVez(IServis servis, string unos)
    {
        int idBroda = int.Parse(unos.Split(" ")[1]);

        if (!ProbajPronaciNeprivezaniBrod(idBroda, out Brod brod)) { return; }

        servis.PriveziZaRezerviraniVez(brod);
    }

    private static void PriveziZaSlobodniVez(IServis servis, string unos)
    {
        int idBroda = int.Parse(unos.Split(" ")[1]);
        int trajanje = int.Parse(unos.Split(" ")[2]);

        if (!ProbajPronaciNeprivezaniBrod(idBroda, out Brod brod)) { return; }

        servis.PriveziZaSlobodniVez(brod, trajanje);
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
            CultureInfo.InvariantCulture);

        servis.PromjeniVirtualnoVrijeme(novoVrijeme);
    }

    private static bool ProbajPronaciNeprivezaniBrod(int idBroda, out Brod pronadeniBrod)
    {
        Brod? brod = BrodskaLuka.Instance.SviBrodovi.FirstOrDefault(b => b.Id == idBroda);

        if (brod != null) {
            bool brodJePrivezan = BrodskaLuka.Instance.SviVezovi.Any(v => v.PrivezaniBrod?.Id == idBroda);
            pronadeniBrod = brod;

            if (!brodJePrivezan) { return true; }

            IspisiGresku($"Brod sa ID {idBroda} je pronađen ali je već privezan na vez.");
            return false;
        }

        pronadeniBrod = null!;
        IspisiGresku($"Brod sa ID {idBroda} nije pronađen.");
        return false;
    }

    public static void IspisiGresku(string porukaGreske)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[Greška {RedniBrojPogreske++}]: {porukaGreske}");
        Console.ResetColor();
    }
}