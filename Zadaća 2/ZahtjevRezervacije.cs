namespace avranic_zadaca_1;

public class ZahtjevRezervacije
{
    public Brod Brod { get; }
    public DateTime DatumVrijemeOd { get; }
    public int TrajanjePriveza { get; }

    public ZahtjevRezervacije(Brod brod, DateTime datumVrijemeOd, int trajanjePriveza)
    {
        Brod = brod;
        DatumVrijemeOd = datumVrijemeOd;
        TrajanjePriveza = trajanjePriveza;
    }
}