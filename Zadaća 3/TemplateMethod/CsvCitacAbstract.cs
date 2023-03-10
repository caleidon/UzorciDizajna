#region

using avranic_zadaca_1.Utilities;

#endregion

namespace avranic_zadaca_1;

public abstract class CsvCitacAbstract<T> where T : class
{
    protected readonly Model Model;
    protected CsvCitacAbstract(Model model) { Model = model; }

    public List<T> Citaj(string path)
    {
        if (!File.Exists(path)) {
            if (this is not ZahtjevRezervacijeCsvCitac) {
                Model.IspisiGresku(
                    $"Nije moguće pronaći datoteku {path}. Provjerite da li je putanja ispravna. Ovo je esencijalna datoteka aplikacija, stoga će se aplikacija sada zatvoriti.",
                    true);
                Environment.Exit(0);
            } else {
                Model.IspisiGresku($"Nije moguće pronaći datoteku {path}. Provjerite da li je putanja ispravna.", true);
            }
        }

        using var reader = new StreamReader(path);
        var procitaneStavke = new List<T>();

        string? pomocnaPolja = reader.ReadLine();

        if (pomocnaPolja == null) {
            Model.IspisiGresku($"Prazna prva linija u datoteci {path}. Program će se zatvoriti.", true);
            Environment.Exit(0);
        }

        string[] stupacCsva = pomocnaPolja.Split(';');

        int linijaCsva = 1;

        while (!reader.EndOfStream) {
            int indexStupca = 0;
            linijaCsva++;

            try {
                string? line = reader.ReadLine();

                if (line == null) {
                    Model.IspisiGresku($"Prazna linija u datoteci {path} na liniji {linijaCsva}", true);
                    continue;
                }

                string[] values = line.Trim().Split(';');

                for (int i = 0; i < values.Length; i++) { values[i] = values[i].Trim(); }

                T podatak = ParsirajPodatke(values, procitaneStavke, out indexStupca);

                if (!procitaneStavke.Contains(podatak)) { procitaneStavke.Add(podatak); }
            } catch (Exception e) {
                string razlog = e switch {
                    ArgumentException            => e.Message,
                    FormatException              => "neispravan format broja",
                    IndexOutOfRangeException     => "vrijednost nije unesena",
                    OverflowException            => "neispravna vrijednost vremena",
                    StavkaNijePronadenaException => e.Message,
                    PogresniPodaciException      => e.Message,
                    _ => throw new Exception(
                        $"Neutvrđena greška kod ucitavanja datoteke {path} na liniji {linijaCsva} i stupcu {indexStupca}. Exception je bio: {e}")
                };

                Model.IspisiGresku(
                    $"Greška u datoteci {path}:{linijaCsva} u stupcu {stupacCsva[indexStupca]}. Razlog: {razlog}",
                    false);

                if (this is BrodskaLukaCsvCitac) {
                    Model.IspisiGresku(
                        "Desila se fatalna greška prilikom učitavanja brodske luke iz CSV datoteke. Kako je brodska luka osnovni element programa, program se ne može nastaviti.",
                        true);
                    Environment.Exit(0);
                }
            }
        }

        return procitaneStavke;
    }

    protected abstract T ParsirajPodatke(string[] values, IEnumerable<T> postojeciPodaci, out int indexStupca);
}