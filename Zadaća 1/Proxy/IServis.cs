namespace avranic_zadaca_1.Sucelja;

public interface IServis
{
    void IspisiStatuseVezova();
    void PromjeniVirtualnoVrijeme(DateTime novoVrijeme);
    void IspisiZauzetostiVezova(Vez.VrstaVeza vrsta, bool zauzeti, DateTime datumOd, DateTime datumDo);
    void UcitajZahtjeveRezervacije(string nazivDatoteke);
    void PriveziZaRezerviraniVez(Brod brod);
    void PriveziZaSlobodniVez(Brod brod, int trajanjeUSatima);
}