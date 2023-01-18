#region

using avranic_zadaca_1.Observer;

#endregion

namespace avranic_zadaca_1;

public class Kanal : IMediator
{
    public int Id { get; }
    public int Frekvencija { get; }
    private int MaksimalanBroj { get; }
    private readonly List<ISubscriber> _subscribers = new();

    public Kanal(int id, int frekvencija, int maksimalanBroj)
    {
        Id = id;
        Frekvencija = frekvencija;
        MaksimalanBroj = maksimalanBroj;
    }

    public bool IspunjeniZahtjeviZaKomunikaciju() { return _subscribers.Count < MaksimalanBroj; }

    public void Subscribe(ISubscriber subscriber)
    {
        if (_subscribers.Contains(subscriber)) { return; }

        _subscribers.Add(subscriber);
    }

    public void Unsubscribe(ISubscriber subscriber)
    {
        if (!_subscribers.Contains(subscriber)) { return; }

        _subscribers.Remove(subscriber);
    }

    public void PosaljiPoruku(PorukaVhfKanala poruka)
    {
        foreach (ISubscriber subscriber in
                 _subscribers.ToList().Where(subscriber => subscriber != poruka.Posiljatelj)) {
            subscriber.PrimiPoruku(poruka, this);
        }
    }
}