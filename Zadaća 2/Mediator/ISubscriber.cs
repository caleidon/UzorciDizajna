namespace avranic_zadaca_1.Observer;

public interface ISubscriber
{
    public void PrimiPoruku(PorukaVhfKanala poruka, IMediator kanal);
}