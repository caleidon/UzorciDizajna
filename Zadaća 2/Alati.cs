namespace avranic_zadaca_1;

public static class Alati
{
    public static void IspisiGresku(string porukaGreske)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[Greška {Program.RedniBrojPogreske++}]: {porukaGreske}");
        Console.ResetColor();
    }

    public static void IspisiPotvrdu(string porukaPotvrde)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(porukaPotvrde);
        Console.ResetColor();
    }

    public static void IspisiVirtualnoVrijeme()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[{VirtualniSat.Instance.VirtualnoVrijeme}]");
        Console.ResetColor();
    }

    // public static void DebugLog(string poruka)
    // {
    //     Console.ForegroundColor = ConsoleColor.Yellow;
    //     Console.WriteLine($"DEBUG: {poruka}");
    //     Console.ResetColor();
    // }

    public static bool ProbajPronaciBrod(int idBroda, bool neprivezan, out Brod pronadeniBrod)
    {
        Brod? brod = BrodskaLuka.Instance.SviBrodovi.FirstOrDefault(b => b.Id == idBroda);

        if (brod != null) {
            pronadeniBrod = brod;

            if (!neprivezan) { return true; }

            if (!brod.JePrivezan) { return true; }

            IspisiGresku($"Brod {idBroda} je pronađen ali je već privezan na vez.");
            return false;
        }

        pronadeniBrod = null!;
        IspisiGresku($"Brod {idBroda} nije pronađen.");
        return false;
    }

    public static IEnumerable<Vez> SortirajPoEkonomicnosti(this IEnumerable<Vez> vezovi)
    {
        return vezovi.OrderBy(v => v.MaksimalnaDuljina)
                     .ThenBy(v => v.MaksimalnaSirina)
                     .ThenBy(v => v.MaksimalnaDubina)
                     .ThenBy(v => v.CijenaPoSatu);
    }

    public static void ProvjeriPripadaLiSvakiSvakiVezMolu()
    {
        for (int index = BrodskaLuka.Instance.SviVezovi.Count - 1; index >= 0; index--) {
            Vez vez = BrodskaLuka.Instance.SviVezovi[index];
            bool sadrzanUMolu = BrodskaLuka.Instance.SviMolovi.Any(mol => mol.Vezovi.Contains(vez));

            if (sadrzanUMolu) { continue; }

            BrodskaLuka.Instance.SviVezovi.RemoveAt(index);
            IspisiGresku($"Vez {vez.Id} ne pripada ni jednom molu pa je uklonjen iz liste validnih vezova.");
        }
    }
}