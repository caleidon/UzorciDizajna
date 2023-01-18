namespace avranic_zadaca_1.Observer;

public class OdgovorLuckeKapetanije : PorukaVhfKanala
{
    public Brod BrodOdgovora { get; }
    public bool Odobreno { get; }

    public OdgovorLuckeKapetanije(ISubscriber posiljatelj, Brod brod, bool odobreno) : base(posiljatelj)
    {
        BrodOdgovora = brod;
        Odobreno = odobreno;
    }
}