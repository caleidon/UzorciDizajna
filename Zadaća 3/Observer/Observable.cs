#region

using avranic_zadaca_1;

#endregion

public class Observable
{
    private readonly List<IObserver> _observers = new();

    public void AddObserver(IObserver observer)
    {
        if (_observers.Contains(observer)) { return; }

        _observers.Add(observer);
    }

    public void RemoveObserver(IObserver observer)
    {
        if (!_observers.Contains(observer)) { return; }

        _observers.Remove(observer);
    }

    protected void NotifyObservers(Model.IzvrsenaNaredba izvrsenaNaredba, bool odmahOsvjeziEkran)
    {
        foreach (IObserver observer in _observers) { observer.Update(izvrsenaNaredba, odmahOsvjeziEkran); }
    }
}