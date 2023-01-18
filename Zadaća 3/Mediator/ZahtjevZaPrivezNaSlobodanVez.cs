namespace avranic_zadaca_1.Observer;

public class ZahtjevZaPrivezNaSlobodanVez : PorukaVhfKanala
{
    public int TrajanjeUSatima { get; }

    public ZahtjevZaPrivezNaSlobodanVez(ISubscriber posiljatelj, int trajanjeUSatima) : base(posiljatelj)
    {
        TrajanjeUSatima = trajanjeUSatima;
    }
}