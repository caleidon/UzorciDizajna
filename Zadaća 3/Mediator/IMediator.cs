namespace avranic_zadaca_1.Observer;

public interface IMediator
{
    public void Subscribe(ISubscriber subscriber);
    public void Unsubscribe(ISubscriber subscriber);
    public void PosaljiPoruku(PorukaVhfKanala poruka);
}