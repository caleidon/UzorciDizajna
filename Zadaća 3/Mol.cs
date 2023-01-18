// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace avranic_zadaca_1;

public class Mol : Composite, IVezIteratorCreator
{
    public int Id { get; }
    public string Naziv { get; }

    public Mol(int id, string naziv)
    {
        Id = id;
        Naziv = naziv;
    }

    public IIterator<Vez> CreateVezIterator()
    {
        List<Vez> vezovi = Djeca.Cast<Vez>().ToList();
        return new VezIterator(vezovi);
    }

    public List<Vez> DajSveVezove()
    {
        IIterator<Vez> vezIterator = CreateVezIterator();
        var vezovi = new List<Vez>();

        while (!vezIterator.IsDone()) { vezovi.Add(vezIterator.GetNext()); }

        return vezovi;
    }
}