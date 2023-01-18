namespace avranic_zadaca_1;

public class IspisivacTablice
{
    private static IspisivacTablice? _instance;

    public static IspisivacTablice Instance => _instance ??= new IspisivacTablice();

    public bool IspisZaglavlja { get; set; } = true;
    public bool IspisPodnozja { get; set; } = true;
    public bool IspisRednihBrojeva { get; set; } = true;

    public void IspisiTablicu(List<string> zaglavlje, List<List<object>> podaci)
    {
        if (IspisZaglavlja) { IspisiZaglavlje(zaglavlje, podaci); }

        IspisiPodatke(zaglavlje, podaci);

        if (IspisPodnozja) { IspisiPodnozje(podaci.Count); }
    }

    private void IspisiPodatke(List<string> zaglavlje, List<List<object>> podaci)
    {
        int redniBroj = 1;

        foreach (List<object> redovi in podaci) {
            if (IspisRednihBrojeva) { Console.Write($"|{redniBroj++ + ". ",-4}"); }

            for (int index = 0; index < redovi.Count; index++) {
                object red = redovi[index];
                int sirina = zaglavlje[index].Length + 2;

                if (podaci.Any()) {
                    int tempSirina = podaci.Max(p => p[index].ToString()!.Length) + 2;

                    if (tempSirina > sirina) { sirina = tempSirina; }
                }

                switch (red) {
                    case string s:
                        Console.Write($"|{s.PadRight(sirina)}");
                        break;
                    case int i:
                        Console.Write($"|{i.ToString().PadLeft(sirina)}");
                        break;
                }
            }

            Console.WriteLine("|");
        }
    }

    private void IspisiZaglavlje(List<string> zaglavlje, List<List<object>> podaci)
    {
        if (IspisRednihBrojeva) { Console.Write("|Rbr "); }

        for (int index = 0; index < zaglavlje.Count; index++) {
            string red = zaglavlje[index];

            int sirina = red.Length + 2;

            if (podaci.Any()) {
                int tempSirina = podaci.Max(p => p[index].ToString()!.Length) + 2;

                if (tempSirina > sirina) { sirina = tempSirina; }
            }

            Console.Write($"|{red.PadRight(sirina)}");
        }

        Console.WriteLine("|");
    }

    private void IspisiPodnozje(int ukupniBrojRedaka) { Console.WriteLine($"|Ukupno: {ukupniBrojRedaka} retka  |"); }
}