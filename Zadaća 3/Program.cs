#region

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
            @"(?=.*-l (?<luka>[^\s]+))(?=.*-v (?<vez>[^\s]+))(?=.*-b (?<brod>[^\s]+))(?=.*-m (?<mol>[^\s]+))(?=.*-mv (?<molVez>[^\s]+))(?=.*-k (?<kanal>[^\s]+))(?=.*-br (?<brojRedaka>[^\s]+))(?=.*-vt (?<omjerEkrana>[^\s]+))(?=.*-pd (?<ulogaEkrana>[^\s]+))(?=.*-r (?<raspored>[^\s]+))?");

        Match matcher = regex.Match(string.Join(" ", args));

        if (!matcher.Success || matcher.Groups.Count > 11) {
            throw new ArgumentException("Neispravni ulazni argumenti!");
        }

        int brojRedaka = int.Parse(matcher.Groups["brojRedaka"].Value);

        if (brojRedaka is < 24 or > 80) { throw new ArgumentException("Broj redaka mora biti između 24 i 80!"); }

        string omjerEkrana = matcher.Groups["omjerEkrana"].Value;

        if (omjerEkrana != "50:50" && omjerEkrana != "25:75" && omjerEkrana != "75:25") {
            throw new ArgumentException($"Omjer ekrana mora biti 50:50, 25:75 ili 75:25, a ne {omjerEkrana}!");
        }

        string ulogaEkrana = matcher.Groups["ulogaEkrana"].Value;

        if (ulogaEkrana != "R:P" && ulogaEkrana != "P:R") {
            throw new ArgumentException($"Uloga ekrana mora biti R:P ili P:R, a ne {ulogaEkrana}!");
        }

        bool greskeSuDolje = ulogaEkrana == "R:P";

        int omjerGornjegDijelaEkrana = int.Parse(omjerEkrana.Split(':')[0]);

        string datotekaLuka = matcher.Groups["luka"].Value;
        string datotekaMolova = matcher.Groups["mol"].Value;
        string datotekaVeza = matcher.Groups["vez"].Value;
        string datotekaMolVezova = matcher.Groups["molVez"].Value;
        string datotekaKanala = matcher.Groups["kanal"].Value;
        string datotekaBroda = matcher.Groups["brod"].Value;
        string datotekaRasporeda = matcher.Groups["raspored"].Value;

        // učitavanje modela prvo jer je potreban brodskoj luci
        var model = new Model();

        // zatim prvo učitati brodsku luku jer se također učitava početno virtualno vrijeme
        // koje inicijaliziramo u ModelProxy-u
        var brodskaLukaCsvCitac = new BrodskaLukaCsvCitac(model);
        brodskaLukaCsvCitac.Citaj(datotekaLuka);

        var modelProxy = new ModelProxy(model);
        BrodskaLuka.Instance.Model = model;

        var view = new View(model, brojRedaka, omjerGornjegDijelaEkrana, greskeSuDolje);
        model.AddObserver(view);
        modelProxy.PostaviView(view);

        var controller = new Controller(modelProxy, view);
        view.RegisterController(controller);

        // učitavanje ostatka podataka
        BrodskaLuka.Instance.SviBrodovi = new BrodCsvCitac(model).Citaj(datotekaBroda);

        foreach (Mol mol in new MolCsvCitac(model).Citaj(datotekaMolova)) { BrodskaLuka.Instance.AddChild(mol); }

        List<Vez> sviVezovi = new VezCsvCitac(model).Citaj(datotekaVeza);
        BrodskaLuka.Instance.SviMolVezovi = new MolVezCsvCitac(sviVezovi, model).Citaj(datotekaMolVezova);
        BrodskaLuka.Instance.SviKanali = new KanalCsvCitac(model).Citaj(datotekaKanala);

        if (datotekaRasporeda != "") {
            BrodskaLuka.Instance.SviRasporedi = new RasporedCsvCitac(model).Citaj(datotekaRasporeda);
        }

        ProvjeriPripadaLiSvakiSvakiVezMolu(model);

        // početak rada programa
        controller.ZapocniRadPrograma();
        return 0;
    }

    private static void ProvjeriPripadaLiSvakiSvakiVezMolu(IModel model)
    {
        for (int index = BrodskaLuka.Instance.DajSveVezove().Count - 1; index >= 0; index--) {
            Vez vez = BrodskaLuka.Instance.DajSveVezove()[index];
            bool sadrzanUMolu = BrodskaLuka.Instance.DajSveMolove().Any(mol => mol.DajSveVezove().Contains(vez));

            if (sadrzanUMolu) { continue; }

            BrodskaLuka.Instance.DajSveVezove().RemoveAt(index);

            model.IspisiGresku($"Vez {vez.Id} ne pripada ni jednom molu pa je uklonjen iz liste validnih vezova.",
                true);
        }
    }
}