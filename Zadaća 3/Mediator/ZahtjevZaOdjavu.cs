namespace avranic_zadaca_1.Observer;

public class ZahtjevZaOdjavu : PorukaVhfKanala
{
    public ZahtjevZaOdjavu(ISubscriber posiljatelj) : base(posiljatelj) { }
}