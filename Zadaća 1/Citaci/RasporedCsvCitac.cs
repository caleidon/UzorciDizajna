#region

using avranic_zadaca_1.Sucelja;
using avranic_zadaca_1.Utilities;

#endregion

namespace avranic_zadaca_1;

public class RasporedCsvCitac : CsvCitacAbstract<Raspored>
{
    protected override Raspored ParsirajPodatke(string[] values,
                                                IEnumerable<Raspored> postojeciPodaci,
                                                out int indexStupca)
    {
        indexStupca = -1;

        int idVeza = int.Parse(values[++indexStupca]);

        Vez vez = BrodskaLuka.Instance.SviVezovi.FirstOrDefault(v => v.Id == idVeza) ??
                  throw new StavkaNijePronadenaException("Vez s id-em " + idVeza + " ne postoji.");

        int idBroda = int.Parse(values[++indexStupca]);

        Brod brod = BrodskaLuka.Instance.SviBrodovi.FirstOrDefault(b => b.Id == idBroda) ??
                    throw new StavkaNijePronadenaException("Brod s id-em " + idBroda + " ne postoji.");

        if (!vez.PodrzavaBrod(brod)) {
            throw new PogresniPodaciException($"Vez sa ID {vez.Id} ne podržava brod sa ID {brod.Id}.");
        }

        string daniUTjednuString = values[++indexStupca];
        TimeSpan vrijemeOd = TimeSpan.Parse(values[++indexStupca]);
        TimeSpan vrijemeDo = TimeSpan.Parse(values[++indexStupca]);

        List<int> daniUTjednuBrojevi = daniUTjednuString.Split(',').Select(int.Parse).ToList();

        if (daniUTjednuBrojevi.Count != daniUTjednuBrojevi.Distinct().Count() || daniUTjednuBrojevi.Count > 7) {
            throw new ArgumentException("Dani u tjednu moraju biti jedinstveni i ne smije ih biti više od 7.");
        }

        IEnumerable<Raspored> postojeciPodaciLista = postojeciPodaci.ToList();

        if (postojeciPodaciLista.Any(r =>
                r.Vez.Id == idVeza &&
                r.Brod.Id == idBroda &&
                r.VrijemeOd == vrijemeOd &&
                r.VrijemeDo == vrijemeDo &&
                r.Dani != daniUTjednuBrojevi)) {
            throw new ArgumentException("Nije moguce imati isti vremenski period sa razlicitim danima u tjednu.");
        }

        if (postojeciPodaciLista.Any(r =>
                r.Vez.Id == idVeza &&
                r.Brod.Id == idBroda &&
                r.Dani == daniUTjednuBrojevi &&
                r.VrijemeOd == vrijemeOd &&
                r.VrijemeDo == vrijemeDo)) {
            throw new ArgumentException("Nije moguće imati više rasporeda u istom vremenskom intervalu.");
        }

        ZauzetostKreator zauzetostKreator = new RasporednaZauzetostKreator();

        vez.SveZauzetosti.Add(zauzetostKreator.KreirajZauzetost(
            new ParametriRasporedneZauzetosti(vez, brod, daniUTjednuBrojevi, vrijemeOd, vrijemeDo)));

        return new Raspored(vez, brod, daniUTjednuBrojevi, vrijemeOd, vrijemeDo);
    }
}