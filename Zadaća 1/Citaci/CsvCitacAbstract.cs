#region

using avranic_zadaca_1.Utilities;

#endregion

namespace avranic_zadaca_1;

public abstract class CsvCitacAbstract<T> where T : class
{
    public List<T> Citaj(string path)
    {
        using var reader = new StreamReader(path);
        var procitaneStavke = new List<T>();

        // preskoci prvu informativnu liniju
        string? pomocnaPolja = reader.ReadLine();

        if (pomocnaPolja == null) { throw new Exception($"Prazna prva linija u datoteci {path} "); }

        string[] stupacCsva = pomocnaPolja.Split(';');

        int linijaCsva = 1;

        while (!reader.EndOfStream) {
            int indexStupca = 0;
            linijaCsva++;

            try {
                string? line = reader.ReadLine();

                if (line == null) {
                    Program.IspisiGresku($"Prazna linija u datoteci {path} na liniji {linijaCsva}");
                    continue;
                }

                string[] values = line.Split(';');

                T podatak = ParsirajPodatke(values, procitaneStavke, out indexStupca);
                procitaneStavke.Add(podatak);
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

                Program.IspisiGresku(
                    $"Greška u datoteci {path}:{linijaCsva} u stupcu {stupacCsva[indexStupca]}. Razlog: {razlog}");
            }
        }

        return procitaneStavke;
    }

    protected abstract T ParsirajPodatke(string[] values, IEnumerable<T> postojeciPodaci, out int indexStupca);
}