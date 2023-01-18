namespace avranic_zadaca_1;

public class Mol
{
    public int Id { get; }
    public string Naziv { get; }
    public List<Vez> Vezovi { get; } = new();

    public Mol(int id, string naziv)
    {
        Id = id;
        Naziv = naziv;
    }
}