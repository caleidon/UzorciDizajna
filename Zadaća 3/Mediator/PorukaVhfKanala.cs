namespace avranic_zadaca_1.Observer;

public abstract class PorukaVhfKanala
{
    public ISubscriber Posiljatelj { get; }
    protected PorukaVhfKanala(ISubscriber posiljatelj) { Posiljatelj = posiljatelj; }
}