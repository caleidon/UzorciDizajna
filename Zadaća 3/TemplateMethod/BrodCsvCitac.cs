#region

using avranic_zadaca_1.Utilities;

#endregion

namespace avranic_zadaca_1;

public class BrodCsvCitac : CsvCitacAbstract<Brod>
{
    public BrodCsvCitac(Model model) : base(model) { }

    protected override Brod ParsirajPodatke(string[] values, IEnumerable<Brod> postojeciPodaci, out int indexStupca)
    {
        indexStupca = -1;

        int id = int.Parse(values[++indexStupca]);

        if (postojeciPodaci.Any(brod => brod.Id == id)) { throw new PogresniPodaciException($"Brod {id} vec postoji"); }

        string oznakaBroda = values[++indexStupca];
        string naziv = values[++indexStupca];
        var vrsta = (Brod.VrstaBroda) Enum.Parse(typeof(Brod.VrstaBroda), values[++indexStupca]);
        float duljina = float.Parse(values[++indexStupca]);
        float sirina = float.Parse(values[++indexStupca]);
        float gaz = float.Parse(values[++indexStupca]);
        float maksimalnaBrzina = float.Parse(values[++indexStupca]);
        int kapacitetPutnika = int.Parse(values[++indexStupca]);
        int kapacitetTereta = int.Parse(values[++indexStupca]);
        int kapacitetOsobnihVozila = int.Parse(values[++indexStupca]);

        if (kapacitetPutnika == 0 && kapacitetTereta == 0 && kapacitetOsobnihVozila == 0) {
            throw new PogresniPodaciException("Barem jedan od kapaciteta broda mora biti različit od nule.");
        }

        return new Brod(id, oznakaBroda, naziv, vrsta, duljina, sirina, gaz, maksimalnaBrzina, kapacitetPutnika,
            kapacitetTereta, kapacitetOsobnihVozila, Model);
    }
}