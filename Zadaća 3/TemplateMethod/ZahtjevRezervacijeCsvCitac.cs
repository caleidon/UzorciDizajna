#region

using System.Globalization;
using avranic_zadaca_1.Utilities;

#endregion

namespace avranic_zadaca_1;

public class ZahtjevRezervacijeCsvCitac : CsvCitacAbstract<ZahtjevRezervacije>
{
    public ZahtjevRezervacijeCsvCitac(Model model) : base(model) { }

    protected override ZahtjevRezervacije ParsirajPodatke(string[] values,
                                                          IEnumerable<ZahtjevRezervacije> postojeciPodaci,
                                                          out int indexStupca)
    {
        indexStupca = -1;

        int idBroda = int.Parse(values[++indexStupca]);

        Brod brod = BrodskaLuka.Instance.SviBrodovi.FirstOrDefault(b => b.Id == idBroda) ??
                    throw new StavkaNijePronadenaException($"Brod {idBroda} ne postoji.");

        DateTime datumVrijemeOd = DateTime.Parse(values[++indexStupca], CultureInfo.CreateSpecificCulture("hr-HR"));
        int trajanjePriveza = int.Parse(values[++indexStupca]);

        return new ZahtjevRezervacije(brod, datumVrijemeOd, trajanjePriveza);
    }
}