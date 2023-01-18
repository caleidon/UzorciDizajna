#region

using avranic_zadaca_1.Observer;
using avranic_zadaca_1.Utilities;

#endregion

namespace avranic_zadaca_1;

public class KanalCsvCitac : CsvCitacAbstract<Kanal>
{
    protected override Kanal ParsirajPodatke(string[] values, IEnumerable<Kanal> postojeciPodaci, out int indexStupca)
    {
        indexStupca = -1;

        int id = int.Parse(values[++indexStupca]);

        IEnumerable<Kanal> listaPostojecihKanala = postojeciPodaci.ToList();

        if (listaPostojecihKanala.Any(k => k.Id == id)) {
            throw new PogresniPodaciException($"Kanal {id} već postoji.");
        }

        int frekvencija = int.Parse(values[++indexStupca]);

        if (listaPostojecihKanala.Any(k => k.Frekvencija == frekvencija)) {
            throw new PogresniPodaciException($"Kanal s frekvencijom {frekvencija} već postoji.");
        }

        int maksimalanBroj = int.Parse(values[++indexStupca]);

        if (maksimalanBroj <= 1) {
            throw new PogresniPodaciException(
                $"Maksimalan broj povezanih sudionika na kanalu {id} mora biti veći od 1.");
        }

        var kanal = new Kanal(id, frekvencija, maksimalanBroj);
        ((IMediator) kanal).Subscribe(BrodskaLuka.Instance);

        return kanal;
    }
}