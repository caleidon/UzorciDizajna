namespace avranic_zadaca_1;

public class MolVez
{
    public Mol Mol { get; }
    public List<Vez> Vezovi { get; }

    public MolVez(Mol mol, List<Vez> vezovi)
    {
        Mol = mol;
        Vezovi = vezovi;
    }
}