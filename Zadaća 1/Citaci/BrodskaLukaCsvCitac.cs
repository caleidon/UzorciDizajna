namespace avranic_zadaca_1;

public class BrodskaLukaCsvCitac : CsvCitacAbstract<BrodskaLuka>
{
    protected override BrodskaLuka ParsirajPodatke(string[] values,
                                                   IEnumerable<BrodskaLuka> postojeciPodaci,
                                                   out int indexStupca)
    {
        indexStupca = -1;

        var brodskaLuka = BrodskaLuka.Instance;
        brodskaLuka.Naziv = values[++indexStupca];
        brodskaLuka.GpsSirina = values[++indexStupca];
        brodskaLuka.GpsVisina = values[++indexStupca];
        brodskaLuka.DubinaLuke = int.Parse(values[++indexStupca]);
        brodskaLuka.UkupniBrojPutnickihVezova = int.Parse(values[++indexStupca]);
        brodskaLuka.UkupniBrojPoslovnihVezova = int.Parse(values[++indexStupca]);
        brodskaLuka.UkupniBrojOstalihVezova = int.Parse(values[++indexStupca]);
        brodskaLuka.InicijalnoVirtualnoVrijeme = DateTime.Parse(values[++indexStupca]);

        return brodskaLuka;
    }
}