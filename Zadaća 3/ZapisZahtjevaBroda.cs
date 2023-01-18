#region

#endregion

namespace avranic_zadaca_1;

public class ZapisZahtjevaBroda
{
    public enum TipZahtjeva
    {
        RezerviraniVez,
        SlobodanVez,
        Odjava
    }

    public Brod Brod { get; }
    public bool Odobreno { get; }
    public DateTime OdobrenoOd { get; }
    public DateTime OdobrenoDo { get; }
    public TipZahtjeva Tip { get; }

    public ZapisZahtjevaBroda(Brod brod, bool odobreno, DateTime odobrenoOd, DateTime odobrenoDo, TipZahtjeva tip)
    {
        Brod = brod;
        Odobreno = odobreno;
        OdobrenoOd = odobrenoOd;
        OdobrenoDo = odobrenoDo;
        Tip = tip;
    }
}