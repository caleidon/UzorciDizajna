#region

using avranic_zadaca_1.Sucelja;

#endregion

namespace avranic_zadaca_1;

public class ServisProxy : IServis
{
    private readonly Servis _servis;

    public ServisProxy(Servis servis)
    {
        _servis = servis;
        VirtualniSat.Instance.ZapocniVirtualniSat(BrodskaLuka.Instance.InicijalnoVirtualnoVrijeme);
        VirtualniSat.NaPromjenuVremena += NaPromjenuVremena;
    }

    private void NaPromjenuVremena() { _servis.PromjeniVirtualnoVrijeme(VirtualniSat.Instance.VirtualnoVrijeme); }

    private static void IspisiVirtualnoVrijeme()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[{VirtualniSat.Instance.VirtualnoVrijeme}]");
        Console.ResetColor();
    }

    public void IspisiStatuseVezova()
    {
        IspisiVirtualnoVrijeme();
        _servis.IspisiStatuseVezova();
    }

    public void PromjeniVirtualnoVrijeme(DateTime novoVrijeme)
    {
        IspisiVirtualnoVrijeme();
        _servis.PromjeniVirtualnoVrijeme(novoVrijeme);
        Console.WriteLine($"Virtualno vrijeme je postavljeno na {novoVrijeme}");
    }

    public void IspisiZauzetostiVezova(Vez.VrstaVeza vrsta, bool zauzeti, DateTime datumOd, DateTime datumDo)
    {
        IspisiVirtualnoVrijeme();
        _servis.IspisiZauzetostiVezova(vrsta, zauzeti, datumOd, datumDo);
    }

    public void UcitajZahtjeveRezervacije(string nazivDatoteke)
    {
        IspisiVirtualnoVrijeme();
        _servis.UcitajZahtjeveRezervacije(nazivDatoteke);
    }

    public void PriveziZaRezerviraniVez(Brod brod)
    {
        IspisiVirtualnoVrijeme();
        _servis.PriveziZaRezerviraniVez(brod);
    }

    public void PriveziZaSlobodniVez(Brod brod, int trajanjeUSatima)
    {
        IspisiVirtualnoVrijeme();
        _servis.PriveziZaSlobodniVez(brod, trajanjeUSatima);
    }
}