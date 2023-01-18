#region

using avranic_zadaca_1.Utilities;

#endregion

namespace avranic_zadaca_1;

public class VezCsvCitac : CsvCitacAbstract<Vez>
{
    protected override Vez ParsirajPodatke(string[] values, IEnumerable<Vez> postojeciPodaci, out int indexStupca)
    {
        indexStupca = -1;

        int id = int.Parse(values[++indexStupca]);

        List<Vez> postojeciPodaciLista = postojeciPodaci.ToList();

        if (postojeciPodaciLista.Any(vez => vez.Id == id)) {
            throw new PogresniPodaciException("Vez " + id + " vec postoji");
        }

        string oznakaVeza = values[++indexStupca];
        var vrsta = (Vez.VrstaVeza) Enum.Parse(typeof(Vez.VrstaVeza), values[++indexStupca]);

        int ukupniBrodPutnickihVezova = postojeciPodaciLista.Count(v => v.Vrsta == Vez.VrstaVeza.PU);
        int ukupniBrojPoslovnihVezova = postojeciPodaciLista.Count(v => v.Vrsta == Vez.VrstaVeza.PO);
        int ukupniBrojOstalihVezova = postojeciPodaciLista.Count(v => v.Vrsta == Vez.VrstaVeza.OS);

        switch (vrsta) {
            case Vez.VrstaVeza.PU when ukupniBrodPutnickihVezova >= BrodskaLuka.Instance.UkupniBrojPutnickihVezova:
                throw new PogresniPodaciException(
                    $"Broj putnickih vezova je veci od maksimalnog broja vezova ({ukupniBrodPutnickihVezova}) " +
                    $"definiranog u brodskoj luci. Izostavljeni id veza: {id}");
            case Vez.VrstaVeza.PO when ukupniBrojPoslovnihVezova >= BrodskaLuka.Instance.UkupniBrojPoslovnihVezova:
                throw new PogresniPodaciException(
                    $"Broj poslovnih vezova je veci od maksimalnog broja vezova ({ukupniBrojPoslovnihVezova}) " +
                    $"definiranog u brodskoj luci. Izostavljeni id veza: {id}");
            case Vez.VrstaVeza.OS when ukupniBrojOstalihVezova >= BrodskaLuka.Instance.UkupniBrojOstalihVezova:
                throw new PogresniPodaciException(
                    $"Broj ostalih vezova je veci od maksimalnog broja vezova ({ukupniBrojOstalihVezova}) " +
                    $"definiranog u brodskoj luci. Izostavljeni id veza: {id}");
        }

        int cijenaVezaPoSatu = int.Parse(values[++indexStupca]);
        int maksimalnaDuljina = int.Parse(values[++indexStupca]);
        int maksimalnaSirina = int.Parse(values[++indexStupca]);
        int maksimalnaDubina = int.Parse(values[++indexStupca]);

        if (maksimalnaDubina > BrodskaLuka.Instance.DubinaLuke) {
            throw new PogresniPodaciException(
                $"Vez {id} ima veću dubinu od luke ({maksimalnaDubina} > {BrodskaLuka.Instance.DubinaLuke})");
        }

        return new Vez(id, oznakaVeza, vrsta, cijenaVezaPoSatu, maksimalnaDuljina, maksimalnaSirina, maksimalnaDubina);
    }
}