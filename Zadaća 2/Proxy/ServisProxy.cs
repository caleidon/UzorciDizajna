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

    public void IspisiStatuseVezova()
    {
        Alati.IspisiVirtualnoVrijeme();
        _servis.IspisiStatuseVezova();
    }

    public void PromjeniVirtualnoVrijeme(DateTime novoVrijeme)
    {
        Alati.IspisiVirtualnoVrijeme();
        _servis.PromjeniVirtualnoVrijeme(novoVrijeme);
    }

    public void IspisiZauzetostiVezova(Vez.VrstaVeza vrsta, bool zauzeti, DateTime datumOd, DateTime datumDo)
    {
        Alati.IspisiVirtualnoVrijeme();
        _servis.IspisiZauzetostiVezova(vrsta, zauzeti, datumOd, datumDo);
    }

    public void UcitajZahtjeveRezervacije(string nazivDatoteke)
    {
        Alati.IspisiVirtualnoVrijeme();
        _servis.UcitajZahtjeveRezervacije(nazivDatoteke);
    }

    public void PosaljiPorukuZahjevaPrivezaZaRezerviraniVez(Brod brod)
    {
        Alati.IspisiVirtualnoVrijeme();
        _servis.PosaljiPorukuZahjevaPrivezaZaRezerviraniVez(brod);
    }

    public void PosaljiPorukuZahjevaPrivezaZaSlobodniVez(Brod brod, int trajanjeUSatima)
    {
        Alati.IspisiVirtualnoVrijeme();
        _servis.PosaljiPorukuZahjevaPrivezaZaSlobodniVez(brod, trajanjeUSatima);
    }

    public void PromjeniPostavkeTablicnogIspisa(bool prikaziZaglavlje, bool prikaziPodnozje, bool prikaziRedneBrojeve)
    {
        Alati.IspisiVirtualnoVrijeme();
        _servis.PromjeniPostavkeTablicnogIspisa(prikaziZaglavlje, prikaziPodnozje, prikaziRedneBrojeve);
    }

    public void IspisiZauzetostVezovaPremaVrstiNaDatum(DateTime vrijemeOd, DateTime vrijemeDo)
    {
        Alati.IspisiVirtualnoVrijeme();
        _servis.IspisiZauzetostVezovaPremaVrstiNaDatum(vrijemeOd, vrijemeDo);
    }

    public void SpojiSeNaKanal(Brod brod, int frekvencija, bool odjava)
    {
        Alati.IspisiVirtualnoVrijeme();
        _servis.SpojiSeNaKanal(brod, frekvencija, odjava);
    }

    public void IspisiDnevnik()
    {
        Alati.IspisiVirtualnoVrijeme();
        _servis.IspisiDnevnik();
    }

    public void OrganizirajHitniPrijevoz(int brojSluzbenika, int brojImigranta)
    {
        Alati.IspisiVirtualnoVrijeme();
        _servis.OrganizirajHitniPrijevoz(brojSluzbenika, brojImigranta);
    }
}