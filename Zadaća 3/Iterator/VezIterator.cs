#region

using avranic_zadaca_1;

#endregion

public class VezIterator : IIterator<Vez>
{
    private readonly List<Vez> _vezovi;
    private int _pozicija = -1;
    public VezIterator(List<Vez> vezovi) { _vezovi = vezovi; }

    public Vez GetFirst() { return _vezovi[0]; }

    public Vez GetNext() { return _vezovi[++_pozicija]; }

    public bool IsDone() { return _pozicija >= _vezovi.Count - 1; }
}