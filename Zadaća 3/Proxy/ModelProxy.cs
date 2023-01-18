#region

using avranic_zadaca_1.Sucelja;

#endregion

namespace avranic_zadaca_1;

public class ModelProxy : IModel
{
    private readonly Model _model;
    private View _view = null!;

    public ModelProxy(Model model)
    {
        _model = model;
        VirtualniSat.Instance.ZapocniVirtualniSat(BrodskaLuka.Instance.InicijalnoVirtualnoVrijeme);
        VirtualniSat.NaPromjenuVremena += NaPromjenuVremena;
    }

    public void PostaviView(View view) { _view = view; }

    private void NaPromjenuVremena() { _model.PromjeniVirtualnoVrijeme(VirtualniSat.Instance.VirtualnoVrijeme, false); }

    public void IspisiStatuseVezova()
    {
        _view.IspisiVirtualnoVrijeme();
        _model.IspisiStatuseVezova();
    }

    public void PromjeniVirtualnoVrijeme(DateTime novoVrijeme, bool pozvanoKrozNaredbu)
    {
        _view.IspisiVirtualnoVrijeme();
        _model.PromjeniVirtualnoVrijeme(novoVrijeme, true);
    }

    public void IspisiZauzetostiVezova(Vez.VrstaVeza vrsta, bool zauzeti, DateTime datumOd, DateTime datumDo)
    {
        _view.IspisiVirtualnoVrijeme();
        _model.IspisiZauzetostiVezova(vrsta, zauzeti, datumOd, datumDo);
    }

    public void UcitajZahtjeveRezervacije(string nazivDatoteke)
    {
        _view.IspisiVirtualnoVrijeme();
        _model.UcitajZahtjeveRezervacije(nazivDatoteke);
    }

    public void PosaljiPorukuZahjevaPrivezaZaRezerviraniVez(Brod brod)
    {
        _view.IspisiVirtualnoVrijeme();
        _model.PosaljiPorukuZahjevaPrivezaZaRezerviraniVez(brod);
    }

    public void PosaljiPorukuZahjevaPrivezaZaSlobodniVez(Brod brod, int trajanjeUSatima)
    {
        _view.IspisiVirtualnoVrijeme();
        _model.PosaljiPorukuZahjevaPrivezaZaSlobodniVez(brod, trajanjeUSatima);
    }

    public void PromjeniPostavkeTablicnogIspisa(bool prikaziZaglavlje, bool prikaziPodnozje, bool prikaziRedneBrojeve)
    {
        _view.IspisiVirtualnoVrijeme();
        _model.PromjeniPostavkeTablicnogIspisa(prikaziZaglavlje, prikaziPodnozje, prikaziRedneBrojeve);
    }

    public void IspisiZauzetostVezovaPremaVrstiNaDatum(DateTime vrijemeOd, DateTime vrijemeDo)
    {
        _view.IspisiVirtualnoVrijeme();
        _model.IspisiZauzetostVezovaPremaVrstiNaDatum(vrijemeOd, vrijemeDo);
    }

    public void SpojiSeNaKanal(Brod brod, int frekvencija, bool odjava)
    {
        _view.IspisiVirtualnoVrijeme();
        _model.SpojiSeNaKanal(brod, frekvencija, odjava);
    }

    public void IspisiDnevnik()
    {
        _view.IspisiVirtualnoVrijeme();
        _model.IspisiDnevnik();
    }

    public void OrganizirajHitniPrijevoz(int brojSluzbenika, int brojImigranta)
    {
        _view.IspisiVirtualnoVrijeme();
        _model.OrganizirajHitniPrijevoz(brojSluzbenika, brojImigranta);
    }

    public void SpremiPostojeceStanjeSvihVezova(string nazivDatoteke)
    {
        _view.IspisiVirtualnoVrijeme();
        _model.SpremiPostojeceStanjeSvihVezova(nazivDatoteke);
    }

    public void VratiPostojeceStanjeSvihVezova(string nazivDatoteke)
    {
        _view.IspisiVirtualnoVrijeme();
        _model.VratiPostojeceStanjeSvihVezova(nazivDatoteke);
    }

    public void IspisiGresku(string greska, bool odmahOsvjeziEkran) { _model.IspisiGresku(greska, odmahOsvjeziEkran); }
}