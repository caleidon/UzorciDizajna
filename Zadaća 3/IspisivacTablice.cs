namespace avranic_zadaca_1;

public class IspisivacTablice
{
    private static IspisivacTablice? _instance;

    public static IspisivacTablice Instance => _instance ??= new IspisivacTablice();

    public bool IspisZaglavlja { get; set; } = true;
    public bool IspisPodnozja { get; set; } = true;
    public bool IspisRednihBrojeva { get; set; } = true;

    public List<string> StvoriTablicu(List<string> zaglavlje, List<List<object>> podaci)
    {
        var tablica = new List<string>();

        if (IspisZaglavlja) { DodajZaglavlje(zaglavlje, podaci, tablica); }

        DodajPodatke(zaglavlje, podaci, tablica);

        if (IspisPodnozja) { DodajPodnozje(podaci.Count, tablica); }

        return tablica;
    }

    private void DodajPodatke(List<string> zaglavlje, List<List<object>> podaci, List<string> tablica)
    {
        int redniBroj = 1;

        foreach (List<object> redovi in podaci) {
            string linijaTablice = "";

            if (IspisRednihBrojeva) { linijaTablice += $"|{redniBroj++ + ". ",-4}"; }

            for (int index = 0; index < redovi.Count; index++) {
                object red = redovi[index];
                int sirina = zaglavlje[index].Length + 2;

                if (podaci.Any()) {
                    int tempSirina = podaci.Max(p => p[index].ToString()!.Length) + 2;

                    if (tempSirina > sirina) { sirina = tempSirina; }
                }

                switch (red) {
                    case string s:
                        linijaTablice += $"|{s.PadRight(sirina)}";
                        break;
                    case int i:
                        linijaTablice += $"|{i.ToString().PadLeft(sirina)}";
                        break;
                }
            }

            linijaTablice += "|";
            tablica.Add(linijaTablice);
        }
    }

    private void DodajZaglavlje(List<string> zaglavlje, List<List<object>> podaci, List<string> tablica)
    {
        string linijaTablice = "";

        if (IspisRednihBrojeva) { linijaTablice += "|Rbr "; }

        for (int index = 0; index < zaglavlje.Count; index++) {
            string red = zaglavlje[index];

            int sirina = red.Length + 2;

            if (podaci.Any()) {
                int tempSirina = podaci.Max(p => p[index].ToString()!.Length) + 2;

                if (tempSirina > sirina) { sirina = tempSirina; }
            }

            linijaTablice += $"|{red.PadRight(sirina)}";
        }

        linijaTablice += "|";
        tablica.Add(linijaTablice);
    }

    private void DodajPodnozje(int ukupniBrojRedaka, List<string> tablica)
    {
        tablica.Add($"|Ukupno: {ukupniBrojRedaka} retka  |");
    }
}