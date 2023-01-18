#region

using avranic_zadaca_1;

#endregion

public class MolIterator : IIterator<Mol>
{
    private readonly List<Mol> _molovi;
    private int _pozicija = -1;
    public MolIterator(List<Mol> molovi) { _molovi = molovi; }

    public Mol GetFirst() { return _molovi[0]; }

    public Mol GetNext() { return _molovi[++_pozicija]; }

    public bool IsDone() { return _pozicija >= _molovi.Count - 1; }
}