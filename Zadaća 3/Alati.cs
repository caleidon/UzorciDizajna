namespace avranic_zadaca_1;

public static class Alati
{
    public static void NapuniStringArray(this string?[] array, string? value)
    {
        for (int i = 0; i < array.Length; i++) { array[i] = value; }
    }

    public static IEnumerable<Vez> SortirajPoEkonomicnosti(this IEnumerable<Vez> vezovi)
    {
        return vezovi.OrderBy(v => v.MaksimalnaDuljina)
                     .ThenBy(v => v.MaksimalnaSirina)
                     .ThenBy(v => v.MaksimalnaDubina)
                     .ThenBy(v => v.CijenaPoSatu);
    }
}