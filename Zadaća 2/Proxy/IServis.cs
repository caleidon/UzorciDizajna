namespace avranic_zadaca_1.Sucelja;

public interface IServis
{
    void IspisiStatuseVezova();
    void PromjeniVirtualnoVrijeme(DateTime novoVrijeme);
    void IspisiZauzetostiVezova(Vez.VrstaVeza vrsta, bool zauzeti, DateTime datumOd, DateTime datumDo);
    void UcitajZahtjeveRezervacije(string nazivDatoteke);
    void PosaljiPorukuZahjevaPrivezaZaRezerviraniVez(Brod brod);
    void PosaljiPorukuZahjevaPrivezaZaSlobodniVez(Brod brod, int trajanjeUSatima);
    void PromjeniPostavkeTablicnogIspisa(bool prikaziZaglavlje, bool prikaziPodnozje, bool prikaziRedneBrojeve);
    void IspisiZauzetostVezovaPremaVrstiNaDatum(DateTime vrijemeOd, DateTime vrijemeDo);
    void SpojiSeNaKanal(Brod brod, int frekvencija, bool odjava);
    void IspisiDnevnik();
    void OrganizirajHitniPrijevoz(int brojSluzbenika, int brojImigranta);
}